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
using AssetSnap.Front.Components.Groups.Builder.GroupOptions;
using AssetSnap.Instance.Input;
using AssetSnap.States;
using Godot;

namespace AssetSnap.Front.Components.Groups.Builder
{
	/// <summary>
	/// Provides options for editing a group in the editor.
	/// </summary>
	[Tool]
	public partial class EditorGroupOptions : LibraryComponent
	{
		/** Option Values **/
		private SnapToObject _GroupBuilderEditorGroupOptionSnapToObject;
		private SnapLayer _GroupBuilderEditorGroupOptionSnapLayer;
		private SnapToObjectOffsetX _GroupBuilderEditorGroupOptionSnapToObjectOffsetX;
		private SnapToObjectOffsetZ _GroupBuilderEditorGroupOptionSnapToObjectOffsetZ;
		private SnapToHeight _GroupBuilderEditorGroupOptionSnapToHeight;
		private SnapToHeightValue _GroupBuilderEditorGroupOptionSnapToHeightValue;
		private SnapToX _GroupBuilderEditorGroupOptionSnapToX;
		private SnapToXValue _GroupBuilderEditorGroupOptionSnapToXValue;
		private SnapToZ _GroupBuilderEditorGroupOptionSnapToZ;
		private SnapToZValue _GroupBuilderEditorGroupOptionSnapToZValue;
		private SphereCollision _GroupBuilderEditorGroupOptionSphereCollision;
		private ConcaveCollision _GroupBuilderEditorGroupOptionConcaveCollision;
		private ConvexCollision _GroupBuilderEditorGroupOptionConvexCollision;
		private ConvexClean _GroupBuilderEditorGroupOptionConvexClean;
		private ConvexSimplify _GroupBuilderEditorGroupOptionConvexSimplify;
		private DragOffset _GroupBuilderEditorGroupOptionDragOffset;
		private LevelOfDetailsState _GroupBuilderEditorGroupOptionLevelOfDetailsState;
		private LevelOfDetails _GroupBuilderEditorGroupOptionLevelOfDetails;
		private PlacementSimple _GroupBuilderEditorGroupOptionPlacementSimple;
		private PlacementOptimized _GroupBuilderEditorGroupOptionPlacementOptimized;
		private VisibilityBegin _GroupBuilderEditorGroupOptionVisibilityBegin;
		private VisibilityBeginMargin _GroupBuilderEditorGroupOptionVisibilityBeginMargin;
		private VisibilityEnd _GroupBuilderEditorGroupOptionVisibilityEnd;
		private VisibilityEndMargin _GroupBuilderEditorGroupOptionVisibilityEndMargin;
		private VisibilityFadeMode _GroupBuilderEditorGroupOptionFadeMode;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="EditorGroupOptions"/> class.
		/// </summary>
		public EditorGroupOptions()
		{
			Name = "GroupBuilderEditorGroupOptions";

			_UsingTraits = new()
			{
				{ typeof(Labelable).ToString() },
				{ typeof(Buttonable).ToString() },
				{ typeof(ScrollContainerable).ToString() },
				{ typeof(Containerable).ToString() },
				{ typeof(Panelable).ToString() },
			};

			//_include = false;
		}

		/// <summary>
		/// Initializes the editor group options.
		/// </summary>
		public override void Initialize()
		{
			if (_Initiated)
			{
				return;
			}

			base.Initialize();
			_Initiated = true;
			Visible = false;

			SizeFlagsHorizontal = SizeFlags.ExpandFill;
			SizeFlagsVertical = SizeFlags.ExpandFill;

			_InitializeFields();
			_InitializeComponents();

			_FinalizeFields();
		}

		/// <summary>
		/// Initializes the fields for the editor group options.
		/// </summary>
		public void _InitializeFields()
		{
			Trait<Panelable>()
				.SetName("GroupBuilderEditorGroupOptionsPanel")
				.SetType(Panelable.PanelType.LightPanelContainer)
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
				.Instantiate();

			Trait<ScrollContainerable>()
				.SetName("GroupBuilderEditorGroupOptionsScroll")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.Instantiate();

			Trait<Containerable>()
				.SetName("GroupBuilderEditorGroupOptionsOuterContainer")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetOrientation(Containerable.ContainerOrientation.Vertical)
				.SetLayout(Containerable.ContainerLayout.OneColumn)
				.Instantiate();

			Trait<Containerable>()
				.SetName("GroupBuilderEditorGroupOptionsInnerContainer")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetOrientation(Containerable.ContainerOrientation.Vertical)
				.SetInnerOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetLayout(Containerable.ContainerLayout.TwoColumns)
				.Instantiate();

			Trait<Containerable>()
				.SetName("GroupBuilderEditorGroupOptionsFinalContainer")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetMargin(15, "left")
				.SetMargin(15, "right")
				.SetMargin(10, "bottom")
				.SetMargin(10, "top")
				.SetOrientation(Containerable.ContainerOrientation.Vertical)
				.SetInnerOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetLayout(Containerable.ContainerLayout.ThreeColumns)
				.Instantiate();

			Trait<Containerable>()
				.SetName("GroupBuilderEditorGroupOptionsInnerButtonContainer")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetLayout(Containerable.ContainerLayout.OneColumn)
				.Instantiate();

			Trait<Labelable>()
				.SetName("GroupBuilderEditorGroupOptionsTitle")
				.SetType(Labelable.TitleType.HeaderMedium)
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetMargin(0, "bottom")
				.SetText("Group Options")
				.Instantiate();

			Trait<Buttonable>()
				.SetName("GroupBuilderEditorGroupOptionsClose")
				.SetMargin(10, "top")
				.SetType(Buttonable.ButtonType.SmallFlatButton)
				.SetText("Close Group Options")
				.SetTooltipText("Returns you back to the group editor")
				.SetCursorShape(Control.CursorShape.PointingHand)
				.SetIcon(GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/close.svg"))
				.SetIconAlignment(HorizontalAlignment.Right)
				.SetAction(() => { _OnCloseOptionsPressed(); })
				.Instantiate();

			Trait<Labelable>()
				.SetName("GroupBuilderEditorGroupOptionsDescription")
				.SetType(Labelable.TitleType.HeaderSmall)
				.SetText("The options below will be applied when the group is placed on all child objects unless specific options has been set for the object.")
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetAutoWrap(TextServer.AutowrapMode.Word)
				.SetMargin(0, "top")
				.SetDimensions(500, 0)
				.Instantiate();
		}
		
		/// <summary>
		/// Shows the control.
		/// </summary>
		public void DoShow()
		{
			Visible = true;
		}

		/// <summary>
		/// Hides the control.
		/// </summary>
		public void DoHide()
		{
			Visible = false;
		}

		/// <summary>
		/// Updates the specified group option.
		/// </summary>
		/// <param name="Name">The name of the option.</param>
		/// <param name="value">The value of the option.</param>
		public void _UpdateGroupOption(string Name, Variant value)
		{
			if (true == _GroupBuilderEditorGroupOptionSnapToObject.GetValue())
			{
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.InputShow();
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.InputShow();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.InputHide();
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.InputHide();
			}

			if (true == _GroupBuilderEditorGroupOptionSnapToHeight.GetValue())
			{
				_GroupBuilderEditorGroupOptionSnapToHeightValue.InputShow();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToHeightValue.InputHide();
			}

			if (true == _GroupBuilderEditorGroupOptionSnapToX.GetValue())
			{
				_GroupBuilderEditorGroupOptionSnapToXValue.InputShow();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToXValue.InputHide();
			}

			if (true == _GroupBuilderEditorGroupOptionSnapToZ.GetValue())
			{
				_GroupBuilderEditorGroupOptionSnapToZValue.InputShow();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToZValue.InputHide();
			}

			if (true == _GroupBuilderEditorGroupOptionConvexCollision.GetValue())
			{
				_GroupBuilderEditorGroupOptionConvexClean.InputShow();
				_GroupBuilderEditorGroupOptionConvexSimplify.InputShow();
			}
			else
			{
				_GroupBuilderEditorGroupOptionConvexClean.InputHide();
				_GroupBuilderEditorGroupOptionConvexSimplify.InputHide();
			}
		}

		/// <summary>
		/// Updates all group options.
		/// </summary>
		public void _UpdateGroupOptions()
		{
			if (null == StatesUtils.Get().Group)
			{
				return;
			}

			if (false == _Initiated)
			{
				GD.PushWarning("Group options object is not initialized");
				return;
			}

			_GroupBuilderEditorGroupOptionSnapToObject.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapToObject);
			_GroupBuilderEditorGroupOptionSnapLayer.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapLayer);
			_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapToObjectOffsetXValue);
			_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapToObjectOffsetZValue);
			_GroupBuilderEditorGroupOptionSnapToHeight.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapToHeight);
			_GroupBuilderEditorGroupOptionSnapToHeightValue.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapToHeightValue);
			_GroupBuilderEditorGroupOptionSnapToX.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapToX);
			_GroupBuilderEditorGroupOptionSnapToXValue.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapToXValue);
			_GroupBuilderEditorGroupOptionSnapToZ.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapToZ);
			_GroupBuilderEditorGroupOptionSnapToZValue.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapToZValue);
			_GroupBuilderEditorGroupOptionSphereCollision.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SphereCollision);
			_GroupBuilderEditorGroupOptionConcaveCollision.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.ConcaveCollision);
			_GroupBuilderEditorGroupOptionConvexCollision.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.ConvexCollision);
			_GroupBuilderEditorGroupOptionConvexClean.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.ConvexClean);
			_GroupBuilderEditorGroupOptionConvexSimplify.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.ConvexSimplify);
			_GroupBuilderEditorGroupOptionPlacementSimple.SetValue(StatesUtils.Get().PlacingType == GlobalStates.PlacingTypeEnum.Simple);
			_GroupBuilderEditorGroupOptionPlacementOptimized.SetValue(StatesUtils.Get().PlacingType == GlobalStates.PlacingTypeEnum.Optimized);
			_GroupBuilderEditorGroupOptionDragOffset.SetValue(DragAddInputDriver.GetInstance().SizeOffset);

			if (true == _GroupBuilderEditorGroupOptionSnapToObject.GetValue())
			{
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.InputShow();
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.InputShow();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.InputHide();
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.InputHide();
			}

			if (true == _GroupBuilderEditorGroupOptionSnapToHeight.GetValue())
			{
				_GroupBuilderEditorGroupOptionSnapToHeightValue.InputShow();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToHeightValue.InputHide();
			}

			if (true == _GroupBuilderEditorGroupOptionSnapToX.GetValue())
			{
				_GroupBuilderEditorGroupOptionSnapToXValue.InputShow();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToXValue.InputHide();
			}

			if (true == _GroupBuilderEditorGroupOptionSnapToZ.GetValue())
			{
				_GroupBuilderEditorGroupOptionSnapToZValue.InputShow();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToZValue.InputHide();
			}

			if (true == _GroupBuilderEditorGroupOptionConvexCollision.GetValue())
			{
				_GroupBuilderEditorGroupOptionConvexClean.InputShow();
				_GroupBuilderEditorGroupOptionConvexSimplify.InputShow();
			}
			else
			{
				_GroupBuilderEditorGroupOptionConvexClean.InputHide();
				_GroupBuilderEditorGroupOptionConvexSimplify.InputHide();
			}
		}

		/// <summary>
		/// Removes the specified option instance.
		/// </summary>
		/// <param name="_object">The object to remove.</param>
		/// <param name="debug">Whether to enable debug mode.</param>
		private void _RemoveOptionInstance(GroupOptionComponent _object, bool debug = false)
		{
			if (
				EditorPlugin.IsInstanceValid(_object) &&
				_object.GetParent() == this
			)
			{
				_object.Clear(debug);
				RemoveChild(_object);
			}
		}

		/// <summary>
		/// Clears the specified option instance.
		/// </summary>
		/// <param name="_object">The object to clear.</param>
		/// <param name="debug">Whether to enable debug mode.</param>
		private void _ClearOptionInstance(GroupOptionComponent _object, bool debug = false)
		{
			if (
				EditorPlugin.IsInstanceValid(_object) &&
				_object.GetParent() == this
			)
			{
				_object.Free();
			}
		}

		/// <summary>
		/// Initializes the components for the editor group options.
		/// </summary>
		private void _InitializeComponents()
		{
			List<string> PlacementModesComponents = new()
			{
				"Groups.Builder.GroupOptions.PlacementSimple",
				"Groups.Builder.GroupOptions.PlacementOptimized",
			};

			List<string> SnaptoObjectComponents = new()
			{
				"Groups.Builder.GroupOptions.SnapToObject",
				"Groups.Builder.GroupOptions.SnapToObjectPosition",
				"Groups.Builder.GroupOptions.SnapLayer",
				"Groups.Builder.GroupOptions.SnapToObjectOffsetX",
				"Groups.Builder.GroupOptions.SnapToObjectOffsetZ",
			};

			List<string> SnapToPlaneComponents = new()
			{
				"Groups.Builder.GroupOptions.SnapToHeight",
				"Groups.Builder.GroupOptions.SnapToHeightValue",
				"Groups.Builder.GroupOptions.SnapToX",
				"Groups.Builder.GroupOptions.SnapToXValue",
				"Groups.Builder.GroupOptions.SnapToZ",
				"Groups.Builder.GroupOptions.SnapToZValue",
			};

			List<string> CollisionComponents = new()
			{
				"Groups.Builder.GroupOptions.SphereCollision",
				"Groups.Builder.GroupOptions.ConcaveCollision",
				"Groups.Builder.GroupOptions.ConvexCollision",
				"Groups.Builder.GroupOptions.ConvexClean",
				"Groups.Builder.GroupOptions.ConvexSimplify",
			};
			
			List<string> FadeModeComponents = new()
			{
				"Groups.Builder.GroupOptions.VisibilityFadeMode",
			};
			
			List<string> DragComponents = new()
			{
				"Groups.Builder.GroupOptions.DragOffset",
			};
			
			List<string> VisibilityComponents = new()
			{
				"Groups.Builder.GroupOptions.VisibilityBegin",
				"Groups.Builder.GroupOptions.VisibilityBeginMargin",
				"Groups.Builder.GroupOptions.VisibilityEnd",
				"Groups.Builder.GroupOptions.VisibilityEndMargin",
			};
			
			List<string> LODComponents = new()
			{
				"Groups.Builder.GroupOptions.LevelOfDetailsState",
				"Groups.Builder.GroupOptions.LevelOfDetails",
			};

			/** Initializing Snap Object fields **/
			_InitializeGroupOptionPlacementModeTitle(Trait<Containerable>().Select(2).GetInnerContainer(0));
			if (_GlobalExplorer.Components.HasAll(PlacementModesComponents.ToArray()))
			{
				_GroupBuilderEditorGroupOptionPlacementSimple = GlobalExplorer.GetInstance().Components.Single<PlacementSimple>(true);
				_GroupBuilderEditorGroupOptionPlacementOptimized = GlobalExplorer.GetInstance().Components.Single<PlacementOptimized>(true);

				if (null != _GroupBuilderEditorGroupOptionPlacementSimple)
				{
					_GroupBuilderEditorGroupOptionPlacementSimple.Parent = this;
					_GroupBuilderEditorGroupOptionPlacementSimple.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(0).AddChild(_GroupBuilderEditorGroupOptionPlacementSimple);

					_GroupBuilderEditorGroupOptionPlacementSimple.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				if (null != _GroupBuilderEditorGroupOptionPlacementOptimized)
				{
					_GroupBuilderEditorGroupOptionPlacementOptimized.Parent = this;
					_GroupBuilderEditorGroupOptionPlacementOptimized.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(0).AddChild(_GroupBuilderEditorGroupOptionPlacementOptimized);

					_GroupBuilderEditorGroupOptionPlacementOptimized.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
			}

			_InitializeGroupOptionSnapObjectTitle(Trait<Containerable>().Select(2).GetInnerContainer(1));
			if (_GlobalExplorer.Components.HasAll(SnaptoObjectComponents.ToArray()))
			{
				_GroupBuilderEditorGroupOptionSnapToObject = GlobalExplorer.GetInstance().Components.Single<SnapToObject>(true);
				_GroupBuilderEditorGroupOptionSnapLayer = GlobalExplorer.GetInstance().Components.Single<SnapLayer>(true);
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetX = GlobalExplorer.GetInstance().Components.Single<SnapToObjectOffsetX>(true);
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ = GlobalExplorer.GetInstance().Components.Single<SnapToObjectOffsetZ>(true);

				if (null != _GroupBuilderEditorGroupOptionSnapToObject)
				{
					_GroupBuilderEditorGroupOptionSnapToObject.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToObject.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(1).AddChild(_GroupBuilderEditorGroupOptionSnapToObject);

					_GroupBuilderEditorGroupOptionSnapToObject.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				if (null != _GroupBuilderEditorGroupOptionSnapToObjectOffsetX)
				{
					_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(1).AddChild(_GroupBuilderEditorGroupOptionSnapToObjectOffsetX);

					_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				if (null != _GroupBuilderEditorGroupOptionSnapToObjectOffsetZ)
				{
					_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(1).AddChild(_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ);

					_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				if (null != _GroupBuilderEditorGroupOptionSnapLayer)
				{
					_GroupBuilderEditorGroupOptionSnapLayer.Parent = this;
					_GroupBuilderEditorGroupOptionSnapLayer.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(1).AddChild(_GroupBuilderEditorGroupOptionSnapLayer);

					_GroupBuilderEditorGroupOptionSnapLayer.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				// _InitializeGroupOptionSnapObjectPositionTitle(Trait<Containerable>().Select(2).GetInnerContainer(1));
				// if( null != _GroupBuilderEditorGroupOptionSnapToObjectPosition) 
				// {
				// 	_GroupBuilderEditorGroupOptionSnapToObjectPosition.Container = Trait<Containerable>().Select(2).GetInnerContainer(1);
				// 	_GroupBuilderEditorGroupOptionSnapToObjectPosition.Parent = this;
				// 	_GroupBuilderEditorGroupOptionSnapToObjectPosition.Initialize(); 

				// 	_GroupBuilderEditorGroupOptionSnapToObjectPosition.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				// }
			}

			/** Initializing Snap Plane fields **/
			_InitializeGroupOptionSnapTitle(Trait<Containerable>().Select(2).GetInnerContainer(2));
			if (_GlobalExplorer.Components.HasAll(SnapToPlaneComponents.ToArray()))
			{
				_GroupBuilderEditorGroupOptionSnapToHeight = GlobalExplorer.GetInstance().Components.Single<SnapToHeight>(true);
				_GroupBuilderEditorGroupOptionSnapToHeightValue = GlobalExplorer.GetInstance().Components.Single<SnapToHeightValue>(true);
				_GroupBuilderEditorGroupOptionSnapToX = GlobalExplorer.GetInstance().Components.Single<SnapToX>(true);
				_GroupBuilderEditorGroupOptionSnapToXValue = GlobalExplorer.GetInstance().Components.Single<SnapToXValue>(true);
				_GroupBuilderEditorGroupOptionSnapToZ = GlobalExplorer.GetInstance().Components.Single<SnapToZ>(true);
				_GroupBuilderEditorGroupOptionSnapToZValue = GlobalExplorer.GetInstance().Components.Single<SnapToZValue>(true);

				if (null != _GroupBuilderEditorGroupOptionSnapToHeight)
				{
					_GroupBuilderEditorGroupOptionSnapToHeight.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToHeight.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(2).AddChild(_GroupBuilderEditorGroupOptionSnapToHeight);

					_GroupBuilderEditorGroupOptionSnapToHeight.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				if (null != _GroupBuilderEditorGroupOptionSnapToHeightValue)
				{
					_GroupBuilderEditorGroupOptionSnapToHeightValue.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToHeightValue.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(2).AddChild(_GroupBuilderEditorGroupOptionSnapToHeightValue);

					_GroupBuilderEditorGroupOptionSnapToHeightValue.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				if (null != _GroupBuilderEditorGroupOptionSnapToX)
				{
					_GroupBuilderEditorGroupOptionSnapToX.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToX.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(2).AddChild(_GroupBuilderEditorGroupOptionSnapToX);

					_GroupBuilderEditorGroupOptionSnapToX.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				if (null != _GroupBuilderEditorGroupOptionSnapToXValue)
				{
					_GroupBuilderEditorGroupOptionSnapToXValue.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToXValue.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(2).AddChild(_GroupBuilderEditorGroupOptionSnapToXValue);

					_GroupBuilderEditorGroupOptionSnapToXValue.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				if (null != _GroupBuilderEditorGroupOptionSnapToZ)
				{
					_GroupBuilderEditorGroupOptionSnapToZ.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToZ.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(2).AddChild(_GroupBuilderEditorGroupOptionSnapToZ);

					_GroupBuilderEditorGroupOptionSnapToZ.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				if (null != _GroupBuilderEditorGroupOptionSnapToZValue)
				{
					_GroupBuilderEditorGroupOptionSnapToZValue.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToZValue.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(2).AddChild(_GroupBuilderEditorGroupOptionSnapToZValue);

					_GroupBuilderEditorGroupOptionSnapToZValue.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
			}

			/** Initializing Snap Collisions fields **/
			_InitializeGroupOptionCollisionTitle(Trait<Containerable>().Select(2).GetInnerContainer(0));
			if (_GlobalExplorer.Components.HasAll(CollisionComponents.ToArray()))
			{
				_GroupBuilderEditorGroupOptionSphereCollision = GlobalExplorer.GetInstance().Components.Single<SphereCollision>(true);
				_GroupBuilderEditorGroupOptionConcaveCollision = GlobalExplorer.GetInstance().Components.Single<ConcaveCollision>(true);
				_GroupBuilderEditorGroupOptionConvexCollision = GlobalExplorer.GetInstance().Components.Single<ConvexCollision>(true);
				_GroupBuilderEditorGroupOptionConvexClean = GlobalExplorer.GetInstance().Components.Single<ConvexClean>(true);
				_GroupBuilderEditorGroupOptionConvexSimplify = GlobalExplorer.GetInstance().Components.Single<ConvexSimplify>(true);

				if (null != _GroupBuilderEditorGroupOptionSphereCollision)
				{
					_GroupBuilderEditorGroupOptionSphereCollision.Parent = this;
					_GroupBuilderEditorGroupOptionSphereCollision.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(0).AddChild(_GroupBuilderEditorGroupOptionSphereCollision);

					_GroupBuilderEditorGroupOptionSphereCollision.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				if (null != _GroupBuilderEditorGroupOptionConcaveCollision)
				{
					_GroupBuilderEditorGroupOptionConcaveCollision.Parent = this;
					_GroupBuilderEditorGroupOptionConcaveCollision.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(0).AddChild(_GroupBuilderEditorGroupOptionConcaveCollision);

					_GroupBuilderEditorGroupOptionConcaveCollision.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				if (null != _GroupBuilderEditorGroupOptionConvexCollision)
				{
					_GroupBuilderEditorGroupOptionConvexCollision.Parent = this;
					_GroupBuilderEditorGroupOptionConvexCollision.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(0).AddChild(_GroupBuilderEditorGroupOptionConvexCollision);

					_GroupBuilderEditorGroupOptionConvexCollision.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				if (null != _GroupBuilderEditorGroupOptionConvexClean)
				{
					_GroupBuilderEditorGroupOptionConvexClean.Parent = this;
					_GroupBuilderEditorGroupOptionConvexClean.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(0).AddChild(_GroupBuilderEditorGroupOptionConvexClean);

					_GroupBuilderEditorGroupOptionConvexClean.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}

				if (null != _GroupBuilderEditorGroupOptionConvexSimplify)
				{
					_GroupBuilderEditorGroupOptionConvexSimplify.Parent = this;
					_GroupBuilderEditorGroupOptionConvexSimplify.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(0).AddChild(_GroupBuilderEditorGroupOptionConvexSimplify);

					_GroupBuilderEditorGroupOptionConvexSimplify.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
			}

			_InitializeGroupOptionFadeModeTitle(Trait<Containerable>().Select(2).GetInnerContainer(0));
			if (_GlobalExplorer.Components.HasAll(FadeModeComponents.ToArray()))
			{
				_GroupBuilderEditorGroupOptionFadeMode = GlobalExplorer.GetInstance().Components.Single<VisibilityFadeMode>(true);

				if (null != _GroupBuilderEditorGroupOptionFadeMode)
				{
					_GroupBuilderEditorGroupOptionFadeMode.Parent = this;
					_GroupBuilderEditorGroupOptionFadeMode.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(0).AddChild(_GroupBuilderEditorGroupOptionFadeMode);

					_GroupBuilderEditorGroupOptionFadeMode.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
			}
			
			_InitializeGroupOptionDragTitle(Trait<Containerable>().Select(2).GetInnerContainer(1));
			if (_GlobalExplorer.Components.HasAll(DragComponents.ToArray()))
			{
				_GroupBuilderEditorGroupOptionDragOffset = GlobalExplorer.GetInstance().Components.Single<DragOffset>(true);

				if (null != _GroupBuilderEditorGroupOptionDragOffset)
				{
					_GroupBuilderEditorGroupOptionDragOffset.Parent = this;
					_GroupBuilderEditorGroupOptionDragOffset.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(1).AddChild(_GroupBuilderEditorGroupOptionDragOffset);

					_GroupBuilderEditorGroupOptionDragOffset.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
			}
			
			_InitializeGroupOptionVisibilityRangeTitle(Trait<Containerable>().Select(2).GetInnerContainer(1));
			if (_GlobalExplorer.Components.HasAll(VisibilityComponents.ToArray()))
			{
				_GroupBuilderEditorGroupOptionVisibilityBegin = GlobalExplorer.GetInstance().Components.Single<VisibilityBegin>(true);
				_GroupBuilderEditorGroupOptionVisibilityBeginMargin = GlobalExplorer.GetInstance().Components.Single<VisibilityBeginMargin>(true);
				_GroupBuilderEditorGroupOptionVisibilityEnd = GlobalExplorer.GetInstance().Components.Single<VisibilityEnd>(true);
				_GroupBuilderEditorGroupOptionVisibilityEndMargin = GlobalExplorer.GetInstance().Components.Single<VisibilityEndMargin>(true);

				if (null != _GroupBuilderEditorGroupOptionVisibilityBegin)
				{
					_GroupBuilderEditorGroupOptionVisibilityBegin.Parent = this;
					_GroupBuilderEditorGroupOptionVisibilityBegin.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(1).AddChild(_GroupBuilderEditorGroupOptionVisibilityBegin);

					_GroupBuilderEditorGroupOptionVisibilityBegin.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if (null != _GroupBuilderEditorGroupOptionVisibilityBeginMargin)
				{
					_GroupBuilderEditorGroupOptionVisibilityBeginMargin.Parent = this;
					_GroupBuilderEditorGroupOptionVisibilityBeginMargin.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(1).AddChild(_GroupBuilderEditorGroupOptionVisibilityBeginMargin);

					_GroupBuilderEditorGroupOptionVisibilityBeginMargin.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if (null != _GroupBuilderEditorGroupOptionVisibilityEnd)
				{
					_GroupBuilderEditorGroupOptionVisibilityEnd.Parent = this;
					_GroupBuilderEditorGroupOptionVisibilityEnd.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(1).AddChild(_GroupBuilderEditorGroupOptionVisibilityEnd);

					_GroupBuilderEditorGroupOptionVisibilityEnd.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if (null != _GroupBuilderEditorGroupOptionVisibilityEndMargin)
				{
					_GroupBuilderEditorGroupOptionVisibilityEndMargin.Parent = this;
					_GroupBuilderEditorGroupOptionVisibilityEndMargin.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(1).AddChild(_GroupBuilderEditorGroupOptionVisibilityEndMargin);

					_GroupBuilderEditorGroupOptionVisibilityEndMargin.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
			}
			
			_InitializeGroupOptionLODTitle(Trait<Containerable>().Select(2).GetInnerContainer(2));
			if (ExplorerUtils.Get().Components.HasAll(LODComponents.ToArray()))
			{
				_GroupBuilderEditorGroupOptionLevelOfDetailsState = GlobalExplorer.GetInstance().Components.Single<LevelOfDetailsState>(true);
				_GroupBuilderEditorGroupOptionLevelOfDetails = GlobalExplorer.GetInstance().Components.Single<LevelOfDetails>(true);

				if (null != _GroupBuilderEditorGroupOptionLevelOfDetailsState)
				{
					_GroupBuilderEditorGroupOptionLevelOfDetailsState.Parent = this;
					_GroupBuilderEditorGroupOptionLevelOfDetailsState.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(2).AddChild(_GroupBuilderEditorGroupOptionLevelOfDetailsState);

					_GroupBuilderEditorGroupOptionLevelOfDetailsState.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if (null != _GroupBuilderEditorGroupOptionLevelOfDetails)
				{
					_GroupBuilderEditorGroupOptionLevelOfDetails.Parent = this;
					_GroupBuilderEditorGroupOptionLevelOfDetails.Initialize();
					Trait<Containerable>().Select(2).GetInnerContainer(2).AddChild(_GroupBuilderEditorGroupOptionLevelOfDetails);

					_GroupBuilderEditorGroupOptionLevelOfDetails.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
			}
			
		}

		/// <summary>
		/// Finalizes the fields by setting up containers and adding elements to them.
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
			containerTwoTwo.SizeFlagsHorizontal = Control.SizeFlags.ShrinkEnd;

			Trait<Labelable>()
				.Select(0)
				.AddToContainer(
					containerTwoOne
				);

			Trait<Labelable>()
				.Select(1)
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
		/// Initializes the title for snap object position in the specified container.
		/// </summary>
		/// <param name="Container">The container to add the title to.</param>
		private void _InitializeGroupOptionSnapObjectPositionTitle(Godot.Container Container)
		{
			Container.AddChild(_GenerateTitle("InitializeGroupOptionSnapObjectPositionTitle", "Snap object position"));
		}

		/// <summary>
		/// Initializes the title for snap object options in the specified container.
		/// </summary>
		/// <param name="Container">The container to add the title to.</param>
		private void _InitializeGroupOptionSnapObjectTitle(Godot.Container Container)
		{
			Container.AddChild(_GenerateTitle("InitializeGroupOptionSnapObjectTitleContainer", "Snap to object options"));
		}

		/// <summary>
		/// Initializes the title for snap options in the specified container.
		/// </summary>
		/// <param name="Container">The container to add the title to.</param>
		private void _InitializeGroupOptionSnapTitle(Godot.Container Container)
		{
			Container.AddChild(_GenerateTitle("InitializeGroupOptionSnapTitleContainer", "Snap options"));
		}

		/// <summary>
		/// Initializes the title for collision options in the specified container.
		/// </summary>
		/// <param name="Container">The container to add the title to.</param>
		private void _InitializeGroupOptionCollisionTitle(Godot.Container Container)
		{
			Container.AddChild(_GenerateTitle("InitializeGroupOptionCollisionTitleContainer", "Collision options"));
		}

		/// <summary>
		/// Initializes the title for drag options in the specified container.
		/// </summary>
		/// <param name="Container">The container to add the title to.</param>
		private void _InitializeGroupOptionDragTitle(Godot.Container Container)
		{
			Container.AddChild(_GenerateTitle("InitializeGroupOptionDragTitleContainer", "Drag options"));
		}

		/// <summary>
		/// Initializes the title for placement modes in the specified container.
		/// </summary>
		/// <param name="Container">The container to add the title to.</param>
		private void _InitializeGroupOptionPlacementModeTitle(Godot.Container Container)
		{
			Container.AddChild(_GenerateTitle("_InitializeGroupOptionPlacementModeTitle", "Placement Modes"));
		}
		
		/// <summary>
		/// Initializes the title for level of details in the specified container.
		/// </summary>
		/// <param name="Container">The container to add the title to.</param>
		private void _InitializeGroupOptionLODTitle(Godot.Container Container)
		{
			Container.AddChild(_GenerateTitle("_InitializeGroupOptionLODTitle", "Level of details"));
		}
		
		/// <summary>
		/// Initializes the title for visibility range in the specified container.
		/// </summary>
		/// <param name="Container">The container to add the title to.</param>
		private void _InitializeGroupOptionVisibilityRangeTitle(Godot.Container Container ) 
		{
			Container.AddChild(_GenerateTitle("_InitializeGroupOptionVisibilityTitle", "Visibility"));
		}
		
		/// <summary>
		/// Initializes the title for fade mode in the specified container.
		/// </summary>
		/// <param name="Container">The container to add the title to.</param>
		private void _InitializeGroupOptionFadeModeTitle(Godot.Container Container)
		{
			Container.AddChild(_GenerateTitle("_InitializeGroupOptionVisibilityFadeModeTitle", "Visibility Fade Mode"));
		}

		/// <summary>
		/// Generates a title label inside a margin container.
		/// </summary>
		/// <param name="name">The name of the margin container.</param>
		/// <param name="text">The text of the title label.</param>
		/// <returns>The generated margin container with the title label.</returns>
		private MarginContainer _GenerateTitle(string name, string text)
		{
			MarginContainer _innerContainer = new()
			{
				Name = name
			};

			_innerContainer.AddThemeConstantOverride("margin_top", 5);
			_innerContainer.AddThemeConstantOverride("margin_bottom", 5);
			_innerContainer.AddThemeConstantOverride("margin_right", 15);

			Label title = new()
			{
				ThemeTypeVariation = "HeaderSmall",
				Text = text
			};

			_innerContainer.AddChild(title);
			return _innerContainer;
		}

		/// <summary>
		/// Handles the event when the close options button is pressed.
		/// </summary>
		private void _OnCloseOptionsPressed()
		{
			DoHide();
			_GlobalExplorer.GroupBuilder._Editor.Listing.DoShow();
		}
	}
}

#endif