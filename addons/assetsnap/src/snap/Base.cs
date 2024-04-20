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

	public class Base
	{
		private static Base _Instance;
		public static Base Singleton 
		{
			get
			{
				if( null == _Instance ) 
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
			if( null == ExplorerUtils.Get() ) 
			{
				return;
			}
			
			CurrentOpacity = ExplorerUtils.Get().Settings.GetKey("boundary_box_opacity").As<float>();
			CurrentX = ExplorerUtils.Get().States.SnapToXValue;
			CurrentY = ExplorerUtils.Get().States.SnapToHeightValue;
			CurrentZ = ExplorerUtils.Get().States.SnapToZValue;
		}
		
		public void Tick( double delta )
		{
			if( null == ExplorerUtils.Get() ) 
			{
				return;
			} 
			
			if( ShouldUpdateOpacity() ) 
			{
				UpdateOpacity();				
			}
			
			if( ShouldUpdateTransform() ) 
			{
				UpdateTransform();
			}
			
			if( ShouldShowBoundary() ) 
			{
				if( ShouldSnapToHeight() && false == IsAngleActive( GlobalStates.SnapAngleEnums.Y )) 
				{
					SpawnBoundary( GlobalStates.SnapAngleEnums.Y );
					ExplorerUtils.Get().States.BoundaryActiveAngles.Add(GlobalStates.SnapAngleEnums.Y);
				}
				else if( false == ShouldSnapToHeight() && IsAngleActive( GlobalStates.SnapAngleEnums.Y ) ) 
				{
					RemoveBoundary(GlobalStates.SnapAngleEnums.Y);
					ExplorerUtils.Get().States.BoundaryActiveAngles.Remove(GlobalStates.SnapAngleEnums.Y);
				}
				
				if( ShouldSnapToX()  && false == IsAngleActive( GlobalStates.SnapAngleEnums.X )) 
				{
					SpawnBoundary( GlobalStates.SnapAngleEnums.X );
					ExplorerUtils.Get().States.BoundaryActiveAngles.Add(GlobalStates.SnapAngleEnums.X);
				}
				else if( false == ShouldSnapToX() && IsAngleActive( GlobalStates.SnapAngleEnums.X ) ) 
				{
					RemoveBoundary(GlobalStates.SnapAngleEnums.X);
					ExplorerUtils.Get().States.BoundaryActiveAngles.Remove(GlobalStates.SnapAngleEnums.X);
				}
				
				if( ShouldSnapToZ()  && false == IsAngleActive( GlobalStates.SnapAngleEnums.Z ))
				{
					SpawnBoundary( GlobalStates.SnapAngleEnums.Z );
					ExplorerUtils.Get().States.BoundaryActiveAngles.Add(GlobalStates.SnapAngleEnums.Z);
				}
				else if( false == ShouldSnapToZ() && IsAngleActive( GlobalStates.SnapAngleEnums.Z ) ) 
				{
					RemoveBoundary(GlobalStates.SnapAngleEnums.Z);
					ExplorerUtils.Get().States.BoundaryActiveAngles.Remove(GlobalStates.SnapAngleEnums.Z);
				}
				
				if( HasActiveAngle() ) 
				{
					ExplorerUtils.Get().States.BoundarySpawned = GlobalStates.SpawnStateEnum.Spawned;
				}
				else 
				{
					ExplorerUtils.Get().States.BoundarySpawned = GlobalStates.SpawnStateEnum.Null;
				}
			}
			else if( IsBoundaryShown() ) 
			{
				RemoveBoundaries();
			}
		}
		
		private void SpawnBoundary( GlobalStates.SnapAngleEnums angle ) 
		{
			Boundary boundary = new Boundary(angle);
			boundary.Spawn(ExplorerUtils.Get()._Plugin);
			
			boundaries.Add( boundary );
		}
		
		private void RemoveBoundary( GlobalStates.SnapAngleEnums angle ) 
		{
			Boundary removed = null;
			
			foreach( Boundary boundary in boundaries ) 
			{
				if( boundary.Angle == angle ) 
				{
					boundary.ExitTree();
					removed = boundary;
				}
			}
			
			if( null == removed ) 
			{
				boundaries.Remove(removed);
			}
		}
		
		private void RemoveBoundaries()
		{
			foreach( Boundary boundary in boundaries ) 
			{
				boundary.ExitTree();
			}

			boundaries = new();
			ExplorerUtils.Get().States.BoundarySpawned = GlobalStates.SpawnStateEnum.Null;
			ExplorerUtils.Get().States.BoundaryActiveAngles = new();
		}
		
		private bool ShouldUpdateOpacity()
		{
			float BoundaryOpacity = ExplorerUtils.Get().Settings.GetKey("boundary_box_opacity").As<float>();

			return
				BoundaryOpacity != CurrentOpacity;
		}
		
		/*
		** Updates the opacity of the boundary
		** 
		** @param float value
		** @return void
		*/
		private void UpdateOpacity()
		{
			if( boundaries.Count == 0 ) 
			{
				return;
			}
			
			float BoundaryOpacity = ExplorerUtils.Get().Settings.GetKey("boundary_box_opacity").As<float>();
			foreach( Boundary boundary in boundaries ) 
			{
				boundary.UpdateOpacity(BoundaryOpacity);
			}
			
			CurrentOpacity = BoundaryOpacity;
		}
		
		private void UpdateTransform()
		{
			if( boundaries.Count == 0 ) 
			{
				return;
			}
			
			foreach( Boundary boundary in boundaries ) 
			{
				boundary.UpdateTransform();
			}
			
			CurrentX = ExplorerUtils.Get().States.SnapToXValue;
			CurrentY = ExplorerUtils.Get().States.SnapToHeightValue;
			CurrentZ = ExplorerUtils.Get().States.SnapToZValue;
		}
		
		private bool ShouldUpdateTransform()
		{
			return
				ExplorerUtils.Get().States.SnapToXValue != CurrentX ||
				ExplorerUtils.Get().States.SnapToHeightValue != CurrentY ||
				ExplorerUtils.Get().States.SnapToZValue != CurrentZ;
		}
		
		private bool IsBoundaryShown()
		{
			return
				ExplorerUtils.Get().States.BoundarySpawned == GlobalStates.SpawnStateEnum.Spawned;
		}
		
		private bool HasActiveAngle()
		{
			return
				ExplorerUtils.Get().States.BoundaryActiveAngles.Count != 0;
		}
		
		private bool IsAngleActive( GlobalStates.SnapAngleEnums angle )
		{
			return
				true == ExplorerUtils.Get().States.BoundaryActiveAngles.Contains(angle);
		}
		
		private bool ShouldSnapToHeight()
		{
			return 
				ExplorerUtils.Get().States.SnapToHeight == GlobalStates.LibraryStateEnum.Enabled;
		}
		
		private bool ShouldSnapToX()
		{
			return 
				ExplorerUtils.Get().States.SnapToX == GlobalStates.LibraryStateEnum.Enabled;
		}
		
		private bool ShouldSnapToZ()
		{
			return 
				ExplorerUtils.Get().States.SnapToZ == GlobalStates.LibraryStateEnum.Enabled;
		}
		
		private bool ShouldShowBoundary()
		{
			return
				ExplorerUtils.Get().States.SnapToHeight == GlobalStates.LibraryStateEnum.Enabled ||
				ExplorerUtils.Get().States.SnapToX == GlobalStates.LibraryStateEnum.Enabled ||
				ExplorerUtils.Get().States.SnapToZ == GlobalStates.LibraryStateEnum.Enabled;
		}
	}
}
#endif
