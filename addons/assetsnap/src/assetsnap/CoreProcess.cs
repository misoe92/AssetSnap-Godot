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

namespace AssetSnap.Core 
{
	using AssetSnap.Component;
	using AssetSnap.Front.Components;
	using AssetSnap.Front.Nodes;
	using AssetSnap.Static;
	using Godot;
	
	public class CoreProcess : Core 
	{
		/*
		** Handle of plugin process ticks
		**
		** @param double delta
		** @return void
		*/
		public void Tick( double delta ) 
		{
			if( null == _GlobalExplorer || null == _GlobalExplorer.Settings) 
			{
				return;
			}

			_GlobalExplorer.Snap.Tick(delta);
			
			if( null != _GlobalExplorer.GroupBuilder ) 
			{
				Vector2 mouseGlobalPosition = _GlobalExplorer._Plugin.GetViewport().GetMousePosition();
				_GlobalExplorer.GroupBuilder.MaybeHideMenu(mouseGlobalPosition);	
			}		
			
			_GlobalExplorer.DeltaTime += (float)delta;
			
			if( null != _GlobalExplorer._ForceFocus && EditorPlugin.IsInstanceValid( _GlobalExplorer._ForceFocus )) 
			{
				if( EditorInterface.Singleton.GetInspector().GetEditedObject() != _GlobalExplorer._ForceFocus ) 
				{
					EditorInterface.Singleton.EditNode(_GlobalExplorer._ForceFocus);
				}

				_GlobalExplorer._Forced += 1;
				
				if( 5 == _GlobalExplorer._Forced ) 
				{
					_GlobalExplorer._ForceFocus = null;
					_GlobalExplorer._Forced = 0;
				}
			}
			else if(null != _GlobalExplorer._ForceFocus && false == EditorPlugin.IsInstanceValid( _GlobalExplorer._ForceFocus )) 
			{
				GD.PushWarning("Invalid focus object");
				_GlobalExplorer._ForceFocus = null;
				_GlobalExplorer._Forced = 0;
			}

			if(
				ShouldHideContextMenu()
			) 
			{
				_GlobalExplorer.ContextMenu.Hide();
			}
			else if (
				ShouldShowContextMenu()
			)
			{
				_GlobalExplorer.ContextMenu.Show();
			}

			if(
				false == HasRayProjections() ||
				( false == HasLibrary() && false == HandleIsGroup() ) ||
				( false == HasHandle() && false == HandleIsGroup() )
			) 
			{
				if( HasDecal() ) 
				{
					_GlobalExplorer.Decal.Hide();
				}

				return;
			}
	
			// Checks if current handle is a model
			if( HasDecal() && HandleIsGroup() ) 
			{
				// If so we handle model specific operations
				ProcessGroupHandle();
			}
			// Checks if current handle is a model
			else if( HasDecal() && HandleIsModel() && false == HandleIsGroup() ) 
			{
				// If so we handle model specific operations
				ProcessModelHandle();
			}
		}
		
		private bool ShouldHideContextMenu()
		{
			if( null == _GlobalExplorer || true == _GlobalExplorer.ContextMenu.IsHidden() ) 
			{
				return false;
			}
			
			if(
				null == _GlobalExplorer.States.CurrentScene ||
				true == _GlobalExplorer.GroupMainScreen.Visible
			) 
			{
				return true;
			}
			
			if( null == _GlobalExplorer.GetHandle() ) 
			{
				return true;
			}
			
			Node3D Handle = _GlobalExplorer.GetHandle();
			
			if( Handle is AsMeshInstance3D meshInstance3D ) 
			{
				if( meshInstance3D.Floating == false ) 
				{
					return _GlobalExplorer.States.CurrentScene != _GlobalExplorer.States.EditingObject.Owner;
				}
				else
				{
					return false;
				}
			}
			
			return false;
		}
		
		private bool ShouldShowContextMenu()
		{
			if( null == _GlobalExplorer || false == _GlobalExplorer.ContextMenu.IsHidden() ) 
			{
				return false;
			}
			
			if(
				null == _GlobalExplorer.States.CurrentScene ||
				null != _GlobalExplorer.GroupMainScreen && 
				true == _GlobalExplorer.GroupMainScreen.Visible
			) 
			{
				return false;
			}
			
			if( null != _GlobalExplorer.GroupedObject ) 
			{
				return true;
			}
			
			if( null == _GlobalExplorer.GetHandle() ) 
			{
				return false;
			}

			Node3D Handle = _GlobalExplorer.GetHandle();
			
			if( Handle is AsMeshInstance3D meshInstance3D ) 
			{
				if( meshInstance3D.Floating == false ) 
				{
					return _GlobalExplorer.States.CurrentScene == _GlobalExplorer.GetHandle().Owner;
				}
				else
				{
					return true;
				}
			}
			return true;
		}
		
		/*
		** Model specific process operations
		**
		** @return void
		*/		
		private void ProcessModelHandle()
		{ 
			// Checks if our decal is hidden
			if( _GlobalExplorer.Decal.IsHidden() ) 
			{
				// If it is we show it
				_GlobalExplorer.Decal.Show();
			}

			// Checks if current mouse is event is motion
			if( MouseEventIsMove() && _GlobalExplorer.HasProjectNormal() && _GlobalExplorer.HasProjectOrigin() )
			{
				ConfigureRayCast();
				Transform3D ItemTransform = new(Basis.Identity, Vector3.Up);
				
				// Checks if raycast did collide
				if( RaycastDidCollide() ) 
				{
					_GlobalExplorer.PositionDraw = _GlobalExplorer.Raycast.GetNode().GetCollisionPoint();
					ItemTransform.Origin = _GlobalExplorer.GetPositionDrawn();

					// Checks if the object can and should snap to another object
					ItemTransform.Origin = HandleObjectSnap(ItemTransform.Origin);
					
					// Checks if glue is used and applies it
					if( UsesGlue(_GlobalExplorer.CurrentLibrary._LibrarySettings._LSSnapToHeight) ) 
					{
						ItemTransform.Origin = ApplyGlue(ItemTransform.Origin,_GlobalExplorer.CurrentLibrary._LibrarySettings._LSSnapToHeight);
					}
					
					if( UsesGlue(_GlobalExplorer.CurrentLibrary._LibrarySettings._LSSnapToX) ) 
					{
						ItemTransform.Origin = ApplyGlue(ItemTransform.Origin,_GlobalExplorer.CurrentLibrary._LibrarySettings._LSSnapToX);
					}
					
					if( UsesGlue(_GlobalExplorer.CurrentLibrary._LibrarySettings._LSSnapToZ) ) 
					{
						ItemTransform.Origin = ApplyGlue(ItemTransform.Origin,_GlobalExplorer.CurrentLibrary._LibrarySettings._LSSnapToZ);
					}
					
					// Updates decal preview
					if( EditorPlugin.IsInstanceValid(_GlobalExplorer.Decal)) 
					{
						_GlobalExplorer.Decal._UpdateDecalPreview(true);						
					}
				}
				else 
				{
					// Use mouse position and hard set height to 0
					ItemTransform.Origin = new Vector3(0, 0.25f, 0);
					_GlobalExplorer.Decal.Hide();
				}
				
				// Sets transform and resets the current mouse input
				_GlobalExplorer._CurrentMouseInput = EventMouse.EventNone;
				_GlobalExplorer.Decal.SetTransform(ItemTransform);
			}
		}
		
		private void ProcessGroupHandle()
		{
			// Checks if our decal is hidden
			if( _GlobalExplorer.Decal.IsHidden() ) 
			{
				// If it is we show it
				_GlobalExplorer.Decal.Show();
			}
			
			// Checks if current mouse is event is motion
			if( MouseEventIsMove() && _GlobalExplorer.HasProjectNormal() && _GlobalExplorer.HasProjectOrigin() )
			{
				ConfigureRayCast();
				Transform3D ItemTransform = new(Basis.Identity, Vector3.Up);
				
				// Checks if raycast did collide
				if( RaycastDidCollide() ) 
				{
					_GlobalExplorer.PositionDraw = _GlobalExplorer.Raycast.GetNode().GetCollisionPoint();
					ItemTransform.Origin = _GlobalExplorer.GetPositionDrawn();
					// Checks if the object can and should snap to another object
					ItemTransform.Origin = HandleObjectSnap(ItemTransform.Origin);
					
					// Checks if glue is used and applies it
					if( _GlobalExplorer.States.SnapToHeight == GlobalStates.LibraryStateEnum.Enabled ) 
					{
						ItemTransform.Origin.Y = _GlobalExplorer.States.SnapToHeightValue;
					}
					
					if( _GlobalExplorer.States.SnapToX == GlobalStates.LibraryStateEnum.Enabled ) 
					{
						ItemTransform.Origin.X = _GlobalExplorer.States.SnapToXValue;
					}
					
					if( _GlobalExplorer.States.SnapToZ == GlobalStates.LibraryStateEnum.Enabled ) 
					{
						ItemTransform.Origin.Z = _GlobalExplorer.States.SnapToZValue;
					}
					
					// Updates decal preview
					if( EditorPlugin.IsInstanceValid(_GlobalExplorer.Decal)) 
					{
						_GlobalExplorer.Decal._UpdateDecalPreview(true);						
					}
				}
				else  
				{
					// Use mouse position and hard set height to 0
					ItemTransform.Origin = new Vector3(0, 0.25f, 0);
					_GlobalExplorer.Decal.Hide();
				}
				
				// Sets transform and resets the current mouse input
				_GlobalExplorer._CurrentMouseInput = EventMouse.EventNone;
				_GlobalExplorer.Decal.SetTransform(ItemTransform);
			}
		}
		
		/*
		** Checks if object snapping should occur, and if so
		** it then applies the snapping to the Vector3 value
		** and returns it
		**
		** @param Vector3 Origin
		** @return Vector3
		*/
		private Vector3 HandleObjectSnap( Vector3 Origin )
		{
			if( HasLibrary() ) 
			{
				Node Handle = GetHandle();
				
				if( HasLibrarySettings() && Handle is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D) 
				{
					int SnapLayer = asMeshInstance3D.GetSetting<int>("_LSSnapLayer.value");
					if ( ShouldSnap() && SnapStatic.CanSnap(Origin,SnapLayer)) 
					{
						Origin = SnapStatic.Snap(Origin, asMeshInstance3D.GetAabb(), SnapLayer);
					}
				}
			}
			else if( GlobalExplorer.GetInstance().States.PlacingMode == GlobalStates.PlacingModeEnum.Group ) 
			{
				// Snap group style
				Node Handle = GetHandle();
				
				if( Handle is AsGrouped3D asGrouped3D ) 
				{
					Aabb aabb = asGrouped3D.GetAabb();
					if ( ShouldSnap() && SnapStatic.CanSnap(Origin,asGrouped3D.SnapLayer)) 
					{
						Origin = SnapStatic.Snap(Origin, aabb, asGrouped3D.SnapLayer);
					}
				}
			}

			return Origin;
		}
		
		/*
		** Takes an Vector3 value and applies a fixed value on a certain
		** axis depending on how the glue options are set
		**
		** @param Vector3 Origin
		** @param BaseComponent Object
		** @return Vector3
		*/
		private Vector3 ApplyGlue(Vector3 Origin, BaseComponent Object)
		{
			if( Object is LSSnapToHeight _objectHeight ) 
			{
				return _objectHeight.ApplyGlue(Origin);
			}
			
			if( Object is LSSnapToX _objectX ) 
			{
				return _objectX.ApplyGlue(Origin);
			}

			if( Object is LSSnapToZ _objectZ ) 
			{
				return _objectZ.ApplyGlue(Origin);
			}
			

			return Origin;
		}
		
		/*
		** Configures and runs our raycast, so it's
		** reading can be used afterwards
		**
		** @return void
		*/
		private void ConfigureRayCast()
		{
			if( EditorPlugin.IsInstanceValid(_GlobalExplorer.Raycast) ) 
			{
				_GlobalExplorer.Raycast.ResetCollider();
				
				Transform3D GlobalTrans = _GlobalExplorer.Raycast.GetTransform();
				GlobalTrans.Origin = _GlobalExplorer.GetProjectOrigin();
				GlobalTrans.Basis.Y = _GlobalExplorer.GetProjectNormal();
				_GlobalExplorer.Raycast.SetTransform(GlobalTrans);
				_GlobalExplorer.Raycast.TargetPosition = new Vector3(0, 1000, 0);
				_GlobalExplorer.Raycast.Update();
			}
		}
		
		/*
		** Fetches the instance of our current
		** library settings
		**
		** @return LibrarySettings
		*/
		private LibrarySettings GetLibrarySettings()
		{
			if( false == HasLibrary() ) 
			{
				return null;
			}
			
			if( false == HasLibrarySettings() ) 
			{
				return null;
			}

			return _GlobalExplorer.CurrentLibrary._LibrarySettings;
		}
		
		/*
		** Fetches the instance of our
		** current handle
		**
		** @return Node
		*/
		private Node GetHandle()
		{
			return _GlobalExplorer.GetHandle();
		}
		
		/*
		** Checks if an handle is currently set
		**
		** @return bool
		*/
		private bool HasHandle()
		{
			return _GlobalExplorer.GetHandle() != null;
		}
		
		/*
		** Checks if decal is available
		**
		** @return bool
		*/
		private bool HasDecal()
		{
			return null != _GlobalExplorer.Decal;
		}
			
		/*
		** Checks if current handle is that of
		** a model
		*/
		private bool HandleIsModel()
		{
			return _GlobalExplorer.GetHandle() is MeshInstance3D && _GlobalExplorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Model;
		}
		
		private bool HandleIsGroup()
		{
			return _GlobalExplorer.GetHandle() is AsGrouped3D && _GlobalExplorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Group;
		}
		
		/*
		** Checks if our raycast object have
		** collided
		**
		** @return bool
		*/
		private bool RaycastDidCollide()
		{
			return _GlobalExplorer.Raycast.HasCollision();
		}
			
		/*
		** Checks if glue is being used or not
		**
		** @param BaseComponent Object
		** @return bool
		*/
		private bool UsesGlue(BaseComponent Object)
		{
			if( Object is LSSnapToHeight _objectHeight ) 
			{
				return HasLibrary() && _objectHeight.IsSnapToGlue();
			}
			
			if( Object is LSSnapToX _objectX ) 
			{
				return HasLibrary() && _objectX.IsSnapToGlue();
			}

			if( Object is LSSnapToZ _objectZ ) 
			{
				return HasLibrary() && _objectZ.IsSnapToGlue();
			}
			
			return false;
		}
		
		/*
		** Checks if object snapping is currently turned on.
		**
		** @return bool
		*/
		private bool ShouldSnap()
		{
			return _GlobalExplorer.States.SnapToObject == GlobalStates.LibraryStateEnum.Enabled;
		}
		
		/*
		** Checks if our active library has an active
		** library settings component bound to it
		**
		** @return bool
		*/
		private bool HasLibrarySettings()
		{
			if( false == HasLibrary() ) 
			{
				return false;
			}
			
			return _GlobalExplorer.CurrentLibrary._LibrarySettings != null;
		}
		
		/*
		** Checks if we have an active library
		**
		** @return bool
		*/
		private bool HasLibrary()
		{
			return null != _GlobalExplorer.CurrentLibrary;
		}
		
		/*
		** Checks if we have a valid ray projection reading
		**
		** @return bool
		*/
		private bool HasRayProjections()
		{
			return _GlobalExplorer.ProjectRayOrigin != Vector3.Zero && _GlobalExplorer.ProjectRayNormal != Vector3.Zero;
		}
		
		/*
		** Checks if the current mouse event
		** is an Motion mouse event
		**
		** @return bool
		*/
		private bool MouseEventIsMove()
		{
			EventMouse MouseEvent = _GlobalExplorer._CurrentMouseInput;
			return MouseEvent == EventMouse.EventMove;
		}
	}
}