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
using AssetSnap.Trait;
using Godot;

namespace AssetSnap.Component
{
	/// <summary>
	/// A trait for creating and managing labeled UI elements.
	/// </summary>
	[Tool]
	public partial class Labelable : ContainerTrait
	{
		/*
		** Enums
		*/
		public enum TitleType
		{
			HeaderSmall,
			HeaderMedium,
			HeaderLarge,
			TextSmall,
			TextMedium,
			TextTinyDiffused,
		};

		/*
		** Protected
		*/
		protected string Title = "";
		protected string Suffix = "";
		protected TitleType Type = TitleType.HeaderMedium;
		protected TextServer.AutowrapMode AutowrapMode = TextServer.AutowrapMode.Off;
		protected HorizontalAlignment _HorizontalAlignment;

		/// <summary>
		/// Default constructor for Labelable.
		/// </summary>
		public Labelable()
		{
			Name = "Labelable";
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

		/// <summary>
		/// Instantiate an instance of the trait.
		/// </summary>
		/// <returns>Returns the instantiated Labelable.</returns>
		public override Labelable Instantiate()
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

			Label Label = new()
			{
				Name = TraitName,
				Text = Title,
				CustomMinimumSize = CustomMinimumSize,
				Size = Size,
				ThemeTypeVariation = Type.ToString(),
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
				AutowrapMode = AutowrapMode,
				HorizontalAlignment = _HorizontalAlignment
			};

			GetInnerContainer(0)
				.AddChild(Label);

			if ("" != Suffix)
			{
				Label suffix = new()
				{
					Name = TraitName + "-suffix",
					Text = Suffix,
					ThemeTypeVariation = "TextExtraSmall",
					SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter,
					SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
					HorizontalAlignment = HorizontalAlignment.Right
				};

				GetInnerContainer(0)
					.AddChild(suffix);

				Dependencies.Add(TraitName + "_Suffix", suffix);
			}

			Dependencies[TraitName + "_WorkingNode"] = Label;

			Plugin.Singleton.traitGlobal.AddInstance(Iteration, Label, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.traitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);


			Reset();
			Iteration += 1;
			Dependencies = new();

			return this;
		}

		/// <summary>
		/// Selects a placed label in the nodes array by index.
		/// </summary>
		/// <param name="index">The index of the label to select.</param>
		/// <param name="debug">Whether to output debug information.</param>
		/// <returns>Returns the selected Labelable.</returns>
		public override Labelable Select(int index, bool debug = false)
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

		/// <summary>
		/// Selects a placed label in the nodes array by name.
		/// </summary>
		/// <param name="name">The name of the label to select.</param>
		/// <returns>Returns the selected Labelable.</returns>
		public override Labelable SelectByName(string name)
		{
			base._SelectByName(name);

			return this;
		}

		/// <summary>
		/// Adds the currently chosen button to a specified container.
		/// </summary>
		/// <param name="Container">The container to add the button to.</param>
		/// <param name="index">The optional index at which to add the button.</param>
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
		/// Sets the name of the current label.
		/// </summary>
		/// <param name="text">The name to set.</param>
		/// <returns>Returns the modified Labelable.</returns>
		public Labelable SetName(string text)
		{
			base._SetName(text);

			return this;
		}

		/// <summary>
		/// Sets the text of the current label.
		/// </summary>
		/// <param name="text">The text to set.</param>
		/// <returns>Returns the modified Labelable.</returns>
		public Labelable SetText(string text)
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

		/// <summary>
		/// Sets the suffix of the current label.
		/// </summary>
		/// <param name="text">The suffix to set.</param>
		/// <returns>Returns the modified Labelable.</returns>
		public Labelable SetSuffix(string text)
		{
			Suffix = text;

			return this;
		}

		/// <summary>
		/// Sets the theme type of the label.
		/// </summary>
		/// <param name="type">The type of the label.</param>
		/// <returns>Returns the modified Labelable.</returns>
		public Labelable SetType(TitleType type)
		{
			Type = type;

			return this;
		}

		/// <summary>
        /// Sets the horizontal size flag for the container, controlling the behavior of the x-axis.
        /// </summary>
        /// <param name="flag">The size flag to set.</param>
        /// <returns>The updated <see cref="Labelable"/> instance.</returns>
		public override Labelable SetContainerHorizontalSizeFlag(Control.SizeFlags flag)
		{
			base.SetContainerHorizontalSizeFlag(flag);

			return this;
		}

		/// <summary>
		/// Sets the alignment for the text of the current label.
		/// </summary>
		/// <param name="alignment">The alignment to set.</param>
		/// <returns>Returns the modified Labelable.</returns>
		public Labelable SetAlignment(HorizontalAlignment alignment)
		{
			_HorizontalAlignment = alignment;

			return this;
		}

		/// <summary>
		/// Sets the dimensions for the current label.
		/// </summary>
		/// <param name="width">The width of the label.</param>
		/// <param name="height">The height of the label.</param>
		/// <returns>Returns the modified Labelable.</returns>
		public override Labelable SetDimensions(int width, int height)
		{
			base.SetDimensions(width, height);

			return this;
		}

		/// <summary>
		/// Sets the horizontal size flag, which controls the x axis, and how it should act.
		/// </summary>
		/// <param name="flag">The size flag to set.</param>
		/// <returns>Returns the modified Labelable.</returns>
		public override Labelable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}

		/// <summary>
		/// Sets the vertical size flag, which controls the y axis, and how it should act.
		/// </summary>
		/// <param name="flag">The size flag to set.</param>
		/// <returns>Returns the modified Labelable.</returns>
		public override Labelable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}

		/// <summary>
		/// Sets the auto wrap of the label, allowing it to break lines based on rules.
		/// </summary>
		/// <param name="mode">The auto wrap mode to set.</param>
		/// <returns>Returns the modified Labelable.</returns>
		public Labelable SetAutoWrap(TextServer.AutowrapMode mode)
		{
			AutowrapMode = mode;

			return this;
		}

		/// <summary>
		/// Sets margin values for the currently chosen label.
		/// </summary>
		/// <param name="value">The margin value to set.</param>
		/// <param name="side">The side for which to set the margin.</param>
		/// <returns>Returns the modified Labelable.</returns>
		public override Labelable SetMargin(int value, string side = "")
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
			return Title;
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
		/// <param name="debug">Whether to output debug information.</param>
		/// <returns>Returns true if the label is valid, otherwise false.</returns>
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