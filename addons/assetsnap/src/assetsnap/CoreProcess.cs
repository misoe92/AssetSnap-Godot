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
	using AssetSnap.Explorer;
	using AssetSnap.Front.Components.Library;
	using AssetSnap.Front.Components.Library.Sidebar;

	using AssetSnap.Front.Nodes;
	using AssetSnap.Settings;
	using AssetSnap.States;
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
		public void Tick(double delta)
		{
			if (null == ExplorerUtils.Get() || null == SettingsUtils.Get() || false == EditorPlugin.IsInstanceValid(Plugin.Singleton) )
			{
				return;
			}
			
			if( false == Plugin.Singleton.HasInternalContainer() ) 
			{
				return;
			}
			
			if( false == EditorPlugin.IsInstanceValid( ExplorerUtils.Get().GetHandle() ) ) 
			{
				StatesUtils.Get().EditingObject = null;
				StatesUtils.Get().EditingTitle = null;
			}

			if (null != ExplorerUtils.Get().GroupBuilder && EditorPlugin.IsInstanceValid(Plugin.Singleton) && ExplorerUtils.Get().GroupBuilder.MenuVisible())
			{
				if (null == Plugin.Singleton.GetViewport())
				{
					return;
				}

				Vector2 mouseGlobalPosition = Plugin.Singleton.GetViewport().GetMousePosition();
				ExplorerUtils.Get().GroupBuilder.MaybeHideMenu(mouseGlobalPosition);
			}

			ExplorerUtils.Get().DeltaTime += (float)delta;

			if (
				null != ExplorerUtils.Get()._ForceFocus &&
				EditorPlugin.IsInstanceValid(ExplorerUtils.Get()._ForceFocus)
			)
			{
				if (
					EditorInterface.Singleton.GetInspector().GetEditedObject() != ExplorerUtils.Get()._ForceFocus
				)
				{
					EditorInterface.Singleton.EditNode(ExplorerUtils.Get()._ForceFocus);
				}

				ExplorerUtils.Get()._Forced += 1;

				if (5 == ExplorerUtils.Get()._Forced)
				{
					ExplorerUtils.Get()._ForceFocus = null;
					ExplorerUtils.Get()._Forced = 0;
				}
			}
			else if (
				null != ExplorerUtils.Get()._ForceFocus &&
				false == EditorPlugin.IsInstanceValid(ExplorerUtils.Get()._ForceFocus)
			)
			{
				GD.PushWarning("Invalid focus object");
				ExplorerUtils.Get()._ForceFocus = null;
				ExplorerUtils.Get()._Forced = 0;
			}

			if (
				ShouldHideContextMenu()
			)
			{
				ExplorerUtils.Get().ContextMenu.Hide();
			}
			else if (
				ShouldShowContextMenu()
			)
			{
				ExplorerUtils.Get().ContextMenu.Show();
			}

			if (
				false == HasRayProjections() ||
				(false == HasLibrary() && false == HandleIsGroup()) ||
				(false == HasHandle() && false == HandleIsGroup())
			)
			{
				if (HasDecal())
				{
					ExplorerUtils.Get().Decal.Hide();
				}
			}
			else
			{
				// Checks if current handle is a model
				if (HasDecal() && HandleIsGroup())
				{
					// If so we handle model specific operations
					ProcessGroupHandle();
				}
				// Checks if current handle is a model
				else if (HasDecal() && HandleIsModel() && false == HandleIsGroup())
				{
					// If so we handle model specific operations
					ProcessModelHandle();
				}
				// Checks if current handle is a node3d
				else if (HasDecal() && HandleIsNode3D() && false == HandleIsModel() && false == HandleIsGroup())
				{
					// If so we handle model specific operations
					ProcessModelHandle();
				}
			}

			ExplorerUtils.Get().Snap.Tick(delta);
		}

		private bool ShouldHideContextMenu()
		{
			if (null == ExplorerUtils.Get() || true == ExplorerUtils.Get().ContextMenu.IsHidden())
			{
				return false;
			}

			if (
				null == ExplorerUtils.Get().States.CurrentScene ||
				true == ExplorerUtils.Get().GroupMainScreen.Visible
			)
			{
				return true;
			}

			if (null == ExplorerUtils.Get().GetHandle())
			{
				return true;
			}

			Node3D Handle = ExplorerUtils.Get().GetHandle();

			if (Handle is AsMeshInstance3D meshInstance3D)
			{
				if (meshInstance3D.Floating == false)
				{
					return ExplorerUtils.Get().States.CurrentScene != ExplorerUtils.Get().States.EditingObject.Owner;
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
			if (null == ExplorerUtils.Get() || false == ExplorerUtils.Get().ContextMenu.IsHidden())
			{
				return false;
			}

			if (
				null == ExplorerUtils.Get().States.CurrentScene ||
				null != ExplorerUtils.Get().GroupMainScreen &&
				true == ExplorerUtils.Get().GroupMainScreen.Visible
			)
			{
				return false;
			}

			if (null != ExplorerUtils.Get().GroupedObject)
			{
				return true;
			}

			if (null == ExplorerUtils.Get().GetHandle())
			{
				return false;
			}

			Node3D Handle = ExplorerUtils.Get().GetHandle();

			if (Handle is AsMeshInstance3D meshInstance3D)
			{
				if (meshInstance3D.Floating == false)
				{
					return ExplorerUtils.Get().States.CurrentScene == ExplorerUtils.Get().GetHandle().Owner;
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
			if (ExplorerUtils.Get().Decal.IsHidden())
			{
				// If it is we show it
				ExplorerUtils.Get().Decal.Show();
			}
			// Checks if current mouse is event is motion
			if (MouseEventIsMove() && ExplorerUtils.Get().HasProjectNormal() && ExplorerUtils.Get().HasProjectOrigin())
			{
				ConfigureRayCast();
				Transform3D ItemTransform = new(Basis.Identity, Vector3.Up);

				// Checks if raycast did collide
				if (RaycastDidCollide())
				{
					ExplorerUtils.Get().PositionDraw = ExplorerUtils.Get().Raycast.GetNode().GetCollisionPoint();
					ItemTransform.Origin = ExplorerUtils.Get().GetPositionDrawn();

					// Checks if the object can and should snap to another object
					ItemTransform.Origin = HandleObjectSnap(ItemTransform.Origin);

					// Checks if glue is used and applies it
					if (UsesGlue(StatesUtils.Get().CurrentLibrary._LibrarySettings._LSSnapToHeight))
					{
						ItemTransform.Origin = ApplyGlue(ItemTransform.Origin, StatesUtils.Get().CurrentLibrary._LibrarySettings._LSSnapToHeight);
					}

					if (UsesGlue(StatesUtils.Get().CurrentLibrary._LibrarySettings._LSSnapToX))
					{
						ItemTransform.Origin = ApplyGlue(ItemTransform.Origin, StatesUtils.Get().CurrentLibrary._LibrarySettings._LSSnapToX);
					}

					if (UsesGlue(StatesUtils.Get().CurrentLibrary._LibrarySettings._LSSnapToZ))
					{
						ItemTransform.Origin = ApplyGlue(ItemTransform.Origin, StatesUtils.Get().CurrentLibrary._LibrarySettings._LSSnapToZ);
					}

					// Updates decal preview
					if (null != ExplorerUtils.Get().Decal)
					{
						ExplorerUtils.Get().Decal._UpdateDecalPreview(true);
					}
				}
				else
				{
					// Use mouse position and hard set height to 0
					ItemTransform.Origin = new Vector3(0, 0.25f, 0);
					ExplorerUtils.Get().Decal.Hide();
				}

				// Sets transform and resets the current mouse input
				ExplorerUtils.Get()._CurrentMouseInput = EventMouse.EventNone;
				ExplorerUtils.Get().Decal.SetTransform(ItemTransform);
			}
		}

		private void ProcessGroupHandle()
		{
			// Checks if our decal is hidden
			if (ExplorerUtils.Get().Decal.IsHidden())
			{
				// If it is we show it
				ExplorerUtils.Get().Decal.Show();
			}

			// Checks if current mouse is event is motion
			if (MouseEventIsMove() && ExplorerUtils.Get().HasProjectNormal() && ExplorerUtils.Get().HasProjectOrigin())
			{
				ConfigureRayCast();
				Transform3D ItemTransform = new(Basis.Identity, Vector3.Up);

				// Checks if raycast did collide
				if (RaycastDidCollide())
				{
					ExplorerUtils.Get().PositionDraw = ExplorerUtils.Get().Raycast.GetNode().GetCollisionPoint();
					ItemTransform.Origin = ExplorerUtils.Get().GetPositionDrawn();
					// Checks if the object can and should snap to another object
					ItemTransform.Origin = HandleObjectSnap(ItemTransform.Origin);

					// Checks if glue is used and applies it
					if (ExplorerUtils.Get().States.SnapToHeight == GlobalStates.LibraryStateEnum.Enabled)
					{
						ItemTransform.Origin.Y = ExplorerUtils.Get().States.SnapToHeightValue;
					}

					if (ExplorerUtils.Get().States.SnapToX == GlobalStates.LibraryStateEnum.Enabled)
					{
						ItemTransform.Origin.X = ExplorerUtils.Get().States.SnapToXValue;
					}

					if (ExplorerUtils.Get().States.SnapToZ == GlobalStates.LibraryStateEnum.Enabled)
					{
						ItemTransform.Origin.Z = ExplorerUtils.Get().States.SnapToZValue;
					}

					// Updates decal preview
					if (null != ExplorerUtils.Get().Decal)
					{
						ExplorerUtils.Get().Decal._UpdateDecalPreview(true);
					}
				}
				else
				{
					// Use mouse position and hard set height to 0
					ItemTransform.Origin = new Vector3(0, 0.25f, 0);
					ExplorerUtils.Get().Decal.Hide();
				}

				// Sets transform and resets the current mouse input
				ExplorerUtils.Get()._CurrentMouseInput = EventMouse.EventNone;
				ExplorerUtils.Get().Decal.SetTransform(ItemTransform);
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
		private Vector3 HandleObjectSnap(Vector3 Origin)
		{
			if (HasLibrary())
			{
				Node Handle = GetHandle();

				if (Handle is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D && HasLibrarySettings())
				{
					int SnapLayer = asMeshInstance3D.GetSetting<int>("_LSSnapLayer.value");
					if (ShouldSnap() && SnapStatic.CanSnap(Origin, SnapLayer))
					{
						Origin = SnapStatic.Snap(Origin, asMeshInstance3D.GetAabb(), SnapLayer);
					}
				}
				else if (Handle is AsNode3D asNode3D && HasLibrarySettings())
				{
					int SnapLayer = asNode3D.GetSetting<int>("_LSSnapLayer.value");
					if (ShouldSnap() && SnapStatic.CanSnap(Origin, SnapLayer))
					{
						Origin = SnapStatic.Snap(Origin, asNode3D.GetAabb(), SnapLayer);
					}
				}
			}
			else if (ExplorerUtils.Get().States.PlacingMode == GlobalStates.PlacingModeEnum.Group)
			{
				// Snap group style
				Node Handle = GetHandle();

				if (Handle is AsGrouped3D asGrouped3D)
				{
					Aabb aabb = asGrouped3D.GetAabb();
					if (ShouldSnap() && SnapStatic.CanSnap(Origin, asGrouped3D.SnapLayer))
					{
						Origin = SnapStatic.Snap(Origin, aabb, asGrouped3D.SnapLayer);
					}
				}
			}
			else
			{

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
			if (Object is SnapToHeight _objectHeight)
			{
				return _objectHeight.ApplyGlue(Origin);
			}

			if (Object is SnapToX _objectX)
			{
				return _objectX.ApplyGlue(Origin);
			}

			if (Object is SnapToZ _objectZ)
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
			if (null != ExplorerUtils.Get().Raycast)
			{
				ExplorerUtils.Get().Raycast.ResetCollider();

				Transform3D GlobalTrans = ExplorerUtils.Get().Raycast.GetTransform();
				GlobalTrans.Origin = ExplorerUtils.Get().GetProjectOrigin();
				GlobalTrans.Basis.Y = ExplorerUtils.Get().GetProjectNormal();
				ExplorerUtils.Get().Raycast.SetTransform(GlobalTrans);
				ExplorerUtils.Get().Raycast.TargetPosition = new Vector3(0, 1000, 0);
				ExplorerUtils.Get().Raycast.Update();
			}
		}

		/*
		** Fetches the instance of our
		** current handle
		**
		** @return Node
		*/
		private Node GetHandle()
		{
			return ExplorerUtils.Get().GetHandle();
		}

		/*
		** Checks if an handle is currently set
		**
		** @return bool
		*/
		private bool HasHandle()
		{
			return ExplorerUtils.Get().GetHandle() != null;
		}

		/*
		** Checks if decal is available
		**
		** @return bool
		*/
		private bool HasDecal()
		{
			return null != ExplorerUtils.Get().Decal;
		}

		/*
		** Checks if current handle is that of
		** a model
		*/
		private bool HandleIsModel()
		{
			return EditorPlugin.IsInstanceValid(ExplorerUtils.Get().GetHandle()) && ExplorerUtils.Get().GetHandle() is MeshInstance3D && ExplorerUtils.Get().States.PlacingMode == GlobalStates.PlacingModeEnum.Model;
		}

		private bool HandleIsNode3D()
		{
			return EditorPlugin.IsInstanceValid(ExplorerUtils.Get().GetHandle()) && ExplorerUtils.Get().GetHandle() is AsNode3D node3d && false == node3d.IsPlaced();
		}

		private bool HandleIsGroup()
		{
			return EditorPlugin.IsInstanceValid(ExplorerUtils.Get().GetHandle()) && ExplorerUtils.Get().GetHandle() is AsGrouped3D && ExplorerUtils.Get().States.PlacingMode == GlobalStates.PlacingModeEnum.Group;
		}

		/*
		** Checks if our raycast object have
		** collided
		**
		** @return bool
		*/
		private bool RaycastDidCollide()
		{
			return ExplorerUtils.Get().Raycast.HasCollision();
		}

		/*
		** Checks if glue is being used or not
		**
		** @param BaseComponent Object
		** @return bool
		*/
		private bool UsesGlue(BaseComponent Object)
		{
			if (Object is SnapToHeight _objectHeight)
			{
				return HasLibrary() && _objectHeight.IsSnapToGlue();
			}

			if (Object is SnapToX _objectX)
			{
				return HasLibrary() && _objectX.IsSnapToGlue();
			}

			if (Object is SnapToZ _objectZ)
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
			return ExplorerUtils.Get().States.SnapToObject == GlobalStates.LibraryStateEnum.Enabled;
		}

		/*
		** Checks if our active library has an active
		** library settings component bound to it
		**
		** @return bool
		*/
		private bool HasLibrarySettings()
		{
			if (false == HasLibrary())
			{
				return false;
			}

			return StatesUtils.Get().CurrentLibrary._LibrarySettings != null;
		}

		/*
		** Checks if we have an active library
		**
		** @return bool
		*/
		private bool HasLibrary()
		{
			return null != StatesUtils.Get().CurrentLibrary;
		}

		/*
		** Checks if we have a valid ray projection reading
		**
		** @return bool
		*/
		private bool HasRayProjections()
		{
			return ExplorerUtils.Get().ProjectRayOrigin != Vector3.Zero && ExplorerUtils.Get().ProjectRayNormal != Vector3.Zero;
		}

		/*
		** Checks if the current mouse event
		** is an Motion mouse event
		**
		** @return bool
		*/
		private bool MouseEventIsMove()
		{
			EventMouse MouseEvent = ExplorerUtils.Get()._CurrentMouseInput;
			return MouseEvent == EventMouse.EventMove;
		}
	}
}