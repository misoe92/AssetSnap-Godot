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
	using System.Reflection;
	using AssetSnap.Abstracts;
	using Godot;
	using Godot.Collections;

	[Tool]
	public partial class BaseComponent : AbstractComponentBase
	{
		/* Debugging Purpose */
		public bool _include = true;
		protected bool Disposed = false;
		/* -- */
		
		public Array<string> UsingTraits = new(){};
		
		protected Node _Container;
 
		[Export]
		public Node Container 
		{ 
			get => _Container;
			set
			{
				_Container = value;
			}
		}

		/*
		** Virtual method for entering
		** Ensures that there is always a enter method
		** to call
		**
		** @return void
		*/
		public virtual void Enter()
		{
			//
		}
		
		/*
		** Virtual method for initialization
		** Ensures that there is always a initialize method
		** to call
		** 
		** @return void
		*/
		public virtual void Initialize()
		{
			if( GetParent() == Container ) 
			{
				return;
			}

			Container.AddChild(this);
		}
		
		public virtual Variant GetValueFor( string key )
		{
			Type type = GetType();
			FieldInfo field = type.GetField(key);
			
			if (field != null)
			{
				object value = field.GetValue(this);
				
				if( value is bool boolVal ) 
				{
					return (Variant)boolVal;			
				}
				else if( value is int intVal ) 
				{
					return (Variant)intVal;			
				}
				else if( value is float floatVal ) 
				{
					return (Variant)floatVal;			
				}
				else if( value is double doubleVal ) 
				{
					return (Variant)doubleVal;			
				}
				else if( value is string stringVal ) 
				{
					return (Variant)stringVal;			
				}
				else
				{
					return (Variant)value;
				}
			}
			else
			{
				Console.WriteLine($"Property '{key}' not found or not writable in class '{type.Name}'.");
			}
			
			return false;
		}
		
		/*
		** Dynamicly set of property on the component
		**
		** @param string key
		** @param Variant value
		** @return bool
		*/
		public virtual bool SetProperty( string key, Variant value )
		{
			Type type = GetType();
			FieldInfo field = type.GetField(key);
			PropertyInfo property = type.GetProperty(key);
			
			if (field != null)
			{
				object convertedValue = ConvertVariantToFieldType(value, field.FieldType);
				if (convertedValue != null)
				{
					field.SetValue(this, convertedValue);
					return true;
				}
				else
				{
					GD.PushWarning($"Cannot set field '{key}' with value of type '{value.GetType().Name}' in class '{type.Name}'.");
				}
			}
			else if( property != null ) 
			{
				object convertedValue = ConvertVariantToFieldType(value, property.PropertyType);
				if (convertedValue != null)
				{
					property.SetValue(this, convertedValue);
					return true;
				}
				else
				{
					GD.PushWarning($"Cannot set property '{key}' with value of type '{value.GetType().Name}' in class '{type.Name}'.");
				}
			}
			else
			{
				GD.PushWarning($"Property or field with name '{key}' not found or not writable in class '{type.Name}'.");
			}
			
			return false;
		}
		
		/*
		** Converts a variant type to a field type i.e Bool,float,int etc.
		**
		** @param Variant value
		** @param Type targetType
		** @return object
		*/
		private object ConvertVariantToFieldType(Variant value, Type targetType)
		{
			if (targetType == typeof(bool))
			{
				return value.As<bool>();
			}
			else if (targetType == typeof(int))
			{
				return value.As<int>();
			}
			else if (targetType == typeof(float))
			{
				return value.As<float>();
			}
			
			// If the type is not supported, return null
			return null;
		}
		
		// /*
		// ** Cleaning of various fields when the component
		// ** is leaving the tree
		// **
		// ** @return void
		// */
		// public override void _ExitTree()
		// {
		// 	// if( IsInstanceValid(_Container) ) 
		// 	// {
		// 	// 	_Container.QueueFree();
		// 	// }
		// 	_Container = null;
		// 	// Disposed = true; 
		// }
		
		public virtual void Clear()
		{
			
		}
	}
}
#endif 