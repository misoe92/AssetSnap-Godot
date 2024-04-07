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

namespace AssetSnap.Front.Components
{
	using System.Collections.Generic;
	using AssetSnap.Component;
	using AssetSnap.Instance.Input;
	using Godot;

	[Tool]
	public partial class GroupBuilderEditorGroupOptions : LibraryComponent
	{
		public GroupBuilderEditorGroupOptions()
		{
			Name = "GroupBuilderEditorGroupOptions";
			//_include = false;
		}
		
		/** Option Values **/
		private GroupBuilderEditorGroupOptionSnapToObject _GroupBuilderEditorGroupOptionSnapToObject;
		private GroupBuilderEditorGroupOptionSnapLayer _GroupBuilderEditorGroupOptionSnapLayer;
		private GroupBuilderEditorGroupOptionSnapToObjectOffsetX _GroupBuilderEditorGroupOptionSnapToObjectOffsetX;
		private GroupBuilderEditorGroupOptionSnapToObjectOffsetZ _GroupBuilderEditorGroupOptionSnapToObjectOffsetZ;
		private GroupBuilderEditorGroupOptionSnapToObjectPosition _GroupBuilderEditorGroupOptionSnapToObjectPosition;
		private GroupBuilderEditorGroupOptionSnapToHeight _GroupBuilderEditorGroupOptionSnapToHeight;
		private GroupBuilderEditorGroupOptionSnapToHeightValue _GroupBuilderEditorGroupOptionSnapToHeightValue;
		private GroupBuilderEditorGroupOptionSnapToX _GroupBuilderEditorGroupOptionSnapToX;
		private GroupBuilderEditorGroupOptionSnapToXValue _GroupBuilderEditorGroupOptionSnapToXValue;
		private GroupBuilderEditorGroupOptionSnapToZ _GroupBuilderEditorGroupOptionSnapToZ;
		private GroupBuilderEditorGroupOptionSnapToZValue _GroupBuilderEditorGroupOptionSnapToZValue;
		private GroupBuilderEditorGroupOptionSphereCollision _GroupBuilderEditorGroupOptionSphereCollision;
		private GroupBuilderEditorGroupOptionConcaveCollision _GroupBuilderEditorGroupOptionConcaveCollision;
		private GroupBuilderEditorGroupOptionConvexCollision _GroupBuilderEditorGroupOptionConvexCollision;
		private GroupBuilderEditorGroupOptionConvexClean _GroupBuilderEditorGroupOptionConvexClean;
		private GroupBuilderEditorGroupOptionConvexSimplify _GroupBuilderEditorGroupOptionConvexSimplify;
		private GroupBuilderEditorGroupOptionDragOffset _GroupBuilderEditorGroupOptionDragOffset;
		private GroupBuilderEditorGroupOptionPlacementSimple _GroupBuilderEditorGroupOptionPlacementSimple;
		private GroupBuilderEditorGroupOptionPlacementOptimized _GroupBuilderEditorGroupOptionPlacementOptimized;
		
		public override void Initialize()
		{
			AddTrait(typeof(Containerable));
			AddTrait(typeof(Labelable));
			AddTrait(typeof(Buttonable));
			AddTrait(typeof(Panelable));
			AddTrait(typeof(ScrollContainerable));
			Initiated = true;
			
			_InitializeFields();
			_InitializeComponents();

			_FinalizeFields();
			
			base.Initialize();
		}
		
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
				.SetVisible(false)
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
				.SetText( "Close Group Options" )
				.SetTooltipText( "Returns you back to the group editor" )
				.SetCursorShape(Control.CursorShape.PointingHand)
				.SetIcon(GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/close.svg"))
				.SetIconAlignment(HorizontalAlignment.Right)
				.SetAction( () => { _OnCloseOptionsPressed(); } )
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
		
		private void _InitializeComponents()
		{
			List<string> PlacementModesComponents = new()
			{
				"GroupBuilderEditorGroupOptionPlacementSimple",
				"GroupBuilderEditorGroupOptionPlacementOptimized",
			};

			List<string> SnaptoObjectComponents = new()
			{
				"GroupBuilderEditorGroupOptionSnapToObject",
				"GroupBuilderEditorGroupOptionSnapToObjectPosition",
				"GroupBuilderEditorGroupOptionSnapLayer",
				"GroupBuilderEditorGroupOptionSnapToObjectOffsetX",
				"GroupBuilderEditorGroupOptionSnapToObjectOffsetZ",
			};
			
			List<string> SnapToPlaneComponents = new()
			{
				"GroupBuilderEditorGroupOptionSnapToHeight",
				"GroupBuilderEditorGroupOptionSnapToHeightValue",
				"GroupBuilderEditorGroupOptionSnapToX",
				"GroupBuilderEditorGroupOptionSnapToXValue",
				"GroupBuilderEditorGroupOptionSnapToZ",
				"GroupBuilderEditorGroupOptionSnapToZValue",
			};
			
			List<string> CollisionComponents = new()
			{
				"GroupBuilderEditorGroupOptionSphereCollision",
				"GroupBuilderEditorGroupOptionConcaveCollision",
				"GroupBuilderEditorGroupOptionConvexCollision",
				"GroupBuilderEditorGroupOptionConvexClean",
				"GroupBuilderEditorGroupOptionConvexSimplify",
			};
			
			List<string> DragComponents = new()
			{
				"GroupBuilderEditorGroupOptionDragOffset",
			};
			
			/** Initializing Snap Object fields **/
			_InitializeGroupOptionPlacementModeTitle(Trait<Containerable>().Select(2).GetInnerContainer(0));
			if (_GlobalExplorer.Components.HasAll(PlacementModesComponents.ToArray()))
			{
				_GroupBuilderEditorGroupOptionPlacementSimple = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionPlacementSimple>(true);
				_GroupBuilderEditorGroupOptionPlacementOptimized = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionPlacementOptimized>(true);
			
				if( null != _GroupBuilderEditorGroupOptionPlacementSimple) 
				{
					_GroupBuilderEditorGroupOptionPlacementSimple.Container = Trait<Containerable>().Select(2).GetInnerContainer(0);
					_GroupBuilderEditorGroupOptionPlacementSimple.Parent = this;
					_GroupBuilderEditorGroupOptionPlacementSimple.Initialize(); 
				
					_GroupBuilderEditorGroupOptionPlacementSimple.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
			
				if( null != _GroupBuilderEditorGroupOptionPlacementOptimized) 
				{
					_GroupBuilderEditorGroupOptionPlacementOptimized.Container = Trait<Containerable>().Select(2).GetInnerContainer(0);
					_GroupBuilderEditorGroupOptionPlacementOptimized.Parent = this;
					_GroupBuilderEditorGroupOptionPlacementOptimized.Initialize(); 
				
					_GroupBuilderEditorGroupOptionPlacementOptimized.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
			}
			
			_InitializeGroupOptionSnapObjectTitle(Trait<Containerable>().Select(2).GetInnerContainer(1));
			if (_GlobalExplorer.Components.HasAll( SnaptoObjectComponents.ToArray() )) 
			{
				_GroupBuilderEditorGroupOptionSnapToObject = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionSnapToObject>(true);
				_GroupBuilderEditorGroupOptionSnapLayer = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionSnapLayer>(true);
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetX = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionSnapToObjectOffsetX>(true);
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionSnapToObjectOffsetZ>(true);
				_GroupBuilderEditorGroupOptionSnapToObjectPosition = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionSnapToObjectPosition>(true);
				
				if( null != _GroupBuilderEditorGroupOptionSnapToObject) 
				{
					_GroupBuilderEditorGroupOptionSnapToObject.Container = Trait<Containerable>().Select(2).GetInnerContainer(1);
					_GroupBuilderEditorGroupOptionSnapToObject.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToObject.Initialize(); 
				
					_GroupBuilderEditorGroupOptionSnapToObject.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
			
				if( null != _GroupBuilderEditorGroupOptionSnapToObjectOffsetX) 
				{
					_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.Container = Trait<Containerable>().Select(2).GetInnerContainer(1);
					_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.Initialize(); 
				
					_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if( null != _GroupBuilderEditorGroupOptionSnapToObjectOffsetZ) 
				{
					_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.Container = Trait<Containerable>().Select(2).GetInnerContainer(1);
					_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.Initialize(); 
				
					_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if( null != _GroupBuilderEditorGroupOptionSnapLayer) 
				{
					_GroupBuilderEditorGroupOptionSnapLayer.Container = Trait<Containerable>().Select(2).GetInnerContainer(1);
					_GroupBuilderEditorGroupOptionSnapLayer.Parent = this;
					_GroupBuilderEditorGroupOptionSnapLayer.Initialize(); 
				
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
			if (_GlobalExplorer.Components.HasAll( SnapToPlaneComponents.ToArray() )) 
			{
				_GroupBuilderEditorGroupOptionSnapToHeight = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionSnapToHeight>(true);
				_GroupBuilderEditorGroupOptionSnapToHeightValue = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionSnapToHeightValue>(true);
				_GroupBuilderEditorGroupOptionSnapToX = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionSnapToX>(true);
				_GroupBuilderEditorGroupOptionSnapToXValue = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionSnapToXValue>(true);
				_GroupBuilderEditorGroupOptionSnapToZ = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionSnapToZ>(true);
				_GroupBuilderEditorGroupOptionSnapToZValue = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionSnapToZValue>(true);
				
				if( null != _GroupBuilderEditorGroupOptionSnapToHeight) 
				{
					_GroupBuilderEditorGroupOptionSnapToHeight.Container = Trait<Containerable>().Select(2).GetInnerContainer(2);
					_GroupBuilderEditorGroupOptionSnapToHeight.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToHeight.Initialize(); 
				
					_GroupBuilderEditorGroupOptionSnapToHeight.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if( null != _GroupBuilderEditorGroupOptionSnapToHeightValue) 
				{
					_GroupBuilderEditorGroupOptionSnapToHeightValue.Container = Trait<Containerable>().Select(2).GetInnerContainer(2);
					_GroupBuilderEditorGroupOptionSnapToHeightValue.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToHeightValue.Initialize(); 
			
					_GroupBuilderEditorGroupOptionSnapToHeightValue.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if( null != _GroupBuilderEditorGroupOptionSnapToX) 
				{
					_GroupBuilderEditorGroupOptionSnapToX.Container = Trait<Containerable>().Select(2).GetInnerContainer(2);
					_GroupBuilderEditorGroupOptionSnapToX.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToX.Initialize(); 

					_GroupBuilderEditorGroupOptionSnapToX.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if( null != _GroupBuilderEditorGroupOptionSnapToXValue) 
				{
					_GroupBuilderEditorGroupOptionSnapToXValue.Container = Trait<Containerable>().Select(2).GetInnerContainer(2);
					_GroupBuilderEditorGroupOptionSnapToXValue.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToXValue.Initialize(); 
					
					_GroupBuilderEditorGroupOptionSnapToXValue.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if( null != _GroupBuilderEditorGroupOptionSnapToZ) 
				{
					_GroupBuilderEditorGroupOptionSnapToZ.Container = Trait<Containerable>().Select(2).GetInnerContainer(2);
					_GroupBuilderEditorGroupOptionSnapToZ.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToZ.Initialize(); 
					
					_GroupBuilderEditorGroupOptionSnapToZ.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if( null != _GroupBuilderEditorGroupOptionSnapToZValue) 
				{
					_GroupBuilderEditorGroupOptionSnapToZValue.Container = Trait<Containerable>().Select(2).GetInnerContainer(2);
					_GroupBuilderEditorGroupOptionSnapToZValue.Parent = this;
					_GroupBuilderEditorGroupOptionSnapToZValue.Initialize(); 
			
					_GroupBuilderEditorGroupOptionSnapToZValue.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
			}
			
			/** Initializing Snap Collisions fields **/
			_InitializeGroupOptionCollisionTitle(Trait<Containerable>().Select(2).GetInnerContainer(0));
			if (_GlobalExplorer.Components.HasAll( CollisionComponents.ToArray() )) 
			{
				_GroupBuilderEditorGroupOptionSphereCollision = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionSphereCollision>(true);
				_GroupBuilderEditorGroupOptionConcaveCollision = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionConcaveCollision>(true);
				_GroupBuilderEditorGroupOptionConvexCollision = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionConvexCollision>(true);
				_GroupBuilderEditorGroupOptionConvexClean = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionConvexClean>(true);
				_GroupBuilderEditorGroupOptionConvexSimplify = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionConvexSimplify>(true);
			
				if( null != _GroupBuilderEditorGroupOptionSphereCollision) 
				{
					_GroupBuilderEditorGroupOptionSphereCollision.Container = Trait<Containerable>().Select(2).GetInnerContainer(0);
					_GroupBuilderEditorGroupOptionSphereCollision.Parent = this;
					_GroupBuilderEditorGroupOptionSphereCollision.Initialize();

					_GroupBuilderEditorGroupOptionSphereCollision.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if( null != _GroupBuilderEditorGroupOptionConcaveCollision) 
				{
					_GroupBuilderEditorGroupOptionConcaveCollision.Container = Trait<Containerable>().Select(2).GetInnerContainer(0);
					_GroupBuilderEditorGroupOptionConcaveCollision.Parent = this;
					_GroupBuilderEditorGroupOptionConcaveCollision.Initialize(); 
				
					_GroupBuilderEditorGroupOptionConcaveCollision.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if( null != _GroupBuilderEditorGroupOptionConvexCollision) 
				{
					_GroupBuilderEditorGroupOptionConvexCollision.Container = Trait<Containerable>().Select(2).GetInnerContainer(0);
					_GroupBuilderEditorGroupOptionConvexCollision.Parent = this;
					_GroupBuilderEditorGroupOptionConvexCollision.Initialize(); 
					
					_GroupBuilderEditorGroupOptionConvexCollision.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if( null != _GroupBuilderEditorGroupOptionConvexClean) 
				{
					_GroupBuilderEditorGroupOptionConvexClean.Container = Trait<Containerable>().Select(2).GetInnerContainer(0);
					_GroupBuilderEditorGroupOptionConvexClean.Parent = this;
					_GroupBuilderEditorGroupOptionConvexClean.Initialize(); 
					
					_GroupBuilderEditorGroupOptionConvexClean.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
				
				if( null != _GroupBuilderEditorGroupOptionConvexSimplify) 
				{
					_GroupBuilderEditorGroupOptionConvexSimplify.Container = Trait<Containerable>().Select(2).GetInnerContainer(0);
					_GroupBuilderEditorGroupOptionConvexSimplify.Parent = this;
					_GroupBuilderEditorGroupOptionConvexSimplify.Initialize(); 
					
					_GroupBuilderEditorGroupOptionConvexSimplify.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
			}
			
			_InitializeGroupOptionDragTitle(Trait<Containerable>().Select(2).GetInnerContainer(1));
			if (_GlobalExplorer.Components.HasAll( DragComponents.ToArray() )) 
			{
				_GroupBuilderEditorGroupOptionDragOffset = GlobalExplorer.GetInstance().Components.Single<GroupBuilderEditorGroupOptionDragOffset>(true);
			
				if( null != _GroupBuilderEditorGroupOptionDragOffset) 
				{
					_GroupBuilderEditorGroupOptionDragOffset.Container = Trait<Containerable>().Select(2).GetInnerContainer(1);
					_GroupBuilderEditorGroupOptionDragOffset.Parent = this;
					_GroupBuilderEditorGroupOptionDragOffset.Initialize();

					_GroupBuilderEditorGroupOptionDragOffset.GroupOptionChanged += (string Name, Variant value) => { _UpdateGroupOption(Name, value); };
				}
			}
		}
		
		private void _FinalizeFields()
		{
			Container containerOne = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer();
			
			Container containerTwoOne = Trait<Containerable>()
				.Select(1)
				.GetInnerContainer(0);
				
			Container containerTwoTwo = Trait<Containerable>()
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
					Container
				);
		}
		
		public void Show()
		{
			Trait<Panelable>()
				.Select(0)
				.SetVisible(true);
		}
		
		public void Hide()
		{
			Trait<Panelable>()
				.Select(0)
				.SetVisible(false);
		}

		public void _UpdateGroupOption(string Name, Variant value ) 
		{
			if( true == _GroupBuilderEditorGroupOptionSnapToObject.GetValue()) 
			{
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.Show();
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.Show();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.Hide();
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.Hide();
			}
			
			if( true == _GroupBuilderEditorGroupOptionSnapToHeight.GetValue()) 
			{
				_GroupBuilderEditorGroupOptionSnapToHeightValue.Show();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToHeightValue.Hide();
			}
			
			if( true == _GroupBuilderEditorGroupOptionSnapToX.GetValue()) 
			{
				_GroupBuilderEditorGroupOptionSnapToXValue.Show();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToXValue.Hide();
			}
			
			if( true == _GroupBuilderEditorGroupOptionSnapToZ.GetValue() ) 
			{
				_GroupBuilderEditorGroupOptionSnapToZValue.Show();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToZValue.Hide();
			}
			
			if( true == _GroupBuilderEditorGroupOptionConvexCollision.GetValue() ) 
			{
				_GroupBuilderEditorGroupOptionConvexClean.Show();
				_GroupBuilderEditorGroupOptionConvexSimplify.Show();
			}
			else
			{
				_GroupBuilderEditorGroupOptionConvexClean.Hide();
				_GroupBuilderEditorGroupOptionConvexSimplify.Hide();
			}
		}

		public void _UpdateGroupOptions()
		{
			if( null == _GlobalExplorer.States.Group ) 
			{
				return;
			}
			
			if( false == Initiated ) 
			{
				GD.PushWarning("Group options object is not initialized");
				return;
			}
			_GroupBuilderEditorGroupOptionSnapToObject.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapToObject);
			_GroupBuilderEditorGroupOptionSnapLayer.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapLayer);
			_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.ObjectOffsetX);
			_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.ObjectOffsetZ);
			_GroupBuilderEditorGroupOptionSnapToHeight.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapToHeight);
			_GroupBuilderEditorGroupOptionSnapToHeightValue.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapHeightValue);
			_GroupBuilderEditorGroupOptionSnapToX.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapToX);
			_GroupBuilderEditorGroupOptionSnapToXValue.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapXValue);
			_GroupBuilderEditorGroupOptionSnapToZ.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapToZ);
			_GroupBuilderEditorGroupOptionSnapToZValue.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SnapZValue);
			_GroupBuilderEditorGroupOptionSphereCollision.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.SphereCollision);
			_GroupBuilderEditorGroupOptionConcaveCollision.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.ConcaveCollision);
			_GroupBuilderEditorGroupOptionConvexCollision.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.ConvexCollision);
			_GroupBuilderEditorGroupOptionConvexClean.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.ConvexClean);
			_GroupBuilderEditorGroupOptionConvexSimplify.SetValue(_GlobalExplorer.GroupBuilder._Editor.Group.ConvexSimplify);
			_GroupBuilderEditorGroupOptionPlacementSimple.SetValue(_GlobalExplorer.States.PlacingType == GlobalStates.PlacingTypeEnum.Simple);
			_GroupBuilderEditorGroupOptionPlacementOptimized.SetValue(_GlobalExplorer.States.PlacingType == GlobalStates.PlacingTypeEnum.Optimized);
			_GroupBuilderEditorGroupOptionDragOffset.SetValue(DragAddInputDriver.GetInstance().SizeOffset);
			
			if( true == _GroupBuilderEditorGroupOptionSnapToObject.GetValue()) 
			{
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.Show();
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.Show();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetX.Hide();
				_GroupBuilderEditorGroupOptionSnapToObjectOffsetZ.Hide();
			}
			
			if( true == _GroupBuilderEditorGroupOptionSnapToHeight.GetValue()) 
			{
				_GroupBuilderEditorGroupOptionSnapToHeightValue.Show();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToHeightValue.Hide();
			}
			
			if( true == _GroupBuilderEditorGroupOptionSnapToX.GetValue()) 
			{
				_GroupBuilderEditorGroupOptionSnapToXValue.Show();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToXValue.Hide();
			}
			
			if( true == _GroupBuilderEditorGroupOptionSnapToZ.GetValue() ) 
			{
				_GroupBuilderEditorGroupOptionSnapToZValue.Show();
			}
			else
			{
				_GroupBuilderEditorGroupOptionSnapToZValue.Hide();
			}
			
			if( true == _GroupBuilderEditorGroupOptionConvexCollision.GetValue() ) 
			{
				_GroupBuilderEditorGroupOptionConvexClean.Show();
				_GroupBuilderEditorGroupOptionConvexSimplify.Show();
			}
			else
			{
				_GroupBuilderEditorGroupOptionConvexClean.Hide();
				_GroupBuilderEditorGroupOptionConvexSimplify.Hide();
			}
		}
		
		private void _InitializeGroupOptionSnapObjectPositionTitle(Container Container)
		{
			Container.AddChild(_GenerateTitle("InitializeGroupOptionSnapObjectPositionTitle","Snap object position"));
		}
		
		private void _InitializeGroupOptionSnapObjectTitle(Container Container) 
		{
			Container.AddChild(_GenerateTitle("InitializeGroupOptionSnapObjectTitleContainer","Snap to object options"));
		}
		
		private void _InitializeGroupOptionSnapTitle(Container Container)
		{
			Container.AddChild(_GenerateTitle("InitializeGroupOptionSnapTitleContainer","Snap options"));
		}
		
		private void _InitializeGroupOptionCollisionTitle(Container Container)
		{
			Container.AddChild(_GenerateTitle("InitializeGroupOptionCollisionTitleContainer","Collision options"));
		}
		
		private void _InitializeGroupOptionDragTitle(Container Container) 
		{
			Container.AddChild(_GenerateTitle("InitializeGroupOptionDragTitleContainer","Drag options"));
		}
		
		private void _InitializeGroupOptionPlacementModeTitle(Container Container)
		{
			Container.AddChild(_GenerateTitle("_InitializeGroupOptionPlacementModeTitle","Placement Modes"));
		}
		
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
		
		private void _OnCloseOptionsPressed()
		{
			Hide();
			_GlobalExplorer.GroupBuilder._Editor.Listing.Show();
		}
	}
}