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
	/// A containerable component that extends ContainerTrait, providing functionality for working with containers.
	/// </summary>
	[Tool]
	public partial class Containerable : ContainerTrait
	{
		/// <summary>
		/// Constructor for the Containerable class.
		/// </summary>
		public Containerable()
		{
			Name = "Containerable";
			TypeString = GetType().ToString();
		}
		
		/// <summary>
		/// Adds the currently chosen container to a specified container.
		/// </summary>
		/// <param name="Container">The container to which the chosen container will be added.</param>
		/// <param name="index">Optional index at which to add the container.</param>
		public virtual void AddToContainer(Node Container, int? index = null)
		{
			if (null == Dependencies || false == Dependencies.ContainsKey(TraitName + "_MarginContainer"))
			{
				GD.PushError("Container was not found @ AddToContainer");

				if (null == Dependencies)
				{
					return;
				}

				GD.PushError("AddToContainer::Keys-> ", Dependencies.Keys);
				GD.PushError("AddToContainer::ADDTO-> ", TraitName + "_MarginContainer");
				return;
			}

			if (null != Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().GetParent())
			{
				GD.PushError("Container already has a parent @ AddToContainer - ", TraitName, Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().GetParent().Name);

				if (null == Dependencies)
				{
					return;
				}

				GD.PushError("AddToContainer::Keys-> ", Dependencies.Keys);
				GD.PushError("AddToContainer::ADDTO-> ", TraitName + "_MarginContainer");
				return;
			}

			base._AddToContainer(Container, Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>(), index);
		}

		/// <summary>
		/// Instantiate an instance of the trait.
		/// </summary>
		/// <returns>Returns the instantiated Containerable instance.</returns>
		public override Containerable Instantiate()
		{
			base._Instantiate();
			base.Instantiate();

			Plugin.Singleton.TraitGlobal.AddInstance(Iteration, Dependencies[TraitName + "_Container"].As<Container>(), OwnerName, TypeString, Dependencies);
			Plugin.Singleton.TraitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);

			Reset();
			Iteration += 1;
			Dependencies = new();

			return this;
		}

		/// <summary>
		/// Selects a placed container in the nodes array by index.
		/// </summary>
		/// <param name="index">The index of the container to select.</param>
		/// <param name="debug">Optional parameter to enable debugging.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public override Containerable Select(int index, bool debug = false)
		{
			base.Select(index, debug);

			return this;
		}

		/// <summary>
		/// Selects a placed container in the nodes array by name.
		/// </summary>
		/// <param name="name">The name of the container to select.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public override Containerable SelectByName(string name)
		{
			base.SelectByName(name);

			return this;
		}

		/// <summary>
		/// Sets a name for the current container.
		/// </summary>
		/// <param name="text">The name to set.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public Containerable SetName(string text)
		{
			base._SetName(text);

			return this;
		}

		/// <summary>
		/// Sets the layout of the container.
		/// </summary>
		/// <param name="layout">The layout to set.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public override Containerable SetLayout(ContainerLayout layout)
		{
			base.SetLayout(layout);

			return this;
		}

		/// <summary>
		/// Sets the visibility state of the currently chosen container.
		/// </summary>
		/// <param name="state">The visibility state to set.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public override Containerable SetVisible(bool state)
		{
			base.SetVisible(state);

			return this;
		}

		/// <summary>
		/// Toggles the visibility state of the currently chosen container.
		/// </summary>
		/// <param name="debug">Optional parameter to enable debugging.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public override Containerable ToggleVisible( bool debug = false)
		{
			base.ToggleVisible(debug);

			return this;
		}

		/// <summary>
		/// Sets the size of the container.
		/// </summary>
		/// <param name="width">The width to set.</param>
		/// <param name="height">The height to set.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public override Containerable SetDimensions(int width, int height)
		{
			base.SetDimensions(width, height);

			return this;
		}

		/// <summary>
		/// Sets the orientation of the container.
		/// </summary>
		/// <param name="orientation">The orientation to set.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public override Containerable SetOrientation(ContainerOrientation orientation)
		{
			base.SetOrientation(orientation);

			return this;
		}

		/// <summary>
		/// Sets the separation value for the container.
		/// </summary>
		/// <param name="seperation">The separation value to set.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public override Containerable SetSeparation(int seperation)
		{
			base.SetSeparation(seperation);

			return this;
		}

		/// <summary>
		/// Sets the inner orientation of the container.
		/// </summary>
		/// <param name="orientation">The inner orientation to set.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public override Containerable SetInnerOrientation(ContainerOrientation orientation)
		{
			base.SetInnerOrientation(orientation);

			return this;
		}

		/// <summary>
		/// Sets the horizontal size flag, which controls the x axis.
		/// </summary>
		/// <param name="flag">The size flag to set.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public override Containerable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}

		/// <summary>
		/// Sets the vertical size flag, which controls the y axis.
		/// </summary>
		/// <param name="flag">The size flag to set.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public override Containerable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}

		/// <summary>
		/// Sets margin values for the currently chosen container.
		/// </summary>
		/// <param name="value">The margin value.</param>
		/// <param name="side">The side for which to set the margin.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public override Containerable SetMargin(int value, string side = "")
		{
			base.SetMargin(value, side);

			return this;
		}

		/// <summary>
		/// Sets padding values for the currently chosen container.
		/// </summary>
		/// <param name="value">The padding value.</param>
		/// <param name="side">The side for which to set the padding.</param>
		/// <returns>Returns the updated Containerable instance.</returns>
		public override Containerable SetPadding(int value, string side = "")
		{
			base.SetPadding(value, side);

			return this;
		}

		/// <summary>
		/// Returns the outer container of the container layout.
		/// </summary>
		/// <returns>Returns the outer container.</returns>
		public override Container GetOuterContainer()
		{
			return base.GetOuterContainer();
		}

		/// <summary>
		/// Returns an inner container depending on a specified index.
		/// </summary>
		/// <param name="index">The index of the inner container to retrieve. Default is 0.</param>
		/// <returns>Returns the inner container.</returns>
		public override Container GetInnerContainer(int index = 0)
		{
			return base.GetInnerContainer(index);
		}

		/// <summary>
		/// Returns the parent container of the current container.
		/// </summary>
		/// <returns>Returns the parent container.</returns>
		public Node GetContainerParent()
		{
			return GetParentContainer();
		}
	}
}
#endif