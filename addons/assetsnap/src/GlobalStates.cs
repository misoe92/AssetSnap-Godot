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
	using System.Collections.Generic;
	using System.Reflection;
	using AssetSnap.States;

	using AssetSnap.Front.Nodes;
	using Godot;

	[Tool]
	public partial class GlobalStates : LoadStates 
	{
	
		/** Library Enums **/
		public enum LibraryStateEnum 
		{
			Disabled,
			Enabled,
		}
		
		/** Snap Enums **/
		public enum SpawnStateEnum
		{
			Null,
			Spawned,
		}
		public enum SnapPosition 
		{
			Top,
			Middle,
			Bottom,
		}
		public enum VisibilityStateEnum 
		{
			Hidden,
			Visible,
		}
		
		public enum SnapAngleEnums
		{
			Y,
			X,
			Z,
		}
		
		public enum PlacingModeEnum
		{
			Model,
			Group,
		}
		
		public enum PlacingTypeEnum
		{
			Simple, // Simple mesh instances
			Optimized, // Multimesh
		}
		
		/** Decal States **/
		private SpawnStateEnum _DecalSpawned = SpawnStateEnum.Null;
		private VisibilityStateEnum _DecalVisible = VisibilityStateEnum.Hidden;
		private PlacingModeEnum _PlacingMode = PlacingModeEnum.Model;
		private PlacingTypeEnum _PlacingType = PlacingTypeEnum.Simple;
		private string _EditingTitle = "None";
		private Node3D _EditingObject = null;
		private Node _CurrentScene = null;
		private Library.Instance _CurrentLibrary = null;
	
		[ExportCategory("General")]
		[Export]
		public string EditingTitle 
		{
			get => _EditingTitle;
			set 
			{
				// if( value != _EditingTitle ) 
				// {
					_EditingTitle = value;
					StateChanged();
				// }
			}
		}
		[Export]
		public Node3D EditingObject
		{
			get => _EditingObject;
			set 
			{
				if( value != _EditingObject ) 
				{
					_EditingObject = value;
					StateChanged();
				}
			}
		}
		
		[Export]
		public Node CurrentScene
		{
			get => _CurrentScene;
			set 
			{
				if( value != _CurrentScene ) 
				{
					_CurrentScene = value;
					StateChanged();
				}
			}
		}

		[Export]
		public Library.Instance CurrentLibrary
		{
			get => _CurrentLibrary;
			set 
			{
				if( value != _CurrentLibrary ) 
				{
					_CurrentLibrary = value;
					StateChanged();
					
					GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
					if( null != _GlobalExplorer && null != _GlobalExplorer._Plugin && null != value ) 
					{
						Plugin _plugin = _GlobalExplorer._Plugin;
						_plugin.EmitSignal(Plugin.SignalName.LibraryChanged, value.GetName());
					}
				}
			}
		}
		
		[Export]
		public SpawnStateEnum DecalSpawned
		{
			get => _DecalSpawned;
			set
			{
				if( value != _DecalSpawned ) 
				{
					_DecalSpawned = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public VisibilityStateEnum DecalVisible
		{
			get => _DecalVisible;
			set
			{
				if( value != _DecalVisible ) 
				{
					_DecalVisible = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public PlacingModeEnum PlacingMode
		{
			get => _PlacingMode;
			set
			{
				if( value != _PlacingMode ) 
				{
					_PlacingMode = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public PlacingTypeEnum PlacingType
		{
			get => _PlacingType;
			set
			{
				if( value != _PlacingType ) 
				{
					_PlacingType = value;
					StateChanged();	
				}
			}
		}
		/** Group Enums **/
		//

		/** Library States **/
		private LibraryStateEnum _SnapToObject = LibraryStateEnum.Disabled;
		private LibraryStateEnum _SnapToHeight = LibraryStateEnum.Disabled;
		private LibraryStateEnum _SnapToHeightGlue = LibraryStateEnum.Disabled;
		private LibraryStateEnum _SnapToX = LibraryStateEnum.Disabled;
		private LibraryStateEnum _SnapToXGlue = LibraryStateEnum.Disabled;
		private LibraryStateEnum _SnapToZ = LibraryStateEnum.Disabled;
		private LibraryStateEnum _SnapToZGlue = LibraryStateEnum.Disabled;
		private LibraryStateEnum _SphereCollision = LibraryStateEnum.Disabled;
		private LibraryStateEnum _ConcaveCollision = LibraryStateEnum.Disabled;
		private LibraryStateEnum _ConvexCollision = LibraryStateEnum.Disabled;
		private LibraryStateEnum _ConvexClean = LibraryStateEnum.Disabled;
		private LibraryStateEnum _ConvexSimplify = LibraryStateEnum.Disabled;
		
		[ExportCategory("Placement States & Values")]
		[Export]
		public LibraryStateEnum SnapToObject
		{
			get => _SnapToObject;
			set
			{
				if( value != _SnapToObject ) 
				{
					_SnapToObject = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public LibraryStateEnum SnapToHeight
		{
			get => _SnapToHeight;
			set
			{
				if( value != _SnapToHeight ) 
				{
					_SnapToHeight = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public LibraryStateEnum SnapToHeightGlue
		{
			get => _SnapToHeightGlue;
			set
			{
				if( value != _SnapToHeightGlue ) 
				{
					_SnapToHeightGlue = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public LibraryStateEnum SnapToX
		{
			get => _SnapToX;
			set
			{
				if( value != _SnapToX ) 
				{
					_SnapToX = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public LibraryStateEnum SnapToXGlue
		{
			get => _SnapToXGlue;
			set
			{
				if( value != _SnapToXGlue ) 
				{
					_SnapToXGlue = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public LibraryStateEnum SnapToZ
		{
			get => _SnapToZ;
			set
			{
				if( value != _SnapToZ ) 
				{
					_SnapToZ = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public LibraryStateEnum SnapToZGlue
		{
			get => _SnapToZGlue;
			set
			{
				if( value != _SnapToZGlue ) 
				{
					_SnapToZGlue = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public LibraryStateEnum SphereCollision
		{
			get => _SphereCollision;
			set
			{
				if( value != _SphereCollision ) 
				{
					_SphereCollision = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public LibraryStateEnum ConcaveCollision
		{
			get => _ConcaveCollision;
			set
			{
				if( value != _ConcaveCollision ) 
				{
					_ConcaveCollision = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public LibraryStateEnum ConvexCollision
		{
			get => _ConvexCollision;
			set
			{
				if( value != _ConvexCollision ) 
				{
					_ConvexCollision = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public LibraryStateEnum ConvexClean
		{
			get => _ConvexClean;
			set
			{
				if( value != _ConvexClean ) 
				{
					_ConvexClean = value;
					StateChanged();	
				}
			}
		}
		
		[Export]
		public LibraryStateEnum ConvexSimplify
		{
			get => _ConvexSimplify;
			set
			{
				if( value != _ConvexSimplify ) 
				{
					_ConvexSimplify = value;
					StateChanged();	
				}
			}
		}
		

		/** Group States **/
		
		
		/** Library Values **/
		private int _SnapLayer = 0;
		private float _SnapToObjectOffsetXValue = 0;
		private float _SnapToObjectOffsetZValue = 0;
		private float _SnapToHeightValue = 0;
		private float _SnapToXValue = 0;
		private float _SnapToZValue = 0;
		private float _DragSizeOffset = 0;
		
	
		
		[Export]
		public int SnapLayer
		{
			get => _SnapLayer;
			set
			{
				if( value != _SnapLayer ) 
				{
					_SnapLayer = value;
					StateChanged();
				}
			}
		}
		
		[Export]
		public float SnapToObjectOffsetXValue
		{
			get => _SnapToObjectOffsetXValue;
			set
			{
				if( value != _SnapToObjectOffsetXValue ) 
				{
					_SnapToObjectOffsetXValue = value;
					StateChanged();
				}
			}
		}
		
		[Export]
		public float SnapToObjectOffsetZValue
		{
			get => _SnapToObjectOffsetZValue;
			set
			{
				if( value != _SnapToObjectOffsetZValue ) 
				{
					_SnapToObjectOffsetZValue = value;
					StateChanged();
				}
			}
		}
		
		[Export]
		public float SnapToHeightValue
		{
			get => _SnapToHeightValue;
			set
			{
				if( value != _SnapToHeightValue ) 
				{
					_SnapToHeightValue = value;
					StateChanged();
				}
			}
		}
		
		[Export]
		public float SnapToXValue
		{
			get => _SnapToXValue;
			set
			{
				if( value != _SnapToXValue ) 
				{
					_SnapToXValue = value;
					StateChanged();
				}
			}
		}
		
		[Export]
		public float SnapToZValue
		{
			get => _SnapToZValue;
			set
			{
				if( value != _SnapToZValue ) 
				{
					_SnapToZValue = value;
					StateChanged();
				}
			}
		}
		
		[Export]
		public float DragSizeOffset 
		{
			get => _DragSizeOffset;
			set 
			{
				if( value != _DragSizeOffset ) 
				{
					_DragSizeOffset = value;
					StateChanged();
				}
			}
		}
		/** Snap States **/
		private SpawnStateEnum _BoundarySpawned = SpawnStateEnum.Null;
		
		[ExportCategory("Boundary")]
		[Export]
		public SpawnStateEnum BoundarySpawned
		{
			get => _BoundarySpawned;
			set
			{
				if( value != _BoundarySpawned ) 
				{
					_BoundarySpawned = value;
					StateChanged();	
				}
			}
		}
		
		public List<SnapAngleEnums> BoundaryActiveAngles { get; set; } = new List<SnapAngleEnums>();

		/** Group Values **/
		private SnapPosition _GroupSnapsTo = SnapPosition.Middle;
		private GroupResource _Group;
		private AsGrouped3D _GroupedObject;
		private Godot.Collections.Dictionary<string, Godot.Collections.Array<AsGrouped3D>> _GroupedObjects = new();
		
		[ExportCategory("Group")]
		[Export]
		public SnapPosition GroupSnapsTo
		{
			get => _GroupSnapsTo;
			set 
			{
				_GroupSnapsTo = value;
				StateChanged();
			}
		}
		
		[Export]
		public GroupResource Group
		{
			get => _Group;
			set 
			{
				_Group = value;
				StateChanged();
			}
		}
		
		[Export]
		public AsGrouped3D GroupedObject
		{
			get => _GroupedObject;
			set 
			{
				_GroupedObject = value;
				StateChanged();
			}
		}
		
		[Export]
		public Godot.Collections.Dictionary<string, Godot.Collections.Array<AsGrouped3D>> GroupedObjects
		{
			get => _GroupedObjects;
			set 
			{
				_GroupedObjects = value;
				StateChanged();
			}
		}

		public Godot.Collections.Dictionary<Mesh, AsOptimizedMultiMeshGroup3D> OptimizedGroups = new();

		public string Name = "GlobalStates";

		private static GlobalStates _Instance;
		public static GlobalStates Singleton 
		{
			get
			{
				if( null == _Instance )
				{
					_Instance = new();
				}

				return _Instance;
			}
		}
		
		public bool Has( string name ) 
		{
			// Get all fields and properties of the class
			FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			// Check if any field or property matches the provided name
			foreach (var field in fields)
			{
				if (field.Name == name)
					return true;
			}

			foreach (var property in properties)
			{
				if (property.Name == name)
					return true;
			}

			return false;
		}
		
		public void Set( string name, Variant value ) 
		{
			// Get the type of the class
			Type type = GetType();
			
			// Get the field or property with the provided name
			FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if( value.VariantType == Variant.Type.Bool ) 
			{
				bool boolVal = value.As<bool>();
				if( boolVal ) 
				{
					LibraryStateEnum EnumVal = LibraryStateEnum.Enabled;
					property.SetValue(this, EnumVal);
				}
				else 
				{
					LibraryStateEnum EnumVal = LibraryStateEnum.Disabled;
					property.SetValue(this, EnumVal);
				}			
			}
			else
			{
				object typedValue;
				switch (value.VariantType)
				{
					case Variant.Type.Int:
						typedValue = value;
						break;
					case Variant.Type.Float:
						typedValue = (float)value;
						break;
					default:
						GD.Print("Unsupported variant type for property: " + name);
						return;
				}
				
				// Set the value of the field or property
				if (field != null)
					field.SetValue(this, typedValue);
				else if (property != null)
					property.SetValue(this, typedValue);
			}

			StateChanged();
		}
	}
}
#endif