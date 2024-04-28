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
	using Godot;

	[Tool]
	public partial class EditorTopbar : LibraryComponent
	{

		public EditorTopbar()
		{
			Name = "GroupBuilderEditorTopbar";
			//_include = false;
		}

		[Export]
		public EditorTitleInput TitleInput { set; get; }
		[Export]
		public EditorPlace PlaceButton { set; get; }
		[Export]
		public EditorSave SaveButton { set; get; }
		[Export]
		public EditorClose CloseButton { set; get; }

		private MarginContainer _MarginContainer;
		private HBoxContainer _BoxContainer;
		private MarginContainer _InnerMarginContainer;
		private HBoxContainer _InnerBoxContainer;
		private MarginContainer totalMarginContainer;
		private Label _TotalItems;

		public override void Initialize()
		{
			base.Initialize();

			Initiated = true;

			_InitializeFields();

			_MarginContainer.AddThemeConstantOverride("margin_left", 15);
			_MarginContainer.AddThemeConstantOverride("margin_right", 15);
			_MarginContainer.AddThemeConstantOverride("margin_top", 10);
			_MarginContainer.AddThemeConstantOverride("margin_bottom", 0);

			_SetupGroupTitle(_BoxContainer);

			_SetupCloseButton(_InnerBoxContainer);
			_SetupPlaceButton(_InnerBoxContainer);

			_InnerMarginContainer.AddChild(_InnerBoxContainer);
			_BoxContainer.AddChild(_InnerMarginContainer);
			_MarginContainer.AddChild(_BoxContainer);

			AddChild(_MarginContainer);
		}

		public void UpdateTotalItems(int items)
		{
			if (null != totalMarginContainer && null != _TotalItems && 0 != items)
			{
				_TotalItems.Text = "Total items in group: " + items;
				totalMarginContainer.Visible = true;
			}
			else if (null != totalMarginContainer && null != _TotalItems)
			{
				totalMarginContainer.Visible = false;
			}
		}

		public void Update()
		{
			if (_GlobalExplorer.GroupBuilder._Editor.Group == null || false == IsInstanceValid(_GlobalExplorer.GroupBuilder._Editor.Group))
			{
				if (IsInstanceValid(SaveButton))
				{
					SaveButton.DoHide();
				}
				if (IsInstanceValid(CloseButton))
				{
					CloseButton.DoHide();
				}
				if (IsInstanceValid(PlaceButton))
				{
					PlaceButton.DoHide();
				}
				if (IsInstanceValid(TitleInput))
				{
					TitleInput.Update();
				}

				UpdateTotalItems(0);

				return;
			}

			if (IsInstanceValid(SaveButton))
			{
				SaveButton.DoShow();
			}
			if (IsInstanceValid(CloseButton))
			{
				CloseButton.DoShow();
			}
			if (IsInstanceValid(PlaceButton))
			{
				PlaceButton.DoShow();
			}
			if (IsInstanceValid(TitleInput))
			{
				TitleInput.Update();
			}

			UpdateTotalItems(_GlobalExplorer.GroupBuilder._Editor.Group._Paths.Count);
		}

		public string GetTitle()
		{
			return TitleInput._InputField.Text;
		}

		public bool TitleEquals(string Name)
		{
			return Name == TitleInput._InputField.Text;
		}

		private void _InitializeFields()
		{
			_MarginContainer = new();
			_BoxContainer = new();
			_InnerMarginContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};
			_InnerBoxContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ShrinkEnd
			};
		}


		private void _SetupGroupTitle(HBoxContainer container)
		{
			List<string> Components = new()
			{
				"Groups.Builder.EditorSave",
				"Groups.Builder.EditorTitleInput",
			};

			HBoxContainer hBoxContainer = new()
			{
				Name = "GroupTitleContainer"
			};

			totalMarginContainer = new()
			{
				Visible = false,
			};
			totalMarginContainer.AddThemeConstantOverride("margin_right", 200);
			totalMarginContainer.AddThemeConstantOverride("margin_top", 2);
			HBoxContainer totalInnerContainer = new();

			_TotalItems = new()
			{
				Text = "Total items in group: " + 0,
				ThemeTypeVariation = Labelable.TitleType.TextSmall.ToString(),
			};

			Label _label = new()
			{
				Text = "Active Group",
				ThemeTypeVariation = "HeaderMedium",
			};

			totalInnerContainer.AddChild(_TotalItems);
			totalMarginContainer.AddChild(totalInnerContainer);
			hBoxContainer.AddChild(totalMarginContainer);

			MarginContainer MarginContainer = new() { };
			// MarginContainer.AddThemeConstantOverride("margin_top", 10);
			HBoxContainer InnerContainer = new();

			InnerContainer.AddChild(_label);

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				TitleInput = _GlobalExplorer.Components.Single<EditorTitleInput>();
				TitleInput.Container = InnerContainer;
				TitleInput.Initialize();

				SaveButton = _GlobalExplorer.Components.Single<EditorSave>();
				SaveButton.Container = InnerContainer;
				SaveButton.Initialize();
			}

			MarginContainer.AddChild(InnerContainer);
			hBoxContainer.AddChild(MarginContainer);

			container.AddChild(hBoxContainer);
		}

		private void _SetupCloseButton(HBoxContainer container)
		{
			List<string> Components = new()
			{
				"Groups.Builder.EditorClose",
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				CloseButton = _GlobalExplorer.Components.Single<EditorClose>();
				CloseButton.Container = container;
				CloseButton.Initialize();
			}
		}

		private void _SetupPlaceButton(HBoxContainer container)
		{
			List<string> Components = new()
			{
				"Groups.Builder.EditorPlace",
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				PlaceButton = _GlobalExplorer.Components.Single<EditorPlace>();
				PlaceButton.Container = container;
				PlaceButton.Initialize();
			}
		}
	}
}