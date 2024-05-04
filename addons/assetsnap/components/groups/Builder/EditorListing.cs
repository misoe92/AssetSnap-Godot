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
using Godot;

namespace AssetSnap.Front.Components.Groups.Builder
{
	/// <summary>
	/// Partial class representing an editor listing for the GroupBuilder component.
	/// </summary>
	[Tool]
	public partial class EditorListing : LibraryComponent
	{
		/// <summary>
		/// Constructor for EditorListing class.
		/// </summary>
		public EditorListing()
		{
			Name = "GroupBuilderEditorListing";

			UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
				{ typeof(Labelable).ToString() },
				{ typeof(Buttonable).ToString() },
				{ typeof(Panelable).ToString() },
				{ typeof(ScrollContainerable).ToString() },
			};

			//_include = false;
		}

		/// <summary>
		/// Initializes the EditorListing component.
		/// </summary>
		public override void Initialize()
		{
			SizeFlagsVertical = Control.SizeFlags.ExpandFill;
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

			if (Initiated == true)
			{
				return;
			}

			base.Initialize();

			Initiated = true;

			_InitializeFields();

			if (null != _GlobalExplorer.GroupBuilder._Editor.Group)
			{
				if (_GlobalExplorer.GroupBuilder._Editor.Group._Paths.Count == 0)
				{
					_SetupNoFound();
					HideTopbar();
				}
				else
				{
					ShowTopbar();
					_SetupGroupObjects();
				}
			}
			else
			{
				HideTopbar();
				_SetupNoGroup();
			}

			_FinalizeFields();
		}
		
		/// <summary>
		/// Hides the EditorListing component.
		/// </summary>		
		public void DoHide()
		{
			Visible = false;
		}

		/// <summary>
		/// Shows the EditorListing component.
		/// </summary>
		public void DoShow()
		{
			Visible = true;
		}

		/// <summary>
		/// Hides the top bar UI element.
		/// </summary>
		public void HideTopbar()
		{
			Trait<Containerable>()
				.Select(1)
				.SetVisible(false);
		}

		/// <summary>
		/// Shows the top bar UI element.
		/// </summary>
		public void ShowTopbar()
		{
			Trait<Containerable>()
				.Select(1)
				.SetVisible(true);
		}

		/// <summary>
		/// Updates the EditorListing component.
		/// </summary>
		public void Update()
		{
			if (null != ExplorerUtils.Get().GroupBuilder._Editor.Group)
			{
				if (ExplorerUtils.Get().GroupBuilder._Editor.Group._Paths.Count == 0)
				{
					HideTopbar();
					_SetupNoFound();
				}
				else
				{
					ShowTopbar();
					_SetupGroupObjects();
				}
			}
			else
			{
				HideTopbar();
				_SetupNoGroup();
			}
		}

		/// <summary>
		/// Resets the EditorListing component.
		/// </summary>
		public void Reset()
		{
			Godot.Container container = Trait<Containerable>()
				.Select(2)
				.GetInnerContainer();

			foreach (Node child in container.GetChildren())
			{
				container.RemoveChild(child);
				child.Free();
			}
		}

		/// <summary>
		/// Initializes the fields of the EditorListing component.
		/// </summary>
		private void _InitializeFields()
		{
			Trait<Panelable>()
				.SetName("GroupBuilderEditorListingMargin")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetMargin(5, "top")
				.SetMargin(5, "bottom")
				.SetMargin(15, "left")
				.SetMargin(15, "right")
				.SetPadding(10, "top")
				.SetPadding(10, "bottom")
				.SetPadding(15, "left")
				.SetPadding(15, "right")
				.SetVisible(true)
				.Instantiate();

			Trait<ScrollContainerable>()
				.SetName("GroupBuilderEditorListingScroll")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.Instantiate();

			Trait<Containerable>()
				.SetName("GroupBuilderEditorListingContainer")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetLayout(Containerable.ContainerLayout.OneColumn)
				.Instantiate();

			Trait<Containerable>()
				.SetName("GroupBuilderEditorListingInnerContainer")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetInnerOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetLayout(Containerable.ContainerLayout.TwoColumns)
				.Instantiate();

			Trait<Containerable>()
				.SetName("GroupBuilderEditorListingFinalContainer")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetMargin(15, "left")
				.SetMargin(15, "right")
				.SetMargin(10, "bottom")
				.SetLayout(Containerable.ContainerLayout.OneColumn)
				.Instantiate();

			Trait<Containerable>()
				.SetName("GroupBuilderEditorListingFinalButtonContainer")
				.SetMargin(10, "top")
				.SetMargin(0, "bottom")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetLayout(Containerable.ContainerLayout.OneColumn)
				.Instantiate();

			Trait<Labelable>()
				.SetName("GroupBuilderEditorListingDescription")
				.SetType(Labelable.TitleType.TextMedium)
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetMargin(10, "bottom")
				.SetText("Below objects are currently bound to the group. You can add more from the library at anytime")
				.Instantiate();

			Trait<Buttonable>()
				.SetName("GroupBuilderEditorListingAdvancedOptions")
				.SetMargin(10, "top")
				.SetMargin(10, "bottom")
				.SetType(Buttonable.ButtonType.SmallFlatButton)
				.SetText("Advanced Group Options")
				.SetTooltipText("Advanced options all group objects will default to")
				.SetCursorShape(Control.CursorShape.PointingHand)
				.SetIcon(GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/settings.svg"))
				.SetIconAlignment(HorizontalAlignment.Right)
				.SetAction(() => { _OnOpenGroupOptions(); })
				.Instantiate();
		}

		/// <summary>
		/// Finalizes the fields of the EditorListing component.
		/// </summary>
		private void _FinalizeFields()
		{
			Godot.Container containerOne = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer();

			Godot.Container containerTwoOne = Trait<Containerable>()
				.Select(1)
				.GetInnerContainer(0);

			Godot.Container containerTwoTwo = Trait<Containerable>()
				.Select(1)
				.GetInnerContainer(1);

			containerTwoTwo.SizeFlagsHorizontal = Control.SizeFlags.ShrinkEnd;

			Trait<Labelable>()
				.Select(0)
				.AddToContainer(
					containerTwoOne
				);

			Trait<Buttonable>()
				.Select(0)
				.AddToContainer(
					Trait<Containerable>()
						.Select(3)
						.GetInnerContainer()
				);

			Trait<Containerable>()
				.Select(3)
				.AddToContainer(
					containerTwoTwo
				);

			Trait<Containerable>()
				.Select(2)
				.AddToContainer(
					Trait<ScrollContainerable>()
						.Select(0)
						.GetScrollContainer()
				);

			Trait<Containerable>()
				.Select(1)
				.AddToContainer(
					containerOne
				);

			Trait<ScrollContainerable>()
				.Select(0)
				.AddToContainer(
					containerOne
				);

			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					Trait<Panelable>()
						.Select(0)
						.GetNode()
				);

			Trait<Panelable>()
				.Select(0)
				.AddToContainer(
					this
				);
		}

		/// <summary>
		/// Sets up the group objects for the EditorListing component.
		/// </summary>
		private void _SetupGroupObjects()
		{
			List<string> Components = new()
			{
				"Groups.Builder.EditorGroupObject",
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				for (int i = 0; i < _GlobalExplorer.GroupBuilder._Editor.Group._Paths.Count; i++)
				{
					string path = _GlobalExplorer.GroupBuilder._Editor.Group._Paths[i];
					Vector3 origin = _GlobalExplorer.GroupBuilder._Editor.Group._Origins[i];
					Vector3 scale = _GlobalExplorer.GroupBuilder._Editor.Group._Scales[i];
					Vector3 rotation = _GlobalExplorer.GroupBuilder._Editor.Group._Rotations[i];

					if (false == _GlobalExplorer.GroupBuilder._Editor.Group._Options.Count > i)
					{
						_GlobalExplorer.GroupBuilder._Editor.Group._Options.Add(new());
					}

					Godot.Collections.Dictionary<string, Variant> options = _GlobalExplorer.GroupBuilder._Editor.Group._Options[i];

					EditorGroupObject SingleEntry = GlobalExplorer.GetInstance().Components.Single<EditorGroupObject>(true);

					SingleEntry.Path = path;
					SingleEntry.Origin = origin;
					SingleEntry.ObjectScale = scale;
					SingleEntry.ObjectRotation = rotation;
					SingleEntry.Options = options;
					SingleEntry.Index = i;

					SingleEntry.Initialize();
					Trait<Containerable>()
						.Select(2)
						.GetInnerContainer()
						.AddChild(SingleEntry);
				}
			}
		}

		/// <summary>
		/// Sets up the UI when no objects are found.
		/// </summary>
		private void _SetupNoFound()
		{
			MarginContainer NoFoundMargin = new();

			NoFoundMargin.AddThemeConstantOverride("margin_left", 5);
			NoFoundMargin.AddThemeConstantOverride("margin_right", 5);
			NoFoundMargin.AddThemeConstantOverride("margin_top", 10);
			NoFoundMargin.AddThemeConstantOverride("margin_bottom", 10);

			Label NoFoundTitle = new()
			{
				Text = "No objects was found, please go to a library to add objects to the group",
				SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};

			NoFoundMargin.AddChild(NoFoundTitle);
			Trait<Containerable>().Select(2).GetInnerContainer().AddChild(NoFoundMargin);
		}

		/// <summary>
		/// Sets up the UI when no group is chosen.
		/// </summary>
		private void _SetupNoGroup()
		{
			MarginContainer NoFoundMargin = new();

			NoFoundMargin.AddThemeConstantOverride("margin_left", 5);
			NoFoundMargin.AddThemeConstantOverride("margin_right", 5);
			NoFoundMargin.AddThemeConstantOverride("margin_top", 10);
			NoFoundMargin.AddThemeConstantOverride("margin_bottom", 10);

			Label NoFoundTitle = new()
			{
				Text = "No group is chosen, please select a group to edit or place it",
				SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};

			NoFoundMargin.AddChild(NoFoundTitle);

			Trait<Containerable>()
				.Select(2)
				.GetInnerContainer()
				.AddChild(NoFoundMargin);
		}

		/// <summary>
        /// Event handler for opening group options.
        /// </summary>
		private void _OnOpenGroupOptions()
		{
			_GlobalExplorer.GroupBuilder._Editor.GroupOptions.DoShow();
			DoHide();
		}
	}
}

#endif