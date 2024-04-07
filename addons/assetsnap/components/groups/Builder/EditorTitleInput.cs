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
	public partial class GroupBuilderEditorTitleInput : LibraryComponent
	{
		public GroupBuilderEditorTitleInput()
		{
			Name = "GroupBuilderEditorTitleInput";
			//_include = false;
		}
		
		public LineEdit _InputField;

		public override void Initialize()
		{
			Initiated = true;
			
			_InputField = new()
			{
				Text = "None, select one in the sidebar or create one to start"
			};
			
			_InputField.AddThemeConstantOverride("minimum_character_width", 36);
			_InputField.Connect(LineEdit.SignalName.TextChanged, Callable.From( (string value ) => { _OnMaybeUpdateGroupName( value ); }));

			Container.AddChild(_InputField);
		}
		
		public void Update()
		{
			if( null == _GlobalExplorer ) 
			{
				_GlobalExplorer = GlobalExplorer.GetInstance();
			}
			
			if( null == _GlobalExplorer.GroupBuilder._Editor.Group || false == IsInstanceValid(_GlobalExplorer.GroupBuilder._Editor.Group) ) 
			{
				_InputField.Text = "None, select one in the sidebar or create one to start";
				return;
			}

			_InputField.Text = _GlobalExplorer.GroupBuilder._Editor.Group.Name;
		}
		
		private void _OnMaybeUpdateGroupName(string text)
		{
			if( null == _GlobalExplorer.GroupBuilder._Editor.Group ) 
			{
				_InputField.Text = "None, select one in the sidebar or create one to start";
				return;
			}
			
			if( "" == text ) 
			{
				_InputField.Text = _GlobalExplorer.GroupBuilder._Editor.Group.Name;
				return;
			}
		}

		public override void _ExitTree()
		{
			if( IsInstanceValid( _InputField ) ) 
			{
				// _InputField.TextChanged -= (string value ) => { _OnMaybeUpdateGroupName( value ); };
				_InputField.QueueFree();
			}
			
			base._ExitTree();
		}
	}
}