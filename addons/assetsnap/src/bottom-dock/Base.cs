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

namespace AssetSnap.BottomDock
{
	using System.Collections.Generic;
	using AssetSnap.Front.Components;
	using Godot;

	[Tool]
	public partial class Base : Node
	{
		private static readonly string ThemePath = "res://addons/assetsnap/assets/themes/SnapTheme.tres";
		private readonly Theme _Theme = GD.Load<Theme>(ThemePath);

		private PanelContainer _PanelContainer;
		private HBoxContainer MainContainer;
		private VBoxContainer SubContainerOne;
		private VBoxContainer SubContainerTwo;
		private VBoxContainer SubContainerThree;
		private TabContainer _Container;

		private List<string> GeneralComponents = new()
		{
			"Introduction",
			"AddFolderToLibrary",
			"LibrariesListing",
			"Contribute",
		};

		public TabContainer Container
		{
			get => _Container;
			set
			{
				_Container = value;
			}
		}
		
		private static Base _Instance;
		
		public static Base GetInstance()
		{
			if( null == _Instance ) 
			{
				_Instance = new()
				{
					Name = "AssetSnapBottomDock"
				};
			}

			return _Instance;
		}
		/*
		** Initializes the bottom dock tab
		**
		** @return void
		*/
		public void Initialize()
		{
			SetupTabContainer();
			SetupGeneralTab();
		}
		
		/*
		** Configures and initializes the container
		**
		** @return void
		*/
		private void SetupTabContainer()
		{
			_Container = new()
			{
				Name = "AssetSnapTabContainer",
				Theme = _Theme
			};
		}
		
		/*
		** Configure and initialize the "General" tab.
		**
		** @return void
		*/
		private void SetupGeneralTab()
		{
			SetupGeneralTabContainers();
			PlaceGeneralTabContainers();

			InitializeGeneralTabComponents();
		}
		
		/*
		** Initialies the containers which will be used
		** in our general tab..
		**
		** @return void
		*/
		private void SetupGeneralTabContainers()
		{
			_PanelContainer = new()
			{
				Name = "General",
				CustomMinimumSize = new Vector2(0, 205)
			};

			MainContainer = new() 
			{
				Name = "GeneralMainContainer",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.Fill,
			};
			
			SubContainerOne = new()
			{
				Name = "GeneralSubContainerOne",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.Fill,
			};
			
			SubContainerTwo = new()
			{
				Name = "GeneralSubContainerTwo",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.Fill,
			};
			
			SubContainerThree = new()
			{
				Name = "GeneralSubContainerThree",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.Fill,
			};
		}
		
		/*
		** Places our containers in the tabcontainer
		** so it becomes visible and useable.
		**
		** @return void
		*/
		private void PlaceGeneralTabContainers()
		{
			MainContainer.AddChild(SubContainerOne);
			MainContainer.AddChild(SubContainerTwo);
			MainContainer.AddChild(SubContainerThree);

			_PanelContainer.AddChild(MainContainer);
			_Container.AddChild(_PanelContainer);
		}
		
		/*
		** Initializes the various components that the general tab
		** need to display it's content
		**
		** @return void
		*/
		private void InitializeGeneralTabComponents()
		{
			if (GlobalExplorer.GetInstance().Components.HasAll( GeneralComponents.ToArray() )) 
			{
				Introduction _Introduction = GlobalExplorer.GetInstance().Components.Single<Introduction>();
				
				if( _Introduction != null ) 
				{
					_Introduction.Container = SubContainerOne;
					_Introduction.Initialize();
				}
				 
				AddFolderToLibrary _AddFolderToLibrary = GlobalExplorer.GetInstance().Components.Single<AddFolderToLibrary>();
				
				if( _AddFolderToLibrary != null ) 
				{
					_AddFolderToLibrary.Container = SubContainerOne;
					_AddFolderToLibrary.Initialize();
				} 
				
				LibrariesListing _LibrariesListing = GlobalExplorer.GetInstance().Components.Single<LibrariesListing>();
				
				if( _LibrariesListing != null ) 
				{
					_LibrariesListing.Container = SubContainerTwo;
					_LibrariesListing.Initialize();
				}
				
				Contribute Contribute = GlobalExplorer.GetInstance().Components.Single<Contribute>();
				
				if( Contribute != null )  
				{
					Contribute.Container = SubContainerThree;
					Contribute.Initialize();
				}
			}
		}
		
		/*
		** Adds a child to our tab container
		**
		** @return void
		*/
		public void Add( Container container )
		{
			if( Has( container ) ) 
			{
				Remove(container);
			}

			_Container.AddChild(container, true);
		}
		
		public void AddToBottomPanel()
		{
			GlobalExplorer explorer = GlobalExplorer.GetInstance();
			if( null == explorer ) 
			{
				return;
			}
			
			if( null == explorer._Plugin || null == _Container ) 
			{
				return;
			}
			
			explorer._Plugin.AddControlToBottomPanel(_Container, "Assets" );
		}
		
		public void RemoveFromBottomPanel()
		{
			GlobalExplorer explorer = GlobalExplorer.GetInstance();
			if( null == explorer ) 
			{
				return;
			}
			
			if( null == explorer._Plugin || null == _Container ) 
			{
				return;
			}
			
			explorer._Plugin.RemoveControlFromBottomPanel(_Container);
		}
		
		/*
		** Check if a child exists in our tab container 
		**
		** @return void
		*/
		public bool Has( Container container )
		{
			foreach( Node child in _Container.GetChildren() ) 
			{
				if( EditorPlugin.IsInstanceValid(child) && child.Name == container.Name ) 
				{
					return true;
				}
			}
 
			return false;
		}

		public void Remove( Container container ) 
		{
			foreach( Node child in _Container.GetChildren() ) 
			{
				if(  EditorPlugin.IsInstanceValid(child) && child.Name == container.Name ) 
				{
					// _Container.RemoveChild(child);
					child.QueueFree();
				}
			}
		}
		
		public void RemoveByName( string name ) 
		{
			foreach( Node child in _Container.GetChildren() ) 
			{
				if(  EditorPlugin.IsInstanceValid(child) && child.Name == name ) 
				{
					// _Container.RemoveChild(child);
					child.QueueFree();
				}
			}
		}
		/*
		** Cleaning, for when the dock is removed from
		** the tree.
		**
		** @return void
		*/
		public override void _ExitTree()
		{
			if( EditorPlugin.IsInstanceValid(SubContainerThree) ) 
			{
				SubContainerThree.QueueFree();
				SubContainerThree = null;
			}
			if( EditorPlugin.IsInstanceValid(SubContainerTwo) ) 
			{
				SubContainerTwo.QueueFree();
				SubContainerTwo = null;
			} 
			if( EditorPlugin.IsInstanceValid(SubContainerOne) ) 
			{ 
				SubContainerOne.QueueFree();
				SubContainerOne = null;
			}
			if( EditorPlugin.IsInstanceValid(MainContainer) ) 
			{
				MainContainer.QueueFree();
				MainContainer = null;
			}
			if( EditorPlugin.IsInstanceValid(_PanelContainer) ) 
			{
				_PanelContainer.QueueFree();
				_PanelContainer = null;
			} 
			
			RemoveFromBottomPanel();

			if( EditorPlugin.IsInstanceValid(_Container) ) 
			{
				_Container.Free();
				_Container = null;
			} 
		}
	}
} 