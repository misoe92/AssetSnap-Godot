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

namespace AssetSnap.Front.Nodes
{
	using AssetSnap.Explorer;
	using AssetSnap.States;
	using Godot;

	[Tool]
	public partial class AsGroupContextMenu : Control
	{
		private readonly Theme _Theme = GD.Load<Theme>("res://addons/assetsnap/assets/themes/SnapMenu.tres");

		public PanelContainer MenuPanelContainer;
		public Button AddToCurrentGroup;
		private string FocusedMesh = "";

		public void ShowMenu(Vector2 Coordinates, string MeshPath)
		{
			if (EditorPlugin.IsInstanceValid(MenuPanelContainer) && EditorPlugin.IsInstanceValid(StatesUtils.Get().Group))
			{
				MenuPanelContainer.Visible = true;
				MenuPanelContainer.Position = Coordinates;

				FocusedMesh = MeshPath;
			}
		}

		public void MaybeHideMenu(Vector2 Coordinates)
		{
			if (EditorPlugin.IsInstanceValid(MenuPanelContainer) && true == MenuPanelContainer.Visible)
			{
				if (null != ExplorerUtils.Get().BottomDock.Container && false == ExplorerUtils.Get().BottomDock.Container.Visible)
				{
					HideMenu();
					return;
				}

				Vector2 CurrentPosition = MenuPanelContainer.Position;
				if (CurrentPosition.X + MenuPanelContainer.Size.X + 20 < Coordinates.X || CurrentPosition.X - 20 > Coordinates.X || CurrentPosition.Y + MenuPanelContainer.Size.Y + 20 < Coordinates.Y || CurrentPosition.Y - 20 > Coordinates.Y)
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

		public bool IsVisible()
		{
			return MenuPanelContainer.Visible;
		}

		public void Initialize()
		{
			if( false == Plugin.Singleton.HasInternalContainer() ) 
			{
				return;
			}
			
			MenuPanelContainer = new()
			{
				Name = "MenuPanelContainer",
				Visible = false,
				Theme = _Theme
			};
			MarginContainer MenuMarginContainer = new()
			{
				Name = "MenuMarginContainer",
			};
			VBoxContainer MenuBoxContainer = new()
			{
				Name = "MenuBoxContainer"
			};

			Label MenuTitle = new()
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

			Plugin.Singleton
				.GetInternalContainer()
				.AddChild(MenuPanelContainer);
		}

		public bool IsHidden()
		{
			return MenuPanelContainer.Visible == false;
		}

		private void _SetupAddToCurrentGroupButton(VBoxContainer MenuBoxContainer)
		{
			AddToCurrentGroup = new()
			{
				Text = "Add to current group",
				TooltipText = "Click to add the object to the active group",
				CustomMinimumSize = new Vector2I(180, 20),
				MouseDefaultCursorShape = Control.CursorShape.PointingHand
			};

			AddToCurrentGroup.Connect(Button.SignalName.Pressed, Callable.From(() => { _OnAddToCurrentGroup(); }));

			MenuBoxContainer.AddChild(AddToCurrentGroup);
		}

		private void _OnAddToCurrentGroup()
		{
			ExplorerUtils.Get().GroupBuilder._Editor.AddMeshToGroup(FocusedMesh);
			HideMenu();
		}
	}
}