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
	/// <summary>
	/// A trait for managing lists of components within a container.
	/// </summary>
	[Tool]
	public partial class Listable : Trait.Base
	{
		private string _ComponentName = "";
		private int _Count = 0;
		private Action<int, GodotObject> _OnIterationAction;
		
		/// <summary>
		/// Default constructor for Listable.
		/// </summary>
		public Listable()
		{
			Name = "Listable";
			TypeString = GetType().ToString();

			_Margin = new()
			{
				{"left", 20},
				{"right", 20},
				{"top", 0},
				{"bottom", 25},
			};
		}
		
		/// <summary>
		/// Adds the currently chosen list to a specified container.
		/// </summary>
		/// <param name="Container">The container to add the list to.</param>
		public void AddToContainer( Node Container ) 
		{
			if( false == Dependencies.ContainsKey(TraitName + "_WorkingNode") ) 
			{
				return;
			}
			
			base._AddToContainer(Container, Dependencies[TraitName + "_WorkingNode"].As<VBoxContainer>());
		}
		
		/// <summary>
		/// Instantiate an instance of the trait.
		/// </summary>
		/// <returns>Returns the instantiated Listable.</returns>
		public Listable Instantiate()
		{
			base._Instantiate();
			
			VBoxContainer _WorkingNode = new()
			{
				Name = "Listable-Container",
				CustomMinimumSize = _CustomMinimumSize,
				Size = _Size,
			};
			
			Godot.Collections.Array _components = new();
			
			for( int i = 0; i < _Count; i++) 
			{
				string[] ComponentSingleArr = _ComponentName.Split(".");
				string ComponentSingleName = ComponentSingleArr[ComponentSingleArr.Length - 1];
				
				List<string> Components = new()
				{
					ComponentSingleName,
				};
				
				if (GlobalExplorer.GetInstance().Components.HasAll( Components.ToArray() )) 
				{
					GodotObject component = GlobalExplorer.GetInstance().Components.Single(_ComponentName, true);
					
					if( null != component && EditorPlugin.IsInstanceValid( component ) && component is BaseComponent baseComponent ) 
					{
						if( null != _OnIterationAction ) 
						{
							_OnIterationAction(i, component);
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

			Plugin.Singleton.TraitGlobal.AddInstance(Iteration, _WorkingNode, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.TraitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);
			
			Reset();
			Iteration += 1;
			Dependencies = new();
			
			return this;
		}
		
		/// <summary>
		/// Selects a placed list in the nodes array by index.
		/// </summary>
		/// <param name="index">The index of the list to select.</param>
		/// <param name="debug">Whether to output debug information.</param>
		/// <returns>Returns the selected Listable.</returns>
		public override Listable Select( int index, bool debug = false ) 
		{
			base._Select(index, debug);

			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode")) 
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.TraitGlobal.GetDependencies(index, TypeString, OwnerName);
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
		
		/// <summary>
		/// Selects a placed list in the nodes array by name.
		/// </summary>
		/// <param name="name">The name of the list to select.</param>
		/// <returns>Returns the selected Listable.</returns>
		public Listable SelectByName( string name ) 
		{
			base._SelectByName(name);

			return this;
		}
		
		/// <summary>
		/// Adds a callback to be run on each entry of the list.
		/// </summary>
		/// <param name="OnIteration">The callback to be executed.</param>
		/// <returns>Returns the modified Listable.</returns>
		public Listable Each( Action<int, GodotObject> OnIteration ) 
		{
			_OnIterationAction = OnIteration;
			
			return this;
		}
		
		/// <summary>
		/// Sets the name of the current list.
		/// </summary>
		/// <param name="text">The name to set.</param>
		/// <returns>Returns the modified Listable.</returns>
		public Listable SetName( string text ) 
		{
			Name = text;
			TraitName = text;

			return this;
		}
		
		/// <summary>
		/// Sets the dimensions of the list.
		/// </summary>
		/// <param name="width">The width to set.</param>
		/// <param name="height">The height to set.</param>
		/// <returns>Returns the modified Listable.</returns>
		public override Listable SetDimensions( int width, int height )
		{
			base.SetDimensions(width, height);

			return this;
		}
		
		/// <summary>
		/// Sets the total count for the list.
		/// </summary>
		/// <param name="_count">The count to set.</param>
		/// <returns>Returns the modified Listable.</returns>
		public Listable SetCount( int _count ) 
		{
			_Count = _count;

			return this;
		}
		
		/// <summary>
		/// Sets the component to be used.
		/// </summary>
		/// <param name="componentName">The name of the component to set.</param>
		/// <returns>Returns the modified Listable.</returns>
		public Listable SetComponent( string componentName ) 
		{
			_ComponentName = componentName;

			return this;
		}
		
		/// <summary>
		/// Sets the horizontal size flag, which controls the x axis.
		/// </summary>
		/// <param name="flag">The size flag to set.</param>
		/// <returns>Returns the modified Listable.</returns>
		public override Listable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}
		
		/// <summary>
		/// Sets the horizontal size flag, which controls the y axis.
		/// </summary>
		/// <param name="flag">The size flag to set.</param>
		/// <returns>Returns the modified Listable.</returns>
		public override Listable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}

		/// <summary>
		/// Clears the list at the specified index.
		/// </summary>
		/// <param name="index">The index of the list to clear.</param>
		/// <param name="debug">Whether to output debug information.</param>
		public override void Clear(int index = 0, bool debug = false)
		{
			if( null == TypeString || null == OwnerName || null == Plugin.Singleton || null == Plugin.Singleton.TraitGlobal ) 
			{
				Dependencies = new();
				return;
			}

			if( -1 != index ) 
			{
				if( false == Plugin.Singleton.TraitGlobal.HasInstance( index, TypeString, OwnerName ) ) 
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
				Godot.Collections.Dictionary<int, GodotObject> instances = Plugin.Singleton.TraitGlobal.AllInstances(TypeString, OwnerName);
	
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

		/// <summary>
		/// Resets the trait
		/// </summary>
		private void Reset()
		{
			_Count = 0;
			_ComponentName = "";
			_OnIterationAction = null;
		}
	}
}

#endif