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
namespace AssetSnap.Snap
{
	using System.Collections.Generic;
	using AssetSnap.Explorer;
	using AssetSnap.Settings;

	using AssetSnap.States;

	public class Base
	{
		private static Base _Instance;
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

		private float CurrentOpacity;
		private float CurrentX = 0;
		private float CurrentY = 0;
		private float CurrentZ = 0;

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

		public void _MaybeUpdateOpacity(Godot.Collections.Array data)
		{
			if (data[0].As<string>() == "boundary_box_opacity")
			{
				SetBoxOpacity(data[1].As<float>());
			}
		}

		public void SetBoxOpacity(float value)
		{
			// BoundaryOpacity = value;
			UpdateOpacity(value);
		}

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

			if (ShouldShowBoundary())
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
			else if (ShouldHideBoundary())
			{
				RemoveBoundaries();
			}
		}

		private void SpawnBoundary(GlobalStates.SnapAngleEnums angle)
		{
			Boundary boundary = new Boundary(angle);
			boundary.Spawn(ExplorerUtils.Get()._Plugin);

			boundaries.Add(boundary);
		}

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

		private bool ShouldUpdateOpacity()
		{
			float BoundaryOpacity = SettingsUtils.Get().GetKey("boundary_box_opacity").As<float>();

			return
				BoundaryOpacity != CurrentOpacity;
		}

		/*
		** Updates the opacity of the boundary
		** 
		** @param float value
		** @return void
		*/
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

		private bool IsBoundaryShown()
		{
			return
				StatesUtils.Get().BoundarySpawned == GlobalStates.SpawnStateEnum.Spawned;
		}

		private bool HasActiveAngle()
		{
			return
				StatesUtils.Get().BoundaryActiveAngles.Count != 0;
		}

		private bool IsAngleActive(GlobalStates.SnapAngleEnums angle)
		{
			return
				true == StatesUtils.Get().BoundaryActiveAngles.Contains(angle);
		}

		private bool ShouldSnapToHeight()
		{
			return
				StatesUtils.Get().SnapToHeight == GlobalStates.LibraryStateEnum.Enabled &&
				false == StatesUtils.Get().EditingObjectIsPlaced;
		}

		private bool ShouldSnapToX()
		{
			return
				StatesUtils.Get().SnapToX == GlobalStates.LibraryStateEnum.Enabled &&
				false == StatesUtils.Get().EditingObjectIsPlaced;
		}

		private bool ShouldSnapToZ()
		{
			return
				StatesUtils.Get().SnapToZ == GlobalStates.LibraryStateEnum.Enabled &&
				false == StatesUtils.Get().EditingObjectIsPlaced;
		}

		private bool ShouldShowBoundary()
		{
			return
					StatesUtils.Get().SnapToHeight == GlobalStates.LibraryStateEnum.Enabled ||
					StatesUtils.Get().SnapToX == GlobalStates.LibraryStateEnum.Enabled ||
					StatesUtils.Get().SnapToZ == GlobalStates.LibraryStateEnum.Enabled;
		}

		private bool ShouldHideBoundary()
		{
			return
				StatesUtils.Get().SnapToHeight == GlobalStates.LibraryStateEnum.Disabled &&
				StatesUtils.Get().SnapToX == GlobalStates.LibraryStateEnum.Disabled &&
				StatesUtils.Get().SnapToZ == GlobalStates.LibraryStateEnum.Disabled &&
				StatesUtils.Get().BoundarySpawned == GlobalStates.SpawnStateEnum.Spawned;
		}
	}
}
#endif
