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

using System.Collections.Generic;
using AssetSnap;
using AssetSnap.Front.Nodes;
using Godot;

[Tool]
public partial class AsGrouped3D : AsGroup3D
{

	private List<GroupedConnection> Connections = new();


	[Export]
	public string GroupPath { get; set; } = "";

	[Export]
	public int SnapLayer { get; set; } = 0;
	
	[Export]
	public float ObjectOffsetX { get; set; } = 0.0f;
	
	[Export]
	public float ObjectOffsetZ { get; set; } = 0.0f;
	
	[Export]
	public float SnapHeightValue { get; set; } = 0.0f;
	
	[Export]
	public float SnapXValue { get; set; } = 0.0f;
	
	[Export]
	public float SnapZValue { get; set; } = 0.0f;

	[Export]
	public bool OptimizedSpawn { get; set; } = false;
	
	[Export]
	public bool SphereCollision { get; set; } = false;
	
	[Export]
	public bool ConvexCollision { get; set; } = false;
	
	[Export]
	public bool ConvexClean { get; set; } = false;
	
	[Export]
	public bool ConvexSimplify { get; set; } = false;
	
	[Export]
	public bool ConcaveCollision { get; set; } = false;
	
	[Export]
	public bool SnapToObject { get; set; } = false;
	
	[Export]
	public bool SnapToHeight { get; set; } = false;
	
	[Export]
	public bool SnapToX { get; set; } = false;
	
	[Export]
	public bool SnapToZ { get; set; } = false;

	[Export]
	public float DistanceToTop { get; set; } = 0;
	
	[Export]
	public float DistanceToBottom { get; set; } = 0;
	
	[Export]
	public float DistanceToLeft { get; set; } = 0;
	
	[Export]
	public float DistanceToRight { get; set; } = 0;
	
	[Export]
	public Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> ChildOptions = new();

	public override void _EnterTree()
	{
		GroupResource resource = GD.Load<Resource>(GroupPath) as GroupResource;
		GlobalExplorer explorer = GlobalExplorer.GetInstance();
		
		if( explorer.States.GroupedObjects.ContainsKey(GroupPath) ) 
		{
			GlobalExplorer.GetInstance().States.GroupedObjects[GroupPath].Add(this);
		}
		else 
		{
			GlobalExplorer.GetInstance().States.GroupedObjects.Add(GroupPath, new() { this });	
		}
		
		if( OptimizedSpawn ) 
		{
			// Since we dont have AsMeshInstances to provide the usual snapping. We will have to use
			// position data from our paths.
			for( int i = 0; i < resource._Origins.Count; i++ ) 
			{
				GlobalExplorer.GetInstance().Waypoints.Register(this, resource._Origins[i], resource._Rotations[i], resource._Scales[i]);
			}
		}
		
		base._EnterTree();
	}
	
	public Aabb GetAabb()
	{
		GroupResource resource = GD.Load<Resource>(GroupPath) as GroupResource;

		Godot.Collections.Array<string> meshPaths = resource._Paths;
		Godot.Collections.Dictionary<int, Vector3> OgOrigins = resource._Origins;
		Godot.Collections.Array<Mesh> meshes = new();
		Godot.Collections.Array<Vector3> origins = new();
		
		for( int i = 0; i < meshPaths.Count; i++ ) 
		{
			meshes.Add(GD.Load<Mesh>(meshPaths[i]));
			origins.Add(OgOrigins[i]);
		}

		Aabb aabb = AabbUtils.CalculateCombinedAABB(origins, meshes);
		
		return aabb;
	}
	
	/*
	** Will reload the group data and re draw the local space of the node 
	**
	** @returns void
	*/ 
	public void Update()
	{
		GroupResource resource = GD.Load<Resource>(GroupPath) as GroupResource;

		if( false == OptimizedSpawn ) 
		{
			ClearCurrentChildren();
			
			if( ShouldAddCollision() ) 
			{
				resource.AddCollidingChildren(this, ChildOptions);
			}
			else
			{
				resource.AddChildren(this, ChildOptions);
			}
		}
		else 
		{
			ClearCurrentChildren();

			List<Vector3> origins = new();
			List<Vector3> rotations = new();
			List<Vector3> scales = new();
			Godot.Collections.Array<Mesh> meshes = new();

			for (int i = 0; i < Connections.Count; i++)
			{
				GroupedConnection connection = Connections[i];

				Transform3D transform = Transform3D.Identity;
				transform.Scaled(resource._Scales[i]);
				// Convert the rotation from degrees to radians
				float rotationRadiansX = Mathf.DegToRad(resource._Rotations[i].X);
				float rotationRadiansY = Mathf.DegToRad(resource._Rotations[i].Y);
				float rotationRadiansZ = Mathf.DegToRad(resource._Rotations[i].Z);

				// Create a rotation basis around each axis
				Basis rotationBasisX = Basis.Identity.Rotated(Vector3.Right, rotationRadiansX);
				Basis rotationBasisY = Basis.Identity.Rotated(Vector3.Up, rotationRadiansY);
				Basis rotationBasisZ = Basis.Identity.Rotated(Vector3.Forward, rotationRadiansZ);

				// Combine the rotation around each axis
				Basis finalRotation = rotationBasisX * rotationBasisY * rotationBasisZ;

				// Assuming you have a transform called transform
				transform.Basis = finalRotation;
				transform.Origin = Transform.Origin + new Vector3(resource._Origins[i].X, resource._Origins[i].Y, resource._Origins[i].Z);
				
				connection.Update( transform );
				connection.UpdateUsing( transform, ChildOptions[i] );
				meshes.Add(connection.InstanceMesh);
				origins.Add(resource._Origins[i]);
				rotations.Add(resource._Rotations[i]);
				scales.Add(resource._Scales[i]);
			}

			AddMultiCollisions(origins, scales, rotations, meshes, this, ChildOptions);
		}
	}
	
	public void AddConnection( int id, AsOptimizedMultiMeshGroup3D optimizedMeshGroup, Mesh mesh )
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

	public void AddMultiCollisions(List<Vector3> _Positions, List<Vector3> _Scales, List<Vector3> _Rotations, Godot.Collections.Array<Mesh> _Meshes, AsGrouped3D grouped3D, Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> ChildOptions )
	{
		Node _SceneRoot = GlobalExplorer.GetInstance()._Plugin.GetTree().EditedSceneRoot;

		for (int i = 0; i < _Positions.Count; i++)
		{
			Vector3 _Pos = _Positions[i];
			Transform3D _Trans = Transform3D.Identity;
			_Trans.Origin = new Vector3(_Pos.X, _Pos.Y, _Pos.Z);
			AsStaticBody3D _Body = new()
			{
				Transform = _Trans,
				UsingMultiMesh = true,
				// Mesh = _Meshes[i],
				// MeshName = _Meshes[i].ResourceName,
				// InstanceTransform = _Trans,
				// InstanceScale = _Scales[i],
				// InstanceRotation = _Rotations[i],
			};

			grouped3D.AddChild(_Body);
			_Body.Owner = _SceneRoot;

			int typeState = 0;
			int argState = 0;

			bool IsChildConvex = ChildOptions[i].ContainsKey("ConvexCollision") ? ChildOptions[i]["ConvexCollision"].As<bool>() : false;
			bool IsChildConvexClean = ChildOptions[i].ContainsKey("ConvexClean") ? ChildOptions[i]["ConvexClean"].As<bool>() : false;
			bool IsChildConvexSimplify = ChildOptions[i].ContainsKey("ConvexSimplify") ? ChildOptions[i]["ConvexSimplify"].As<bool>() : false;
			bool IsChildConcave = ChildOptions[i].ContainsKey("ConcaveCollision") ? ChildOptions[i]["ConcaveCollision"].As<bool>() : false;
			bool IsChildSphere = ChildOptions[i].ContainsKey("SphereCollision") ? ChildOptions[i]["SphereCollision"].As<bool>() : false;

			if (
				true == grouped3D.ConvexCollision &&
				false == IsChildConvex &&
				false == IsChildConcave &&
				false == IsChildSphere ||
				true == IsChildConvex	
			) 
			{
				typeState = 1;
				
				if(
					true == grouped3D.ConvexClean &&
					false == grouped3D.ConvexSimplify &&
					false == IsChildConvex ||
					true == IsChildConvexClean &&
					true == IsChildConvexSimplify
				) 
				{
					argState = 1;	
				}
				else if( 
					false == grouped3D.ConvexClean &&
					true == grouped3D.ConvexSimplify &&
					false == IsChildConvex ||
					false == IsChildConvexClean &&
					true == IsChildConvexSimplify
				) 
				{
					argState = 2;	
				}
				else if( 
					true == grouped3D.ConvexClean &&
					true == grouped3D.ConvexSimplify &&
					false == IsChildConvex ||
					true == IsChildConvexClean &&
					true == IsChildConvexSimplify
				) 
				{
					argState = 3;	
				}
			}
			else if( 
				true == grouped3D.ConcaveCollision &&
				false == IsChildConvex &&
				false == IsChildConcave &&
				false == IsChildSphere ||
				true == IsChildConcave	
			) 
			{
				typeState = 2;
			}
			else if(
				true == grouped3D.SphereCollision &&
				false == IsChildConvex &&
				false == IsChildConcave &&
				false == IsChildSphere ||
				true == IsChildSphere	
			) 
			{
				typeState = 3;
			}

			_Body.Initialize();
		}
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
		GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
		bool AddCollisions = _GlobalExplorer.Settings.GetKey("add_collisions").As<bool>();

		return AddCollisions;
	}
	
	public override void _ExitTree()
	{
		GlobalExplorer explorer = GlobalExplorer.GetInstance();
		
		if( explorer.States.GroupedObjects.ContainsKey(GroupPath) ) 
		{
			GlobalExplorer.GetInstance().States.GroupedObjects[GroupPath].Remove(this);
			
			if( GlobalExplorer.GetInstance().States.GroupedObjects[GroupPath].Count == 0 ) 
			{
				GlobalExplorer.GetInstance().States.GroupedObjects.Remove(GroupPath);
			}
		}

		base._EnterTree();
	}
	
}