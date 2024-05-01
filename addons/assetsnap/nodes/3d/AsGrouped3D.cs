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

using System;
using System.Collections.Generic;
using AssetSnap.Explorer;
using AssetSnap.Front.Nodes;
using AssetSnap.Nodes;
using AssetSnap.States;
using AssetSnap.Static;
using Godot;

[Tool]
public partial class AsGrouped3D : AsGroup3D
{

	private List<GroupedConnection> Connections = new();

	private string _GroupPath = "";
	private int _SnapLayer = 0;
	private float _ObjectOffsetX = 0.0f;
	private float _ObjectOffsetZ = 0.0f;	
	private float _SnapHeightValue = 0.0f;	
	private float _SnapXValue = 0.0f;
	private float _SnapZValue = 0.0f;
	private float _DistanceToTop = 0.0f;
	private float _DistanceToBottom = 0.0f;
	private float _DistanceToLeft = 0.0f;
	private float _DistanceToRight = 0.0f;
	private bool _OptimizedSpawn = false;
	private bool _SphereCollision = false;
	private bool _ConvexCollision = false;
	private bool _ConvexClean = false;
	private bool _ConvexSimplify = false;
	private bool _ConcaveCollision = false;
	private bool _SnapToObject = false;
	private bool _SnapToHeight = false;
	private bool _SnapToX = false;
	private bool _SnapToZ = false;
	private Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> _ChildOptions = new();

	[Export]
	public string GroupPath { get => _GroupPath; set { _GroupPath = value; } }

	[Export]
	public int SnapLayer { get => _SnapLayer; set { _SnapLayer = value; Update(); } }

	[Export]
	public float ObjectOffsetX { get => _ObjectOffsetX; set { _ObjectOffsetX = value; } }

	[Export]
	public float ObjectOffsetZ { get => _ObjectOffsetZ; set { _ObjectOffsetZ = value; } }

	[Export]
	public float SnapHeightValue { get => _SnapHeightValue; set { _SnapHeightValue = value; } }

	[Export]
	public float SnapXValue { get => _SnapXValue; set { _SnapXValue = value; } }

	[Export]
	public float SnapZValue { get => _SnapZValue; set { _SnapZValue = value; } }
	
	[Export]
	public float DistanceToTop { get => _DistanceToTop; set { _DistanceToTop = value; } }

	[Export]
	public float DistanceToBottom { get => _DistanceToBottom; set { _DistanceToBottom = value; } }

	[Export]
	public float DistanceToLeft { get => _DistanceToLeft; set { _DistanceToLeft = value; } }

	[Export]
	public float DistanceToRight { get => _DistanceToRight; set { _DistanceToRight = value; } }

	[Export]
	public bool OptimizedSpawn { get => _OptimizedSpawn; set { _OptimizedSpawn = value; } }

	[Export]
	public bool SphereCollision { get => _SphereCollision; set { _SphereCollision = value; Update(); } }

	[Export]
	public bool ConvexCollision { get => _ConvexCollision; set { _ConvexCollision = value; Update(); } }

	[Export]
	public bool ConvexClean { get => _ConvexClean; set { _ConvexClean = value; Update(); } }

	[Export]
	public bool ConvexSimplify { get => _ConvexSimplify; set { _ConvexSimplify = value; Update(); } }

	[Export]
	public bool ConcaveCollision { get => _ConcaveCollision; set { _ConcaveCollision = value; Update(); } }

	[Export]
	public bool SnapToObject { get => _SnapToObject; set { _SnapToObject = value; } }

	[Export]
	public bool SnapToHeight { get => _SnapToHeight; set { _SnapToHeight = value; } }

	[Export]
	public bool SnapToX { get => _SnapToX; set { _SnapToX = value; } }

	[Export]
	public bool SnapToZ { get => _SnapToZ; set { _SnapToZ = value; } }

	[Export]
	public Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> ChildOptions { get => _ChildOptions; set { _ChildOptions = value; } }

	public override void _EnterTree()
	{
		GroupResource resource = GD.Load<Resource>(GroupPath) as GroupResource;
		if (StatesUtils.Get().GroupedObjects.ContainsKey(GroupPath))
		{
			StatesUtils.Get().GroupedObjects[GroupPath].Add(this);
		}
		else
		{
			StatesUtils.Get().GroupedObjects.Add(GroupPath, new() { this });
		}

		if (OptimizedSpawn)
		{
			// Since we dont have AsMeshInstances to provide the usual snapping. We will have to use
			// position data from our paths.
			for (int i = 0; i < resource._Origins.Count; i++)
			{
				ExplorerUtils.Get().Waypoints.Register(this, resource._Origins[i], resource._Rotations[i], resource._Scales[i]);
			}
		}

		base._EnterTree();
	}

	public Aabb GetAabb()
	{
		return NodeUtils.CalculateNodeAabb(this);
	}

	/*
	** Will reload the group data and re draw the local space of the node 
	**
	** @returns void
	*/
	public void Update()
	{
		if( null == GetParent() || GetParent().Name == "Decal") 
		{
			return;
		}

		GroupResource resource = GD.Load<Resource>(GroupPath) as GroupResource;

		if (false == OptimizedSpawn)
		{
			ClearCurrentChildren();
			resource.AddChildren(this, ChildOptions);
		}
		else
		{
			ClearCurrentChildren();
			int Instanced = 0;
			for (int i = 0; i < Connections.Count; i++)
			{
				GroupedConnection connection = Connections[i];
				int index = 0;
				
				foreach( string path in resource._Paths )
				{
					if( path == connection.InstanceMesh.ResourcePath ) 
					{
						break;
					}
					else 
					{
						index += 1;
					}
				}

				if( connection.InstanceId != 0 ) 
				{
					Instanced += 1;
				}

				Transform3D transform = new Transform3D(Basis.Identity, Vector3.Zero);
				transform.Scaled(resource._Scales[index]);
				// Convert the rotation from degrees to radians
				float rotationRadiansX = Mathf.DegToRad(resource._Rotations[index].X);
				float rotationRadiansY = Mathf.DegToRad(resource._Rotations[index].Y);
				float rotationRadiansZ = Mathf.DegToRad(resource._Rotations[index].Z);

				// Create a rotation basis around each axis
				Basis rotationBasisX = Basis.Identity.Rotated(Vector3.Right, rotationRadiansX);
				Basis rotationBasisY = Basis.Identity.Rotated(Vector3.Up, rotationRadiansY);
				Basis rotationBasisZ = Basis.Identity.Rotated(Vector3.Forward, rotationRadiansZ);

				// Combine the rotation around each axis
				Basis finalRotation = rotationBasisX * rotationBasisY * rotationBasisZ;

				// Assuming you have a transform called transform
				transform.Basis = finalRotation;
				transform.Origin = Transform.Origin + new Vector3(resource._Origins[index].X, resource._Origins[index].Y, resource._Origins[index].Z);

				connection.Update(transform);
				connection.UpdateUsing(transform, ChildOptions[index]);
			}
		}
	}

	public void AddConnection(int id, AsOptimizedMultiMeshGroup3D optimizedMeshGroup, Mesh mesh)
	{
		Connections.Add(
			new OptimizedMultiMeshConnection()
			{
				InstanceId = id,
				InstanceMesh = mesh,
				OptimizedMultiMesh = optimizedMeshGroup,
				Source = this,
			}
		);
	}

	public void Clear()
	{
		ClearCurrentChildren();
	}

	public bool IsPlaced()
	{
		return GetParent() != null && GetParent().Name != "AsDecal";
	}

	private bool ShouldAddCollision()
	{
		return SettingsStatic.ShouldAddCollision();
	}

	public override void _ExitTree()
	{
		if (StatesUtils.Get().GroupedObjects.ContainsKey(GroupPath))
		{
			StatesUtils.Get().GroupedObjects[GroupPath].Remove(this);

			if (StatesUtils.Get().GroupedObjects[GroupPath].Count == 0)
			{
				StatesUtils.Get().GroupedObjects.Remove(GroupPath);
			}
		}

		base._EnterTree();
	}

}