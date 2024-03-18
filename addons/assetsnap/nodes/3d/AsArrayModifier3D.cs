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

using AssetSnap;
using AssetSnap.ASNode.MeshInstance;
using AssetSnap.Front.Nodes;
using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class AsArrayModifier3D : AsGroup3D
{
	private bool Initialized = false;
	public AsStaticBody3D _Parent;
	private string _Name;
	
	[ExportGroup("Settings")]
	[ExportSubgroup("Modifier")]
	
	[Export]
	public new string Name
	{
		get => _Name;
		set
		{
			base.Name = value;
			_Name = value;
		}
	}
	
	[ExportSubgroup("Mesh")]
	private Mesh _Mesh;
	
	[Export]
	public Mesh Mesh {
		get => _Mesh;
		set 
		{
			_Mesh = value;
		}
	}
	
	private string _InstanceName;
	
	[Export]
	public string InstanceName
	{
		get => _InstanceName;
		set
		{
			_InstanceName = value;
		}
	}
	
	private string _InstanceLibrary;
	
	[Export]
	public string InstanceLibrary 
	{
		get => _InstanceLibrary;
		set 
		{
			_InstanceLibrary = value;
		}
	}
	
	private Transform3D _InstanceTransform;
	
	[Export]
	public Transform3D InstanceTransform 
	{
		get => _InstanceTransform;
		set 
		{
			_InstanceTransform = value;
		}
	}
	
	private Vector3 _InstanceRotation;
	
	[Export]
	public Vector3 InstanceRotation 
	{
		get => _InstanceRotation;
		set 
		{
			_InstanceRotation = value;
		}
	}
	
	private Vector3 _InstanceScale;
	
	[Export]
	public Vector3 InstanceScale 
	{
		get => _InstanceScale;
		set 
		{
			_InstanceScale = value;
		}
	}
	
	[ExportSubgroup("Optimization")]
	private bool _UseMultiMesh = false;
	
	[Export]
	public bool UseMultiMesh {
		get => _UseMultiMesh;
		set 
		{
			_UseMultiMesh = value;
			UpdateArray();
		}
	}
	
	[ExportSubgroup("Collisions")]
	private bool _ForceCollisions = false;
	
	[Export]
	public bool ForceCollisions {
		get => _ForceCollisions;
		set 
		{
			_ForceCollisions = value;
			UpdateArray();
		}
	}
	private bool _NoCollisions = false;
	
	[Export]
	public bool NoCollisions {
		get => _NoCollisions;
		set 
		{
			_NoCollisions = value;
			UpdateArray();
		}
	}
	
	[Export]
	public bool UseSphere {
		get => _UseSphere;
		set 
		{
			_UseSphere = value;
			UpdateArray();
			NotifyPropertyListChanged();
		}
	}
	private bool _UseSphere = false;
	
	[Export]
	public bool UseConvexPolygon {
		get => _UseConvexPolygon;
		set 
		{
			_UseConvexPolygon = value;
			UpdateArray();
			NotifyPropertyListChanged();
		}
	}
	private bool _UseConvexPolygon = false;
	
	[Export]
	public bool UseConvexClean {
		get => _UseConvexClean;
		set 
		{
			_UseConvexClean = value;
			UpdateArray();
			NotifyPropertyListChanged();
		}
	}
	private bool _UseConvexClean = false;
	
	[Export]
	public bool UseConvexSimplify {
		get => _UseConvexSimplify;
		set 
		{
			_UseConvexSimplify = value;
			UpdateArray();
			NotifyPropertyListChanged();
		}
	}
	private bool _UseConvexSimplify = false;
	
	[Export]
	public bool UseConcavePolygon {
		get => _UseConcavePolygon;
		set 
		{
			_UseConcavePolygon = value;
			UpdateArray();
			NotifyPropertyListChanged();
		}
	}
	private bool _UseConcavePolygon = false;
	
	[ExportCategory("General")]
	private bool _OffsetBySize = true;
	
	[Export]
	public bool OffsetBySize {
		get => _OffsetBySize;
		set 
		{
			_OffsetBySize = value;
			UpdateArray();
			NotifyPropertyListChanged();
		}
	}
	
	private bool _OffsetByXAngle = true;
	
	[Export]
	public bool OffsetByXAngle {
		get => _OffsetByXAngle;
		set 
		{
			_OffsetByXAngle = value;
			UpdateArray();
		}
	}
	
	private bool _OffsetByZAngle = false;
	
	[Export]
	public bool OffsetByZAngle {
		get => _OffsetByZAngle;
		set 
		{
			_OffsetByZAngle = value;
			UpdateArray();
		}
	}
	
	private bool _ReverseOffsetAngle = false;
	
	[Export]
	public bool ReverseOffsetAngle {
		get => _ReverseOffsetAngle;
		set 
		{
			_ReverseOffsetAngle = value;
			UpdateArray();
		}
	}
	
	private int _Amount = 1;
	
	[Export]
	public int Amount {
		get => _Amount;
		set 
		{
			_Amount = value;

			UpdateArray();
		}
	}
	
	private float _OffsetX = 0.0f;
	[Export]
	public float OffsetX {
		get => _OffsetX;
		set 
		{
			_OffsetX = value;
			
			UpdateArray();
		}
	}
	
	private float _OffsetY = 0.0f;
	[Export]
	public float OffsetY {
		get => _OffsetY;
		set 
		{
			_OffsetY = value;
			
			UpdateArray();
		}
	}
	
	private float _OffsetZ = 0.0f;
	[Export]
	public float OffsetZ {
		get => _OffsetZ;
		set 
		{
			_OffsetZ = value;
			
			UpdateArray();
		}
	}

	public override void _Ready()
	{
		Initialized = true;
		base._Ready();
	}

	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		if( _OffsetBySize == false && property.ContainsKey("name") && ( property["name"].As<string>() == "OffsetByXAngle" || property["name"].As<string>() == "OffsetByZAngle"  ) ) 
		{
			var usage = PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if( _OffsetBySize == true && property.ContainsKey("name") && ( property["name"].As<string>() == "OffsetByXAngle" || property["name"].As<string>() == "OffsetByZAngle"  ) ) 
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
			property["usage"] = (int)usage;
		}
		
		if( ( UseConvexPolygon == true || UseConcavePolygon == true ) && property.ContainsKey("name") && ( property["name"].As<string>() == "UseSphere" || property["name"].As<string>() == "UseSphere" ) ) 
		{
			var usage = PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if( UseConvexPolygon == false && UseConcavePolygon == false && property.ContainsKey("name") && ( property["name"].As<string>() == "UseSphere" || property["name"].As<string>() == "UseSphere" ) ) 
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
			property["usage"] = (int)usage;
		}
		
		if( UseConvexPolygon == false && property.ContainsKey("name") && ( property["name"].As<string>() == "UseConvexClean" || property["name"].As<string>() == "UseConvexClean" ) ) 
		{
			var usage = PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if( UseConvexPolygon == true && property.ContainsKey("name") && ( property["name"].As<string>() == "UseConvexClean" || property["name"].As<string>() == "UseConvexClean" ) ) 
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
			property["usage"] = (int)usage;
		}
		
		if( UseConvexPolygon == false && property.ContainsKey("name") && ( property["name"].As<string>() == "UseConvexSimplify" || property["name"].As<string>() == "UseConvexSimplify" ) ) 
		{
			var usage = PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if( UseConvexPolygon == true && property.ContainsKey("name") && ( property["name"].As<string>() == "UseConvexSimplify" || property["name"].As<string>() == "UseConvexSimplify" ) ) 
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
			property["usage"] = (int)usage;
		}
		
		if( ( UseSphere == true || UseConcavePolygon == true ) && property.ContainsKey("name") && ( property["name"].As<string>() == "UseConvexPolygon" || property["name"].As<string>() == "UseConvexPolygon" ) ) 
		{
			var usage = PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if( UseSphere == false && UseConcavePolygon == false && property.ContainsKey("name") && ( property["name"].As<string>() == "UseConvexPolygon" || property["name"].As<string>() == "UseConvexPolygon" ) ) 
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
			property["usage"] = (int)usage;
		}
		
		if( ( UseSphere == true || UseConvexPolygon == true ) && property.ContainsKey("name") && ( property["name"].As<string>() == "UseConcavePolygon" || property["name"].As<string>() == "UseConcavePolygon" ) ) 
		{
			var usage = PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if( UseSphere == false && UseConvexPolygon == false && property.ContainsKey("name") && ( property["name"].As<string>() == "UseConcavePolygon" || property["name"].As<string>() == "UseConcavePolygon" ) ) 
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
			property["usage"] = (int)usage;
		}
		
		base._ValidateProperty(property);
	}
	
	public void Update()
	{
		UpdateArray();
	}
	
	private void UpdateArray()
	{
		if( false == Initialized || _Mesh == null ) 
		{
			return;
		}
			
		ClearCurrentChildren();
		
		if( UseMultiMesh == false ) 
		{
			CreateSimpleArray();
		}
		else 
		{
			CreateMultiArray();
		}
	}
	
	private void CreateSimpleArray()
	{
		if( GetParent() == null ) 
		{
			GD.PushError("Parent aint set yet");
			return;
		}
		
		if( _Mesh == null ) 
		{
			throw new Exception("Mesh is invalid");
		}
		
		for( int i = 0; i < _Amount; i++) 
		{
			AssetSnap.Front.Nodes.AsMeshInstance3D _Model = new()
			{
				Mesh = _Mesh,
				Transform = InstanceTransform,
				Scale = InstanceScale,
				RotationDegrees = InstanceRotation,
			}; 
						
			if( _Model == null ) 
			{
				throw new Exception("Model is invalid");
			}

			_Model.SetLibraryName(InstanceLibrary);
			
			Transform3D Trans = InstanceTransform;

			float ExtraOffsetX = 0;
			float ExtraOffsetY = 0;
			float ExtraOffsetZ = 0;
			
			if( OffsetBySize ) 
			{
				Aabb ModelAabb = _Mesh.GetAabb();
				
				if( OffsetByXAngle && ReverseOffsetAngle == false ) 
				{
					ExtraOffsetX += ModelAabb.Size.X;					
				}
				
				if( OffsetByZAngle && ReverseOffsetAngle == false ) 
				{
					ExtraOffsetZ += ModelAabb.Size.Z;					
				}
				
				if( OffsetByXAngle && ReverseOffsetAngle == true ) 
				{
					ExtraOffsetX -= ModelAabb.Size.X;					
				}
				
				if( OffsetByZAngle && ReverseOffsetAngle == true ) 
				{
					ExtraOffsetZ -= ModelAabb.Size.Z;					
				}
			}

			Trans.Origin.X = ( OffsetX * i) + ( ExtraOffsetX * i ) + Trans.Origin.X;
			Trans.Origin.Y = ( OffsetY * i) + ( ExtraOffsetY * i ) + Trans.Origin.Y;
			Trans.Origin.Z = ( OffsetZ * i) + ( ExtraOffsetZ * i ) + Trans.Origin.Z;
			
			AddChild(_Model, true);
			if( GetTree() != null && _Model.Owner == null ) 
			{
				_Model.Owner = _SceneRoot;
			}

			_Model.Name = _Model.Name + "/" + i;
			_Model.Transform = Trans;
			
			if( ShouldAddCollision() && _NoCollisions == false || _ForceCollisions ) 
			{
				_AddCollisions(_Model, i);
			}
			
			Randomnize();
		}
	}
	
	private void CreateMultiArray()
	{
		MultiMesh _MultiMesh = new()
		{
			TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
			Mesh = _Mesh,
			InstanceCount = _Amount,
		};

		MultiMeshInstance3D _MultiMeshInstance = new()
		{
			Multimesh = _MultiMesh
		};
		
		List<Vector3> positions = new List<Vector3>();
	
		for( int i = 0; i < _Amount; i++) 
		{
			Transform3D Trans = InstanceTransform;

			float ExtraOffsetX = 0;
			float ExtraOffsetY = 0;
			float ExtraOffsetZ = 0;
			
			if( OffsetBySize ) 
			{
				Aabb ModelAabb = _Mesh.GetAabb();
				
				if( OffsetByXAngle ) 
				{
					ExtraOffsetX += ModelAabb.Size.X;					
				}
				
				if( OffsetByZAngle ) 
				{
					ExtraOffsetZ += ModelAabb.Size.Z;					
				}
			}

			Trans.Origin.X = ( OffsetX * i) + ( ExtraOffsetX * i ) + Trans.Origin.X;
			Trans.Origin.Y = ( OffsetY * i) + ( ExtraOffsetY * i ) + Trans.Origin.Y;
			Trans.Origin.Z = ( OffsetZ * i) + ( ExtraOffsetZ * i ) + Trans.Origin.Z;

			_MultiMesh.SetInstanceTransform(i, Trans);
			positions.Add(Trans.Origin);
		}

		AddChild(_MultiMeshInstance);
		_MultiMeshInstance.Owner = GetTree().EditedSceneRoot;
		_MultiMeshInstance.Name = _InstanceName + "/multiMesh";
		
		if( ShouldAddCollision() && _NoCollisions == false || _ForceCollisions ) 
		{
			Aabb _aabb = Mesh.GetAabb();
			_AddMultiCollisions(_MultiMeshInstance, positions, _aabb);
		}
		
		Randomnize();
	}
	
	private void _AddCollisions(Node3D _model, int index)
	{
		GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
		
		// Only add collisions to models
		if( _model.HasMeta("AsModel") == false ) 
		{
			return;
		}
		
		if( _model is AssetSnap.Front.Nodes.AsMeshInstance3D model ) 
		{
			AsStaticBody3D _Body = new()
			{
				Name = "AsBody@idx-" + GetChildCount(),
				Transform = model.Transform,
				
				Mesh = model.Mesh,
				MeshName = model.Name,
				InstanceTransform = model.Transform,
				InstanceScale = InstanceScale,
				InstanceRotation = model.RotationDegrees,
				InstanceLibrary = InstanceLibrary,
				InstanceSpawnSettings = model.SpawnSettings,
			};
	
			AddChild(_Body, true);
			_Body.Owner = _SceneRoot;

			int typeState = 0;
			int argState = 0;
			
			if( UseConvexPolygon ) 
			{
				typeState = 1;
				
				if(UseConvexClean && false == UseConvexSimplify ) 
				{
					argState = 1;	
				}
				else if( false == UseConvexClean && UseConvexSimplify ) 
				{
					argState = 2;	
				}
				else if( UseConvexClean && UseConvexSimplify ) 
				{
					argState = 3;	
				}
			}
			
			if( UseConcavePolygon ) 
			{
				typeState = 2;
			}
			
			if( UseSphere ) 
			{
				typeState = 3;
			} 
			
			_Body.Initialize(typeState, argState);
			model.GetParent().RemoveChild(model);
		}
	}
	
	private void _AddMultiCollisions(MultiMeshInstance3D _MultiMeshInstance, List<Vector3> _Positions, Aabb ModelAabb )
	{
		GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			
		for( int i = 0; i < _Positions.Count; i++ ) 
		{
			Vector3 _Pos = _Positions[i];
			Transform3D _Trans = Transform3D.Identity;

			_Trans.Origin = new Vector3(_Pos.X, _Pos.Y, _Pos.Z);
			AsStaticBody3D _Body = new()
			{
				Transform = _Trans,
				UsingMultiMesh = true,
										
				Mesh = _Mesh,
				MeshName = _Mesh.ResourceName,
				InstanceLibrary = InstanceLibrary,
				InstanceScale = _MultiMeshInstance.Multimesh.GetInstanceTransform(i).Basis.Scale
			};

			AddChild(_Body);
			_Body.Owner = _GlobalExplorer._Plugin.GetTree().EditedSceneRoot;
			
			int typeState = 0;
			int argState = 0;
			
			if( UseConvexPolygon ) 
			{
				typeState = 1;
				
				if(UseConvexClean && false == UseConvexSimplify ) 
				{
					argState = 1;	
				}
				else if( false == UseConvexClean && UseConvexSimplify ) 
				{
					argState = 2;	
				}
				else if( UseConvexClean && UseConvexSimplify ) 
				{
					argState = 3;	
				}
			}
			
			if( UseConcavePolygon ) 
			{
				typeState = 2;
			}
			
			if( UseSphere ) 
			{
				typeState = 3;
			}

			_Body.Initialize(typeState, argState);
		}
	}
	
	private bool ShouldAddCollision()
	{
		GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
		bool AddCollisions = _GlobalExplorer.Settings.GetKey("add_collisions").As<bool>();

		return AddCollisions;
	}
 
	public override void _ExitTree()
	{
		ClearCurrentChildren();
		
		_Parent = null;
		Mesh = null;
		
		base._ExitTree();
	}
}