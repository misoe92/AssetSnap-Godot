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

namespace AssetSnap.Library
{
	using System.Collections.Generic;
	using AssetSnap.Explorer;
	using AssetSnap.Front.Components.Library;
	using Godot;
	
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
		public Settings _LibrarySettings;
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
		
		/*
		** Initializes the library
		** Setting up it's nodes and UI and more
		** 
		** @return void
		*/
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
				// GD.Print("Removed tab entry");
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

		/*
		** Fetches the current panels
		**
		** @return Godot.Collections.Array<AsLibraryPanelContainer>
		*/
		public Godot.Collections.Array<AsLibraryPanelContainer> GetPanels()
		{
			return Panels;
		}
		
		/*
		** Sets the current panels
		**
		** @param Godot.Collections.Array<AsLibraryPanelContainer> _Panels
		** @return void
		*/
		public void SetPanels( Godot.Collections.Array<AsLibraryPanelContainer> _Panels )
		{
			Panels = _Panels;
		}
		
		/*
		** Adds a panel container to the panels
		**
		** @param AsLibraryPanelContainer _PanelContainer
		** @return void
		*/
		public void AddPanel(AsLibraryPanelContainer _PanelContainer)
		{
			Panels.Add(_PanelContainer);
		} 
		
		/*
		** Clears all panel states except for the one matching
		** the one given in params
		**
		** @param AsLibraryPanelContainer panel
		** @return void
		*/
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
		
		/*
		** Clears all panel states
		**
		** @return void
		*/
		public void ClearAllPanelState()
		{
			for( int i = 0; i < Panels.Count; i++) 
			{
				AsLibraryPanelContainer _panel = Panels[i];
				_panel.SetState(false);			
			}	
		}
		
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
		 
		/*
		** Setups the library body if the component exists
		** 
		** @return void
		*/
		private void _SetupLibraryBody()
		{
			Component.Base Components = _GlobalExplorer.Components;
			if ( Components.HasAll( BodyComponents.ToArray() )) 
			{
				_LibraryBody = Components.Single<Body>(true);
				
				if( HasLibraryBody() ) 
				{
					_LibraryBody.Container = PanelContainer;
					_LibraryBody.Library = this;
					_LibraryBody.Initialize();
				}
			}
		}
		
		/*
		** Setups the library title if the component exists
		**
		** @return void
		*/
		private void _SetupLibraryTopbar()
		{
			Component.Base Components = _GlobalExplorer.Components;
			if ( Components.HasAll( TopbarComponents.ToArray() )) 
			{
				_LibraryTopbar = Components.Single<Topbar>(true);
				
				if( HasLibraryTopbar() ) 
				{
					_LibraryTopbar.Container = _LibraryBody.GetRightInnerContainer();
					_LibraryTopbar.Library = this;
					_LibraryTopbar.Initialize();
				}
			}
		}
		
		/*
		** Setups the library settings if the component exists
		**
		** @return void
		*/
		private void _SetupLibrarySettings()
		{
			Component.Base Components = _GlobalExplorer.Components;
			if (Components.HasAll( SettingsComponents.ToArray() )) 
			{
				_LibrarySettings = Components.Single<Settings>(true);
				
				if( HasLibrarySettings() ) 
				{
					_LibrarySettings.Container = _LibraryBody.GetLeftInnerContainer();
					_LibrarySettings.Library = this;
					_LibrarySettings.Initialize();
				}
			}
		}
		
		/*
		** Setups the library listing if the component exists
		**
		** @return void
		*/
		private void _SetupLibraryListing()
		{
			Component.Base Components = _GlobalExplorer.Components;
			if (Components.HasAll( ListingComponents.ToArray() )) 
			{
				_LibraryListing = Components.Single<Listing>(true);
				
				if( HasLibraryListing() ) 
				{
					_LibraryListing.Container = _LibraryBody.GetRightInnerContainer();
					_LibraryListing.Folder = Folder;
					_LibraryListing.Library = this;
					_LibraryListing.Initialize();
				}
			}
		}
		
		public string GetName()
		{
			var split = Folder.Split('/');
			return split[ split.Length - 1];
		}
		
		/*
		** Checks if the library title exists
		**
		** @return bool
		*/
		private bool HasLibraryTopbar()
		{
			return null != _LibraryTopbar;
		}
						
		/*
		** Checks if the library listing exists
		**
		** @return bool
		*/
		private bool HasLibraryListing()
		{
			return null != _LibraryListing;
		}
						
		/*
		** Checks if the library settings exists
		**
		** @return bool
		*/
		private bool HasLibrarySettings()
		{
			return null != _LibrarySettings;
		}
						
		/*
		** Checks if the library body exists
		**
		** @return bool
		*/
		private bool HasLibraryBody()
		{
			return null != _LibraryBody;
		}
		
		/*
		** Returns the last entry name for the folder
		** So it can be used as a name it also removes _
		**
		** @return string
		*/
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
		
		private void _OnLibraryChanged( string name )
		{
			if( name == GetName() && null != _LibrarySettings ) 
			{
				// This is the library that was changed to.
				// Update the values in the global states based on this library.
				_LibrarySettings.Sync();
			}
		}
		
		public void Reset()
		{
			ClearAllPanelState();
			_LibrarySettings.ClearAll();
		}

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