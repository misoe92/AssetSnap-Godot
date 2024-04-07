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
	public partial class Spinboxable : Trait.Base
	{
		/*
		** Public
		*/
		public MarginContainer _marginContainer;
		public VBoxContainer _innerContainer;
		
		/*
		** Private
		*/
		private List<Callable?> _Actions = new();
		private Callable? _Action;
		private string _Prefix = "";
		private string TooltipText = "";
		private float _Step = 1;
		private float MinimumValue = 0;
		private float MaximumValue = 0;
		private double DefaultValue = 0;
		
		/*
		** Public methods
		*/
		
		/*
		** Instantiate an instance of the trait
		**
		** @return Spinboxable
		*/
		public Spinboxable Instantiate()
		{
			base._Instantiate( GetType().ToString() );

			_marginContainer = new()
			{
				Name ="SpinBoxMargin",
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical
			};
			_innerContainer = new()
			{
				Name ="SpinBoxBoxContainer",
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical
			};
				
			foreach( (string side, int value ) in Margin ) 
			{
				_marginContainer.AddThemeConstantOverride("margin_" + side, value);
			}
			
			SpinBox WorkingInput = new()
			{
				Name = Name,
				Prefix = _Prefix,
				Step = _Step,
				TooltipText = TooltipText,
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical
			};
		
			if( MaximumValue != 0 ) 
			{
				WorkingInput.MaxValue = MaximumValue;
			}
			
			if( MinimumValue != 0 ) 
			{
				WorkingInput.MinValue = MinimumValue;
			}
				
			if( 0 != DefaultValue ) 
			{
				WorkingInput.Value = DefaultValue;
			}
			
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
				WorkingInput.Connect(SpinBox.SignalName.ValueChanged, _callable);
			}

			Nodes.Add(WorkingInput);
			_Actions.Add(_Action);
			
			WorkingNode = WorkingInput;

			_innerContainer.AddChild(WorkingInput);
			_marginContainer.AddChild(_innerContainer);

			Reset();
			
			return this;
		}
		
		/*
		** Selects an placed spinbox in the
		** nodes array by index
		**
		** @param int index
		** @return Spinboxable
		*/
		public Spinboxable Select(int index)
		{
			base._Select(index);
			
			if( IsInstanceValid(WorkingNode) && WorkingNode is SpinBox InputNode )
			{
				_Action = _Actions[index];
				
				if( IsInstanceValid(InputNode.GetParent()) ) 
				{
					_innerContainer = InputNode.GetParent() as VBoxContainer;
					if( IsInstanceValid(_innerContainer.GetParent()) ) 
					{
						_marginContainer = _innerContainer.GetParent() as MarginContainer;
					}
				}
			}

			return this;
		}
		
		/*
		** Selects an placed spinbox in the
		** nodes array by name
		**
		** @param string name
		** @return Spinboxable
		*/
		public Spinboxable SelectByName( string name ) 
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
		** Sets the name of the current spinbox
		**
		** @param string text
		** @return Spinboxable
		*/
		public Spinboxable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/*
		** Sets the prefix of the current spinbox
		**
		** @param string Prefix
		** @return Spinboxable
		*/
		public Spinboxable SetPrefix( string Prefix ) 
		{
			_Prefix = Prefix;
			
			return this;
		}
		
		/*
		** Sets the tooltip text of the current spinbox
		**
		** @param string text
		** @return Spinboxable
		*/
		public Spinboxable SetTooltipText( string text ) 
		{
			TooltipText = text;
			
			return this;
		}
		
		/*
		** Sets the step value of the current spinbox
		**
		** @param float StepSize
		** @return Spinboxable
		*/
		public Spinboxable SetStep( float StepSize ) 
		{
			_Step = StepSize;
			
			return this;
		}
		
		/*
		** Sets the minimum value of the current spinbox
		**
		** @param float minValue
		** @return Spinboxable
		*/
		public Spinboxable SetMinValue(float minValue ) 
		{
			MinimumValue = minValue;
			
			return this;
		}
		
		/*
		** Sets the maximum value of the current spinbox
		**
		** @param float maxValue
		** @return Spinboxable
		*/
		public Spinboxable SetMaxValue(float maxValue ) 
		{
			MaximumValue = maxValue;
			
			return this;
		}
		
		/*
		** Sets the value of the current spinbox
		**
		** @param double value
		** @return Spinboxable
		*/
		public Spinboxable SetValue( double value )
		{
			DefaultValue = value;
			
			if( IsInstanceValid(WorkingNode) && WorkingNode is SpinBox WorkingInput) 
			{
				WorkingInput.Value = value;
			}

			return this;
		}
		
		/*
		** Sets the visibility state of the current spinbox
		**
		** @param bool state
		** @return Spinboxable
		*/
		public Spinboxable SetVisible(bool state)
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
		** Sets the action of the current spinbox
		**
		** @param Callable action
		** @return Spinboxable
		*/
		public Spinboxable SetAction( Callable action ) 
		{
			_Action = action;
			
			return this;
		}
		
		/*
		** Sets the dimensions of the current spinbox
		**
		** @param int width
		** @param int height
		** @return Spinboxable
		*/
		public override Spinboxable SetDimensions( int width, int height )
		{
			base.SetDimensions(width,height);
			
			return this;
		}
				
		/*
		** Sets the vertical size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Spinboxable
		*/
		public override Spinboxable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}
		
		/*
		** Sets the vertical size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Spinboxable
		*/
		public override Spinboxable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}
		
		/*
		** Sets margin values for 
		** the currently chosen spinbox
		**
		** @param int value
		** @param string side
		** @return Spinboxable
		*/
		public Spinboxable SetMargin( int value, string side = "" ) 
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
			
			return this;
		}
		
		/*
		** Getter Methods
		*/
		
		/*
		** Fetches the value of the current spinbox
		**
		** @return double
		*/
		public double GetValue()
		{
			if( IsInstanceValid(WorkingNode) && WorkingNode is SpinBox WorkingInput) 
			{
				return WorkingInput.Value;
			}

			return 0;
		}
		
		/*
		** Booleans
		*/
		
		/*
		** Checks if the current spinbox is visible
		**
		** @return bool
		*/
		public bool IsVisible()
		{
			if (null != _marginContainer && IsInstanceValid(_marginContainer))
			{
				return _marginContainer.Visible;
			}
			
			return false;
		}
		
		/*
		** Private methods
		*/
		
		/*
		** Resets the trait to
		** a cleared state
		**
		** @return void
		*/
		private Spinboxable Reset()
		{
			Size = Vector2.Zero;
			CustomMinimumSize = Vector2.Zero;
			_Prefix = "";
			TooltipText = "";
			DefaultValue = 0;
			_Step = 1;
			MinimumValue = 0;
			MaximumValue = 0;
			
			WorkingNode = null;
			_Action = null;
			_marginContainer = null;
			_innerContainer = null;
			
			return this;
		}
		
		/*
		** Cleanup
		*/
		public override void _ExitTree()
		{
			Reset();
			 
			base._ExitTree();
		}
	}
}
#endif