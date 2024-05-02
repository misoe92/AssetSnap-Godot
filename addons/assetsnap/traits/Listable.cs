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
using AssetSnap.Explorer;
using Godot;

namespace AssetSnap.Component
{
	[Tool]
	public partial class Listable : Trait.Base
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
		private Action<int, GodotObject> OnIterationAction;
		private string ComponentName = "";
		private int count = 0;
		
		/*
		** Public methods
		*/
		public Listable()
		{
			Name = "Listable";
			TypeString = GetType().ToString();
		}
		
		/*
		** Instantiate an instance of the trait
		**
		** @return Listable
		*/
		public Listable Instantiate()
		{
			base._Instantiate();
			
			VBoxContainer _WorkingNode = new()
			{
				Name = "Listable-Container",
				CustomMinimumSize = CustomMinimumSize,
				Size = Size,
			};
			
			Godot.Collections.Array _components = new();
			
			for( int i = 0; i < count; i++) 
			{
				string[] ComponentSingleArr = ComponentName.Split(".");
				string ComponentSingleName = ComponentSingleArr[ComponentSingleArr.Length - 1];
				
				List<string> Components = new()
				{
					ComponentSingleName,
				};
				
				if (GlobalExplorer.GetInstance().Components.HasAll( Components.ToArray() )) 
				{
					GodotObject component = GlobalExplorer.GetInstance().Components.Single(ComponentName, true);
					
					if( null != component && EditorPlugin.IsInstanceValid( component ) && component is BaseComponent baseComponent ) 
					{
						if( null != OnIterationAction ) 
						{
							OnIterationAction(i, component);
						}
						else 
						{
							GD.Print("Iteration action invalid");
						}
						
						_components.Add(component);
						_WorkingNode.AddChild(baseComponent);
					}
				}
			}
			
			Dependencies[TraitName + "_Components"] = _components;
			Dependencies[TraitName + "_WorkingNode"] = _WorkingNode;

			Plugin.Singleton.traitGlobal.AddInstance(Iteration, _WorkingNode, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.traitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);
			
			Reset();
			Iteration += 1;
			Dependencies = new();
			
			return this;
		}
		
		/*
		** Selects an placed list in the
		** nodes array by index
		**
		** @param int index
		** @return Listable
		*/
		public override Listable Select( int index, bool debug = false ) 
		{
			base._Select(index, debug);

			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode")) 
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.traitGlobal.GetDependencies(index, TypeString, OwnerName);
				Dependencies = dependencies;
			}
			else
			{
				if( debug ) 
				{
					GD.PushError("No dependencies found for panel", index, TraitName);
				}
			}
 
			return this;
		}	
		
		/*
		** Selects an placed list in the
		** nodes array by name
		**
		** @param string name
		** @return Listable
		*/
		public Listable SelectByName( string name ) 
		{
			base._SelectByName(name);

			return this;
		}
		
		/*
		** Adds a callback to be ran on each entry of
		** the list
		**
		** @param Action<int, BaseComponent> OnIteration
		** @return Listable
		*/
		public Listable Each( Action<int, GodotObject> OnIteration ) 
		{
			OnIterationAction = OnIteration;
			
			return this;
		}
		
		/*
		** Adds the currently chosen list
		** to a specified container
		**
		** @param Node Container
		** @return void
		*/
		public void AddToContainer( Node Container ) 
		{
			if( false == Dependencies.ContainsKey(TraitName + "_WorkingNode") ) 
			{
				return;
			}
			
			base._AddToContainer(Container, Dependencies[TraitName + "_WorkingNode"].As<VBoxContainer>());
		}
		
		/*
		** Setter Methods
		*/
		
		/*
		** Sets the name of the current list
		**
		** @param string text
		** @return Listable
		*/
		public Listable SetName( string text ) 
		{
			Name = text;
			TraitName = text;

			return this;
		}
		
		/*
		** Sets the dimensions of the list
		**
		** @param int width
		** @param int height
		** @return Listable
		*/
		public override Listable SetDimensions( int width, int height )
		{
			base.SetDimensions(width, height);

			return this;
		}
		
		/*
		** Sets the total count
		** for the list
		**
		** @param int count
		** @return Listable
		*/
		public Listable SetCount( int _count ) 
		{
			count = _count;

			return this;
		}
		
		/*
		** Sets the component
		** to be used
		**
		** @param string componentName
		** @return Listable
		*/
		public Listable SetComponent( string componentName ) 
		{
			ComponentName = componentName;

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Listable
		*/
		public override Listable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Listable
		*/
		public override Listable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}

		public override void Clear(int index = 0, bool debug = false)
		{
			if( null == TypeString || null == OwnerName || null == Plugin.Singleton || null == Plugin.Singleton.traitGlobal ) 
			{
				Dependencies = new();
				return;
			}

			if( -1 != index ) 
			{
				if( false == Plugin.Singleton.traitGlobal.HasInstance( index, TypeString, OwnerName ) ) 
				{
					if( debug ) 
					{
						GD.Print("Instance not found, or not valid");
					}
					
					return;
				}
				
				if( debug ) 
				{
					GD.Print("Index::", index, TypeString, OwnerName);
				}
				Select(index);
				Godot.Collections.Array<BaseComponent> compArray = Dependencies[TraitName + "_Components"].AsGodotArray<BaseComponent>();
				
				foreach( BaseComponent component in compArray ) 
				{
					ExplorerUtils.Get().Components.Remove(component);
				}
			}
			else 
			{
				Godot.Collections.Dictionary<int, GodotObject> instances = Plugin.Singleton.traitGlobal.AllInstances(TypeString, OwnerName);
	
				if( null == instances || instances.Count == 0 ) 
				{
					Dependencies = new();
					return;
				}
	
				foreach( (int idx, GodotObject node) in instances )
				{
					if( debug ) 
					{
						GD.Print("Index::", idx, TypeString, OwnerName);
					}
					Select(idx);
					Godot.Collections.Array<BaseComponent> compArray = Dependencies[TraitName + "_Components"].AsGodotArray<BaseComponent>();
					
					foreach( BaseComponent component in compArray ) 
					{
						ExplorerUtils.Get().Components.Remove(component);
					}
				}
			}
			
			base.Clear(index, debug);
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
		private void Reset()
		{
			count = 0;
			ComponentName = "";
			OnIterationAction = null;
		}
	}
}
#endif