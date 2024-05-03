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

using Godot;

namespace AssetSnap.Static
{
	/// <summary>
    /// Utility class for handling snapping functionality.
    /// </summary>
	public static class SnapStatic
	{
		/// <summary>
        /// Checks if snapping is possible with a set of coordinates and on a given layer.
        /// </summary>
        /// <param name="Coordinates">The coordinates to check for snapping.</param>
        /// <param name="Layer">The layer to check for snapping.</param>
        /// <returns>True if snapping is possible, false otherwise.</returns>
		public static bool CanSnap( Vector3 Coordinates, int Layer ) 
		{
			return GlobalExplorer.GetInstance().Snappable.CanSnap(Coordinates, Layer);
		}
		
		/// <summary>
        /// Snaps a set of coordinates on a giving layer to each other.
        /// </summary>
        /// <param name="Coordinates">The coordinates to snap.</param>
        /// <param name="aabb">The axis-aligned bounding box to snap to.</param>
        /// <param name="Layer">The layer to snap to.</param>
        /// <returns>The snapped coordinates.</returns>
		public static Vector3 Snap( Vector3 Coordinates, Aabb aabb, int Layer ) 
		{
			return GlobalExplorer.GetInstance().Snappable.Snap(Coordinates, aabb, Layer);
		}
		
		/// <summary>
        /// Checks if snapping is enabled on the x-axis.
        /// </summary>
        /// <returns>True if snapping is enabled on the x-axis, false otherwise.</returns>
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

		/// <summary>
        /// Checks if snapping is enabled on the z-axis.
        /// </summary>
        /// <returns>True if snapping is enabled on the z-axis, false otherwise.</returns>
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
		
		/// <summary>
        /// Fetches the snap offset on the x-axis.
        /// </summary>
        /// <returns>The snap offset on the x-axis.</returns>
		public static float GetObjectSnapOffsetX()
		{			
			if( null == GlobalExplorer.GetInstance() ) 
			{
				return 0.0f;
			}

			return GlobalExplorer.GetInstance().States.SnapToObjectOffsetXValue;
		}
			
		/// <summary>
        /// Fetches the snap offset on the z-axis.
        /// </summary>
        /// <returns>The snap offset on the z-axis.</returns>
		public static float GetObjectSnapOffsetZ()
		{		
			if( null == GlobalExplorer.GetInstance() ) 
			{
				return 0.0f;
			}

			return GlobalExplorer.GetInstance().States.SnapToObjectOffsetZValue;
		}
		
		/// <summary>
        /// Checks if object snap offset on the x-axis is enabled.
        /// </summary>
        /// <returns>True if object snap offset on the x-axis is enabled, false otherwise.</returns>
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
		
		/// <summary>
        /// Checks if object snap offset on the z-axis is enabled.
        /// </summary>
        /// <returns>True if object snap offset on the z-axis is enabled, false otherwise.</returns>
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