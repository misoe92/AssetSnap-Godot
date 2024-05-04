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
	public partial class Spinboxable : ContainerTrait
	{
		private string _Prefix = "";
		private string _TooltipText = "";
		private float _Step = 1;
		private float _MinimumValue = 0;
		private float _MaximumValue = 0;
		private double _DefaultValue = 0;
		
		private List<Callable?> _Actions = new();
		private Callable? _Action;
		
		/* The `public Spinboxable()` constructor in the `Spinboxable` class is initializing the `Name`
		property to "Spinboxable" and setting the `TypeString` property to the string representation of
		the class type. This constructor is called when an instance of the `Spinboxable` class is created,
		and it helps in setting initial values for these properties. */
		public Spinboxable()
		{
			Name = "Spinboxable";
			TypeString = GetType().ToString();
		}
		
		/// <summary>
		/// The AddToContainer function adds the currently chosen button to a specified container after
		/// checking for its existence in the Dependencies dictionary.
		/// </summary>
		/// <param name="Node">The `Node` parameter in the `AddToContainer` method represents the container to
		/// which the currently chosen button will be added. It is the target container where the button will
		/// be placed.</param>
		/// <returns>
		/// void
		/// </returns>
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
		/// The below function instantiates a SpinBox control with specified properties and connects it to a
		/// callable action.
		/// </summary>
		/// <returns>
		/// The method `Instantiate()` is returning an instance of the `Spinboxable` trait.
		/// </returns>
		public override Spinboxable Instantiate()
		{
			base._Instantiate();
			base.Instantiate();
			
			SpinBox WorkingInput = new()
			{
				Name = TraitName,
				Prefix = _Prefix,
				Step = _Step,
				TooltipText = _TooltipText,
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical
			};
		
			if( _MaximumValue != 0 ) 
			{
				WorkingInput.MaxValue = _MaximumValue;
			}
			
			if( _MinimumValue != 0 ) 
			{
				WorkingInput.MinValue = _MinimumValue;
			}
				
			if( 0 != _DefaultValue ) 
			{
				WorkingInput.Value = _DefaultValue;
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

			Dependencies[TraitName + "_WorkingNode"] = WorkingInput;
			
			Plugin.Singleton.TraitGlobal.AddInstance(Iteration, WorkingInput, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.TraitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);
			
			_Actions.Add(_Action);

			GetInnerContainer(0).AddChild(WorkingInput);

			Reset();
			Iteration += 1;
			Dependencies = new();
			
			return this;
		}
		
		/// <summary>
		/// This C# function selects a placed spinbox in the nodes array by index and returns a Spinboxable
		/// object.
		/// </summary>
		/// <param name="index">The `index` parameter in the `Select` method is used to specify the index of
		/// the spinbox that you want to select from the nodes array.</param>
		/// <param name="debug">The `debug` parameter in the `Select` method is a boolean flag that can be set
		/// to `true` or `false`. When set to `true`, it enables debugging mode which can help in
		/// troubleshooting and identifying any issues during the execution of the method. If set to `false`,
		/// debugging</param>
		/// <returns>
		/// The method `Select` is returning an object that implements the `Spinboxable` interface.
		/// </returns>
		public override Spinboxable Select(int index, bool debug = false)
		{
			base._Select(index, debug);
			
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode") ) 
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.TraitGlobal.GetDependencies(index, TypeString, OwnerName);
				Dependencies = dependencies;
			}
			
			return this;
		}
		
		/// <summary>
		/// The SelectByName function in C# selects a placed spinbox in the nodes array by name and sets it as
		/// a dependency.
		/// </summary>
		/// <param name="name">The `name` parameter in the `SelectByName` method is a string that represents
		/// the name of the spinbox that you want to select from the array of nodes. The method iterates
		/// through the array of nodes (which seem to be buttons in this case) and checks if the name of
		/// the</param>
		/// <returns>
		/// The method `SelectByName` is returning an object that implements the `Spinboxable` interface.
		/// </returns>
		public override Spinboxable SelectByName( string name ) 
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
		/// This C# function sets the name of the current spinbox and returns a Spinboxable object.
		/// </summary>
		/// <param name="text">The `text` parameter in the `SetName` method is a string that represents the name
		/// you want to set for the current spinbox.</param>
		/// <returns>
		/// The method `SetName` is returning an object of type `Spinboxable`.
		/// </returns>
		public Spinboxable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/// <summary>
		/// This C# function sets the prefix of a spinbox and returns the object for chaining.
		/// </summary>
		/// <param name="Prefix">The `SetPrefix` method in the code snippet allows you to set the prefix of
		/// the current spinbox. The method takes a string parameter named `Prefix`, which represents the
		/// prefix you want to set for the spinbox.</param>
		/// <returns>
		/// The method `SetPrefix` is returning an object that implements the `Spinboxable` interface.
		/// </returns>
		public Spinboxable SetPrefix( string Prefix ) 
		{
			_Prefix = Prefix;
			
			return this;
		}
		
		/// <summary>
		/// The SetTooltipText function sets the tooltip text of the current spinbox and returns a Spinboxable
		/// object.
		/// </summary>
		/// <param name="text">The `text` parameter in the `SetTooltipText` method is a string that represents
		/// the tooltip text that you want to set for the current spinbox.</param>
		/// <returns>
		/// The method `SetTooltipText` is returning an object that implements the `Spinboxable` interface.
		/// </returns>
		public Spinboxable SetTooltipText( string text ) 
		{
			_TooltipText = text;
			
			return this;
		}
		
		/// <summary>
		/// The SetStep function sets the step value of the current spinbox in C#.
		/// </summary>
		/// <param name="StepSize">StepSize is a float value that represents the step value to set for the
		/// current spinbox. This value determines the increment or decrement amount when the spinbox is
		/// adjusted up or down.</param>
		/// <returns>
		/// The method `SetStep` returns an object of type `Spinboxable`.
		/// </returns>
		public Spinboxable SetStep( float StepSize ) 
		{
			_Step = StepSize;
			
			return this;
		}
		
		/// <summary>
		/// The SetMinValue function in C# sets the minimum value of the current spinbox and returns a
		/// Spinboxable object.
		/// </summary>
		/// <param name="minValue">The `minValue` parameter in the `SetMinValue` method represents the minimum
		/// value that can be set for the current spinbox. This value determines the lower limit of the range
		/// of values that the spinbox can accept.</param>
		/// <returns>
		/// The method `SetMinValue` is returning an object of type `Spinboxable`.
		/// </returns>
		public Spinboxable SetMinValue(float minValue ) 
		{
			_MinimumValue = minValue;
			
			return this;
		}
		
		/// <summary>
		/// The SetMaxValue function in C# sets the maximum value of a spinbox and returns the object
		/// implementing the Spinboxable interface.
		/// </summary>
		/// <param name="maxValue">The `maxValue` parameter is a floating-point number that represents the
		/// maximum value that can be set for the current spinbox.</param>
		/// <returns>
		/// The method `SetMaxValue` is returning an object of type `Spinboxable`.
		/// </returns>
		public Spinboxable SetMaxValue(float maxValue ) 
		{
			_MaximumValue = maxValue;
			
			return this;
		}
		
		/// <summary>
		/// This C# function sets the value of a spinbox element and returns the object implementing the
		/// Spinboxable interface.
		/// </summary>
		/// <param name="value">The `value` parameter in the `SetValue` method is of type `double` and
		/// represents the value that will be set for the current spinbox.</param>
		/// <returns>
		/// The method `SetValue` returns an object that implements the `Spinboxable` interface.
		/// </returns>
		public Spinboxable SetValue( double value )
		{
			_DefaultValue = value;
			
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode") && Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() is SpinBox WorkingInput) 
			{
				WorkingInput.Value = value;
			}

			return this;
		}
		
		/// <summary>
		/// The function SetVisible sets the visibility state of the current spinbox.
		/// </summary>
		/// <param name="state">The `state` parameter in the `SetVisible` method is a boolean value that
		/// determines the visibility state of the spinbox. If `state` is `true`, the spinbox will be visible;
		/// if `state` is `false`, the spinbox will be hidden.</param>
		/// <returns>
		/// The method `SetVisible` is returning an object of type `Spinboxable`.
		/// </returns>
		public override Spinboxable SetVisible(bool state)
		{
			base.SetVisible(state);

			return this;
		}
		
		/// <summary>
		/// The SetAction method assigns a Callable action to the current spinbox and returns a Spinboxable
		/// object.
		/// </summary>
		/// <param name="Callable">In Java, `Callable` is a functional interface in the `java.util.concurrent`
		/// package. It represents a task that can be executed and return a result. The `call()` method of the
		/// `Callable` interface is used to perform the task and return the result.</param>
		/// <returns>
		/// The method SetAction returns an object of type Spinboxable.
		/// </returns>
		public Spinboxable SetAction( Callable action ) 
		{
			_Action = action;
			
			return this;
		}
		
		/// <summary>
		/// The SetDimensions method in C# sets the dimensions of the current spinbox and returns a
		/// Spinboxable object.
		/// </summary>
		/// <param name="width">The `width` parameter specifies the width of the spinbox. It determines how
		/// wide the spinbox will be displayed on the screen.</param>
		/// <param name="height">The height parameter in the SetDimensions method specifies the vertical
		/// dimension of the spinbox. It determines how tall the spinbox will be when displayed on the
		/// screen.</param>
		/// <returns>
		/// The method is returning an object that implements the Spinboxable interface.
		/// </returns>
		public override Spinboxable SetDimensions( int width, int height )
		{
			base.SetDimensions(width,height);
			
			return this;
		}
				
		/// <summary>
		/// The function SetHorizontalSizeFlags sets the vertical size flag for the x axis in a C# class.
		/// </summary>
		/// <param name="flag">The `flag` parameter in the `SetHorizontalSizeFlags` method is of type
		/// `Control.SizeFlags`. It is used to set the vertical size flag, which controls the x axis
		/// behavior.</param>
		/// <returns>
		/// The method is returning an object that implements the Spinboxable interface.
		/// </returns>
		public override Spinboxable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}
		
		/// <summary>
		/// This C# function sets the vertical size flags for a control, controlling the y-axis behavior.
		/// </summary>
		/// <param name="flag">The `flag` parameter in the `SetVerticalSizeFlags` method is of type
		/// `Control.SizeFlags`. It is used to set the vertical size flag, which controls the y-axis behavior
		/// of the control.</param>
		/// <returns>
		/// The method is returning an object of type Spinboxable.
		/// </returns>
		public override Spinboxable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}
		
		/// <summary>
		/// The SetMargin function sets margin values for the currently chosen spinbox in C#.
		/// </summary>
		/// <param name="value">The `value` parameter represents the margin value that you want to set for the
		/// spinbox. It is an integer value that determines the size of the margin.</param>
		/// <param name="side">The `side` parameter in the `SetMargin` method is a string that specifies which
		/// side of the spinbox the margin value should be applied to. It can have values like "top",
		/// "bottom", "left", "right", or an empty string if the margin value should be applied to</param>
		/// <returns>
		/// The method `SetMargin` is returning an object of type `Spinboxable`.
		/// </returns>
		public override Spinboxable SetMargin( int value, string side = "" ) 
		{
			base.SetMargin(value, side);
			
			return this;
		}
		
		/// <summary>
		/// This C# function retrieves the value of the current spinbox.
		/// </summary>
		/// <returns>
		/// The method `GetValue` returns a double value, which is the value of the current spinbox. If the
		/// spinbox exists in the Dependencies dictionary and is of type SpinBox, then the method returns the
		/// value of the spinbox. Otherwise, it returns 0.
		/// </returns>
		public double GetValue()
		{
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode") && Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() is SpinBox WorkingInput) 
			{
				return WorkingInput.Value;
			}

			return 0;
		}
		
		/// <summary>
		/// The Reset method clears the state of a trait by resetting its properties to default values.
		/// </summary>
		protected override void Reset()
		{
			Size = Vector2.Zero;
			CustomMinimumSize = Vector2.Zero;
			_Prefix = "";
			_TooltipText = "";
			_DefaultValue = 0;
			_Step = 1;
			_MinimumValue = 0;
			_MaximumValue = 0;
			_Action = null;

			base.Reset();
		}
	}
}
#endif