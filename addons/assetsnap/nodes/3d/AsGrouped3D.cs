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
using AssetSnap.Explorer;
using AssetSnap.Nodes;
using AssetSnap.States;
using AssetSnap.Static;
using Godot;

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// Represents a 3D grouped node with additional functionality for managing connections and updating group data.
	/// </summary>
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

		/// <summary>
		/// The path of the group.
		/// </summary>
		[Export]
		public string GroupPath { get => _GroupPath; set { _GroupPath = value; } }

		/// <summary>
		/// The layer to snap to.
		/// </summary>
		[Export]
		public int SnapLayer { get => _SnapLayer; set { _SnapLayer = value; Update(); } }

		/// <summary>
		/// The offset in the X-axis.
		/// </summary>
		[Export]
		public float ObjectOffsetX { get => _ObjectOffsetX; set { _ObjectOffsetX = value; } }

		/// <summary>
		/// The offset in the Z-axis.
		/// </summary>
		[Export]
		public float ObjectOffsetZ { get => _ObjectOffsetZ; set { _ObjectOffsetZ = value; } }

		/// <summary>
		/// The value to snap height to.
		/// </summary>
		[Export]
		public float SnapHeightValue { get => _SnapHeightValue; set { _SnapHeightValue = value; } }

		/// <summary>
		/// The value to snap in the X-axis.
		/// </summary>
		[Export]
		public float SnapXValue { get => _SnapXValue; set { _SnapXValue = value; } }

		/// <summary>
		/// The value to snap in the Z-axis.
		/// </summary>
		[Export]
		public float SnapZValue { get => _SnapZValue; set { _SnapZValue = value; } }
		
		/// <summary>
		/// The value to snap in the Z-axis.
		/// </summary>
		[Export]
		public float DistanceToTop { get => _DistanceToTop; set { _DistanceToTop = value; } }

		/// <summary>
		/// The distance to the bottom.
		/// </summary>
		[Export]
		public float DistanceToBottom { get => _DistanceToBottom; set { _DistanceToBottom = value; } }

		/// <summary>
		/// The distance to the left.
		/// </summary>
		[Export]
		public float DistanceToLeft { get => _DistanceToLeft; set { _DistanceToLeft = value; } }

		/// <summary>
		/// The distance to the right.
		/// </summary>
		[Export]
		public float DistanceToRight { get => _DistanceToRight; set { _DistanceToRight = value; } }

		/// <summary>
		/// Indicates if spawn is optimized.
		/// </summary>
		[Export]
		public bool OptimizedSpawn { get => _OptimizedSpawn; set { _OptimizedSpawn = value; } }

		/// <summary>
		/// Indicates if sphere collision is enabled.
		/// </summary>
		[Export]
		public bool SphereCollision { get => _SphereCollision; set { _SphereCollision = value; Update(); } }

		/// <summary>
		/// Indicates if convex collision is enabled.
		/// </summary>
		[Export]
		public bool ConvexCollision { get => _ConvexCollision; set { _ConvexCollision = value; Update(); } }

		/// <summary>
		/// Indicates if convex cleaning is enabled.
		/// </summary>
		[Export]
		public bool ConvexClean { get => _ConvexClean; set { _ConvexClean = value; Update(); } }

		/// <summary>
		/// Indicates if convex simplification is enabled.
		/// </summary>
		[Export]
		public bool ConvexSimplify { get => _ConvexSimplify; set { _ConvexSimplify = value; Update(); } }

		/// <summary>
		/// Indicates if concave collision is enabled.
		/// </summary>
		[Export]
		public bool ConcaveCollision { get => _ConcaveCollision; set { _ConcaveCollision = value; Update(); } }

		/// <summary>
		/// Indicates if snapping to object is enabled.
		/// </summary>
		[Export]
		public bool SnapToObject { get => _SnapToObject; set { _SnapToObject = value; } }

		/// <summary>
		/// Indicates if snapping to height is enabled.
		/// </summary>
		[Export]
		public bool SnapToHeight { get => _SnapToHeight; set { _SnapToHeight = value; } }

		/// <summary>
		/// Indicates if snapping to the X-axis is enabled.
		/// </summary>
		[Export]
		public bool SnapToX { get => _SnapToX; set { _SnapToX = value; } }

		/// <summary>
		/// Indicates if snapping to the Z-axis is enabled.
		/// </summary>
		[Export]
		public bool SnapToZ { get => _SnapToZ; set { _SnapToZ = value; } }

		/// <summary>
        /// Options for child nodes.
        /// </summary>
		[Export]
		public Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> ChildOptions { get => _ChildOptions; set { _ChildOptions = value; } }

		/// <summary>
		/// Called when the node enters the scene tree.
		/// </summary>
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

		/// <summary>
		/// Returns the Axis-Aligned Bounding Box (AABB) of the node.
		/// </summary>
		/// <returns>The AABB of the node.</returns>
		public Aabb GetAabb()
		{
			return NodeUtils.CalculateNodeAabb(this);
		}

		/// <summary>
		/// Updates the group, reloading group data and redrawing the local space of the node.
		/// </summary>
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

		/// <summary>
		/// Adds a connection to the group.
		/// </summary>
		/// <param name="id">The ID of the connection.</param>
		/// <param name="optimizedMeshGroup">The optimized mesh group associated with the connection.</param>
		/// <param name="mesh">The mesh of the connection.</param>
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

		/// <summary>
		/// Clears the group by removing all children nodes.
		/// </summary>
		public void Clear()
		{
			ClearCurrentChildren();
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
		/// Checks if collision should be added to the group.
		/// </summary>
		/// <returns>True if collision should be added, otherwise false.</returns>
		private bool ShouldAddCollision()
		{
			return SettingsStatic.ShouldAddCollision();
		}

		/// <summary>
		/// Called when the node exits the scene tree.
		/// </summary>
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
}