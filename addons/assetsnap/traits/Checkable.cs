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
using AssetSnap.Trait;
using Godot;

namespace AssetSnap.Component
{
	/// <summary>
	/// A checkable component that extends ContainerTrait, providing functionality for creating checkboxes.
	/// </summary>
	[Tool]
	public partial class Checkable : ContainerTrait
	{
		private string _Text = "";
		private string _TooltipText = "";
		private bool _ButtonPressed = false;
		private List<Callable?> _Actions = new();
		private Callable? _Action;
		
		/// <summary>
		/// Constructor for the Checkable class.
		/// </summary>
		public Checkable()
		{
			Name = "Checkable";
			TypeString = GetType().ToString();
		}
		
		/// <summary>
		/// Adds the currently chosen button to a specified container.
		/// </summary>
		/// <param name="Container">The container to which the chosen button will be added.</param>
		public void AddToContainer( Node Container )
		{
			if( false == Dependencies.ContainsKey(TraitName + "_MarginContainer") ) 
			{
				GD.PushError("Container was not found @ AddToContainer");
				GD.PushError("AddToContainer::Keys-> ", Dependencies.Keys);
				GD.PushError("AddToContainer::ADDTO-> ", TraitName + "_MarginContainer");
				return;
			}
			
			_AddToContainer(Container, Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>());
		}
		
		/// <summary>
		/// Instantiate an instance of the trait.
		/// </summary>
		/// <returns>Returns the instantiated Checkable instance.</returns>
		public override Checkable Instantiate()
		{
			base._Instantiate();
			base.Instantiate();
			
			// Setup the checkbox
			CheckBox WorkingInput = new()
			{
				Name = TraitName,
				Text = _Text,
				TooltipText = _TooltipText,
				ButtonPressed = _ButtonPressed,
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
			base.GetInnerContainer(0).AddChild(WorkingInput);
			
			// Connect the button to it's action
			if( _Action is Callable _callable ) 
			{
				WorkingInput.Connect(CheckBox.SignalName.Pressed, _callable);
			}
			
			Dependencies[TraitName + "_WorkingNode"] = WorkingInput;
			
			// Add the button to the nodes array			
			Plugin.Singleton.TraitGlobal.AddInstance(Iteration, WorkingInput, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.TraitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);
			
			// Add the action to the actions array			
			_Actions.Add(_Action);
			
			// Clear the trait
			Reset();
			
			Iteration += 1;
			Dependencies = new();
			
			return this;
		}
		
		/// <summary>
		/// Selects a placed checkbox in the nodes array by index.
		/// </summary>
		/// <param name="index">The index of the checkbox to select.</param>
		/// <param name="debug">Optional parameter to enable debugging.</param>
		/// <returns>Returns the updated Checkable instance.</returns>
		public override Checkable Select(int index, bool debug = false)
		{
			base._Select(index, debug);
			
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode")) 
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.TraitGlobal.GetDependencies(index, TypeString, OwnerName);
				Dependencies = dependencies;
			}
			
			return this;
		}
		
		/// <summary>
		/// Selects a placed checkbox in the nodes array by name.
		/// </summary>
		/// <param name="name">The name of the checkbox to select.</param>
		/// <returns>Returns the updated Checkable instance.</returns>
		public override Checkable SelectByName( string name ) 
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
		/// Sets the name of the current checkbox.
		/// </summary>
		/// <param name="text">The name to set.</param>
		/// <returns>Returns the updated Checkable instance.</returns>
		public Checkable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/// <summary>
		/// Sets the text of the current checkbox.
		/// </summary>
		/// <param name="text">The text to set.</param>
		/// <returns>Returns the updated Checkable instance.</returns>
		public Checkable SetText( string text ) 
		{
			_Text = text;
			
			return this;
		}
		
		/// <summary>
		/// Sets the tooltip text of the current checkbox.
		/// </summary>
		/// <param name="text">The tooltip text to set.</param>
		/// <returns>Returns the updated Checkable instance.</returns>
		public Checkable SetTooltipText( string text ) 
		{
			_TooltipText = text;
			
			return this;
		}
		
		/// <summary>
		/// Sets the value of the current checkbox.
		/// </summary>
		/// <param name="value">The value to set.</param>
		/// <returns>Returns the updated Checkable instance.</returns>
		public Checkable SetValue( bool value )
		{
			if(
				false != Dependencies.ContainsKey(TraitName + "_WorkingNode") &&
				Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() is CheckBox WorkingInput
			) 
			{
				WorkingInput.ButtonPressed = value;
			}
			else 
			{
				_ButtonPressed = value;
			}

			return this;
		}
		
		/// <summary>
		/// Sets the visibility state of the currently chosen checkbox.
		/// </summary>
		/// <param name="state">The visibility state to set.</param>
		/// <returns>Returns the updated Checkable instance.</returns>
		public override Checkable SetVisible( bool state ) 
		{
			base.SetVisible(state);

			return this;
		}
		
		/// <summary>
		/// Sets the horizontal size flag, which controls the x axis.
		/// </summary>
		/// <param name="flag">The size flag to set.</param>
		/// <returns>Returns the updated Checkable instance.</returns>
		public override Checkable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag); 

			return this;
		}
		
		/// <summary>
		/// Sets the vertical size flag, which controls the y axis.
		/// </summary>
		/// <param name="flag">The size flag to set.</param>
		/// <returns>Returns the updated Checkable instance.</returns>
		public override Checkable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag); 

			return this;
		}
		
		/// <summary>
		/// Sets the callable for the currently chosen checkbox.
		/// </summary>
		/// <param name="action">The callable action to set.</param>
		/// <returns>Returns the updated Checkable instance.</returns>
		public Checkable SetAction( Callable action ) 
		{
			_Action = action;			
			return this;
		}
		
		/// <summary>
		/// Sets margin values for the currently chosen checkbox.
		/// </summary>
		/// <param name="value">The margin value.</param>
		/// <param name="side">The side for which to set the margin.</param>
		/// <returns>Returns the updated Checkable instance.</returns>
		public override Checkable SetMargin( int value, string side = "" ) 
		{
			base.SetMargin(value, side);
			return this;
		}
		
		/// <summary>
		/// Sets the dimensions for the checkbox.
		/// </summary>
		/// <param name="width">The width to set.</param>
		/// <param name="height">The height to set.</param>
		/// <returns>Returns the updated Checkable instance.</returns>
		public override Checkable SetDimensions( int width, int height )
		{
			base.SetDimensions(width, height);

			return this;
		}
		
		/// <summary>
		/// Fetches the value of the current checkbox.
		/// </summary>
		/// <returns>Returns the value of the current checkbox.</returns>
		public bool GetValue()
		{
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode") && Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() is CheckBox WorkingInput) 
			{
				// GD.PushError(Name);
				return WorkingInput.ButtonPressed;
			}
			
			return false;
		}
		
		/// <summary>
		/// Checks if any nodes exist.
		/// </summary>
		/// <returns>Returns true if nodes exist; otherwise, false.</returns>
		public bool HasNodes()
		{
			return null != Nodes && Nodes.Count != 0;
		}
		
		/// <summary>
		/// Resets the trait to a cleared state.
		/// </summary>
		protected override void Reset()
		{
			_Action = null;
			base.Reset();
		}
	}
}

#endif