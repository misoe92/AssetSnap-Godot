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

namespace AssetSnap.Front.Components.Groups.Builder.GroupOptions
{
	using AssetSnap.Component;
	using AssetSnap.States;
	using Godot;

	[Tool]
	public partial class LevelOfDetails : GroupOptionSpinboxableComponent
	{
		public LevelOfDetails()
		{
			Name = "GroupsBuilderGroupOptionsLevelOfDetails";
			
			UsingTraits = new()
			{
				{ typeof(Spinboxable).ToString() },
			};
		}
		
		protected override void _InitializeFields()
		{
			Trait<Spinboxable>()
				.SetName("GroupBuilderEditorGroupOptionLevelOfDetails")
				.SetMargin(35, "right")
				.SetPrefix("LoD Level: ")
				.SetStep(0.1f)
				.SetMaxValue(5)
				.SetMinValue(0)
				.SetAction(Callable.From((double value) => { _OnValueChanged((float)value); }))
				.Instantiate();
				
			Trait<Spinboxable>()
				.Select(0)
				.GetNode<SpinBox>()
				.GetLineEdit().AddThemeConstantOverride("minimum_character_width", 24);
		}

		private void _OnValueChanged( float value )
		{
			if( StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group ) 
			{
				if( StatesUtils.Get().LevelOfDetailsState == GlobalStates.LibraryStateEnum.Enabled )
				{
					StatesUtils.Get().LevelOfDetails = value;
				}
				
				Parent._UpdateGroupOptions();
				_HasGroupDataHasChanged();
			}
		}
	}
}