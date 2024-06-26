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
	/// Component for enabling snapping to the X axis in group builder options.
	/// </summary>
	[Tool]
	public partial class SnapToX : GroupOptionCheckableComponent
	{
		/// <summary>
		/// Constructor for SnapToX component.
		/// </summary>
		public SnapToX()
		{
			Name = "GroupsBuilderGroupOptionsSnapToX";
			_UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
		}
		
		/// <summary>
		/// Initializes the fields for SnapToX component.
		/// </summary>
		protected override void _InitializeFields()
		{
			Trait<Checkable>()
				.SetName("InitializeGroupOptionSnapXContainer")
				.SetMargin(35, "right")
				.SetText("Snap to x")
				.SetAction( Callable.From( () => { _OnCheck(); }) )
				.Instantiate();
		}
		
		/// <summary>
        /// Handles the check event for snapping to the X axis.
        /// </summary>
		private void _OnCheck()
		{
			_GlobalExplorer.GroupBuilder._Editor.Group.SnapToX = !_GlobalExplorer.GroupBuilder._Editor.Group.SnapToX;
			
			if( true == _GlobalExplorer.GroupBuilder._Editor.Group.SnapToX && StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group )
			{
				StatesUtils.Get().SnapToX = GlobalStates.LibraryStateEnum.Enabled;
			}
			else if( StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group )
			{
				StatesUtils.Get().SnapToX = GlobalStates.LibraryStateEnum.Disabled;
			}
																		
			Parent._UpdateGroupOptions();

			_MaybeUpdateGrouped("SnapToX", _GlobalExplorer.GroupBuilder._Editor.Group.SnapToX);
			_HasGroupDataHasChanged();
		}
	}
}

#endif