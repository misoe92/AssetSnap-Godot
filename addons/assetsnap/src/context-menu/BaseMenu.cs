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
namespace AssetSnap.ContextMenu
{
	using System;
	using AssetSnap.Front.Nodes;
	using Godot;

	public partial class BaseMenu
	{
		protected readonly string DefaultButtonText = "";
		protected PopupMenu _PopupMenu;
		protected Vector2I popupPosition;
		
		/* Container */
		protected VBoxContainer _BoxContainer;
		
		/* Button */
		protected Button _Button;

		/* Items */
		protected string[] items = Array.Empty<string>();
		protected bool Visible = false;
		
		public virtual void Initialize()
		{
			return;
		}
		
		public void Spawn()
		{
			_BoxContainer.AddChild(_Button);
			_BoxContainer.AddChild(_PopupMenu);

			_Button.Connect(PopupMenu.SignalName.IndexPressed, Callable.From( () => { _onDropdownItemSelected(); } ) ); 
			_Button.Connect(Button.SignalName.Pressed, Callable.From( () => { _TogglePopupMenu(); } ) ); 
			
			GlobalExplorer.GetInstance()._Plugin.AddControlToContainer(EditorPlugin.CustomControlContainer.SpatialEditorMenu, _BoxContainer ); 
		}
		
		public void Show()
		{
			Vector2 buttonGlobalPosition = _BoxContainer.GlobalPosition;
			popupPosition = (Vector2I)(buttonGlobalPosition + new Vector2(0, _Button.Size.Y + 5));
			// Set the position of the PopupMenu just below the button
			_PopupMenu.Position = popupPosition;
			
			_PopupMenu.Popup();
			Visible = true;
		}
		
		public void Hide()
		{
			_PopupMenu.Hide();
			Visible = false;
		}
		
		public bool IsHidden()
		{
			return Visible == false;
		}
		
		public bool ShouldShow()
		{
			return false;
		}
		
		private void _TogglePopupMenu()
		{
			if ( IsHidden() ) 
			{
				Show();
			}
			else 
			{
				Hide();
			}
		}
		
		public virtual void _onDropdownItemSelected()
		{
			Visible = false;
		}
		
		public void _Exit()
		{
			if( EditorPlugin.IsInstanceValid(_Button) ) 
			{
				_Button.QueueFree();
			}
			
			if( EditorPlugin.IsInstanceValid(_PopupMenu) ) 
			{
				_PopupMenu.QueueFree();
			}
			if( EditorPlugin.IsInstanceValid(_BoxContainer) ) 
			{
				_BoxContainer.QueueFree();
			}
		}
	} 
}
#endif