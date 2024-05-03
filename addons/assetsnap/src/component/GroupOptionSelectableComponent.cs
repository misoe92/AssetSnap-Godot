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
	/// <summary>
	/// Component for managing group options with selectable functionality.
	/// </summary>
	[Tool]
	public partial class GroupOptionSelectableComponent : GroupOptionComponent
	{
		/// <summary>
		/// The parent editor group options.
		/// </summary>
		public EditorGroupOptions Parent;
		
		/// <summary>
		/// Initializes the component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
		
			Initiated = true;
			
			_InitializeFields();
			_FinalizeFields();
		}
		
		/// <summary>
		/// Shows the selectable input.
		/// </summary>
		public void InputShow()
		{
			Trait<Selectable>()
				.Select(0)
				.SetVisible(true);
		}
		
		/// <summary>
		/// Hides the selectable input.
		/// </summary>
		public void InputHide()
		{
			Trait<Selectable>()
				.Select(0)
				.SetVisible(false);
		}
		
		/// <summary>
		/// Gets the value of the currently selected option.
		/// </summary>
		/// <returns>The value of the currently selected option.</returns>
		public string GetValue()
		{
			OptionButton select = Trait<Selectable>()
				.Select(0)
				.GetNode<OptionButton>();
				
			return select.GetItemText( select.GetIndex() );
		}
		
		/// <summary>
		/// Checks if the selectable input is hidden.
		/// </summary>
		/// <returns>True if the selectable input is hidden, false otherwise.</returns>
		public bool IsHidden()
		{
			return Trait<Selectable>()
				.Select(0)
				.IsVisible() == false;
		}
		
		/// <summary>
		/// Initializes additional fields of the component.
		/// </summary>
		protected virtual void _InitializeFields(){}
		
		/// <summary>
        /// Finalizes the initialization of the fields.
        /// </summary>
		protected void _FinalizeFields()
		{
			Trait<Selectable>()
				.Select(0)
				.AddToContainer(this);
		}
	}
}

#endif