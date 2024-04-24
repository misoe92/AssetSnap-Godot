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
	using AssetSnap.Front.Components;
	using AssetSnap.Front.Nodes;
	using AssetSnap.Instance.Input;
	using AssetSnap.Library;
	using AssetSnap.States;
	using Godot;

	public class CoreHandles : Core
	{
		/*
		** Handles communication with the scene tree,
		** Performs actions depending on the current node in focus
		**
		** @param GodotObject _object
		** @return bool
		*/
		public bool Handle(GodotObject _object)
		{
			if (null == ExplorerUtils.Get() || null == Plugin.Singleton || null == ExplorerUtils.Get().ContextMenu || null == _object)
			{
				return true;
			}

			// Check if context menu is shown
			// if( false == _GlobalExplorer.ContextMenu.IsHidden() ) 
			// {
			// 	// If so, hide
			// 	_GlobalExplorer.ContextMenu.Hide();
			// }

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
				if (null != _GlobalExplorer.CurrentLibrary)
				{
					_GlobalExplorer.CurrentLibrary._LibrarySettings._LSEditing.SetText("None");
				}

				if (null != _GlobalExplorer.Library && null != _GlobalExplorer.Library.Libraries)
				{
					// Goes through all libraries and resets it's data
					foreach (Instance _Library in _GlobalExplorer.Library.Libraries)
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

		/*
		** Handles the interaction with the model node
		**
		** @param AsNode3D Node
		** @return void
		*/
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

		/*
		** Handles the interaction with the model node
		**
		** @param AssetSnap.Front.Nodes.AsMeshInstance3D Node
		** @return void
		*/
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

		/*
		** Handles the interaction with the model node
		**
		** @param AssetSnap.Front.Nodes.AsMeshInstance3D Node
		** @return void
		*/
		private void _HandleGroup(AsGrouped3D _Node)
		{
			_GlobalExplorer.States.PlacingMode = GlobalStates.PlacingModeEnum.Group;

			if (_GlobalExplorer.ContextMenu.IsHidden())
			{
				_GlobalExplorer.ContextMenu.Show();
			}

			if (_Node.IsPlaced())
			{
				_GlobalExplorer.States.EditingObject = _Node;
				_GlobalExplorer.States.GroupedObject = _Node;
				_GlobalExplorer.States.Group = GD.Load<Resource>(_Node.GroupPath) as GroupResource;
				_GlobalExplorer.BottomDock.SetTabByIndex(1);

				_GlobalExplorer.GroupBuilder._Editor.GroupPath = _Node.GroupPath;
				// _GlobalExplorer.GroupBuilder._Editor	
				// Check if drag add is currently active
				if (_GlobalExplorer.InputDriver is DragAddInputDriver DraggableInputDriver)
				{
					DraggableInputDriver.CalculateObjectSize();
				}
			}
		}

		/*
		** Updates the library settings of the current node being
		** worked on.
		**
		** @param AssetSnap.Front.Nodes.AsMeshInstance3D _Node
		** @return void
		*/
		private void HandleNodeLibrarySettings(Node _Node)
		{
			if (_Node is AsNode3D node3d)
			{
				foreach ((string key, Variant value) in node3d.GetSettings())
				{
					if (_GlobalExplorer.States.Has(key))
					{
						_GlobalExplorer.States.Set(key, value);
					}
				}
			}

			if (_Node is AsMeshInstance3D meshInstance3D)
			{
				foreach ((string key, Variant value) in meshInstance3D.GetSettings())
				{
					if (_GlobalExplorer.States.Has(key))
					{
						_GlobalExplorer.States.Set(key, value);
					}
				}
			}

		}

		/*
		** Checks if the given object is an model
		**
		** @param GodotObject _object
		** @return bool
		*/
		private bool _ShouldHandleModel(GodotObject _object)
		{
			return EditorPlugin.IsInstanceValid(_object) && (_object is AsMeshInstance3D || _object is AsNode3D);
		}

		/*
		** Checks if the given object is an model
		**
		** @param GodotObject _object
		** @return bool
		*/
		private bool _ShouldHandleGroup(GodotObject _object)
		{
			return EditorPlugin.IsInstanceValid(_object) && _object is AsGrouped3D;
		}

		/*
		** Converts the given object to an model type
		**
		** @param GodotObject _object
		** @return AssetSnap.Front.Nodes.AsMeshInstance3D
		*/
		private AssetSnap.Front.Nodes.AsMeshInstance3D _ObjectToModel(GodotObject _object)
		{
			return _object as AssetSnap.Front.Nodes.AsMeshInstance3D;
		}

		private AsGrouped3D _ObjectToGrouped(GodotObject _object)
		{
			return _object as AsGrouped3D;
		}

		/*
		** Resets the current handle
		*/
		public void ResetHandle()
		{
			_GlobalExplorer.HandleNode = null;
			_GlobalExplorer.States.EditingObject = null;
			_GlobalExplorer.States.Group = null;
			_GlobalExplorer.States.GroupedObject = null;
		}
	}
}