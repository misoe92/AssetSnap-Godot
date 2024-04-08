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
namespace AssetSnap.GroupBuilder
{
	using System.Collections.Generic;
	using AssetSnap.Explorer;

	using AssetSnap.Front.Components.Groups.Builder;
	using AssetSnap.Front.Nodes;
	using AssetSnap.States;
	using Godot;
	
	[Tool]
	public partial class Base : Node
	{
		public PanelContainer Container;

		public Front.Components.Groups.Container _GroupContainer;
		public Sidebar _Sidebar;
		public Editor _Editor;

		public PanelContainer MenuPanelContainer;
		public MarginContainer MenuMarginContainer;
		public VBoxContainer MenuBoxContainer;
		public Button AddToCurrentGroup;
		public Label MenuTitle;

		private GlobalExplorer _GlobalExplorer;
		private readonly Theme _Theme = GD.Load<Theme>("res://addons/assetsnap/assets/themes/SnapMenu.tres");
		
		private Godot.Collections.Array<AsMeshInstance3D> _Items = new();

		private string FocusedMesh = "";
		
		private readonly List<string> OuterComponents = new()
		{
			"Groups.Container",
		};
		
		private readonly List<string> InnerComponents = new()
		{
			"Groups.Builder.Sidebar",
			"Groups.Builder.Editor",
		};
		
		public void Initialize()
		{
			_GlobalExplorer = GlobalExplorer.GetInstance();
			_InitializeMenu();

			StatesUtils.SetLoad("GroupBuilder", true);
		}

		/*
		** Initializes the container in the assets tab
		*/
		public void InitializeContainer()
		{
			Container = new()
			{
				Name = "GroupBuilder",
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			};

			/** Add the tab item if settings is turned on **/
			Component.Base Components = ExplorerUtils.Get().Components;
			if ( Components.HasAll( OuterComponents.ToArray() )) 
			{
				_GroupContainer = Components.Single<AssetSnap.Front.Components.Groups.Container>();
				
				if( HasGroupContainer() ) 
				{
					_GroupContainer.Container = Container;
					_GroupContainer.Initialize();
			
					if ( Components.HasAll( InnerComponents.ToArray() )) 
					{
						_Sidebar = Components.Single<Sidebar>();
						_Editor = Components.Single<Editor>();
						
						if( HasSidebar() ) 
						{
							_Sidebar.Container = _GroupContainer.GetLeftInnerContainer();
							_Sidebar.Initialize();
						}
						
						if( HasListing() ) 
						{
							_Editor.Container = _GroupContainer.GetRightInnerContainer();
							_Editor.Initialize();
						}
						
						StatesUtils.SetLoad("GroupBuilderContainer", true);
					}
					else
					{
						GD.PushError("Components was not found @ GroupBuilder -> Inner");
					}
				}
				else 
				{
					GD.PushError("Could not spawn group container");
				}
			}
			else
			{
				GD.PushError("Components was not found @ GroupBuilder -> Outer");
			}
		}
		
		public void ClearContainer()
		{
			Container.GetParent().RemoveChild(Container);
			Container.QueueFree();
		}
		
		public void ShowMenu( Vector2 Coordinates, string MeshPath ) 
		{
			if( IsInstanceValid( MenuPanelContainer ) && IsInstanceValid(_GlobalExplorer.States.Group) ) 
			{
				MenuPanelContainer.Visible = true;
				MenuPanelContainer.Position = Coordinates;

				FocusedMesh = MeshPath;
			}
		}
		
		public void MaybeHideMenu( Vector2 Coordinates ) 
		{
			if( IsInstanceValid( MenuPanelContainer ) &&  true == MenuPanelContainer.Visible ) 
			{
				if( false == GlobalExplorer.GetInstance().BottomDock.Container.Visible ) 
				{
					HideMenu();
					return;
				}
				
				Vector2 CurrentPosition = MenuPanelContainer.Position;
				if( CurrentPosition.X + MenuPanelContainer.Size.X + 20  < Coordinates.X || CurrentPosition.X - 20 > Coordinates.X || CurrentPosition.Y + MenuPanelContainer.Size.Y + 20 < Coordinates.Y || CurrentPosition.Y - 20 > Coordinates.Y ) 
				{
					HideMenu();
					return;
				}
			}
		}
		
		public void HideMenu()
		{
			MenuPanelContainer.Visible = false;
			MenuPanelContainer.Position = new Vector2(-400, -400);
			FocusedMesh = "";
		}
		
		public bool HasSidebar()
		{
			return null != _Sidebar;
		}
		
		public bool HasListing()
		{
			return null != _Editor;
		}
		
		private void _InitializeMenu()
		{
			MenuPanelContainer = new()
			{
				Name = "MenuPanelContainer",
				Visible = false,
				Theme = _Theme
			};
			MenuMarginContainer = new()
			{
				Name = "MenuMarginContainer",
			};
			MenuBoxContainer = new()
			{
				Name = "MenuBoxContainer"
			};

			MenuTitle = new()
			{
				Text = "Menu",
				ThemeTypeVariation = "HeaderSmall",
				SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			}; 
			
			MenuMarginContainer.AddThemeConstantOverride("margin_left", 5);
			MenuMarginContainer.AddThemeConstantOverride("margin_right", 5);
			MenuMarginContainer.AddThemeConstantOverride("margin_top", 5);
			MenuMarginContainer.AddThemeConstantOverride("margin_bottom", 5);

			MenuBoxContainer.AddChild(MenuTitle);

			_SetupAddToCurrentGroupButton(MenuBoxContainer);

			MenuMarginContainer.AddChild(MenuBoxContainer);
			MenuPanelContainer.AddChild(MenuMarginContainer);

			MenuPanelContainer.Position = new Vector2(-400, -400);
 
			AddChild(MenuPanelContainer);
		}
		
		private void _SetupAddToCurrentGroupButton( VBoxContainer MenuBoxContainer )
		{
			AddToCurrentGroup = new()
			{
				Text = "Add to current group",
				TooltipText = "Click to add the object to the active group",
				CustomMinimumSize = new Vector2I(180, 20),
				MouseDefaultCursorShape = Control.CursorShape.PointingHand
			};

			AddToCurrentGroup.Connect( Button.SignalName.Pressed, Callable.From( () => { _OnAddToCurrentGroup(); }));

			MenuBoxContainer.AddChild(AddToCurrentGroup);
		}
		
		private void _OnAddToCurrentGroup()
		{
			_Editor.AddMeshToGroup(FocusedMesh);
			HideMenu();
		}
		
		private bool HasGroupContainer()
		{
			return null != _GroupContainer;
		}

		public override void _ExitTree()
		{
			// if( )
			// AddToCurrentGroup.Disconnect( Button.SignalName.Pressed, Callable.From( () => { _OnAddToCurrentGroup(); }));
			base._ExitTree();
		}
	}
}
#endif