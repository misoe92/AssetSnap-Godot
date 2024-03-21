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
namespace AssetSnap.Waypoint
{
	using System;
	using System.Collections.Generic;
	using AssetSnap.Front.Nodes;
	using Godot;
	
	public class WaypointList
	{
		private BaseWaypoint[] Waypoints;
		public delegate void CallableMethod(BaseWaypoint waypoint);

		public WaypointList()
		{
			Waypoints = Array.Empty<BaseWaypoint>();
		}
		/*
		** Adds a single waypoint
		**
		** @param Node3D _model
		** @param Vector3 Origin
		** @param Vector3 Rotation
		** @param Vector3 Scale
		** @return void
		*/
		public void Add(Node3D _model, Vector3 Origin, Vector3 Rotation, Vector3 Scale )
		{
			List<BaseWaypoint> _WaypointList = new List<BaseWaypoint>(Waypoints);
			BaseWaypoint Waypoint = new( Origin, Rotation, Scale, _model );
			_WaypointList.Add(Waypoint);
			Waypoints = _WaypointList.ToArray();
		}
		
		/*
		** Removes a single waypoint
		**
		** @param Node3D _model
		** @param Vector3 Origin
		** @param Vector3 Rotation
		** @param Vector3 Scale
		** @return void
		*/
		public void Remove(MeshInstance3D ModelInstance, Vector3 Origin)
		{
			for (int i = 0; i < Waypoints.Length; i++) 
			{
				bool state = true;
				BaseWaypoint Point = Waypoints[i];
				
				if( EditorPlugin.IsInstanceValid(Point.GetModel()) && Point.GetModel().HasMeta("AsModel") ) 
				{
					AsMeshInstance3D Model = Point.GetModel() as AssetSnap.Front.Nodes.AsMeshInstance3D;
					
					if( ModelInstance.HasMeta("AsModel")) 
					{
						AsMeshInstance3D _Model = ModelInstance as AssetSnap.Front.Nodes.AsMeshInstance3D;
						if (Model.Mesh == _Model.Mesh) 
						{
							state = false;
						}
					}
					else 
					{
						state = false;
					}
				}
				else
				{
					state = false;
				}
				
				if( false == EditorPlugin.IsInstanceValid(Point.GetModel()) || Point.GetOrigin() == Origin && state ) 
				{
					List<BaseWaypoint> _SpawnPointsList = new List<BaseWaypoint>(Waypoints);
					_SpawnPointsList.Remove(Point);
					Waypoints = _SpawnPointsList.ToArray();
				}
			}
		}
		
		/*
		** Updates a waypoint by it's origin
		**
		** @param string Type
		** @param Variant Value
		** @param Vector3 Where
		** @return void
		*/
		public void Update(string Type, Variant Value, Vector3 Where ) 
		{
			if( "Scale" == Type ) 
			{
				foreach( BaseWaypoint Waypoint in Waypoints ) 
				{
					if( Waypoint.GetOrigin() == Where ) 
					{
						Waypoint.SetScale((Vector3)Value);
					}
				}
			}
		}
		
		public bool Has( Node3D node ) 
		{
			GD.Print(Waypoints.Length);
			foreach( BaseWaypoint Waypoint in Waypoints ) 
			{
				if( Waypoint.GetModel() == node ) 
				{
					return true;
				}
			}

			return false;
		}
		
		public void Each( CallableMethod callback ) 
		{
			foreach( BaseWaypoint Waypoint in Waypoints ) 
			{
				if( EditorPlugin.IsInstanceValid( Waypoint.GetModel() ) ) 
				{
					callback(Waypoint);					
				}
				else
				{
					List<BaseWaypoint> _SpawnPointsList = new List<BaseWaypoint>(Waypoints);
					_SpawnPointsList.Remove(Waypoint);
					Waypoints = _SpawnPointsList.ToArray();
				}
			} 
		}
		
		/*
		** Checks if any waypoints is available
		**
		** @return bool
		*/
		public bool IsEmpty() 
		{
			return Waypoints.Length == 0;
		}
		
		
		public void _Exit()
		{
			if( Waypoints.Length != 0 ) 
			{
				Waypoints = Array.Empty<BaseWaypoint>();
			}
		}
	}
}
#endif