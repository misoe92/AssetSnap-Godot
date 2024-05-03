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
using AssetSnap.Nodes;
using AssetSnap.States;
using AssetSnap.Static;
using Godot;

namespace AssetSnap.Instance.Input
{
	/// <summary>
	/// Handles input events for drag-and-drop functionality.
	/// </summary>
	public class DragAddInputDriver : BaseInputDriver
	{
		/** Private **/
		private bool Dragging = false;
		private Vector3 DragFrom;
		private Vector3 DragTo;
		private float Distance = 0.0f;
		// private float Size = 0.0f;
		private float SizeX = 0.0f;
		private float SizeY = 0.0f;
		private float SizeZ = 0.0f;
		private int NumPoints = 0;
		private Godot.Collections.Array<Node3D> _Buffer;

		/// <summary>
		/// Offset for the size of the dragged objects.
		/// </summary>
		public float SizeOffset = 0;

		private static DragAddInputDriver _Instance;

		/// <summary>
		/// Gets the singleton instance of the DragAddInputDriver class.
		/// </summary>
		/// <returns>The instance of DragAddInputDriver.</returns>
		public new static DragAddInputDriver GetInstance()
		{
			if (null == _Instance)
			{
				_Instance = new();
			}

			return _Instance;
		}

		/// <summary>
		/// Constructs a new instance of DragAddInputDriver.
		/// </summary>
		public DragAddInputDriver() : base()
		{
			_Buffer = new();
		}

		/// <summary>
		/// Handles input events regarding the drag-and-drop functionality.
		/// </summary>
		/// <param name="Camera">The Camera3D used for rendering.</param>
		/// <param name="Event">The input event.</param>
		/// <returns>An integer indicating the input event handling result.</returns>
		public override int _Input(Camera3D Camera, InputEvent Event)
		{
			GlobalExplorer explorer = GlobalExplorer.GetInstance();
			/** Check if Ctrl is pressed **/
			if (
				false == Dragging &&
				Event is InputEventMouseButton _MouseButtonInitialEvent
			)
			{
				if (SettingsStatic.CanMultiDrop() && InputsStatic.ShiftInputPressed(_MouseButtonInitialEvent) && InputsStatic.AltInputPressed(_MouseButtonInitialEvent))
				{
					StatesUtils.Get().MultiDrop = true;
				}
				
				if (
					_MouseButtonInitialEvent.ButtonIndex == MouseButton.Left &&
					false == _MouseButtonInitialEvent.Pressed &&
					Godot.Input.IsKeyPressed(Key.Ctrl) &&
					explorer.HasPositionDrawn()
				)
				{
					Dragging = true;
					DragFrom = explorer.GetPositionDrawn();
					// explorer.Decal.Hide();
					return (int)EditorPlugin.AfterGuiInput.Stop;
				}
			}

			if (
				Dragging &&
				Event is InputEventKey _InputEventKey
			)
			{
				if (_InputEventKey.Keycode == Key.Escape)
				{
					/** Reset buffer **/
					ResetBuffer();
					/** Reset DragTo and DragFrom **/
					ResetDragPath();

					Dragging = false;
				}
			}

			if (
				Dragging &&
				Event is InputEventMouseMotion &&
				explorer.HasPositionDrawn()
			)
			{
				ResetBuffer();

				DragTo = explorer.GetPositionDrawn();
				Distance = DragFrom.DistanceTo(DragTo);

				/** Calculate how many objects can be spawned **/
				CalculateSpawnAmount();

				/** Calculate path between DragFrom and DragTo **/
				Vector3[] path = CalculateDragPath();

				/** Spawn the calculated amount **/
				SpawnCalculatedAmount(path);
			}

			if (
				Dragging &&
				Event is InputEventMouseButton _MouseButtonEvent &&
				explorer.HasPositionDrawn()
			)
			{
				ResetBuffer();

				DragTo = explorer.GetPositionDrawn();
				Distance = DragFrom.DistanceTo(DragTo);

				/** Calculate how many objects can be spawned **/
				CalculateSpawnAmount();

				/** Calculate path between DragFrom and DragTo **/
				Vector3[] path = CalculateDragPath();

				/** Spawn the calculated amount **/
				SpawnCalculatedAmount(path);

				/** Apply Dragging **/
				if (
					_MouseButtonEvent.ButtonIndex == MouseButton.Left &&
					false == _MouseButtonEvent.Pressed
				)
				{
					/** Place the whole buffer of objects **/
					SpawnBuffer();
					/** Reset buffer **/
					ResetBuffer();
					/** Reset DragTo and DragFrom **/
					ResetDragPath();

					Dragging = false;

					return (int)EditorPlugin.AfterGuiInput.Stop;
				}

				/** Cancel Dragging **/
				if (
					_MouseButtonEvent.ButtonIndex == MouseButton.Right &&
					false == _MouseButtonEvent.Pressed
				)
				{
					/** Reset buffer **/
					ResetBuffer();
					/** Reset DragTo and DragFrom **/
					ResetDragPath();

					Dragging = false;

					return (int)EditorPlugin.AfterGuiInput.Stop;
				}
			}

			if (
				Dragging &&
				Godot.Input.IsKeyPressed(Key.Ctrl) &&
				Event is InputEventMouseButton inputEventMouseButton &&
				(
					inputEventMouseButton.ButtonIndex == MouseButton.WheelUp ||
					inputEventMouseButton.ButtonIndex == MouseButton.WheelDown
				)
			)
			{
				if (false == inputEventMouseButton.Pressed)
				{
					float scrollAmount = Mathf.Abs(inputEventMouseButton.Factor); // Get the absolute scroll amount

					if (inputEventMouseButton.ButtonIndex == MouseButton.WheelUp)
					{
						SizeOffset += 0.1f * scrollAmount;
					}
					else if (inputEventMouseButton.ButtonIndex == MouseButton.WheelDown)
					{
						SizeOffset -= 0.1f * scrollAmount;
					}

					GlobalExplorer.GetInstance().States.DragSizeOffset = SizeOffset;
				}

				GlobalExplorer.GetInstance().AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;
				return (int)EditorPlugin.AfterGuiInput.Stop;
			}

			if (!Dragging)
			{
				/** Proceed with standard input **/
				return base._Input(Camera, Event);
			}

			return (int)EditorPlugin.AfterGuiInput.Stop;
		}

		/// <summary>
		/// Calculates the size of the current object being dragged.
		/// </summary>
		/// <returns>The size of the object as a Vector3.</returns>
		public Vector3 CalculateObjectSize()
		{
			Node3D handle = GlobalExplorer.GetInstance().GetHandle();

			// Get the AABB of the model
			Aabb aabb = NodeUtils.CalculateNodeAabb(handle);

			SizeX = aabb.Size.X;
			SizeY = aabb.Size.Y;
			SizeZ = aabb.Size.Z;

			return aabb.Size;
		}

		/// <summary>
        /// Checks if the drag-and-drop functionality is currently active.
        /// </summary>
        /// <returns>True if dragging, false otherwise.</returns>
		public bool IsDragging()
		{
			return Dragging;
		}

		/// <summary>
		/// Spawns the calculated amount of models.
		/// </summary>
		/// <param name="paths">An array of Vector3 representing the calculated path.</param>
		/// <returns>void</returns>
		private void SpawnCalculatedAmount(Vector3[] paths)
		{
			Node3D Handle = GlobalExplorer.GetInstance().GetHandle();

			/** Spawn Element **/
			/** Add spawned element to buffer for easy unloading **/
			for (int i = 0; i < paths.Length; i++)
			{
				Vector3 Path = paths[i];
				Node3D Duplicate = Handle.Duplicate() as Node3D;
				GlobalExplorer.GetInstance()._Plugin.AddChild(Duplicate);

				Transform3D Trans = Duplicate.Transform;
				Trans.Origin = Path;
				Duplicate.Transform = Trans;

				_Buffer.Add(Duplicate);
			}
		}

		/// <summary>
		/// Spawns the already created buffer of models.
		/// </summary>
		/// <returns>void</returns>
		private void SpawnBuffer()
		{
			int ite = 0;
			foreach (Node3D Instance in _Buffer)
			{
				Node3D Duplicate = Instance.Duplicate() as Node3D;
				GlobalExplorer.GetInstance().Waypoints.Spawn(Duplicate, Instance.Transform.Origin, Instance.RotationDegrees, Instance.Scale);
				ite += 1;
			}
		}

		/// <summary>
		/// Resets the buffer of models.
		/// </summary>
		/// <returns>void</returns>
		private void ResetBuffer()
		{
			foreach (Node3D Instance in _Buffer)
			{
				if (EditorPlugin.IsInstanceValid(Instance.GetParent()))
				{
					Instance.GetParent().RemoveChild(Instance);
				}

				Instance.QueueFree();
			}

			_Buffer = new();
		}

		/// <summary>
		/// Resets the generated path to be used to place models on.
		/// </summary>
		/// <returns>void</returns>
		private void ResetDragPath()
		{
			DragFrom = Vector3.Zero;
			DragTo = Vector3.Zero;
		}

		/// <summary>
		/// Calculates a path of vector3's the models can spawn on.
		/// </summary>
		/// <returns>An array of Vector3 representing the calculated path.</returns>
		private Vector3[] CalculateDragPath()
		{
			List<Vector3> VectorList = new();

			if (NumPoints == 0)
			{
				return new Vector3[0];
			}

			for (int i = 0; i <= NumPoints; i++)
			{
				float t = i / (float)NumPoints;
				Vector3 point = DragFrom.Lerp(DragTo, t);
				VectorList.Add(point);
			}

			return VectorList.ToArray();
		}

		/// <summary>
		/// Defines which side the current drag action is facing.
		/// </summary>
		/// <returns>An integer representing the direction of the drag action.</returns>
		private int Facing()
		{
			if (DragFrom.X < DragTo.X || DragFrom.X > DragTo.X)
			{
				return 1;
			}

			if (DragFrom.Y < DragTo.Y || DragFrom.Y > DragTo.Y)
			{
				return 2;
			}

			if (DragFrom.Z < DragTo.Z || DragFrom.Z > DragTo.Z)
			{
				return 3;
			}


			return 0;
		}

		/// <summary>
		/// Calculates the amount of models that can be spawned in the available space.
		/// </summary>
		/// <returns>An integer representing the number of models that can be spawned.</returns>
		private int CalculateSpawnAmount()
		{
			float Size = 0.0f;

			if (Facing() == 1)
			{
				Size = SizeX - SizeOffset;
			}

			if (Facing() == 2)
			{
				Size = SizeY - SizeOffset;
			}

			if (Facing() == 3)
			{
				Size = SizeZ - SizeOffset;
			}

			NumPoints = Mathf.CeilToInt(Distance / Size);
			return NumPoints;
		}
	}
}

#endif