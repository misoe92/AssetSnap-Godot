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
	using AssetSnap.Component;
	using AssetSnap.Front.Components;
	using Godot;

	[Tool]
	public partial class AsBottomDock : VBoxContainer
	{
		public VBoxContainer _LoadingContainer;
		public TabContainer _TabContainer;
		private List<string> GeneralComponents = new()
		{
			"Introduction",
			"Actions",
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
		private readonly Callable _callable = Callable.From((int index) => { _OnTabChanged(index); });

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
			SetupLoadingContainers();
			SetupGeneralTabContainers();
			PlaceGeneralTabContainers();

			InitializeGeneralTabComponents();

			_TabContainer.Connect(TabContainer.SignalName.TabChanged, _callable);

			AddChild(_TabContainer);
			AddChild(_LoadingContainer);
		}

		private void SetupLoadingContainers()
		{
			_LoadingContainer = new();

			MarginContainer margin = new();

			margin.AddThemeConstantOverride("margin_top", 10);
			margin.AddThemeConstantOverride("margin_bottom", 10);
			margin.AddThemeConstantOverride("margin_left", 10);
			margin.AddThemeConstantOverride("margin_right", 10);

			VBoxContainer inner = new();

			Label label = new()
			{
				ThemeTypeVariation = Labelable.TitleType.HeaderMedium.ToString(),
				Text = "No scene is currently active"
			};

			Label description = new()
			{
				Text = "This addon requires an active scene for it to work."
			};

			inner.AddChild(label);
			inner.AddChild(description);
			margin.AddChild(inner);
			_LoadingContainer.AddChild(margin);
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
				Visible = false,
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
			if (GlobalExplorer.GetInstance().Components.HasAll(GeneralComponents.ToArray()))
			{
				Introduction _Introduction = GlobalExplorer.GetInstance().Components.Single<Introduction>();

				if (_Introduction != null)
				{
					_Introduction.Container = SubContainerOne;
					_Introduction.Initialize();
				}

				Actions _Actions = GlobalExplorer.GetInstance().Components.Single<Actions>();

				if (_Actions != null)
				{
					_Actions.Container = SubContainerOne;
					_Actions.Initialize();
				}

				Contribute Contribute = GlobalExplorer.GetInstance().Components.Single<Contribute>();

				if (Contribute != null)
				{
					Contribute.Container = SubContainerThree;
					Contribute.Initialize();
				}

				Plugin.Singleton.FoldersLoaded += () => { _OnLoadListing(); };
			}
		}

		private void _OnLoadListing()
		{
			GlobalExplorer.GetInstance().Components.Clear<LibrariesListing>();
			LibrariesListing _LibrariesListing = GlobalExplorer.GetInstance().Components.Single<LibrariesListing>();

			if (_LibrariesListing != null)
			{
				_LibrariesListing.Container = SubContainerTwo;
				_LibrariesListing.Initialize();
			}
		}

		private static void _OnTabChanged(int index)
		{
			if (
				null == GlobalExplorer.GetInstance() ||
				null == GlobalExplorer.GetInstance().States ||
				null == GlobalExplorer.GetInstance().GetLibraryByIndex(index - 1)
			)
			{
				return;
			}

			GlobalExplorer.GetInstance().States.CurrentLibrary = GlobalExplorer.GetInstance().GetLibraryByIndex(index - 1);
		}
	}
}
#endif
