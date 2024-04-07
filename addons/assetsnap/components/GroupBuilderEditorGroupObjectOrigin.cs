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
	using AssetSnap.Component;
	using Godot;

	[Tool]
	public partial class GroupBuilderEditorGroupObjectOrigin : GroupObjectComponent
	{

		public GroupBuilderEditorGroupObject Parent;
		
		public double GetValue( int index ) 
		{
			return Trait<Spinboxable>()
				.Select(index)
				.GetValue();
		}
		
		protected override void _RegisterTraits()
		{
			AddTrait(typeof(Containerable));
			AddTrait(typeof(Spinboxable));
			AddTrait(typeof(Labelable));
		}
		
		protected override void _InitializeFields()
		{
			if( Parent == null ) 
			{
				GD.PushError("No parent found @ Object Origin");
				return;
			}
			
			Trait<Containerable>()
				.SetName("GroupObjectOrigin")
				.SetMargin(10, "left")
				.SetMargin(15, "right")
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetInnerOrientation(Containerable.ContainerOrientation.Vertical)
				.SetDimensions(75, 0)
				.Instantiate();
				
			Trait<Labelable>()
				.SetMargin(0)
				.SetMargin(10, "top")
				.SetMargin(5, "bottom")
				.SetName("GroupObjectOriginLabel")
				.SetType(Labelable.TitleType.HeaderSmall)
				.SetText("Origin")
				.Instantiate();
		
			// X
			Trait<Labelable>()
				.SetName("GroupObjectsOriginXLabel")
				.SetMargin(0)
				.SetType(Labelable.TitleType.HeaderSmall)
				.SetText("X")
				.Instantiate();
				
			Trait<Spinboxable>()
				.SetName("GroupObjectsOriginXValue")
				.SetValue(Parent.Origin.X)
				.SetStep(0.1f)
				.SetMinValue(-200.0f)
				.SetMaxValue(200.0f)
				.SetAction( Callable.From( (double value) => { _OnValueChange(); } ) )
				.Instantiate();
			
			Trait<Spinboxable>()
				.Select(0)
				.GetNode<SpinBox>()
				.GetLineEdit()
				.AddThemeConstantOverride("minimum_character_width", 2);
			
			// Y
			Trait<Labelable>()
				.SetName("GroupObjectsOriginYLabel")
				.SetMargin(0)
				.SetType(Labelable.TitleType.HeaderSmall)
				.SetText("Y")
				.Instantiate();
				
			Trait<Spinboxable>()
				.SetName("GroupObjectsOriginYValue")
				.SetValue(Parent.Origin.Y)
				.SetStep(0.1f)
				.SetMinValue(-200.0f)
				.SetMaxValue(200.0f)
				.SetAction( Callable.From( (double value) => { _OnValueChange(); } ) )
				.Instantiate();
			
			Trait<Spinboxable>()
				.Select(1)
				.GetNode<SpinBox>()
				.GetLineEdit()
				.AddThemeConstantOverride("minimum_character_width", 2);
			
			// Z
			Trait<Labelable>()
				.SetMargin(0)
				.SetName("GroupObjectsOriginZLabel")
				.SetType(Labelable.TitleType.HeaderSmall)
				.SetText("Z")
				.Instantiate();
				
			Trait<Spinboxable>()
				.SetName("GroupObjectsOriginZValue")
				.SetValue(Parent.Origin.Z)
				.SetStep(0.1f)
				.SetMinValue(-200.0f)
				.SetMaxValue(200.0f)
				.SetAction( Callable.From( (double value) => { _OnValueChange(); } ) )
				.Instantiate();
			
			Trait<Spinboxable>()
				.Select(2)
				.GetNode<SpinBox>()
				.GetLineEdit()
				.AddThemeConstantOverride("minimum_character_width", 2);
		}
		
		protected override void _FinalizeFields()
		{
			Trait<Labelable>()
				.Select(1)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				);
				
			Trait<Spinboxable>()
				.Select(0)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				);
				
			Trait<Labelable>()
				.Select(2)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				);
				
			Trait<Spinboxable>()
				.Select(1)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				);
				
			Trait<Labelable>()
				.Select(3)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				);
				
			Trait<Spinboxable>()
				.Select(2)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				);
				
			Trait<Labelable>()
				.Select(0)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetOuterContainer(),
					0
				);
			
			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					Container
				);
		}
		
		private void _OnValueChange()
		{
			GroupBuilderEditorGroupObjectRotation Rotation = Parent._GroupBuilderEditorGroupObjectRotation;
			GroupBuilderEditorGroupObjectScale Scale = Parent._GroupBuilderEditorGroupObjectScale;
			
			GlobalExplorer.GetInstance().GroupBuilder._Editor.UpdateMeshInGroup(
				Index,
				new Vector3((float)GetValue(0), (float)GetValue(1), (float)GetValue(2)),
				new Vector3((float)Rotation.GetValue(0), (float)Rotation.GetValue(1), (float)Rotation.GetValue(2)),
				new Vector3((float)Scale.GetValue(0), (float)Scale.GetValue(1), (float)Scale.GetValue(2))
			);

			_TriggerGroupedUpdate();
		}
	}
}