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

	public class Base
	{
		private GlobalExplorer _GlobalExplorer;
		
		public List<Boundary> boundaries = new List<Boundary>();
		
		private float CurrentOpacity;
		private float CurrentX = 0;
		private float CurrentY = 0;
		private float CurrentZ = 0;
		
		public void Initialize()
		{
			_GlobalExplorer = GlobalExplorer.GetInstance();
			CurrentOpacity = _GlobalExplorer.Settings.GetKey("boundary_box_opacity").As<float>();

			CurrentX = _GlobalExplorer.States.SnapToXValue;
			CurrentY = _GlobalExplorer.States.SnapToHeightValue;
			CurrentZ = _GlobalExplorer.States.SnapToZValue;
		}
		
		public void Tick( double delta )
		{
			if( null == _GlobalExplorer ) 
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
					_GlobalExplorer.States.BoundaryActiveAngles.Add(GlobalStates.SnapAngleEnums.Y);
				}
				else if( false == ShouldSnapToHeight() && IsAngleActive( GlobalStates.SnapAngleEnums.Y ) ) 
				{
					RemoveBoundary(GlobalStates.SnapAngleEnums.Y);
					_GlobalExplorer.States.BoundaryActiveAngles.Remove(GlobalStates.SnapAngleEnums.Y);
				}
				
				if( ShouldSnapToX()  && false == IsAngleActive( GlobalStates.SnapAngleEnums.X )) 
				{
					SpawnBoundary( GlobalStates.SnapAngleEnums.X );
					_GlobalExplorer.States.BoundaryActiveAngles.Add(GlobalStates.SnapAngleEnums.X);
				}
				else if( false == ShouldSnapToX() && IsAngleActive( GlobalStates.SnapAngleEnums.X ) ) 
				{
					RemoveBoundary(GlobalStates.SnapAngleEnums.X);
					_GlobalExplorer.States.BoundaryActiveAngles.Remove(GlobalStates.SnapAngleEnums.X);
				}
				
				if( ShouldSnapToZ()  && false == IsAngleActive( GlobalStates.SnapAngleEnums.Z ))
				{
					SpawnBoundary( GlobalStates.SnapAngleEnums.Z );
					_GlobalExplorer.States.BoundaryActiveAngles.Add(GlobalStates.SnapAngleEnums.Z);
				}
				else if( false == ShouldSnapToZ() && IsAngleActive( GlobalStates.SnapAngleEnums.Z ) ) 
				{
					RemoveBoundary(GlobalStates.SnapAngleEnums.Z);
					_GlobalExplorer.States.BoundaryActiveAngles.Remove(GlobalStates.SnapAngleEnums.Z);
				}
				
				if( HasActiveAngle() ) 
				{
					_GlobalExplorer.States.BoundarySpawned = GlobalStates.SpawnStateEnum.Spawned;
				}
				else 
				{
					_GlobalExplorer.States.BoundarySpawned = GlobalStates.SpawnStateEnum.Null;
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
			boundary.Spawn(_GlobalExplorer._Plugin);
			
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
			_GlobalExplorer.States.BoundarySpawned = GlobalStates.SpawnStateEnum.Null;
			_GlobalExplorer.States.BoundaryActiveAngles = new();
		}
		
		private bool ShouldUpdateOpacity()
		{
			float BoundaryOpacity = _GlobalExplorer.Settings.GetKey("boundary_box_opacity").As<float>();

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
			
			float BoundaryOpacity = _GlobalExplorer.Settings.GetKey("boundary_box_opacity").As<float>();
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
			
			CurrentX = _GlobalExplorer.States.SnapToXValue;
			CurrentY = _GlobalExplorer.States.SnapToHeightValue;
			CurrentZ = _GlobalExplorer.States.SnapToZValue;
		}
		
		private bool ShouldUpdateTransform()
		{
			return
				_GlobalExplorer.States.SnapToXValue != CurrentX ||
				_GlobalExplorer.States.SnapToHeightValue != CurrentY ||
				_GlobalExplorer.States.SnapToZValue != CurrentZ;
		}
		
		private bool IsBoundaryShown()
		{
			return
				_GlobalExplorer.States.BoundarySpawned == GlobalStates.SpawnStateEnum.Spawned;
		}
		
		private bool HasActiveAngle()
		{
			return
				_GlobalExplorer.States.BoundaryActiveAngles.Count != 0;
		}
		
		private bool IsAngleActive( GlobalStates.SnapAngleEnums angle )
		{
			return
				true == _GlobalExplorer.States.BoundaryActiveAngles.Contains(angle);
		}
		
		private bool ShouldSnapToHeight()
		{
			return 
				_GlobalExplorer.States.SnapToHeight == GlobalStates.LibraryStateEnum.Enabled;
		}
		
		private bool ShouldSnapToX()
		{
			return 
				_GlobalExplorer.States.SnapToX == GlobalStates.LibraryStateEnum.Enabled;
		}
		
		private bool ShouldSnapToZ()
		{
			return 
				_GlobalExplorer.States.SnapToZ == GlobalStates.LibraryStateEnum.Enabled;
		}
		
		private bool ShouldShowBoundary()
		{
			return
				_GlobalExplorer.States.SnapToHeight == GlobalStates.LibraryStateEnum.Enabled ||
				_GlobalExplorer.States.SnapToX == GlobalStates.LibraryStateEnum.Enabled ||
				_GlobalExplorer.States.SnapToZ == GlobalStates.LibraryStateEnum.Enabled;
		}
	}
}
#endif
