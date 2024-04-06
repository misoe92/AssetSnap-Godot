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
using System;
using System.Collections.Generic;
using Godot;
namespace AssetSnap.Component
{
	
	[Tool]
	public partial class Buttonable : Trait.Base
	{
		[Export]
		public Godot.Collections.Array<Callable> _Actions = new Godot.Collections.Array<Callable>();

		public enum ButtonType
		{
			DefaultButton,
			ActionButton,
			DangerButton,
			SuccesButton,
			FlatButton,
			DisabledButton,
			SmallDefaultButton,
			SmallActionButton,
			SmallDangerButton,
			SmallSuccesButton,
			SmallFlatButton,
			SmallDisabledButton,
		}
		
		public new Godot.Collections.Dictionary<string, int> Margin = new()
		{
			{"left", 20},
			{"right", 20},
			{"top", 0},
			{"bottom", 25},
		};
		
		private Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;
		private Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
		
		public string Text = "";
		public string TooltipText = "";
		public int index = 0;

		public ButtonType WorkingButtonType = ButtonType.DefaultButton;
		public Godot.Control.CursorShape DefaultCursorShape = Godot.Control.CursorShape.PointingHand;

		public Texture2D Icon;
		public HorizontalAlignment IconAlignment;
		
		public Buttonable()
		{
			// _Actions += () => { GD.Print("Change found")};
		}
		
		public Buttonable Instantiate()
		{
			base._Instantiate( GetType().ToString() );
			
			Button WorkingButton = new()
			{
				Name = Name,
				Text = Text,
				TooltipText = TooltipText,
				ThemeTypeVariation = WorkingButtonType.ToString(),
				MouseDefaultCursorShape = DefaultCursorShape,
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
				Visible = Visible
			};
			
			if( null != Icon ) 
			{
				WorkingButton.Icon = Icon;
				WorkingButton.IconAlignment = IconAlignment;
			}

			WorkingButton.SetMeta("index", index);
		
			Callable actionCallable = _Actions[index];
			Godot.Error error = WorkingButton.Connect( Button.SignalName.Pressed, actionCallable);
			
			if( error != Godot.Error.Ok)
			{
				GD.Print("Error connecting signal: " + error.ToString());
			}
			
			Nodes.Add(WorkingButton);
			WorkingNode = WorkingButton;

			index += 1;
			Reset();
		
			return this;
		}
		
		public Buttonable Select(int index)
		{
			base._Select(index);
			
			return this;
		}
		
		public Buttonable SelectByName( string name ) 
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
		
		public Buttonable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		public Buttonable SetText( string text ) 
		{
			Text = text;
			
			return this;
		}
		
		public Buttonable SetTooltipText( string text ) 
		{
			TooltipText = text;
			
			return this;
		}
		
		public Buttonable SetVisible( bool state ) 
		{
			base._SetVisible(state);
			
			if( null != WorkingNode && WorkingNode is Button button) 
			{
				button.Visible = state;
			}
			
			return this;
		}
		
		public Buttonable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;

			return this;
		}
		
		public Buttonable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;

			return this;
		}
		
		public Buttonable SetType( ButtonType type ) 
		{
			WorkingButtonType = type;
			
			return this;
		}
		
		public Buttonable SetCursorShape( Godot.Control.CursorShape shape ) 
		{
			DefaultCursorShape = shape;
			
			return this;
		}
		
		public Buttonable SetIcon( Texture2D icon )
		{
			Icon = icon;

			return this;
		}
		
		public Buttonable SetIconAlignment( HorizontalAlignment alignment )
		{
			IconAlignment = alignment;
			
			return this;
		}
		
		public Buttonable SetAction( Action action ) 
		{
			_Actions.Add(Callable.From(action));
			
			return this;
		}
		
		public Buttonable SetMargin( int value, string side = "" ) 
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
		
		public bool IsVisible()
		{
			return Visible;
		}
		
		public void AddToContainer( Node Container )
		{
			if( null != WorkingNode ) 
			{
				// Single placement
				Container.AddChild(WorkingNode);
			}
			else 
			{
				MarginContainer _Container = new()
				{
					SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
					SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin,
				};
				
				foreach( (string side, int value ) in Margin ) 
				{
					_Container.AddThemeConstantOverride("margin_" + side, value);
				}
				 
				// Multi placement
				foreach( Button button in Nodes ) 
				{
					_Container.AddChild(button);
				}

				base._AddToContainer(Container, _Container);
			}
		}
		
		private void Reset()
		{
			Text = "";
			TooltipText = "";
			WorkingNode = null;
			WorkingButtonType = ButtonType.DefaultButton;
			DefaultCursorShape = Godot.Control.CursorShape.PointingHand;
		}
		
		private void _ClearNodes()
		{
			for( int i = 0; i < Nodes.Count; i++ ) 
			{
				Select(i); 
				WorkingNode.QueueFree();
			}
		}

		public override void _ExitTree()
		{
			base._ExitTree(); 
			// Reset();
		}
	}
}
#endif