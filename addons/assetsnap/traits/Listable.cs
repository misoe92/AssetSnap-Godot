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
using System.Collections.Generic;
using Godot;

namespace AssetSnap.Component
{
	[Tool]
	public partial class Listable : Trait.Base
	{
		public new Godot.Collections.Dictionary<string, int> Margin = new()
		{
			{"left", 20},
			{"right", 20},
			{"top", 0},
			{"bottom", 25},
		};
		
		public string ComponentName = "";
		
		public int Count = 0;
		
		private Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		private Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		
		private Vector2 CustomMinimumSize;
		private Vector2 Size;
		
		public Action<int, BaseComponent> OnIterationAction;
		
		public Listable Instantiate()
		{
			try
			{
				base._Instantiate( GetType().ToString() );
				
				VBoxContainer _WorkingNode = new()
				{
					CustomMinimumSize = CustomMinimumSize,
					Size = Size,
				};
				
				for( int i = 0; i < Count; i++) 
				{
					string[] ComponentSingleArr = ComponentName.Split(".");
					string ComponentSingleName = ComponentSingleArr[ComponentSingleArr.Length - 1];
					
					List<string> Components = new()
					{
						ComponentSingleName,
					};
					
					if (GlobalExplorer.GetInstance().Components.HasAll( Components.ToArray() )) 
					{
						BaseComponent component = GlobalExplorer.GetInstance().Components.Single(ComponentName, true);
						
						if( null != component && IsInstanceValid( component ) ) 
						{
							component.Container = _WorkingNode;
							OnIterationAction(i, component);
						}
					}
				}

				Nodes.Add(_WorkingNode);
				WorkingNode = _WorkingNode;
				Reset();
			}
			catch( Exception e) 
			{
				throw;
			}
			
			return this;
		}
		
		public Listable Select( int index ) 
		{
			base._Select(index);

			return this;
		}	
		
		public Listable SetDimensions( int width, int height )
		{
			CustomMinimumSize = new Vector2( width, height);
			Size = new Vector2( width, height);

			return this;
		}
		
		public Listable SelectByName( string name ) 
		{
			base._SelectByName(name);

			return this;
		}
		
		public Listable SetName( string text ) 
		{
			Name = text;

			return this;
		}
		
		public Listable SetCount( int count ) 
		{
			Count = count;

			return this;
		}
		
		public Listable SetComponent( string componentName ) 
		{
			ComponentName = componentName;

			return this;
		}
		
		public Listable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;

			return this;
		}
		
		public Listable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;

			return this;
		}
		
		public Listable Each( Action<int, BaseComponent> OnIteration ) 
		{
			OnIterationAction = OnIteration;
			
			return this;
		}
		
		public void AddToContainer( Node Container ) 
		{
			try 
			{
				base._AddToContainer(Container, WorkingNode);
			}
			catch( Exception e ) 
			{
				throw;
			}
		}
		
		private void Reset()
		{
			WorkingNode = null;
			Count = 0;
		}

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