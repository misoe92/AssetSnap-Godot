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
namespace AssetSnap
{
	using AssetSnap.Front.Nodes;
	using Godot;

	[Tool]
	public partial class NodeExplorer : CameraExplorer
	{
		protected AssetSnap.Front.Nodes.AsMeshInstance3D _Model;
		protected Node _HandleNode;
		protected Library.Instance _CurrentLibrary;
		protected Callable? UpdateHandleCallable;
		
		/*
		** Defines whether or not a forced focus is active
		*/
		public int _Forced = 0;
		
		/*
		** The current node being handled
		*/
		public Node HandleNode
		{
			get => _HandleNode;
			set {
				_HandleNode = value;
			}
		}
		

		/*
		** The node to force focus to
		*/
		public Node3D _ForceFocus;
		
		/*
		** Whether or not a library is active
		*/
		public bool HasLibrary
		{
			get => CurrentLibrary != null;
		}
		
		/*
		** The active library
		*/
		public Library.Instance CurrentLibrary 
		{
			get => _CurrentLibrary;
			set
			{
				_CurrentLibrary = value;
			}
		}
		
		/*
		** Current selected model from the asset library
		*/
		public AssetSnap.Front.Nodes.AsMeshInstance3D Model 
		{
			get => _Model;
			set
			{
				_Model = value;
			}
		}
		
		/*
		** Whether or not a model is active
		*/
		public bool HasModel
		{
			get => Model != null;
		}
				
		/*
		** Whether or not current model is placed.
		*/
		public bool IsModelPlaced
		{
			get => Model.IsPlaced();
		}

		public NodeExplorer()
		{
			// UpdateHandleCallable = new(this, "UpdateHandle");
			
			// if(false == IsUpdateHandleConnected()) 
			// {
			// 	EditorInterface.Singleton.GetInspector().Connect(EditorInspector.SignalName.EditedObjectChanged, UpdateCallable());
			// }
		}
		
		/*
		** Used to check whether or not HandleNode is in focus,
		** and if not perform an action
		**
		** @return void
		*/
		protected void UpdateHandle()
		{
			if( EditorInterface.Singleton.GetInspector().GetEditedObject() == null && Model == null) 
			{
				HandleNode = null;
				
				if( false == EditorPlugin.IsInstanceValid( ContextMenu ))
				{
					return;	
				}
				
				ContextMenu.Hide();
			}
		}
		
		/*
		** Clears the current handle
		**
		** @return void
		*/
		public void ClearHandle()
		{
			Model = null;
			HandleNode = null;
		}
		
		public Callable UpdateCallable()
		{
			return (Callable)UpdateHandleCallable;
		}
		
		/*
		** Checks if the update handle method is connected
		**
		** @return void
		*/
		protected bool IsUpdateHandleConnected() 
		{
			if (UpdateHandleCallable is Callable callable)
			{
				EditorInspector Inspector = EditorInterface.Singleton.GetInspector();
				return Inspector.IsConnected(EditorInspector.SignalName.EditedObjectChanged, callable);
			}
			
			return false;
		}
		
		/*
		** Fetches the current handle
		**
		** @return Node3D
		*/
		public Node3D GetHandle()
		{
			Node3D Handle = HandleNode as Node3D;
			if (Handle == null)
			{
				Handle = Model;
			}
			
			if (Handle == null)
			{
				return Handle;
			}

			return Handle;
		}
		
		/*
		** Checks if the current handle is
		** that of a Model
		**
		** @return bool
		*/
		public bool HandleIsModel()
		{
			Node3D Handle = Model;

			if (Handle == null)
			{
				Handle = HandleNode as Node3D;
			}
			
			if (Handle == null)
			{
				return false;
			}

			return Handle is AssetSnap.Front.Nodes.AsMeshInstance3D;
		}
		
	}
}
#endif