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
	/// Represents an option for cleaning convex groups.
	/// </summary>
	[Tool]
	public partial class ConvexClean : GroupOptionCheckableComponent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConvexClean"/> class.
		/// </summary>
		public ConvexClean()
		{
			Name = "GroupsBuilderGroupOptionsConvexClean";
		
			_UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
		}
		
		/// <summary>
		/// Initializes fields specific to the ConvexClean option.
		/// </summary>
		protected override void _InitializeFields()
		{
			Trait<Checkable>()
				.SetName("InitializeGroupOptionCollisionConvexCleanContainer")
				.SetMargin(35, "right")
				.SetMargin(10, "left")
				.SetText("Convex Clean")
				.SetAction( Callable.From( () => { _OnCheck(); }) )
				.Instantiate();
		}
		
		/// <summary>
        /// Handles the check event of the ConvexClean option.
        /// </summary>
		private void _OnCheck()
		{
			_GlobalExplorer.GroupBuilder._Editor.Group.ConvexClean = !_GlobalExplorer.GroupBuilder._Editor.Group.ConvexClean;
			
			if( true == _GlobalExplorer.GroupBuilder._Editor.Group.ConvexClean && StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group )
			{
				StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Enabled;
			}
			else if( StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group )
			{
				StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Disabled;
			}
			Parent._UpdateGroupOptions();

			_MaybeUpdateGrouped("ConvexClean", _GlobalExplorer.GroupBuilder._Editor.Group.ConvexClean);
			_HasGroupDataHasChanged();
		}
	}
}

#endif