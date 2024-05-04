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

using AssetSnap.Front.Components.Library.Sidebar;
using AssetSnap.Front.Nodes;
using AssetSnap.Component;
using AssetSnap.Explorer;
using AssetSnap.Settings;
using AssetSnap.States;
using AssetSnap.Static;
using Godot;

namespace AssetSnap.Core
{
	/// <summary>
	/// Handles the processing logic for various operations in the plugin.
	/// </summary>
	public class CoreProcess : Core
	{
		/// <summary>
		/// Handle of plugin process ticks
		/// </summary>
		/// <param name="delta">The time elapsed since the last frame, in seconds.</param>
		/// <returns>void</returns>
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
				_ShouldHideContextMenu()
			)
			{
				ExplorerUtils.Get().ContextMenu.Hide();
			}
			else if (
				_ShouldShowContextMenu()
			)
			{
				ExplorerUtils.Get().ContextMenu.Show();
			}
			
			if( 
				StatesUtils.Get().EditingObject != null &&
				null != StatesUtils.Get().EditingObject.GetParent() &&
				StatesUtils.Get().EditingObject.GetParent().Name == "AsDecal"
			) 
			{
				ExplorerUtils.Get().Decal.Show();
			}
			else 
			{
				ExplorerUtils.Get().Decal.Hide();
			}

			if (
				false == _HasRayProjections() ||
				(false == _HasLibrary() && false == _HandleIsGroup()) ||
				(false == _HasHandle() && false == _HandleIsGroup())
			)
			{
				
			}
			else
			{
				// Checks if current handle is a model
				if (_HasDecal() && _HandleIsGroup())
				{
					// If so we handle model specific operations
					_ProcessGroupHandle();
				}
				// Checks if current handle is a model
				else if (_HasDecal() && _HandleIsModel() && false == _HandleIsGroup())
				{
					// If so we handle model specific operations
					_ProcessModelHandle();
				}
				// Checks if current handle is a node3d
				else if (_HasDecal() && _HandleIsNode3D() && false == _HandleIsModel() && false == _HandleIsGroup())
				{
					// If so we handle model specific operations
					_ProcessModelHandle();
				}
			}

			ExplorerUtils.Get().Snap.Tick(delta);
		}

		/// <summary>
		/// Determines whether the context menu should be hidden.
		/// </summary>
		/// <returns>True if the context menu should be hidden, otherwise false.</returns>
		private bool _ShouldHideContextMenu()
		{
			if (null == ExplorerUtils.Get() || null == ExplorerUtils.Get().ContextMenu || true == ExplorerUtils.Get().ContextMenu.IsHidden())
			{
				return false;
			}

			if (
				null == StatesUtils.Get().CurrentScene ||
				null != ExplorerUtils.Get().GroupMainScreen &&
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

		/// <summary>
        /// Determines whether the context menu should be shown.
        /// </summary>
        /// <returns>True if the context menu should be shown, otherwise false.</returns>
		private bool _ShouldShowContextMenu()
		{
			if (null == ExplorerUtils.Get() || null == ExplorerUtils.Get().ContextMenu || false == ExplorerUtils.Get().ContextMenu.IsHidden())
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

			if (null != StatesUtils.Get().GroupedObject)
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

		/// <summary>
		/// Model specific process operations
		/// </summary>
		/// <returns>void</returns>
		private void _ProcessModelHandle()
		{
			// Checks if current mouse is event is motion
			if (_MouseEventIsMove() && ExplorerUtils.Get().HasProjectNormal() && ExplorerUtils.Get().HasProjectOrigin())
			{
				_ConfigureRayCast();
				Transform3D ItemTransform = new(Basis.Identity, Vector3.Up);

				// Checks if raycast did collide
				if (_RaycastDidCollide())
				{
					ExplorerUtils.Get().PositionDraw = ExplorerUtils.Get().Raycast.GetNode().GetCollisionPoint();
					ItemTransform.Origin = ExplorerUtils.Get().GetPositionDrawn();

					// Checks if the object can and should snap to another object
					ItemTransform.Origin = _HandleObjectSnap(ItemTransform.Origin);

					// Checks if glue is used and applies it
					if (_UsesGlue(StatesUtils.Get().CurrentLibrary._LibrarySettings.SnapToHeight))
					{
						ItemTransform.Origin = _ApplyGlue(ItemTransform.Origin, StatesUtils.Get().CurrentLibrary._LibrarySettings.SnapToHeight);
					}

					if (_UsesGlue(StatesUtils.Get().CurrentLibrary._LibrarySettings.SnapToX))
					{
						ItemTransform.Origin = _ApplyGlue(ItemTransform.Origin, StatesUtils.Get().CurrentLibrary._LibrarySettings.SnapToX);
					}

					if (_UsesGlue(StatesUtils.Get().CurrentLibrary._LibrarySettings.SnapToZ))
					{
						ItemTransform.Origin = _ApplyGlue(ItemTransform.Origin, StatesUtils.Get().CurrentLibrary._LibrarySettings.SnapToZ);
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
				}

				// Sets transform and resets the current mouse input
				ExplorerUtils.Get()._CurrentMouseInput = EventMouse.EventNone;
				ExplorerUtils.Get().Decal.SetTransform(ItemTransform);
			}
		}

		/// <summary>
		/// Handles group specific process operations
		/// </summary>
		/// <returns>void</returns>
		private void _ProcessGroupHandle()
		{
			// Checks if current mouse is event is motion
			if (_MouseEventIsMove() && ExplorerUtils.Get().HasProjectNormal() && ExplorerUtils.Get().HasProjectOrigin())
			{
				_ConfigureRayCast();
				Transform3D ItemTransform = new(Basis.Identity, Vector3.Up);

				// Checks if raycast did collide
				if (_RaycastDidCollide())
				{
					ExplorerUtils.Get().PositionDraw = ExplorerUtils.Get().Raycast.GetNode().GetCollisionPoint();
					ItemTransform.Origin = ExplorerUtils.Get().GetPositionDrawn();
					// Checks if the object can and should snap to another object
					ItemTransform.Origin = _HandleObjectSnap(ItemTransform.Origin);

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
				}

				// Sets transform and resets the current mouse input
				ExplorerUtils.Get()._CurrentMouseInput = EventMouse.EventNone;
				ExplorerUtils.Get().Decal.SetTransform(ItemTransform);
			}
		}

		/// <summary>
		/// Checks if object snapping should occur, and if so,
		/// it then applies the snapping to the Vector3 value
		/// and returns it.
		/// </summary>
		/// <param name="Origin">The original Vector3 value.</param>
		/// <returns>The modified Vector3 value after snapping.</returns>
		private Vector3 _HandleObjectSnap(Vector3 Origin)
		{
			if (_HasLibrary())
			{
				Node Handle = _GetHandle();

				if (Handle is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D && _HasLibrarySettings())
				{
					int SnapLayer = asMeshInstance3D.GetSetting<int>("_LSSnapLayer.value");
					if (_ShouldSnap() && SnapStatic.CanSnap(Origin, SnapLayer))
					{
						Origin = SnapStatic.Snap(Origin, asMeshInstance3D.GetAabb(), SnapLayer);
					}
				}
				else if (Handle is AsNode3D asNode3D && _HasLibrarySettings())
				{
					int SnapLayer = asNode3D.GetSetting<int>("_LSSnapLayer.value");
					if (_ShouldSnap() && SnapStatic.CanSnap(Origin, SnapLayer))
					{
						Origin = SnapStatic.Snap(Origin, asNode3D.GetAabb(), SnapLayer);
					}
				}
			}
			else if (ExplorerUtils.Get().States.PlacingMode == GlobalStates.PlacingModeEnum.Group)
			{
				// Snap group style
				Node Handle = _GetHandle();

				if (Handle is AsGrouped3D asGrouped3D)
				{
					Aabb aabb = asGrouped3D.GetAabb();
					if (_ShouldSnap() && SnapStatic.CanSnap(Origin, asGrouped3D.SnapLayer))
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

		/// <summary>
		/// Takes a Vector3 value and applies a fixed value on a certain
		/// axis depending on how the glue options are set.
		/// </summary>
		/// <param name="Origin">The original Vector3 value.</param>
		/// <param name="Object">The object to apply glue to.</param>
		/// <returns>The modified Vector3 value after applying glue.</returns>
		private Vector3 _ApplyGlue(Vector3 Origin, BaseComponent Object)
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

		/// <summary>
		/// Configures and runs our raycast, so its
		/// reading can be used afterwards.
		/// </summary>
		/// <returns>void</returns>
		private void _ConfigureRayCast()
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

		/// <summary>
		/// Fetches the instance of our current handle.
		/// </summary>
		/// <returns>The current handle Node.</returns>
		private Node _GetHandle()
		{
			return ExplorerUtils.Get().GetHandle();
		}

		/// <summary>
		/// Checks if a handle is currently set.
		/// </summary>
		/// <returns>True if a handle is set, otherwise false.</returns>
		private bool _HasHandle()
		{
			return ExplorerUtils.Get().GetHandle() != null;
		}

		/// <summary>
		/// Checks if decal is available.
		/// </summary>
		/// <returns>True if a decal is available, otherwise false.</returns>
		private bool _HasDecal()
		{
			return null != ExplorerUtils.Get().Decal;
		}

		/// <summary>
		/// Checks if the current handle is that of a model.
		/// </summary>
		/// <returns>True if the current handle is that of a model, otherwise false.</returns>
		private bool _HandleIsModel()
		{
			return EditorPlugin.IsInstanceValid(ExplorerUtils.Get().GetHandle()) && ExplorerUtils.Get().GetHandle() is MeshInstance3D && ExplorerUtils.Get().States.PlacingMode == GlobalStates.PlacingModeEnum.Model;
		}

		/// <summary>
		/// Checks if the current handle is that of a Node3D.
		/// </summary>
		/// <returns>True if the current handle is that of a Node3D, otherwise false.</returns>
		private bool _HandleIsNode3D()
		{
			return EditorPlugin.IsInstanceValid(ExplorerUtils.Get().GetHandle()) && ExplorerUtils.Get().GetHandle() is AsNode3D node3d && false == node3d.IsPlaced();
		}

		/// <summary>
		/// Checks if the current handle is that of a group.
		/// </summary>
		/// <returns>True if the current handle is that of a group, otherwise false.</returns>
		private bool _HandleIsGroup()
		{
			return EditorPlugin.IsInstanceValid(ExplorerUtils.Get().GetHandle()) && ExplorerUtils.Get().GetHandle() is AsGrouped3D && ExplorerUtils.Get().States.PlacingMode == GlobalStates.PlacingModeEnum.Group;
		}

		/// <summary>
		/// Checks if our raycast object has collided.
		/// </summary>
		/// <returns>True if the raycast has collided, otherwise false.</returns>
		private bool _RaycastDidCollide()
		{
			return ExplorerUtils.Get().Raycast.HasCollision();
		}

		/// <summary>
		/// Checks if glue is being used or not.
		/// </summary>
		/// <param name="Object">The object to check for glue usage.</param>
		/// <returns>True if glue is being used, otherwise false.</returns>
		private bool _UsesGlue(BaseComponent Object)
		{
			if (Object is SnapToHeight _objectHeight)
			{
				return _HasLibrary() && _objectHeight.IsSnapToGlue();
			}

			if (Object is SnapToX _objectX)
			{
				return _HasLibrary() && _objectX.IsSnapToGlue();
			}

			if (Object is SnapToZ _objectZ)
			{
				return _HasLibrary() && _objectZ.IsSnapToGlue();
			}

			return false;
		}

		/// <summary>
		/// Checks if object snapping is currently turned on.
		/// </summary>
		/// <returns>True if object snapping is turned on, otherwise false.</returns>
		private bool _ShouldSnap()
		{
			return ExplorerUtils.Get().States.SnapToObject == GlobalStates.LibraryStateEnum.Enabled;
		}

		/// <summary>
		/// Checks if our active library has active
		/// library settings component bound to it.
		/// </summary>
		/// <returns>True if library settings are available, otherwise false.</returns>
		private bool _HasLibrarySettings()
		{
			if (false == _HasLibrary())
			{
				return false;
			}

			return StatesUtils.Get().CurrentLibrary._LibrarySettings != null;
		}

		/// <summary>
		/// Checks if we have an active library.
		/// </summary>
		/// <returns>True if an active library is available, otherwise false.</returns>
		private bool _HasLibrary()
		{
			return null != StatesUtils.Get().CurrentLibrary;
		}

		/// <summary>
		/// Checks if we have a valid ray projection reading.
		/// </summary>
		/// <returns>True if ray projections are valid, otherwise false.</returns>
		private bool _HasRayProjections()
		{
			return ExplorerUtils.Get().ProjectRayOrigin != Vector3.Zero && ExplorerUtils.Get().ProjectRayNormal != Vector3.Zero;
		}

		/// <summary>
		/// Checks if the current mouse event is a motion mouse event.
		/// </summary>
		/// <returns>True if the mouse event is a motion event, otherwise false.</returns>
		private bool _MouseEventIsMove()
		{
			EventMouse MouseEvent = ExplorerUtils.Get()._CurrentMouseInput;
			return MouseEvent == EventMouse.EventMove;
		}
	}
}

#endif