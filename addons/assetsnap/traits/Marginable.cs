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
	[Tool]
	public partial class Marginable : Trait.Base
	{
		/*
		** Private
		*/
		private new Godot.Collections.Dictionary<string, int> Margin = new()
		{
			{"left", 20},
			{"right", 20},
			{"top", 0},
			{"bottom", 25},
		};
		private Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		private Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		
		/*
		** Public methods
		*/
		
		/*
		** Instantiate an instance of the trait
		**
		** @return Marginable
		*/	
		public Marginable Instantiate()
		{
			base._Instantiate( GetType().ToString() );
		
			MarginContainer _WorkingNode = new()
			{
				Name = Name,
			};
		
			foreach( (string side, int value ) in Margin ) 
			{
				_WorkingNode.AddThemeConstantOverride("margin_" + side, value);
			}

			Nodes.Add(_WorkingNode);
			WorkingNode = _WorkingNode;

			Reset();
			
			return this;
		}
		
		/*
		** Selects an placed margin container
		** in the nodes array by index
		**
		** @param int index
		** @return Marginable
		*/
		public Marginable Select(int index)
		{
			base._Select(index);

			return this;
		}
		
		/*
		** Selects an placed margin container
		** in the nodes array by name
		**
		** @param string name
		** @return Marginable
		*/
		public Marginable SelectByName(string name)
		{
			base._SelectByName(name);

			return this;
		}
		
		/*
		** Adds the currently chosen margin
		** container to a specified container
		**
		** @param Node Container
		** @return void
		*/
		public void AddToContainer( Node Container ) 
		{
			base._AddToContainer(Container, WorkingNode);
		}
		
		/*
		** Setter Methods
		*/
		
		/*
		** Sets the name of the current margin container
		**
		** @param string text
		** @return Marginable
		*/
		public Marginable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/*
		** Sets the visibility state of the
		** currently chosen margin container
		**
		** @param bool state
		** @return Marginable
		*/
		public Marginable SetVisible( bool state ) 
		{
			base._SetVisible(state);
			
			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Marginable
		*/
		public Marginable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Marginable
		*/
		public Marginable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;

			return this;
		}
		
		/*
		** Sets margin values for the
		** currently chosen margin container
		**
		** @param int value
		** @param string side
		** @return Marginable
		*/
		public Marginable SetMargin( int value, string side = "" ) 
		{
			_SetMargin(value, side);
			
			return this;
		}
		
		/*
		** Getter methods
		*/
		
		/*
		** Fetches the margin container node
		**
		** @return MarginContainer
		*/
		public override MarginContainer GetNode() 
		{
			Node _node = base.GetNode();
			
			return _node as MarginContainer;
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
		private void Reset()
		{
			WorkingNode = null;
		}
	}
}
#endif