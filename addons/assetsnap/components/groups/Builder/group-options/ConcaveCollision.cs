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
	/// Represents a component responsible for handling concave collision options for group objects.
	/// </summary>
	[Tool]
	public partial class ConcaveCollision : GroupOptionCheckableComponent
	{
		/// <summary>
		/// Constructor for the ConcaveCollision class.
		/// </summary>
		public ConcaveCollision()
		{
			Name = "GroupsBuilderGroupOptionsConcaveCollision";
		
			_UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
		}
		
		/// <summary>
		/// Initializes the fields of the ConcaveCollision component.
		/// </summary>
		protected override void _InitializeFields()
		{
			Trait<Checkable>()
				.SetName("InitializeGroupOptionCollisionConcaveContainer")
				.SetMargin(35, "right")
				.SetText("Concave collision")
				.SetAction( Callable.From( () => { _OnCheck(); }) )
				.Instantiate();
		}
		
		/// <summary>
        /// Handles the event when the concave collision option is checked.
        /// </summary>
		private void _OnCheck()
		{
			_GlobalExplorer.GroupBuilder._Editor.Group.ConcaveCollision = !_GlobalExplorer.GroupBuilder._Editor.Group.ConcaveCollision;
				
			_GlobalExplorer.GroupBuilder._Editor.Group.SphereCollision = false;
			_GlobalExplorer.GroupBuilder._Editor.Group.ConvexCollision = false;
			_GlobalExplorer.GroupBuilder._Editor.Group.ConvexClean = false;
			_GlobalExplorer.GroupBuilder._Editor.Group.ConvexSimplify = false;
			
			if( true == _GlobalExplorer.GroupBuilder._Editor.Group.ConcaveCollision && StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group )
			{
				StatesUtils.Get().ConcaveCollision = GlobalStates.LibraryStateEnum.Enabled;
				
				StatesUtils.Get().ConvexCollision = GlobalStates.LibraryStateEnum.Disabled;
				StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Disabled;
				StatesUtils.Get().ConvexSimplify = GlobalStates.LibraryStateEnum.Disabled;
				StatesUtils.Get().SphereCollision = GlobalStates.LibraryStateEnum.Disabled;
			}
			else if( StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group )
			{
				StatesUtils.Get().ConcaveCollision = GlobalStates.LibraryStateEnum.Disabled;
			}
			Parent._UpdateGroupOptions();
			
			_MaybeUpdateGrouped("ConcaveCollision", _GlobalExplorer.GroupBuilder._Editor.Group.ConcaveCollision);
			_MaybeUpdateGrouped("SphereCollision", false);
			_MaybeUpdateGrouped("ConvexCollision", false);
			_MaybeUpdateGrouped("ConvexClean", false);
			_MaybeUpdateGrouped("ConvexSimplify", false);
			_HasGroupDataHasChanged();
		}
	}
}

#endif