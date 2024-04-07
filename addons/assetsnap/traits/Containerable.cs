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
	public partial class Containerable : ContainerTrait
	{
		/*
		** Public methods
		*/
		
		/*
		** Instantiate an instance of the trait
		**
		** @return Containerable
		*/	
		public override Containerable Instantiate()
		{
			base._Instantiate( GetType().ToString() );
			base.Instantiate();

			Nodes.Add(_ContainerNode);
			WorkingNode = _ContainerNode;

			Reset();
			
			return this;
		}
		
		/*
		** Selects an placed container in the
		** nodes array by index
		**
		** @param int index
		** @return Containerable
		*/
		public override Containerable Select(int index)
		{
			base.Select(index);
			
			return this;
		}
		
		/*
		** Selects an placed container in the
		** nodes array by name
		**
		** @param string name
		** @return Containerable
		*/
		public override Containerable SelectByName( string name ) 
		{
			base.SelectByName(name);
			
			return this;
		}
		
		/*
		** Adds the currently chosen container
		** to a specified container
		**
		** @param Node Container
		** @param int|null index
		** @return void
		*/
		public void AddToContainer( Node Container, int? index = null ) 
		{
			base._AddToContainer(Container, _MarginContainer, index);
		}
		
		/*
		** Setter Methods
		*/
		
		/*
		** Sets the name of the current container
		**
		** @param string text
		** @return Containerable
		*/
		public Containerable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/*
		** Sets the layout of the container
		**
		** @param ContainerLayout layout
		** @return Containerable
		*/
		public override Containerable SetLayout( ContainerLayout layout ) 
		{
			base.SetLayout(layout);
			
			return this;
		}
		
		/*
		** Sets the visibility state of the
		** currently chosen container
		**
		** @param bool state
		** @return Containerable
		*/
		public override Containerable SetVisible( bool state ) 
		{
			base.SetVisible(state);
			
			return this;
		}
		
		/*
		** Toggles the visibility state of the
		** currently chosen container
		**
		** @return Containerable
		*/
		public override Containerable ToggleVisible() 
		{
			base.ToggleVisible();
			
			return this;
		}
		
		/*
		** Sets the size of the container
		**
		** @param int width
		** @param int height
		** @return Containerable
		*/
		public override Containerable SetDimensions( int width, int height )
		{
			base.SetDimensions(width, height);
			
			return this;
		}
		
		/*
		** Sets the orientation of the container
		**
		** @param ContainerOrientation orientation
		** @return Containerable
		*/
		public override Containerable SetOrientation(ContainerOrientation orientation) 
		{
			base.SetOrientation(orientation);
			
			return this;
		}
		
		/*
		** Sets the inner orientation of the container
		**
		** @param ContainerOrientation orientation
		** @return Containerable
		*/
		public override Containerable SetInnerOrientation(ContainerOrientation orientation) 
		{
			base.SetInnerOrientation(orientation);
			
			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Containerable
		*/
		public override Containerable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Containerable
		*/
		public override Containerable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);
			
			return this;
		}
		
		/*
		** Sets margin values for 
		** the currently chosen container
		**
		** @param int value
		** @param string side
		** @return Containerable
		*/
		public override Containerable SetMargin( int value, string side = "" ) 
		{
			base.SetMargin(value, side);
			
			return this;
		}
		
		/*
		** Sets padding values for 
		** the currently chosen container
		**
		** @param int value
		** @param string side
		** @return Containerable
		*/
		public override Containerable SetPadding( int value, string side = "" ) 
		{
			base.SetPadding(value, side);
			
			return this;
		}
		
		/*
		** Getter Methods
		*/
		
		/*
		** Returns the outer container
		** of the container layout
		**
		** @return Container
		*/
		public override Container GetOuterContainer()
		{
			return base.GetOuterContainer();
		}
		
		/*
		** Returns a inner container
		** depending on a specified index
		**
		** @param int(0) index
		** @return Container
		*/
		public override Container GetInnerContainer( int index = 0 )
		{
			return base.GetInnerContainer( index );
		}
	}
}
#endif