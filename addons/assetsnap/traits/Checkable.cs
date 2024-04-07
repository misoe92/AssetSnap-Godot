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
		/*
		** Public
		*/
		public MarginContainer _marginContainer;
		public VBoxContainer _innerContainer;
		
		/*
		** Private
		*/
		private Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		private Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		private List<Callable?> _Actions = new();
		private Callable? _Action;
		private Vector2 Size;
		private Vector2 CustomMinimumSize;
		
		private string Text = "";
		private string TooltipText = "";
		private bool ButtonPressed = false;
		
		/*
		** Public methods
		*/
		
		/*
		** Instantiate an instance of the trait
		**
		** @return Checkable
		*/
		public Checkable Instantiate()
		{
			base._Instantiate( GetType().ToString() );

			// Setup the containers
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

			// Setup the checkbox
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

			// Setup node structure
			_innerContainer.AddChild(WorkingInput);
			_marginContainer.AddChild(_innerContainer);
			
			// Connect the button to it's action
			if( _Action is Callable _callable ) 
			{
				WorkingInput.Connect(CheckBox.SignalName.Pressed, _callable);
			}
			
			// Add the button to the nodes array			
			Nodes.Add(WorkingInput);
			
			// Add the action to the actions array			
			_Actions.Add(_Action);
			
			// Clear the trait
			Reset();
			
			return this;
		}
		
		/*
		** Selects an placed checkbox in the
		** nodes array by index
		**
		** @param int index
		** @return Checkable
		*/
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
		
		/*
		** Selects an placed checkbox in the
		** nodes array by name
		**
		** @param string name
		** @return Checkable
		*/
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
		
		/*
		** Adds the currently chosen button
		** to a specified container
		**
		** @param Node Container
		** @return void
		*/
		public void AddToContainer( Node Container )
		{
			_AddToContainer(Container, _marginContainer);
		}
		
		/*
		** Setter Methods
		*/
		
		/*
		** Sets the name of the current checkbox
		**
		** @param string text
		** @return Checkable
		*/
		public Checkable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/*
		** Sets the text of the current checkbox
		**
		** @param string text
		** @return Checkable
		*/
		public Checkable SetText( string text ) 
		{
			Text = text;
			
			return this;
		}
		
		/*
		** Sets the tooltip text of the current checkbox
		**
		** @param string text
		** @return Checkable
		*/
		public Checkable SetTooltipText( string text ) 
		{
			TooltipText = text;
			
			return this;
		}
		
		/*
		** Sets the value of the current checkbox
		**
		** @param bool value
		** @return Checkable
		*/
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
		
		/*
		** Sets the visibility state of the
		** currently chosen checkbox
		**
		** @param bool state
		** @return Checkable
		*/
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
		
		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Checkable
		*/
		public Checkable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;

			return this;
		}
		
		/*
		** Sets the vertical size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Checkable
		*/
		public Checkable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;

			return this;
		}
		
		/*
		** Sets the callable for the
		** currently chosen checkbox
		**
		** @param Callable action
		** @return Checkable
		*/
		public Checkable SetAction( Callable action ) 
		{
			_Action = action;			
			return this;
		}
		
		/*
		** Sets margin values for 
		** the currently chosen checkbox
		**
		** @param int value
		** @param string side
		** @return Checkable
		*/
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
		
		/*
		** Sets the dimensions for the checkbox
		**
		** @param int width
		** @param int height
		** @return Checkable
		*/
		public Checkable SetDimensions( int width, int height )
		{
			CustomMinimumSize = new Vector2( width, height);
			Size = new Vector2( width, height);

			return this;
		}
		
		/*
		** Getter Methods
		*/
		
		/*
		** Fetches the value of
		** the current checkbox
		**
		** @return bool
		*/
		public bool GetValue()
		{
			if( IsInstanceValid( WorkingNode ) && WorkingNode is CheckBox WorkingInput) 
			{
				// GD.PushError(Name);
				return WorkingInput.ButtonPressed;
			}
			
			return false;
		}
		
		/*
		** Booleans
		*/
		
		/*
		** Checks if any nodes exists
		**
		** @return bool
		*/
		public bool HasNodes()
		{
			return null != Nodes && Nodes.Count != 0;
		}
		
		/*
		** Checks if the current checkbox is visible
		**
		** @return bool
		*/
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
		
		/*
		** Private
		*/
		
		/*
		** Resets the trait to
		** a cleared state
		**
		** @return void
		*/
		private void Reset()
		{
			WorkingNode = null;
			_marginContainer = null;
			_innerContainer = null;
			_Action = null; 
		}
		
		/*
		** Cleanup
		*/
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