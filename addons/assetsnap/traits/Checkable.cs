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
using System.Collections.Generic;
using Godot;
namespace AssetSnap.Component
{
	[Tool]
	public partial class Checkable : Trait.Base
	{
		private List<Callable?> _Actions = new();
		private Callable? _Action;
		private Vector2 Size;
		private Vector2 CustomMinimumSize;

		private string Text = "";
		private string TooltipText = "";

		private bool ButtonPressed = false;
		
		private Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		private Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;

		public MarginContainer _marginContainer;

		public VBoxContainer _innerContainer;
		
		public Checkable Instantiate()
		{
			base._Instantiate( GetType().ToString() );

			_marginContainer = new()
			{
				Name="CheckboxMarginContainer",
				Visible = Visible,
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical
			};
			
			_innerContainer = new()
			{
				Name="CheckboxBoxContainer",
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical
			};
				
			foreach( (string side, int value ) in Margin ) 
			{
				_marginContainer.AddThemeConstantOverride("margin_" + side, value);
			}

			CheckBox WorkingInput = new()
			{
				Name = Name,
				Text = Text,
				TooltipText = TooltipText,
				ButtonPressed = ButtonPressed,
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical
			};

			if( Vector2.Zero != CustomMinimumSize ) 
			{
				WorkingInput.CustomMinimumSize = CustomMinimumSize;
			}
			
			if( Vector2.Zero != Size ) 
			{
				WorkingInput.Size = Size;
			}

			if( _Action is Callable _callable ) 
			{
				WorkingInput.Connect(CheckBox.SignalName.Pressed, _callable);
			}

			_innerContainer.AddChild(WorkingInput);
			_marginContainer.AddChild(_innerContainer);
			WorkingNode = WorkingInput;
			
			Nodes.Add(WorkingNode);
			_Actions.Add(_Action);
			
			Reset();
			
			return this;
		}
		
		public bool HasNodes()
		{
			return null != Nodes && Nodes.Count != 0;
		}
		
		public Checkable Select(int index)
		{
			base._Select(index);
			
			if(
				false == _select ||
				false == IsInstanceValid( WorkingNode )
			) 
			{
				return this;
			}

			if( WorkingNode is CheckBox InputNode )
			{
				if( index < _Actions.Count ) 
				{
					_Action = _Actions[index];
				}
				
				if( IsInstanceValid( InputNode.GetParent() ) ) 
				{
					_innerContainer = InputNode.GetParent() as VBoxContainer;
					if( IsInstanceValid( _innerContainer.GetParent() ) ) 
					{
						_marginContainer = _innerContainer.GetParent() as MarginContainer;
					}
				}
			}
			
			return this;
		}
		
		public Checkable SelectByName( string name ) 
		{
			foreach( Button button in Nodes ) 
			{
				if( button.Name == name ) 
				{
					WorkingNode = button;
					break;
				}
			}

			return this;
		}
		
		public override bool IsValid()
		{
			return base.IsValid();
		}

		public bool IsVisible()
		{
			if (null != _marginContainer && IsInstanceValid(_marginContainer))
			{
				return _marginContainer.Visible;
			}
			else 
			{
				return false;
			}
		}
		
		public Checkable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;

			return this;
		}
		
		public Checkable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;

			return this;
		}
		
		public Checkable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		public Checkable SetText( string text ) 
		{
			Text = text;
			
			return this;
		}
		
		public Checkable SetTooltipText( string text ) 
		{
			TooltipText = text;
			
			return this;
		}
		
		public Checkable SetAction( Callable action ) 
		{
			_Action = action;			
			return this;
		}
				
		public Checkable SetVisible( bool state ) 
		{
			if( null != _marginContainer && IsInstanceValid( _marginContainer ) )  
			{
				_marginContainer.Visible = state;
			}
			else 
			{
				Visible = state;
			}

			return this;
		}
		
		public Checkable SetDimensions( int width, int height )
		{
			CustomMinimumSize = new Vector2( width, height);
			Size = new Vector2( width, height);

			return this;
		}
		
		
		public Checkable SetMargin( int value, string side = "" ) 
		{
			if( side == "" ) 
			{
				Margin["top"] = value;
				Margin["bottom"] = value;
				Margin["left"] = value;
				Margin["right"] = value;
			}
			else 
			{
				Margin[side] = value;
			}
			
			if( null != _marginContainer)  
			{
				_marginContainer.AddThemeConstantOverride("margin_" + side, value);
			}
			
			return this;
		}
		
		public bool GetValue()
		{
			if( IsInstanceValid( WorkingNode ) && WorkingNode is CheckBox WorkingInput) 
			{
				// GD.PushError(Name);
				return WorkingInput.ButtonPressed;
			}
			
			return false;
		}
		
		public Checkable SetValue( bool value )
		{
			if( IsInstanceValid( WorkingNode ) && WorkingNode is CheckBox WorkingInput) 
			{
				WorkingInput.ButtonPressed = value;
			}
			else 
			{
				ButtonPressed = value;
			}

			return this;
		}
		
		public void AddToContainer( Node Container )
		{
			if( null != WorkingNode ) 
			{
				// Single placement
				Container.AddChild(_marginContainer);
			}
			else 
			{
				// Multi placement
				for( int i = 0; i<Nodes.Count;i++ )
				{
					Select(i);
					Container.AddChild(_marginContainer);
				}
			}
		}
		
		public void FreeAction( Callable callable ) 
		{
			WorkingNode.Disconnect(CheckBox.SignalName.Pressed, callable);
		}
		
		private void Reset()
		{
			WorkingNode = null;
			_marginContainer = null;
			_innerContainer = null;
			_Action = null; 
		}
		
		public override void _ExitTree()
		{
			Nodes = new();
			disposed = true;
			Reset();
			 
			base._ExitTree();
		}
	}
}
#endif