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
		public new Godot.Collections.Dictionary<string, int> Margin = new()
		{
			{"left", 20},
			{"right", 20},
			{"top", 0},
			{"bottom", 25},
		};
		
		private Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		private Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		
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
		
		public Marginable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		public Marginable SetVisible( bool state ) 
		{
			base._SetVisible(state);
			
			return this;
		}
		
		public Marginable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;

			return this;
		}
		
		public Marginable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;

			return this;
		}
		
		public Marginable Select(int index)
		{
			base._Select(index);

			return this;
		}
		
		public Marginable SelectByName(string name)
		{
			base._SelectByName(name);

			return this;
		}
		
		public Marginable SetMargin( int value, string side = "" ) 
		{
			_SetMargin(value, side);
			
			return this;
		}
		
		public override MarginContainer GetNode() 
		{
			Node _node = base.GetNode();
			
			return _node as MarginContainer;
		}
		
		public void AddToContainer( Node Container ) 
		{
			base._AddToContainer(Container, WorkingNode);
		}
		
		private void Reset()
		{
			WorkingNode = null;
		}
	}
}
#endif