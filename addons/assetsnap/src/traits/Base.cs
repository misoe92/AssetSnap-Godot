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
	using System;
	using Godot;
	
	[Tool]
	public partial class Base : Node
	{
		/** Defines if a select statement has failed **/
		protected bool _select = true;
		public bool disposed = false;

		public Godot.Collections.Dictionary<string, int> Margin = new()
		{
			{"left", 0},
			{"right", 0},
			{"top", 0},
			{"bottom", 0},
		};
		
		public Godot.Collections.Dictionary<string, int> Padding = new()
		{
			{"left", 0},
			{"right", 0},
			{"top", 0},
			{"bottom", 0},
		};

		protected Godot.Collections.Array<GodotObject> Nodes = new();

		public bool Visible = true;

		protected Node WorkingNode;
		
		public string TypeString = "";
	
		public void _Instantiate( string typeString )
		{
			if( Name == "" ) 
			{
				GD.PushWarning("No name was set for the traitable: ", typeString);
			}

			TypeString = typeString;
		}
		
		protected void _AddToContainer( Node Container, Node Node, int? index = null )
		{
			if( null == Node ) 
			{
				GD.PushError("No node selected: ", Name, TypeString );
				return;
			}
			
			if( null == Container ) 
			{
				GD.PushError("Provided container is invalid: ", Name, TypeString );
				return;
			}

			if( Node == Container ) 
			{
				GD.PushError("Provided container is the same as this object: ", Name, TypeString );
				return;
			}
			
			if( null != Node ) 
			{
				// Single placement
				if( index is int intIndex ) 
				{
					Container.AddChild(Node);
					Container.MoveChild(Node, intIndex);
				}
				else
				{
					Container.AddChild(Node);
				}
			}
			else 
			{
				// Multi placement
				foreach( Node node in Nodes ) 
				{
					Container.AddChild(node);
					if( index is int intIndex ) 
					{
						Container.MoveChild(node, intIndex);
					}
				}
			}
		}
		
		public virtual bool IsValid()
		{
			return _select != false && disposed != true;
		}
		
		public bool ContainsIndex( int index ) 
		{
			return Nodes.Count >= index;
		}
		
		public virtual void _SetMargin( int value, string side ) 
		{
			if( side == "" ) 
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
		}
		
		public virtual void _SetPadding( int value, string side ) 
		{
			if( side == "" ) 
			{
				Padding["top"] = value;
				Padding["bottom"] = value;
				Padding["left"] = value;
				Padding["right"] = value;
			}
			else 
			{
				Padding[side] = value;
			}
		}
		
		public virtual void _SetVisible( bool visible ) 
		{
			Visible = visible;
		}
		
		public virtual void _SetName( string text ) 
		{
			Name = text;
		}
		
		public virtual void _Select( int index ) 
		{
			_select = true;

			if (
				false == HasTypeString() ||
				IsDisposed()
			)
			{
				_select = false;
				return;
			}

			if (false == HasNodeEntries())
			{
				_select = false;
				return;
			}

			if (false == IndexWithinBounds(index))
			{
				_select = false;
				return;
			}

			if (
				null == Nodes[index] ||
				Nodes[index].IsQueuedForDeletion()
			)
			{
				_select = false;
				return;
			}

			if (IsInstanceValid(Nodes[index]) && Nodes[index] is Node childNode)
			{
				WorkingNode = childNode;
				_select = true;
				return;
			}

			_select = false;
			return;
		}
		
		public virtual void _SelectByName( string name ) 
		{
			foreach( Label label in Nodes ) 
			{
				if( label.Name == name ) 
				{
					WorkingNode = label;
					break;
				}
			}
		}
		
		public bool IsDisposed()
		{
			return disposed;
		}
		
		public T GetNode<T>()
		{
			if( null != WorkingNode ) 
			{
				// Single placement
				if( WorkingNode is T WorkingNodeFull) 
				{
					return WorkingNodeFull;
				}

				return default(T);
			}

			return default(T);
		}
		
		public virtual Node GetNode()
		{
			if( null != WorkingNode ) 
			{
				return WorkingNode;
			}

			return null;
		}
		
		private bool IndexWithinBounds( int index ) 
		{
			return 
				index < Nodes.Count &&
				index >= 0;
		}
		
		private bool HasNodeEntries()
		{
			return null != Nodes &&
				Nodes.Count != 0;
		}
		
		private bool HasTypeString()
		{
			return null != TypeString && "" != TypeString;
		}

		public override void _ExitTree()
		{
			disposed = true;
			
			base._ExitTree();
		}
	}
}
#endif