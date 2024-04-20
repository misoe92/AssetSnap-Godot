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
	[Tool]
	public partial class Checkable : ContainerTrait
	{
		/*
		** Private
		*/
		private List<Callable?> _Actions = new();
		private Callable? _Action;
		
		private string Text = "";
		private string TooltipText = "";
		private bool ButtonPressed = false;
		
		/*
		** Public methods
		*/
		public Checkable()
		{
			Name = "Checkable";
			TypeString = GetType().ToString();
		}
		
		/*
		** Instantiate an instance of the trait
		**
		** @return Checkable
		*/
		public override Checkable Instantiate()
		{
			base._Instantiate();
			base.Instantiate();
			
			// Setup the checkbox
			CheckBox WorkingInput = new()
			{
				Name = TraitName,
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
			base.GetInnerContainer(0).AddChild(WorkingInput);
			
			// Connect the button to it's action
			if( _Action is Callable _callable ) 
			{
				WorkingInput.Connect(CheckBox.SignalName.Pressed, _callable);
			}
			
			Dependencies[TraitName + "_WorkingNode"] = WorkingInput;
			
			// Add the button to the nodes array			
			Plugin.Singleton.traitGlobal.AddInstance(Iteration, WorkingInput, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.traitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);
			
			// Add the action to the actions array			
			_Actions.Add(_Action);
			
			// Clear the trait
			Reset();
			
			Iteration += 1;
			Dependencies = new();
			
			return this;
		}
		
		/*
		** Selects an placed checkbox in the
		** nodes array by index
		**
		** @param int index
		** @return Checkable
		*/
		public override Checkable Select(int index, bool debug = false)
		{
			base._Select(index, debug);
			
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode")) 
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.traitGlobal.GetDependencies(index, TypeString, OwnerName);
				Dependencies = dependencies;
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
		
		/*
		** Adds the currently chosen button
		** to a specified container
		**
		** @param Node Container
		** @return void
		*/
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
			if(
				false != Dependencies.ContainsKey(TraitName + "_WorkingNode") &&
				Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() is CheckBox WorkingInput
			) 
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
		public override Checkable SetVisible( bool state ) 
		{
			base.SetVisible(state);

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Checkable
		*/
		public override Checkable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag); 

			return this;
		}
		
		/*
		** Sets the vertical size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Checkable
		*/
		public override Checkable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag); 

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
		public override Checkable SetMargin( int value, string side = "" ) 
		{
			base.SetMargin(value, side);
			return this;
		}
		
		/*
		** Sets the dimensions for the checkbox
		**
		** @param int width
		** @param int height
		** @return Checkable
		*/
		public override Checkable SetDimensions( int width, int height )
		{
			base.SetDimensions(width, height);

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
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode") && Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() is CheckBox WorkingInput) 
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
		** Private
		*/
		
		/*
		** Resets the trait to
		** a cleared state
		**
		** @return void
		*/
		protected override void Reset()
		{
			_Action = null;

			base.Reset();
		}
	}
}
#endif