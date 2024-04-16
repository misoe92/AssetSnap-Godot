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

namespace AssetSnap.Front.Components.Groups.Builder.GroupOptions
{
	using AssetSnap.Component;
	using Godot;

	[Tool]
	public partial class SnapToObjectPosition : GroupOptionCheckableComponent
	{
		public SnapToObjectPosition()
		{
			Name = "GroupsBuilderGroupOptionsSnapToObjectPosition";
			
			UsingTraits = new()
			{
				{ typeof(Spinboxable).ToString() },
			};
		}
		
		protected override void _InitializeFields()
		{
			Trait<Checkable>()
				.SetName("InitializeGroupOptionSnapObjectContainer")
				.SetMargin(35, "right")
				.SetText("Top")
				.SetValue(_GlobalExplorer.States.GroupSnapsTo == GlobalStates.SnapPosition.Top)
				.SetAction( Callable.From( () => { _OnSnapGroupToTop(); }) )
				.Instantiate();
				
			Trait<Checkable>()
				.SetName("InitializeGroupOptionSnapObjectContainer")
				.SetMargin(35, "right")
				.SetText("Middle")
				.SetValue(_GlobalExplorer.States.GroupSnapsTo == GlobalStates.SnapPosition.Middle)
				.SetAction( Callable.From( () => { _OnSnapGroupToMiddle(); }) )
				.Instantiate();
				
			Trait<Checkable>()
				.SetName("InitializeGroupOptionSnapObjectContainer")
				.SetMargin(35, "right")
				.SetText("Bottom")
				.SetValue(_GlobalExplorer.States.GroupSnapsTo == GlobalStates.SnapPosition.Bottom)
				.SetAction( Callable.From( () => { _OnSnapGroupToBottom(); }) )
				.Instantiate();
		}
		
		protected override void _FinalizeFields()
		{
			base._FinalizeFields();
		
			Trait<Checkable>()
				.Select(1)
				.AddToContainer(
					Container
				);
				
			Trait<Checkable>()
				.Select(2)
				.AddToContainer(
					Container
				);
		}
		
		private void _OnSnapGroupToTop()
		{
			_GlobalExplorer.States.GroupSnapsTo = GlobalStates.SnapPosition.Top;

			Trait<Checkable>()
				.Select(1)
				.SetValue(false);
				
			Trait<Checkable>()
				.Select(2)
				.SetValue(false);
		}
		
		
		private void _OnSnapGroupToMiddle()
		{
			_GlobalExplorer.States.GroupSnapsTo = GlobalStates.SnapPosition.Middle;
			
			Trait<Checkable>()
				.Select(0)
				.SetValue(false);
				
			Trait<Checkable>()
				.Select(2)
				.SetValue(false);
		}
		
		
		private void _OnSnapGroupToBottom()
		{
			_GlobalExplorer.States.GroupSnapsTo = GlobalStates.SnapPosition.Bottom;
			
			Trait<Checkable>()
				.Select(0)
				.SetValue(false);
				
			Trait<Checkable>()
				.Select(1)
				.SetValue(false);
		}
	}
}