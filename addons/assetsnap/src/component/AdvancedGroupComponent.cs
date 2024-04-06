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
using AssetSnap.Front.Nodes;
using Godot;

namespace AssetSnap.Component
{
	[Tool]
	public partial class AdvancedGroupComponent : GroupObjectComponent
	{
		protected override void _RegisterTraits()
		{
			AddTrait(typeof(Containerable));
			AddTrait(typeof(Labelable));
		}
		
		protected override void _InitializeFields()
		{
			Trait<Containerable>()
				.SetName("AdvancedGroupComponentContainer")
				.SetMargin(0)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetInnerOrientation(Containerable.ContainerOrientation.Vertical)
				.Instantiate();
				
			Trait<Labelable>()
				.SetName("AdvancedGroupComponentTitle")
				.SetMargin(0)
				.SetMargin(5, "bottom")
				.SetText(Text)
				.SetType(Labelable.TitleType.HeaderSmall)
				.Instantiate();
		}
		protected override void _FinalizeFields()
		{
			Container OuterContainer = Trait<Containerable>()
				.Select(0)
				.GetOuterContainer();
				
			Trait<Labelable>()
				.Select(0)
				.AddToContainer(
					OuterContainer,
					0
				);
							
			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					Container
				);
		}
	}
}
#endif