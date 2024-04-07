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

namespace AssetSnap.Front.Components
{
	using AssetSnap.Component;
	using Godot;

	[Tool]
	public partial class GroupBuilderEditorGroupOptionConcaveCollision : GroupOptionCheckableComponent
	{
		protected override void _InitializeFields()
		{
			Trait<Checkable>()
				.SetName("InitializeGroupOptionCollisionConcaveContainer")
				.SetMargin(35, "right")
				.SetText("Concave collision")
				.SetAction( Callable.From( () => { _OnCheck(); }) )
				.Instantiate();
		}
	
		private void _OnCheck()
		{
			_GlobalExplorer.GroupBuilder._Editor.Group.ConcaveCollision = !_GlobalExplorer.GroupBuilder._Editor.Group.ConcaveCollision;
				
			_GlobalExplorer.GroupBuilder._Editor.Group.SphereCollision = false;
			_GlobalExplorer.GroupBuilder._Editor.Group.ConvexCollision = false;
			_GlobalExplorer.GroupBuilder._Editor.Group.ConvexClean = false;
			_GlobalExplorer.GroupBuilder._Editor.Group.ConvexSimplify = false;
			
			if( true == _GlobalExplorer.GroupBuilder._Editor.Group.ConcaveCollision && _GlobalExplorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Group )
			{
				_GlobalExplorer.States.ConcaveCollision = GlobalStates.LibraryStateEnum.Enabled;
				
				_GlobalExplorer.States.ConvexCollision = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.ConvexClean = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.ConvexSimplify = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.SphereCollision = GlobalStates.LibraryStateEnum.Disabled;
			}
			else if( _GlobalExplorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Group )
			{
				_GlobalExplorer.States.ConcaveCollision = GlobalStates.LibraryStateEnum.Disabled;
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