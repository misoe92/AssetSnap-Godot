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
using AssetSnap.Instance.Input;
using AssetSnap.States;
using Godot;

namespace AssetSnap.Core
{
	/// <summary>
	/// Handles communication with the scene tree and performs actions depending on the current node in focus.
	/// </summary>
	public class CoreHandles : Core
	{
		/// <summary>
		/// Handles communication with the scene tree and performs actions depending on the current node in focus.
		/// </summary>
		/// <param name="_object">The object in focus.</param>
		/// <returns>True if handling was successful, otherwise false.</returns>
		public bool Handle(GodotObject _object)
		{
			if (
				null == ExplorerUtils.Get() ||
				null == Plugin.Singleton ||
				null == ExplorerUtils.Get().ContextMenu ||
				null == _object
			)
			{
				return true;
			}
			
			if (_ShouldHandleModel(_object))
			{
				if (_object is AsNode3D node3d)
				{
					_HandleNode(node3d);
				}
				else
				{
					AssetSnap.Front.Nodes.AsMeshInstance3D AsMeshInstance = _ObjectToModel(_object);
					// Handle the model we are working with
					_HandleModel(AsMeshInstance);
				}
			}
			else if (_ShouldHandleGroup(_object))
			{
				AsGrouped3D AsGrouped = _ObjectToGrouped(_object);
				// Handle the grouped node we are working with
				_HandleGroup(AsGrouped);
			}
			else
			{
				if (null != StatesUtils.Get().CurrentLibrary)
				{
					StatesUtils.Get().CurrentLibrary._LibrarySettings._LSEditing.SetText("None");
				}

				if (null != ExplorerUtils.Get().Library && null != ExplorerUtils.Get().Library.Libraries)
				{
					// Goes through all libraries and resets it's data
					foreach (Library.Instance _Library in ExplorerUtils.Get().Library.Libraries)
					{
						if (EditorPlugin.IsInstanceValid(_Library) && null != _Library._LibrarySettings)
						{
							_Library._LibrarySettings.ClearAll();
							_Library._LibrarySettings._LSEditing.SetText("None");
						}
					}
				}

				// Clear the node that is being handled
				ResetHandle();
			}

			return true;
		}

		/// <summary>
		/// Handles the interaction with the model node.
		/// </summary>
		/// <param name="_Node">The 3D node.</param>
		private void _HandleNode(AsNode3D _Node)
		{
			StatesUtils.Get().PlacingMode = GlobalStates.PlacingModeEnum.Model;

			if (ExplorerUtils.Get().ContextMenu.IsHidden())
			{
				ExplorerUtils.Get().ContextMenu.Show();
			}

			if (_Node.IsPlaced())
			{
				if (null != ExplorerUtils.Get().Library)
				{
					Library.Base LibraryBase = ExplorerUtils.Get().Library;
					foreach (Library.Instance instance in LibraryBase.Libraries)
					{
						instance.ClearAllPanelState();
						instance._LibrarySettings.ClearAll();
						instance._LibrarySettings._LSEditing.SetText("None");
					}

					ExplorerUtils.Get().Model = null;
				}
				StatesUtils.Get().EditingObject = _Node;

				if (false == _Node.HasLibraryName())
				{
					return;
				}

				Library.Instance Library = ExplorerUtils.Get().GetLibraryByName(_Node.GetLibraryName());

				if (null == Library)
				{
					GD.Print("Library was not found: ", _Node.GetLibraryName());
				}

				ExplorerUtils.Get().BottomDock.SetTab(Library);
				Library._LibrarySettings._LSEditing.SetText(_Node.Name);

				/** Update library settings **/
				if (_Node.HasLibrarySettings())
				{
					HandleNodeLibrarySettings(_Node);
				}

				// Check if drag add is currently active
				if (ExplorerUtils.Get().InputDriver is DragAddInputDriver DraggableInputDriver)
				{
					DraggableInputDriver.CalculateObjectSize();
				}
			}
		}

		/// <summary>
		/// Handles the interaction with the model node.
		/// </summary>
		/// <param name="_Node">The mesh instance node.</param>
		private void _HandleModel(AssetSnap.Front.Nodes.AsMeshInstance3D _Node)
		{
			StatesUtils.Get().PlacingMode = GlobalStates.PlacingModeEnum.Model;

			if (ExplorerUtils.Get().ContextMenu.IsHidden())
			{
				ExplorerUtils.Get().ContextMenu.Show();
			}

			if (_Node.IsPlaced())
			{
				if (null != ExplorerUtils.Get().Library)
				{
					Library.Base LibraryBase = ExplorerUtils.Get().Library;
					foreach (Library.Instance instance in LibraryBase.Libraries)
					{
						instance.ClearAllPanelState();
						instance._LibrarySettings.ClearAll();
						instance._LibrarySettings._LSEditing.SetText("None");
					}

					ExplorerUtils.Get().Model = null;
				}
				StatesUtils.Get().EditingObject = _Node;

				if (false == _Node.HasLibraryName())
				{
					return;
				}

				Library.Instance Library = ExplorerUtils.Get().GetLibraryByName(_Node.GetLibraryName());

				if (null == Library)
				{
					GD.Print("Library was not found: ", _Node.GetLibraryName());
				}

				ExplorerUtils.Get().BottomDock.SetTab(Library);
				Library._LibrarySettings._LSEditing.SetText(_Node.Name);

				/** Update library settings **/
				if (_Node.HasLibrarySettings())
				{
					HandleNodeLibrarySettings(_Node);
				}

				// Check if drag add is currently active
				if (ExplorerUtils.Get().InputDriver is DragAddInputDriver DraggableInputDriver)
				{
					DraggableInputDriver.CalculateObjectSize();
				}
			}
			else
			{
				GD.PushWarning("Node is not placed");
			}
		}

		/// <summary>
		/// Handles the interaction with the grouped node.
		/// </summary>
		/// <param name="_Node">The grouped 3D node.</param>
		private void _HandleGroup(AsGrouped3D _Node)
		{
			StatesUtils.Get().PlacingMode = GlobalStates.PlacingModeEnum.Group;

			if (ExplorerUtils.Get().ContextMenu.IsHidden())
			{
				ExplorerUtils.Get().ContextMenu.Show();
			}

			if (_Node.IsPlaced())
			{
				StatesUtils.Get().EditingObject = _Node;
				StatesUtils.Get().GroupedObject = _Node;
				StatesUtils.Get().Group = GD.Load<Resource>(_Node.GroupPath) as GroupResource;
				ExplorerUtils.Get().BottomDock.SetTabByIndex(1);

				ExplorerUtils.Get().GroupBuilder._Editor.GroupPath = _Node.GroupPath;
				ExplorerUtils.Get().GroupBuilder._Editor.GroupOptions._UpdateGroupOptions();
				ExplorerUtils.Get().GroupBuilder._Sidebar.DoHide();
				// _GlobalExplorer.GroupBuilder._Editor	
				// Check if drag add is currently active
				if (_GlobalExplorer.InputDriver is DragAddInputDriver DraggableInputDriver)
				{
					DraggableInputDriver.CalculateObjectSize();
				}
			}
		}

		/// <summary>
		/// Updates the library settings of the current node being worked on.
		/// </summary>
		/// <param name="_Node">The node whose library settings need to be updated.</param>
		private void HandleNodeLibrarySettings(Node _Node)
		{
			if (_Node is AsNode3D node3d)
			{
				foreach ((string key, Variant value) in node3d.GetSettings())
				{
					if (StatesUtils.Get().Has(key))
					{
						StatesUtils.Get().Set(key, value);
					}
				}
			}

			if (_Node is AsMeshInstance3D meshInstance3D)
			{
				foreach ((string key, Variant value) in meshInstance3D.GetSettings())
				{
					if (StatesUtils.Get().Has(key))
					{
						StatesUtils.Get().Set(key, value);
					}
				}
			}
		}

		/// <summary>
		/// Checks if the given object is a model.
		/// </summary>
		/// <param name="_object">The object to check.</param>
		/// <returns>True if the object is a model, otherwise false.</returns>
		private bool _ShouldHandleModel(GodotObject _object)
		{
			return EditorPlugin.IsInstanceValid(_object) && (_object is AsMeshInstance3D || _object is AsNode3D);
		}

		/// <summary>
		/// Checks if the given object is a grouped node.
		/// </summary>
		/// <param name="_object">The object to check.</param>
		/// <returns>True if the object is a grouped node, otherwise false.</returns>
		private bool _ShouldHandleGroup(GodotObject _object)
		{
			return EditorPlugin.IsInstanceValid(_object) && _object is AsGrouped3D;
		}

		/// <summary>
		/// Converts the given object to a model type.
		/// </summary>
		/// <param name="_object">The object to convert.</param>
		/// <returns>The converted model instance.</returns>
		private AssetSnap.Front.Nodes.AsMeshInstance3D _ObjectToModel(GodotObject _object)
		{
			return _object as AssetSnap.Front.Nodes.AsMeshInstance3D;
		}

		/// <summary>
        /// Converts the given object to a grouped node.
        /// </summary>
        /// <param name="_object">The object to convert.</param>
        /// <returns>The converted grouped instance.</returns>
		private AsGrouped3D _ObjectToGrouped(GodotObject _object)
		{
			return _object as AsGrouped3D;
		}

		/// <summary>
        /// Resets the current handle.
        /// </summary>
		public void ResetHandle()
		{
			StatesUtils.Get().EditingTitle = null;
			StatesUtils.Get().EditingObject = null;
			StatesUtils.Get().Group = null;
			StatesUtils.Get().GroupedObject = null;
		}
	}
}

#endif