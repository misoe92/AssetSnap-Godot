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

using Godot;

namespace AssetSnap.Component
{
	/// <summary>
	/// A partial class representing a panel container trait, derived from Base.
	/// </summary>
	[Tool]
	public partial class Panelable : Trait.Base
	{
		/// <summary>
		/// Enumeration of panel types.
		/// </summary>
		public enum PanelType
		{
			DefaultPanelContainer,
			RoundedPanelContainer,
			LightPanelContainer
		}

		private PanelType _Type = PanelType.DefaultPanelContainer;
		private MarginContainer _MarginContainer;
		private MarginContainer _PaddingContainer;

		/// <summary>
		/// Default constructor for Panelable.
		/// </summary>
		public Panelable()
		{
			Name = "Panelable";
			TypeString = GetType().ToString();

			Margin = new()
			{
				{"left", 0},
				{"right", 0},
				{"top", 5},
				{"bottom", 5},
			};

			Padding = new()
			{
				{"left", 10},
				{"right", 10},
				{"top", 5},
				{"bottom", 5},
			};
		}
		
		/// <summary>
		/// Adds the currently chosen panel container to a specified container.
		/// </summary>
		/// <param name="Container">The container to which to add the panel container.</param>
		public void AddToContainer(Node Container)
		{
			if (false == Dependencies.ContainsKey(TraitName + "_MarginContainer"))
			{
				GD.PushError("Container was not found @ AddToContainer");
				GD.PushError("AddToContainer::Keys-> ", Dependencies.Keys);
				GD.PushError("AddToContainer::ADDTO-> ", TraitName + "_MarginContainer");
				return;
			}

			base._AddToContainer(Container, Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>());
		}

		/// <summary>
		/// Instantiate an instance of the trait.
		/// </summary>
		/// <returns>Returns the instantiated Panelable.</returns>
		public Panelable Instantiate()
		{
			base._Instantiate();

			// Create MarginContainer
			_MarginContainer = new()
			{
				Name = "PanelMarginContainer",
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
				Visible = Visible,
			};

			// Apply margin to MarginContainer
			foreach ((string side, int value) in Margin)
			{
				_MarginContainer.AddThemeConstantOverride("margin_" + side, value);
			}

			// Create PanelContainer
			PanelContainer WorkingPanel = new()
			{
				Name = TraitName,
				ThemeTypeVariation = _Type.ToString(),
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
			};
			
			// Create PaddingContainer
			_PaddingContainer = new()
			{
				Name = "PanelPaddingContainer",
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
			};

			// Apply padding to PaddingContainer
			foreach ((string side, int value) in Padding)
			{
				_PaddingContainer.AddThemeConstantOverride("margin_" + side, value);
			}

			// Add children to appropriate parents
			WorkingPanel.AddChild(_PaddingContainer);
			_MarginContainer.AddChild(WorkingPanel);

			// Update dependencies and instances
			Dependencies[TraitName + "_WorkingNode"] = WorkingPanel;
			Dependencies[TraitName + "_PanelPaddingContainer"] = _PaddingContainer;
			Dependencies[TraitName + "_MarginContainer"] = _MarginContainer;

			Plugin.Singleton.TraitGlobal.AddInstance(Iteration, WorkingPanel, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.TraitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);

			// Reset and update iteration
			Reset();
			Iteration += 1;
			Dependencies = new();

			return this;
		}

		/// <summary>
		/// Selects a placed panel container in the nodes array by index.
		/// </summary>
		/// <param name="index">The index of the panel container.</param>
		/// <returns>Returns the modified Panelable.</returns>
		public Panelable Select(int index)
		{
			base._Select(index);

			if (false != Dependencies.ContainsKey(TraitName + "_WorkingNode"))
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.TraitGlobal.GetDependencies(index, TypeString, OwnerName);
				Dependencies = dependencies;
			}

			return this;
		}

		/// <summary>
		/// Selects a placed panel container in the nodes array by name.
		/// </summary>
		/// <param name="name">The name of the panel container.</param>
		/// <returns>Returns the modified Panelable.</returns>
		public Panelable SelectByName(string name)
		{
			foreach (Button button in Nodes)
			{
				if (button.Name == name)
				{
					Dependencies["WorkingNode"] = button;
					break;
				}
			}

			return this;
		}

		/// <summary>
		/// Sets the name of the current panel container.
		/// </summary>
		/// <param name="text">The name to set.</param>
		/// <returns>Returns the modified Panelable.</returns>
		public Panelable SetName(string text)
		{
			base._SetName(text);

			return this;
		}

		/// <summary>
		/// Sets the theme type of the panel container.
		/// </summary>
		/// <param name="type">The type of panel container to set.</param>
		/// <returns>Returns the modified Panelable.</returns>
		public Panelable SetType(PanelType type)
		{
			_Type = type;
			return this;
		}

		/// <summary>
		/// Sets the visible state of the current panel container.
		/// </summary>
		/// <param name="state">The visibility state to set.</param>
		/// <returns>Returns the modified Panelable.</returns>
		public Panelable SetVisible(bool state)
		{
			base._SetVisible(state);

			if (
				null != Dependencies &&
				false != Dependencies.ContainsKey(TraitName + "_WorkingNode")
			)
			{
				Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible = state;
			}

			return this;
		}

		/// <summary>
		/// Sets the horizontal size flag of the panel container.
		/// </summary>
		/// <param name="flag">The size flag to set.</param>
		/// <returns>Returns the modified Panelable.</returns>
		public override Panelable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}

		/// <summary>
		/// Sets the vertical size flag of the panel container.
		/// </summary>
		/// <param name="flag">The size flag to set.</param>
		/// <returns>Returns the modified Panelable.</returns>
		public override Panelable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}

		/// <summary>
		/// Sets margin values for the currently chosen panel container.
		/// </summary>
		/// <param name="value">The margin value to set.</param>
		/// <param name="side">The side for which to set the margin.</param>
		/// <returns>Returns the modified Panelable.</returns>
		public Panelable SetMargin(int value, string side = "")
		{
			if (side == "")
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
		/// Sets padding values for the currently chosen panel container.
		/// </summary>
		/// <param name="value">The padding value to set.</param>
		/// <param name="side">The side for which to set the padding.</param>
		/// <returns>Returns the modified Panelable.</returns>
		public Panelable SetPadding(int value, string side)
		{
			base._SetPadding(value, side);

			return this;
		}

		/// <summary>
		/// Returns the inner container of the panel container.
		/// </summary>
		/// <returns>Returns the inner container.</returns>
		public MarginContainer GetContainer()
		{
			if (false == Dependencies.ContainsKey(TraitName + "_PanelPaddingContainer"))
			{
				return null;
			}

			return Dependencies[TraitName + "_PanelPaddingContainer"].As<MarginContainer>();
		}

		/// <summary>
		/// Resets the trait to a cleared state.
		/// </summary>
		private void Reset()
		{
			_MarginContainer = null;
			_PaddingContainer = null;
			_Type = PanelType.DefaultPanelContainer;
		}
	}
}
#endif