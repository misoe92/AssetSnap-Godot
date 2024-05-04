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
	/// Represents a component for managing the level of details state in the group options of a builder.
	/// </summary>
	[Tool]
	public partial class LevelOfDetailsState : GroupOptionCheckableComponent
	{
		private readonly string _Tooltip = "When enabled you will be able to choose a Level of details level that all models in the group will default to, if none is set models will default to Project settings";
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LevelOfDetailsState"/> class.
		/// </summary>
		public LevelOfDetailsState()
		{
			Name = "GroupsBuilderGroupOptionsLevelOfDetailsState";
			
			_UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
		}
		
		/// <summary>
		/// Initializes the fields of the level of details state component.
		/// </summary>
		protected override void _InitializeFields()
		{
				
			Trait<Checkable>()
				.SetName("GroupBuilderEditorGroupOptionLevelOfDetailsState")
				.SetAction( Callable.From( () => { _OnValueChanged(); } ) )
				.SetMargin(35, "right")
				.SetTooltipText(_Tooltip)
				.SetText("Specify Level of details")
				.Instantiate();
		}

		/// <summary>
        /// Handles the value changed event of the level of details state component.
        /// </summary>
		private void _OnValueChanged()
		{
			if( StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group ) 
			{
				if( StatesUtils.Get().LevelOfDetailsState == GlobalStates.LibraryStateEnum.Disabled ) 
				{
					StatesUtils.Get().LevelOfDetailsState = GlobalStates.LibraryStateEnum.Enabled; 
				}
				else 
				{
					StatesUtils.Get().LevelOfDetailsState = GlobalStates.LibraryStateEnum.Disabled; 
				}
				
				Parent._UpdateGroupOptions();
				_HasGroupDataHasChanged();
			} 
		}
	}
}

#endif