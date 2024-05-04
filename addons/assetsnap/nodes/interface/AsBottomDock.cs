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

using System.Collections.Generic;
using AssetSnap.Component;
using AssetSnap.Front.Components;
using AssetSnap.States;
using Godot;

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// Partial class representing the bottom dock UI element.
	/// </summary>
	[Tool]
	public partial class AsBottomDock : VBoxContainer
	{
		public VBoxContainer _LoadingContainer;
		public TabContainer _TabContainer;
		
		private static readonly string _ThemePath = "res://addons/assetsnap/assets/themes/SnapTheme.tres";
		private readonly Theme _Theme = GD.Load<Theme>(_ThemePath);
		private readonly Callable _Callable = Callable.From((int index) => { _OnTabChanged(index); });
		private List<string> _GeneralComponents = new()
		{
			"Introduction",
			"Actions",
			"LibrariesListing",
			"Contribute",
		};
		private PanelContainer _PanelContainer;
		private HBoxContainer _MainContainer;
		private VBoxContainer _SubContainerOne;
		private VBoxContainer _SubContainerTwo;
		private VBoxContainer _SubContainerThree;


		/// <summary>
		/// Called when the node enters the scene tree.
		/// </summary>
		public override void _EnterTree()
		{
			Name = "BottomDock";
			Theme = _Theme;
			
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
			SizeFlagsVertical = Control.SizeFlags.ExpandFill;

			// CustomMinimumSize = new Vector2(0, 205);
			SetupGeneralTab();
		}

		/// <summary>
		/// Configures and initializes the "General" tab.
		/// </summary>
		private void SetupGeneralTab()
		{
			SetupLoadingContainers();
			SetupGeneralTabContainers();
			PlaceGeneralTabContainers();

			InitializeGeneralTabComponents();

			_TabContainer.Connect(TabContainer.SignalName.TabChanged, _Callable);

			AddChild(_TabContainer);
			AddChild(_LoadingContainer);
		}

		/// <summary>
		/// Sets up the loading containers.
		/// </summary>
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

		/// <summary>
		/// Initializes the containers used in the general tab.
		/// </summary>
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

			_MainContainer = new()
			{
				Name = "GeneralMainContainer",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};

			_SubContainerOne = new()
			{
				Name = "GeneralSubContainerOne",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};

			_SubContainerTwo = new()
			{
				Name = "GeneralSubContainerTwo",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};

			_SubContainerThree = new()
			{
				Name = "GeneralSubContainerThree",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};
		}

		/// <summary>
		/// Places the containers in the tab container.
		/// </summary>
		private void PlaceGeneralTabContainers()
		{
			_MainContainer.AddChild(_SubContainerOne);
			_MainContainer.AddChild(_SubContainerTwo);
			_MainContainer.AddChild(_SubContainerThree);

			_PanelContainer.AddChild(_MainContainer);
			_TabContainer.AddChild(_PanelContainer);
		}

		/// <summary>
		/// Initializes the various components of the general tab.
		/// </summary>
		private void InitializeGeneralTabComponents()
		{
			if (GlobalExplorer.GetInstance().Components.HasAll(_GeneralComponents.ToArray()))
			{
				Introduction _Introduction = GlobalExplorer.GetInstance().Components.Single<Introduction>();

				if (_Introduction != null)
				{
					_Introduction.Initialize();
					_SubContainerOne.AddChild(_Introduction);
				}

				Actions _Actions = GlobalExplorer.GetInstance().Components.Single<Actions>();

				if (_Actions != null)
				{
					_Actions.Initialize();
					_SubContainerOne.AddChild(_Actions);
				}

				Contribute Contribute = GlobalExplorer.GetInstance().Components.Single<Contribute>();

				if (Contribute != null)
				{
					Contribute.Initialize();
					_SubContainerThree.AddChild(Contribute);
				}

				Plugin.Singleton.FoldersLoaded += () => { _OnLoadListing(); };
			}
		}

		/// <summary>
		/// Event handler for the load listing event.
		/// </summary>
		private void _OnLoadListing()
		{
			GlobalExplorer.GetInstance().Components.Clear<LibrariesListing>();
			LibrariesListing _LibrariesListing = GlobalExplorer.GetInstance().Components.Single<LibrariesListing>();

			if (_LibrariesListing != null)
			{
				_LibrariesListing.Initialize();
				_SubContainerTwo.AddChild(_LibrariesListing);
			}
		}

		/// <summary>
		/// Event handler for tab changes.
		/// </summary>
		/// <param name="index">The index of the tab.</param>
		private static void _OnTabChanged(int index)
		{
			if (
				null == GlobalExplorer.GetInstance() ||
				null == StatesUtils.Get() ||
				null == GlobalExplorer.GetInstance().GetLibraryByIndex(index - 1)
			)
			{
				return;
			}

			StatesUtils.Get().CurrentLibrary = GlobalExplorer.GetInstance().GetLibraryByIndex(index - 1);
		}
	}
}

#endif
