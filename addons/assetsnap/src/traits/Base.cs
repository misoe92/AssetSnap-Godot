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

namespace AssetSnap.Trait
{
	using Godot;
	
	[Tool]
	public partial class Base : Node
	{
		/*
		** Public
		*/
		public int Iteration = 0;
		public string TraitName = "";
		[Export]
		public bool Build = false;
		
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
			
		[Export]
		public string TypeString;
		[Export]
		public string OwnerName;

		protected Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		protected Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		[Export]
		public Godot.Collections.Array<GodotObject> Nodes;
		protected Vector2 CustomMinimumSize = Vector2.Zero;
		protected Vector2 Size = Vector2.Zero;
		public int TotalCount = 0;
		protected bool _select = true;
		public bool disposed = false;
		protected bool Visible = true;
	
		public Godot.Collections.Dictionary<string, Variant> Dependencies = new();

		public Base()
		{
			Name = "TraitBase";
			Nodes = new();
			
			// block unloading with a strong handle
			var handle = System.Runtime.InteropServices.GCHandle.Alloc(this);

			// register cleanup code to prevent unloading issues
			System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(System.Reflection.Assembly.GetExecutingAssembly()).Unloading += alc =>
			{
				Build = true;
				handle.Free();
			};
		}

		/*
		** Checks if trait name is set
		** and sets the type string
		**
		** @param string typeString
		** @return void
		*/
		public void _Instantiate()
		{
			if( Name == "" ) 
			{
				GD.PushWarning("No name was set for the traitable: ", TypeString);
			}
			
			if( OwnerName == "" ) 
			{
				GD.PushWarning("No owner was set for the traitable: ", TypeString);
			}

			TotalCount += 1;
			Dependencies = new();
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
				GD.PushError("No node selected: ", TraitName, TypeString );
				return;
			}
			
			if( null == Container ) 
			{
				GD.PushError("Provided container is invalid: ", TraitName, TypeString );
				// throw new Exception("");
				return;
			}

			if( Node == Container ) 
			{
				GD.PushError("Provided container is the same as this object: ", TraitName, TypeString );
				return;
			}
			
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
		
		public virtual Trait.Base Select( int index, bool debug = false ) 
		{
			_Select(index, debug);
			return this;
		}

		/*
		** Selects an existing instance of
		** the trait
		**
		** @param int index
		** @return void
		*/
		public virtual void _Select( int index, bool debug = false ) 
		{
			Dependencies = new();
			_select = true;

			if( null == Plugin.Singleton ) 
			{
				if( debug ) 
				{
					GD.PushError("No PluginController found");
				}
				_select = false;
				return;
			}
			
			if( null == Plugin.Singleton.traitGlobal ) 
			{
				if (debug)
				{
					GD.PushError("No global trait found");
				}
				_select = false;
				return;
			}
			
			if( null == TypeString ) 
			{
				if (debug)
				{
					GD.PushError("No typestring is currently set");
				}
				_select = false;
			}
			
			if( null == OwnerName ) 
			{
				if (debug)
				{
					GD.PushError("No ownername is currently set");
				}
				_select = false;
			}
			
			if( null == TypeString || null == OwnerName) 
			{
				return;
			} 

			string NameResult = Plugin.Singleton.traitGlobal.GetName(index, TypeString, OwnerName);
			TraitName = NameResult;
			

			if (false == HasNodeEntries())	
			{
				_select = false;
				if (debug)
				{
					GD.PushError("No Entries for owner: ", OwnerName, " - ", NameResult);
				}
				return;
			}

			if (false == ContainsIndex(index))
			{
				_select = false;
				if (debug)
				{
					GD.PushError("No contain index");
				}
				
				return;
			}

			Node traitIndex = Plugin.Singleton.traitGlobal.GetInstance(index, TypeString, OwnerName, debug);
			if (EditorPlugin.IsInstanceValid(traitIndex) && traitIndex is Control childNode)
			{
				Dependencies[TraitName + "_WorkingNode"] = childNode;

				if( debug ) 
				{
					GD.Print("Node found");
				}
				
				_select = true;
				return;
			}

			if (debug)
			{
				GD.PushError("Failed: ", TraitName, "::", traitIndex, "::", OwnerName, "::", TypeString, "->", EditorPlugin.IsInstanceValid(traitIndex),"||", traitIndex is Control);
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
					Dependencies[TraitName + "_WorkingNode"] = label;
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
			TraitName = text;
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
		** Sets the minimum size of the container
		**
		** @param int width
		** @param int height
		** @return Containerable
		*/
		public virtual Base SetMinimumDimension( int width, int height )
		{
			CustomMinimumSize = new Vector2( width, height);

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
			if( Dependencies.ContainsKey(TraitName + "_WorkingNode") ) 
			{
				// Single placement
				if( Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() is T WorkingNodeFull) 
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
		public virtual Node GetNode( bool debug = false )
		{
			if( null != Dependencies && Dependencies.ContainsKey(TraitName + "_WorkingNode") ) 
			{
				return Dependencies[TraitName + "_WorkingNode"].As<GodotObject>() as Node;
			}
			
			if( debug ) 
			{
				if( null == Dependencies ) 
				{
					GD.PushError("Dependencies not set");
				}
				
				if( null != Dependencies && false == Dependencies.ContainsKey(TraitName + "_WorkingNode") ) 
				{
					GD.PushError("No active set of dependencies was found");
				}
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
		public virtual bool IsValid( bool debug = false )
		{
			if( debug ) 
			{
				GD.PushError("DEBUG DATA FOR TRAIT");
			}
			
			if( TotalCount == 0 ) 
			{
				if( debug ) 
				{
					GD.PushError("No items found");
				}
				
				return false;
			}
			
			if( _select == false ) 
			{
				if( debug ) 
				{
					GD.PushError("Failed to select item");
				}

				return false;
			}
				
			if( IsDisposed() ) 
			{
				if( debug ) 
				{
					GD.PushError("The object has been disposed");
				}
				
				return false;
			}
			
			if( Build ) 
			{
				if( debug ) 
				{
					GD.PushError("Object is currently building");
				}
				
				return false;
			}
				
			if( null == Plugin.Singleton ) 
			{
				if( debug ) 
				{
					GD.PushError("Plugin is not available");
				}

				return false;
			}

			if( null == Plugin.Singleton || null == Plugin.Singleton.traitGlobal ) 
			{
				if( debug ) 
				{
					GD.PushError("Trait Global is not available");
				}
				
				return false;
			}
				
			if( null == OwnerName ) 
			{
				if( debug ) 
				{
					GD.PushError("Owner name was not found");
				}
				
				return false;
			}

			if( null == TypeString ) 
			{
				if( debug ) 
				{
					GD.PushError("TypeString was not found");
				}
				
				return false;
			}
			
			if( 0 == Dependencies.Count ) 
			{
				if( debug ) 
				{
					GD.PushError("Dependencies was not set");
				}
				
				return false;
			}

			if( debug ) 
			{
				GD.PushError("Seems to return valid");
			}
			
			return true;
		}
		
		/*
		** Checks if the trait has been disposed
		**
		** @return bool
		*/
		public bool IsDisposed()
		{
			return false;
		}
		
		/*
		** Checks if the nodes array
		** contains an specified index
		**
		** @return bool
		*/
		public bool ContainsIndex( int index ) 
		{
			return Plugin.Singleton.traitGlobal.CountOwner( OwnerName ) >= index;
		}
		
		/*
		** Checks if the nodes array
		** is empty or null
		**
		** @return bool
		*/
		private bool HasNodeEntries()
		{
			return Plugin.Singleton.traitGlobal.Count() != 0 && Plugin.Singleton.traitGlobal.CountOwner( OwnerName ) != 0;
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
		
		public virtual void Clear(int index = 0, bool debug = false)
		{
			if( null == TypeString || null == OwnerName || null == Plugin.Singleton || null == Plugin.Singleton.traitGlobal ) 
			{
				Dependencies = new();
				return;
			}

			Godot.Collections.Dictionary<int, Node> instances = Plugin.Singleton.traitGlobal.AllInstances(TypeString, OwnerName);
			
			if( null == instances || instances.Count == 0 ) 
			{
				Dependencies = new();
				return;
			}

			// GD.Print(instances, instances.Count);
			foreach( (int idx, Node node) in instances )
			{
				if( debug ) 
				{
					GD.Print("Index::", idx, TypeString, OwnerName);
				}

				Plugin.Singleton.traitGlobal.RemoveInstance(idx, TypeString, OwnerName );
			}

			Dependencies = new();
		}
		
		public virtual int Count()
		{
			if( null == TypeString || null == OwnerName || null == Plugin.Singleton || null == Plugin.Singleton.traitGlobal ) 
			{
				return 0;
			}
			
			Godot.Collections.Dictionary<int, Node> instances = Plugin.Singleton.traitGlobal.AllInstances(TypeString, OwnerName);
			
			if( null == instances || instances.Count == 0 ) 
			{
				return 0;
			}

			return instances.Count;
		}
			
		public void OnBeforeSerialize()
		{
			Build = true;
		}
		
		public void OnAfterDeserialize()
		{
			Build = false;
		}

		public override void _ExitTree()
		{
			base._ExitTree();
		}
	}
}