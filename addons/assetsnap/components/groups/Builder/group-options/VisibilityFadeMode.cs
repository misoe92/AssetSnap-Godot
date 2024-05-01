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
	public partial class VisibilityFadeMode : GroupOptionSelectableComponent
	{
		public VisibilityFadeMode()
		{
			Name = "GroupsBuilderGroupOptionsVisibilityFadeMode";
			
			UsingTraits = new()
			{
				{ typeof(Selectable).ToString() },
			};
		}
		
		protected override void _InitializeFields()
		{
			Trait<Selectable>()
				.SetName("GroupBuilderEditorGroupOptionVisibilityFadeMode")
				.SetType(Selectable.Type.SelectableSmall)
				.SetText("Fade Mode")
				.SetMargin(0)
				.SetMargin(35, "right")
				.AddItem("Use project default")
				.AddItem("Disabled")
				.AddItem("Self")
				.AddItem("Dependencies")
				.SetAction(( int index ) => { _OnValueChanged( index ); })
				.Instantiate();
		}

		private void _OnValueChanged( int index )
		{
			if( StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group ) 
			{
				OptionButton selectable = Trait<Selectable>().Select(0).GetNode() as OptionButton;
				StatesUtils.Get().VisibilityFadeMode = selectable.GetItemText(index);
			
				Parent._UpdateGroupOptions();
				_HasGroupDataHasChanged();
			}
		}
	}
}