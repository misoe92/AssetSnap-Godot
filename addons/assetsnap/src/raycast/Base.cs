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

using Godot;

namespace AssetSnap.Raycast
{
	/// <summary>
	/// Partial class for handling raycast functionality.
	/// </summary>
	public partial class Base
	{
		private Vector3 _TargetPosition;
		private GodotObject _Collider;
		
		/// <summary>
		/// Gets the RayCast3D node instance.
		/// </summary>
		public RayCast3D Node 
		{
			get => GetNode();
		}
		
		/// <summary>
		/// Gets or sets the target position of the raycast.
		/// </summary>
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

		/// <summary>
		/// Gets the singleton instance of the Base class.
		/// </summary>
		public static Base Singleton 
		{
			get
			{
				if( null == _Instance ) 
				{
					_Instance = new(){};
				}
				
				return _Instance;
			}
		}
		
		/// <summary>
		/// Initializes the raycast handler.
		/// </summary>
		public void Initialize()
		{
			InitializeNode();
			Hide();
		}
		
		/// <summary>
		/// Exits the raycast handler.
		/// </summary>
		public void Exit()
		{
			ClearNode();
		}
		
		/// <summary>
		/// Shows the raycast.
		/// </summary>
		public void Show()
		{	
			/** Only show if we have an actual model to show **/
			if( false == IsHidden() )
			{
				return;
			}
			
			GetNode().Visible = true;
		}
		
		/// <summary>
		/// Hides the raycast.
		/// </summary>
		public void Hide()
		{
			if( IsHidden() )
			{
				return;
			}
			
			GetNode().Visible = false;
		}
		
		/// <summary>
		/// Forces the raycast to update and fetches its collider reading.
		/// </summary>
		public void Update()
		{
			if( GetNode() == null ) 
			{
				return;
			}

			GetNode().ForceRaycastUpdate();
			_Collider = GetNode().GetCollider();
		}
		
		/// <summary>
		/// Sets the collision transform.
		/// </summary>
		/// <param name="GlobalTrans">The global transform to set.</param>
		public void SetTransform(Transform3D GlobalTrans)
		{
			if( GetNode() == null ) 
			{
				return;
			}
			
			GetNode().GlobalTransform = GlobalTrans;
		}
			
		/// <summary>
		/// Resets the current collider.
		/// </summary>
		public void ResetCollider()
		{
			_Collider = null;
		}
		
		/// <summary>
		/// Fetches the collision transform.
		/// </summary>
		/// <returns>The collision transform.</returns>
		public Transform3D GetTransform()
		{
			if( GetNode() == null ) 
			{
				return new Transform3D(Basis.Identity, Vector3.Up);
			}
			
			return GetNode().GlobalTransform;
		}
		
		/// <summary>
        /// Fetches an instance of the raycast.
        /// </summary>
        /// <returns>The RayCast3D instance.</returns>
		public RayCast3D GetNode()
		{
			if(
				false == GlobalExplorer.GetInstance()._Plugin.GetInternalContainer().HasNode( "RayCast" ) ||
				false == EditorPlugin.IsInstanceValid(GlobalExplorer.GetInstance()._Plugin.GetInternalContainer().GetNode("RayCast"))
			) 
			{
				if( false == GlobalExplorer.GetInstance()._Plugin.GetInternalContainer().HasNode( "RayCast" ) )
				{
					InitializeNode();
				}
				else 
				{
					return null;
				}
			}
			
			return GlobalExplorer.GetInstance()._Plugin.GetInternalContainer().GetNode("RayCast") as RayCast3D;
		}
		
		/// <summary>
        /// Fetches the collision instance.
        /// </summary>
        /// <returns>The collision instance.</returns>
		public GodotObject GetCollider()
		{
			return _Collider;
		}
		
		/// <summary>
        /// Checks if any collision is available.
        /// </summary>
        /// <returns>True if a collision is available, false otherwise.</returns>
		public bool HasCollision()
		{
			return _Collider != null;
		}
			
		/// <summary>
        /// Checks if the raycast is hidden.
        /// </summary>
        /// <returns>True if the raycast is hidden, false otherwise.</returns>
		public bool IsHidden()
		{
			return GetNode() != null && GetNode().Visible == false;
		}
		
		/// <summary>
        /// Initializes the raycast node.
        /// </summary>
		private void InitializeNode()
		{
			RayCast3D Node = new() 
			{
				Name = "RayCast",
				CollisionMask = 100,
			};
			
			GlobalExplorer.GetInstance()
				._Plugin
				.GetInternalContainer()
				.AddChild(Node);
		}
		
		/// <summary>
        /// Clears the raycast node.
        /// </summary>	
		private void ClearNode()
		{
			// Node.GetParent().RemoveChild(Node);
			GetNode().QueueFree();
		}
	}
}
#endif