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
using System.Reflection;
using AssetSnap.Abstracts;
using AssetSnap.Explorer;
using Godot;
using Godot.Collections;

namespace AssetSnap.Component
{
	/// <summary>
	/// Base class for components, providing common functionality.
	/// </summary>
	[Tool]
	public partial class BaseComponent : AbstractComponentBase
	{
		/// <summary>
		/// The type string representing the component's type.
		/// </summary>
		public string TypeString = "";
		
		/* Debugging Purpose */
		public bool Include = true;
		protected bool _Disposed = false;
		/* -- */
		
		/// <summary>
		/// The list of traits used by this component.
		/// </summary>
		protected Array<string> _UsingTraits = new(){};
		
		/// <summary>
		/// Virtual method for entering. Ensures that there is always an enter method to call.
		/// </summary>
		public virtual void Enter()
		{
			//
		}
		
		/// <summary>
		/// Virtual method for initialization. Ensures that there is always an initialize method to call.
		/// </summary>
		public virtual void Initialize()
		{
			if( null != GetParent()) 
			{
				return;
			}

			// Container.AddChild(this);
			
			if( Name == "" ) 
			{
				throw new Exception("Invalid name for component");
			}
		}
		
		/// <summary>
		/// Gets the value for a specified key.
		/// </summary>
		/// <param name="key">The key for which to retrieve the value.</param>
		/// <returns>The value associated with the specified key.</returns>
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
		
		/// <summary>
		/// Dynamically sets a property on the component.
		/// </summary>
		/// <param name="key">The name of the property to set.</param>
		/// <param name="value">The value to set the property to.</param>
		/// <returns>True if the property was successfully set, false otherwise.</returns>
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
		
		/// <summary>
		/// Converts a variant type to a field type (e.g., bool, float, int).
		/// </summary>
		/// <param name="value">The variant value to convert.</param>
		/// <param name="targetType">The target type to convert to.</param>
		/// <returns>The converted value.</returns>
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
			
		/// <summary>
		/// Clears the component.
		/// </summary>
		/// <param name="debug">Optional parameter to enable debugging output.</param>	
		public virtual void Clear( bool debug = false )
		{
			ExplorerUtils.Get().Components.Remove(this);
			if( TypeString != "" ) 
			{
				ExplorerUtils.Get().Components.RemoveByTypeString(TypeString);
			}
		}
	}
}

#endif 