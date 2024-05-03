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

using AssetSnap.Front.Nodes;
using AssetSnap.Static;
using AssetSnap.Waypoint;
using Godot;

namespace AssetSnap.Snap
{
	/// <summary>
	/// Defines a base class for snapping functionality.
	/// </summary>
	public class SnappableBase
	{
		/// <summary>
		/// The default snap distance.
		/// </summary>
		public float SnapDistance = 1.0f;
		
		private static SnappableBase _Instance;
		
		/// <summary>
		/// Gets the singleton instance of the <see cref="SnappableBase"/> class.
		/// </summary>
		public static SnappableBase Singleton
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
		
		/// <summary>
		/// Snaps the given coordinates to nearby waypoints.
		/// </summary>
		/// <param name="Coordinates">The coordinates to snap.</param>
		/// <param name="aabb">The axis-aligned bounding box.</param>
		/// <param name="Layer">The snap layer.</param>
		/// <returns>The snapped coordinates.</returns>
		public Vector3 Snap(Vector3 Coordinates, Aabb aabb, int Layer = 0)
		{
			if( false == GlobalExplorer.GetInstance().Waypoints.HasAnyWaypoints() ) 
			{
				return Vector3.Zero;
			}

			Node3D node = HandleStatic.Get();
			
			bool SnapToX = SnapStatic.IsSnapX();
			bool SnapToZ = SnapStatic.IsSnapZ();
			
			float ObjectSnapOffsetX = 0.0f;
			float ObjectSnapOffsetZ = 0.0f;
			
			if( SnapStatic.HasObjectSnapOffsetX() ) 
			{
				ObjectSnapOffsetX = SnapStatic.GetObjectSnapOffsetX();
			}
			
			if( SnapStatic.HasObjectSnapOffsetZ() ) 
			{
				ObjectSnapOffsetZ = SnapStatic.GetObjectSnapOffsetZ();
			}
			
			Vector3 snappedCoordinates = Coordinates;
			
			WaypointsStatic.Each(
				(BaseWaypoint Point ) => {
					Node3D model = Point.GetModel();
					Aabb AABB = Point.GetAabb();

					Vector3 MeshSize = AABB.Size * Point.GetScale();
					Vector3 OuterMeshSize = aabb.Size * new Vector3(0.5f, 0.5f, 0.5f);
					Vector3 spawnPointGlobal = Point.GetModel().GlobalTransform.Origin;

					if( false == IsSnapLayerValid( model, Layer ) ) 
					{
						return;
					}
					
					if (!SnapToX &&
						(Coordinates.X > spawnPointGlobal.X - MeshSize.X ) &&
						(Coordinates.X < spawnPointGlobal.X + MeshSize.X ) &&
						(
							(Coordinates.Z > spawnPointGlobal.Z - MeshSize.Z - SnapDistance) &&
							(Coordinates.Z < spawnPointGlobal.Z + MeshSize.Z + SnapDistance)
						)
					)
					{
						if(Coordinates.Z - spawnPointGlobal.Z < ( MeshSize.Z / 2)) 
						{
							snappedCoordinates.Z = spawnPointGlobal.Z - MeshSize.Z;
							
							if( node is AsGrouped3D asGrouped3DZ ) 
							{
								snappedCoordinates.Z = snappedCoordinates.Z - asGrouped3DZ.DistanceToTop;
							}
							
							if( ObjectSnapOffsetZ != 0 ) 
							{
								snappedCoordinates.Z -= ObjectSnapOffsetZ;
							}
						}
						else 
						{
							snappedCoordinates.Z = spawnPointGlobal.Z + MeshSize.Z;
							
							if( node is AsGrouped3D asGrouped3DZ ) 
							{
								snappedCoordinates.Z = snappedCoordinates.Z + asGrouped3DZ.DistanceToBottom;
							}
							
							if( ObjectSnapOffsetZ != 0 ) 
							{
								snappedCoordinates.Z += ObjectSnapOffsetZ;
							}
						}
						snappedCoordinates.X = spawnPointGlobal.X;
						
						return;
					}

					// Check if the coordinates are within the snap distance on the X-axis
					if (!SnapToZ &&
						(Coordinates.Z > spawnPointGlobal.Z - MeshSize.Z) &&
						(Coordinates.Z < spawnPointGlobal.Z + MeshSize.Z) &&
						(
							(Coordinates.X > spawnPointGlobal.X - MeshSize.X - SnapDistance) &&
							(Coordinates.X < spawnPointGlobal.X + MeshSize.X + SnapDistance)
						)
					)
					{
						if(Coordinates.X - spawnPointGlobal.X < ( MeshSize.X / 2)) 
						{
							snappedCoordinates.X = spawnPointGlobal.X - MeshSize.X;
							
							if( node is AsGrouped3D asGrouped3DZ ) 
							{
								snappedCoordinates.X = snappedCoordinates.X - asGrouped3DZ.DistanceToLeft;
							}
							
							if( ObjectSnapOffsetX != 0 ) 
							{
								snappedCoordinates.X -= ObjectSnapOffsetX;
							}
						}
						else 
						{
							snappedCoordinates.X = spawnPointGlobal.X + MeshSize.X;
							
							if( node is AsGrouped3D asGrouped3DZ ) 
							{
								snappedCoordinates.X = snappedCoordinates.X - asGrouped3DZ.DistanceToRight;
							}
							
							if( ObjectSnapOffsetX != 0 ) 
							{
								snappedCoordinates.X += ObjectSnapOffsetX;
							}
						}
						
						snappedCoordinates.Z = spawnPointGlobal.Z;
						return;
					}
				}
			);
			
			return snappedCoordinates;
		}
		
		/// <summary>
		/// Checks if snapping is possible with the given coordinates and layer.
		/// </summary>
		/// <param name="Coordinates">The coordinates to check.</param>
		/// <param name="Layer">The snap layer.</param>
		/// <returns>True if snapping is possible, false otherwise.</returns>
		public bool CanSnap(Vector3 Coordinates, int Layer = 0)
		{
			if( false == WaypointsStatic.HasAnyWaypoints() ) 
			{
				GD.Print("Houston, We got a problem.");
				return false;
			}

			bool Snapping = false;
			bool SnapToX = SnapStatic.IsSnapX();
			bool SnapToZ = SnapStatic.IsSnapZ();

			WaypointsStatic.Each(
				(BaseWaypoint Point ) => {
					Node3D model = Point.GetModel();
					Aabb AABB = Point.GetAabb();

					Vector3 MeshSize = AABB.Size * ( Point.GetScale() * new Vector3(0.5f, 0.5f, 0.5f));
					Vector3 spawnPointGlobal = Point.GetModel().GlobalTransform.Origin;

					if( false == IsSnapLayerValid(model, Layer) ) 
					{
						return;
					}
										
					// Check if the coordinates are within the snap distance on the X-axis
					if (!SnapToX &&
						(Coordinates.X > spawnPointGlobal.X - MeshSize.X) &&
						(Coordinates.X < spawnPointGlobal.X + MeshSize.X) &&
						(
							(Coordinates.Z > spawnPointGlobal.Z - MeshSize.Z - SnapDistance) &&
							(Coordinates.Z < spawnPointGlobal.Z + MeshSize.Z + SnapDistance)
						)
					)
					{
						Snapping = true;
						return;
					}

					// Check if the coordinates are within the snap distance on the X-axis
					if (!SnapToZ &&
						(Coordinates.Z > spawnPointGlobal.Z - MeshSize.Z) &&
						(Coordinates.Z < spawnPointGlobal.Z + MeshSize.Z) &&
						(
							(Coordinates.X > spawnPointGlobal.X - MeshSize.X - SnapDistance) &&
							(Coordinates.X < spawnPointGlobal.X + MeshSize.X + SnapDistance)
						)
					)
					{
						Snapping = true;
						return;
					}
				}
			);

			return Snapping;
		}
		
		/// <summary>
        /// Checks if the snap layer of the model is valid.
        /// </summary>
        /// <param name="model">The 3D model node.</param>
        /// <param name="Layer">The snap layer to check.</param>
        /// <returns>True if the snap layer is valid, false otherwise.</returns>
		private bool IsSnapLayerValid( Node3D model, int Layer = 0)
		{
			if( model is AsMeshInstance3D asMeshInstance3D ) 
			{
				int SnapLayer = asMeshInstance3D.GetSetting<int>("_LSSnapLayer.value");
				if( SnapLayer != Layer ) 
				{
					return false;
				}
			}
			else if ( model is AsGrouped3D asGrouped3D )
			{
				int SnapLayer = asGrouped3D.SnapLayer;
				if( SnapLayer != Layer ) 
				{
					return false;
				}
			}
			else 
			{
				return false;
			}

			return true;
		}
	}
}
#endif