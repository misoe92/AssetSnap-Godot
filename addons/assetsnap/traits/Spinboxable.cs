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
		private List<Callable?> _Actions = new();
		private Callable? _Action;
		
		private Vector2 Size;
		private Vector2 CustomMinimumSize;
		private string _Prefix = "";
		private string TooltipText = "";
		private float _Step = 1;
		private float MinimumValue = 0;
		private float MaximumValue = 0;
		private double DefaultValue = 0;

		private MarginContainer _marginContainer;

		private VBoxContainer _innerContainer;
		
		public Spinboxable Instantiate()
		{
			base._Instantiate( GetType().ToString() );

			_marginContainer = new()
			{
				Name ="SpinBoxMargin",
			};
			_innerContainer = new()
			{
				Name ="SpinBoxBoxContainer",
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
		public override bool IsValid()
		{
			if( null != Nodes || Nodes.Count > 0 ) 
			{
				return false;
			}
			
			return base.IsValid();
		}
		
		public bool IsVisible()
		{
			if (null != _marginContainer && IsInstanceValid(_marginContainer))
			{
				return _marginContainer.Visible;
			}
			
			return false;
		}
		
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
		
		public Spinboxable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		public Spinboxable SetTooltipText( string text ) 
		{
			TooltipText = text;
			
			return this;
		}
		
		public Spinboxable SetAction( Callable action ) 
		{
			_Action = action;
			
			return this;
		}
		public Spinboxable SetPrefix( string Prefix ) 
		{
			_Prefix = Prefix;
			
			return this;
		}
		public Spinboxable SetStep( float StepSize ) 
		{
			_Step = StepSize;
			
			return this;
		}
		
		public Spinboxable SetMinValue(float minValue ) 
		{
			MinimumValue = minValue;
			
			return this;
		}
		
		public Spinboxable SetMaxValue(float maxValue ) 
		{
			MaximumValue = maxValue;
			
			return this;
		}
		
		public Spinboxable SetDimensions( int width, int height )
		{
			CustomMinimumSize = new Vector2( width, height);
			Size = new Vector2( width, height);

			return this;
		}
		
		
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
		
		public double GetValue()
		{
			if( IsInstanceValid(WorkingNode) && WorkingNode is SpinBox WorkingInput) 
			{
				return WorkingInput.Value;
			}

			return 0;
		}
		
		public Spinboxable SetValue( double value )
		{
			DefaultValue = value;
			
			if( IsInstanceValid(WorkingNode) && WorkingNode is SpinBox WorkingInput) 
			{
				WorkingInput.Value = value;
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
		
		public Spinboxable Reset()
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
		
		public override void _ExitTree()
		{
			Reset();
			 
			base._ExitTree();
		}
	}
}
#endif