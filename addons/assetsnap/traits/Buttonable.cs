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
using Godot;
namespace AssetSnap.Component
{
	
	[Tool]
	public partial class Buttonable : Trait.Base
	{
		/*
		** Enums
		*/
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
		
		/*
		** Exports
		*/
		[Export]
		public Godot.Collections.Array<Callable> _Actions = new Godot.Collections.Array<Callable>();
		
		/*
		** Private
		*/
		private new Godot.Collections.Dictionary<string, int> Margin = new()
		{
			{"left", 20},
			{"right", 20},
			{"top", 0},
			{"bottom", 25},
		};
		private new Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;
		private new Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
		private ButtonType WorkingButtonType = ButtonType.DefaultButton;
		private Control.CursorShape DefaultCursorShape = Control.CursorShape.PointingHand;
		private Control.MouseFilterEnum MouseFilter = Control.MouseFilterEnum.Pass;
		private Texture2D Icon;
		private Theme Theme;
		private HorizontalAlignment IconAlignment;
		private string Text = "";
		private string TooltipText = "";

		public Buttonable()
		{
			Name = "Buttonable";
			TypeString = GetType().ToString();
		}
		
		/*
		** Public methods
		*/
		
		/*
		** Instantiate an instance of the trait
		**
		** @return Buttonable
		*/	
		public Buttonable Instantiate()
		{
			base._Instantiate();
			
			// Setup the button
			Button WorkingButton = new()
			{
				Name = TraitName,
				Text = Text,
				TooltipText = TooltipText,
				ThemeTypeVariation = WorkingButtonType.ToString(),
				MouseDefaultCursorShape = DefaultCursorShape,
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
				Visible = Visible,
				MouseFilter = MouseFilter,
				Theme = Theme,
			};
			
			if( null != Icon ) 
			{
				WorkingButton.Icon = Icon;
				WorkingButton.IconAlignment = IconAlignment;
			}

			WorkingButton.SetMeta("index", Iteration);

			// Connect the button to it's action
			if( _Actions.Count > Iteration ) 
			{
				Callable actionCallable = _Actions[Iteration];
				Godot.Error error = WorkingButton.Connect( Button.SignalName.Pressed, actionCallable);
				if( error != Godot.Error.Ok)
				{
					GD.Print("Error connecting signal: " + error.ToString());
				}
			}

			Dependencies[TraitName + "_WorkingNode"] = WorkingButton;
			
			// Add the button to the nodes array			
			Plugin.Singleton.traitGlobal.AddInstance(Iteration, WorkingButton, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.traitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);

			// Increase Iteration and clear the trait
			Iteration += 1;
			Reset();
		
			Dependencies = new();
			
			return this;
		}
		
		/*
		** Selects an placed button in the
		** nodes array by Iteration
		**
		** @param int index
		** @return Buttonable
		*/
		public Buttonable Select(int index)
		{			
			base._Select(index);
			
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode")) 
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.traitGlobal.GetDependencies(index, TypeString, OwnerName);
				Dependencies = dependencies;
			}
			
			return this;
		}
		
		/*
		** Selects an placed button in the
		** nodes array by name
		**
		** @param string name
		** @return Buttonable
		*/
		public Buttonable SelectByName( string name ) 
		{
			foreach( Button button in Nodes ) 
			{
				if( button.Name == name ) 
				{
					Dependencies[TraitName + "_WorkingNode"] = button;
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
			if( false == Dependencies.ContainsKey(TraitName + "_WorkingNode")) 
			{
				return;
			}
			
			_AddToContainer(Container, Dependencies[TraitName + "_WorkingNode"].As<Button>());
		}
		
		/*
		** Setter Methods
		*/
		
		/*
		** Sets the name of the current button
		**
		** @param string text
		** @return Buttonable
		*/
		public Buttonable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/*
		** Sets the text of the current button
		**
		** @param string text
		** @return Buttonable
		*/
		public Buttonable SetText( string text ) 
		{
			if( text == "" ) 
			{
				TraitName = "ButtonableIcon-" + Iteration;
				return this;
			}
			
			Text = text;
			TraitName = text;
			
			return this;
		}
		
		/*
		** Sets the tooltip text of the current button
		**
		** @param string text
		** @return Buttonable
		*/
		public Buttonable SetTooltipText( string text ) 
		{
			TooltipText = text;
			
			return this;
		}
		
		public Buttonable SetTheme( Theme theme ) 
		{
			Theme = theme;
				
			return this;
		}
		
		public Buttonable SetMouseFilter(Control.MouseFilterEnum filter)
		{
			MouseFilter = filter;
			return this;
		}
		
		/*
		** Sets the visibility state of the
		** currently chosen button
		**
		** @param bool state
		** @return Buttonable
		*/
		public Buttonable SetVisible( bool state ) 
		{
			base._SetVisible(state);
			
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode") && Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() is Button button) 
			{
				button.Visible = state;
			}
			
			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Buttonable
		*/
		public override Buttonable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}
		
		/*
		** Sets the vertical size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Buttonable
		*/
		public override Buttonable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}
		
		/*
		** Sets the theme type of the button,
		** which lays out a set of specified rules
		** from the theme that the button follows
		**
		** @param ButtonType type
		** @return Buttonable
		*/
		public Buttonable SetType( ButtonType type ) 
		{
			WorkingButtonType = type;
			
			return this;
		}
		
		/*
		** Sets the cursorshape of the
		** currently chosen button
		**
		** @param Control.CursorShape shape
		** @return Buttonable
		*/
		public Buttonable SetCursorShape( Control.CursorShape shape ) 
		{
			DefaultCursorShape = shape;
			
			return this;
		}
		
		/*
		** Sets the icon of the
		** currently chosen button
		**
		** @param Texture2D icon
		** @return Buttonable
		*/
		public Buttonable SetIcon( Texture2D icon )
		{
			Icon = icon;
			
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode") && Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() is Button button) 
			{
				button.Icon = icon;
			}

			return this;
		}
		
		/*
		** Sets the icon alignment
		** of the currently chosen
		** button
		**
		** @param HorizontalAlignment alignment
		** @return Buttonable
		*/
		public Buttonable SetIconAlignment( HorizontalAlignment alignment )
		{
			IconAlignment = alignment;
			
			return this;
		}
		
		/*
		** Sets the action for the
		** currently chosen button
		**
		** @param Action action
		** @return Buttonable
		*/
		public Buttonable SetAction( Action action ) 
		{
			_Actions.Add(Callable.From(action));
			
			return this;
		}
		
		/*
		** Sets margin values for 
		** the currently chosen button
		**
		** @param int value
		** @param string side
		** @return Buttonable
		*/
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
		
		/*
		** Booleans
		*/
		
		/*
		** Checks if the currently
		** chosen button is visible
		**
		** @return bool
		*/
		public bool IsVisible()
		{
			return Visible;
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
			Text = "";
			TooltipText = "";
			WorkingButtonType = ButtonType.DefaultButton;
			DefaultCursorShape = Godot.Control.CursorShape.PointingHand;
		}
	}
}
#endif