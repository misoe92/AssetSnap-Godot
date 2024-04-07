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
		
		/*
		** Instantiate an instance of the trait
		**
		** @return ScrollContainerable
		*/	
		public override ScrollContainerable Instantiate()
		{
			UsePaddingContainer = false;
			base._Instantiate( GetType().ToString() );
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
			
			Nodes.Add(_WorkingNode);
			WorkingNode = _WorkingNode;

			Reset();
			
			return this;
		}
		
		/*
		** Selects an placed scroll container
		** in the nodes array by index
		**
		** @param int index
		** @return ScrollContainerable
		*/
		public override ScrollContainerable Select(int index)
		{
			base._Select(index);
		
			if( null != WorkingNode ) 
			{
				_InnerContainer = WorkingNode.GetParent().GetParent() as Container;
			}
			
			if( null != WorkingNode ) 
			{
				_MarginContainer = WorkingNode.GetParent().GetParent().GetParent().GetParent().GetParent() as MarginContainer;
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
					WorkingNode = container;
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
			base._AddToContainer(Container, _MarginContainer);
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
			if( null != WorkingNode ) 
			{
				// Single placement
				return _ScrollInnerContainer as Container;
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
			WorkingNode = null;
			Orientation = ContainerOrientation.Vertical;

			base.Reset();
		}

		/*
		** Cleanup
		*/
		public override void _ExitTree()
		{
			if( EditorPlugin.IsInstanceValid(WorkingNode) ) 
			{
				WorkingNode.QueueFree();
			}
		}
	}
}
#endif