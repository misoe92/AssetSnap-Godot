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
		private Vector3 _Origin;
		private Vector3 _Rotation;
		private Vector3 _Scale; 
		private Node3D _Model;
		private AsGroup3D _Group;
		private bool _IsModel = false;
		private bool _IsGroup = false;
		
		/// <summary>
		/// Constructor for a waypoint associated with a model.
		/// </summary>
		/// <param name="Origin">The origin position of the waypoint.</param>
		/// <param name="Rotation">The rotation of the waypoint.</param>
		/// <param name="Scale">The scale of the waypoint.</param>
		/// <param name="Model">The associated 3D model node.</param>
		public BaseWaypoint( Vector3 Origin, Vector3 Rotation, Vector3 Scale, Node3D Model ) 
		{
			_Origin = Origin;
			_Rotation = Rotation;
			_Scale = Scale;
			_Model = Model;
			
			if( _Model.HasMeta("AsModel")) 
			{
				_IsModel = true;			
				_IsGroup = false;			
			}
			else if( _Model.HasMeta("AsGroup") ) 
			{
				_IsModel = false;			
				_IsGroup = true;			
			}
		}
		
		/// <summary>
		/// Constructor for a waypoint associated with a group.
		/// </summary>
		/// <param name="_Origin">The origin position of the waypoint.</param>
		/// <param name="_Group">The associated 3D group node.</param>
		public BaseWaypoint( Vector3 Origin, AsGroup3D Group ) 
		{
			_Origin = Origin;
			_Group = Group;
			_IsGroup = true;
		}
		
		/// <summary>
		/// Sets the scale of the waypoint model.
		/// </summary>
		/// <param name="scale">The new scale to set.</param>
		public void SetScale( Vector3 scale ) 
		{
			_Scale = scale;
		}
			
		/// <summary>
		/// Retrieves the axis-aligned bounding box (AABB) of the associated model.
		/// </summary>
		/// <returns>The AABB of the model.</returns>
		public Aabb GetAabb()
		{
			if( EditorPlugin.IsInstanceValid( _Model ) && _Model is AsGrouped3D ) 
			{
				AsGrouped3D Model = _Model as AsGrouped3D;
				return Model.GetAabb();
			}
			
			if( _IsGroup && EditorPlugin.IsInstanceValid( _Model ) ) 
			{
				AsGroup3D Model = _Model as AsGroup3D; 
				return new Aabb();
			}
			
			if( _IsModel && EditorPlugin.IsInstanceValid( _Model ) ) 
			{
				AsMeshInstance3D Model = _Model as AssetSnap.Front.Nodes.AsMeshInstance3D; 
				return Model.GetAabb();
			}
			
			return new Aabb();
		}
		
		/// <summary>
		/// Retrieves the associated model node.
		/// </summary>
		/// <returns>The associated model node.</returns>
		public Node3D GetModel()
		{
			return _Model;
		}
		
		/// <summary>
		/// Retrieves the origin position of the model.
		/// </summary>
		/// <returns>The origin position of the model.</returns>
		public Vector3 GetOrigin()
		{
			return _Origin;
		}
		
		/// <summary>
		/// Retrieves the scale of the model.
		/// </summary>
		/// <returns>The scale of the model.</returns>
		public Vector3 GetScale()
		{
			return _Scale;
		}
	}
}
#endif