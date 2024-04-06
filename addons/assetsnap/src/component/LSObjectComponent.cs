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

namespace AssetSnap.Component
{
	using Godot;

	public partial class LSObjectComponent : LibraryComponent
	{
		protected MarginContainer _MarginContainer;
		protected VBoxContainer _InnerContainer;
		protected Label _Label;
		
		public LSObjectComponent()
		{
			Name = "LSObjectComponent";
			//_include = false;
		}
		
		public virtual void SetVisible( bool state ) 
		{
			_MarginContainer.Visible = state;
		}
		
		public virtual bool IsVisible() 
		{
			return true == _MarginContainer.Visible;
		}
		
		public bool IsSnapToObject()
		{
			return _GlobalExplorer.States.SnapToObject == GlobalStates.LibraryStateEnum.Enabled;
		}

		public override void _ExitTree()
		{
			if( IsInstanceValid(_Label) ) 
			{
				_Label.QueueFree();
				_Label = null;
			}
			
			if( IsInstanceValid(_InnerContainer) ) 
			{
				_InnerContainer.QueueFree();
				_InnerContainer = null;
			}
			
			if( IsInstanceValid(_MarginContainer) ) 
			{
				_MarginContainer.QueueFree();
				_MarginContainer = null;
			}
			
			base._ExitTree();
		}
	}
}