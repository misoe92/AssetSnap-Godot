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
	using AssetSnap.Front.Components;
	using Godot;
	
	[Tool]
	public partial class Instance : Node
	{
		public int Index;
		private string _Folder;
		private string _FileName; 
		private GlobalExplorer _GlobalExplorer;
		private Godot.Collections.Array<AsLibraryPanelContainer> Panels = new();
		private TabContainer _Container;
		private PanelContainer _PanelContainer;
		private HBoxContainer _BoxContainerRight; 
		
		private readonly List<string> BodyComponents = new()
		{
			"LibraryBody",
		};
		
		private readonly List<string> TopbarComponents = new()
		{
			"LibraryTopbar",
		};
		
		private readonly List<string> SettingsComponents = new()
		{
			"LibraryBody",
			"LibrarySettings",
		};
		
		private readonly List<string> ListingComponents = new()
		{
			"LibraryBody",
			"LibraryListing",
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
		
		public TabContainer Container 
		{
			get => _Container;
			set 
			{
				_Container = value;
			}
		}
		
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
		public LibraryBody _LibraryBody;
		public LibraryTopbar _LibraryTopbar;
		public LibrarySettings _LibrarySettings;
		public LibraryListing _LibraryListing;
		
		public Instance()
		{
			Name = "LibraryInstance";
		}
		
		/*
		** Initializes the library
		** Setting up it's nodes and UI and more
		** 
		** @return void
		*/
		public void Initialize()
		{
			_GlobalExplorer = GlobalExplorer.GetInstance();
			
			_PanelContainer = new()
			{
				Name = _FileName.Capitalize()
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

			_Container.AddChild(_PanelContainer);
			_PanelContainer.SetMeta("FolderPath", _Folder); 

			_GlobalExplorer._Plugin.Connect(Plugin.SignalName.LibraryChanged, Callable.From( ( string name ) => { _OnLibraryChanged( name ); }));
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
				_LibraryBody = Components.Single<LibraryBody>(true);
				
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
				_LibraryTopbar = Components.Single<LibraryTopbar>(true);
				
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
				_LibrarySettings = Components.Single<LibrarySettings>(true);
				
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
				_LibraryListing = Components.Single<LibraryListing>(true);
				
				if( HasLibraryListing() ) 
				{
					_LibraryListing.Container = _LibraryBody.GetRightInnerContainer();
					_LibraryListing.Folder = Folder;
					_LibraryListing.Library = this;
					
					_LibraryListing.Initialize();
					
					if( null != _LibraryTopbar ) 
					{
						_LibraryTopbar.ItemCount = GetPanels().Count;
					}
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
		** Checks if the library search exists
		**
		** @return bool
		*/
		private bool HasLibrarySearch()
		{
			return null != _LibraryTopbar._LibrarySearch;
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
			// GD.Print(GetName());
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

			if( null != _BoxContainerRight && IsInstanceValid(_BoxContainerRight) ) 
			{
				_BoxContainerRight.QueueFree();
			}

			base._ExitTree();
		}
	}
}