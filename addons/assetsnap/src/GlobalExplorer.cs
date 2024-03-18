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
namespace AssetSnap
{
	using System;
	using System.Reflection;
	using AssetSnap.Instance.Input;
	using Godot;

	[Tool]
	public partial class GlobalExplorer : NodeExplorer 
	{
		private static GlobalExplorer Instance;
		
		/*
		** The input driver which are currently in use
		*/
		public Instance.Input.BaseInputDriver InputDriver 
		{
			get => DragIsAllowed() ? DragAddInputDriver.GetInstance() : BaseInputDriver.GetInstance();
		}

		/*
		** Fetches singleton instance of the GlobalExplorer
		**
		** @return GlobalExplorer  
		*/ 
		public static GlobalExplorer InitializeExplorer()
		{
			if( null == Instance ) 
			{
				Instance = new GlobalExplorer()
				{
					_Settings = new(),
					_Waypoints = new(),
					_Components = new(),
					_ContextMenu = new(), 
					_BottomDock = new(),
					_Decal = new(),
					_Raycast = new(),
					_Modifiers = new(),
					_Library = new(),
				};
			}
			
			return Instance;
		}
		
		public static GlobalExplorer GetInstance()
		{
			return Instance;
		}
		
		public Library.Instance GetLibraryByName( string name )
		{
			foreach( Library.Instance instance in _Library.Libraries ) 
			{
				if( EditorPlugin.IsInstanceValid( instance ) && instance.GetName() == name ) 
				{
					return instance;
				}
			}
			
			return null;
		}
		
		public void PrintChildNames( Node which ) 
		{
			foreach( Node child in which.GetChildren() ) 
			{
				if( child is Control childControl && HasProperty(childControl, "Text")) 
				{
					string text = "";
					TryGetProperty(childControl, "Text", out text);
				}

				if( 0 != child.GetChildCount() ) 
				{
					PrintChildNames(child);
				}
			}
		}
		
		public static bool HasProperty(Control control, string propertyName)
		{
			Type type = control.GetType();
			PropertyInfo propertyInfo = type.GetProperty(propertyName);
			return propertyInfo != null;
		}
		
		public static bool TryGetProperty<T>(Control control, string propertyName, out T value)
		{
			value = default(T);
			Type type = control.GetType();
			PropertyInfo propertyInfo = type.GetProperty(propertyName);
			if (propertyInfo != null)
			{
				try
				{
					value = (T)propertyInfo.GetValue(control);
					return true;
				}
				catch (Exception e)
				{
					GD.PrintErr($"Failed to get property {propertyName}: {e.Message}");
					return false;
				}
			}
			else
			{
				return false;
			}
		}
		
		public void PrintFields(Node which)
		{
			Type type = which.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			Console.WriteLine($"Fields for class {type.Name}:");

			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(which);
				Console.WriteLine($"{field.Name}: {value}");
			}
		}
		
		/*
		** Checks whether or not it's allowed to perform drag adding of models
		**
		** @return bool
		*/
		private bool DragIsAllowed()
		{
			bool value = Settings.GetKey("allow_drag_add").As<bool>(); 
			return value;
		}
	}
}
#endif
