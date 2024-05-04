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
	/// Provides functionality for snapping groups to objects in the group builder.
	/// </summary>
	[Tool]
	public partial class SnapToObject : GroupOptionCheckableComponent
	{
		/// <summary>
		/// Constructor for SnapToObject class.
		/// </summary>
		public SnapToObject()
		{
			Name = "GroupsBuilderGroupOptionsSnapToObject";
			
			_UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
		}
		
		/// <summary>
		/// Initializes the fields required for SnapToObject component.
		/// </summary>
		protected override void _InitializeFields()
		{
			Trait<Checkable>()
				.SetName("InitializeGroupOptionSnapObjectContainer")
				.SetMargin(35, "right")
				.SetText("Snap to objects")
				.SetAction( Callable.From( () => { _OnSnapToObject(); }) )
				.Instantiate();
		}
		
		/// <summary>
        /// Handles the event when the snap to object option is toggled.
        /// </summary>
		private void _OnSnapToObject()
		{
			_GlobalExplorer.GroupBuilder._Editor.Group.SnapToObject = !_GlobalExplorer.GroupBuilder._Editor.Group.SnapToObject;
				
			if( true == _GlobalExplorer.GroupBuilder._Editor.Group.SnapToObject && _GlobalExplorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Group )
			{
				_GlobalExplorer.States.SnapToObject = GlobalStates.LibraryStateEnum.Enabled;
			}
			else if( _GlobalExplorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Group )
			{
				_GlobalExplorer.States.SnapToObject = GlobalStates.LibraryStateEnum.Disabled;
			}
												
			Parent._UpdateGroupOptions();

			_MaybeUpdateGrouped("SnapToObject", _GlobalExplorer.GroupBuilder._Editor.Group.SnapToObject);
			_HasGroupDataHasChanged();
		}
	}
}

#endif