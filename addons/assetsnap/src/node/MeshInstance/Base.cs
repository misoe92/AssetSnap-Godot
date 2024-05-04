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

using Godot;

namespace AssetSnap.ASNode.MeshInstance
{
	[Tool]
	public partial class Base : MeshInstance3D
	{
		[Export]
		public string LibraryName { get; set; }

		[Export]
		public Godot.Collections.Dictionary<string, Variant> SpawnSettings { get; set; } = new();

		[Export]
		public bool Floating { get; set; } = false;

		[Export]
		public bool WaypointAdded { get; set; } = false;

		/// <summary>
		/// Called when the node enters the scene tree.
		/// </summary>
		public override void _EnterTree()
		{
			if (false == Floating && IsInstanceValid(this))
			{
				GlobalExplorer.GetInstance().Waypoints.Register(this, Transform.Origin, RotationDegrees, Scale);
				WaypointAdded = true;
			}

			base._Ready();
		}

		/// <summary>
		/// Sets the floating state of the mesh instance.
		/// </summary>
		/// <param name="state">The state to set.</param>
		public void SetIsFloating(bool state)
		{
			Floating = state;

			if (false == state && IsInstanceValid(this))
			{
				GlobalExplorer.GetInstance().Waypoints.Register(this, Transform.Origin, RotationDegrees, Scale);
				WaypointAdded = true;
			}
		}

		/// <summary>
		/// Updates the scale of the waypoint.
		/// </summary>
		public void UpdateWaypointScale()
		{
			GlobalExplorer.GetInstance().Waypoints.UpdateScaleOnPoint(Transform.Origin, Scale);
		}

		public void UpdateWaypointRotation()
		{

		}

		/// <summary>
		/// Adds a setting to the spawn settings dictionary.
		/// </summary>
		/// <param name="key">The key of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		public void AddSetting(string key, Variant value)
		{
			SpawnSettings.Add(key, value);
		}

		/// <summary>
		/// Removes a setting from the spawn settings dictionary.
		/// </summary>
		/// <param name="key">The key of the setting to remove.</param>
		public void RemoveSetting(string key)
		{
			SpawnSettings.Remove(key);
		}

		/// <summary>
		/// Gets the settings associated with this mesh instance.
		/// </summary>
		/// <returns>The dictionary containing the settings.</returns>
		public Godot.Collections.Dictionary<string, Variant> GetSettings()
		{
			return SpawnSettings;
		}

		/// <summary>
		/// Gets a specific setting value by its key.
		/// </summary>
		/// <param name="key">The key of the setting.</param>
		/// <returns>The value of the setting.</returns>
		public Variant GetSetting(string key)
		{
			Variant value = false;
			SpawnSettings.TryGetValue(key, out value);

			return value;
		}

		/// <summary>
		/// Gets a specific setting value by its key and casts it to the specified type.
		/// </summary>
		/// <typeparam name="T">The type to cast the setting value to.</typeparam>
		/// <param name="key">The key of the setting.</param>
		/// <returns>The value of the setting casted to the specified type.</returns>
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
		/// Gets the name of the library associated with this mesh instance.
		/// </summary>
		/// <returns>The name of the library.</returns>
		public string GetLibraryName()
		{
			return LibraryName;
		}

		/// <summary>
		/// Checks if a setting with the specified key exists.
		/// </summary>
		/// <param name="key">The key of the setting to check.</param>
		/// <returns>True if the setting exists, false otherwise.</returns>
		public bool HasSetting(string key)
		{
			return SpawnSettings.ContainsKey(key);
		}

		/// <summary>
		/// Checks if the mesh instance has a library name assigned.
		/// </summary>
		/// <returns>True if the mesh instance has a library name assigned, false otherwise.</returns>
		public bool HasLibraryName()
		{
			return LibraryName != null && LibraryName != "";
		}

		/// <summary>
		/// Sets the name of the library associated with this mesh instance.
		/// </summary>
		/// <param name="name">The name of the library.</param>
		public void SetLibraryName(string name)
		{
			LibraryName = name;
		}

		/// <summary>
		/// Gets the library instance associated with this mesh instance.
		/// </summary>
		/// <returns>The library instance, or null if no library is associated.</returns>
		public Library.Instance GetLibrary()
		{
			if (null == LibraryName || "" == LibraryName)
			{
				return null;
			}

			return GlobalExplorer.GetInstance().GetLibraryByName(LibraryName);
		}

		/// <summary>
		/// Gets the settings of the library associated with this mesh instance.
		/// </summary>
		/// <returns>The dictionary containing the library settings.</returns>
		public Godot.Collections.Dictionary<string, Variant> GetLibrarySettings()
		{
			return SpawnSettings;
		}

		/// <summary>
		/// Gets the current mouse input from the global explorer instance.
		/// </summary>
		/// <returns>The current mouse input event.</returns>
		public EventMouse CurrentMouseInput()
		{
			return GlobalExplorer.GetInstance().CurrentMouseInput;
		}

		/// <summary>
		/// Gets the handle node from the global explorer instance.
		/// </summary>
		/// <returns>The handle node.</returns>
		public Node3D GetHandle()
		{
			return GlobalExplorer.GetInstance().GetHandle();
		}

		/// <summary>
		/// Gets the time in seconds since the last frame.
		/// </summary>
		/// <returns>The time in seconds since the last frame.</returns>
		public float GetDeltaTime()
		{
			return GlobalExplorer.GetInstance().DeltaTime;
		}

		/// <summary>
		/// Checks if the mesh instance is placed in the scene.
		/// </summary>
		/// <returns>True if the mesh instance is placed in the scene; otherwise, false.</returns>
		public bool IsPlaced()
		{
			return
				GetParent() != null &&
				GetParent().Name != "AsDecal" &&
				null != GetParent() &&
				null != GetParent().GetParent() &&
				GetParent().GetParent().Name != "AsDecal";
		}

		/// <summary>
		/// Checks if the mesh instance has library settings.
		/// </summary>
		/// <returns>True if the mesh instance has library settings; otherwise, false.</returns>
		public bool HasLibrarySettings()
		{
			return SpawnSettings != null && SpawnSettings.Count != 0;
		}

		/// <summary>
		/// Checks if the handle is a model.
		/// </summary>
		/// <returns>True if the handle is a model; otherwise, false.</returns>
		public bool HandleIsModel()
		{
			return GlobalExplorer.GetInstance().HandleIsModel();
		}

		/// <summary>
		/// Checks if the mesh instance has a model.
		/// </summary>
		/// <returns>True if the mesh instance has a model; otherwise, false.</returns>
		public bool HasModel()
		{
			return GlobalExplorer.GetInstance().HasModel;
		}

		/// <summary>
        /// Checks if the model is placed in the scene.
        /// </summary>
        /// <returns>True if the model is placed in the scene; otherwise, false.</returns>
		public bool IsModelPlaced()
		{
			return GlobalExplorer.GetInstance().IsModelPlaced;
		}

		/// <summary>
		/// Called when the node is about to be removed from the scene tree.
		/// </summary>
		public override void _ExitTree()
		{
			if (null != GlobalExplorer.GetInstance() && null != GlobalExplorer.GetInstance().Waypoints)
			{
				if (IsInstanceValid(this))
				{
					GlobalExplorer.GetInstance().Waypoints.Remove(this, Transform.Origin);
				}
			}

			Floating = true;
			WaypointAdded = false;

			base._ExitTree();
		}
	}
}