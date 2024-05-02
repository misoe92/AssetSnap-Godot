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
	using AssetSnap.Explorer;
	using AssetSnap.Front.Nodes;
	using AssetSnap.States;
	using AssetSnap.Static;
	using Godot;

	public class BaseInputDriver
	{
		/*
		** Holds a reference to the current instance
		*/
		private static BaseInputDriver _Instance;

		/*
		** Checks if Is Multi is currently enabled
		*/
		public bool IsMulti = false;

		/*
		** Fetches an instance of the input driver
		*/
		public static BaseInputDriver GetInstance()
		{
			if (null == _Instance)
			{
				_Instance = new();
			}

			return _Instance;
		}

		/*
		** Handles base input events
		** 
		** @param Camera3D Camera
		** @param InputEvent Event
		** @return int
		*/
		public virtual int _Input(Camera3D Camera, InputEvent Event)
		{
			if (false == ShouldListen())
			{
				return (int)EditorPlugin.AfterGuiInput.Pass;
			}

			_ListenForReset(Event);

			int _result = _ListenForMouseButtons(Event);
			if (_result != 0)
			{
				return _result;
			}

			return (int)EditorPlugin.AfterGuiInput.Pass;
		}

		/*
		** Forces focus to a model
		**
		** @return void
		*/
		public void FocusAsset(Node3D _model)
		{
			GlobalExplorer.GetInstance()._ForceFocus = _model;
		}

		/*
		** Listens to for reset events
		**
		** @return void
		*/
		private void _ListenForReset(InputEvent Event)
		{
			if (Event is InputEventKey _KeyEvent)
			{
				if (IsKeyReset(_KeyEvent))
				{
					StatesUtils.Get().EditingTitle = "";
					StatesUtils.Get().EditingObject = null;
					StatesUtils.Get().Group = null;
					StatesUtils.Get().GroupedObject = null;
					StatesUtils.Get().SnapToHeight = GlobalStates.LibraryStateEnum.Disabled;
					StatesUtils.Get().SnapToHeightGlue = GlobalStates.LibraryStateEnum.Disabled;
					StatesUtils.Get().SnapToObject = GlobalStates.LibraryStateEnum.Disabled;
					StatesUtils.Get().SnapToX = GlobalStates.LibraryStateEnum.Disabled;
					StatesUtils.Get().SnapToXGlue = GlobalStates.LibraryStateEnum.Disabled;
					StatesUtils.Get().SnapToZ = GlobalStates.LibraryStateEnum.Disabled;
					StatesUtils.Get().SnapToZGlue = GlobalStates.LibraryStateEnum.Disabled;
					StatesUtils.Get().CurrentLibrary.ClearAllPanelState();
				}
			}
		}

		/*
		** Listens to mouse button events
		**
		** @return void
		*/
		private int _ListenForMouseButtons(InputEvent Event)
		{
			if (Event is InputEventMouseButton _MouseButtonEvent)
			{
				if (InputsStatic.HasMouseLeftPressed(_MouseButtonEvent))
				{
					StatesUtils.Get().MultiDrop = false;
					Node3D Node = ExplorerUtils.Get().GetHandle();
					if (
						SettingsStatic.CanMultiDrop() &&
						InputsStatic.ShiftInputPressed(_MouseButtonEvent) &&
						InputsStatic.AltInputPressed(_MouseButtonEvent)
					)
					{
						IsMulti = true;
					}
					else
					{
						IsMulti = false;
					}

					ExplorerUtils.Get().Waypoints.Spawn(
						Node,
						ExplorerUtils.Get().Decal.GetNode().Transform.Origin,
						Node.RotationDegrees,
						Node.Scale
					);

					if (SettingsStatic.CanMultiDrop() && InputsStatic.ShiftInputPressed(_MouseButtonEvent) && InputsStatic.AltInputPressed(_MouseButtonEvent))
					{
						StatesUtils.Get().MultiDrop = true;
						if (Node is AsMeshInstance3D meshInstance3D)
						{
							AssetSnap.Front.Nodes.AsMeshInstance3D Duplicate = new()
							{
								Name = Node.Name,
								Mesh = meshInstance3D.Mesh,
								Transform = new Transform3D(Basis.Identity, new Vector3(0, 0, 0)),
								Scale = Node.Scale,
								RotationDegrees = Node.RotationDegrees,
								Floating = true,
								SpawnSettings = meshInstance3D.SpawnSettings.Duplicate(true),
							};

							Duplicate.SetLibraryName(meshInstance3D.GetLibraryName());
							StatesUtils.Get().CurrentLibrary = Duplicate.GetLibrary();
							StatesUtils.Get().EditingTitle = Duplicate.Name;
							StatesUtils.Get().EditingObject = Duplicate;
							StatesUtils.Get().GroupedObject = null;
						}
						else if (Node is AsNode3D node3d)
						{
							AsNode3D newAsNode3D = node3d.Duplicate() as AsNode3D;

							StatesUtils.Get().CurrentLibrary = newAsNode3D.GetLibrary();
							StatesUtils.Get().EditingTitle = newAsNode3D.Name;
							StatesUtils.Get().EditingObject = newAsNode3D;
							StatesUtils.Get().GroupedObject = null;
						}
						else if (Node is AsGrouped3D grouped3D)
						{
							AsGrouped3D newGrouped3D = grouped3D.Duplicate() as AsGrouped3D;
							newGrouped3D.GroupPath = grouped3D.GroupPath;

							StatesUtils.Get().EditingTitle = grouped3D.Name;
							StatesUtils.Get().EditingObject = newGrouped3D;
							StatesUtils.Get().GroupedObject = newGrouped3D;
						}

						return (int)EditorPlugin.AfterGuiInput.Stop;
					}
					else
					{
						if (ExplorerUtils.Get().States.PlacingMode == GlobalStates.PlacingModeEnum.Model)
						{
							if (null != StatesUtils.Get().CurrentLibrary)
							{
								StatesUtils.Get().CurrentLibrary.ClearAllPanelState();
								StatesUtils.Get().CurrentLibrary = null;
							}

							StatesUtils.Get().Group = null;
							StatesUtils.Get().GroupedObject = null;
						}

						if (ExplorerUtils.Get().States.PlacingMode == GlobalStates.PlacingModeEnum.Group)
						{
							StatesUtils.Get().CurrentLibrary = null;
							StatesUtils.Get().PlacingMode = GlobalStates.PlacingModeEnum.Model;
							StatesUtils.Get().Group = null;
							StatesUtils.Get().GroupedObject = null;
						}
					}

					// Focus the selected node in the editor
					// EditorInterface.Singleton.GetSelection().Clear();
					if (SettingsStatic.ShouldFocusAsset() && false == ExplorerUtils.Get().InputDriver.IsMulti)
					{
						StatesUtils.Get().EditingObject = null;
						StatesUtils.Get().GroupedObject = null;
						if (EditorPlugin.IsInstanceValid(Node))
						{
							ExplorerUtils.Get().InputDriver.FocusAsset(Node);
						}
					}
					else if (false == SettingsStatic.ShouldFocusAsset())
					{
						StatesUtils.Get().CurrentLibrary._LibrarySettings._LSEditing.SetText("None");
						StatesUtils.Get().EditingObject = null;
						StatesUtils.Get().GroupedObject = null;
					}

					return (int)EditorPlugin.AfterGuiInput.Stop;
				}
			}
			return (int)EditorPlugin.AfterGuiInput.Pass;
		}

		/*
		** Checks if input should be listened to.
		*/
		private bool ShouldListen()
		{
			return
				null != StatesUtils.Get().CurrentLibrary ||
				StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group;
		}

		/*
		** Checks if current keycode is escape
		**
		** @param InputEventKey _KeyEvent
		** @return bool
		*/
		private bool IsKeyReset(InputEventKey _KeyEvent)
		{
			return _KeyEvent.Keycode == Key.Escape;
		}
	}
}