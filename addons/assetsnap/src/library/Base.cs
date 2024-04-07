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
	using System;
	using System.Collections.Generic;
    using AssetSnap.Explorer;
    using AssetSnap.Front.Components;
	using Godot;
	
	[Tool]
	public partial class Base : Node
	{
		private GlobalExplorer _GlobalExplorer;
		
		private Instance[] _Libraries = Array.Empty<Instance>();

		private static Base _Instance;
		
		public static Base GetInstance()
		{
			if( null == _Instance ) 
			{
				_Instance = new()
				{
					Name = "AssetSnapLibraries",
				};
			}

			return _Instance;
		}
		
		public Instance[] Libraries 
		{
			get => _Libraries;
		}
		
		public void Initialize()
		{
			_GlobalExplorer = GlobalExplorer.GetInstance();
			
			/** Initialize libraries **/
			if( _GlobalExplorer.Settings.FolderCount != 0 ) 
			{
				string[] Folders = _GlobalExplorer.Settings.Folders; 
				
				for( int i = 0; i < _GlobalExplorer.Settings.FolderCount; i++)  
				{
					string Folder = Folders[i];
					_GlobalExplorer.Library.New(_GlobalExplorer.BottomDock, Folder, i); 
				}
			} 
		}
		
		/*
		** Creates a new instance of Library
		**
		** @param TabContainer _Container
		** @param string _Folder
		** @return void
		*/
		public void New( BottomDock.Base Dock,  string _Folder, int index )
		{
			if( false == IsGlobalExplorerValid() ) 
			{
				return;
			}
			
			Instance _Base = new()
			{
				Container = Dock.Container,
				Folder = _Folder,
				Index = index,
			};

			AddChild(_Base);
 
			_Base.Initialize(); 
			
			List<Instance> _LibrariesList = new(_Libraries)
			{
				_Base
			};

			_Libraries = _LibrariesList.ToArray();
		}
		
		/*
		** Refreshes current libraries
		**
		** @param TabContainer _Container
		*/
		public void Refresh(BottomDock.Base Dock)
		{
			if( false == IsGlobalExplorerValid() ) 
			{
				return;
			}
			
			// Resets current settings
			ExplorerUtils.Get().Settings.Reset();

			if( HasFolders() ) 
			{
				string[] Folders = ExplorerUtils.Get().Settings.Folders;
				
				for( int i = 0; i < ExplorerUtils.Get().Settings.FolderCount; i++) 
				{
					string Folder = Folders[i];
					bool exist = false;
					 
					foreach(Instance Library in _Libraries ) 
					{ 
						if( Library.Folder == Folder ) 
						{
							exist = true;
						}  
					}
					 
					if( false == exist ) 
					{
						New(Dock, Folder, i);
					}
				}
			} 
			
			if( HasLibraryListing() ) 
			{
				LibrariesListing _LibrariesListing = GetLibraryListing();
				_LibrariesListing.ForceUpdate();
			}
			
			RebindSettingsContainer();
		}
		
		/*
		** Rebinds the settings container to the bottom dock
		**
		** @return LibrariesListing
		*/  
		private void RebindSettingsContainer()
		{
			_GlobalExplorer.Settings.InitializeContainer();
			_GlobalExplorer.Settings.Container.Name = "Settings";
			_GlobalExplorer.BottomDock.Add(_GlobalExplorer.Settings.Container);
		}
		
		public void RemoveLibrary( string FolderPath )
		{
			List<Library.Instance> list = new(_Libraries);
			
			foreach(Instance Library in _Libraries ) 
			{
				if( Library.Folder == FolderPath ) 
				{
					list.Remove(Library);
					
					if( null != Library.GetParent() && IsInstanceValid(Library.GetParent()) ) 
					{
						Library.GetParent().RemoveChild(Library);
					}
					Library.QueueFree();
				}
			}
		}
		
		public void Reset()
		{
			foreach(Instance Library in _Libraries ) 
			{
				Library.ClearAllPanelState();
				Library._LibrarySettings.ClearAll();
			}
		}
		
		/*
		** Get library listing
		**
		** @return LibrariesListing
		*/
		private LibrariesListing GetLibraryListing()
		{
			LibrariesListing _LibrariesListing = _GlobalExplorer.Components.Single<LibrariesListing>();
			return _LibrariesListing;
		}
		
		/*
		** Checks if there are ny folders founds
		*/
		private bool HasFolders()
		{
			return _GlobalExplorer.Settings.FolderCount != 0;
		}
						
		/*
		** Checks if the library listing exists
		**
		** @return bool
		*/
		private bool HasLibraryListing()
		{
			LibrariesListing _LibrariesListing = _GlobalExplorer.Components.Single<LibrariesListing>();
			return null != _LibrariesListing;
		}
		
		/*
		** Checks if the global class GlobalExplorer reference is
		** available
		**
		** @return bool
		*/
		private bool IsGlobalExplorerValid()
		{
			return ExplorerUtils.IsValid();
		}

		public override void _ExitTree()
		{
			for( int i = 0; i < _Libraries.Length; i++ ) 
			{
				if( null != _Libraries[i] && IsInstanceValid( _Libraries[i] ) )
				{
					if( null != _Libraries[i].GetParent() && IsInstanceValid(_Libraries[i].GetParent())) 
					{
						_Libraries[i].GetParent().RemoveChild(_Libraries[i]);
					}
					
					_Libraries[i].QueueFree();
				} 
			}
			
			_Libraries = Array.Empty<Instance>();
			_GlobalExplorer = null;
			_Instance = null; 

			base._ExitTree();
		}
	}
}