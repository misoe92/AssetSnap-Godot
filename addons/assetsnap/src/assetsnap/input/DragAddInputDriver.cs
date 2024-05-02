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

namespace AssetSnap.Instance.Input
{
	using System.Collections.Generic;
	using AssetSnap.Front.Nodes;
	using AssetSnap.Nodes;
	using AssetSnap.States;
	using AssetSnap.Static;
	using Godot;

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

		public float SizeOffset = 0;

		private static DragAddInputDriver _Instance;

		public new static DragAddInputDriver GetInstance()
		{
			if (null == _Instance)
			{
				_Instance = new();
			}

			return _Instance;
		}

		/*
		** Construction of the class
		*/
		public DragAddInputDriver() : base()
		{
			_Buffer = new();
		}

		/*
		** Handles input events regarding the drag
		** add functionality
		** 
		** @param Camera3D Camera
		** @param InputEvent Event
		** @return int
		*/
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
					Input.IsKeyPressed(Key.Ctrl) &&
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
				Input.IsKeyPressed(Key.Ctrl) &&
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

		/*
		** Calculates the current object size
		** 
		** @return Vector3
		*/
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

		public bool IsDragging()
		{
			return Dragging;
		}

		/*
		** Spawns the calculated amount of models
		** 
		** @return void
		*/
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

		/*
		** Spawns the already created buffer of models
		** 
		** @return void
		*/
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

		/*
		** Resets the buffer of models
		** 
		** @return void
		*/
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

		/*
		** Resets the generated path, that
		** was to be used to place models on
		** 
		** @return void
		*/
		private void ResetDragPath()
		{
			DragFrom = Vector3.Zero;
			DragTo = Vector3.Zero;
		}

		/*
		** Calculates a path of vector3's the models can spawn on
		** 
		** @return Vector3[]
		*/
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

		/*
		** Defines which side the current drag
		** action is facing
		** 
		** @return int
		*/
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

		/*
		** Calculates amount of models that can be
		** spawned in the available space
		** 
		** @return int
		*/
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