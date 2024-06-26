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
	/// Component allowing snapping group objects to different positions.
	/// </summary>
	[Tool]
	public partial class SnapToObjectPosition : GroupOptionCheckableComponent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SnapToObjectPosition"/> class.
		/// </summary>
		public SnapToObjectPosition()
		{
			Name = "GroupsBuilderGroupOptionsSnapToObjectPosition";
			
			_UsingTraits = new()
			{
				{ typeof(Spinboxable).ToString() },
			};
		}
		
		/// <summary>
		/// Initializes the fields of the group option component.
		/// </summary>
		protected override void _InitializeFields()
		{
			Trait<Checkable>()
				.SetName("InitializeGroupOptionSnapObjectContainer")
				.SetMargin(35, "right")
				.SetText("Top")
				.SetValue(StatesUtils.Get().GroupSnapsTo == GlobalStates.SnapPosition.Top)
				.SetAction( Callable.From( () => { _OnSnapGroupToTop(); }) )
				.Instantiate();
				
			Trait<Checkable>()
				.SetName("InitializeGroupOptionSnapObjectContainer")
				.SetMargin(35, "right")
				.SetText("Middle")
				.SetValue(StatesUtils.Get().GroupSnapsTo == GlobalStates.SnapPosition.Middle)
				.SetAction( Callable.From( () => { _OnSnapGroupToMiddle(); }) )
				.Instantiate();
				
			Trait<Checkable>()
				.SetName("InitializeGroupOptionSnapObjectContainer")
				.SetMargin(35, "right")
				.SetText("Bottom")
				.SetValue(StatesUtils.Get().GroupSnapsTo == GlobalStates.SnapPosition.Bottom)
				.SetAction( Callable.From( () => { _OnSnapGroupToBottom(); }) )
				.Instantiate();
		}
		
		/// <summary>
		/// Finalizes the fields of the group option component.
		/// </summary>
		protected override void _FinalizeFields()
		{
			base._FinalizeFields();
		
			Trait<Checkable>()
				.Select(1)
				.AddToContainer(
					this
				);
				
			Trait<Checkable>()
				.Select(2)
				.AddToContainer(
					this
				);
		}
		
		/// <summary>
        /// Sets the snap position of the group to the top.
        /// </summary>
		private void _OnSnapGroupToTop()
		{
			StatesUtils.Get().GroupSnapsTo = GlobalStates.SnapPosition.Top;

			Trait<Checkable>()
				.Select(1)
				.SetValue(false);
				
			Trait<Checkable>()
				.Select(2)
				.SetValue(false);
		}
		
		/// <summary>
        /// Sets the snap position of the group to the middle.
        /// </summary>
		private void _OnSnapGroupToMiddle()
		{
			StatesUtils.Get().GroupSnapsTo = GlobalStates.SnapPosition.Middle;
			
			Trait<Checkable>()
				.Select(0)
				.SetValue(false);
				
			Trait<Checkable>()
				.Select(2)
				.SetValue(false);
		}
		
		/// <summary>
        /// Sets the snap position of the group to the bottom.
        /// </summary>
		private void _OnSnapGroupToBottom()
		{
			StatesUtils.Get().GroupSnapsTo = GlobalStates.SnapPosition.Bottom;
			
			Trait<Checkable>()
				.Select(0)
				.SetValue(false);
				
			Trait<Checkable>()
				.Select(1)
				.SetValue(false);
		}
	}
}

#endif