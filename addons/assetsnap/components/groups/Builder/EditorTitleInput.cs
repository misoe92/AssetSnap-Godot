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
	/// A component for editing the title of a group in the builder interface.
	/// </summary>
	[Tool]
	public partial class EditorTitleInput : LibraryComponent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EditorTitleInput"/> class.
		/// </summary>
		public EditorTitleInput()
		{
			Name = "GroupBuilderEditorTitleInput";
			SizeFlagsVertical = SizeFlags.ShrinkCenter;
			//_include = false;
		}

		/// <summary>
		/// The input field for editing the title.
		/// </summary>
		public LineEdit _InputField;

		/// <summary>
		/// Initializes the component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			Initiated = true;

			_InputField = new()
			{
				Text = "None, select one in the sidebar or create one to start"
			};

			_InputField.AddThemeConstantOverride("minimum_character_width", 36);
			_InputField.Connect(LineEdit.SignalName.TextChanged, Callable.From((string value) => { _OnMaybeUpdateGroupName(value); }));

			AddChild(_InputField);
		}

		/// <summary>
		/// Updates the input field with the name of the current group.
		/// </summary>
		public void Update()
		{
			if (null == _GlobalExplorer.GroupBuilder._Editor.Group || false == IsInstanceValid(_GlobalExplorer.GroupBuilder._Editor.Group))
			{
				_InputField.Text = "None, select one in the sidebar or create one to start";
				return;
			}

			_InputField.Text = _GlobalExplorer.GroupBuilder._Editor.Group.Name;
		}

		/// <summary>
        /// Handles the event when the input field text changes.
        /// </summary>
        /// <param name="text">The new text entered in the input field.</param>
		private void _OnMaybeUpdateGroupName(string text)
		{
			if (null == _GlobalExplorer.GroupBuilder._Editor.Group)
			{
				_InputField.Text = "None, select one in the sidebar or create one to start";
				return;
			}

			if ("" == text)
			{
				_InputField.Text = _GlobalExplorer.GroupBuilder._Editor.Group.Name;
				return;
			}
		}
	}
}

#endif