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
	/// Component representing a sphere collision option in a group builder.
	/// </summary>
	[Tool]
	public partial class SphereCollision : GroupOptionCheckableComponent
	{
		/// <summary>
		/// Constructor for SphereCollision class.
		/// </summary>
		public SphereCollision()
		{
			Name = "GroupsBuilderGroupOptionsSphereCollision";
		
			_UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
		}
		
		/// <summary>
		/// Initializes the fields of the SphereCollision component.
		/// </summary>
		protected override void _InitializeFields()
		{
			Trait<Checkable>()
				.SetName("InitializeGroupOptionCollisionSphereContainer")
				.SetMargin(35, "right")
				.SetText("Sphere collision")
				.SetAction( Callable.From( () => { _OnCheck(); }) )
				.Instantiate();
		}

		/// <summary>
        /// Handles the event when the sphere collision option is checked or unchecked.
        /// </summary>
		private void _OnCheck()
		{
			_GlobalExplorer.GroupBuilder._Editor.Group.SphereCollision = !_GlobalExplorer.GroupBuilder._Editor.Group.SphereCollision;
			_GlobalExplorer.GroupBuilder._Editor.Group.ConcaveCollision = false;
			_GlobalExplorer.GroupBuilder._Editor.Group.ConvexCollision = false;
			_GlobalExplorer.GroupBuilder._Editor.Group.ConvexClean = false;
			_GlobalExplorer.GroupBuilder._Editor.Group.ConvexSimplify = false;
			
			if(
				true == _GlobalExplorer.GroupBuilder._Editor.Group.SphereCollision &&
				_GlobalExplorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Group
			)
			{
				_GlobalExplorer.States.SphereCollision = GlobalStates.LibraryStateEnum.Enabled;
				
				_GlobalExplorer.States.ConvexCollision = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.ConvexClean = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.ConvexSimplify = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.ConcaveCollision = GlobalStates.LibraryStateEnum.Disabled;
			}
			else if( _GlobalExplorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Group )
			{
				_GlobalExplorer.States.SphereCollision = GlobalStates.LibraryStateEnum.Disabled;
			}
			
			Parent._UpdateGroupOptions();
			
			_MaybeUpdateGrouped("SphereCollision", _GlobalExplorer.GroupBuilder._Editor.Group.SphereCollision);
			_MaybeUpdateGrouped("ConcaveCollision", false);
			_MaybeUpdateGrouped("ConvexCollision", false);
			_MaybeUpdateGrouped("ConvexClean", false);
			_MaybeUpdateGrouped("ConvexSimplify", false);
			_HasGroupDataHasChanged();
		}
	}
}

#endif