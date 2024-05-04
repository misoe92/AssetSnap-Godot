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

using System;
using AssetSnap.Waypoint;

namespace AssetSnap.Static
{
	/// <summary>
	/// Static class for handling waypoint-related operations.
	/// </summary>
	public static class WaypointsStatic
	{
		/// <summary>
		/// Performs an action on each waypoint.
		/// </summary>
		/// <param name="action">The action to perform on each waypoint.</param>
		public static void Each( Action<BaseWaypoint> action )
		{
			GlobalExplorer.GetInstance().Waypoints.WaypointList.Each(
				(BaseWaypoint Point) =>
				{
					Action<BaseWaypoint> callable = action;
					callable.Invoke(Point);
				}
			);
		}
		
		/// <summary>
		/// Checks if any waypoints exist.
		/// </summary>
		/// <returns>True if any waypoints exist, false otherwise.</returns>
		public static bool HasAnyWaypoints()
		{
			return GlobalExplorer.GetInstance().Waypoints.HasAnyWaypoints();
		}
	}
}