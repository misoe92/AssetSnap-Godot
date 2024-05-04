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
	/// Represents a group option for convex collision.
	/// </summary>
	[Tool]
	public partial class ConvexCollision : GroupOptionCheckableComponent
	{
		/// <summary>
		/// Constructor for ConvexCollision class.
		/// </summary>
		public ConvexCollision()
		{
			Name = "GroupsBuilderGroupOptionsConvexCollision";
			
			UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
		}

		/// <summary>
		/// Initializes the fields for the ConvexCollision group option.
		/// </summary>
		protected override void _InitializeFields()
		{
			Trait<Checkable>()
				.SetName("InitializeGroupOptionCollisionConvexContainer")
				.SetMargin(35, "right")
				.SetText("Convex collision")
				.SetAction( Callable.From( () => { _OnCheck(); }) )
				.Instantiate();
		}

		/// <summary>
        /// Event handler for when the convex collision option is checked.
        /// </summary>
		private void _OnCheck()
		{
			_GlobalExplorer.GroupBuilder._Editor.Group.ConvexCollision = !_GlobalExplorer.GroupBuilder._Editor.Group.ConvexCollision;
			_GlobalExplorer.GroupBuilder._Editor.Group.ConcaveCollision = false;
			_GlobalExplorer.GroupBuilder._Editor.Group.SphereCollision = false;
			
			if(
				true == _GlobalExplorer.GroupBuilder._Editor.Group.ConvexCollision &&
				_GlobalExplorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Group
			)
			{
				_GlobalExplorer.States.ConvexCollision = GlobalStates.LibraryStateEnum.Enabled;
				_GlobalExplorer.States.ConcaveCollision = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.SphereCollision = GlobalStates.LibraryStateEnum.Disabled;
			}
			else if( _GlobalExplorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Group )
			{
				_GlobalExplorer.States.ConvexCollision = GlobalStates.LibraryStateEnum.Disabled;
			}
			
			Parent._UpdateGroupOptions();
			_HasGroupDataHasChanged();
			_MaybeUpdateGrouped("ConvexCollision", _GlobalExplorer.GroupBuilder._Editor.Group.ConvexCollision);
			_MaybeUpdateGrouped("SphereCollision", false);
			_MaybeUpdateGrouped("ConcaveCollision", false);
		}
	}
}

#endif