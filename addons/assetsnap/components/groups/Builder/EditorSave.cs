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

namespace AssetSnap.Front.Components.Groups.Builder
{
	/// <summary>
	/// Represents an editor component for saving group builder data.
	/// </summary>
	[Tool]
	public partial class EditorSave : LibraryComponent
	{
		private static readonly string _Text = "Save";

		/// <summary>
		/// Initializes a new instance of the <see cref="EditorSave"/> class.
		/// </summary>
		public EditorSave()
		{
			Name = "GroupBuilderEditorSave";
			TooltipText = "Will save the title and the object values";
			MouseDefaultCursorShape = Control.CursorShape.PointingHand;

			_UsingTraits = new()
			{
				{ typeof(Buttonable).ToString() },
				{ typeof(Containerable).ToString() },
			};

			//_include = false;
		}

		/// <summary>
		/// Initializes the editor save component.
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
		/// Shows the editor save component.
		/// </summary>
		public void DoShow()
		{
			Trait<Buttonable>()
				.Select(0)
				.SetVisible(true);
		}

		/// <summary>
		/// Hides the editor save component.
		/// </summary>
		public void DoHide()
		{
			Trait<Buttonable>()
				.Select(0)
				.SetVisible(false);
		}

		/// <summary>
		/// Called when the save action is triggered.
		/// </summary>
		private void _OnSave()
		{
			_GlobalExplorer.GroupBuilder._Editor.UpdateGroup();
		}

		/// <summary>
		/// Initializes the fields required for the editor save component.
		/// </summary>
		private void _InitializeFields()
		{
			Trait<Containerable>()
				.SetName("GroupBuilderEditorSaveContainer")
				.SetVerticalSizeFlags(SizeFlags.ShrinkCenter)
				.SetMargin(6, "top")
				.Instantiate();

			Trait<Buttonable>()
				.SetName("GroupBuilderEditorSave")
				.SetText(_Text)
				.SetTooltipText(TooltipText)
				.SetType(Buttonable.ButtonType.SmallSuccesButton)
				.SetCursorShape(MouseDefaultCursorShape)
				.SetVisible(false)
				.SetAction(() => { _OnSave(); })
				.Instantiate();
		}

		/// <summary>
        /// Finalizes the fields of the editor save component.
        /// </summary>
		private void _FinalizeFields()
		{
			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					this
				);

			Trait<Buttonable>()
				.Select(0)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				);
		}
	}
}

#endif