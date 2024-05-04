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
using AssetSnap.States;
using AssetSnap.Static;
using Godot;

namespace AssetSnap.Instance.Input
{
	/// <summary>
	/// Base class for input drivers.
	/// </summary>
	public class BaseInputDriver
	{
		/// <summary>
		/// Fetches an instance of the input driver.
		/// </summary>
		/// <returns>The input driver instance.</returns>
		public static BaseInputDriver GetInstance()
		{
			if (null == _Instance)
			{
				_Instance = new();
			}

			return _Instance;
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether Multi mode is currently enabled.
		/// </summary>
		public bool IsMulti = false;
		
		/// <summary>
		/// Holds a reference to the current instance.
		/// </summary>
		private static BaseInputDriver _Instance;

		/// <summary>
		/// Handles base input events.
		/// </summary>
		/// <param name="Camera">The 3D camera.</param>
		/// <param name="Event">The input event.</param>
		/// <returns>An integer representing the handling result.</returns>
		public virtual int _Input(Camera3D Camera, InputEvent Event)
		{
			if (false == _ShouldListen())
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

		/// <summary>
		/// Forces focus to a model.
		/// </summary>
		/// <param name="_model">The model to focus on.</param>
		public void FocusAsset(Node3D _model)
		{
			GlobalExplorer.GetInstance()._ForceFocus = _model;
		}

		/// <summary>
		/// Listens to for reset events.
		/// </summary>
		/// <param name="Event">The input event.</param>
		private void _ListenForReset(InputEvent Event)
		{
			if (Event is InputEventKey _KeyEvent)
			{
				if (_IsKeyReset(_KeyEvent))
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

		/// <summary>
		/// Listens to mouse button events.
		/// </summary>
		/// <param name="Event">The input event.</param>
		/// <returns>An integer representing the handling result.</returns>
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

		/// <summary>
		/// Checks if input should be listened to.
		/// </summary>
		/// <returns><c>true</c> if input should be listened to; otherwise, <c>false</c>.</returns>
		private bool _ShouldListen()
		{
			return
				null != StatesUtils.Get().CurrentLibrary ||
				StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group;
		}

		/// <summary>
		/// Checks if the current keycode is escape.
		/// </summary>
		/// <param name="_KeyEvent">The key event to check.</param>
		/// <returns><c>true</c> if the keycode is escape; otherwise, <c>false</c>.</returns>
		private bool _IsKeyReset(InputEventKey _KeyEvent)
		{
			return _KeyEvent.Keycode == Key.Escape;
		}
	}
}

#endif