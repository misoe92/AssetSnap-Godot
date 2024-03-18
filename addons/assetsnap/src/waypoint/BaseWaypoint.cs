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
	using AssetSnap.Front.Nodes;
	using Godot;

	public partial class BaseWaypoint
	{
		private Vector3 Origin;
		private Vector3 Rotation;
		private Vector3 Scale; 
		private Node3D Model;
		private AsGroup3D Group;
		private bool IsModel = false;
		private bool IsGroup = false;
		
		/*
		** Waypoint constructor
		**
		** @param Vector3 _Origin
		** @param Vector3 _Rotation
		** @param Vector3 _Scale
		** @param Node3D _Model
		** @return void
		*/
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
		
		/*
		** Waypoint constructor
		**
		** @param Vector3 _Origin
		** @param AsGroup3D _Group
		** @return void
		*/
		public BaseWaypoint( Vector3 _Origin, AsGroup3D _Group ) 
		{
			Origin = _Origin;
			Group = _Group;
			IsGroup = true;
		}
		
		/*
		** Set's the scale of the waypoint model
		**
		** @param Vector3 scale
		** @return void
		*/
		public void SetScale( Vector3 scale ) 
		{
			Scale = scale;
		}
			
		/*
		** Fetches the aabb of the model
		**
		** @return Aabb
		*/
		public Aabb GetAabb()
		{
			if( IsModel ) 
			{
				AsMeshInstance3D _Model = Model as AssetSnap.Front.Nodes.AsMeshInstance3D; 
				return _Model.GetAabb();
			}
			
			return new Aabb();
		}
		
		/*
		** Fetches the model
		**
		** @return Node3D
		*/
		public Node3D GetModel()
		{
			return Model;
		}
		
		/*
		** Fetches the model's origin position
		**
		** @return Vector3
		*/
		public Vector3 GetOrigin()
		{
			return Origin;
		}
		
		/*
		** Fetches the model's scale
		**
		** @return Vector3
		*/
		public Vector3 GetScale()
		{
			return Scale;
		}
	}
}
#endif