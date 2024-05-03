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
	/// Component for snapping scale values.
	/// </summary>
	[Tool]
	public partial class SnapScale : LibraryComponent
	{ 
		/// <summary>
		/// The height at which snapping occurs.
		/// </summary>
		private float SnapHeight;
		
		/// <summary>
		/// The scale factor along the X-axis.
		/// </summary>
		public float ScaleX;
		
		/// <summary>
		/// The scale factor along the Y-axis.
		/// </summary>
		public float ScaleY;
		
		/// <summary>
		/// The scale factor along the Z-axis.
		/// </summary>
		public float ScaleZ;
		
		/// <summary>
		/// A flag indicating whether scaling is active.
		/// </summary>
		public bool value;
		
		/// <summary>
		/// The callable object for handling state changes.
		/// </summary>
		public Callable? StateChangeCallable;
		
		/// <summary>
		/// The current input event.
		/// </summary>
		public InputEvent CurrentEvent;

		/// <summary>
        /// Default constructor for SnapScale.
        /// </summary>
		public SnapScale()
		{
			Name = "LibrarySnapScale";
			
			UsingTraits = new(){};
			
			//_include = false; 
		}
		
		/// <summary>
		/// Initialization of the component.
		/// </summary>
		/// <returns>void</returns>
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
		
		/// <summary>
		/// Updates scale values on input changes.
		/// </summary>
		/// <param name="which">The type of scale change.</param>
		/// <returns>void</returns>
		private void _OnScaleListStateChange( string which )
		{
			if( null == ExplorerUtils.Get().ContextMenu ) 
			{
				return;
			}
			
			if( which == "Scale" )  
			{
				value = true;
				
				Vector3 _Scale = ExplorerUtils.Get().ContextMenu.GetScaleValues();
				
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
		
		/// <summary>
		/// Checks if scale is currently active and whether or not to apply it.
		/// </summary>
		/// <param name="@event">The input event.</param>
		/// <returns>void</returns>
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

			if( ShouldScaleAll(angle) ) 
			{
				DoScaleAll(Handle);
				ExplorerUtils.Get().AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;
			}
			else if( ShouldScaleX(angle) ) 
			{
				DoScaleX(Handle);
				ExplorerUtils.Get().AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;
			}
			else if( ShouldScaleY(angle) ) 
			{
				DoScaleY(Handle);
				ExplorerUtils.Get().AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;
			}
			else if( ShouldScaleZ(angle) ) 
			{
				DoScaleZ(Handle);
				ExplorerUtils.Get().AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;
			}

			base._Input(@event);
		}
				
		/// <summary>
		/// Scales on all angles.
		/// </summary>
		/// <param name="Handle">The node to scale.</param>
		/// <returns>void</returns>
		public void DoScaleAll( Node3D Handle )
		{
			Vector3 _Scale = ExplorerUtils.Get().ContextMenu.GetScaleValues();
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

			ExplorerUtils.Get().ContextMenu.SetScaleValues(_Scale);
				
			if( Handle is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D) 
			{
				asMeshInstance3D.UpdateWaypointScale();
			}
		}
				
		/// <summary>
		/// Scales on x angle.
		/// </summary>
		/// <param name="Handle">The node to scale.</param>
		/// <returns>void</returns>
		public void DoScaleX( Node3D Handle )
		{
			Vector3 _Scale = ExplorerUtils.Get().ContextMenu.GetScaleValues();
			
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
			
			ExplorerUtils.Get().ContextMenu.SetScaleValues(_Scale);
				
			if( Handle is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D) 
			{
				asMeshInstance3D.UpdateWaypointScale();
			}
		}
		
		/// <summary>
		/// Scales on y angle.
		/// </summary>
		/// <param name="Handle">The node to scale.</param>
		/// <returns>void</returns>
		public void DoScaleY( Node3D Handle )
		{
			Vector3 _Scale = ExplorerUtils.Get().ContextMenu.GetScaleValues();
			
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
			
			ExplorerUtils.Get().ContextMenu.SetScaleValues(_Scale);
				
			if( Handle is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D) 
			{
				asMeshInstance3D.UpdateWaypointScale();
			}
		}
		
		/// <summary>
		/// Scales on z angle.
		/// </summary>
		/// <param name="Handle">The node to scale.</param>
		/// <returns>void</returns>
		public void DoScaleZ( Node3D Handle )
		{
			Vector3 _Scale = ExplorerUtils.Get().ContextMenu.GetScaleValues();
			
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
			
			ExplorerUtils.Get().ContextMenu.SetScaleValues(_Scale);
			
			if( Handle is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D) 
			{
				asMeshInstance3D.UpdateWaypointScale();
			}
		}
		
		/// <summary>
		/// Applies the scale changes.
		/// </summary>
		/// <param name="angle">The angle to apply the scale.</param>
		/// <param name="Scale">The current scale.</param>
		/// <param name="CurrentAngleScale">The current angle scale.</param>
		/// <param name="_default">The default value.</param>
		/// <param name="reverse">Whether to reverse the scale change.</param>
		/// <returns>The updated scale.</returns>
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
		
		/// <summary>
		/// Checks if mouse wheel up is active.
		/// </summary>
		/// <returns>True if the mouse wheel is scrolling up, otherwise false.</returns>
		public bool IsWheelUp()
		{
			if( CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				return MouseButtonEvent.ButtonIndex == MouseButton.WheelUp && false == MouseButtonEvent.Pressed;
			}

			return false;
		}
		
		/// <summary>
		/// Checks if mouse wheel down is active.
		/// </summary>
		/// <returns>True if the mouse wheel is scrolling down, otherwise false.</returns>
		public bool IsWheelDown()
		{
			if( CurrentEvent is InputEventMouseButton MouseButtonEvent ) 
			{
				return MouseButtonEvent.ButtonIndex == MouseButton.WheelDown && false == MouseButtonEvent.Pressed;
			} 

			return false;
		} 
		
		/// <summary>
		/// Checks if scale on all angles should occur.
		/// </summary>
		/// <param name="angle">The current angle.</param>
		/// <returns>True if scale on all angles should occur, otherwise false.</returns>
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
		
		/// <summary>
		/// Checks if scale on x angle should occur.
		/// </summary>
		/// <param name="angle">The current angle.</param>
		/// <returns>True if scale on x angle should occur, otherwise false.</returns>
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
		
		/// <summary>
		/// Checks if scale on y angle should occur.
		/// </summary>
		/// <param name="angle">The current angle.</param>
		/// <returns>True if scale on y angle should occur, otherwise false.</returns>
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
		
		/// <summary>
		/// Checks if scale on z angle should occur.
		/// </summary>
		/// <param name="angle">The current angle.</param>
		/// <returns>True if scale on z angle should occur, otherwise false.</returns>
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
			
		/// <summary>
		/// Fetches the current snap scale.
		/// </summary>
		/// <returns>The current snap height.</returns>
		public float GetSnapHeight() 
		{
			return SnapHeight;
		}
	}
}

#endif