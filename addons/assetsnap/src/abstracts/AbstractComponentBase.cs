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

using AssetSnap.Explorer;
using Godot;

namespace AssetSnap.Abstracts
{
	/// <summary>
	/// Base class for component-based functionality.
	/// </summary>
	[Tool]
	public abstract partial class AbstractComponentBase : VBoxContainer
	{
		/// <summary>
        /// Gets the global explorer instance.
        /// </summary>
		protected GlobalExplorer _GlobalExplorer { get => ExplorerUtils.Get(); }
		// protected Plugin _Plugin { get => _GlobalExplorer._Plugin; }
		// protected Component.Base Components { get => _GlobalExplorer.Components; }
		// protected Waypoint.Base Waypoints { get => _GlobalExplorer.Waypoints; }
		// protected ContextMenu.Base ContextMenu { get => _GlobalExplorer.ContextMenu; } 
		// protected Library.Base Libraries { get => _GlobalExplorer.Library; }
		// protected Modifier.Base Modifiers { get => _GlobalExplorer.Modifiers; }
		// protected BottomDock.Base BottomDock { get => _GlobalExplorer.BottomDock; }
		// protected Raycast.Base Raycast { get => _GlobalExplorer.Raycast; } 
		// protected AssetSnap.Decal.Base Decal { get => _GlobalExplorer.Decal; }
		// protected BaseInputDriver InputDriver { get => _GlobalExplorer.InputDriver; }
		// protected SettingsConfig Settings { get => _GlobalExplorer.Settings; }

		/// <summary>
        /// Fetches the handle from the global explorer.
        /// </summary>
        /// <returns>The handle node.</returns>
		public Node3D GetHandle() 
		{
			return _GlobalExplorer.GetHandle();
		}
	} 
}