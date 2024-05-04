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
using Godot;

namespace AssetSnap.Waypoint
{
	public partial class BaseWaypoint
	{
		private Vector3 Origin;
		private Vector3 Rotation;
		private Vector3 Scale; 
		private Node3D Model;
		private AsGroup3D Group;
		private bool IsModel = false;
		private bool IsGroup = false;
		
		/// <summary>
		/// Constructor for a waypoint associated with a model.
		/// </summary>
		/// <param name="_Origin">The origin position of the waypoint.</param>
		/// <param name="_Rotation">The rotation of the waypoint.</param>
		/// <param name="_Scale">The scale of the waypoint.</param>
		/// <param name="_Model">The associated 3D model node.</param>
		public BaseWaypoint( Vector3 _Origin, Vector3 _Rotation, Vector3 _Scale, Node3D _Model ) 
		{
			Origin = _Origin;
			Rotation = _Rotation;
			Scale = _Scale;
			Model = _Model;
			
			if( _Model.HasMeta("AsModel")) 
			{
				IsModel = true;			
				IsGroup = false;			
			}
			else if( _Model.HasMeta("AsGroup") ) 
			{
				IsModel = false;			
				IsGroup = true;			
			}
		}
		
		/// <summary>
		/// Constructor for a waypoint associated with a group.
		/// </summary>
		/// <param name="_Origin">The origin position of the waypoint.</param>
		/// <param name="_Group">The associated 3D group node.</param>
		public BaseWaypoint( Vector3 _Origin, AsGroup3D _Group ) 
		{
			Origin = _Origin;
			Group = _Group;
			IsGroup = true;
		}
		
		/// <summary>
		/// Sets the scale of the waypoint model.
		/// </summary>
		/// <param name="scale">The new scale to set.</param>
		public void SetScale( Vector3 scale ) 
		{
			Scale = scale;
		}
			
		/// <summary>
		/// Retrieves the axis-aligned bounding box (AABB) of the associated model.
		/// </summary>
		/// <returns>The AABB of the model.</returns>
		public Aabb GetAabb()
		{
			if( EditorPlugin.IsInstanceValid( Model ) && Model is AsGrouped3D ) 
			{
				AsGrouped3D _Model = Model as AsGrouped3D;
				return _Model.GetAabb();
			}
			
			if( IsGroup && EditorPlugin.IsInstanceValid( Model ) ) 
			{
				AsGroup3D _Model = Model as AsGroup3D; 
				return new Aabb();
			}
			
			if( IsModel && EditorPlugin.IsInstanceValid( Model ) ) 
			{
				AsMeshInstance3D _Model = Model as AssetSnap.Front.Nodes.AsMeshInstance3D; 
				return _Model.GetAabb();
			}
			
			return new Aabb();
		}
		
		/// <summary>
		/// Retrieves the associated model node.
		/// </summary>
		/// <returns>The associated model node.</returns>
		public Node3D GetModel()
		{
			return Model;
		}
		
		/// <summary>
		/// Retrieves the origin position of the model.
		/// </summary>
		/// <returns>The origin position of the model.</returns>
		public Vector3 GetOrigin()
		{
			return Origin;
		}
		
		/// <summary>
		/// Retrieves the scale of the model.
		/// </summary>
		/// <returns>The scale of the model.</returns>
		public Vector3 GetScale()
		{
			return Scale;
		}
	}
}
#endif