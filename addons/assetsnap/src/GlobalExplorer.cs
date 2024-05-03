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
	using AssetSnap.States;
	using Godot;

	/// <summary>
	/// Partial class for managing global methods and interaction with AssetSnap.
	/// </summary>
	[Tool]
	public partial class GlobalExplorer : NodeExplorer
	{
		private static GlobalExplorer Instance;
		
		/// <summary>
		/// Singleton instance of the GlobalExplorer.
		/// </summary>
		public static GlobalExplorer Singleton
		{
			get
			{
				if (Instance == null)
				{
					Instance = new();
				}

				return Instance;
			}
		}

		/// <summary>
		/// Gets the input driver currently in use.
		/// </summary>
		/// <returns>The input driver instance.</returns>
		public Instance.Input.BaseInputDriver InputDriver
		{
			get => DragIsAllowed() ? DragAddInputDriver.GetInstance() : BaseInputDriver.GetInstance();
		}

		/// <summary>
		/// Initializes the GlobalExplorer singleton instance.
		/// </summary>
		/// <returns>The initialized GlobalExplorer instance.</returns>
		public static GlobalExplorer InitializeExplorer()
		{
			if (null == Instance)
			{
				Instance = new GlobalExplorer()
				{
					_Settings = Front.Configs.SettingsConfig.Singleton,
					_Waypoints = new(),
					_Components = Component.Base.Singleton,
					_ContextMenu = AssetSnap.ContextMenu.Base.Singleton,
					_BottomDock = AssetSnap.BottomDock.Base.Singleton,
					_Decal = new(),
					_Raycast = new(),
					_Modifiers = new(),
					_Library = AssetSnap.Library.Base.Singleton,
					_GroupBuilder = AssetSnap.GroupBuilder.Base.Singleton,
					_Snap = AssetSnap.Snap.Base.Singleton,
					_States = GlobalStates.Singleton,
					_Inspector = new(),
					_Snappable = AssetSnap.Snap.SnappableBase.Singleton,
				};
			}

			return Instance;
		}

		/// <summary>
		/// Retrieves the singleton instance of GlobalExplorer.
		/// </summary>
		/// <returns>The singleton instance of GlobalExplorer.</returns>
		public static GlobalExplorer GetInstance()
		{
			return Singleton;
		}

		/// <summary>
		/// Retrieves a library instance by its name.
		/// </summary>
		/// <param name="name">The name of the library instance to retrieve.</param>
		/// <returns>The library instance.</returns>
		public Library.Instance GetLibraryByName(string name)
		{
			foreach (Library.Instance instance in Library.Libraries)
			{
				if (EditorPlugin.IsInstanceValid(instance) && instance.GetName() == name)
				{
					return instance;
				}
			}

			return null;
		}

		/// <summary>
		/// Retrieves a library instance by its index.
		/// </summary>
		/// <param name="index">The index of the library instance to retrieve.</param>
		/// <returns>The library instance.</returns>
		public Library.Instance GetLibraryByIndex(int index)
		{
			if (index > -1 && Library.Libraries.Count > index && EditorPlugin.IsInstanceValid(Library.Libraries[index]))
			{
				return Library.Libraries[index] as Library.Instance;
			}

			return null;
		}

		/// <summary>
		/// Prints the names of child nodes.
		/// </summary>
		/// <param name="which">The node whose child names to print.</param>
		public void PrintChildNames(Node which)
		{
			foreach (Node child in which.GetChildren())
			{
				if (child is Control childControl && HasProperty(childControl, "Text"))
				{
					string text = "";
					TryGetProperty(childControl, "Text", out text);
				}

				if (0 != child.GetChildCount())
				{
					PrintChildNames(child);
				}
			}
		}

		/// <summary>
		/// Sets focus to a specific 3D node.
		/// </summary>
		/// <param name="Node">The 3D node to set focus to.</param>
		public void SetFocusToNode(Node3D Node)
		{
			if (null == Node)
			{
				StatesUtils.Get().CurrentLibrary.ClearActivePanelState(null);
				StatesUtils.Get().CurrentLibrary._LibrarySettings._LSEditing.SetText("None");

				StatesUtils.Get().EditingObject = null;
				StatesUtils.Get().Group = null;
				StatesUtils.Get().GroupedObject = null;

				return;
			}

			EditorInterface.Singleton.EditNode(Node);
			StatesUtils.Get().EditingObject = Node;
			StatesUtils.Get().EditingTitle = Node.Name;

			if (Node is AsMeshInstance3D _instance)
			{
				HandleNode = _instance;
				Model = _instance;

				Library.Instance Library = GetLibraryByName(_instance.GetLibraryName());
				Library._LibrarySettings._LSEditing.SetText(Node.Name);
				StatesUtils.Get().CurrentLibrary = Library;

				if (InputDriver is DragAddInputDriver DraggableInputDriver)
				{
					DraggableInputDriver.CalculateObjectSize();
				}
			}

			if (Node is AsNode3D _nodeInstance)
			{
				HandleNode = _nodeInstance;

				Library.Instance Library = GetLibraryByName(_nodeInstance.GetLibraryName());
				Library._LibrarySettings._LSEditing.SetText(Node.Name);
				StatesUtils.Get().CurrentLibrary = Library;

				if (InputDriver is DragAddInputDriver DraggableInputDriver)
				{
					DraggableInputDriver.CalculateObjectSize();
				}
			}

			if (Node is AsGrouped3D _Grouped3D)
			{
				StatesUtils.Get().PlacingMode = GlobalStates.PlacingModeEnum.Group;

				Transform3D transform = _Grouped3D.Transform;
				transform.Origin = new Vector3(0, 0, 0);
				_Grouped3D.Transform = transform;

				StatesUtils.Get().GroupedObject = _Grouped3D;

				if (InputDriver is DragAddInputDriver DraggableInputDriver)
				{
					DraggableInputDriver.CalculateObjectSize();
				}
			}
		}

		/// <summary>
		/// Checks whether a Control has a specified property.
		/// </summary>
		/// <param name="control">The Control to check.</param>
		/// <param name="propertyName">The name of the property to check for.</param>
		/// <returns>True if the Control has the specified property; otherwise, false.</returns>
		public static bool HasProperty(Control control, string propertyName)
		{
			Type type = control.GetType();
			PropertyInfo propertyInfo = type.GetProperty(propertyName);
			return propertyInfo != null;
		}

		/// <summary>
		/// Attempts to retrieve the value of a property from a Control.
		/// </summary>
		/// <typeparam name="T">The type of the property value.</typeparam>
		/// <param name="control">The Control to retrieve the property from.</param>
		/// <param name="propertyName">The name of the property to retrieve.</param>
		/// <param name="value">The retrieved value of the property.</param>
		/// <returns>True if the property value was successfully retrieved; otherwise, false.</returns>
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

		/// <summary>
        /// Prints the fields of a Node.
        /// </summary>
        /// <param name="which">The Node whose fields to print.</param>
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

		/// <summary>
        /// Checks whether drag adding of models is allowed.
        /// </summary>
        /// <returns>True if drag adding is allowed; otherwise, false.</returns>
		private bool DragIsAllowed()
		{
			bool value = Settings.GetKey("allow_drag_add").As<bool>();
			return value;
		}

	}
}
#endif
