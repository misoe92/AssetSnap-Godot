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

namespace AssetSnap.Front.Components.Library
{
	using Godot;
	using AssetSnap.Component;
	using AssetSnap.Front.Nodes;
	using AssetSnap.Explorer;
	using AssetSnap.States;

	[Tool]
	public partial class SnapGrab : LibraryComponent
	{
		public SnapGrab()
		{
			Name = "LibrarySnapGrab";

			UsingTraits = new() { };

			//_include = false;  
		}

		/*
		** Checks if rotation is currently active
		** and whether or not to apply it
		**
		** @return void
		*/
		public async override void _Input(InputEvent @event)
		{
			if (false == _ShouldGrab())
			{
				return;
			}

			if (
				null == ExplorerUtils.Get() ||
				false == EditorPlugin.IsInstanceValid(ExplorerUtils.Get().GetHandle())
			)
			{
				return;
			}

			if (@event is InputEventKey keyEvent && keyEvent.Keycode == Key.A && Input.IsKeyPressed(Key.Shift) && Input.IsKeyPressed(Key.Alt) && keyEvent.IsPressed() == false)
			{
				// Grab the currently chosen node.
				Node _Node = _GlobalExplorer.GetHandle();
				Node Parent = null;
				AssetSnap.Library.Instance CurrentLibrary = null;

				if (_Node is not AsMeshInstance3D && _Node is not AsGrouped3D && _Node is not AsNode3D )
				{
					return;
				}

				if (_Node is AsMeshInstance3D _MeshInstance3D)
				{
					CurrentLibrary = ExplorerUtils.Get().GetLibraryByName(_MeshInstance3D.GetLibraryName());
					if (null == CurrentLibrary)
					{
						GD.PushWarning("No library");
						return;
					}

					Parent = _MeshInstance3D.GetParent();
					if (null != Parent)
					{
						Parent.RemoveChild(_MeshInstance3D);
					}
					else
					{
						GD.PushWarning("No Parent");
						return;
					}

					await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
					StatesUtils.Get().EditingObjectIsPlaced = false;
					ExplorerUtils.Get().SetFocusToNode(_MeshInstance3D);
				}

				if (_Node is AsNode3D _Node3D)
				{
					AsNode3D newAsNode = _Node3D.Duplicate() as AsNode3D;
					Parent = _Node3D.GetParent();
					if (null != Parent)
					{
						Parent.RemoveChild(_Node3D);
					}
					else
					{
						GD.PushWarning("No Parent");
						return;
					}

					await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
					StatesUtils.Get().EditingObjectIsPlaced = false;
					ExplorerUtils.Get().SetFocusToNode(newAsNode);
				}

				if (_Node is AsGrouped3D _Grouped3D)
				{
					AsGrouped3D newGroup3D = _Grouped3D.Duplicate() as AsGrouped3D;
					Parent = _Grouped3D.GetParent();
					if (null != Parent)
					{
						Parent.RemoveChild(_Grouped3D);
					}
					else
					{
						GD.PushWarning("No Parent");
						return;
					}

					await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
					StatesUtils.Get().EditingObjectIsPlaced = false;
					ExplorerUtils.Get().SetFocusToNode(newGroup3D);
				}
			}
		}

		/*
		** Returns the current state of
		** object grabbing
		**
		** @return bool
		*/
		private bool _ShouldGrab()
		{
			if (null == ExplorerUtils.Get() || null == ExplorerUtils.Get().Settings)
			{
				return false;
			}

			bool ModelGrab = ExplorerUtils.Get().Settings.GetKey("allow_model_grab").As<bool>();
			return ModelGrab;
		}
	}
}