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
using Godot;

namespace AssetSnap.Front.Components.Groups.Builder.GroupOptions
{
	/// <summary>
	/// Represents a simple placement group option for the group builder.
	/// </summary>
	[Tool]
	public partial class PlacementSimple : GroupOptionCheckableComponent
	{
		/// <summary>
		/// Constructor for PlacementSimple class.
		/// </summary>
		public PlacementSimple()
		{
			Name = "GroupsBuilderGroupOptionsPlacementSimple";
		
			UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
		}
		
		/// <summary>
		/// Initializes the fields for the PlacementSimple component.
		/// </summary>
		protected override void _InitializeFields()
		{
			Trait<Checkable>()
				.SetName("GroupBuilderEditorGroupOptionPlacementSimple")
				.SetMargin(35, "right")
				.SetText("Simple placement")
				.SetAction( Callable.From( () => { _OnCheck(); }) )
				.Instantiate();
		}

		/// <summary>
        /// Handles the check event for the simple placement option.
        /// </summary>
		private void _OnCheck()
		{
			_GlobalExplorer.States.PlacingType = GlobalStates.PlacingTypeEnum.Simple;
			
			Parent._UpdateGroupOptions();
			_MaybeUpdateGrouped("PlacementSimple", _GlobalExplorer.States.PlacingType == GlobalStates.PlacingTypeEnum.Simple);
			_HasGroupDataHasChanged();
		}
	}
}

#endif