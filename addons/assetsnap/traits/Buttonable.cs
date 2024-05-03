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
	/// <summary>
	/// A tool class for creating customizable buttons with various functionalities.
	/// </summary>	
	[Tool]
	public partial class Buttonable : Trait.Base
	{
		/*
		** Enums
		*/
		
		/// <summary>
		/// Enumeration of different button types.
		/// </summary>
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
		** Private fields
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

		/// <summary>
		/// Constructor for the Buttonable class.
		/// </summary>
		public Buttonable()
		{
			Name = "Buttonable";
			TypeString = GetType().ToString();
		}
		
		/// <summary>
		/// Instantiate an instance of the trait.
		/// </summary>
		/// <returns>Returns the instantiated Buttonable instance.</returns>	
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
		
		/// <summary>
		/// Selects a placed button in the nodes array by Iteration.
		/// </summary>
		/// <param name="index">The index of the button to select.</param>
		/// <returns>Returns the updated Buttonable instance.</returns>
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
		
		/// <summary>
		/// Selects a placed button in the nodes array by name.
		/// </summary>
		/// <param name="name">The name of the button to select.</param>
		/// <returns>Returns the updated Buttonable instance.</returns>
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
		
		/// <summary>
		/// Adds the currently chosen button to a specified container.
		/// </summary>
		/// <param name="Container">The container to which the chosen button will be added.</param>
		public void AddToContainer( Node Container )
		{
			if( false == Dependencies.ContainsKey(TraitName + "_WorkingNode")) 
			{
				return;
			}
			
			_AddToContainer(Container, Dependencies[TraitName + "_WorkingNode"].As<Button>());
		}
		
		/// <summary>
		/// Sets the name of the current button.
		/// </summary>
		/// <param name="text">The name to set.</param>
		/// <returns>Returns the updated Buttonable instance.</returns>
		public Buttonable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/// <summary>
		/// Sets the text of the current button.
		/// </summary>
		/// <param name="text">The text to set.</param>
		/// <returns>Returns the updated Buttonable instance.</returns>
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
		
		/// <summary>
		/// Sets the tooltip text of the current button.
		/// </summary>
		/// <param name="text">The tooltip text to set.</param>
		/// <returns>Returns the updated Buttonable instance.</returns>
		public Buttonable SetTooltipText( string text ) 
		{
			TooltipText = text;
			
			return this;
		}
		
		/// <summary>
		/// Sets the theme of the current button.
		/// </summary>
		/// <param name="theme">The theme to set.</param>
		/// <returns>Returns the updated Buttonable instance.</returns>
		public Buttonable SetTheme( Theme theme ) 
		{
			Theme = theme;
				
			return this;
		}
		
		/// <summary>
        /// Sets the mouse filter for the current button.
        /// </summary>
        /// <param name="filter">The mouse filter to set.</param>
        /// <returns>Returns the updated Buttonable instance.</returns>
		public Buttonable SetMouseFilter(Control.MouseFilterEnum filter)
		{
			MouseFilter = filter;
			return this;
		}
		
		/// <summary>
        /// Sets the visibility state of the currently chosen button.
        /// </summary>
        /// <param name="state">The visibility state to set.</param>
        /// <returns>Returns the updated Buttonable instance.</returns>
		public Buttonable SetVisible( bool state ) 
		{
			base._SetVisible(state);
			
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode") && Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() is Button button) 
			{
				button.Visible = state;
			}
			
			return this;
		}
		
		/// <summary>
		/// Sets the horizontal size flag, which controls the x axis, and how it should act.
		/// </summary>
		/// <param name="flag">The Control.SizeFlags flag to set.</param>
		/// <returns>Returns the updated Buttonable instance.</returns>
		public override Buttonable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}
		
		/// <summary>
		/// Sets the vertical size flag, which controls the y axis, and how it should act.
		/// </summary>
		/// <param name="flag">The Control.SizeFlags flag to set.</param>
		/// <returns>Returns the updated Buttonable instance.</returns>
		public override Buttonable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}
		
		/// <summary>
        /// Sets the theme type of the button.
        /// </summary>
        /// <param name="type">The button type to set.</param>
        /// <returns>Returns the updated Buttonable instance.</returns>
		public Buttonable SetType( ButtonType type ) 
		{
			WorkingButtonType = type;
			
			return this;
		}
		
		/// <summary>
        /// Sets the cursor shape of the currently chosen button.
        /// </summary>
        /// <param name="shape">The cursor shape to set.</param>
        /// <returns>Returns the updated Buttonable instance.</returns>
		public Buttonable SetCursorShape( Control.CursorShape shape ) 
		{
			DefaultCursorShape = shape;
			
			return this;
		}
		
		/// <summary>
        /// Sets the icon of the currently chosen button.
        /// </summary>
        /// <param name="icon">The icon to set.</param>
        /// <returns>Returns the updated Buttonable instance.</returns>
		public Buttonable SetIcon( Texture2D icon )
		{
			Icon = icon;
			
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode") && Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() is Button button) 
			{
				button.Icon = icon;
			}

			return this;
		}
		
		/// <summary>
        /// Sets the icon alignment of the currently chosen button.
        /// </summary>
        /// <param name="alignment">The icon alignment to set.</param>
        /// <returns>Returns the updated Buttonable instance.</returns>
		public Buttonable SetIconAlignment( HorizontalAlignment alignment )
		{
			IconAlignment = alignment;
			
			return this;
		}
		
		/// <summary>
        /// Sets the action for the currently chosen button.
        /// </summary>
        /// <param name="action">The action to set.</param>
        /// <returns>Returns the updated Buttonable instance.</returns>
		public Buttonable SetAction( Action action ) 
		{
			_Actions.Add(Callable.From(action));
			
			return this;
		}
		
		/// <summary>
        /// Sets margin values for the currently chosen button.
        /// </summary>
        /// <param name="value">The margin value.</param>
        /// <param name="side">The side for which to set the margin.</param>
        /// <returns>Returns the updated Buttonable instance.</returns>
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
		
		/// <summary>
        /// Checks if the currently chosen button is visible.
        /// </summary>
        /// <returns>Returns true if the button is visible; otherwise, false.</returns>
		public bool IsVisible()
		{
			return Visible;
		}
		
		/// <summary>
        /// Resets the trait to a cleared state.
        /// </summary>
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