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
	/// Represents a group option component for optimized placement.
	/// </summary>
	[Tool]
	public partial class PlacementOptimized : GroupOptionCheckableComponent
	{
		/// <summary>
		/// Constructs a new instance of the PlacementOptimized class.
		/// </summary>
		public PlacementOptimized()
		{
			Name = "GroupsBuilderGroupOptionsPlacementOptimized";
			
			_UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
		}
		
		/// <summary>
		/// Initializes fields of the PlacementOptimized component.
		/// </summary>
		protected override void _InitializeFields()
		{
			Trait<Checkable>()
				.SetName("GroupBuilderEditorGroupOptionPlacementOptimized")
				.SetMargin(35, "right")
				.SetText("Optimized placement")
				.SetAction( Callable.From( () => { _OnCheck(); }) )
				.Instantiate();
		}

		/// <summary>
        /// Handles the check event for the optimized placement.
        /// </summary>
		private void _OnCheck()
		{
			StatesUtils.Get().PlacingType = GlobalStates.PlacingTypeEnum.Optimized;
			
			Parent._UpdateGroupOptions();
			_MaybeUpdateGrouped("PlacementOptimized", StatesUtils.Get().PlacingType == GlobalStates.PlacingTypeEnum.Optimized);
			_HasGroupDataHasChanged();
		}
	}
}

#endif