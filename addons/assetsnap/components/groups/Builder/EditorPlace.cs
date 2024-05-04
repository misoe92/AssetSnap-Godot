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
using AssetSnap.Explorer;
using AssetSnap.Instance.Input;
using AssetSnap.States;
using Godot;

namespace AssetSnap.Front.Components.Groups.Builder
{
	/// <summary>
	/// Editor component for placing groups of objects in the 3D world.
	/// </summary>
	[Tool]
	public partial class EditorPlace : LibraryComponent
	{
		private static readonly string Text = "Place";

		/// <summary>
		/// Constructor for EditorPlace class.
		/// </summary>
		public EditorPlace()
		{
			Name = "GroupBuilderEditorPlace";
			TooltipText = "Will let you place the group of objects in the 3D world";
			MouseDefaultCursorShape = Control.CursorShape.PointingHand;

			_UsingTraits = new()
			{
				{ typeof(Buttonable).ToString() },
			};

			//_include = false;
		}

		/// <summary>
		/// Initializes the EditorPlace component.
		/// </summary>
		public override void Initialize()
		{
			if (_Initiated)
			{
				return;
			}

			base.Initialize();

			_Initiated = true;

			_InitializeFields();
			_FinalizeFields();
		}

		/// <summary>
		/// Shows the EditorPlace component.
		/// </summary>
		public void DoShow()
		{
			Trait<Buttonable>()
				.Select(0)
				.SetVisible(true);
		}

		/// <summary>
		/// Hides the EditorPlace component.
		/// </summary>
		public void DoHide()
		{
			Trait<Buttonable>()
				.Select(0)
				.SetVisible(false);
		}

		/// <summary>
		/// Handles the action when placing a group.
		/// </summary>
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

		/// <summary>
		/// Initializes the fields of the EditorPlace component.
		/// </summary>
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

		/// <summary>
        /// Finalizes the fields of the EditorPlace component.
        /// </summary>
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

#endif