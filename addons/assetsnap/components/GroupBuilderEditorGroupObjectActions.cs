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
	public partial class GroupBuilderEditorGroupObjectActions : GroupObjectComponent
	{
		protected override void _RegisterTraits()
		{
			AddTrait(typeof(Containerable));
			AddTrait(typeof(Buttonable));
			AddTrait(typeof(Labelable));
			
		}
		protected override void _InitializeFields()
		{
			Trait<Containerable>()
				.SetName("GroupObjectsActions")
				.SetMargin(15, "left")
				.SetMargin(15, "right")
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetInnerOrientation(Containerable.ContainerOrientation.Vertical)
				.Instantiate();

			Trait<Labelable>()
				.SetName("GroupObjectsActionsLabel")
				.SetMargin(0)
				.SetMargin(10, "top")
				.SetMargin(5, "bottom")
				.SetType(Labelable.TitleType.HeaderSmall)
				.SetText("Quick Actions")
				.Instantiate();

			Trait<Buttonable>()
				.SetName("GroupObjectsActionsInnerRemoveButton")
				.SetType(Buttonable.ButtonType.SmallFlatButton)
				.SetText("Remove")
				.SetTooltipText("Removes the object from this group")
				.SetCursorShape(Control.CursorShape.PointingHand)
				.SetAction(() => { _OnRemoveObjectEntry(); })
				.Instantiate();
				
			Trait<Buttonable>()
				.SetName("GroupObjectsActionsInnerDuplicateButton")
				.SetType(Buttonable.ButtonType.SmallFlatButton)
				.SetText("Duplicate")
				.SetTooltipText("Duplicates the object, and adds the duplicated object to the group")
				.SetCursorShape(Control.CursorShape.PointingHand)
				.SetAction(() => { _OnDuplicateObjectEntry(); })
				.Instantiate();
		}
		
		protected override void _FinalizeFields()
		{
			Trait<Buttonable>()
				.Select(0)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				);
				
			Trait<Buttonable>()
				.Select(1)
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
		
		private void _OnRemoveObjectEntry()
		{
			GlobalExplorer.GetInstance().GroupBuilder._Editor.RemoveMeshInGroup(Index, Path);
		}
		
		private void _OnDuplicateObjectEntry()
		{
			GlobalExplorer.GetInstance().GroupBuilder._Editor.DuplicateMeshInGroup(Index);
		}
	}
}