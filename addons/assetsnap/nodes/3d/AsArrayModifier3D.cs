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
using AssetSnap.Front.Nodes;
using AssetSnap.Nodes;
using AssetSnap.States;
using AssetSnap.Static;
using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class AsArrayModifier3D : AsGroup3D
{
	private Node _Duplicates;
	public AsStaticBody3D _Parent;
	private string _Name;
	private string _DuplicateType;
	private bool Initialized = false;

	[ExportGroup("Settings")]
	[ExportSubgroup("Modifier")]

	[Export]
	public string ArrayName
	{
		get => _Name;
		set
		{
			base.Name = value;
			_Name = value;
		}
	}

	[Export]
	public string DuplicateType
	{
		get => _DuplicateType;
		set
		{
			_DuplicateType = value;
		}
	}

	[Export]
	public Node Duplicates
	{
		get => _Duplicates;
		set
		{
			_Duplicates = value;
			GD.Print(_Duplicates is AsMeshInstance3D);
		}
	}

	[ExportSubgroup("Mesh")]
	private Mesh _Mesh;

	[Export]
	public Mesh Mesh
	{
		get => _Mesh;
		set
		{
			_Mesh = value;
		}
	}

	private Godot.Collections.Array<Mesh> _Meshes;

	[Export]
	public Godot.Collections.Array<Mesh> Meshes
	{
		get => _Meshes;
		set
		{
			_Meshes = value;
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

	[ExportSubgroup("Optimization")]
	private bool _UseMultiMesh = false;

	[Export]
	public bool UseMultiMesh
	{
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
	public bool ForceCollisions
	{
		get => _ForceCollisions;
		set
		{
			_ForceCollisions = value;
			UpdateArray();
		}
	}
	private bool _NoCollisions = false;

	[Export]
	public bool NoCollisions
	{
		get => _NoCollisions;
		set
		{
			_NoCollisions = value;
			UpdateArray();
		}
	}

	[Export]
	public bool UseSphere
	{
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
	public bool UseConvexPolygon
	{
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
	public bool UseConvexClean
	{
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
	public bool UseConvexSimplify
	{
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
	public bool UseConcavePolygon
	{
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
	public bool OffsetBySize
	{
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
	public bool OffsetByXAngle
	{
		get => _OffsetByXAngle;
		set
		{
			_OffsetByXAngle = value;
			UpdateArray();
		}
	}

	private bool _OffsetByZAngle = false;

	[Export]
	public bool OffsetByZAngle
	{
		get => _OffsetByZAngle;
		set
		{
			_OffsetByZAngle = value;
			UpdateArray();
		}
	}

	private bool _ReverseOffsetAngle = false;

	[Export]
	public bool ReverseOffsetAngle
	{
		get => _ReverseOffsetAngle;
		set
		{
			_ReverseOffsetAngle = value;
			UpdateArray();
		}
	}

	private int _Amount = 1;

	[Export]
	public int Amount
	{
		get => _Amount;
		set
		{
			_Amount = value;
			UpdateArray();
		}
	}

	private float _OffsetX = 0.0f;
	[Export]
	public float OffsetX
	{
		get => _OffsetX;
		set
		{
			_OffsetX = value;
			UpdateArray();
		}
	}

	private float _OffsetY = 0.0f;
	[Export]
	public float OffsetY
	{
		get => _OffsetY;
		set
		{
			_OffsetY = value;
			UpdateArray();
		}
	}

	private float _OffsetZ = 0.0f;
	[Export]
	public float OffsetZ
	{
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
		_SceneRoot = Plugin.Singleton.GetTree().EditedSceneRoot;

		base._Ready();

		if (GetChildCount() == 0)
		{
			UpdateArray();
		}
	}

	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		if (_OffsetBySize == false && property.ContainsKey("name") && (property["name"].As<string>() == "OffsetByXAngle" || property["name"].As<string>() == "OffsetByZAngle"))
		{
			var usage = PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if (_OffsetBySize == true && property.ContainsKey("name") && (property["name"].As<string>() == "OffsetByXAngle" || property["name"].As<string>() == "OffsetByZAngle"))
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
			property["usage"] = (int)usage;
		}

		if ((UseConvexPolygon == true || UseConcavePolygon == true) && property.ContainsKey("name") && (property["name"].As<string>() == "UseSphere" || property["name"].As<string>() == "UseSphere"))
		{
			var usage = PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if (UseConvexPolygon == false && UseConcavePolygon == false && property.ContainsKey("name") && (property["name"].As<string>() == "UseSphere" || property["name"].As<string>() == "UseSphere"))
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
			property["usage"] = (int)usage;
		}

		if (UseConvexPolygon == false && property.ContainsKey("name") && (property["name"].As<string>() == "UseConvexClean" || property["name"].As<string>() == "UseConvexClean"))
		{
			var usage = PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if (UseConvexPolygon == true && property.ContainsKey("name") && (property["name"].As<string>() == "UseConvexClean" || property["name"].As<string>() == "UseConvexClean"))
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
			property["usage"] = (int)usage;
		}

		if (UseConvexPolygon == false && property.ContainsKey("name") && (property["name"].As<string>() == "UseConvexSimplify" || property["name"].As<string>() == "UseConvexSimplify"))
		{
			var usage = PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if (UseConvexPolygon == true && property.ContainsKey("name") && (property["name"].As<string>() == "UseConvexSimplify" || property["name"].As<string>() == "UseConvexSimplify"))
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
			property["usage"] = (int)usage;
		}

		if ((UseSphere == true || UseConcavePolygon == true) && property.ContainsKey("name") && (property["name"].As<string>() == "UseConvexPolygon" || property["name"].As<string>() == "UseConvexPolygon"))
		{
			var usage = PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if (UseSphere == false && UseConcavePolygon == false && property.ContainsKey("name") && (property["name"].As<string>() == "UseConvexPolygon" || property["name"].As<string>() == "UseConvexPolygon"))
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
			property["usage"] = (int)usage;
		}

		if ((UseSphere == true || UseConvexPolygon == true) && property.ContainsKey("name") && (property["name"].As<string>() == "UseConcavePolygon" || property["name"].As<string>() == "UseConcavePolygon"))
		{
			var usage = PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if (UseSphere == false && UseConvexPolygon == false && property.ContainsKey("name") && (property["name"].As<string>() == "UseConcavePolygon" || property["name"].As<string>() == "UseConcavePolygon"))
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
		if (false == Initialized || null == Duplicates)
		{
			return;
		}

		if (GetParent() == null)
		{
			GD.PushError("Parent aint set yet");
			return;
		}

		if (null == GetTree())
		{
			GD.PushError("No tree was found");
			return;
		}

		if (GetChildCount() != 0)
		{
			ClearCurrentChildren();
		}

		StatesUtils.Get().ConcaveCollision = GlobalStates.LibraryStateEnum.Disabled;
		StatesUtils.Get().ConvexCollision = GlobalStates.LibraryStateEnum.Disabled;
		StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Disabled;
		StatesUtils.Get().ConvexSimplify = GlobalStates.LibraryStateEnum.Disabled;
		StatesUtils.Get().SphereCollision = GlobalStates.LibraryStateEnum.Disabled;

		// Apply options
		if (UseSphere)
		{
			StatesUtils.Get().SphereCollision = GlobalStates.LibraryStateEnum.Enabled;
		}

		if (UseConcavePolygon)
		{
			StatesUtils.Get().ConcaveCollision = GlobalStates.LibraryStateEnum.Enabled;
		}

		if (UseConvexPolygon)
		{
			StatesUtils.Get().ConvexCollision = GlobalStates.LibraryStateEnum.Enabled;

			if (UseConvexClean)
			{
				StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Enabled;
			}

			if (UseConvexSimplify)
			{
				StatesUtils.Get().ConvexSimplify = GlobalStates.LibraryStateEnum.Enabled;
			}
		}

		if (UseMultiMesh == false)
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
		try
		{
			for (int i = 0; i < _Amount; i++)
			{
				Node3D _Model = Duplicates.Duplicate() as Node3D;

				if (_Model == null)
				{
					throw new Exception("Model is invalid");
				}

				float ExtraOffsetX = 0;
				float ExtraOffsetY = 0;
				float ExtraOffsetZ = 0;

				Transform3D transform = new Transform3D(Basis.Identity, Vector3.Zero);

				if (OffsetBySize)
				{
					Aabb ModelAabb = NodeUtils.CalculateNodeAabb(Duplicates);
					if (OffsetByXAngle)
					{
						ExtraOffsetX += ModelAabb.Size.X;
					}

					if (OffsetByZAngle)
					{
						ExtraOffsetZ += ModelAabb.Size.Z;
					}
				}

				transform = ApplyModelTransforms(transform, new Vector3(ExtraOffsetX, ExtraOffsetY, ExtraOffsetZ), i);
				if (_Model is AsMeshInstance3D)
				{
					ApplyModelMeta(_Model);
				}

				if (_Model is AsNode3D)
				{
					foreach (Node child in _Model.GetChildren())
					{
						ApplyModelMeta(child);
					}
				}

				AddChild(_Model, true);
				_Model.Owner = GetTree().EditedSceneRoot;
				_Model.Transform = transform;

				if (_Model is AsMeshInstance3D meshInstance3D)
				{
					meshInstance3D.SetLibraryName(InstanceLibrary);
					meshInstance3D.SetIsFloating(false);
					meshInstance3D.GetCollisionBody().Initialize();
					meshInstance3D.UpdateViewability();
				}

				if (_Model is AsNode3D node3d)
				{
					node3d.SetLibraryName(InstanceLibrary);
					node3d.SetIsFloating(false);

					foreach (Node child in node3d.GetChildren())
					{
						child.Owner = GetTree().EditedSceneRoot;

						if (child is AsMeshInstance3D childMeshInstance3D)
						{
							childMeshInstance3D.SetLibraryName(InstanceLibrary);
							childMeshInstance3D.SetIsFloating(false);
							childMeshInstance3D.GetCollisionBody().Initialize();
							childMeshInstance3D.UpdateViewability();
						}
					}
				}

				// Randomnize();
			}
		}
		catch (Exception e)
		{
			GD.PushWarning(e.Message);
		}
	}

	private void CreateMultiArray()
	{
		// Check if duplicated item is AsMeshInstance
		// If yes SimpleMultiMeshArray
		// If no AdvancedMultiMeshArray 

		if (Duplicates is AsMeshInstance3D asMeshInstance3D)
		{
			// Simple MultiMeshInstance3D
			_SimpleMultiMeshArray(asMeshInstance3D);
		}
		else
		{
			_AdvancedMultiMeshArray();
		}

		// Randomnize();
	}

	private void _SimpleMultiMeshArray(AsMeshInstance3D asMeshInstance3D)
	{
		MultiMesh _MultiMesh = new()
		{
			TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
			Mesh = asMeshInstance3D.Mesh,
			InstanceCount = _Amount,
		};

		AsMultiMeshInstance3D _MultiMeshInstance = new()
		{
			Multimesh = _MultiMesh
		};
		ApplyModelMeta(_MultiMeshInstance);

		List<Vector3> positions = new List<Vector3>();

		for (int i = 0; i < _Amount; i++)
		{
			Transform3D transform = new Transform3D(Basis.Identity, Vector3.Zero);

			float ExtraOffsetX = 0;
			float ExtraOffsetY = 0;
			float ExtraOffsetZ = 0;

			if (OffsetBySize)
			{
				Aabb ModelAabb = NodeUtils.CalculateNodeAabb(asMeshInstance3D);

				if (OffsetByXAngle)
				{
					ExtraOffsetX += ModelAabb.Size.X;
				}

				if (OffsetByZAngle)
				{
					ExtraOffsetZ += ModelAabb.Size.Z;
				}
			}

			transform = ApplyModelTransforms(transform, new Vector3(ExtraOffsetX, ExtraOffsetY, ExtraOffsetZ), i);
			_MultiMesh.SetInstanceTransform(i, transform);
			positions.Add(transform.Origin);
		}

		AddChild(_MultiMeshInstance);
		_MultiMeshInstance.Owner = _SceneRoot;
		_MultiMeshInstance.Name = _Name + "/multiMesh";
	}

	private void _AdvancedMultiMeshArray()
	{
		if (Duplicates is AsNode3D asNode3D)
		{
			// Childable
			foreach (MeshInstance3D child in asNode3D.GetChildren())
			{
				MultiMesh _MultiMesh = new()
				{
					TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
					Mesh = child.Mesh,
					InstanceCount = _Amount,
				};

				AsMultiMeshInstance3D _MultiMeshInstance = new()
				{
					Multimesh = _MultiMesh
				};
				ApplyModelMeta(_MultiMeshInstance);

				List<Vector3> positions = new List<Vector3>();

				for (int i = 0; i < _Amount; i++)
				{
					Transform3D transform = new Transform3D(Basis.Identity, Vector3.Zero);

					float ExtraOffsetX = 0;
					float ExtraOffsetY = 0;
					float ExtraOffsetZ = 0;

					if (OffsetBySize)
					{
						Aabb ModelAabb = NodeUtils.CalculateNodeAabb(asNode3D);

						if (OffsetByXAngle)
						{
							ExtraOffsetX += ModelAabb.Size.X;
						}

						if (OffsetByZAngle)
						{
							ExtraOffsetZ += ModelAabb.Size.Z;
						}
					}

					transform = ApplyModelTransforms(transform, new Vector3(ExtraOffsetX, ExtraOffsetY, ExtraOffsetZ), i);
					_MultiMesh.SetInstanceTransform(i, transform);
					positions.Add(transform.Origin);
				}

				AddChild(_MultiMeshInstance);
				_MultiMeshInstance.Owner = _SceneRoot;
				_MultiMeshInstance.Name = _InstanceName + "/multiMesh";
			}
		}
	}

	private Transform3D ApplyModelTransforms(Transform3D transform, Vector3 ExtraOffset, int i)
	{
		if (ReverseOffsetAngle)
		{
			transform.Origin.X -= transform.Origin.X + (OffsetX * i) + (ExtraOffset.X * i);
			transform.Origin.Y -= transform.Origin.Y + (OffsetY * i) + (ExtraOffset.Y * i);
			transform.Origin.Z -= transform.Origin.Z + (OffsetZ * i) + (ExtraOffset.Z * i);
		}
		else
		{
			transform.Origin.X += transform.Origin.X + (OffsetX * i) + (ExtraOffset.X * i);
			transform.Origin.Y += transform.Origin.Y + (OffsetY * i) + (ExtraOffset.Y * i);
			transform.Origin.Z += transform.Origin.Z + (OffsetZ * i) + (ExtraOffset.Z * i);
		}

		return transform;
	}

	private void ApplyModelMeta(Node _Model)
	{
		if (ForceCollisions)
		{
			_Model.SetMeta("Collision", true);
		}

		if (NoCollisions)
		{
			_Model.SetMeta("Collision", false);
		}
	}

	private bool ShouldAddCollision()
	{
		return SettingsStatic.ShouldAddCollision();
	}
}