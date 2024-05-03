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

	/// <summary>
	/// Represents a list of waypoints.
	/// </summary>
	public class WaypointList
	{
		private BaseWaypoint[] Waypoints;
		public delegate void CallableMethod(BaseWaypoint waypoint);

		public WaypointList()
		{
			Waypoints = Array.Empty<BaseWaypoint>();
		}
		
		/// <summary>
		/// Adds a single waypoint to the list.
		/// </summary>
		/// <param name="_model">The 3D node representing the model associated with the waypoint.</param>
		/// <param name="Origin">The origin of the waypoint.</param>
		/// <param name="Rotation">The rotation of the waypoint.</param>
		/// <param name="Scale">The scale of the waypoint.</param>
		public void Add(Node3D _model, Vector3 Origin, Vector3 Rotation, Vector3 Scale)
		{
			List<BaseWaypoint> _WaypointList = new List<BaseWaypoint>(Waypoints);
			BaseWaypoint Waypoint = new(Origin, Rotation, Scale, _model);
			_WaypointList.Add(Waypoint);
			Waypoints = _WaypointList.ToArray();
		}

		/// <summary>
		/// Removes a single waypoint from the list.
		/// </summary>
		/// <param name="ModelInstance">The model instance associated with the waypoint to remove.</param>
		/// <param name="Origin">The origin of the waypoint to remove.</param>
		public void Remove(Node ModelInstance, Vector3 Origin)
		{
			for (int i = 0; i < Waypoints.Length; i++)
			{
				bool state = true;
				BaseWaypoint Point = Waypoints[i];

				if (EditorPlugin.IsInstanceValid(Point.GetModel()) && Point.GetModel().HasMeta("AsModel"))
				{
					AsMeshInstance3D Model = Point.GetModel() as AssetSnap.Front.Nodes.AsMeshInstance3D;

					if (ModelInstance.HasMeta("AsModel") && ModelInstance is AssetSnap.Front.Nodes.AsMeshInstance3D && null != Model)
					{
						AsMeshInstance3D _Model = ModelInstance as AssetSnap.Front.Nodes.AsMeshInstance3D;
						if (Model.Mesh != _Model.Mesh)
						{
							state = false;
						}
					}
					else if (ModelInstance is AssetSnap.Front.Nodes.AsMeshInstance3D && null != Model)
					{
						state = false;
					}
				}
				else
				{
					state = false;
				}

				if (false == EditorPlugin.IsInstanceValid(Point.GetModel()) || Point.GetOrigin() == Origin && state)
				{
					List<BaseWaypoint> _SpawnPointsList = new List<BaseWaypoint>(Waypoints);
					_SpawnPointsList.Remove(Point);
					Waypoints = _SpawnPointsList.ToArray();
				}
			}
		}

		/// <summary>
		/// Updates a waypoint by its origin.
		/// </summary>
		/// <param name="Type">The type of update (e.g., "Scale").</param>
		/// <param name="Value">The value of the update.</param>
		/// <param name="Where">The origin of the waypoint to update.</param>
		public void Update(string Type, Variant Value, Vector3 Where)
		{
			if ("Scale" == Type)
			{
				foreach (BaseWaypoint Waypoint in Waypoints)
				{
					if (Waypoint.GetOrigin() == Where)
					{
						Waypoint.SetScale((Vector3)Value);
					}
				}
			}
		}

		/// <summary>
		/// Checks if a given node is contained within any waypoint in the list.
		/// </summary>
		/// <param name="node">The node to check for.</param>
		/// <returns>True if the node is contained within any waypoint; otherwise, false.</returns>
		public bool Has(Node3D node)
		{
			foreach (BaseWaypoint Waypoint in Waypoints)
			{
				if (Waypoint.GetModel() == node)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
        /// Executes a callback method for each waypoint in the list.
        /// </summary>
        /// <param name="callback">The callback method to execute for each waypoint.</param>
		public void Each(CallableMethod callback)
		{
			foreach (BaseWaypoint Waypoint in Waypoints)
			{
				if (EditorPlugin.IsInstanceValid(Waypoint.GetModel()))
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

		/// <summary>
        /// Checks if the waypoint list is empty.
        /// </summary>
        /// <returns>True if the waypoint list is empty; otherwise, false.</returns>
		public bool IsEmpty()
		{
			return Waypoints.Length == 0;
		}

		/// <summary>
		/// Clears the list of waypoints.
		/// </summary>
		public void _Exit()
		{
			if (Waypoints.Length != 0)
			{
				Waypoints = Array.Empty<BaseWaypoint>();
			}
		}
	}
}
#endif