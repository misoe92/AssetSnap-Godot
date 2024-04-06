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
	using AssetSnap.Front.Nodes;
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
					_GroupBuilder = new(),
					_Snap = new(),
					_States = new(),
					_Inspector = new(),
					_Snappable = new(),
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
		
		public Library.Instance GetLibraryByIndex( long index )
		{
			if( index > -1 && _Library.Libraries.Length > index && EditorPlugin.IsInstanceValid(_Library.Libraries[index]) ) 
			{
				return _Library.Libraries[index];
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
		
		public void SetFocusToNode( Node3D Node ) 
		{
			if( null == Node ) 
			{
				CurrentLibrary.ClearActivePanelState(null);
				CurrentLibrary._LibrarySettings._LSEditing.SetText("None");

				States.EditingObject = null;
				HandleNode = null;
				Model = null;
				CurrentLibrary = null;

				return;
			}
		
			EditorInterface.Singleton.EditNode(Node);
			States.EditingObject = Node;
			
			if( Node is AsMeshInstance3D _instance ) 
			{
				HandleNode = _instance;
				Model = _instance;
				
				Library.Instance Library = GetLibraryByName(_instance.GetLibraryName());
				CurrentLibrary = Library;
				Library._LibrarySettings._LSEditing.SetText(Node.Name);
								
				if( InputDriver is DragAddInputDriver DraggableInputDriver ) 
				{
					DraggableInputDriver.CalculateObjectSize();
				}
			}
			
			if( Node is AsGrouped3D _Grouped3D ) 
			{
				States.PlacingMode = GlobalStates.PlacingModeEnum.Group;
				
				Transform3D transform = _Grouped3D.Transform;
				transform.Origin = new Vector3(0, 0, 0);
				_Grouped3D.Transform = transform;
				
				States.GroupedObject = _Grouped3D;
				
				if( InputDriver is DragAddInputDriver DraggableInputDriver ) 
				{
					DraggableInputDriver.CalculateObjectSize();
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

			GD.Print($"Fields for class {type.Name}:");

			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(which);
				GD.Print($"{field.Name}: {value}");
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
