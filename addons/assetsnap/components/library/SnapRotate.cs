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

namespace AssetSnap.Front.Components.Library
{
	using Godot;
	using AssetSnap.Component;
	using AssetSnap.Front.Nodes;
	using AssetSnap.Explorer;

	[Tool]
	public partial class SnapRotate : LibraryComponent
	{
		public bool value = false;
		public float RotationX = 0.0f;
		public float RotationY = 0.0f;
		public float RotationZ = 0.0f;

		public Callable? StateChangeCallable;
		public InputEvent CurrentEvent;

		public SnapRotate()
		{
			Name = "LibrarySnapRotate";
			
			UsingTraits = new(){};
			
			//_include = false;  
		}
		
		/*
		** Initialization of component
		**
		** @return void
		*/
		public override void Initialize() 
		{
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
			
			StateChangeCallable = new(this, "_OnListStateChange");
			if( StateChangeCallable is Callable callable && null != ContextMenu.GetInstance()) 
			{
				ContextMenu.GetInstance().Connect(AsContextMenu.SignalName.QuickActionsChanged, callable);
			}
		}
		
		/*
		** Updates rotation values on
		** input changes
		**
		** @return void
		*/
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
		
		/*
		** Checks if rotation is currently active
		** and whether or not to apply it
		**
		** @return void
		*/
		public override void _Input(InputEvent @event)
		{
			if(
				null == ExplorerUtils.Get()
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
		
		/*
		** Rotates on all angles
		**
		** @return void
		*/
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
		
		/*
		** Rotates on x angle
		**
		** @return void
		*/
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
		
		/*
		** Rotates on y angle
		**
		** @return void
		*/
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
		
		/*
		** Rotates on z angle
		**
		** @return void
		*/
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
		
		/*
		** Applies the rotation changes
		**
		** @return Vector3
		*/
		public Vector3 Apply( string angle, Vector3 Rotation, float CurrentAngleRotation, float _default, bool reverse = false )
		{
			bool specific = false;
			
			if(CurrentAngleRotation != -1) 
			{
				if( false == reverse) 
				{
					if(CurrentAngleRotation > 355)
					{
						CurrentAngleRotation = 0 - CurrentAngleRotation;
					}
				}
				else 
				{
					if(CurrentAngleRotation - 5 <= 0) 
					{
						CurrentAngleRotation = 360 + CurrentAngleRotation;
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
		
		/*
		** Checks if mouse wheel up is active
		**
		** @return bool
		*/
		public bool IsWheelUp()
		{
			if( CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				return MouseButtonEvent.ButtonIndex == MouseButton.WheelUp && false == MouseButtonEvent.Pressed;
			}

			return false;
		}
		
		/*
		** Checks if mouse wheel down is active
		**
		** @return bool
		*/
		public bool IsWheelDown()
		{
			if( CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				return MouseButtonEvent.ButtonIndex == MouseButton.WheelDown && false == MouseButtonEvent.Pressed;
			}

			return false;
		} 
		
		/*
		** Checks if rotation on all angles should occur
		**
		** @return bool
		*/
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
		
		/*
		** Checks if rotation on x angle should occur
		**
		** @return bool
		*/
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
		
		/*
		** Checks if rotation on y angle should occur
		**
		** @return bool
		*/
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
		
		/*
		** Checks if rotation on z angle should occur
		**
		** @return bool
		*/
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

		/*
		** Checks if the rotation state
		** is currently active
		**
		** @return bool
		*/
		public bool IsActive()
		{
			return value == true;
		}
		
		/*
		** Fetches the current rotation vector
		**
		** @return Vector3
		*/
		public Vector3 GetRotationVector() 
		{
			return new Vector3(RotationX, RotationY, RotationZ);
		}
	}
}