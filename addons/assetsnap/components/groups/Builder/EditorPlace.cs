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

namespace AssetSnap.Front.Components.Groups.Builder
{
	using AssetSnap.Component;
	using AssetSnap.Explorer;
	using AssetSnap.Instance.Input;
	using AssetSnap.States;
	using Godot;

	[Tool]
	public partial class EditorPlace : LibraryComponent
	{
		private static readonly string Text = "Place";

		public EditorPlace()
		{
			Name = "GroupBuilderEditorPlace";
			TooltipText = "Will let you place the group of objects in the 3D world";
			MouseDefaultCursorShape = Control.CursorShape.PointingHand;

			UsingTraits = new()
			{
				{ typeof(Buttonable).ToString() },
			};

			//_include = false;
		}

		public override void Initialize()
		{
			if (Initiated)
			{
				return;
			}

			base.Initialize();

			Initiated = true;

			_InitializeFields();
			_FinalizeFields();
		}

		public void DoShow()
		{
			Trait<Buttonable>()
				.Select(0)
				.SetVisible(true);
		}

		public void DoHide()
		{
			Trait<Buttonable>()
				.Select(0)
				.SetVisible(false);
		}

		private void _OnPlaceGroup()
		{
			ExplorerUtils.Get().GroupBuilder._Editor.OpenGroupOptions();
			StatesUtils.Get().PlacingMode = GlobalStates.PlacingModeEnum.Group;
			StatesUtils.Get().Group = _GlobalExplorer.GroupBuilder._Editor.Group;
			StatesUtils.Get().GroupedObject = _GlobalExplorer.GroupBuilder._Editor.Group.Build();

			ExplorerUtils.Get().GroupBuilder._Editor.Group.EachProperty(
				(string key, Variant value) =>
				{
					if( false == StatesUtils.Get().Is( key, value) ) {
						StatesUtils.Get().Set(key, value);
					}
				}
			);

			EditorInterface.Singleton.EditNode(StatesUtils.Get().GroupedObject);

			if (ExplorerUtils.Get().InputDriver is DragAddInputDriver DraggableInputDriver)
			{
				DraggableInputDriver.CalculateObjectSize();
			}
		}

		private void _InitializeFields()
		{
			Trait<Buttonable>()
				.SetName("GroupBuilderEditorPlace")
				.SetType(Buttonable.ButtonType.ActionButton)
				.SetText(Text)
				.SetTooltipText(TooltipText)
				.SetCursorShape(MouseDefaultCursorShape)
				.SetVisible(false)
				.SetAction(() => { _OnPlaceGroup(); })
				.Instantiate();
		}

		private void _FinalizeFields()
		{
			Trait<Buttonable>()
				.Select(0)
				.AddToContainer(
					this
				);
		}
	}
}