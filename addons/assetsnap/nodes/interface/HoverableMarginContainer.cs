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

using Godot;

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// Represents a margin container with hoverable behavior.
	/// </summary>
	[Tool]
	public partial class HoverableMarginContainer : MarginContainer
	{
		private Callable? MouseEnterCallable; 
		private Callable? MouseLeaveCallable; 
		private Callable? MouseClickCallable;

		/// <summary>
		/// The color when not hovered.
		/// </summary>
		[Export]
		public Color NormalColor = new Color(28.0f / 255.0f,28.0f / 255.0f,28.0f / 255.0f,0.0f / 255.0f);
		
		/// <summary>
		/// The color when hovered.
		/// </summary>
		[Export]
		public Color HoverColor = new Color(28.0f / 255.0f,28.0f / 255.0f,28.0f / 255.0f,255.0f / 255.0f);
		
		/// <summary>
		/// The color when focused.
		/// </summary>
		[Export]
		public Color FocusColor = new Color(28.0f / 255.0f,28.0f / 255.0f,28.0f / 255.0f,255.0f / 255.0f);
		
		/// <summary>
		/// The text color when not hovered.
		/// </summary>
		[Export]
		public Color TextNormalColor = new Color(255.0f / 255.0f,255.0f / 255.0f,255.0f / 255.0f,255.0f / 255.0f);
		
		/// <summary>
		/// The text color when hovered.
		/// </summary>
		[Export]
		public Color TextHoverColor = new Color(189.0f / 255.0f,189.0f / 255.0f,189.0f / 255.0f,255.0f / 255.0f);
		
		/// <summary>
        /// The text color when focused.
        /// </summary>
		[Export]
		public Color TextFocusColor = new Color(189.0f / 255.0f,189.0f / 255.0f,189.0f / 255.0f,255.0f / 255.0f);
	
		/// <summary>
        /// Configures the hoverable margin container.
        /// </summary>
		public override void _EnterTree()
		{
			MouseEnterCallable = new Callable(this, "_MouseEnter");
			MouseLeaveCallable = new Callable(this, "_MouseLeave");
			MouseClickCallable = new Callable(this, "_MouseClick");
			
			if( MouseEnterCallable is Callable _EnterCallable ) 
			{
				Connect(Control.SignalName.MouseEntered, _EnterCallable);		
			}
			
			if( MouseLeaveCallable is Callable _LeaveCallable ) 
			{
				Connect(Control.SignalName.MouseExited, _LeaveCallable); 
			}
				
			if( MouseClickCallable is Callable _ClickCallable ) 
			{
				Connect(Control.SignalName.GuiInput, _ClickCallable); 
			}
		}

		/// <summary>
        /// Handles transitions to hover color.
        /// </summary>
		public virtual void _MouseEnter()
		{
			Label _Label = GetNode<Label>("VBoxContainer/Label");
			SelfModulate = HoverColor;
			
			if( null == _Label ) 
			{
				GD.PushWarning("No label was found on MouseEnter @ HoverableMarginContainer");
				return;
			}
			 
			_Label.SelfModulate = TextHoverColor;
		}
		
		/// <summary>
        /// Handles transitions back to normal color.
        /// </summary>
		public virtual void _MouseLeave()
		{
			Label _Label = GetNode<Label>("VBoxContainer/Label");
			SelfModulate = NormalColor;
			
			if( null == _Label ) 
			{
				GD.PushWarning("No label was found on MouseEnter @ HoverableMarginContainer");
				return;
			}

			_Label.SelfModulate = TextNormalColor;
		}
		
		/// <summary>
        /// If parent is a select list, on click sets the active of said select list to that of this class.
        /// </summary>
        /// <param name="event">The input event.</param>
		public virtual void _MouseClick( InputEvent @event )
		{
			if( @event is InputEventMouseButton EventMouseButton ) 
			{
				if( EventMouseButton.ButtonIndex == MouseButton.Left && false == EventMouseButton.Pressed ) 
				{
					var _parent = GetParent();
					
					if( _parent is AsSelectList _parentSelect ) 
					{
						if( _parentSelect.GetActive() != this ) 
						{
							_parentSelect.SetActive(this);
						}
						else 
						{
							_parentSelect.SetActive(null);
						}
					}
				}
			}
		}
		
		/// <summary>
        /// Checks if parent is a select list, if so checks if this item equals the active item.
        /// </summary>
        /// <returns>True if active, false otherwise.</returns>
		public virtual bool IsActive()
		{
			var _parent = GetParent();
			
			if( _parent is AsSelectList _parentSelect ) 
			{
				if( _parentSelect.GetActive() == this ) 
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
        /// Cleans up references, parameters, and fields.
        /// </summary>
		public override void _ExitTree()
		{
			if( MouseEnterCallable is Callable _EnterCallable ) 
			{
				if(IsConnected(Control.SignalName.MouseEntered, _EnterCallable)) 
				{
					Disconnect(Control.SignalName.MouseEntered, _EnterCallable);		
				}
			}
			
			if( MouseLeaveCallable is Callable _LeaveCallable ) 
			{
				if(IsConnected(Control.SignalName.MouseExited, _LeaveCallable)) 
				{
					Disconnect(Control.SignalName.MouseExited, _LeaveCallable);		
				}
			}
				 
			if( MouseClickCallable is Callable _ClickCallable ) 
			{
				if(IsConnected(Control.SignalName.GuiInput, _ClickCallable)) 
				{
					Disconnect(Control.SignalName.GuiInput, _ClickCallable);		
				}
			}

			MouseEnterCallable = null;
			MouseLeaveCallable = null;
			MouseClickCallable = null;
			
			base._ExitTree();
		}
	}
}
