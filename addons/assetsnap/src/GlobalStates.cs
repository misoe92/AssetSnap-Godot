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
using System.Collections.Generic;
using System.Reflection;
using AssetSnap.States;
using AssetSnap.Front.Nodes;
using Godot;

namespace AssetSnap
{
	/// <summary>
	/// This class represents the global states for the AssetSnap tool.
	/// </summary>
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
		
		/// <summary>
		/// Gets or sets the title of the currently edited object.
		/// </summary>
		[ExportCategory("General")]
		[Export]
		public string EditingTitle
		{
			get => _EditingTitle;
			set
			{
				if( value != _EditingTitle ) 
				{
					_EditingTitle = value;
					StateChanged("EditingTitle", value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the currently edited object.
		/// </summary>
		[Export]
		public Node3D EditingObject
		{
			get => _EditingObject;
			set
			{
				if (value != _EditingObject)
				{
					_EditingObject = value;
					StateChanged("EditingObject", value);

					if (_EditingObject is Node3D)
					{
						EditingObjectIsPlaced = null != _EditingObject.GetParent();
					}
					else
					{
						EditingObjectIsPlaced = false;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the currently edited object is placed in the scene.
		/// </summary>
		[Export]
		public bool EditingObjectIsPlaced
		{
			get => _EditingObjectIsPlaced;
			set
			{
				if (value != _EditingObjectIsPlaced)
				{
					_EditingObjectIsPlaced = value;
					StateChanged("EditingObjectIsPlaced", value);
				}
			}
		}

		[Export]
		public bool MultiDrop
		{
			get => _MultiDrop;
			set
			{
				if (value != _MultiDrop)
				{
					_MultiDrop = value;
					StateChanged("MultiDrop", value);
				}
			}
		}

		[Export]
		public Node CurrentScene
		{
			get => _CurrentScene;
			set
			{
				if (value != _CurrentScene)
				{
					_CurrentScene = value;
					StateChanged("CurrentScene", value);
				}
			}
		}

		[Export]
		public Library.Instance CurrentLibrary
		{
			get => _CurrentLibrary;
			set
			{
				if (value != _CurrentLibrary)
				{
					_CurrentLibrary = value;
					StateChanged("CurrentLibrary", value);

					GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
					if (null != _GlobalExplorer && null != _GlobalExplorer._Plugin && null != value)
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
				if (value != _DecalSpawned)
				{
					_DecalSpawned = value;
					StateChanged("DecalSpawned", value.ToString());
				}
			}
		}

		[Export]
		public VisibilityStateEnum DecalVisible
		{
			get => _DecalVisible;
			set
			{
				if (value != _DecalVisible)
				{
					_DecalVisible = value;
					StateChanged("DecalVisible", value.ToString());
				}
			}
		}

		[Export]
		public PlacingModeEnum PlacingMode
		{
			get => _PlacingMode;
			set
			{
				if (value != _PlacingMode)
				{
					_PlacingMode = value;
					StateChanged("PlacingMode", value.ToString());
				}
			}
		}

		[Export]
		public PlacingTypeEnum PlacingType
		{
			get => _PlacingType;
			set
			{
				if (value != _PlacingType)
				{
					_PlacingType = value;
					StateChanged("PlacingType", value.ToString());
				}
			}
		}
		
		[ExportCategory("Placement States & Values")]
		[Export]
		public LibraryStateEnum SnapToObject
		{
			get => _SnapToObject;
			set
			{
				if (value != _SnapToObject)
				{
					_SnapToObject = value;
					StateChanged("SnapToObject", value.ToString());
				}
			}
		}

		[Export]
		public LibraryStateEnum SnapToHeight
		{
			get => _SnapToHeight;
			set
			{
				if (value != _SnapToHeight)
				{
					_SnapToHeight = value;
					StateChanged("SnapToHeight", value.ToString());
				}
			}
		}

		[Export]
		public LibraryStateEnum SnapToHeightGlue
		{
			get => _SnapToHeightGlue;
			set
			{
				if (value != _SnapToHeightGlue)
				{
					_SnapToHeightGlue = value;
					StateChanged("SnapToHeightGlue", value.ToString());
				}
			}
		}

		[Export]
		public LibraryStateEnum SnapToX
		{
			get => _SnapToX;
			set
			{
				if (value != _SnapToX)
				{
					_SnapToX = value;
					StateChanged("SnapToX", value.ToString());
				}
			}
		}

		[Export]
		public LibraryStateEnum SnapToXGlue
		{
			get => _SnapToXGlue;
			set
			{
				if (value != _SnapToXGlue)
				{
					_SnapToXGlue = value;
					StateChanged("SnapToXGlue", value.ToString());
				}
			}
		}

		[Export]
		public LibraryStateEnum SnapToZ
		{
			get => _SnapToZ;
			set
			{
				if (value != _SnapToZ)
				{
					_SnapToZ = value;
					StateChanged("SnapToZ", value.ToString());
				}
			}
		}

		[Export]
		public LibraryStateEnum SnapToZGlue
		{
			get => _SnapToZGlue;
			set
			{
				if (value != _SnapToZGlue)
				{
					_SnapToZGlue = value;
					StateChanged("SnapToZGlue", value.ToString());
				}
			}
		}

		[Export]
		public LibraryStateEnum SphereCollision
		{
			get => _SphereCollision;
			set
			{
				if (value != _SphereCollision)
				{
					_SphereCollision = value;
					StateChanged("SphereCollision", value.ToString());
				}
			}
		}

		[Export]
		public LibraryStateEnum ConcaveCollision
		{
			get => _ConcaveCollision;
			set
			{
				if (value != _ConcaveCollision)
				{
					_ConcaveCollision = value;
					StateChanged("ConcaveCollision", value.ToString());
				}
			}
		}

		[Export]
		public LibraryStateEnum ConvexCollision
		{
			get => _ConvexCollision;
			set
			{
				if (value != _ConvexCollision)
				{
					_ConvexCollision = value;
					StateChanged("ConvexCollision", value.ToString());
				}
			}
		}

		[Export]
		public LibraryStateEnum ConvexClean
		{
			get => _ConvexClean;
			set
			{
				if (value != _ConvexClean)
				{
					_ConvexClean = value;
					StateChanged("ConvexClean", value.ToString());
				}
			}
		}

		[Export]
		public LibraryStateEnum ConvexSimplify
		{
			get => _ConvexSimplify;
			set
			{
				if (value != _ConvexSimplify)
				{
					_ConvexSimplify = value;
					StateChanged("ConvexSimplify", value.ToString());
				}
			}
		}

		[Export]
		public LibraryStateEnum LevelOfDetailsState
		{
			get => _LevelOfDetailsState;
			set
			{
				if (value != _LevelOfDetailsState)
				{
					_LevelOfDetailsState = value;
					StateChanged("LevelOfDetailsState", value.ToString());
				}
			}
		}
		
		[Export]
		public int SnapLayer
		{
			get => _SnapLayer;
			set
			{
				if (value != _SnapLayer)
				{
					_SnapLayer = value;
					StateChanged("SnapLayer", value);
				}
			}
		}

		[Export]
		public float SnapToObjectOffsetXValue
		{
			get => _SnapToObjectOffsetXValue;
			set
			{
				if (value != _SnapToObjectOffsetXValue)
				{
					_SnapToObjectOffsetXValue = value;
					StateChanged("SnapToObjectOffsetXValue", value);
				}
			}
		}

		[Export]
		public float SnapToObjectOffsetZValue
		{
			get => _SnapToObjectOffsetZValue;
			set
			{
				if (value != _SnapToObjectOffsetZValue)
				{
					_SnapToObjectOffsetZValue = value;
					StateChanged("SnapToObjectOffsetZValue", value);
				}
			}
		}

		[Export]
		public float SnapToHeightValue
		{
			get => _SnapToHeightValue;
			set
			{
				if (value != _SnapToHeightValue)
				{
					_SnapToHeightValue = value;
					StateChanged("SnapToHeightValue", value);
				}
			}
		}

		[Export]
		public float SnapToXValue
		{
			get => _SnapToXValue;
			set
			{
				if (value != _SnapToXValue)
				{
					_SnapToXValue = value;
					StateChanged("SnapToXValue", value);
				}
			}
		}

		[Export]
		public float SnapToZValue
		{
			get => _SnapToZValue;
			set
			{
				if (value != _SnapToZValue)
				{
					_SnapToZValue = value;
					StateChanged("SnapToZValue", value);
				}
			}
		}

		[Export]
		public float DragSizeOffset
		{
			get => _DragSizeOffset;
			set
			{
				if (value != _DragSizeOffset)
				{
					_DragSizeOffset = value;
					StateChanged("DragSizeOffset", value);
				}
			}
		}
		
		[Export]
		public float LevelOfDetails
		{
			get => _LevelOfDetails;
			set
			{
				if (value != _LevelOfDetails)
				{
					_LevelOfDetails = value;
					StateChanged("LevelOfDetails", value);
				}
			}
		}
		
		[Export]
		public float VisibilityRangeBegin
		{
			get => _VisibilityRangeBegin;
			set
			{
				if (value != _VisibilityRangeBegin)
				{
					_VisibilityRangeBegin = value;
					StateChanged("VisibilityRangeBegin", value);
				}
			}
		}
		
		[Export]
		public float VisibilityRangeBeginMargin
		{
			get => _VisibilityRangeBeginMargin;
			set
			{
				if (value != _VisibilityRangeBeginMargin)
				{
					_VisibilityRangeBeginMargin = value;
					StateChanged("VisibilityRangeBeginMargin", value);
				}
			}
		}
		
		[Export]
		public float VisibilityRangeEnd
		{
			get => _VisibilityRangeEnd;
			set
			{
				if (value != _VisibilityRangeEnd)
				{
					_VisibilityRangeEnd = value;
					StateChanged("VisibilityRangeEnd", value);
				}
			}
		}
		
		[Export]
		public float VisibilityRangeEndMargin
		{
			get => _VisibilityRangeEndMargin;
			set
			{
				if (value != _VisibilityRangeEndMargin)
				{
					_VisibilityRangeEndMargin = value;
					StateChanged("VisibilityRangeEndMargin", value);
				}
			}
		}
		
		[Export]
		public string VisibilityFadeMode
		{
			get => _VisibilityFadeMode;
			set
			{
				if (value != _VisibilityFadeMode)
				{
					_VisibilityFadeMode = value;
					StateChanged("VisibilityFadeMode", value);
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
				if (value != _BoundarySpawned)
				{
					_BoundarySpawned = value;
					StateChanged("BoundarySpawned", value.ToString());
				}
			}
		}
		
		[ExportCategory("Group")]
		[Export]
		public SnapPosition GroupSnapsTo
		{
			get => _GroupSnapsTo;
			set
			{
				_GroupSnapsTo = value;
				StateChanged("GroupSnapsTo", value.ToString());
			}
		}

		[Export]
		public GroupResource Group
		{
			get => _Group;
			set
			{
				_Group = value;
				StateChanged("Group", value);
			}
		}

		[Export]
		public AsGrouped3D GroupedObject
		{
			get => _GroupedObject;
			set
			{
				_GroupedObject = value;
				StateChanged("GroupedObject", value);
			}
		}

		[Export]
		public Godot.Collections.Dictionary<string, Godot.Collections.Array<AsGrouped3D>> GroupedObjects
		{
			get => _GroupedObjects;
			set
			{
				_GroupedObjects = value;
				StateChanged("GroupedObjects", value);
			}
		}
		
		public static GlobalStates Singleton
		{
			get
			{
				if (null == _Instance)
				{
					_Instance = new();
				}

				return _Instance;
			}
		}
		
		public string Name = "GlobalStates";
		public Godot.Collections.Dictionary<Mesh, Godot.Collections.Array<AsOptimizedMultiMeshGroup3D>> OptimizedGroups = new();
		public List<SnapAngleEnums> BoundaryActiveAngles { get; set; } = new List<SnapAngleEnums>();
		
		private static GlobalStates _Instance;
	
		/** Library Values **/
		private string _VisibilityFadeMode = "Use project default";
		private int _SnapLayer = 0;
		private float _SnapToObjectOffsetXValue = 0;
		private float _SnapToObjectOffsetZValue = 0;
		private float _SnapToHeightValue = 0;
		private float _SnapToXValue = 0;
		private float _SnapToZValue = 0;
		private float _DragSizeOffset = 0;
		private float _LevelOfDetails = 0;
		private float _VisibilityRangeBegin = 0;
		private float _VisibilityRangeBeginMargin = 0;
		private float _VisibilityRangeEnd = 0;
		private float _VisibilityRangeEndMargin = 0;


		/** Decal States **/
		private string _EditingTitle = "None";
		private bool _EditingObjectIsPlaced = false;
		private bool _MultiDrop = false;
		private SpawnStateEnum _DecalSpawned = SpawnStateEnum.Null;
		private VisibilityStateEnum _DecalVisible = VisibilityStateEnum.Hidden;
		private PlacingModeEnum _PlacingMode = PlacingModeEnum.Model;
		private PlacingTypeEnum _PlacingType = PlacingTypeEnum.Simple;
		private Node3D _EditingObject = null;
		private Node _CurrentScene = null;
		private Library.Instance _CurrentLibrary = null;

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
		private LibraryStateEnum _LevelOfDetailsState = LibraryStateEnum.Disabled;

		private SnapPosition _GroupSnapsTo = SnapPosition.Middle;
		private GroupResource _Group;
		private AsGrouped3D _GroupedObject;
		private Godot.Collections.Dictionary<string, Godot.Collections.Array<AsGrouped3D>> _GroupedObjects = new();

		/// <summary>
		/// Sets the value associated with a given key.
		/// </summary>
		/// <param name="name">The name of the key.</param>
		/// <param name="value">The value to set.</param>
		public void Set(string name, Variant value)
		{
			// Get the type of the class
			Type type = GetType();

			// Get the field or property with the provided name
			FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (null == property && null == field)
			{
				return;
			}

			if (value.VariantType == Variant.Type.Bool)
			{
				bool boolVal = value.As<bool>();
				if (boolVal)
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
		}
		
		/// <summary>
		/// Retrieves the value associated with a given key.
		/// </summary>
		/// <param name="name">The name of the key.</param>
		/// <returns>The value associated with the key.</returns>
		public Variant Key(string name)
		{
			// Get the type of the class
			Type type = GetType();

			// Get the field or property with the provided name
			FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			object value = "";

			if (null == property && null == field)
			{
				return Variant.CreateFrom( "" );
			}

			if( null != field ) {
				value = field.GetValue( this );
				
				if( value is string stringVal) 
				{
					return stringVal;
				}
				else if( value is bool boolVal)
				{
					return boolVal;
				}
				else if( value is float floatVal)
				{
					return floatVal;
				}
				else if( value is int intVal)
				{
					return intVal;
				}
			}
	
			if( null != property ) {
				value = property.GetValue( this );
				if( value is string stringVal) 
				{
					return stringVal;
				}
				else if( value is bool boolVal)
				{
					return boolVal;
				}
				else if( value is float floatVal)
				{
					return floatVal;
				}
				else if( value is int intVal)
				{
					return intVal;
				}
			}
	
			return "";
		}
		
		/// <summary>
		/// Checks if the class has a field or property with the given name.
		/// </summary>
		/// <param name="name">The name of the field or property to check.</param>
		/// <returns>True if the class has a field or property with the given name, otherwise false.</returns>
		public bool Has(string name)
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
		
		/// <summary>
		/// Checks if the value associated with a key matches the provided value.
		/// </summary>
		/// <param name="key">The key to check.</param>
		/// <param name="value">The value to compare.</param>
		/// <returns>True if the value associated with the key matches the provided value, otherwise false.</returns>
		public bool Is(string key, Variant value) 
		{
			Variant KeyValue = Key(key);
			Variant.Type ValueType = value.VariantType;
			
			if( ValueType != KeyValue.VariantType ) 
			{
				// Not the same type, just return early.
				return false;
			}
			
			if( KeyValue.VariantType == Variant.Type.String) 
			{
				return KeyValue.As<string>() == value.As<string>();
			}
			else if( KeyValue.VariantType == Variant.Type.Bool )
			{
				return KeyValue.As<bool>() == value.As<bool>();
			}
			else if( KeyValue.VariantType == Variant.Type.Float )
			{
				return KeyValue.As<float>() == value.As<float>();
			}
			else if( KeyValue.VariantType == Variant.Type.Int )
			{
				return KeyValue.As<int>() == value.As<int>();
			}

			return false;
		}
	}
}

#endif