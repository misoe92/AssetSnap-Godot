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
using AssetSnap.Trait;
using Godot;

namespace AssetSnap.Component
{
	/// <summary>
	/// A selectable component for use in Godot projects.
	/// </summary>
	[Tool]
	public partial class Selectable : ContainerTrait
	{
		/// <summary>
		/// Types of selectable components.
		/// </summary>
		public enum Type
		{
			SelectableSmall,
			SelectableMedium,
			SelectableLarge,
		};
		
		[Export]
		public Godot.Collections.Array<Callable> _Actions = new Godot.Collections.Array<Callable>();
		
		protected string _Title = "";
		protected string _Suffix = "";
		protected Type _Type = Type.SelectableMedium;
		protected TextServer.AutowrapMode _AutowrapMode = TextServer.AutowrapMode.Off;
		protected HorizontalAlignment _HorizontalAlignment;
		protected Godot.Collections.Array<string> _Items = new();

		/// <summary>
		/// Constructor for the Selectable component.
		/// </summary>
		/// <returns>Returns a new instance of Selectable.</returns>
		public Selectable()
		{
			Name = "Selectable";
			_Margin = new()
			{
				{"left", 15},
				{"right", 15},
				{"top", 10},
				{"bottom", 10},
			};

			TypeString = GetType().ToString();
			_SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
			_SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		}
		
		/// <summary>
		/// Adds the currently chosen button to a specified container.
		/// </summary>
		/// <param name="Container">The container to add the button to.</param>
		/// <param name="index">Optional index to insert the button at.</param>
		public void AddToContainer(Node Container, int? index = null)
		{
			if (null == Dependencies)
			{
				GD.PushError("Dependencies not set @ AddToContainer");
				return;
			}

			if (false == Dependencies.ContainsKey(TraitName + "_MarginContainer"))
			{
				GD.PushError("Container was not found @ AddToContainer");
				GD.PushError("AddToContainer::Keys-> ", Dependencies.Keys);
				GD.PushError("AddToContainer::ADDTO-> ", TraitName + "_MarginContainer");
				return;
			}

			base._AddToContainer(Container, Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>(), index);
			Reset();
		}

		/// <summary>
		/// Instantiate an instance of the trait.
		/// </summary>
		/// <returns>Returns the instantiated Selectable component.</returns>
		public override Selectable Instantiate()
		{
			base._Instantiate();

			if ("" != _Suffix)
			{
				_Orientation = ContainerOrientation.Horizontal;
				_InnerOrientation = ContainerOrientation.Vertical;
				_SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;
			}

			base.Instantiate();
			if (_Title == "")
			{
				GD.PushWarning("Title not found");
				return this;
			}

			OptionButton Select = new()
			{
				Name = TraitName,
				Text = _Title,
				CustomMinimumSize = _CustomMinimumSize,
				Size = _Size,
				ThemeTypeVariation = _Type.ToString(),
				SizeFlagsHorizontal = _SizeFlagsHorizontal,
				SizeFlagsVertical = _SizeFlagsVertical,
			};
			
			// Add the predefined items
			if( _Items.Count != 0 ) 
			{
				for( int i = 0; i < _Items.Count; i++ ) 
				{
					Select.AddItem(_Items[i]);
				}
			}
			
			// Connect the button to it's action
			if( _Actions.Count >= Iteration ) 
			{
				Callable actionCallable = _Actions[Iteration];
				Godot.Error error = Select.Connect( OptionButton.SignalName.ItemSelected, actionCallable);
				if( error != Godot.Error.Ok)
				{
					GD.Print("Error connecting signal: " + error.ToString());
				}
			}

			GetInnerContainer(0)
				.AddChild(Select);

			Dependencies[TraitName + "_WorkingNode"] = Select;

			Plugin.Singleton.TraitGlobal.AddInstance(Iteration, Select, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.TraitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);

			Reset();
			Iteration += 1;
			Dependencies = new();

			return this;
		}
		
		/// <summary>
		/// Adds an item to the selectable component.
		/// </summary>
		/// <param name="text">The text of the item to add.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public Selectable AddItem( string text )
		{
			_Items.Add(text);
			
			if( Dependencies.ContainsKey(TraitName + "_WorkingNode")) 
			{
				OptionButton select = Dependencies[TraitName + "_WorkingNode"].As<OptionButton>();
				select.AddItem(text);
			}
			
			return this;
		}

		/// <summary>
		/// Selects an item in the component by index.
		/// </summary>
		/// <param name="index">The index of the item to select.</param>
		/// <param name="debug">Whether to print debug information.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public override Selectable Select(int index, bool debug = false)
		{
			base._Select(index);

			if (debug)
			{
				GD.Print("Selected", index, TraitName);
			}

			if (false != Dependencies.ContainsKey(TraitName + "_WorkingNode"))
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.TraitGlobal.GetDependencies(index, TypeString, OwnerName);
				Dependencies = dependencies;
				if (debug)
				{
					GD.Print("Dependencies", Dependencies);
				}
			}
			else
			{
				if (debug)
				{
					GD.Print("NO DEPENDENCIES");
				}
			}

			return this;
		}

		/// <summary>
		/// Selects an item in the component by name.
		/// </summary>
		/// <param name="name">The name of the item to select.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public override Selectable SelectByName(string name)
		{
			base._SelectByName(name);

			return this;
		}

		/// <summary>
		/// Sets the action for the currently chosen button.
		/// </summary>
		/// <param name="action">The action to set.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public Selectable SetAction( Action<int> action ) 
		{
			_Actions.Add(Callable.From(action));
			
			return this;
		}
		
		/// <summary>
		/// Sets the name of the current label.
		/// </summary>
		/// <param name="text">The name to set.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public Selectable SetName(string text)
		{
			base._SetName(text);

			return this;
		}

		/// <summary>
		/// Sets the text of the current button.
		/// </summary>
		/// <param name="text">The text to set.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public Selectable SetText(string text)
		{
			_Title = text;

			if (
				false != Dependencies.ContainsKey(TraitName + "_WorkingNode") &&
				Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() is Label labelNode
			)
			{
				labelNode.Text = text;
			}

			return this;
		}

		/// <summary>
		/// Sets the suffix of the current button.
		/// </summary>
		/// <param name="text">The suffix text to set.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public Selectable SetSuffix(string text)
		{
			_Suffix = text;

			return this;
		}

		/// <summary>
		/// Sets the theme type of the button.
		/// </summary>
		/// <param name="type">The type of theme to set.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public Selectable SetType(Type type)
		{
			_Type = type;

			return this;
		}

		/// <summary>
		/// Sets the horizontal size flag for the container.
		/// </summary>
		/// <param name="flag">The horizontal size flag to set.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public override Selectable SetContainerHorizontalSizeFlag(Control.SizeFlags flag)
		{
			base.SetContainerHorizontalSizeFlag(flag);

			return this;
		}

		/// <summary>
		/// Sets the alignment for the text of the current label.
		/// </summary>
		/// <param name="alignment">The horizontal alignment to set.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public Selectable SetAlignment(HorizontalAlignment alignment)
		{
			_HorizontalAlignment = alignment;

			return this;
		}

		/// <summary>
		/// Sets the dimensions for the current label.
		/// </summary>
		/// <param name="width">The width of the label.</param>
		/// <param name="height">The height of the label.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public override Selectable SetDimensions(int width, int height)
		{
			base.SetDimensions(width, height);

			return this;
		}

		/// <summary>
		/// Sets the horizontal size flags for the container.
		/// </summary>
		/// <param name="flag">The horizontal size flags to set.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public override Selectable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}

		/// <summary>
		/// Sets the vertical size flags for the container.
		/// </summary>
		/// <param name="flag">The vertical size flags to set.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public override Selectable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}

		/// <summary>
		/// Sets the auto wrap mode of the label.
		/// </summary>
		/// <param name="mode">The auto wrap mode to set.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public Selectable SetAutoWrap(TextServer.AutowrapMode mode)
		{
			_AutowrapMode = mode;

			return this;
		}

		/// <summary>
		/// Sets margin values for the currently chosen label.
		/// </summary>
		/// <param name="value">The margin value to set.</param>
		/// <param name="side">The side for which to set the margin.</param>
		/// <returns>Returns the modified Selectable component.</returns>
		public override Selectable SetMargin(int value, string side = "")
		{
			base.SetMargin(value, side);

			return this;
		}

		/// <summary>
		/// Fetches the text from the label.
		/// </summary>
		/// <returns>Returns the text of the label.</returns>
		public string GetTitle()
		{
			return _Title;
		}

		/// <summary>
		/// Fetches the inner container of the label.
		/// </summary>
		/// <returns>Returns the inner container of the label.</returns>
		public Container GetInnerContainer()
		{
			return base.GetInnerContainer(0);
		}

		/// <summary>
		/// Checks if the label is valid.
		/// </summary>
		/// <param name="debug">Whether to print debug information.</param>
		/// <returns>Returns true if the label is valid, false otherwise.</returns>
		public override bool IsValid(bool debug = false)
		{
			if (base.IsValid(debug))
			{
				if (
					false != Dependencies.ContainsKey(TraitName + "_MarginContainer")
				)
				{
					return true;
				}
				else
				{
					if (debug)
					{
						GD.PushError("No outer container was found", Dependencies);
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Resets the trait to a cleared state.
		/// </summary>
		protected override void Reset()
		{
			_Title = "";
			_Suffix = "";

			base.Reset();

			_SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
			_SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;

			_Margin = new()
			{
				{"left", 15},
				{"right", 15},
				{"top", 10},
				{"bottom", 10},
			};
		}
	}
}
#endif