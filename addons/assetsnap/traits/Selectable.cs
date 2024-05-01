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
	[Tool]
	public partial class Selectable : ContainerTrait
	{
		/*
		** Enums
		*/
		public enum Type
		{
			SelectableSmall,
			SelectableMedium,
			SelectableLarge,
		};
		
		/*
		** Exports
		*/
		[Export]
		public Godot.Collections.Array<Callable> _Actions = new Godot.Collections.Array<Callable>();
		
		/*
		** Public
		*/

		/*
		** Protected
		*/
		protected string Title = "";
		protected string Suffix = "";
		protected Type _Type = Type.SelectableMedium;
		protected TextServer.AutowrapMode AutowrapMode = TextServer.AutowrapMode.Off;
		protected HorizontalAlignment _HorizontalAlignment;
		protected Godot.Collections.Array<string> Items = new();

		/*
		** Public methods
		*/
		public Selectable()
		{
			Name = "Selectable";
			Margin = new()
			{
				{"left", 15},
				{"right", 15},
				{"top", 10},
				{"bottom", 10},
			};

			TypeString = GetType().ToString();
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
			SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		}

		/*
		** Instantiate an instance of the trait
		**
		** @return Selectable
		*/
		public override Selectable Instantiate()
		{
			base._Instantiate();

			if ("" != Suffix)
			{
				Orientation = ContainerOrientation.Horizontal;
				InnerOrientation = ContainerOrientation.Vertical;
				SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin;
			}

			base.Instantiate();
			if (Title == "")
			{
				GD.PushWarning("Title not found");
				return this;
			}

			OptionButton Select = new()
			{
				Name = TraitName,
				Text = Title,
				CustomMinimumSize = CustomMinimumSize,
				Size = Size,
				ThemeTypeVariation = _Type.ToString(),
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
			};
			
			// Add the predefined items
			if( Items.Count != 0 ) 
			{
				for( int i = 0; i < Items.Count; i++ ) 
				{
					Select.AddItem(Items[i]);
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

			Plugin.Singleton.traitGlobal.AddInstance(Iteration, Select, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.traitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);

			Reset();
			Iteration += 1;
			Dependencies = new();

			return this;
		}
		
		public Selectable AddItem( string text )
		{
			Items.Add(text);
			
			if( Dependencies.ContainsKey(TraitName + "_WorkingNode")) 
			{
				OptionButton select = Dependencies[TraitName + "_WorkingNode"].As<OptionButton>();
				select.AddItem(text);
			}
			
			return this;
		}

		/*
		** Selects an placed label in the
		** nodes array by index
		**
		** @param int index
		** @return Selectable
		*/
		public override Selectable Select(int index, bool debug = false)
		{
			base._Select(index);

			if (debug)
			{
				GD.Print("Selected", index, TraitName);
			}

			if (false != Dependencies.ContainsKey(TraitName + "_WorkingNode"))
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.traitGlobal.GetDependencies(index, TypeString, OwnerName);
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

		/*
		** Selects an placed label in the
		** nodes array by name
		**
		** @param string name
		** @return Selectable
		*/
		public override Selectable SelectByName(string name)
		{
			base._SelectByName(name);

			return this;
		}

		/*
		** Adds the currently chosen button
		** to a specified container
		**
		** @param Node Container
		** @return void
		*/
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

		/*
		** Setter Methods
		*/
		
		/*
		** Sets the action for the
		** currently chosen button
		**
		** @param Action action
		** @return Buttonable
		*/
		public Selectable SetAction( Action<int> action ) 
		{
			_Actions.Add(Callable.From(action));
			
			return this;
		}
		
		/*
		** Sets the name of the current label
		**
		** @param string text
		** @return Selectable
		*/
		public Selectable SetName(string text)
		{
			base._SetName(text);

			return this;
		}

		/*
		** Sets the text of the current button
		**
		** @param string text
		** @return Selectable
		*/
		public Selectable SetText(string text)
		{
			Title = text;

			if (
				false != Dependencies.ContainsKey(TraitName + "_WorkingNode") &&
				Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() is Label labelNode
			)
			{
				labelNode.Text = text;
			}

			return this;
		}

		/*
		** Sets the text of the current button
		**
		** @param string text
		** @return Selectable
		*/
		public Selectable SetSuffix(string text)
		{
			Suffix = text;

			return this;
		}

		/*
		** Sets the theme type of the button,
		** which lays out a set of specified rules
		** from the theme that the button follows
		**
		** @param TitleType type
		** @return Selectable
		*/
		public Selectable SetType(Type type)
		{
			_Type = type;

			return this;
		}

		public override Selectable SetContainerHorizontalSizeFlag(Control.SizeFlags flag)
		{
			base.SetContainerHorizontalSizeFlag(flag);

			return this;
		}


		/*
		** Sets the alignment for the text
		** of the current label
		**
		** @param HorizontalAlignment alignment
		** @return Selectable
		*/
		public Selectable SetAlignment(HorizontalAlignment alignment)
		{
			_HorizontalAlignment = alignment;

			return this;
		}

		/*
		** Sets the dimensions for 
		** the current label
		**
		** @param int width
		** @param int height
		** @return Selectable
		*/
		public override Selectable SetDimensions(int width, int height)
		{
			base.SetDimensions(width, height);

			return this;
		}

		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Selectable
		*/
		public override Selectable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}

		/*
		** Sets the horizontal size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Selectable
		*/
		public override Selectable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}

		/*
		** Sets the auto wrap of the label
		** allowing it to break lines based
		** on rules
		**
		** @param TextServer.AutowrapMode mode
		** @return Selectable
		*/
		public Selectable SetAutoWrap(TextServer.AutowrapMode mode)
		{
			AutowrapMode = mode;

			return this;
		}

		/*
		** Sets margin values for 
		** the currently chosen label
		**
		** @param int value
		** @param string side
		** @return Selectable
		*/
		public override Selectable SetMargin(int value, string side = "")
		{
			base.SetMargin(value, side);

			return this;
		}

		/*
		** Getter Methods
		*/

		/*
		** Fetches the text from the label
		**
		** @return string
		*/
		public string GetTitle()
		{
			return Title;
		}

		/*
		** Fetches the inner container
		** of the label
		**
		** @return Container
		*/
		public Container GetInnerContainer()
		{
			return base.GetInnerContainer(0);
		}

		/*
		** Booleans
		*/

		/*
		** Checks if the label is valid
		**
		** @return bool
		*/
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
			Title = "";
			Suffix = "";

			base.Reset();

			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
			SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;

			Margin = new()
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