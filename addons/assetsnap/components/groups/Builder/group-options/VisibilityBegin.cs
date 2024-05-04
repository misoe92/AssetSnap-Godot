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

using AssetSnap.Component;
using AssetSnap.States;
using Godot;

namespace AssetSnap.Front.Components.Groups.Builder.GroupOptions
{
	/// <summary>
	/// Represents a group option for setting the visibility range beginning.
	/// </summary>
	[Tool]
	public partial class VisibilityBegin : GroupOptionSpinboxableComponent
	{
		/// <summary>
		/// Constructor for the VisibilityBegin class.
		/// </summary>
		public VisibilityBegin()
		{
			Name = "GroupsBuilderGroupOptionsVisibilityBegin";
			
			_UsingTraits = new()
			{
				{ typeof(Spinboxable).ToString() },
			};
		}
		
		/// <summary>
		/// Initializes the fields for the VisibilityBegin component.
		/// </summary>
		protected override void _InitializeFields()
		{
			Trait<Spinboxable>()
				.SetName("GroupBuilderEditorGroupOptionVisibilityBegin")
				.SetMargin(35, "right")
				.SetPrefix("Begin: ")
				.SetValue(0)
				.SetStep(0.1f)
				.SetMinValue(0.0f)
				.SetAction( Callable.From( ( double value ) => { _OnValueChanged( (float)value ); } ) )
				.Instantiate();
				
			Trait<Spinboxable>()
				.Select(0)
				.GetNode<SpinBox>()
				.GetLineEdit().AddThemeConstantOverride("minimum_character_width", 24);
		}

		/// <summary>
        /// Handles the value changed event for the visibility range.
        /// </summary>
        /// <param name="value">The new value of the visibility range.</param>
		private void _OnValueChanged( float value )
		{
			if( StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group ) 
			{
				StatesUtils.Get().VisibilityRangeBegin = value;
				Parent._UpdateGroupOptions();
				_HasGroupDataHasChanged();
			}
		}
	}
}

#endif