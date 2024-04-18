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
		private Instance[] _Libraries = Array.Empty<Instance>();

		private static Base _Instance;
		
		public static Base Singleton {
			get {
				if( _Instance == null ) 
				{
					_Instance = new();
				} 
				
				return _Instance;
			}
		}
		
		public Instance[] Libraries 
		{
			get => _Libraries;
		}
		
		public Base()
		{
			Name = "LibraryBase";
		}
		
		public void Initialize()
		{
			foreach( Instance instance in Libraries ) 
			{
				RemoveLibrary(instance.Folder);
			}
			
			_Libraries = Array.Empty<Instance>();
			
			/** Initialize libraries **/
			if( ExplorerUtils.Get().Settings.FolderCount != 0 ) 
			{
				string[] Folders = ExplorerUtils.Get().Settings.Folders; 
				
				for( int i = 0; i < ExplorerUtils.Get().Settings.FolderCount; i++)  
				{
					string Folder = Folders[i];
					New(ExplorerUtils.Get()._Plugin.GetTabContainer(), Folder, i); 
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
		public void New( TabContainer Dock,  string _Folder, int index )
		{
			if( false == IsGlobalExplorerValid() ) 
			{
				return;
			}
			
			if( AlreadyHaveFolder( _Folder ) ) 
			{
				CleanOldLibrary(_Folder);
			}
			
			Instance _Base = new()
			{
				Folder = _Folder,
				Index = index,
				Dock = Dock,
			};

			_Base.Name = _Base.FileName.Capitalize();

			_Base.Initialize(); 
			
			List<Instance> _LibrariesList = new(_Libraries)
			{
				_Base
			}; 

			_Libraries = _LibrariesList.ToArray();
		}
		
		private bool AlreadyHaveFolder( string Folder )
		{
			foreach( Library.Instance instance in _Libraries ) 
			{
				if( Folder == instance._Folder ) 
				{
					return true;
				}
			}

			return false;
		}
		
		private bool CleanOldLibrary( string Folder )
		{
			foreach( Library.Instance instance in _Libraries ) 
			{
				if( Folder == instance._Folder ) 
				{
					instance.Clear();
					
					instance.GetParent().RemoveChild(instance);
					instance.QueueFree();
					
					return true;
				}
			}

			return false;
		}
		
		/*
		** Rebinds the settings container to the bottom dock
		**
		** @return LibrariesListing
		*/  
		private void RebindSettingsContainer()
		{
			ExplorerUtils.Get().Settings.InitializeContainer();
			if( EditorPlugin.IsInstanceValid(ExplorerUtils.Get().Settings.Container)) 
			{
				ExplorerUtils.Get().Settings.Container.Name = "Settings";
				ExplorerUtils.Get().BottomDock.Add(ExplorerUtils.Get().Settings.Container);
			}
		}
		
		public void RemoveLibrary( string FolderPath )
		{
			List<Library.Instance> list = new(_Libraries);
			
			foreach(Instance Library in _Libraries ) 
			{
				if( Library.Folder == FolderPath ) 
				{
					list.Remove(Library);
					Library.Clear();
					
					if( null != Library.GetParent() && EditorPlugin.IsInstanceValid(Library.GetParent()) ) 
					{
						Library.GetParent().RemoveChild(Library);
					}
					Library.Free();
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
			LibrariesListing _LibrariesListing = ExplorerUtils.Get().Components.Single<LibrariesListing>();
			return _LibrariesListing;
		}
		
		/*
		** Checks if there are ny folders founds
		*/
		private bool HasFolders()
		{
			return ExplorerUtils.Get().Settings.FolderCount != 0;
		}
						
		/*
		** Checks if the library listing exists
		**
		** @return bool
		*/
		private bool HasLibraryListing()
		{
			LibrariesListing _LibrariesListing = ExplorerUtils.Get().Components.Single<LibrariesListing>();
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
	}
}