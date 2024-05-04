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
	/// A partial class representing a scrollable container, derived from ContainerTrait.
	/// </summary>
	[Tool]
	public partial class ScrollContainerable : ContainerTrait
	{
		/// <summary>
		/// The inner VBoxContainer of the scroll container.
		/// </summary>
		public VBoxContainer ScrollInnerContainer;
		
		/// <summary>
		/// The scroll container padding container.
		/// </summary>
		public MarginContainer ScrollPaddingContainer;
		
		/// <summary>
		/// Default constructor for ScrollContainerable.
		/// </summary>
		public ScrollContainerable()
		{
			Name = "ScrollContainerable";
			TypeString = GetType().ToString();
		}
		
		/// <summary>
        /// Adds the currently chosen scroll container to a specified container.
        /// </summary>
        /// <param name="Container">The container to which to add the scroll container.</param>
		public void AddToContainer( Node Container ) 
		{
			if( false == Dependencies.ContainsKey(TraitName + "_MarginContainer") ) 
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
		/// <returns>Returns the instantiated ScrollContainerable.</returns>
		public override ScrollContainerable Instantiate()
		{
			UsePaddingContainer = false;
			base._Instantiate();
			base.Instantiate();
			
			// Create ScrollContainer and its inner containers
			ScrollContainer _WorkingNode = new()
			{
				Name="Scroll",
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			};
			
			ScrollPaddingContainer = new()
			{
				Name="ScrollPadding",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};

			ScrollInnerContainer = new()
			{
				Name="ScrollInner",
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			};
			
			// Apply padding to the ScrollPaddingContainer
			foreach( (string side, int value ) in Padding ) 
			{
				ScrollPaddingContainer.AddThemeConstantOverride("margin_" + side, value);
			}

			// Add inner containers to appropriate parent nodes
			ScrollPaddingContainer.AddChild(ScrollInnerContainer);
			_WorkingNode.AddChild(ScrollPaddingContainer);
			GetInnerContainer(0).AddChild(_WorkingNode);

			// Update dependencies and instances
			Dependencies[TraitName + "_WorkingNode"] = _WorkingNode;
			Dependencies[TraitName + "_ScrollPaddingContainer"] = ScrollPaddingContainer;
			Dependencies[TraitName + "_ScrollInnerContainer"] = ScrollInnerContainer;
			
			Plugin.Singleton.TraitGlobal.AddInstance(Iteration, _WorkingNode, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.TraitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);

			Reset();
			Iteration += 1;
			Dependencies = new();
			
			return this;
		}
		
		/// <summary>
		/// Selects a placed scroll container in the nodes array by index.
		/// </summary>
		/// <param name="index">The index of the scroll container.</param>
		/// <param name="debug">Whether to print debug information.</param>
		/// <returns>Returns the modified ScrollContainerable.</returns>
		public override ScrollContainerable Select(int index, bool debug = false)
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
		/// Selects a placed scroll container in the nodes array by name.
		/// </summary>
		/// <param name="name">The name of the scroll container.</param>
		/// <returns>Returns the modified ScrollContainerable.</returns>
		public override ScrollContainerable SelectByName( string name ) 
		{
			foreach( Container container in Nodes ) 
			{
				if( container.Name == name ) 
				{
					Dependencies[TraitName + "_WorkingNode"] = container;
					break;
				}
			}

			return this;
		}
		
		/// <summary>
		/// Sets the name of the current scroll container.
		/// </summary>
		/// <param name="text">The name to set.</param>
		/// <returns>Returns the modified ScrollContainerable.</returns>
		public ScrollContainerable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/// <summary>
		/// Sets the visibility state of the currently chosen scroll container.
		/// </summary>
		/// <param name="state">The visibility state to set.</param>
		/// <returns>Returns the modified ScrollContainerable.</returns>
		public override ScrollContainerable SetVisible( bool state ) 
		{
			base.SetVisible(state);

			return this;
		}
		
		/// <summary>
		/// Sets the dimensions of the currently chosen scroll container.
		/// </summary>
		/// <param name="width">The width of the container.</param>
		/// <param name="height">The height of the container.</param>
		/// <returns>Returns the modified ScrollContainerable.</returns>
		public override ScrollContainerable SetDimensions( int width, int height ) 
		{
			base.SetDimensions(width,height);

			return this;
		}
		
		/// <summary>
		/// Sets the minimum dimensions of the currently chosen scroll container.
		/// </summary>
		/// <param name="width">The minimum width of the container.</param>
		/// <param name="height">The minimum height of the container.</param>
		/// <returns>Returns the modified ScrollContainerable.</returns>
		public override ScrollContainerable SetMinimumDimension( int width, int height ) 
		{
			base.SetMinimumDimension(width,height);

			return this;
		}
		
		/// <summary>
		/// Sets the horizontal size flag for the container.
		/// </summary>
		/// <param name="flag">The horizontal size flag to set.</param>
		/// <returns>Returns the modified ScrollContainerable.</returns>
		public override ScrollContainerable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}
		
		/// <summary>
		/// Sets the vertical size flag for the container.
		/// </summary>
		/// <param name="flag">The vertical size flag to set.</param>
		/// <returns>Returns the modified ScrollContainerable.</returns>
		public override ScrollContainerable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}
		
		/// <summary>
		/// Sets the orientation of the scroll container.
		/// </summary>
		/// <param name="orientation">The orientation to set.</param>
		/// <returns>Returns the modified ScrollContainerable.</returns>
		public override ScrollContainerable SetOrientation(ContainerOrientation orientation) 
		{
			base.SetOrientation(orientation);

			return this;
		}
		
		/// <summary>
		/// Sets margin values for the currently chosen scroll container.
		/// </summary>
		/// <param name="value">The margin value to set.</param>
		/// <param name="side">The side for which to set the margin.</param>
		/// <returns>Returns the modified ScrollContainerable.</returns>
		public override ScrollContainerable SetMargin( int value, string side = "" ) 
		{
			base.SetMargin(value, side);
			
			return this;
		}
		
		/// <summary>
		/// Returns the inner container of the scroll container.
		/// </summary>
		/// <returns>Returns the inner container.</returns>
		public Container GetScrollContainer()
		{
			if( null != Dependencies && false != Dependencies.ContainsKey(TraitName + "_ScrollInnerContainer") ) 
			{
				// Single placement
				return Dependencies[TraitName + "_ScrollInnerContainer"].As<Container>();
			}
			
			if( null == Dependencies ) 
			{
				GD.PushError("No dependencies set");
				return null;
			}
			
			if( false == Dependencies.ContainsKey(TraitName + "_ScrollInnerContainer") ) 
			{
				GD.PushError("No scroll inner container set");
				return null;
			}

			return null;
		}
		
		/// <summary>
		/// Resets the trait to a cleared state.
		/// </summary>
		protected override void Reset()
		{
			Orientation = ContainerOrientation.Vertical;
			base.Reset();
		}
	}
}
#endif