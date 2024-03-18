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
namespace AssetSnap.Raycast
{
	using Godot;

	public partial class Base : Node
	{
		private RayCast3D _Node;
		private Vector3 _TargetPosition;
		private GodotObject _Collider;
		
		public RayCast3D Node 
		{
			get => _Node;
			set
			{
				_Node = value;
			}
		}
		
		public Vector3 TargetPosition 
		{
			get => _TargetPosition;
			set
			{
				_TargetPosition = value;
				
				if( null != GetNode() ) 
				{
					GetNode().TargetPosition = value;					
				}
			}
		}
		
		private static Base _Instance;
		
		public static Base GetInstance()
		{
			if( null == _Instance ) 
			{
				_Instance = new()
				{
					Name = "AssetSnapRaycast",
				};
			}

			return _Instance;
		}
		
		/*
		** Initializes the raycast handler
		**
		** @return void
		*/
		public void Initialize()
		{
			InitializeNode();
			AddNodeTo(GlobalExplorer.GetInstance()._Plugin);
			Hide();
		}
		
		/*
		** Exits the raycast handler
		**
		** @return void
		*/
		public void Exit()
		{
			ClearNode();
		}
		
		/*
		** Shows the raycast
		**
		** @return void
		*/
		public void Show()
		{	
			/** Only show if we have an actual model to show **/
			if( false == IsHidden() )
			{
				return;
			}
			
			GetNode().Visible = true;
		}
		
		/*
		** Hides the raycast
		**
		** @return void
		*/
		public void Hide()
		{
			if( IsHidden() )
			{
				return;
			}
			
			GetNode().Visible = false;
		}
		
		/*
		** Forces the raycast to update
		** and fetches it's collider reading
		**
		** @return void
		*/
		public void Update()
		{
			if( GetNode() == null ) 
			{
				return;
			}

			GetNode().ForceRaycastUpdate();
			_Collider = GetNode().GetCollider();
		}
		
		/*
		** Sets the collision transform
		**
		** @return Transform3D
		*/
		public void SetTransform(Transform3D GlobalTrans)
		{
			if( GetNode() == null ) 
			{
				return;
			}
			
			GetNode().GlobalTransform = GlobalTrans;
		}
			
		/*
		** Resets the current collider
		**
		** @return void
		*/
		public void ResetCollider()
		{
			_Collider = null;
		}
		/*
		** Fetches the collision transform
		**
		** @return Transform3D
		*/
		public Transform3D GetTransform()
		{
			if( GetNode() == null ) 
			{
				return new Transform3D(Basis.Identity, Vector3.Up);
			}
			
			return GetNode().GlobalTransform;
		}
		
		/*
		** Fetches a instance of the raycast
		**
		** @return RayCast3D
		*/
		public RayCast3D GetNode()
		{
			if( false == EditorPlugin.IsInstanceValid( Node ) ) 
			{
				return null;
			}
			
			return Node;
		}
		
		/*
		** Fetches the collision instance
		**
		** @return GodotObject
		*/
		public GodotObject GetCollider()
		{
			return _Collider;
		}
		
		/*
		** Checks if any collision is available
		**
		** @return bool
		*/
		public bool HasCollision()
		{
			return _Collider != null;
		}
			
		/*
		** Checks if the raycast is hidden
		**
		** @return bool
		*/
		public bool IsHidden()
		{
			return GetNode() != null && GetNode().Visible == false;
		}
		
		/*
		** Initializes the raycast node
		**
		** @return void
		*/
		private void InitializeNode()
		{
			Node = new() 
			{
				Name = "Raycast"
			};
		}
		
		/*
		** Adds the raycast to a given node
		**
		** @return void
		*/	
		private void AddNodeTo( Node To )
		{
			To.AddChild(Node);
		}
		
		/*
		** Clears the raycast node
		**
		** @return void
		*/	
		private void ClearNode()
		{
			// Node.GetParent().RemoveChild(Node);
			Node.QueueFree();
		}

		public override void _ExitTree()
		{
			if( IsInstanceValid(Node) )
			{ 
				Node.QueueFree();
			}
			
			// if( IsInstanceValid(_Collider)) 
			// {
			// 	_Collider.Free();
			// }
			
			base._ExitTree();
		}
		
	}
}
#endif