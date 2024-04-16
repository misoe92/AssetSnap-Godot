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

#if TOOLS
namespace AssetSnap.Front.Nodes
{
	using System.Collections.Generic;
	using AssetSnap.Front.Components;
	using Godot;

	[Tool]
	public partial class AsBottomDock : VBoxContainer
	{
		public TabContainer _TabContainer;
		private List<string> GeneralComponents = new()
		{
			"Introduction",
			"AddFolderToLibrary", 
			"LibrariesListing",
			"Contribute",
		};
		private PanelContainer _PanelContainer;
		private HBoxContainer MainContainer;
		private VBoxContainer SubContainerOne;
		private VBoxContainer SubContainerTwo;
		private VBoxContainer SubContainerThree;
		private static readonly string ThemePath = "res://addons/assetsnap/assets/themes/SnapTheme.tres";
		private readonly Theme _Theme = GD.Load<Theme>(ThemePath);
		public new Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		public new Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		private readonly Callable _callable = Callable.From((long index) => { _OnTabChanged(index); } );
		
		public override void _EnterTree()
		{
			Name = "BottomDock";
			Theme = _Theme;
		
			// CustomMinimumSize = new Vector2(0, 205);
			SetupGeneralTab();
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
			
			_TabContainer.Connect(TabContainer.SignalName.TabChanged, _callable);

			AddChild(_TabContainer);
		}
		
		/*
		** Initialies the containers which will be used
		** in our general tab..
		**
		** @return void
		*/
		private void SetupGeneralTabContainers()
		{
			_TabContainer = new()
			{
				Name = "ASTabContainer",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};
			
			_PanelContainer = new()
			{
				Name = "General",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};

			MainContainer = new() 
			{
				Name = "GeneralMainContainer",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};
			
			SubContainerOne = new()
			{
				Name = "GeneralSubContainerOne",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};
			 
			SubContainerTwo = new()
			{
				Name = "GeneralSubContainerTwo",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};
			
			SubContainerThree = new()
			{
				Name = "GeneralSubContainerThree",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
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
			_TabContainer.AddChild(_PanelContainer);
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
			
		private static void _OnTabChanged( long index )
		{
			if( 
				null == GlobalExplorer.GetInstance() ||
				null == GlobalExplorer.GetInstance().States ||
				null == GlobalExplorer.GetInstance().GetLibraryByIndex(index-1)
			) 
			{ 
				return; 
			}
			
			GlobalExplorer.GetInstance().States.CurrentLibrary = GlobalExplorer.GetInstance().GetLibraryByIndex(index-1);
		}
		
		public override void _ExitTree()
		{
			foreach(Node node in GetChildren() ) 
			{
				RemoveChild(node);
				node.QueueFree();
			}
			 
			base._ExitTree(); 
		}
	}
}
#endif
		