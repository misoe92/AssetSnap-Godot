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

using System.Collections.Generic;
using AssetSnap.Explorer;
using AssetSnap.Settings;
using AssetSnap.States;

namespace AssetSnap.Snap
{
	/// <summary>
	/// Represents the base class for managing snap boundaries.
	/// </summary>
	public class Base
	{
		public static Base Singleton
		{
			get
			{
				if (null == _Instance)
				{
					_Instance = new();
				}

				return _Instance;
			}
		}

		public List<Boundary> boundaries = new List<Boundary>();

		private static Base _Instance;
		private float CurrentOpacity;
		private float CurrentX = 0;
		private float CurrentY = 0;
		private float CurrentZ = 0;

		/// <summary>
		/// Initializes the snap boundaries.
		/// </summary>
		public void Initialize()
		{
			if (null == ExplorerUtils.Get())
			{
				return;
			}

			CurrentOpacity = ExplorerUtils.Get().Settings.GetKey("boundary_box_opacity").As<float>();
			CurrentX = StatesUtils.Get().SnapToXValue;
			CurrentY = StatesUtils.Get().SnapToHeightValue;
			CurrentZ = StatesUtils.Get().SnapToZValue;

			// BoundaryOpacity = SettingsUtils.Get().GetKey("boundary_box_opacity").As<float>();

			Plugin.Singleton.SettingKeyChanged += (Godot.Collections.Array data) => { _MaybeUpdateOpacity(data); };
		}

		/// <summary>
        /// Handles the potential update of the boundary box opacity based on the provided data.
        /// </summary>
        /// <param name="data">The data containing information about the opacity change.</param>
		public void _MaybeUpdateOpacity(Godot.Collections.Array data)
		{
			if (data[0].As<string>() == "boundary_box_opacity")
			{
				SetBoxOpacity(data[1].As<float>());
			}
		}

		/// <summary>
		/// Sets the opacity of the snap boundary box.
		/// </summary>
		/// <param name="value">The opacity value to set.</param>
		public void SetBoxOpacity(float value)
		{
			// BoundaryOpacity = value;
			UpdateOpacity(value);
		}

		/// <summary>
		/// Updates the snap boundaries based on the elapsed time.
		/// </summary>
		/// <param name="delta">The time elapsed since the last frame.</param>
		public void Tick(double delta)
		{
			if (null == ExplorerUtils.Get())
			{
				return;
			}

			if (IsBoundaryShown())
			{
				UpdateTransform();
			}

			if (ShouldHideBoundary())
			{
				RemoveBoundaries();
			}
			else if (ShouldShowBoundary())
			{
				if (ShouldSnapToHeight() && false == IsAngleActive(GlobalStates.SnapAngleEnums.Y))
				{
					SpawnBoundary(GlobalStates.SnapAngleEnums.Y);
					StatesUtils.Get().BoundaryActiveAngles.Add(GlobalStates.SnapAngleEnums.Y);
				}
				else if (false == ShouldSnapToHeight() && IsAngleActive(GlobalStates.SnapAngleEnums.Y))
				{
					RemoveBoundary(GlobalStates.SnapAngleEnums.Y);
					StatesUtils.Get().BoundaryActiveAngles.Remove(GlobalStates.SnapAngleEnums.Y);
				}

				if (ShouldSnapToX() && false == IsAngleActive(GlobalStates.SnapAngleEnums.X))
				{
					SpawnBoundary(GlobalStates.SnapAngleEnums.X);
					StatesUtils.Get().BoundaryActiveAngles.Add(GlobalStates.SnapAngleEnums.X);
				}
				else if (false == ShouldSnapToX() && IsAngleActive(GlobalStates.SnapAngleEnums.X))
				{
					RemoveBoundary(GlobalStates.SnapAngleEnums.X);
					StatesUtils.Get().BoundaryActiveAngles.Remove(GlobalStates.SnapAngleEnums.X);
				}

				if (ShouldSnapToZ() && false == IsAngleActive(GlobalStates.SnapAngleEnums.Z))
				{
					SpawnBoundary(GlobalStates.SnapAngleEnums.Z);
					StatesUtils.Get().BoundaryActiveAngles.Add(GlobalStates.SnapAngleEnums.Z);
				}
				else if (false == ShouldSnapToZ() && IsAngleActive(GlobalStates.SnapAngleEnums.Z))
				{
					RemoveBoundary(GlobalStates.SnapAngleEnums.Z);
					StatesUtils.Get().BoundaryActiveAngles.Remove(GlobalStates.SnapAngleEnums.Z);
				}

				if (HasActiveAngle())
				{
					StatesUtils.Get().BoundarySpawned = GlobalStates.SpawnStateEnum.Spawned;
				}
				else
				{
					StatesUtils.Get().BoundarySpawned = GlobalStates.SpawnStateEnum.Null;
				}
			}
		}

		/// <summary>
		/// Spawns a boundary at the specified angle.
		/// </summary>
		/// <param name="angle">The angle at which to spawn the boundary.</param>
		private void SpawnBoundary(GlobalStates.SnapAngleEnums angle)
		{
			Boundary boundary = new Boundary(angle);
			boundary.Spawn(ExplorerUtils.Get()._Plugin);

			boundaries.Add(boundary);
		}

		/// <summary>
		/// Removes the boundary at the specified angle.
		/// </summary>
		/// <param name="angle">The angle of the boundary to remove.</param>
		private void RemoveBoundary(GlobalStates.SnapAngleEnums angle)
		{
			Boundary removed = null;

			foreach (Boundary boundary in boundaries)
			{
				if (boundary.Angle == angle)
				{
					boundary.ExitTree();
					removed = boundary;
				}
			}

			if (null != removed)
			{
				boundaries.Remove(removed);
			}
		}

		/// <summary>
		/// Removes all snap boundaries.
		/// </summary>
		private void RemoveBoundaries()
		{
			foreach (Boundary boundary in boundaries)
			{
				boundary.ExitTree();
			}

			boundaries = new();
			StatesUtils.Get().BoundarySpawned = GlobalStates.SpawnStateEnum.Null;
			StatesUtils.Get().BoundaryActiveAngles = new();
		}

		/// <summary>
		/// Checks whether the opacity of the snap boundaries should be updated.
		/// </summary>
		/// <returns><c>true</c> if the opacity should be updated; otherwise, <c>false</c>.</returns>
		private bool ShouldUpdateOpacity()
		{
			float BoundaryOpacity = SettingsUtils.Get().GetKey("boundary_box_opacity").As<float>();

			return
				BoundaryOpacity != CurrentOpacity;
		}

		/// <summary>
		/// Updates the opacity of the snap boundaries.
		/// </summary>
		/// <param name="value">The new opacity value.</param>
		private void UpdateOpacity(float value)
		{
			if (boundaries.Count == 0)
			{
				return;
			}

			foreach (Boundary boundary in boundaries)
			{
				boundary.UpdateOpacity(value);
			}

			CurrentOpacity = value;
		}

		private void UpdateTransform()
		{
			if (boundaries.Count == 0)
			{
				return;
			}

			foreach (Boundary boundary in boundaries)
			{
				boundary.UpdateTransform();
			}

			CurrentX = StatesUtils.Get().SnapToXValue;
			CurrentY = StatesUtils.Get().SnapToHeightValue;
			CurrentZ = StatesUtils.Get().SnapToZValue;
		}

		private bool ShouldUpdateTransform()
		{
			return
				StatesUtils.Get().SnapToXValue != CurrentX ||
				StatesUtils.Get().SnapToHeightValue != CurrentY ||
				StatesUtils.Get().SnapToZValue != CurrentZ;
		}

		/// <summary>
		/// Checks whether the snap boundaries are currently visible.
		/// </summary>
		/// <returns><c>true</c> if the boundaries are visible; otherwise, <c>false</c>.</returns>
		private bool IsBoundaryShown()
		{
			return
				StatesUtils.Get().BoundarySpawned == GlobalStates.SpawnStateEnum.Spawned;
		}

		/// <summary>
		/// Checks whether there is any active snap angle.
		/// </summary>
		/// <returns><c>true</c> if there is an active snap angle; otherwise, <c>false</c>.</returns>
		private bool HasActiveAngle()
		{
			return
				StatesUtils.Get().BoundaryActiveAngles.Count != 0;
		}

		/// <summary>
		/// Checks whether the specified snap angle is active.
		/// </summary>
		/// <param name="angle">The snap angle to check.</param>
		/// <returns><c>true</c> if the specified snap angle is active; otherwise, <c>false</c>.</returns>
		private bool IsAngleActive(GlobalStates.SnapAngleEnums angle)
		{
			return
				true == StatesUtils.Get().BoundaryActiveAngles.Contains(angle);
		}

		/// <summary>
		/// Checks whether snapping to height is enabled.
		/// </summary>
		/// <returns><c>true</c> if snapping to height is enabled; otherwise, <c>false</c>.</returns>
		private bool ShouldSnapToHeight()
		{
			return
				StatesUtils.Get().SnapToHeight == GlobalStates.LibraryStateEnum.Enabled &&
				false == StatesUtils.Get().EditingObjectIsPlaced;
		}

		/// <summary>
		/// Checks whether snapping to the X-axis is enabled.
		/// </summary>
		/// <returns><c>true</c> if snapping to the X-axis is enabled; otherwise, <c>false</c>.</returns>
		private bool ShouldSnapToX()
		{
			return
				StatesUtils.Get().SnapToX == GlobalStates.LibraryStateEnum.Enabled &&
				false == StatesUtils.Get().EditingObjectIsPlaced;
		}

		/// <summary>
		/// Checks whether snapping to the Z-axis is enabled.
		/// </summary>
		/// <returns><c>true</c> if snapping to the Z-axis is enabled; otherwise, <c>false</c>.</returns>
		private bool ShouldSnapToZ()
		{
			return
				StatesUtils.Get().SnapToZ == GlobalStates.LibraryStateEnum.Enabled &&
				false == StatesUtils.Get().EditingObjectIsPlaced;
		}

		/// <summary>
		/// Checks whether the snap boundaries should be shown.
		/// </summary>
		/// <returns><c>true</c> if the boundaries should be shown; otherwise, <c>false</c>.</returns>
		private bool ShouldShowBoundary()
		{
			return
				(
					StatesUtils.Get().SnapToHeight == GlobalStates.LibraryStateEnum.Enabled ||
					StatesUtils.Get().SnapToX == GlobalStates.LibraryStateEnum.Enabled ||
					StatesUtils.Get().SnapToZ == GlobalStates.LibraryStateEnum.Enabled
				)
				&&
					null != StatesUtils.Get().EditingObject;
		}

		/// <summary>
		/// Checks whether the snap boundaries should be hidden.
		/// </summary>
		/// <returns><c>true</c> if the boundaries should be hidden; otherwise, <c>false</c>.</returns>
		private bool ShouldHideBoundary()
		{
			return
				(
					StatesUtils.Get().SnapToHeight == GlobalStates.LibraryStateEnum.Disabled &&
					StatesUtils.Get().SnapToX == GlobalStates.LibraryStateEnum.Disabled &&
					StatesUtils.Get().SnapToZ == GlobalStates.LibraryStateEnum.Disabled ||
					null == StatesUtils.Get().EditingObject
				)
				&&
				StatesUtils.Get().BoundarySpawned == GlobalStates.SpawnStateEnum.Spawned;
		}
	}
}
#endif
