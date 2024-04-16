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

namespace AssetSnap.Front.Components.Groups.Builder
{
	using System.Collections.Generic;
	using AssetSnap.Component;
	using AssetSnap.Explorer;
	using AssetSnap.Helpers;
	using Godot;

	[Tool]
	public partial class ListingEntry : LibraryComponent
	{
		public ListingEntry()
		{
			Name = "GroupBuilderListingEntry";
			//_include = false;
		}
		
		public string title { get; set; }
		
		public HBoxContainer RemoveRow;
		public HBoxContainer RemoveInnerRow;
	
		public MarginContainer OuterMarginContainer;
		public MarginContainer InnerMarginContainer;
		public PanelContainer _PanelContainer;
		public HBoxContainer Row;
		public Label Title;
		public HBoxContainer InnerRow;
		public Button Edit;
		public Button Remove;
		public Button Confirm;
		public Button Cancel;
		
		private readonly Theme ActiveTheme = GD.Load<Theme>("res://addons/assetsnap/assets/themes/SnapThemeActiveGroup.tres");
		
		private readonly List<string> RootSidebar = new()
		{
			"Groups.Builder.Sidebar",
		};
		
		private readonly List<string> GroupEditor = new()
		{
			"Groups.Builder.Editor",
		};
		
		public override void Initialize()
		{
			base.Initialize();
		
			Initiated = true;
			
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
		
		public void DeselectGroup()
		{
			Component.Base Components = _GlobalExplorer.Components;
			
			if (Components.HasAll(GroupEditor.ToArray()))
			{
				Editor Editor = Components.Single<Editor>();
				Editor.GroupPath = null;
			}
			
			if ( Components.HasAll( RootSidebar.ToArray() )) 
			{
				Sidebar _Sidebar = Components.Single<Sidebar>();
				_Sidebar.UnFocusGroup(title);
			}
		}
		
		public void SelectGroup() 
		{
			Component.Base Components = _GlobalExplorer.Components;
			
			if (Components.HasAll(GroupEditor.ToArray()))
			{
				Editor Editor = Components.Single<Editor>();
				Editor.GroupPath = title;
			}
			
			if ( Components.HasAll( RootSidebar.ToArray() )) 
			{
				Sidebar _Sidebar = Components.Single<Sidebar>();
				_Sidebar.FocusGroup(title);
			}
		}
		
		public void Focus()
		{
			_PanelContainer.Theme = ActiveTheme;
		}
		
		public void Unfocus()
		{
			_PanelContainer.Theme = null;
		}
		
		private void _SetupTitle()
		{
			Title = new()
			{
				Text = _FormatTitle(title),
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			Row.AddChild(Title);
		}
			
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
			
			Title = new()
			{
				Text = "Are you sure you wish to continue?",
				ThemeTypeVariation = "HeaderSmall",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			_RemoveTitleContainer.AddChild(Title);
			RemoveRow.AddChild(_RemoveTitleContainer);
		}
		
		private void _SetupButtons()
		{
			InnerRow = new();

			_SetupRemoveButton();
			_SetupEditButton();
			
			Row.AddChild(InnerRow);
		}
		
		private void _SetupRemoveButtons()
		{
			RemoveInnerRow = new();

			_SetupCancelButton();
			_SetupConfirmButton();
			
			RemoveRow.AddChild(RemoveInnerRow);
		}
		
		private string _FormatTitle(string title )
		{
			string filename = StringHelper.FilePathToFileName(title);
			string _newTitle = StringHelper.FileNameToTitle(filename);

			return _newTitle;
		}
		
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
		
		private void _OnRemoveGroup()
		{
			Row.Visible = false;
			RemoveRow.Visible = true;
		}
		
		private void _OnCancelRemoveGroup()
		{
			Row.Visible = true;
			RemoveRow.Visible = false;
		}
			
		private void _OnConfirmRemoveGroup()
		{
			Component.Base Components = ExplorerUtils.Get().Components;
			if ( Components.HasAll( RootSidebar.ToArray() )) 
			{
				Sidebar _Sidebar = Components.Single<Sidebar>();
				_Sidebar.RemoveGroup(title);
				_Sidebar.RefreshExistingGroups();
			}
		}
		
		private void _OnSelectGroup()
		{
			Component.Base Components = ExplorerUtils.Get().Components;

			if (Components.HasAll(GroupEditor.ToArray()))
			{
				Editor Editor = Components.Single<Editor>();
				Editor.GroupPath = title;
			}
			
			if ( Components.HasAll( RootSidebar.ToArray() )) 
			{
				Sidebar _Sidebar = Components.Single<Sidebar>();
				_Sidebar.Update();
			}
		}
	}
}