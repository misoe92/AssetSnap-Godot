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
namespace AssetSnap.Decal
{
	using AssetSnap.Explorer;
	using AssetSnap.Front.Nodes;
	using AssetSnap.States;
	using Godot;

	public partial class Base
	{
		private AsDecal3D _Decal;
		private Mesh _ModelMesh;
		private MeshInstance3D _ModelMeshInstance;
		private AsGroup3D _Group;
		private GlobalExplorer _GlobalExplorer;

		public AsDecal3D Decal
		{
			get => _Decal;
			set
			{
				_Decal = value;
			}
		}

		public Mesh ModelMesh
		{
			get => _ModelMesh;
			set
			{
				_ModelMesh = value;
			}
		}

		public MeshInstance3D ModelMeshInstance
		{
			get => _ModelMeshInstance;
			set
			{
				_ModelMeshInstance = value;
			}
		}

		public AsGroup3D Group
		{
			get => _Group;
			set
			{
				_Group = value;
			}
		}

		private static Base _Instance;

		public static Base Singleton
		{
			get
			{
				if (null == _Instance)
				{
					_Instance = new();
				}

				return _Instance;
			}
		}

		/*
		** Initializes the decal handler
		**
		** @return void 
		*/
		public void Initialize()
		{
			ClearChildren();

			Attach_GlobalExplorer();

			if (false == Is_GlobalExplorerValid())
			{
				return;
			}

			InitializeNode();
			SetInitialPosition();
			Hide();
		}
		
		/*
		** Exits from the tree
		**
		** @return void
		*/
		public void Exit()
		{
			Hide();
		}

		public Vector3 GetPosition()
		{
			
			return GetNode().Transform.Origin;
		}

		/*
		** Updates the preview model shown in the decal
		**
		** @param bool State
		** @return void
		*/
		public void _UpdateDecalPreview(bool State)
		{
			if (false == EditorPlugin.IsInstanceValid(ExplorerUtils.Get().GetHandle()) || false == EditorPlugin.IsInstanceValid(GetNode()))
			{
				return;
			}

			if (StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Group)
			{
				AsGrouped3D group = StatesUtils.Get().GroupedObject;
				Node child = null;
				bool ChildFound = false;

				if (GetNode().GetChildCount() > 0)
				{
					child = GetNode().GetChild(0);
					ChildFound = true;
					ClearCurrentChildren(child, group);
				}

				PlaceDecalPreview(State, ChildFound, child, group);
			}

			if (StatesUtils.Get().PlacingMode == GlobalStates.PlacingModeEnum.Model)
			{
				Node model = ExplorerUtils.Get().GetHandle();
				Node Model = null;
				bool ChildFound = false;

				if (GetNode().GetChildCount() > 0)
				{
					Model = GetNode().GetChild(0);
					ChildFound = true;

					ClearCurrentChildren(Model, model);
				}

				PlaceDecalPreview(State, ChildFound, Model, model);
			}

			if (IsHidden())
			{
				Show();
			}
		}

		/*
		** Shows the decal
		**
		** @return void
		*/
		public void Show()
		{
			/** Only show if we have an actual model to show **/
			if (null == GetNode())
			{
				GD.PushError("Decal not instantiated");
				return;
			}

			GetNode().Visible = true;
			StatesUtils.Get().DecalVisible = GlobalStates.VisibilityStateEnum.Visible;
		}

		/*
		** Hides the decal
		**
		** @return void
		*/
		public void Hide()
		{
			if (GetNode() == null || false == GetNode().Visible)
			{
				return;
			}
			
			GetNode().Visible = false;
			StatesUtils.Get().DecalVisible = GlobalStates.VisibilityStateEnum.Hidden;
		}

		/*
		** Fetches the Decal's node
		**
		** @return Node3D
		*/
		public Node3D GetNode()
		{
			if( false == Plugin.Singleton.GetInternalContainer().HasNode("AsDecal") ) 
			{
				InitializeNode();
				SetInitialPosition();
			}
			
			return Plugin.Singleton.GetInternalContainer().GetNode("AsDecal") as Node3D;
		}

		/*
		** Fetches the Decal's mesh instance
		**
		** @return MeshInstance3D
		*/
		public MeshInstance3D GetMeshInstance()
		{
			return ModelMeshInstance;
		}

		/*
		** Sets transform of the decal
		**
		** @param Transform3D GlobalTrans
		** @return void
		*/
		public void SetTransform(Transform3D GlobalTrans)
		{
			GetNode().GlobalTransform = GlobalTrans;
		}

		/*
		** Checks if the decal is hidden
		**
		** @return bool
		*/
		public bool IsHidden()
		{
			if( false == EditorPlugin.IsInstanceValid(GetNode()))
			{
				return true;
			}
			
			return GetNode().Visible == false;
		}

		/*
		** Clears the current children of the decal as
		** long as it does not match the new child.
		**
		** @param AssetSnap.Front.Nodes.AsMeshInstance3D Model
		** @param AssetSnap.Front.Nodes.AsMeshInstance3D model
		** @return void
		*/
		private void ClearCurrentChildren(Node Model, Node model)
		{
			if (EditorPlugin.IsInstanceValid(Model) && Model != model)
			{
				GetNode().RemoveChild(Model);
				Model.QueueFree();
			}
		}

		private void ClearChildren()
		{
			for (int i = 0; i < GlobalExplorer.GetInstance()._Plugin.GetChildCount(); i++)
			{
				Node child = GlobalExplorer.GetInstance()._Plugin.GetChild(i);
				if (child is AsDecal3D)
				{
					child.QueueFree();
				}
			}
		}

		/*
		** Places the decal preview inside of the decal,
		** so it's shown in the editor
		**
		** @param bool State
		** @param bool ChildFound
		** @param AssetSnap.Front.Nodes.AsMeshInstance3D Model
		** @param AssetSnap.Front.Nodes.AsMeshInstance3D model
		** @return void
		*/
		private void PlaceDecalPreview(bool State, bool ChildFound, Node Model, Node model)
		{
			if (true == State && false == ChildFound)
			{
				EditorInterface.Singleton.EditNode(model);
				GetNode().AddChild(model);
				if (model is AsMeshInstance3D meshInstance3D)
				{
					ModelMeshInstance = meshInstance3D;
					ModelMesh = ModelMeshInstance.Mesh;
				}

				if (model is AsGrouped3D group3D)
				{
					Group = group3D;
				}
			}
		}

		/*
		** Attaches the message bus to the class
		**
		** @return void
		*/
		private void Attach_GlobalExplorer()
		{
			_GlobalExplorer = GlobalExplorer.GetInstance();
		}

		/*
		** Initializes the decal node
		**
		** @return void
		*/
		private void InitializeNode()
		{
			AsDecal3D _Decal = new();
			Plugin.Singleton.GetInternalContainer().AddChild(_Decal);
			StatesUtils.Get().DecalSpawned = GlobalStates.SpawnStateEnum.Spawned;
		}

		/*
		** Set an initial position for our
		** decal node
		**
		** @return void
		*/
		private void SetInitialPosition()
		{
			Transform3D Trans = GetNode().Transform;
			Trans.Origin = new Vector3(0, 0, 0);
			GetNode().Transform = Trans;
		}

		/*
		** Checks if message bus is valid
		**
		** @return bool
		*/
		private bool Is_GlobalExplorerValid()
		{
			return null != _GlobalExplorer && null != _GlobalExplorer.Settings;
		}
	}
}
#endif
