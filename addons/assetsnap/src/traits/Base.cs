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
	public partial class Base : Node
	{
		/*
		** Public
		*/
		
		/*
		** protected
		*/
		protected Godot.Collections.Dictionary<string, int> Margin = new()
		{
			{"left", 0},
			{"right", 0},
			{"top", 0},
			{"bottom", 0},
		};
		
		protected Godot.Collections.Dictionary<string, int> Padding = new()
		{
			{"left", 0},
			{"right", 0},
			{"top", 0},
			{"bottom", 0},
		};
		protected Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		protected Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		protected Godot.Collections.Array<GodotObject> Nodes = new();
		protected Vector2 CustomMinimumSize = Vector2.Zero;
		protected Vector2 Size = Vector2.Zero;
		protected bool _select = true;
		public bool disposed = false;
		protected bool Visible = true;
		protected Node WorkingNode;
		protected string TypeString = "";

		/*
		** Checks if trait name is set
		** and sets the type string
		**
		** @param string typeString
		** @return void
		*/
		public void _Instantiate( string typeString )
		{
			if( Name == "" ) 
			{
				GD.PushWarning("No name was set for the traitable: ", typeString);
			}

			TypeString = typeString;
		}
		
		/*
		** Adds the trait to a given container
		**
		** @param Node Container
		** @param Node Node
		** @param int? index
		** @return void
		*/
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
		
			
		/*
		** Selects an existing instance of
		** the trait
		**
		** @param int index
		** @return void
		*/
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

			if (false == ContainsIndex(index))
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
		
		/*
		** Selects an existing instance of
		** the trait based on name
		**
		** @param string name
		** @return void
		*/
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
		
		/*
		** Setter methods
		*/
		
		/*
		** Sets margin of the trait
		**
		** @param int value
		** @param string side
		** @return void
		*/
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
		
		/*
		** Sets padding of the trait
		**
		** @param int value
		** @param string side
		** @return void
		*/
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
		
		/*
		** Sets visible state of the trait
		**
		** @param bool visible
		** @return void
		*/
		public virtual void _SetVisible( bool visible ) 
		{
			Visible = visible;
		}
		
		/*
		** Sets the name of the trait
		**
		** @param string text
		** @return void
		*/
		public virtual void _SetName( string text ) 
		{
			Name = text;
		}
		
		/*
		** Sets the size of the container
		**
		** @param int width
		** @param int height
		** @return Containerable
		*/
		public virtual Base SetDimensions( int width, int height )
		{
			CustomMinimumSize = new Vector2( width, height);
			Size = new Vector2( width, height);

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Containerable
		*/
		public virtual Base SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;
			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Containerable
		*/
		public virtual Base SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;
			return this;
		}
	
		
		/*
		** Getter Methods
		*/
		
		/*
		** Gets node and type casts it
		**
		** return T
		*/
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
		
		/*
		** Gets node and returns it as Node
		**
		** return Node
		*/
		public virtual Node GetNode()
		{
			if( null != WorkingNode ) 
			{
				return WorkingNode;
			}

			return null;
		}
		
		/*
		** Boolean
		*/
		
		/*
		** Checks if the trait select was a succes
		**
		** @return bool
		*/
		public virtual bool IsValid()
		{
			return _select != false && IsDisposed() != true;
		}
		
		/*
		** Checks if the trait has been disposed
		**
		** @return bool
		*/
		public bool IsDisposed()
		{
			return disposed;
		}
		
		/*
		** Checks if the nodes array
		** contains an specified index
		**
		** @return bool
		*/
		public bool ContainsIndex( int index ) 
		{
			return Nodes.Count >= index;
		}
		
		/*
		** Checks if the nodes array
		** is empty or null
		**
		** @return bool
		*/
		private bool HasNodeEntries()
		{
			return null != Nodes &&
				Nodes.Count != 0;
		}
		
		/*
		** Checks if a type string is present
		**
		** @return bool
		*/
		private bool HasTypeString()
		{
			return null != TypeString && "" != TypeString;
		}

		/*
		** Cleanup
		**
		** @return void
		*/
		public override void _ExitTree()
		{
			disposed = true;
			
			base._ExitTree();
		}
	}
}
#endif