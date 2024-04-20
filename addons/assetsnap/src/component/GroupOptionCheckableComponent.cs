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
using AssetSnap.Front.Components.Groups.Builder;
using Godot;

namespace AssetSnap.Component
{
	[Tool]
	public partial class GroupOptionCheckableComponent : GroupOptionComponent
	{
		public EditorGroupOptions Parent;
		
		public override void Initialize()
		{
			base.Initialize();
			
			Initiated = true;
			
			_InitializeFields();
			_FinalizeFields();
		}
		
		public void InputShow()
		{
			Trait<Checkable>()
				.Select(0)
				.SetVisible(true);
		}
		
		public void InputHide()
		{
			Trait<Checkable>()
				.Select(0)
				.SetVisible(false);
		}
		
		public void SetValue( bool value )
		{
			Trait<Checkable>()
				.Select(0)
				.GetNode<CheckBox>()
				.ButtonPressed = value;

			EmitSignal(SignalName.GroupOptionChanged, Name, value);
		}
		
		public bool GetValue()
		{
			return Trait<Checkable>()
				.Select(0)
				.GetNode<CheckBox>()
				.ButtonPressed;
		}
		
		public override Variant GetValueVariant()
		{
			return Trait<Checkable>()
				.Select(0)
				.GetNode<CheckBox>()
				.ButtonPressed;
		}
		
		public bool IsHidden()
		{
			return Trait<Checkable>()
				.Select(0)
				.IsVisible() == false;
		}
		
		protected virtual void _InitializeFields(){}
			
		protected virtual void _FinalizeFields()
		{
			Trait<Checkable>()
				.Select(0)
				.AddToContainer(
					this
				);
		}
	}
}
#endif