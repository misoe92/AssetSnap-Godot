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
using AssetSnap.Front.Components;
using Godot;

namespace AssetSnap.Component
{
	[Tool]
	public partial class GroupOptionSpinboxableComponent : GroupOptionComponent
	{
		public GroupBuilderEditorGroupOptions Parent;
		
		public override void Initialize()
		{
			AddTrait(typeof(Spinboxable));
			Initiated = true;
			
			_InitializeFields();
			_FinalizeFields();
			
			base.Initialize();
		}
		
		public void Show()
		{
			Trait<Spinboxable>()
				.Select(0)
				.SetVisible(true);
		}
		
		public void Hide()
		{
			Trait<Spinboxable>()
				.Select(0)
				.SetVisible(false);
		}
		
		public void SetValue( double value )
		{
			Trait<Spinboxable>()
				.Select(0)
				.GetNode<SpinBox>()
				.Value = value;
		}
		
		public double GetValue()
		{
			return Trait<Spinboxable>()
				.Select(0)
				.GetNode<SpinBox>()
				.Value;
		}
		
		public override Variant GetValueVariant()
		{
			return Trait<Spinboxable>()
				.Select(0)
				.GetNode<SpinBox>()
				.Value;
		}
		
		public bool IsHidden()
		{
			return Trait<Spinboxable>()
				.Select(0)
				.IsVisible() == false;
		}
		
		protected virtual void _InitializeFields(){}
		
		protected void _FinalizeFields()
		{
			Trait<Spinboxable>()
				.Select(0)
				.AddToContainer(Container);
		}
	}
}
#endif