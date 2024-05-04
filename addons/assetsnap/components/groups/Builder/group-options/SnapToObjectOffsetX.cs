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
	/// Represents a component for snapping an object's X offset to a specified value.
	/// </summary>
	[Tool]
	public partial class SnapToObjectOffsetX : GroupOptionSpinboxableComponent
	{
		/// <summary>
		/// Constructor for the SnapToObjectOffsetX class.
		/// </summary>
		public SnapToObjectOffsetX()
		{
			Name = "GroupsBuilderGroupOptionsSnapToObjectOffsetX";

			_UsingTraits = new()
			{
				{ typeof(Spinboxable).ToString() },
			};
		}

		/// <summary>
		/// Initializes the fields of the SnapToObjectOffsetX component.
		/// </summary>
		protected override void _InitializeFields()
		{
			Trait<Spinboxable>()
				.SetName("GroupBuilderEditorGroupOptionSnapToObjectOffsetX")
				.SetMargin(35, "right")
				.SetMargin(10, "left")
				.SetPrefix("Offset X: ")
				.SetValue(0)
				.SetStep(0.1f)
				.SetMinValue(0.0f)
				.SetAction(Callable.From((double value) => { _OnValueChanged((int)value); }))
				.Instantiate();

			Trait<Spinboxable>()
				.Select(0)
				.GetNode<SpinBox>()
				.GetLineEdit().AddThemeConstantOverride("minimum_character_width", 24);
		}

		/// <summary>
        /// Event handler for when the value of the spinbox changes.
        /// </summary>
        /// <param name="value">The new value of the spinbox.</param>
		private void _OnValueChanged(float value)
		{
			_GlobalExplorer.GroupBuilder._Editor.Group.SnapToObjectOffsetXValue = value;

			if (StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group)
			{
				StatesUtils.Get().SnapToObjectOffsetXValue = value;
			}

			Parent._UpdateGroupOptions();

			_MaybeUpdateGrouped("SnapToObjectOffsetXValue", value);
			_HasGroupDataHasChanged();
		}
	}
}

#endif