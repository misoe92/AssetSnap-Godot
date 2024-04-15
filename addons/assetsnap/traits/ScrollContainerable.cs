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
	[Tool]
	public partial class ScrollContainerable : ContainerTrait
	{
		/*
		** Public methods
		*/
		public VBoxContainer _ScrollInnerContainer;
		public MarginContainer _ScrollPaddingContainer;
		
		public ScrollContainerable()
		{
			TypeString = GetType().ToString();
		}
		
		/*
		** Instantiate an instance of the trait
		**
		** @return ScrollContainerable
		*/	
		public override ScrollContainerable Instantiate()
		{
			UsePaddingContainer = false;
			base._Instantiate();
			base.Instantiate();
			
			ScrollContainer _WorkingNode = new()
			{
				Name="Scroll",
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			};
			
			_ScrollPaddingContainer = new()
			{
				Name="ScrollPadding",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};

			_ScrollInnerContainer = new()
			{
				Name="ScrollInner",
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			};
			
			foreach( (string side, int value ) in Padding ) 
			{
				_ScrollPaddingContainer.AddThemeConstantOverride("margin_" + side, value);
			}

			_ScrollPaddingContainer.AddChild(_ScrollInnerContainer);
			_WorkingNode.AddChild(_ScrollPaddingContainer);
			GetInnerContainer(0).AddChild(_WorkingNode);
		
			Dependencies[TraitName + "_WorkingNode"] = _WorkingNode;
			Dependencies[TraitName + "_ScrollPaddingContainer"] = _ScrollPaddingContainer;
			Dependencies[TraitName + "_ScrollInnerContainer"] = _ScrollInnerContainer;
			
			Plugin.Singleton.traitGlobal.AddInstance(Iteration, _WorkingNode, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.traitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);

			Reset();
			Iteration += 1;
			Dependencies = new();
			
			return this;
		}
		
		/*
		** Selects an placed scroll container
		** in the nodes array by index
		**
		** @param int index
		** @return ScrollContainerable
		*/
		public override ScrollContainerable Select(int index, bool debug = false)
		{
			base._Select(index, debug);
					
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode") ) 
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.traitGlobal.GetDependencies(index, TypeString, OwnerName);
				Dependencies = dependencies;
			}
			
			return this;
		}
		
		/*
		** Selects an placed scroll container
		** in the nodes array by name
		**
		** @param string name
		** @return ScrollContainerable
		*/
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
		
		/*
		** Adds the currently chosen scroll
		** container to a specified container
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
			
			base._AddToContainer(Container, Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>());
		}
		
		/*
		** Setter Methods
		*/
		
		/*
		** Sets the name of the current scroll container
		**
		** @param string text
		** @return ScrollContainerable
		*/
		public ScrollContainerable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/*
		** Sets the visibility state of the
		** currently chosen scroll container
		**
		** @param bool state
		** @return ScrollContainerable
		*/
		public override ScrollContainerable SetVisible( bool state ) 
		{
			base.SetVisible(state);

			return this;
		}
		
		/*
		** Sets the dimensions of the
		** currently chosen scroll container
		**
		** @param bool state
		** @return ScrollContainerable
		*/
		public override ScrollContainerable SetDimensions( int width, int height ) 
		{
			base.SetDimensions(width,height);

			return this;
		}
		
		/*
		** Sets the minimum dimensions of the
		** currently chosen scroll container
		**
		** @param bool state
		** @return ScrollContainerable
		*/
		public override ScrollContainerable SetMinimumDimension( int width, int height ) 
		{
			base.SetMinimumDimension(width,height);

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return ScrollContainerable
		*/
		public override ScrollContainerable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return ScrollContainerable
		*/
		public override ScrollContainerable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}
		
		/*
		** Sets the orientation of the scroll container
		**
		** @param ContainerOrientation orientation
		** @return ScrollContainerable
		*/
		public override ScrollContainerable SetOrientation(ContainerOrientation orientation) 
		{
			base.SetOrientation(orientation);

			return this;
		}
		
		/*
		** Sets margin values for 
		** the currently chosen scroll container
		**
		** @param int value
		** @param string side
		** @return ScrollContainerable
		*/
		public override ScrollContainerable SetMargin( int value, string side = "" ) 
		{
			base.SetMargin(value, side);
			
			return this;
		}
		
		/*
		** Getter Methods
		*/
		
		/*
		** Returns the inner container
		**
		** @return Container
		*/
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
		
		/*
		** Private Methods
		*/
		
		/*
		** Resets the trait to
		** a cleared state
		**
		** @return void
		*/
		protected override void Reset()
		{
			Orientation = ContainerOrientation.Vertical;

			base.Reset();
		}
	}
}
#endif