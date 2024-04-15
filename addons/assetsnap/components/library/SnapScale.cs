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

	[Tool]
	public partial class SnapScale : LibraryComponent
	{ 
		private float SnapHeight;
		
		public float ScaleX;
		public float ScaleY;
		public float ScaleZ;
		public bool value;
		
		public Callable? StateChangeCallable;
		public InputEvent CurrentEvent;

		public SnapScale()
		{
			Name = "LibrarySnapScale";
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
			
			if( _GlobalExplorer == null ) 
			{
				return;
			}
			
			if( null == _GlobalExplorer.ContextMenu ) 
			{
				return;
			}
			
			ContextMenu.Base ContextMenu = _GlobalExplorer.ContextMenu;
			
			ScaleX = 0.0f;
			ScaleY = 0.0f; 
			ScaleZ = 0.0f;
			SnapHeight = 0.1f; 
			value = false;

			StateChangeCallable = new(this, "_OnScaleListStateChange");
			if( StateChangeCallable is Callable _callable) 
			{
				ContextMenu.GetInstance().Connect(AsContextMenu.SignalName.QuickActionsChanged, _callable);
			}
		} 
		
		/*
		** Updates scale values on
		** input changes
		**
		** @return void
		*/
		private void _OnScaleListStateChange( string which )
		{
			if( null == _GlobalExplorer.ContextMenu ) 
			{
				return;
			}
			
			if( which == "Scale" )  
			{
				value = true;
				
				Vector3 _Scale = GlobalExplorer.GetInstance().ContextMenu.GetScaleValues();
				
				ScaleX = _Scale.X;
				ScaleY = _Scale.Y;
				ScaleZ = _Scale.Z;
			}
			else 
			{
				if( value == true ) 
				{
					// Means the value just changed 
					
				}
				value = false;
			}
		}
		
		/*
		** Checks if scale is currently active
		** and whether or not to apply it
		**
		** @return void 
		*/
		public override void _Input(InputEvent @event)
		{
			if(
				null == _GlobalExplorer ||
				false == EditorPlugin.IsInstanceValid( _GlobalExplorer.Model )  &&
				false == EditorPlugin.IsInstanceValid( _GlobalExplorer.HandleNode )
			) 
			{ 
				return;
			}

			InputEvent Event = @event; 
			CurrentEvent = Event; 
			int angle = _GlobalExplorer.ContextMenu.GetCurrentAngle();
			Node3D Handle = _GlobalExplorer.HandleNode as Node3D;
			
			if( null == Handle && null != _GlobalExplorer.Model )
			{ 
				Handle = _GlobalExplorer.Model;
			}
			
			if ( null == Handle ) 
			{
				return;
			}

			if( ShouldScaleAll(angle) ) 
			{
				DoScaleAll(Handle);
				_GlobalExplorer.AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;
			}
			else if( ShouldScaleX(angle) ) 
			{
				DoScaleX(Handle);
				_GlobalExplorer.AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;
			}
			else if( ShouldScaleY(angle) ) 
			{
				DoScaleY(Handle);
				_GlobalExplorer.AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;
			}
			else if( ShouldScaleZ(angle) ) 
			{
				DoScaleZ(Handle);
				_GlobalExplorer.AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;
			}

			base._Input(@event);
		}
				
		/*
		** Scales on all angles
		**
		** @return void
		*/
		public void DoScaleAll( Node3D Handle )
		{
			Vector3 _Scale = _GlobalExplorer.ContextMenu.GetScaleValues();
			if( IsWheelUp() ) 
			{
				_Scale = Apply("X", _Scale, ScaleX, 0);
				_Scale = Apply("Y", _Scale, ScaleY, 0);
				_Scale = Apply("Z", _Scale, ScaleZ, 0);
					
				ScaleX = _Scale.X;
				ScaleY = _Scale.Y;
				ScaleZ = _Scale.Z;
			}
			else if( IsWheelDown() ) 
			{
				_Scale = Apply("X", _Scale, ScaleX, 0, true);
				_Scale = Apply("Y", _Scale, ScaleY, 0, true);
				_Scale = Apply("Z", _Scale, ScaleZ, 0, true);
				
				ScaleX = _Scale.X;
				ScaleY = _Scale.Y;
				ScaleZ = _Scale.Z;
			}

			_GlobalExplorer.ContextMenu.SetScaleValues(_Scale);
				
			if( Handle is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D) 
			{
				asMeshInstance3D.UpdateWaypointScale();
			}
		}
				
		/*
		** Scales on x angle
		**
		** @return void
		*/
		public void DoScaleX( Node3D Handle )
		{
			Vector3 _Scale = _GlobalExplorer.ContextMenu.GetScaleValues();
			
			if( IsWheelUp() ) 
			{
				_Scale = Apply("X", _Scale, ScaleX, 0);
				ScaleX = _Scale.X;
			}
			else if( IsWheelDown() )
			{
				_Scale = Apply("X", _Scale, ScaleX, 0, true);
				ScaleX = _Scale.X;
			}
			
			_GlobalExplorer.ContextMenu.SetScaleValues(_Scale);
				
			if( Handle is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D) 
			{
				asMeshInstance3D.UpdateWaypointScale();
			}
		}
		
		/*
		** Scales on y angle
		**
		** @return void
		*/
		public void DoScaleY( Node3D Handle )
		{
			Vector3 _Scale = _GlobalExplorer.ContextMenu.GetScaleValues();
			
			if( IsWheelUp() ) 
			{
				_Scale = Apply("Y", _Scale, ScaleY, 0);
				ScaleY = _Scale.Y;
			}
			else if( IsWheelDown() )
			{
				_Scale = Apply("Y", _Scale, ScaleY, 0, true);
				ScaleY = _Scale.Y;
			}
			
			_GlobalExplorer.ContextMenu.SetScaleValues(_Scale);
				
			if( Handle is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D) 
			{
				asMeshInstance3D.UpdateWaypointScale();
			}
		}
		
		/*
		** Scales on z angle
		**
		** @return void
		*/
		public void DoScaleZ( Node3D Handle )
		{
			Vector3 _Scale = _GlobalExplorer.ContextMenu.GetScaleValues();
			
			if( IsWheelUp() ) 	
			{
				_Scale = Apply("Z", _Scale, ScaleZ, 0);
				ScaleZ = _Scale.Z;
			}
			else if( IsWheelDown() )
			{
				_Scale = Apply("Z", _Scale, ScaleZ, 0, true);
				ScaleZ = _Scale.Z;
			}
			
			_GlobalExplorer.ContextMenu.SetScaleValues(_Scale);
			
			if( Handle is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D) 
			{
				asMeshInstance3D.UpdateWaypointScale();
			}
		}
		
		/*
		** Applies the scale changes
		**
		** @return Vector3
		*/
		public Vector3 Apply( string angle, Vector3 Scale, float CurrentAngleScale, float _default, bool reverse = false )
		{
			bool specific = false;
			
			if(CurrentAngleScale != -1) 
			{
				if( reverse )
				{
					if(CurrentAngleScale < 0) 
					{
						CurrentAngleScale = 0;
					}
				}
				
				if( false == reverse ) 
				{
					CurrentAngleScale += .1f;				
				}
				else 
				{
					CurrentAngleScale -= .1f;				
				}
				
				if( angle == "X" ) 
				{
					Scale.X = CurrentAngleScale;
				}
				else if (angle == "Y") 
				{
					Scale.Y = CurrentAngleScale;
				}
				else if (angle == "Z") 
				{
					Scale.Z = CurrentAngleScale;
				}
				
				specific = true;
			}

			if ( false == specific ) 
			{
				if( angle == "X" ) 
				{
					return new Vector3(_default,Scale.Y,Scale.Z);
				}
				else if (angle == "Y") 
				{
					return new Vector3(Scale.X,_default,Scale.Z);
				}
				else if (angle == "Z") 
				{
					return new Vector3(Scale.X,Scale.Y,_default);
				}

				return Scale;
			}
			
			return Scale;
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
		** Checks if scale on all angles should occur
		**
		** @return bool
		*/
		public bool ShouldScaleAll( int angle ) 
		{
			if( value == true && CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				if( Input.IsKeyPressed(Key.Alt) && Input.IsKeyPressed(Key.Shift) ) 
				{
					return angle == 0;
				}
			}

			return false;
		}
		
		/*
		** Checks if scale on x angle should occur
		**
		** @return bool
		*/
		public bool ShouldScaleX( int angle ) 
		{
			if( value == true && CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				if( Input.IsKeyPressed(Key.Alt) && Input.IsKeyPressed(Key.Shift) ) 
				{
					return angle == 1;
				}
			}
			
			return false;
		}
		
		/*
		** Checks if scale on y angle should occur
		**
		** @return bool
		*/
		public bool ShouldScaleY( int angle ) 
		{
			if( value == true && CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				if( Input.IsKeyPressed(Key.Alt) && Input.IsKeyPressed(Key.Shift) ) 
				{
					return angle == 2;
				}
			}
			
			return false;
		}
		
		/*
		** Checks if scale on z angle should occur
		**
		** @return bool
		*/
		public bool ShouldScaleZ( int angle ) 
		{
			if( value == true && CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				if( Input.IsKeyPressed(Key.Alt) && Input.IsKeyPressed(Key.Shift) ) 
				{
					return angle == 3;
				}
			}
			
			return false;
		}
			
		/*
		** Fetches current snap scale
		**
		** @return float 
		*/
		public float GetSnapHeight() 
		{
			return SnapHeight;
		}
	}
}