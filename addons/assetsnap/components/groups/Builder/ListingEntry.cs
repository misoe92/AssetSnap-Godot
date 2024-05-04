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
using AssetSnap.Explorer;
using AssetSnap.Helpers;
using AssetSnap.States;
using Godot;

namespace AssetSnap.Front.Components.Groups.Builder
{
	/// <summary>
	/// Represents an entry in the group builder listing.
	/// </summary>
	[Tool]
	public partial class ListingEntry : LibraryComponent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListingEntry"/> class.
		/// </summary>
		public ListingEntry()
		{
			Name = "GroupBuilderListingEntry";
			//_include = false;
		}

		/// <summary>
		/// Gets or sets the title of the listing entry.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// The container for removing a row.
		/// </summary>
		public HBoxContainer RemoveRow;
		
		/// <summary>
		/// The container for removing an inner row.
		/// </summary>
		public HBoxContainer RemoveInnerRow;

		/// <summary>
		/// The outer margin container.
		/// </summary>
		public MarginContainer OuterMarginContainer;
		
		/// <summary>
		/// The inner margin container.
		/// </summary>
		public MarginContainer InnerMarginContainer;
		
		/// <summary>
		/// The panel container.
		/// </summary>
		public PanelContainer _PanelContainer;
		
		/// <summary>
		/// The row container.
		/// </summary>
		public HBoxContainer Row;
		
		/// <summary>
		/// The title label.
		/// </summary>
		public Label TitleLabel;
		
		/// <summary>
		/// The title label.
		/// </summary>
		public Label RemoveLabel;
		
		/// <summary>
		/// The inner row container.
		/// </summary>
		public HBoxContainer InnerRow;
		
		/// <summary>
		/// The edit button.
		/// </summary>
		public Button Edit;
		
		/// <summary>
		/// The remove button.
		/// </summary>
		public Button Remove;
		
		/// <summary>
		/// The confirm button.
		/// </summary>
		public Button Confirm;
		
		/// <summary>
		/// The cancel button.
		/// </summary>
		public Button Cancel;

		/// <summary>
		/// The active theme.
		/// </summary>
		private readonly Theme _ActiveTheme = GD.Load<Theme>("res://addons/assetsnap/assets/themes/SnapThemeActiveGroup.tres");

		/// <summary>
		/// The root sidebar list.
		/// </summary>
		private readonly List<string> _RootSidebar = new()
		{
			"Groups.Builder.Sidebar",
		};

		/// <summary>
		/// The group editor list.
		/// </summary>
		private readonly List<string> _GroupEditor = new()
		{
			"Groups.Builder.Editor",
		};

		/// <summary>
		/// Initializes the listing entry.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			_Initiated = true;

			OuterMarginContainer = new()
			{
				Name = "GroupBuilderListingEntryMargin"
			};
			InnerMarginContainer = new()
			{
				Name = "GroupBuilderListingEntryPadding"
			};
			_PanelContainer = new()
			{
				Name = "GroupBuilderListingEntryPanel"
			};
			Row = new()
			{
				Name = "GroupBuilderListingEntryRow"

			};
			RemoveRow = new()
			{
				Visible = false
			};

			OuterMarginContainer.AddThemeConstantOverride("margin_left", 10);
			OuterMarginContainer.AddThemeConstantOverride("margin_right", 10);
			OuterMarginContainer.AddThemeConstantOverride("margin_top", 1);
			OuterMarginContainer.AddThemeConstantOverride("margin_bottom", 1);

			InnerMarginContainer.AddThemeConstantOverride("margin_left", 5);
			InnerMarginContainer.AddThemeConstantOverride("margin_right", 5);
			InnerMarginContainer.AddThemeConstantOverride("margin_top", 1);
			InnerMarginContainer.AddThemeConstantOverride("margin_bottom", 1);

			_SetupTitle();
			_SetupButtons();

			_SetupRemoveTitle();
			_SetupRemoveButtons();

			InnerMarginContainer.AddChild(RemoveRow);
			InnerMarginContainer.AddChild(Row);
			_PanelContainer.AddChild(InnerMarginContainer);
			OuterMarginContainer.AddChild(_PanelContainer);
			AddChild(OuterMarginContainer);
		}

		/// <summary>
		/// Deselects the current group.
		/// </summary>
		public void DeselectGroup()
		{
			Component.Base Components = _GlobalExplorer.Components;

			if (Components.HasAll(_GroupEditor.ToArray()))
			{
				Editor Editor = Components.Single<Editor>();
				Editor.GroupPath = null;
			}

			if (Components.HasAll(_RootSidebar.ToArray()))
			{
				Sidebar _Sidebar = Components.Single<Sidebar>();
				_Sidebar.UnFocusGroup(Title);
			}
		}

		/// <summary>
		/// Selects the current group.
		/// </summary>
		public void SelectGroup()
		{
			Component.Base Components = _GlobalExplorer.Components;

			if (Components.HasAll(_GroupEditor.ToArray()))
			{
				Editor Editor = Components.Single<Editor>();
				Editor.GroupPath = Title;
			}

			if (Components.HasAll(_RootSidebar.ToArray()))
			{
				Sidebar _Sidebar = Components.Single<Sidebar>();
				_Sidebar.FocusGroup(Title);
			}
		}

		/// <summary>
		/// Focuses the listing entry.
		/// </summary>
		public void Focus()
		{
			_PanelContainer.Theme = _ActiveTheme;
		}

		/// <summary>
		/// Unfocuses the listing entry.
		/// </summary>
		public void Unfocus()
		{
			_PanelContainer.Theme = null;
		}

		/// <summary>
		/// Sets up the title.
		/// </summary>
		private void _SetupTitle()
		{
			TitleLabel = new()
			{
				Text = _FormatTitle(Title),
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			Row.AddChild(TitleLabel);
		}

		/// <summary>
		/// Sets up the remove title.
		/// </summary>
		private void _SetupRemoveTitle()
		{
			MarginContainer _RemoveTitleContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			};
			_RemoveTitleContainer.AddThemeConstantOverride("margin_left", 0);
			_RemoveTitleContainer.AddThemeConstantOverride("margin_right", 0);
			_RemoveTitleContainer.AddThemeConstantOverride("margin_top", 2);
			_RemoveTitleContainer.AddThemeConstantOverride("margin_bottom", 2);

			RemoveLabel = new()
			{
				Text = "Are you sure you wish to continue?",
				ThemeTypeVariation = "HeaderSmall",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			_RemoveTitleContainer.AddChild(RemoveLabel);
			RemoveRow.AddChild(_RemoveTitleContainer);
		}

		/// <summary>
		/// Sets up the buttons.
		/// </summary>
		private void _SetupButtons()
		{
			InnerRow = new();

			_SetupRemoveButton();
			_SetupEditButton();

			Row.AddChild(InnerRow);
		}

		/// <summary>
		/// Sets up the remove buttons.
		/// </summary>
		private void _SetupRemoveButtons()
		{
			RemoveInnerRow = new();

			_SetupCancelButton();
			_SetupConfirmButton();

			RemoveRow.AddChild(RemoveInnerRow);
		}

		/// <summary>
		/// Formats the title.
		/// </summary>
		/// <param name="title">The title to format.</param>
		/// <returns>The formatted title.</returns>
		private string _FormatTitle(string title)
		{
			string filename = StringHelper.FilePathToFileName(title);
			string _newTitle = StringHelper.FileNameToTitle(filename);

			return _newTitle;
		}

		/// <summary>
		/// Sets up the edit button.
		/// </summary>
		private void _SetupEditButton()
		{
			Edit = new()
			{
				Text = "Select",
				TooltipText = "Selects this group for editing",
				ThemeTypeVariation = "EditButtonSmall",
				Flat = true,
				MouseDefaultCursorShape = Control.CursorShape.PointingHand,
			};

			Edit.Connect(Button.SignalName.Pressed, Callable.From(() => { _OnSelectGroup(); }));

			InnerRow.AddChild(Edit);
		}

		/// <summary>
		/// Sets up the remove button.
		/// </summary>
		private void _SetupRemoveButton()
		{
			Remove = new()
			{
				Text = "Remove",
				TooltipText = "Removes this group",
				ThemeTypeVariation = "EditButtonSmall",
				Flat = true,
				MouseDefaultCursorShape = Control.CursorShape.PointingHand,
			};

			Remove.Connect(Button.SignalName.Pressed, Callable.From(() => { _OnRemoveGroup(); }));

			InnerRow.AddChild(Remove);
		}

		/// <summary>
		/// Sets up the confirm button.
		/// </summary>
		private void _SetupConfirmButton()
		{
			Confirm = new()
			{
				Text = "Confirm",
				TooltipText = "Confirms the deletion of the group",
				ThemeTypeVariation = "EditButtonSmall",
				Flat = true,
				MouseDefaultCursorShape = Control.CursorShape.PointingHand,
			};

			Confirm.Connect(Button.SignalName.Pressed, Callable.From(() => { _OnConfirmRemoveGroup(); }));

			RemoveInnerRow.AddChild(Confirm);
		}

		/// <summary>
		/// Sets up the cancel button.
		/// </summary>
		private void _SetupCancelButton()
		{
			Cancel = new()
			{
				Text = "Cancel",
				TooltipText = "Cancels the remove of the group",
				ThemeTypeVariation = "EditButtonSmall",
				Flat = true,
				MouseDefaultCursorShape = Control.CursorShape.PointingHand,
			};

			Cancel.Connect(Button.SignalName.Pressed, Callable.From(() => { _OnCancelRemoveGroup(); }));

			RemoveInnerRow.AddChild(Cancel);
		}

		/// <summary>
		/// Handles the removal of the group.
		/// </summary>
		private void _OnRemoveGroup()
		{
			Row.Visible = false;
			RemoveRow.Visible = true;
		}

		/// <summary>
		/// Handles the cancellation of the group removal.
		/// </summary>
		private void _OnCancelRemoveGroup()
		{
			Row.Visible = true;
			RemoveRow.Visible = false;
		}

		/// <summary>
		/// Handles the confirmation of the group removal.
		/// </summary>
		private void _OnConfirmRemoveGroup()
		{
			Component.Base Components = ExplorerUtils.Get().Components;
			if (Components.HasAll(_RootSidebar.ToArray()))
			{
				Sidebar _Sidebar = Components.Single<Sidebar>();
				_Sidebar.RemoveGroup(Title);
				_Sidebar.RefreshExistingGroups();
			}
		}

		/// <summary>
		/// Handles the selection of the group.
		/// </summary>
		private void _OnSelectGroup()
		{
			Component.Base Components = ExplorerUtils.Get().Components;

			if (Components.HasAll(_GroupEditor.ToArray()))
			{
				Editor Editor = Components.Single<Editor>();
				Editor.GroupPath = Title;


				StatesUtils.Get().Group = Editor.Group;
				StatesUtils.Get().PlacingMode = GlobalStates.PlacingModeEnum.Group;
				StatesUtils.Get().EditingTitle = null;
				StatesUtils.Get().EditingObject = null;
				StatesUtils.Get().CurrentLibrary = null;
			}

			if (Components.HasAll(_RootSidebar.ToArray()))
			{
				Sidebar _Sidebar = Components.Single<Sidebar>();
				_Sidebar.Update();
			}
		}
	}
}

#endif