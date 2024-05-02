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

namespace AssetSnap.Core
{
    using AssetSnap.Explorer;
    using AssetSnap.Front.Nodes;
	using Godot;
	public class CoreInput : Core
	{
		private EventMouse _MouseEvent = EventMouse.EventNone;

		/*
		** Handle GUI input events
		*/
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

		/*
		** Projects origin and normals from the camera, given the event
		** position.
		**
		** @param Camera3D Camera
		** @param InputEvent @event
		** @return void
		*/
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

		/*
		** Checks if dragging is currently enabled
		**
		** @return bool
		*/
		public bool ShouldDrag()
		{
			bool value = ExplorerUtils.Get().Settings.GetKey("allow_drag_add").As<bool>();

			if (value is bool valueBool)
			{
				return valueBool;
			}

			return false;
		}

		/*
		** Checks if scroll is disallowed
		**
		** @return bool
		*/
		public GlobalExplorer.ScrollState ScrollAllowed()
		{
			return ExplorerUtils.Get().AllowScroll;
		}

		/*
		** Checks if we have a handle, and if its valid
		**
		** @return bool
		*/
		public bool HasHandle()
		{
			Node NodeHandle = ExplorerUtils.Get().GetHandle();
			return null != NodeHandle;
		}

		/*
		** Sets the mouse event that are currently 
		** being used
		**
		** @param EventMouse value
		** @return void
		*/
		public void SetMouseEvent(EventMouse value)
		{
			_MouseEvent = value;
		}

		/*
		** Fetches the current mouse event
		**
		** @return EventMouse
		*/
		public EventMouse GetMouseEvent()
		{
			return _MouseEvent;
		}
	}
}