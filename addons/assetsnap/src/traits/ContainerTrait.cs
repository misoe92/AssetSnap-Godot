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
namespace AssetSnap.Trait
{
	using Godot;

	[Tool]
	public partial class ContainerTrait : Base
	{
		/*
		** Enums
		*/
		public enum ContainerLayout
		{
			OneColumn,
			TwoColumns,
			ThreeColumns,
			FourColumns,
		};

		public enum ContainerOrientation
		{
			Horizontal,
			Vertical,
		};

		/*
		** Protected
		*/
		protected ContainerLayout Layout = ContainerLayout.OneColumn;
		protected ContainerOrientation Orientation = ContainerOrientation.Vertical;
		protected ContainerOrientation InnerOrientation = ContainerOrientation.Horizontal;

		protected Control.SizeFlags ContainerHorizontalSizeFlag = Control.SizeFlags.ExpandFill;

		protected int Seperation = 1;

		protected bool UsePaddingContainer = true;

		/// <summary>
		/// Shows the current container.
		/// </summary>
		public void Show()
		{
			Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible = true;
		}

		/// <summary>
		/// Hides the current container.
		/// </summary>
		public void Hide()
		{
			Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible = false;
		}

		/// <summary>
		/// Selects a placed container in the nodes array by index.
		/// </summary>
		/// <param name="index">The index of the container.</param>
		/// <param name="debug">Optional parameter to enable debugging.</param>
		/// <returns>The selected container.</returns>
		public override ContainerTrait Select(int index, bool debug = false)
		{
			base._Select(index, debug);

			if (false == Dependencies.ContainsKey(TraitName + "_WorkingNode"))
			{
				if (debug)
				{
					GD.PushError("Node was false @ ContainerTrait -> @", OwnerName, "::", TraitName, "::", TypeString);
					GD.PushError("KEYS::", Dependencies.Keys);
				}

				return this;
			}

			Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.traitGlobal.GetDependencies(index, TypeString, OwnerName);
			Dependencies = dependencies;

			return this;
		}

		/// <summary>
		/// Selects a placed container in the nodes array by name.
		/// </summary>
		/// <param name="name">The name of the container to select.</param>
		/// <returns>The selected container.</returns>
		public virtual ContainerTrait SelectByName(string name)
		{
			foreach (Container container in Nodes)
			{
				if (container.Name == name)
				{
					break;
				}
			}

			return this;
		}

		/// <summary>
		/// Sets the layout of the container.
		/// </summary>
		/// <param name="layout">The layout to set.</param>
		/// <returns>The container with the updated layout.</returns>
		public virtual ContainerTrait SetLayout(ContainerLayout layout)
		{
			Layout = layout;

			return this;
		}

		/// <summary>
		/// Sets the horizontal size flag of the container.
		/// </summary>
		/// <param name="flag">The size flag to set.</param>
		/// <returns>The container with the updated size flag.</returns>
		public virtual ContainerTrait SetContainerHorizontalSizeFlag(Control.SizeFlags flag)
		{
			ContainerHorizontalSizeFlag = flag;

			return this;
		}

		/// <summary>
		/// Sets the visibility state of the currently chosen container.
		/// </summary>
		/// <param name="state">The visibility state to set.</param>
		/// <returns>The container with the updated visibility state.</returns>
		public virtual ContainerTrait SetVisible(bool state)
		{
			Visible = state;

			if (
				null != Dependencies &&
				false != Dependencies.ContainsKey(TraitName + "_MarginContainer")
			)
			{
				Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible = state;
			}

			return this;
		}

		/// <summary>
		/// Toggles the visibility state of the currently chosen container.
		/// </summary>
		/// <param name="debug">Optional parameter to enable debugging.</param>
		/// <returns>The container with the toggled visibility state.</returns>
		public virtual ContainerTrait ToggleVisible( bool debug = false)
		{
			Visible = !Visible;
			
			if (
				null != Dependencies &&
				false != Dependencies.ContainsKey(TraitName + "_MarginContainer")
			)
			{
				Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible = !Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible;
				if( debug )
				{
					GD.PushWarning("Visibility set");
				}
			}
			else 
			{
				if( debug )
				{
					GD.PushWarning("No dependencies found when trying to toggle visibility");
				}
			}

			return this;
		}

		/// <summary>
		/// Sets the orientation of the container.
		/// </summary>
		/// <param name="orientation">The orientation to set.</param>
		/// <returns>The container with the updated orientation.</returns>
		public virtual ContainerTrait SetOrientation(ContainerOrientation orientation)
		{
			Orientation = orientation;
			return this;
		}

		/// <summary>
		/// Sets the separation of the containers.
		/// </summary>
		/// <param name="seperation">The separation value to set.</param>
		/// <returns>The container with the updated separation.</returns>
		public virtual ContainerTrait SetSeparation(int seperation)
		{
			Seperation = seperation;
			return this;
		}

		/// <summary>
		/// Sets the inner orientation of the container.
		/// </summary>
		/// <param name="orientation">The inner orientation to set.</param>
		/// <returns>The container with the updated inner orientation.</returns>
		public virtual ContainerTrait SetInnerOrientation(ContainerOrientation orientation)
		{
			InnerOrientation = orientation;
			return this;
		}


		/// <summary>
		/// Sets margin values for the currently chosen container.
		/// </summary>
		/// <param name="value">The value of the margin.</param>
		/// <param name="side">The side of the margin (optional).</param>
		/// <returns>The container with the updated margin values.</returns>
		public virtual ContainerTrait SetMargin(int value, string side = "")
		{
			_SetMargin(value, side);

			if (side == "")
			{
				if (false != Dependencies.ContainsKey(TraitName + "_MarginContainer"))
				{
					foreach ((string marginSide, int marginValue) in Margin)
					{
						Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().AddThemeConstantOverride("margin_" + marginSide, marginValue);
					}
				}
			}
			else
			{
				if (false != Dependencies.ContainsKey(TraitName + "_MarginContainer"))
				{
					Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().AddThemeConstantOverride("margin_" + side, value);
				}
			}

			return this;
		}

		/// <summary>
		/// Sets padding values for the currently chosen container.
		/// </summary>
		/// <param name="value">The value of the padding.</param>
		/// <param name="side">The side of the padding (optional).</param>
		/// <returns>The container with the updated padding values.</returns>
		public virtual ContainerTrait SetPadding(int value, string side = "")
		{
			_SetPadding(value, side);

			if (side == "")
			{
				if (false != Dependencies.ContainsKey(TraitName + "_PaddingContainer"))
				{
					foreach ((string marginSide, int marginValue) in Margin)
					{
						Dependencies[TraitName + "_PaddingContainer"].As<MarginContainer>().AddThemeConstantOverride("margin_" + marginSide, marginValue);
					}
				}
			}
			else
			{
				if (false != Dependencies.ContainsKey(TraitName + "_PaddingContainer"))
				{
					Dependencies[TraitName + "_PaddingContainer"].As<MarginContainer>().AddThemeConstantOverride("margin_" + side, value);
				}
			}

			return this;
		}

		/// <summary>
		/// Gets the parent container of the currently chosen container.
		/// </summary>
		/// <returns>The parent container.</returns>
		public virtual Node GetParentContainer()
		{
			if (false != Dependencies.ContainsKey(TraitName + "_MarginContainer"))
			{
				// Single placement
				return Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().GetParent();
			}

			return null;
		}

		/// <summary>
		/// Returns the outer container of the container layout.
		/// </summary>
		/// <returns>The outer container.</returns>
		public virtual Container GetOuterContainer()
		{
			if (false != Dependencies.ContainsKey(TraitName + "_InnerContainer"))
			{
				// Single placement
				return Dependencies[TraitName + "_InnerContainer"].As<Container>();
			}

			return null;
		}

		/// <summary>
		/// Returns an inner container depending on a specified index.
		/// </summary>
		/// <param name="index">The index of the inner container.</param>
		/// <returns>The inner container.</returns>
		public virtual Container GetInnerContainer(int index)
		{
			if (null == Dependencies || Dependencies.Count == 0)
			{
				GD.PushError("No dependencies");
				return null;
			}

			if (
				false != Dependencies.ContainsKey(TraitName + "_InnerContainer") &&
				null != Dependencies[TraitName + "_InnerContainer"].As<Container>().GetChild(index)
			)
			{
				// Single placement
				return Dependencies[TraitName + "_InnerContainer"].As<Container>().GetChild(index) as Container;
			}
			else
			{
				GD.Print("Not found @ Inner Container ", TraitName + "_InnerContainer", " ", Dependencies.Keys);
			}

			return null;
		}

		/// <summary>
		/// Checks if the container is visible.
		/// </summary>
		/// <param name="debug">Optional parameter to enable debugging.</param>
		/// <returns>True if the container is visible; otherwise, false.</returns>
		public virtual bool IsVisible( bool debug = false )
		{
			if (false != Dependencies.ContainsKey(TraitName + "_MarginContainer"))
			{
				if( debug ) 
				{
					GD.Print("Visibility state found", Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible);
				}
				return Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible == true;
			}
			
			if( debug ) 
			{
				GD.Print("Visibility state not found");
			}

			return false;
		}

		/// <summary>
		/// Instantiates the container trait.
		/// </summary>
		/// <returns>The instantiated container trait.</returns>
		public virtual ContainerTrait Instantiate()
		{
			int ColumnCount = (int)Layout + 1;
			string prefix = TraitName;

			MarginContainer _MarginContainer = new()
			{
				Name = prefix + "-ContainerMargin",
				SizeFlagsHorizontal = ContainerHorizontalSizeFlag,
				SizeFlagsVertical = SizeFlagsVertical,
				Visible = Visible,
			};

			if (Size != Vector2.Zero)
			{
				_MarginContainer.Size = Size;
			}

			if (CustomMinimumSize != Vector2.Zero)
			{
				_MarginContainer.CustomMinimumSize = CustomMinimumSize;
			}

			MarginContainer _PaddingContainer = new()
			{
				Name = prefix + "-ContainerPadding",
				SizeFlagsHorizontal = ContainerHorizontalSizeFlag,
				SizeFlagsVertical = SizeFlagsVertical,
			};

			Container _InnerContainer;

			if (InnerOrientation == ContainerOrientation.Vertical)
			{
				_InnerContainer = new VBoxContainer()
				{
					Name = prefix + "-ContainerInner",
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
					SizeFlagsVertical = SizeFlagsVertical,
				};
			}
			else
			{
				_InnerContainer = new HBoxContainer()
				{
					Name = prefix + "-ContainerInner",
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
					SizeFlagsVertical = SizeFlagsVertical,
				};
			}

			foreach ((string side, int value) in Margin)
			{
				_MarginContainer.AddThemeConstantOverride("margin_" + side, value);
			}

			VBoxContainer _ContainerNode = new()
			{
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
				Name = prefix + "_Container",
			};

			// if( TypeString == "AssetSnap.Component.Containerable") 
			// {
			// 	GD.Print(SizeFlagsVertical, Name );
			// }

			if (UsePaddingContainer)
			{
				foreach ((string side, int value) in Padding)
				{
					_PaddingContainer.AddThemeConstantOverride("margin_" + side, value);
				}
			}

			for (int i = 0; i < ColumnCount; i++)
			{
				Container innerContainer = Orientation == ContainerOrientation.Horizontal ? new HBoxContainer() : new VBoxContainer();
				innerContainer.SizeFlagsHorizontal = SizeFlagsHorizontal;
				innerContainer.SizeFlagsVertical = SizeFlagsVertical;
				innerContainer.Name = prefix + "-inner-" + i;

				innerContainer.AddThemeConstantOverride("separation", Seperation);

				_InnerContainer.AddChild(innerContainer);
			}

			if (Dependencies.ContainsKey(prefix + "_InnerContainer"))
			{
				GD.Print(prefix + "_InnerContainer already exists");
			}

			Dependencies.Add(prefix + "_InnerContainer", _InnerContainer);
			Dependencies.Add(prefix + "_PaddingContainer", _PaddingContainer);
			Dependencies.Add(prefix + "_Container", _ContainerNode);
			Dependencies.Add(prefix + "_MarginContainer", _MarginContainer);

			_PaddingContainer.AddChild(_InnerContainer);
			_ContainerNode.AddChild(_PaddingContainer);
			_MarginContainer.AddChild(_ContainerNode);

			return this;
		}

		/// <summary>
		/// Resets the trait to a cleared state.
		/// </summary>
		protected virtual void Reset()
		{
			Layout = ContainerLayout.OneColumn;
			Orientation = ContainerOrientation.Vertical;
			InnerOrientation = ContainerOrientation.Vertical;
			Size = Vector2.Zero;
			CustomMinimumSize = Vector2.Zero;
			Dependencies = new();
			TraitName = "";
		}
	}
}
#endif