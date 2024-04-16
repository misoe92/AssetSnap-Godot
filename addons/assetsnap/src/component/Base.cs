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
namespace AssetSnap.Component
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Collections.Generic;
	using Godot;
	using AssetSnap.Explorer;

	[Tool]
	public partial class Base
	{
		private static Godot.Collections.Array<string> _ComponentTypes = new();
		private static Godot.Collections.Dictionary<string, GodotObject> _Components = new();
		private static Godot.Collections.Array<GodotObject> _Instances = new();
		public static Godot.Collections.Dictionary<string, GodotObject> Components { get => _Components; set => _Components = value; }	
		public static Godot.Collections.Array<GodotObject> Instances { get => _Instances; set => _Instances = value; }	
		
		private static Base _Instance = null;
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
		
		/*
		** Initialization of Component BaseHandler
		*/ 
		public void Initialize() 
		{
			string TargetNamespace = "AssetSnap.Front.Components";
			IEnumerable<Type> ClassesInNamespace = GetClassesInNamespace(TargetNamespace);

			EnterClassesInNamespace(ClassesInNamespace);
		}
		
		public int InstancesCount()
		{
			return Instances.Count;	
		}
		
		public int Count()
		{
			return Components.Count;
		}
		
		public System.Collections.Generic.ICollection<string> Keys()
		{
			return Components.Keys;
		}
		
		public void Remove( BaseComponent component )
		{
			if( Instances.Contains(component) ) 
			{
				Instances.Remove(component);
			}
		}
		
		public void RemoveByTypeString( string TypeString ) 
		{
			if( Components.ContainsKey(TypeString) ) 
			{
				Components.Remove(TypeString);
			}
		}
		
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
		
		/*
		** Fetches classes inside a given namespace
		**
		** @param string TargetNamespace - The namespace we wish to extract classes from
		** @return IEnumerable<Type>
		*/
		private IEnumerable<Type> GetClassesInNamespace(string TargetNamespace)
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
		
		/*
		** This enteres the components into the tree if it's valid and
		** initializes elements that are needed for other
		** functionalities
		**
		** @param IEnumerable<Type> ClassesInNamespace
		** @return void
		*/
		private void EnterClassesInNamespace(IEnumerable<Type> ClassesInNamespace)
		{
			foreach (Type classType in ClassesInNamespace)
			{
				string TypeName = classType.FullName;
				string TypeKey = TypeName.Replace("AssetSnap.Front.Components.", "").Split(".").Join("");

				// Create an instance of the class
				_ComponentTypes.Add(TypeKey);
			}
		}
		
		/*
		** Enters the component into the tree and adds it to
		** a list for easy control later on.
		**
		** @param object instance
		** @param string TypeName 
		** @return void 
		*/
		private void EnterComponent( object instance, string TypeName)
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
		
		/*
		** Checks if given object is a base component
		**
		** @param object instance
		** @return bool  
		*/
		private bool IsComponent( object instance )
		{
			return instance is BaseComponent;
		}
		
		private bool ShouldInclude( object instance ) 
		{
			if( instance is BaseComponent _component && _component._include is bool BoolVal ) 
			{
				return BoolVal;
			}
			
			return false;
		}
		
		/*
		** Converts an object to BaseComponent if possible
		** If it fails it will return null
		**
		** @param object instance
		** @return BaseComponent 
		*/
		private BaseComponent ObjectToBaseComponent( object instance ) 
		{
			if( instance is BaseComponent AsComponent ) 
			{
				return AsComponent;
			}
			
			return null;
		}
		
		public void Clear<T>()
		{
			string key = typeof(T).FullName.Replace("AssetSnap.Front.Components.", "").Split(".").Join("");
			
			if( false == Has(key) ) 
			{
				return;
			}
			
			BaseComponent component = _Components[key] as BaseComponent;

			component.Clear();
			
			component.GetParent().RemoveChild(component);
			component.QueueFree();
			
			_Components.Remove(key);
			_Components.Add(key, Activator.CreateInstance<T>() as BaseComponent);
		}
		
		/*
		** Fetches an instance of a single object
		** and returns it to the querier.
		**
		** @param bool unique[false]
		** @return T - Can be various of types depending on the component
		*/
		public T Single<T>( bool unique = false )
		{
			string key = typeof(T).FullName.Replace("AssetSnap.Front.Components.", "").Split(".").Join("");

			if( false == Has(key) ) 
			{
				return default(T);
			}
			
			if( false == _Components.ContainsKey(key) && false == unique ) 
			{
				object instance = Activator.CreateInstance(typeof(T));
				if( IsComponent(instance) && ShouldInclude(instance) ) 
				{
					EnterComponent(instance, key);
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
				BaseComponent component = Activator.CreateInstance<T>() as BaseComponent;
				component.Name = component.Name + _Instances.Count;
				if( null != component.GetParent() && component.GetParent() == Plugin.Singleton
				.GetInternalContainer() ) 
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
				BaseComponent component = _Components[key] as BaseComponent;
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
			return default(T);
		}
		
		/*
		** Fetches an instance of a single object
		** and returns it to the querier.
		**
		** @param bool unique[false]
		** @return T - Can be various of types depending on the component
		*/
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
				if( IsComponent(instance) && ShouldInclude(instance) ) 
				{
					EnterComponent(instance, key);
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
				
				_Instances.Add(component);
				return component;
			} 
			else 
			{
				BaseComponent component = _Components[keyName.Split(".").Join("")] as BaseComponent;
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
		}

		
		/*
		** Checks if the given key has a component
		**
		** @param string key
		** @return bool
		*/
		public bool Has( string key )
		{
			if( false == _ComponentTypes.Contains(key.Split(".").Join("")) ) 
			{
				return false;
			}
			
			return true;
		}
		
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
		
		/*
		** Checks if a given array of keys
		** is to be found in the components 
		** array
		**
		** @param string[] keys
		** @return bool
		*/
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
	}
}
#endif