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

using AssetSnap.Explorer;
using AssetSnap.Nodes;
using AssetSnap.States;
using Godot;

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// Represents a 3D node with various utility methods and properties for modeling.
	/// </summary>
	[Tool]
	public partial class AsNode3D : Node3D, IDriverableModel
	{	
		/// <summary>
		/// Gets or sets the name of the library associated with the node.
		/// </summary>
		[Export]
		public string LibraryName { get; set; }

		/// <summary>
		/// Gets or sets the spawn settings dictionary associated with the node.
		/// </summary>
		[Export]
		public Godot.Collections.Dictionary<string, Variant> SpawnSettings { get => _SpawnSettings; set { _SpawnSettings = value; } }

		/// <summary>
		/// Gets or sets a value indicating whether the node is floating.
		/// </summary>
		[Export]
		public bool Floating { get => _Floating; set{ _Floating = value; } }

		/// <summary>
		/// Gets or sets a value indicating whether a waypoint is added to the node.
		/// </summary>
		[Export]
		public bool WaypointAdded { get => _WaypointAdded; set{ _WaypointAdded = value; } }
		
		private Godot.Collections.Dictionary<string, Variant> _SpawnSettings = new(); 
		private ModelDriver Driver;
		private bool _Floating = false; 
		private bool _WaypointAdded = false; 

		/// <summary>
		/// Constructor for AsNode3D class.
		/// </summary>
		public AsNode3D()
		{
			SetMeta("AsModel", true);
		}
		
		/// <summary>
		/// Called when the node enters the scene tree.
		/// </summary>
		public override void _EnterTree()
		{
			Driver = new();
			Driver.RegisterType(this);

			if (false == Floating && IsInstanceValid(this))
			{
				ExplorerUtils.Get().Waypoints.Register(this, Transform.Origin, RotationDegrees, Scale);
				WaypointAdded = true;
			}

			base._Ready();
		}
		
		/// <summary>
		/// Sets the owner of the child nodes.
		/// </summary>
		/// <param name="owner">The owner node.</param>
		public void SetChildOwner(Node owner)
		{
			foreach( Node child in GetChildren()) 
			{
				child.Owner = owner;
			}
		}

		/// <summary>
		/// Sets the floating state of the node.
		/// </summary>
		/// <param name="state">The floating state.</param>
		public void SetIsFloating(bool state)
		{
			Floating = state;

			if (false == state && IsInstanceValid(this))
			{
				ExplorerUtils.Get().Waypoints.Register(this, Transform.Origin, RotationDegrees, Scale);
				WaypointAdded = true;
			}
		}

		/// <summary>
		/// Updates the scale of the node on the waypoint.
		/// </summary>
		public void UpdateWaypointScale()
		{
			ExplorerUtils.Get().Waypoints.UpdateScaleOnPoint(Transform.Origin, Scale);
		}

		/// <summary>
		/// Updates the rotation of the node on the waypoint.
		/// </summary>
		public void UpdateWaypointRotation()
		{

		}

		/// <summary>
		/// Adds a setting to the node's spawn settings.
		/// </summary>
		/// <param name="key">The key of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		public void AddSetting(string key, Variant value)
		{
			SpawnSettings.Add(key, value);
		}

		/// <summary>
		/// Removes a setting from the spawn settings dictionary based on the provided key.
		/// </summary>
		/// <param name="key">The key of the setting to be removed.</param>
		public void RemoveSetting(string key)
		{
			SpawnSettings.Remove(key);
		}

		/// <summary>
		/// Calculates the axis-aligned bounding box (AABB) of the node.
		/// </summary>
		/// <returns>The axis-aligned bounding box (AABB) of the node.</returns>
		public Aabb GetAabb()
		{
			return NodeUtils.CalculateNodeAabb(this);
		}

		/// <summary>
		/// Retrieves all settings associated with the node.
		/// </summary>
		/// <returns>The dictionary containing all settings associated with the node.</returns>
		public Godot.Collections.Dictionary<string, Variant> GetSettings()
		{
			return SpawnSettings;
		}

		/// <summary>
		/// Retrieves a setting from the spawn settings dictionary based on the provided key.
		/// </summary>
		/// <param name="key">The key of the setting to retrieve.</param>
		/// <returns>The value of the setting associated with the provided key.</returns>
		public Variant GetSetting(string key)
		{
			Variant value = false;
			SpawnSettings.TryGetValue(key, out value);

			return value;
		}

		/// <summary>
		/// Retrieves a typed setting from the spawn settings dictionary based on the provided key.
		/// </summary>
		/// <typeparam name="T">The type of the setting.</typeparam>
		/// <param name="key">The key of the setting to retrieve.</param>
		/// <returns>The typed value of the setting associated with the provided key.</returns>
		public T GetSetting<[MustBeVariant] T>(string key)
		{
			if (HasSetting(key))
			{
				Variant value = GetSetting(key);

				return value.As<T>();
			}

			return default(T);
		}

		/// <summary>
		/// Retrieves the name of the library associated with the node.
		/// </summary>
		/// <returns>The name of the library associated with the node.</returns>
		public string GetLibraryName()
		{
			return LibraryName;
		}

		/// <summary>
		/// Checks if a setting with the provided key exists in the spawn settings dictionary.
		/// </summary>
		/// <param name="key">The key of the setting to check.</param>
		/// <returns>True if the setting exists, otherwise false.</returns>
		public bool HasSetting(string key)
		{
			return SpawnSettings.ContainsKey(key);
		}

		/// <summary>
		/// Checks if the node has a library name.
		/// </summary>
		/// <returns>True if the node has a library name, otherwise false.</returns>
		public bool HasLibraryName()
		{
			return LibraryName != null && LibraryName != "";
		}

		/// <summary>
		/// Sets the name of the library associated with the node.
		/// </summary>
		/// <param name="name">The name of the library.</param>
		public void SetLibraryName(string name)
		{
			LibraryName = name;
		}

		/// <summary>
		/// Retrieves the library associated with the node.
		/// </summary>
		/// <returns>The library instance if found, otherwise null.</returns>
		public Library.Instance GetLibrary()
		{
			if (null == LibraryName || "" == LibraryName)
			{
				return null;
			}

			return ExplorerUtils.Get().GetLibraryByName(LibraryName);
		}

		/// <summary>
		/// Retrieves the settings of the library associated with the node.
		/// </summary>
		/// <returns>The dictionary of settings of the library.</returns>
		public Godot.Collections.Dictionary<string, Variant> GetLibrarySettings()
		{
			return SpawnSettings;
		}

		/// <summary>
		/// Retrieves the current mouse input.
		/// </summary>
		/// <returns>The current mouse input event.</returns>
		public EventMouse CurrentMouseInput()
		{
			return ExplorerUtils.Get().CurrentMouseInput;
		}

		/// <summary>
		/// Retrieves the model node associated with the node.
		/// </summary>
		/// <returns>The model node.</returns>
		public Node3D GetModel()
		{
			return StatesUtils.Get().EditingObject;
		}

		/// <summary>
		/// Retrieves the handle associated with the node.
		/// </summary>
		/// <returns>The handle node.</returns>
		public Node3D GetHandle()
		{
			return StatesUtils.Get().EditingObject;
		}

		/// <summary>
		/// Retrieves the model type of the node.
		/// </summary>
		/// <returns>The model type of the node.</returns>
		public ModelDriver.ModelTypes GetModelType()
		{
			return Driver.GetModelType();
		}

		/// <summary>
		/// Retrieves the delta time.
		/// </summary>
		/// <returns>The delta time.</returns>
		public float GetDeltaTime()
		{
			return ExplorerUtils.Get().DeltaTime;
		}

		/// <summary>
		/// Checks if the node is placed in the scene.
		/// </summary>
		/// <returns>True if the node is placed, otherwise false.</returns>
		public bool IsPlaced()
		{
			return GetParent() != null && GetParent().Name != "AsDecal";
		}

		/// <summary>
		/// Checks if the node has library settings.
		/// </summary>
		/// <returns>True if the node has library settings, otherwise false.</returns>
		public bool HasLibrarySettings()
		{
			return SpawnSettings != null && SpawnSettings.Count != 0;
		}

		/// <summary>
		/// Checks if the handle is a model.
		/// </summary>
		/// <returns>True if the handle is a model, otherwise false.</returns>
		public bool HandleIsModel()
		{
			return ExplorerUtils.Get().HandleIsModel();
		}

		/// <summary>
		/// Checks if the node has a model.
		/// </summary>
		/// <returns>True if the node has a model, otherwise false.</returns>
		public bool HasModel()
		{
			return ExplorerUtils.Get().HasModel;
		}

		/// <summary>
		/// Checks if the model associated with the node is placed.
		/// </summary>
		/// <returns>True if the model associated with the node is placed, otherwise false.</returns>
		public bool IsModelPlaced()
		{
			return ExplorerUtils.Get().IsModelPlaced;
		}

		/// <summary>
		/// Exits the scene tree.
		/// </summary>
		public override void _ExitTree()
		{
			// if (null != ExplorerUtils.Get() && null != ExplorerUtils.Get().Waypoints)
			// {
			// 	if (IsInstanceValid(this))
			// 	{
			// 		// ExplorerUtils.Get().Waypoints.Remove(this, Transform.Origin);
			// 	}
			// }

			Floating = true;
			WaypointAdded = false;

			base._ExitTree();
		}
	}
}