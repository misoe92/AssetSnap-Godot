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

namespace AssetSnap.ASNode.MeshInstance
{
	using Godot;
	
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

		public override void _EnterTree()
		{
			if( false == Floating && IsInstanceValid(this)) 
			{
				GlobalExplorer.GetInstance().Waypoints.Register(this, Transform.Origin, RotationDegrees, Scale);
				WaypointAdded = true;
			}
			
			base._Ready();
		} 

		public void SetIsFloating( bool state )
		{
			Floating = state;
			 
			if( false == state && IsInstanceValid(this)) 
			{
				GlobalExplorer.GetInstance().Waypoints.Register(this, Transform.Origin, RotationDegrees, Scale);
				WaypointAdded = true;
			}
		}
		
		public void UpdateWaypointScale()
		{
			GlobalExplorer.GetInstance().Waypoints.UpdateScaleOnPoint(Transform.Origin, Scale);
		}
		
		public void UpdateWaypointRotation()
		{
			
		}
		
		public void AddSetting( string key, Variant value )
		{
			SpawnSettings.Add(key, value);
		}
		
		public void RemoveSetting( string key )
		{
			SpawnSettings.Remove(key);
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
		
		public T GetSetting<[MustBeVariant] T>( string key ) 
		{
			if( HasSetting( key ) ) 
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
		
		public bool HasSetting( string key )
		{
			return SpawnSettings.ContainsKey(key);
		}
		
		public bool HasLibraryName()
		{
			return LibraryName != null && LibraryName != "";
		}

		public void SetLibraryName( string name ) 
		{
			LibraryName = name;
		}  
		
		public Library.Instance GetLibrary()
		{
			if( null == LibraryName || "" == LibraryName ) 
			{
				return null;
			}
			
			return GlobalExplorer.GetInstance().GetLibraryByName( LibraryName );
		}
		
		public Godot.Collections.Dictionary<string, Variant> GetLibrarySettings()
		{
			return SpawnSettings;
		}
		
		public EventMouse CurrentMouseInput()
		{
			return GlobalExplorer.GetInstance().CurrentMouseInput;
		}
			
		public Node3D GetModel()
		{
			return GlobalExplorer.GetInstance().Model;
		}
		
		public Node3D GetHandle()
		{
			return GlobalExplorer.GetInstance().GetHandle();
		}
		
		public float GetDeltaTime()
		{
			return GlobalExplorer.GetInstance().DeltaTime;
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
			return GlobalExplorer.GetInstance().HandleIsModel();
		}
		
		public bool HasModel()
		{
			return GlobalExplorer.GetInstance().HasModel;
		}
		 
		public bool IsModelPlaced()
		{
			return GlobalExplorer.GetInstance().IsModelPlaced;
		}

		public override void _ExitTree()
		{ 
			if( null != GlobalExplorer.GetInstance() && null != GlobalExplorer.GetInstance().Waypoints ) 
			{
				if( IsInstanceValid(this) ) 
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