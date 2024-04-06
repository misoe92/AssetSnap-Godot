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

namespace AssetSnap.Static
{
	public static class SnapStatic
	{
		/*
		** Checks if snapping is possible with a set of cordinates
		** and on a given layer.
		**
		** @return bool
		*/
		public static bool CanSnap( Vector3 Coordinates, int Layer ) 
		{
			return GlobalExplorer.GetInstance().Snappable.CanSnap(Coordinates, Layer);
		}
		
		/*
		** Snaps a set of cordinates on a giving layer
		** to each other
		**
		** @return Vector3
		*/
		public static Vector3 Snap( Vector3 Coordinates, Aabb aabb, int Layer ) 
		{
			return GlobalExplorer.GetInstance().Snappable.Snap(Coordinates, aabb, Layer);
		}
		
		/*
		** Checks if any snapping is enabled on the x
		** axis
		**
		** @return bool
		*/
		public static bool IsSnapX()
		{
			if( null == GlobalExplorer.GetInstance() ) 
			{
				return false;
			}

			if( GlobalStates.LibraryStateEnum.Disabled == GlobalExplorer.GetInstance().States.SnapToX ) 
			{
				return false;
			}
			
			return true;
		}

		/*
		** Checks if any snapping is enabled on the z
		** axis
		**
		** @return bool
		*/
		public static bool IsSnapZ()
		{
			if( null == GlobalExplorer.GetInstance() ) 
			{
				return false;
			}

			if( GlobalStates.LibraryStateEnum.Disabled == GlobalExplorer.GetInstance().States.SnapToZ ) 
			{
				return false;
			}

			return true;
		}
		
		/*
		** Fetches the snap offset on the x axis
		**
		** @return float
		*/
		public static float GetObjectSnapOffsetX()
		{			
			if( null == GlobalExplorer.GetInstance() ) 
			{
				return 0.0f;
			}

			return GlobalExplorer.GetInstance().States.SnapToObjectOffsetXValue;
		}
			
		/*
		** Fetches the snap offset on the z axis
		**
		** @return float
		*/
		public static float GetObjectSnapOffsetZ()
		{		
			if( null == GlobalExplorer.GetInstance() ) 
			{
				return 0.0f;
			}

			return GlobalExplorer.GetInstance().States.SnapToObjectOffsetZValue;
		}
		
		/*
		** Checks if object snap offset on the x axis
		** is enabled
		**
		** @return bool
		*/
		public static bool HasObjectSnapOffsetX()
		{			
			if( null == GlobalExplorer.GetInstance() ) 
			{
				return false;
			}

			if( 0.0f == GlobalExplorer.GetInstance().States.SnapToObjectOffsetXValue ) 
			{
				return false;
			}

			return true;
		}
		
		/*
		** Checks if object snap offset on the z axis
		** is enabled
		**
		** @return bool
		*/
		public static bool HasObjectSnapOffsetZ()
		{	
			if( null == GlobalExplorer.GetInstance() ) 
			{
				return false;
			}

			if( 0.0f == GlobalExplorer.GetInstance().States.SnapToObjectOffsetZValue ) 
			{
				return false;
			}

			return true;
		}
	}
}
#endif