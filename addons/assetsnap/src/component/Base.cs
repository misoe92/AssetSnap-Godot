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
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Godot;
using AssetSnap.Explorer;

namespace AssetSnap.Component
{
	/// <summary>
	/// Base class for managing components and instances.
	/// </summary>
	[Tool]
	public partial class Base : Node, ISerializationListener
	{
		/// <summary>
		/// Gets the singleton instance of the Base class.
		/// </summary>
		public static Base Singleton
		{
			get
			{
				if( null == _Instance ) 
				{
					_Instance = new();
					_Instance.Initialize();
				}

				return _Instance;
			}
		}
		
		[Export]
		public Godot.Collections.Array<string> ComponentTypes { get => _ComponentTypes; set => _ComponentTypes = value; }	
		[Export]
		public Godot.Collections.Array<GodotObject> DisposeQueue { get => _DisposeQueue; set => _DisposeQueue = value; }	
		[Export]
		public Godot.Collections.Dictionary<string, GodotObject> Components { get => _Components; set => _Components = value; }	
		[Export]
		public Godot.Collections.Array<GodotObject> Instances { get => _Instances; set => _Instances = value; }	
		
		private static Base _Instance = null;
		private Godot.Collections.Array<string> _ComponentTypes = new();
		private Godot.Collections.Array<GodotObject> _DisposeQueue = new();
		private Godot.Collections.Dictionary<string, GodotObject> _Components = new();
		private Godot.Collections.Array<GodotObject> _Instances = new();
		
		/// <summary>
		/// Initializes the Component BaseHandler by registering classes in the specified namespace.
		/// </summary>
		public void Initialize() 
		{
			string TargetNamespace = "AssetSnap.Front.Components";
			IEnumerable<Type> ClassesInNamespace = _GetClassesInNamespace(TargetNamespace);

			_EnterClassesInNamespace(ClassesInNamespace);
		}
		
		/// <summary>
		/// Removes the specified component.
		/// </summary>
		/// <param name="component">The component to remove.</param>
		public void Remove( BaseComponent component )
		{
			if( Instances.Contains(component) ) 
			{
				Instances.Remove(component);
			}
		}
		
				
		/// <summary>
		/// Removes a component by its type string.
		/// </summary>
		/// <param name="TypeString">The type string of the component to remove.</param>
		public void RemoveByTypeString( string TypeString ) 
		{
			if( Components.ContainsKey(TypeString) ) 
			{
				Components.Remove(TypeString);
			}
		}
		
		/// <summary>
		/// Disposes single components in the dispose queue.
		/// </summary>
		public void DisposeSingleComponents()
		{
			while( Components.Count > 0 ) 
			{
				foreach( (string name, GodotObject _object) in Components ) 
				{
					Components.Remove(name);
				
					if( EditorPlugin.IsInstanceValid( _object ) && _object is BaseComponent component )
					{
						component.QueueFree();
					}
				}
			}
		}
		
		/// <summary>
		/// Clears the specified instance from the list of instances.
		/// </summary>
		/// <param name="instance">The instance to clear.</param>
		/// <returns>void</returns>
		public void ClearInstance( BaseComponent instance ) 
		{
			if( Instances.Contains( instance ) ) 
			{
				Instances.Remove(instance);
			}
		}
		
		/// <summary>
		/// Clears the component of the specified type from the Components dictionary.
		/// </summary>
		/// <param name="debug">If true, debug messages will be printed.</param>
		/// <returns>void</returns>
		public void Clear<T>( bool debug = false)
		{
			string key = typeof(T).FullName.Replace("AssetSnap.Front.Components.", "").Split(".").Join("");
			
			if( false == Has(key) ) 
			{
				if( debug ) 
				{
					GD.PushError("Found nothing to clear: ", key);
				}

				return;
			}
			
			if( false == _Components.ContainsKey(key) ) 
			{
				if( debug ) 
				{
					GD.PushError("Found nothing to clear: ", key);
				}
				
				return;
			}
			
			if( false == EditorPlugin.IsInstanceValid(_Components[key]) ) 
			{
				_Components.Remove(key);
				if( debug ) 
				{
					GD.PushError("Cannot clear disposed object: ", key);
				}
				
				return;
			}
			
			BaseComponent component = _Components[key] as BaseComponent;
			
			_Components.Remove(key);
			_DisposeQueue.Add(component);
			component.Clear();
			component.GetParent().RemoveChild(component);
			
			// _Components.Add(key, Activator.CreateInstance<T>() as BaseComponent);
		}
		
		
		/// <summary>
		/// This method is called before serializing the object.
		/// </summary>
		public void OnBeforeSerialize()
		{
			//
		}
		
		/// <summary>
		/// This method is called after deserializing the object.
		/// </summary>
		public void OnAfterDeserialize()
		{
			_Instance = this;
		}
		
		/// <summary>
		/// Fetches an instance of a single object and returns it to the querier.
		/// </summary>
		/// <typeparam name="T">The type of object to fetch.</typeparam>
		/// <param name="unique">If true, ensures a unique instance is returned.</param>
		/// <returns>T - Can be various types depending on the component.</returns>
		public T Single<T>( bool unique = false )
		{
			int CurrentCount = _Instances.Count;
			string key = typeof(T).FullName.Replace("AssetSnap.Front.Components.", "").Split(".").Join("");

			if( false == Has(key) ) 
			{
				return default(T);
			}
			
			if( false == _Components.ContainsKey(key) && false == unique ) 
			{
				object instance = Activator.CreateInstance(typeof(T));
				if( _IsComponent(instance) && _ShouldInclude(instance) ) 
				{
					_EnterComponent(instance, key);
				}
				else  
				{
					if( instance is Node _node && EditorPlugin.IsInstanceValid(_node)) 
					{
						if( null != _node.GetParent() && EditorPlugin.IsInstanceValid( _node.GetParent() ))
						{
							_node.GetParent().RemoveChild(_node);
						}
						
						_node.Free();
					}
				} 
			}

			
			if( unique ) 
			{
				BaseComponent component = Activator.CreateInstance<T>() as BaseComponent;
				component.Name = component.Name + CurrentCount;
				component.TypeString = typeof(T).ToString();
				if( null != component.GetParent() && component.GetParent() == Plugin.Singleton.GetInternalContainer() ) 
				{
					Plugin.Singleton
						.GetInternalContainer()
						.RemoveChild(component);
				}
				
				_Instances.Add(component);
				if( component is T instanceTypedComponent ) 
				{
					return instanceTypedComponent;
				}
			}
			else 
			{
				if( _Components.ContainsKey(key) && _Components[key] is BaseComponent component) 
				{
					if (component is T typedComponent)
					{
						if(
							null != component.GetParent() &&
							component.GetParent() == Plugin.Singleton.GetInternalContainer()
						) 
						{
							Plugin.Singleton
								.GetInternalContainer()
								.RemoveChild(component);
						}
						
						return typedComponent;
					}
					else
					{
						// Handle the case where conversion is not possible
						return default(T);
					}
				}
			} 
			return default(T);
		}
		
		/// <summary>
		/// Fetches an instance of a single object and returns it to the querier.
		/// </summary>
		/// <param name="key">The key representing the type of object to fetch.</param>
		/// <param name="unique">If true, ensures a unique instance is returned.</param>
		/// <returns>BaseComponent - Can be various types depending on the component.</returns>
		public BaseComponent Single( string key, bool unique = false )
		{
			string[] keyArr = key.Split(".");
			string keyName = keyArr[keyArr.Length - 1];
			
			if( false == Has(keyName) ) 
			{
				return null;
			}
			
			if( false == _Components.ContainsKey(keyName.Split(".").Join("")) && false == unique) 
			{
				object instance = Activator.CreateInstance(Type.GetType(keyName));
				if( _IsComponent(instance) && _ShouldInclude(instance) ) 
				{
					_EnterComponent(instance, key);
				}
				else  
				{
					if( instance is Node _node && EditorPlugin.IsInstanceValid(_node)) 
					{
						if( null != _node.GetParent() && EditorPlugin.IsInstanceValid( _node.GetParent() ))
						{
							_node.GetParent().RemoveChild(_node);
						}
						
						_node.QueueFree();
					}
				} 
			}
			
			if( unique ) 
			{
				Type classType = Type.GetType(key);
				BaseComponent component = Activator.CreateInstance(classType) as BaseComponent;
				component.Name = component.Name + _Instances.Count;
				component.TypeString = classType.ToString();
				
				_Instances.Add(component);
				return component;
			} 
			else 
			{
				if( EditorPlugin.IsInstanceValid( _Components[keyName.Split(".").Join("")] ) && _Components[keyName.Split(".").Join("")] is BaseComponent component ) 
				{
					if (component is BaseComponent typedComponent)
					{
						return typedComponent;
					}
					else
					{
						// Handle the case where conversion is not possible
						return null;
					}
				}
				
				return null;
			}
		}

		/// <summary>
		/// Checks if the given key has a component.
		/// </summary>
		/// <param name="key">The key to check.</param>
		/// <returns>True if the component exists, otherwise false.</returns>
		public bool Has( string key )
		{
			if( false == _ComponentTypes.Contains(key.Split(".").Join("")) ) 
			{
				return false;
			}
			
			return true;
		}
		
		/// <summary>
		/// Removes the specified component from the internal container and then adds it again.
		/// </summary>
		/// <param name="key">The key of the component to end.</param>
		public void End( string key ) 
		{
			if( false == Has( key ) ) 
			{
				return;
			}
			
			ExplorerUtils.Get()._Plugin
				.GetInternalContainer()
				.RemoveChild(_Components[key.Split(".").Join("")] as BaseComponent);
				
			_Components[key.Split(".").Join("")] = new();
			
			ExplorerUtils.Get()._Plugin
				.GetInternalContainer()
				.AddChild(_Components[key.Split(".").Join("")] as BaseComponent);
		}
		
		/// <summary>
		/// Gets the count of instances.
		/// </summary>
		/// <returns>The count of instances.</returns>
		public int InstancesCount()
		{
			return Instances.Count;	
		}
		
		/// <summary>
		/// Gets the count of components.
		/// </summary>
		/// <returns>The count of components.</returns>
		public int Count()
		{
			return Components.Count;
		}
		
		/// <summary>
		/// Gets the keys of components.
		/// </summary>
		/// <returns>The keys of components.</returns>
		public System.Collections.Generic.ICollection<string> Keys()
		{
			return Components.Keys;
		}
			
		/// <summary>
		/// Checks if all the given keys are present in the components array.
		/// </summary>
		/// <param name="keys">An array of keys to check.</param>
		/// <returns>True if all keys are present, otherwise false.</returns>
		public bool HasAll( string[] keys )
		{
			if( _ComponentTypes == null || _ComponentTypes.Count == 0 ) 
			{
				GD.PushError("No components was found");
				return false;
			}

			for( int i = 0; i < keys.Length; i++)
			{
				string key = keys[i];
				if( false == _ComponentTypes.Contains(key.Split(".").Join("")) ) 
				{
					return false;
				}
			}
			
			return true;
		}

		/// <summary>
		/// Fetches classes inside a given namespace.
		/// </summary>
		/// <param name="TargetNamespace">The namespace from which to extract classes.</param>
		/// <returns>An enumerable collection of types.</returns>
		private IEnumerable<Type> _GetClassesInNamespace(string TargetNamespace)
		{
			// Get all types in the current assembly
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();

			// Filter types by namespace recursively
			IEnumerable<Type> classesInNamespace = types
				.Where(type => type.Namespace != null && 
							(type.Namespace == TargetNamespace || 
								type.Namespace.StartsWith(TargetNamespace + ".")) && 
							type.IsClass);

			return classesInNamespace;
		}
		
		/// <summary>
		/// Enters the classes in the specified namespace.
		/// </summary>
		/// <param name="ClassesInNamespace">The classes to enter.</param>
		private void _EnterClassesInNamespace(IEnumerable<Type> ClassesInNamespace)
		{
			foreach (Type classType in ClassesInNamespace)
			{
				string TypeName = classType.FullName;
				string TypeKey = TypeName.Replace("AssetSnap.Front.Components.", "").Split(".").Join("");

				// Create an instance of the class
				_ComponentTypes.Add(TypeKey);
			}
		}
		
		/// <summary>
		/// Enters the component into the tree and adds it to
		/// a list for easy control later on.
		/// </summary>
		/// <param name="instance">The instance of the component to enter.</param>
		/// <param name="TypeName">The type name of the component.</param>
		/// <returns>void</returns>
		private void _EnterComponent( object instance, string TypeName)
		{
			BaseComponent _Component = ObjectToBaseComponent(instance);
			
			if( _Component == null ) 
			{
				GD.PushWarning("Component was not entered into the array");
				return;
			}
				
			_Component.TypeString = TypeName;
			_Component.Enter();
			_Components.Add(TypeName, _Component);
		}
		
		/// <summary>
		/// Checks if the given object is a base component.
		/// </summary>
		/// <param name="instance">The object to check.</param>
		/// <returns>bool</returns>
		private bool _IsComponent( object instance )
		{
			return instance is BaseComponent;
		}
		
		/// <summary>
		/// Determines whether the given object should be included.
		/// </summary>
		/// <param name="instance">The object to check.</param>
		/// <returns>bool</returns>
		private bool _ShouldInclude( object instance ) 
		{
			if( instance is BaseComponent _component && _component.Include is bool BoolVal ) 
			{
				return BoolVal;
			}
			
			return false;
		}
		
		/// <summary>
		/// Converts an object to a BaseComponent if possible.
		/// If it fails, it will return null.
		/// </summary>
		/// <param name="instance">The object to convert.</param>
		/// <returns>BaseComponent</returns>
		private BaseComponent ObjectToBaseComponent( object instance ) 
		{
			if( instance is BaseComponent AsComponent ) 
			{
				return AsComponent;
			}
			
			return null;
		}
		
		
	}
}

#endif