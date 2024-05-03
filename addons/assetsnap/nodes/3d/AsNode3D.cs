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
using AssetSnap.Library;
using AssetSnap.Nodes;
using AssetSnap.States;
using Godot;

namespace AssetSnap.Front.Nodes
{
[Tool]
public partial class AsNode3D : Node3D, IDriverableModel
{
	[Export]
	public string LibraryName { get; set; }

	[Export]
	public Godot.Collections.Dictionary<string, Variant> SpawnSettings { get => _SpawnSettings; set { _SpawnSettings = value; } }

	[Export]
	public bool Floating { get => _Floating; set{ _Floating = value; } }

	[Export]
	public bool WaypointAdded { get => _WaypointAdded; set{ _WaypointAdded = value; } }
	
	private Godot.Collections.Dictionary<string, Variant> _SpawnSettings = new(); 
	private ModelDriver Driver;
	private bool _Floating = false; 
	private bool _WaypointAdded = false; 

	public AsNode3D()
	{
		SetMeta("AsModel", true);
	}
	
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
	
	public void SetChildOwner(Node owner)
	{
		foreach( Node child in GetChildren()) 
		{
			child.Owner = owner;
		}
	}

	public void SetIsFloating(bool state)
	{
		Floating = state;

		if (false == state && IsInstanceValid(this))
		{
			ExplorerUtils.Get().Waypoints.Register(this, Transform.Origin, RotationDegrees, Scale);
			WaypointAdded = true;
		}
	}

	public void UpdateWaypointScale()
	{
		ExplorerUtils.Get().Waypoints.UpdateScaleOnPoint(Transform.Origin, Scale);
	}

	public void UpdateWaypointRotation()
	{

	}

	public void AddSetting(string key, Variant value)
	{
		SpawnSettings.Add(key, value);
	}

	public void RemoveSetting(string key)
	{
		SpawnSettings.Remove(key);
	}

	public Aabb GetAabb()
	{
		return NodeUtils.CalculateNodeAabb(this);
	}

	public Godot.Collections.Dictionary<string, Variant> GetSettings()
	{
		return SpawnSettings;
	}

	public Variant GetSetting(string key)
	{
		Variant value = false;
		SpawnSettings.TryGetValue(key, out value);

		return value;
	}

	public T GetSetting<[MustBeVariant] T>(string key)
	{
		if (HasSetting(key))
		{
			Variant value = GetSetting(key);

			return value.As<T>();
		}

		return default(T);
	}

	public string GetLibraryName()
	{
		return LibraryName;
	}

	public bool HasSetting(string key)
	{
		return SpawnSettings.ContainsKey(key);
	}

	public bool HasLibraryName()
	{
		return LibraryName != null && LibraryName != "";
	}

	public void SetLibraryName(string name)
	{
		LibraryName = name;
	}

	public Instance GetLibrary()
	{
		if (null == LibraryName || "" == LibraryName)
		{
			return null;
		}

		return ExplorerUtils.Get().GetLibraryByName(LibraryName);
	}

	public Godot.Collections.Dictionary<string, Variant> GetLibrarySettings()
	{
		return SpawnSettings;
	}

	public EventMouse CurrentMouseInput()
	{
		return ExplorerUtils.Get().CurrentMouseInput;
	}

	public Node3D GetModel()
	{
		return StatesUtils.Get().EditingObject;
	}

	public Node3D GetHandle()
	{
		return StatesUtils.Get().EditingObject;
	}

	public ModelDriver.ModelTypes GetModelType()
	{
		return Driver.GetModelType();
	}

	public float GetDeltaTime()
	{
		return ExplorerUtils.Get().DeltaTime;
	}

	public bool IsPlaced()
	{
		return GetParent() != null && GetParent().Name != "AsDecal";
	}

	public bool HasLibrarySettings()
	{
		return SpawnSettings != null && SpawnSettings.Count != 0;
	}

	public bool HandleIsModel()
	{
		return ExplorerUtils.Get().HandleIsModel();
	}

	public bool HasModel()
	{
		return ExplorerUtils.Get().HasModel;
	}

	public bool IsModelPlaced()
	{
		return ExplorerUtils.Get().IsModelPlaced;
	}

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