// MIT License

// Copyright (c) 2024 Mike Sørensen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using AssetSnap.Explorer;
using AssetSnap.Front.Components.Library;
using AssetSnap.Front.Nodes;
using Godot;

namespace AssetSnap.Library
{
	/// <summary>
	/// Represents an instance of a library within the AssetSnap application.
	/// </summary>
	[Tool]
	public partial class Instance : VBoxContainer
	{
		[Signal]
		public delegate void ItemCountUpdatedEventHandler( int count );
		
		public int Index;
		
		[Export]
		public string _Name;

		[Export]
		public string _Folder;
		
		[Export]
		public string _FileName;
		
		[Export]
		public int ItemCount 
		{
			get => _ItemCount;
			set 
			{
				_ItemCount = value;
				EmitSignal(SignalName.ItemCountUpdated, value);
			}
		}
		
		public int _ItemCount = 0;
		
		private GlobalExplorer _GlobalExplorer;
		private Godot.Collections.Array<AsLibraryPanelContainer> Panels = new();
		private PanelContainer _PanelContainer;
		
		private readonly List<string> BodyComponents = new()
		{
			"Library.Body",
		};
		
		private readonly List<string> TopbarComponents = new()
		{
			"Library.Topbar",
		};
		
		private readonly List<string> SettingsComponents = new()
		{
			"Library.Body",
			"Library.Settings",
		};
		
		private readonly List<string> ListingComponents = new()
		{
			"Library.Body",
			"Library.Listing",
		};
		
		/// <summary>
		/// The folder path of the library instance.
		/// </summary>
		public string Folder 
		{
			get => _Folder;
			set 
			{
				_Folder = value;
				_FileName = AsFileName();
			}
		}

		public bool Initialized = false;
		
		public PanelContainer PanelContainer 
		{
			get => _PanelContainer;
			set 
			{
				_PanelContainer = value;
			}
		}
		
		public string FileName
		{
			get => _FileName;
		}

		/** Components **/
		public Body _LibraryBody;
		public Topbar _LibraryTopbar;
		public Front.Components.Library.Settings _LibrarySettings;
		public Listing _LibraryListing;

		public TabContainer Dock;
		
		public Instance()
		{
			SizeFlagsVertical = SizeFlags.ExpandFill;
			SizeFlagsHorizontal = SizeFlags.ExpandFill;
			
			// if( null != _Name ) 
			// {
			// 	Name = _Name;
			// }
			// else 
			// {
			// 	Name = "Invalid";
			// }
		}

		/// <summary>
		/// Clears all components of the library instance.
		/// </summary>
		/// <param name="debug">Whether to output debug information.</param>
		public void Clear(bool debug = false)
		{
			_GlobalExplorer.Components.Remove(_LibraryTopbar);
			_GlobalExplorer.Components.Remove(_LibrarySettings);
			_GlobalExplorer.Components.Remove(_LibraryListing);
			_GlobalExplorer.Components.Remove(_LibraryBody);
			
			_LibraryTopbar.Clear(debug);
			_LibrarySettings.Clear(debug);
			_LibraryListing.Clear(debug);
			_LibraryBody.Clear(debug);
		}
		
		/// <summary>
		/// Initializes the library instance.
		/// </summary>
		public void Initialize()
		{
			Component.Base Components = ExplorerUtils.Get().Components;
			
			if( Initialized ) 
			{
				// Clear the instances first
				// Components.Clear<Topbar>();
				// Components.Clear<Settings>();
				// Components.Clear<Listing>();
				// Components.Clear<Body>();
				Clear();
				
				_LibraryBody = null;
				_LibraryTopbar = null;
				_LibraryListing = null;
				_LibrarySettings = null;
				
				_PanelContainer.GetParent().RemoveChild(_PanelContainer);
				_PanelContainer.QueueFree();
				
				// To ensure we reset it's position
				Dock.RemoveChild(this);
				// Dock.AddChild(this);
			}
			else 
			{
				Dock.AddChild(this);
			}

			Initialized = true;
			
			_GlobalExplorer = ExplorerUtils.Get();
			
			_PanelContainer = new()
			{
				SizeFlagsVertical = SizeFlags.ExpandFill,
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				Name = _FileName.Capitalize(),
			}; 
			
			_SetupLibraryBody();
			if( HasLibraryBody() ) 
			{
				_SetupLibraryTopbar();
				_SetupLibrarySettings(); 
				_SetupLibraryListing(); 
			}
			else 
			{
				GD.PushWarning("Library body failed to build"); 
			}

			AddChild(_PanelContainer);
			_PanelContainer.SetMeta("FolderPath", _Folder); 

			_GlobalExplorer._Plugin.Connect(Plugin.SignalName.LibraryChanged, Callable.From( ( string name ) => { _OnLibraryChanged( name ); }));
			Plugin.Singleton.EmitSignal(Plugin.SignalName.OnLibraryPopulized);
		}

		/// <summary>
		/// Fetches the current panels.
		/// </summary>
		/// <returns>The array of panel containers.</returns>
		public Godot.Collections.Array<AsLibraryPanelContainer> GetPanels()
		{
			return Panels;
		}
		
		/// <summary>
		/// Sets the current panels.
		/// </summary>
		/// <param name="_Panels">The array of panel containers to set.</param>
		public void SetPanels( Godot.Collections.Array<AsLibraryPanelContainer> _Panels )
		{
			Panels = _Panels;
		}
		
		/// <summary>
		/// Adds a panel container to the list of panels.
		/// </summary>
		/// <param name="_PanelContainer">The panel container to add.</param>
		/// <returns>void</returns>
		public void AddPanel(AsLibraryPanelContainer _PanelContainer)
		{
			Panels.Add(_PanelContainer);
		} 
		
		/// <summary>
		/// Clears the active state of all panels except for the one specified.
		/// </summary>
		/// <param name="panel">The panel container to keep active.</param>
		/// <returns>void</returns>
		public void ClearActivePanelState( AsLibraryPanelContainer panel )
		{
			foreach( AsLibraryPanelContainer _panel in Panels ) 
			{
				if( IsInstanceValid( _panel ) ) 
				{
					_panel.SetState(false);			
				}
			}
		}
		
		/// <summary>
		/// Clears the active state of all panels.
		/// </summary>
		/// <returns>void</returns>
		public void ClearAllPanelState()
		{
			for( int i = 0; i < Panels.Count; i++) 
			{
				AsLibraryPanelContainer _panel = Panels[i];
				_panel.SetState(false);			
			}	
		}
		
		/// <summary>
		/// Removes all panel states and frees the panel containers.
		/// </summary>
		/// <returns>void</returns>
		public void RemoveAllPanelState()
		{
			foreach( AsLibraryPanelContainer _panel in Panels ) 
			{
				if ( IsInstanceValid ( _panel ) ) 
				{
					if( IsInstanceValid( _panel.GetParent() ) ) 
					{
						_panel.GetParent().RemoveChild(_panel);
					}

					_panel.QueueFree();	
				}
			}

			Panels = new();
		}
		 
		/// <summary>
		/// Setups the library body if the component exists.
		/// </summary>
		/// <remarks>
		/// This method initializes the library body component if it exists, sets up its properties, and adds it to the panel container.
		/// </remarks>
		/// <returns>void</returns>
		private void _SetupLibraryBody()
		{
			Component.Base Components = _GlobalExplorer.Components;
			if ( Components.HasAll( BodyComponents.ToArray() )) 
			{
				_LibraryBody = Components.Single<Body>(true);
				
				if( HasLibraryBody() ) 
				{
					_LibraryBody.LibraryName = GetName();
					_LibraryBody.Initialize();
					PanelContainer.AddChild(_LibraryBody);
				}
			}
		}
		
		/// <summary>
		/// Setups the library title if the component exists.
		/// </summary>
		/// <remarks>
		/// This method initializes the library title component if it exists, sets up its properties, and adds it to the library body's right inner container.
		/// </remarks>
		/// <returns>void</returns>
		private void _SetupLibraryTopbar()
		{
			Component.Base Components = _GlobalExplorer.Components;
			if ( Components.HasAll( TopbarComponents.ToArray() )) 
			{
				_LibraryTopbar = Components.Single<Topbar>(true);
				
				if( HasLibraryTopbar() ) 
				{
					_LibraryTopbar.LibraryName = GetName();
					_LibraryTopbar.Initialize();
					_LibraryBody.GetRightInnerContainer().AddChild(_LibraryTopbar);
				}
			}
		}
		
		/// <summary>
		/// Setups the library settings if the component exists.
		/// </summary>
		/// <remarks>
		/// This method initializes the library settings component if it exists, sets up its properties, and adds it to the library body's left inner container.
		/// </remarks>
		/// <returns>void</returns>
		private void _SetupLibrarySettings()
		{
			Component.Base Components = _GlobalExplorer.Components;
			if (Components.HasAll( SettingsComponents.ToArray() )) 
			{
				_LibrarySettings = Components.Single<Front.Components.Library.Settings>(true);
				
				if( HasLibrarySettings() ) 
				{
					_LibrarySettings.LibraryName = GetName();
					_LibrarySettings.Initialize();
					_LibraryBody.GetLeftInnerContainer().AddChild(_LibrarySettings);
				}
			}
		}
		
		/// <summary>
		/// Setups the library listing if the component exists.
		/// </summary>
		/// <remarks>
		/// This method initializes the library listing component if it exists, sets up its properties, and adds it to the library body's right inner container.
		/// </remarks>
		/// <returns>void</returns>
		private void _SetupLibraryListing()
		{
			Component.Base Components = _GlobalExplorer.Components;
			if (Components.HasAll( ListingComponents.ToArray() )) 
			{
				_LibraryListing = Components.Single<Listing>(true);
				
				if( HasLibraryListing() ) 
				{
					_LibraryListing.Folder = Folder;
					_LibraryListing.LibraryName = GetName();
					_LibraryListing.Initialize();
					_LibraryBody.GetRightInnerContainer().AddChild(_LibraryListing);
				}
			}
		}
		
		/// <summary>
		/// Gets the name of the library.
		/// </summary>
		/// <remarks>
		/// This method retrieves the name of the library from its folder path.
		/// </remarks>
		/// <returns>The name of the library.</returns>
		public string GetName()
		{
			var split = Folder.Split('/');
			return split[ split.Length - 1];
		}
		
		/// <summary>
		/// Checks if the library title exists.
		/// </summary>
		/// <returns>True if the library title exists, false otherwise.</returns>
		private bool HasLibraryTopbar()
		{
			return null != _LibraryTopbar;
		}
						
		/// <summary>
		/// Checks if the library listing exists.
		/// </summary>
		/// <returns>True if the library listing exists, false otherwise.</returns>
		private bool HasLibraryListing()
		{
			return null != _LibraryListing;
		}
						
		/// <summary>
		/// Checks if the library settings exists.
		/// </summary>
		/// <returns>True if the library settings exists, false otherwise.</returns>
		private bool HasLibrarySettings()
		{
			return null != _LibrarySettings;
		}
						
		/// <summary>
		/// Checks if the library body exists.
		/// </summary>
		/// <returns>True if the library body exists, false otherwise.</returns>
		private bool HasLibraryBody()
		{
			return null != _LibraryBody;
		}
		
		/// <summary>
		/// Returns the last entry name for the folder.
		/// </summary>
		/// <remarks>
		/// This method extracts the last entry name from the folder path and formats it for use as a name, removing underscores.
		/// </remarks>
		/// <returns>The formatted file name.</returns>
		private string AsFileName() 
		{
			var OnlyFileName = Folder;
			var OnlyFileNameArray = OnlyFileName.Split('/');
			OnlyFileName = OnlyFileNameArray[OnlyFileNameArray.Length - 1];

			OnlyFileNameArray = OnlyFileName.Split(".");
			OnlyFileName = OnlyFileNameArray[0]; 
			
			OnlyFileNameArray = OnlyFileName.Split("_");
			OnlyFileName = OnlyFileNameArray.Join(" ");

			return OnlyFileName; 
		} 
		
		/// <summary>
		/// Handles the library change event.
		/// </summary>
		/// <param name="name">The name of the library that changed.</param>
		private void _OnLibraryChanged( string name )
		{
			if( name == GetName() && null != _LibrarySettings ) 
			{
				// This is the library that was changed to.
				// Update the values in the global states based on this library.
				_LibrarySettings.Sync();
			}
		}
		
		/// <summary>
		/// Resets the instance by clearing panel states and library settings.
		/// </summary>
		public void Reset()
		{
			ClearAllPanelState();
			_LibrarySettings.ClearAll();
		}

		/// <summary>
        /// Called when the node is about to be removed from the scene tree.
        /// </summary>
		public override void _ExitTree()
		{
			if( null != _PanelContainer && IsInstanceValid(_PanelContainer) ) 
			{
				_PanelContainer.QueueFree();
			}

			base._ExitTree();
		}
	}
}