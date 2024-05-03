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

using AssetSnap.Explorer;
using AssetSnap.Front.Nodes;
using Godot;

namespace AssetSnap.Core
{
	/// <summary>
    /// Handles input events for the core functionality.
    /// </summary>
	public class CoreInput : Core
	{
		private EventMouse _MouseEvent = EventMouse.EventNone;

		/// <summary>
        /// Handles GUI input events.
        /// </summary>
        /// <param name="Camera">The 3D camera.</param>
        /// <param name="event">The input event.</param>
        /// <returns>An integer representing the action to take after handling the input.</returns>
		public int Handle(Camera3D Camera, InputEvent @event)
		{
			if (null == ExplorerUtils.Get())
			{
				return (int)EditorPlugin.AfterGuiInput.Pass;
			}

			// If no handle is currently set
			if (false == HasHandle())
			{
				if (ScrollAllowed() == GlobalExplorer.ScrollState.SCROLL_DISABLED)
				{
					ExplorerUtils.Get().AllowScroll = GlobalExplorer.ScrollState.SCROLL_ENABLED;
					return (int)EditorPlugin.AfterGuiInput.Stop;
				}
				else
				{
					return (int)EditorPlugin.AfterGuiInput.Pass;
				}
			}

			if (
				ExplorerUtils.Get().GetHandle() is AsMeshInstance3D meshInstance3D && meshInstance3D.IsPlaced() ||
				ExplorerUtils.Get().GetHandle() is AsGrouped3D grouped3D && grouped3D.IsPlaced() ||
				ExplorerUtils.Get().GetHandle() is AsNode3D node3D && node3D.IsPlaced()
			)
			{
				return (int)EditorPlugin.AfterGuiInput.Pass;
			}

			// Maybe project from camera, depending on the current event
			MaybeProjectFromCamera(Camera, @event);

			// Run the input driver, depending on wheather it's drag or normal
			ExplorerUtils.Get().InputDriver._Input(Camera, @event);

			if (ScrollAllowed() == GlobalExplorer.ScrollState.SCROLL_DISABLED)
			{
				ExplorerUtils.Get().AllowScroll = GlobalExplorer.ScrollState.SCROLL_ENABLED;
				return (int)EditorPlugin.AfterGuiInput.Stop;
			}
			else
			{
				return (int)EditorPlugin.AfterGuiInput.Pass;
			}
		}

		/// <summary>
        /// Projects origin and normals from the camera, given the event position.
        /// </summary>
        /// <param name="Camera">The 3D camera.</param>
        /// <param name="event">The input event.</param>
        /// <returns>void</returns>
		private void MaybeProjectFromCamera(Camera3D Camera, InputEvent @event)
		{
			if (@event is InputEventMouseMotion _MotionEvent)
			{
				if (ExplorerUtils.Get()._CurrentMouseInput != EventMouse.EventClick)
				{
					ExplorerUtils.Get().ProjectRayOrigin = Camera.ProjectRayOrigin(_MotionEvent.Position);
					ExplorerUtils.Get().ProjectRayNormal = Camera.ProjectRayNormal(_MotionEvent.Position);
					ExplorerUtils.Get()._CurrentMouseInput = EventMouse.EventMove;
				}
			}
		}

		/// <summary>
        /// Checks if dragging is currently enabled.
        /// </summary>
        /// <returns>True if dragging is enabled, otherwise false.</returns>
		public bool ShouldDrag()
		{
			bool value = ExplorerUtils.Get().Settings.GetKey("allow_drag_add").As<bool>();

			if (value is bool valueBool)
			{
				return valueBool;
			}

			return false;
		}

		/// <summary>
        /// Checks if scroll is disallowed.
        /// </summary>
        /// <returns>The current scroll state.</returns>
		public GlobalExplorer.ScrollState ScrollAllowed()
		{
			return ExplorerUtils.Get().AllowScroll;
		}

		/// <summary>
        /// Checks if we have a handle, and if it's valid.
        /// </summary>
        /// <returns>True if a handle exists and is valid, otherwise false.</returns>
		public bool HasHandle()
		{
			Node NodeHandle = ExplorerUtils.Get().GetHandle();
			return null != NodeHandle;
		}

		/// <summary>
        /// Sets the mouse event that is currently being used.
        /// </summary>
        /// <param name="value">The mouse event value.</param>
        /// <returns>void</returns>
		public void SetMouseEvent(EventMouse value)
		{
			_MouseEvent = value;
		}

		/// <summary>
        /// Fetches the current mouse event.
        /// </summary>
        /// <returns>The current mouse event.</returns>
		public EventMouse GetMouseEvent()
		{
			return _MouseEvent;
		}
	}
}

#endif