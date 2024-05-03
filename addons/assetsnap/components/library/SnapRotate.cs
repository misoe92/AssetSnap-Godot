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
using AssetSnap.Component;
using AssetSnap.Front.Nodes;
using AssetSnap.Explorer;

namespace AssetSnap.Front.Components.Library
{
	/// <summary>
	/// Component for snapping rotation values.
	/// </summary>
	[Tool]
	public partial class SnapRotate : LibraryComponent
	{
		/// <summary>
		/// A flag indicating whether rotation is active.
		/// </summary>
		public bool value = false;
		
		/// <summary>
		/// The rotation angle along the X-axis.
		/// </summary>
		public float RotationX = 0.0f;
		
		/// <summary>
		/// The rotation angle along the Y-axis.
		/// </summary>
		public float RotationY = 0.0f;
		
		/// <summary>
		/// The rotation angle along the Z-axis.
		/// </summary>
		public float RotationZ = 0.0f;
		
		/// <summary>
		/// The current input event.
		/// </summary>
		public InputEvent CurrentEvent;

		/// <summary>
        /// Default constructor for SnapRotate.
        /// </summary>
		public SnapRotate()
		{
			Name = "LibrarySnapRotate";
			//_include = false;  
		}
		
		/// <summary>
        /// Initialization of the SnapRotate component.
        /// </summary>
		public override void Initialize() 
		{
			UsingTraits = new(){};
			
			base.Initialize();
			
			if( ExplorerUtils.Get() == null )
			{
				return;
			}
			
			if( null == ExplorerUtils.Get().ContextMenu ) 
			{
				return;
			}

			ContextMenu.Base ContextMenu = ExplorerUtils.Get().ContextMenu;
			
			value = false;
			RotationX = 0.0f;
			RotationY = 0.0f;
			RotationZ = 0.0f;
			
			Callable StateChangeCallable = new(this, "_OnListStateChange");
			if( StateChangeCallable is Callable callable && null != ContextMenu.GetInstance()) 
			{
				ContextMenu.GetInstance().Connect(AsContextMenu.SignalName.QuickActionsChanged, callable);
			}
		}
		
		/// <summary>
        /// Updates rotation values on input changes.
        /// </summary>
        /// <param name="which">The type of change.</param>
		private void _OnListStateChange( string which ) 
		{
			if( null == ExplorerUtils.Get().ContextMenu ) 
			{
				return;
			}
			
			if( which == "Rotate" )  
			{
				Vector3 _Rotation = ExplorerUtils.Get().ContextMenu.GetRotateValues();
				
				RotationX = _Rotation.X;
				RotationY = _Rotation.Y;
				RotationZ = _Rotation.Z;
				
				value = true; 
			}
			else 
			{
				value = false;
			}
		}
		
		/// <summary>
        /// Handles input events for rotation.
        /// </summary>
        /// <param name="event">The input event.</param>
		public override void _Input(InputEvent @event)
		{
			if(
				null == ExplorerUtils.Get() ||
				false == Plugin.Singleton.HasInternalContainer()
			) 
			{ 
				return;
			}

			InputEvent Event = @event;
			CurrentEvent = Event;
			int angle = ExplorerUtils.Get().ContextMenu.GetCurrentAngle();
			Node3D Handle = ExplorerUtils.Get().GetHandle();
			
			if( Handle == null ) 
			{
				return;
			}
			
			Vector3 Value = Handle.RotationDegrees; 
			
			if( ShouldRotateAll(angle) ) 
			{
				DoRotateAll(Value, Handle);
				ExplorerUtils.Get().AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;
			}
			else if( ShouldRotateX(angle) ) 
			{
				DoRotateX(Value, Handle);
				ExplorerUtils.Get().AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;
			}
			else if( ShouldRotateY(angle) ) 
			{
				DoRotateY(Value, Handle);
				ExplorerUtils.Get().AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;
			}
			else if( ShouldRotateZ(angle) ) 
			{
				DoRotateZ(Value, Handle);
				ExplorerUtils.Get().AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;
			}
		}
		
		/// <summary>
        /// Rotates the node on all angles.
        /// </summary>
        /// <param name="Rotation">The current rotation.</param>
        /// <param name="Handle">The node handle.</param>
		public void DoRotateAll( Vector3 Rotation, Node3D Handle )
		{
			Vector3 _RotationDegrees = Handle.RotationDegrees;
			if( IsWheelUp() ) 
			{
				
				_RotationDegrees = Apply("X", _RotationDegrees, RotationX, 0);
				_RotationDegrees = Apply("Y", _RotationDegrees, RotationY, 0);
				_RotationDegrees = Apply("Z", _RotationDegrees, RotationZ, 0);
					
				RotationX = _RotationDegrees.X;
				RotationY = _RotationDegrees.Y;
				RotationZ = _RotationDegrees.Z;
			}
			else if( IsWheelDown() ) 
			{
				_RotationDegrees = Apply("X", _RotationDegrees, RotationX, 0, true);
				_RotationDegrees = Apply("Y", _RotationDegrees, RotationY, 0, true);
				_RotationDegrees = Apply("Z", _RotationDegrees, RotationZ, 0, true);
				
				RotationX = _RotationDegrees.X;
				RotationY = _RotationDegrees.Y;
				RotationZ = _RotationDegrees.Z;
			}
			ExplorerUtils.Get().ContextMenu.SetRotationValues(_RotationDegrees);
		}
		
		/// <summary>
        /// Rotates the node along the X-axis.
        /// </summary>
        /// <param name="Rotation">The current rotation.</param>
        /// <param name="Handle">The node handle.</param>
		public void DoRotateX( Vector3 Rotation, Node3D Handle )
		{
			if( IsWheelUp() ) 
			{
				Vector3 _RotationDegrees = Apply("X", Rotation, RotationX, 0);
				RotationX = _RotationDegrees.X;
				ExplorerUtils.Get().ContextMenu.SetRotationValues(_RotationDegrees);
			}
			else if( IsWheelDown() )
			{
				Vector3 _RotationDegrees = Apply("X", Rotation, RotationX, 0, true);
				RotationX = _RotationDegrees.X;
				ExplorerUtils.Get().ContextMenu.SetRotationValues(_RotationDegrees);
			}
		}
		
		/// <summary>
        /// Rotates the node along the Y-axis.
        /// </summary>
        /// <param name="Rotation">The current rotation.</param>
        /// <param name="Handle">The node handle.</param>
		public void DoRotateY( Vector3 Rotation, Node3D Handle )
		{
			if( IsWheelUp() ) 
			{
				Vector3 _RotationDegrees = Apply("Y", Rotation, RotationY, 0);
				RotationY = _RotationDegrees.Y;
				ExplorerUtils.Get().ContextMenu.SetRotationValues(_RotationDegrees);
			}
			else if( IsWheelDown() ) 
			{
				Vector3 _RotationDegrees = Apply("Y", Rotation, RotationY, 0, true);
				RotationY = _RotationDegrees.Y;
				ExplorerUtils.Get().ContextMenu.SetRotationValues(_RotationDegrees);
			}
		}
		
		/// <summary>
        /// Rotates the node along the Z-axis.
        /// </summary>
        /// <param name="Rotation">The current rotation.</param>
        /// <param name="Handle">The node handle.</param>
		public void DoRotateZ( Vector3 Rotation, Node3D Handle )
		{
			if( IsWheelUp() ) 	
			{
				Vector3 _RotationDegrees = Apply("Z", Rotation, RotationZ, 0);
				RotationZ = _RotationDegrees.Z;
				ExplorerUtils.Get().ContextMenu.SetRotationValues(_RotationDegrees);
			}
			else if( IsWheelDown() )
			{
				Vector3 _RotationDegrees = Apply("Z", Rotation, RotationZ, 0, true);
				RotationZ = _RotationDegrees.Z;
				ExplorerUtils.Get().ContextMenu.SetRotationValues(_RotationDegrees);
			}
		}
		
		/// <summary>
        /// Applies the rotation changes.
        /// </summary>
        /// <param name="angle">The angle of rotation.</param>
        /// <param name="Rotation">The current rotation.</param>
        /// <param name="CurrentAngleRotation">The current angle of rotation.</param>
        /// <param name="_default">The default rotation value.</param>
        /// <param name="reverse">Whether to reverse the rotation.</param>
        /// <returns>The updated rotation vector.</returns>
		public Vector3 Apply( string angle, Vector3 Rotation, float CurrentAngleRotation, float _default, bool reverse = false )
		{
			bool specific = false;
			
			if(CurrentAngleRotation != -1) 
			{
				if( false == reverse) 
				{
					if(CurrentAngleRotation > 354)
					{
						CurrentAngleRotation = -5;
					}
				}
				else 
				{
					if(CurrentAngleRotation - 5 <= 0) 
					{
						CurrentAngleRotation = 365 + CurrentAngleRotation;
					}
				}
				
				if( false == reverse ) 
				{
					CurrentAngleRotation += 5;				
				}
				else 
				{
					CurrentAngleRotation -= 5;				
				}
				
				if( angle == "X" ) 
				{
					Rotation.X = CurrentAngleRotation;
				}
				else if (angle == "Y") 
				{
					Rotation.Y = CurrentAngleRotation;
				}
				else if (angle == "Z") 
				{
					Rotation.Z = CurrentAngleRotation;
				}
				
				specific = true;
			}


			if ( false == specific ) 
			{
				if( angle == "X" ) 
				{
					return new Vector3(_default,Rotation.Y,Rotation.Z);
				}
				else if (angle == "Y") 
				{
					return new Vector3(Rotation.X,_default,Rotation.Z);
				}
				else if (angle == "Z") 
				{
					return new Vector3(Rotation.X,Rotation.Y,_default);
				}

				return Vector3.Zero;
			}
			
			return Rotation;
		}
		
		/// <summary>
        /// Checks if the mouse wheel up event is active.
        /// </summary>
        /// <returns>True if mouse wheel up is active, false otherwise.</returns>
		public bool IsWheelUp()
		{
			if( CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				return MouseButtonEvent.ButtonIndex == MouseButton.WheelUp && false == MouseButtonEvent.Pressed;
			}

			return false;
		}
		
		/// <summary>
        /// Checks if the mouse wheel down event is active.
        /// </summary>
        /// <returns>True if mouse wheel down is active, false otherwise.</returns>
		public bool IsWheelDown()
		{
			if( CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				return MouseButtonEvent.ButtonIndex == MouseButton.WheelDown && false == MouseButtonEvent.Pressed;
			}

			return false;
		} 
		
		/// <summary>
        /// Checks if rotation on all angles should occur.
        /// </summary>
        /// <param name="angle">The angle of rotation.</param>
        /// <returns>True if rotation on all angles should occur, false otherwise.</returns>
		public bool ShouldRotateAll( int angle ) 
		{
			if( value == true && CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				if( MouseButtonEvent.AltPressed && MouseButtonEvent.ShiftPressed ) 
				{
					return angle == 0;
				}
			}

			return false;
		}
		
		/// <summary>
        /// Checks if rotation on the X-axis should occur.
        /// </summary>
        /// <param name="angle">The angle of rotation.</param>
        /// <returns>True if rotation on the X-axis should occur, false otherwise.</returns>
		public bool ShouldRotateX( int angle ) 
		{
			if( value == true && CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				if( MouseButtonEvent.AltPressed && MouseButtonEvent.ShiftPressed ) 
				{
					return angle == 1;
				}
			}
			
			return false;
		}
		
		/// <summary>
        /// Checks if rotation on the Y-axis should occur.
        /// </summary>
        /// <param name="angle">The angle of rotation.</param>
        /// <returns>True if rotation on the Y-axis should occur, false otherwise.</returns>
		public bool ShouldRotateY( int angle ) 
		{
			if( value == true && CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				if( MouseButtonEvent.AltPressed && MouseButtonEvent.ShiftPressed ) 
				{
					return angle == 2;
				}
			}
			
			return false;
		}
		
		/// <summary>
        /// Checks if rotation on the Z-axis should occur.
        /// </summary>
        /// <param name="angle">The angle of rotation.</param>
        /// <returns>True if rotation on the Z-axis should occur, false otherwise.</returns>
		public bool ShouldRotateZ( int angle ) 
		{
			if( value == true && CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				if( MouseButtonEvent.AltPressed && MouseButtonEvent.ShiftPressed ) 
				{
					return angle == 3;
				}
			}
			
			return false;
		}

		/// <summary>
        /// Checks if the rotation state is currently active.
        /// </summary>
        /// <returns>True if rotation is active, false otherwise.</returns>
		public bool IsActive()
		{
			return value == true;
		}
		
		/// <summary>
        /// Fetches the current rotation vector.
        /// </summary>
        /// <returns>The current rotation vector.</returns>
		public Vector3 GetRotationVector() 
		{
			return new Vector3(RotationX, RotationY, RotationZ);
		}
	}
}

#endif