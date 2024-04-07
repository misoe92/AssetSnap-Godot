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
		private static Godot.Collections.Dictionary<string, BaseComponent> _Components = new();
		private static Godot.Collections.Array<BaseComponent> _Instances = new();
		
		[Export]
		public static Godot.Collections.Dictionary<string, BaseComponent> Components { get => _Components; set => _Components = value; }	
		
		[Export]
		public static Godot.Collections.Array<BaseComponent> Instances { get => _Instances; set => _Instances = value; }	
		
		/*
		** Initialization of Component BaseHandler
		*/ 
		public void Initialize()
		{
			string TargetNamespace = "AssetSnap.Front.Components";
			IEnumerable<Type> ClassesInNamespace = GetClassesInNamespace(TargetNamespace);

			EnterClassesInNamespace(ClassesInNamespace);
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

			// Filter types by namespace
			System.Collections.Generic.IEnumerable<System.Type> ClassesInNamespace = types
				.Where(type => type.Namespace == TargetNamespace && type.IsClass);

			return ClassesInNamespace;
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
				string TypeName = classType.Name;
				
				// Create an instance of the class
				object instance = Activator.CreateInstance(classType);
				if( IsComponent(instance) && ShouldInclude(instance) ) 
				{
					EnterComponent(instance, TypeName);
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
			
			ExplorerUtils.Get()._Plugin
				.GetInternalContainer()
				.AddChild(_Component, true);
				
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
		
		/*
		** Fetches an instance of a single object
		** and returns it to the querier.
		**
		** @param bool unique[false]
		** @return T - Can be various of types depending on the component
		*/
		public T Single<T>( bool unique = false )
		{
			string key = typeof(T).Name;
			
			if( false == Has(key) ) 
			{
				return default(T);
			}

			BaseComponent component = _Components[key];
			
			if( unique ) 
			{
				component = Activator.CreateInstance<T>() as BaseComponent;
				
				ExplorerUtils.Get()._Plugin
					.GetInternalContainer()
					.AddChild(component, true);
 
				_Instances.Add(component);
			}

			if (component is T typedComponent)
			{
				return typedComponent;
			}
			else
			{
				// Handle the case where conversion is not possible
				return default(T);
			}
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

			BaseComponent component = _Components[keyName];
			
			if( unique ) 
			{
				Type classType = Type.GetType(key);
				component = Activator.CreateInstance(classType) as BaseComponent;
				
				_Instances.Add(component);
			}

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
		
		/*
		** Checks if the given key has a component
		**
		** @param string key
		** @return bool
		*/
		public bool Has( string key )
		{
			if( false == _Components.ContainsKey(key) ) 
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
				.RemoveChild(_Components[key]);
				
			_Components[key] = new();
			
			ExplorerUtils.Get()._Plugin
				.GetInternalContainer()
				.AddChild(_Components[key]);
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
			if( _Components == null || _Components.Count == 0 ) 
			{
				GD.PushError("No components was found");
				return false;
			}
			
			for( int i = 0; i < keys.Length; i++)
			{
				string key = keys[i];
				if( false == _Components.ContainsKey(key) ) 
				{
					return false;
				}
			}
			
			return true;
		}
	}
}
#endif