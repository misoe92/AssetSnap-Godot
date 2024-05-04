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
using Godot;

namespace AssetSnap
{
	/// <summary>
	/// Partial class representing a node explorer for navigating scenes and objects.
	/// </summary>
	[Tool]
	public partial class NodeExplorer : CameraExplorer
	{
		/// <summary>
		/// Defines whether or not a forced focus is active.
		/// </summary>
		public int _Forced = 0;
		
		/// <summary>
		/// The current node being handled.
		/// </summary>
		public Node HandleNode
		{
			get => _HandleNode;
			set {
				_HandleNode = value;
			}
		}
		
		/// <summary>
		/// The node to force focus to.
		/// </summary>
		public Node3D _ForceFocus;
		
		protected AsMeshInstance3D _Model;
		protected Node _HandleNode;
		protected GroupResource _Group;
		protected AsGrouped3D _GroupedObject;
		protected Library.Instance _CurrentLibrary;
		protected Callable? _UpdateHandleCallable;
		
		/// <summary>
		/// Whether or not a model is active.
		/// </summary>
		public bool HasModel
		{
			get => StatesUtils.Get().EditingObject != null;
		}
				
		/// <summary>
		/// Whether or not current model is placed.
		/// </summary>
		public bool IsModelPlaced
		{
			get
			{
				if( StatesUtils.Get().EditingObject is AsMeshInstance3D meshInstance3D ) 
				{
					return meshInstance3D.IsPlaced();				
				}

				return false;
			}
		}
		
		/// <summary>
		/// Default constructor for the NodeExplorer class.
		/// </summary>	
		public NodeExplorer()
		{
			// UpdateHandleCallable = new(this, "UpdateHandle");
			
			// if(false == IsUpdateHandleConnected()) 
			// {
			// 	EditorInterface.Singleton.GetInspector().Connect(EditorInspector.SignalName.EditedObjectChanged, UpdateCallable());
			// }
		}
		
		/// <summary>
		/// Used to check whether or not HandleNode is in focus,
		/// and if not perform an action.
		/// </summary>
		protected void UpdateHandle()
		{
			if( EditorInterface.Singleton.GetInspector().GetEditedObject() == null && StatesUtils.Get().EditingObject == null) 
			{
				HandleNode = null;
				
				if( null == ExplorerUtils.Get().ContextMenu ) 
				{
					return;	
				}
				
				ContextMenu.Hide();
			}
		}
		
		/// <summary>
		/// Clears the current handle.
		/// </summary>
		public void ClearHandle()
		{
			HandleNode = null;
		}
		
		/// <summary>
		/// Returns the update callable.
		/// </summary>
		/// <returns>The update callable.</returns>
		public Callable UpdateCallable()
		{
			return (Callable)_UpdateHandleCallable;
		}
		
		/// <summary>
		/// Checks if the update handle method is connected.
		/// </summary>
		/// <returns>True if the update handle method is connected, false otherwise.</returns>
		protected bool IsUpdateHandleConnected() 
		{
			if (_UpdateHandleCallable is Callable callable)
			{
				EditorInspector Inspector = EditorInterface.Singleton.GetInspector();
				return Inspector.IsConnected(EditorInspector.SignalName.EditedObjectChanged, callable);
			}
			
			return false;
		}
		
		/// <summary>
		/// Fetches the current handle.
		/// </summary>
		/// <returns>The current handle.</returns>
		public Node3D GetHandle()
		{
			if( States.PlacingMode == GlobalStates.PlacingModeEnum.Model ) 
			{
				Node3D Handle = States.EditingObject;
				return Handle;
			}
			
			// Else group mode
			return States.GroupedObject;
		}
		
		/// <summary>
		/// Checks if the current handle is that of a Model.
		/// </summary>
		/// <returns>True if the current handle is a Model, false otherwise.</returns>
		public bool HandleIsModel()
		{
			Node3D Handle = StatesUtils.Get().EditingObject;

			if (Handle == null)
			{
				return false;
			}

			return Handle is AssetSnap.Front.Nodes.AsMeshInstance3D;
		}
	}
}
#endif