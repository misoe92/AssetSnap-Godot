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

namespace AssetSnap.Decal
{
	/// <summary>
	/// Partial class representing a base decal handler.
	/// </summary>
	public partial class Base
	{
		private AsDecal3D _Decal;
		private Mesh _ModelMesh;
		private MeshInstance3D _ModelMeshInstance;
		private AsGroup3D _Group;
		private GlobalExplorer _GlobalExplorer;

		/// <summary>
		/// Gets or sets the decal.
		/// </summary>
		public AsDecal3D Decal
		{
			get => _Decal;
			set
			{
				_Decal = value;
			}
		}

		/// <summary>
		/// Gets or sets the model mesh.
		/// </summary>
		public Mesh ModelMesh
		{
			get => _ModelMesh;
			set
			{
				_ModelMesh = value;
			}
		}

		/// <summary>
		/// Gets or sets the model mesh instance.
		/// </summary>
		public MeshInstance3D ModelMeshInstance
		{
			get => _ModelMeshInstance;
			set
			{
				_ModelMeshInstance = value;
			}
		}

		/// <summary>
		/// Gets or sets the group.
		/// </summary>
		public AsGroup3D Group
		{
			get => _Group;
			set
			{
				_Group = value;
			}
		}

		private static Base _Instance;

		/// <summary>
		/// Gets the singleton instance of Base.
		/// </summary>
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

		/// <summary>
		/// Initializes the decal handler.
		/// </summary>
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
		
		/// <summary>
		/// Exits from the tree.
		/// </summary>
		public void Exit()
		{
			Hide();
		}

		/// <summary>
		/// Gets the position of the decal.
		/// </summary>
		/// <returns>The position of the decal.</returns>
		public Vector3 GetPosition()
		{
			
			return GetNode().Transform.Origin;
		}

		/// <summary>
		/// Updates the preview model shown in the decal.
		/// </summary>
		/// <param name="State">The state of the preview model.</param>
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

		/// <summary>
		/// Shows the decal.
		/// </summary>
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

		/// <summary>
		/// Hides the decal.
		/// </summary>
		public void Hide()
		{
			if (GetNode() == null || false == GetNode().Visible)
			{
				return;
			}
			
			GetNode().Visible = false;
			StatesUtils.Get().DecalVisible = GlobalStates.VisibilityStateEnum.Hidden;
		}

		/// <summary>
		/// Fetches the Decal's node.
		/// </summary>
		/// <returns>The Decal's node.</returns>
		public Node3D GetNode()
		{
			if( false == Plugin.Singleton.GetInternalContainer().HasNode("AsDecal") ) 
			{
				InitializeNode();
				SetInitialPosition();
			}
			
			return Plugin.Singleton.GetInternalContainer().GetNode("AsDecal") as Node3D;
		}

		/// <summary>
		/// Fetches the Decal's mesh instance.
		/// </summary>
		/// <returns>The Decal's mesh instance.</returns>
		public MeshInstance3D GetMeshInstance()
		{
			return ModelMeshInstance;
		}

		/// <summary>
		/// Sets the transform of the decal.
		/// </summary>
		/// <param name="GlobalTrans">The global transform to set.</param>
		public void SetTransform(Transform3D GlobalTrans)
		{
			GetNode().GlobalTransform = GlobalTrans;
		}

		/// <summary>
		/// Checks if the decal is hidden.
		/// </summary>
		/// <returns>True if the decal is hidden, otherwise false.</returns>
		public bool IsHidden()
		{
			if( false == EditorPlugin.IsInstanceValid(GetNode()))
			{
				return true;
			}
			
			return GetNode().Visible == false;
		}

		/// <summary>
		/// Clears the current children of the decal.
		/// </summary>
		/// <param name="Model">The model to clear.</param>
		/// <param name="model">The new model.</param>
		private void ClearCurrentChildren(Node Model, Node model)
		{
			if (EditorPlugin.IsInstanceValid(Model) && Model != model)
			{
				GetNode().RemoveChild(Model);
				Model.QueueFree();
			}
		}

		/// <summary>
		/// Clears the current children of the decal as long as it does not match the new child.
		/// </summary>
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

		/// <summary>
		/// Places the decal preview inside of the decal so it's shown in the editor.
		/// </summary>
		/// <param name="State">The state of the preview model.</param>
		/// <param name="ChildFound">True if a child was found, false otherwise.</param>
		/// <param name="Model">The model to place.</param>
		/// <param name="model">The model node.</param>
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

		/// <summary>
		/// Attaches the message bus to the class.
		/// </summary>
		private void Attach_GlobalExplorer()
		{
			_GlobalExplorer = GlobalExplorer.GetInstance();
		}

		/// <summary>
		/// Initializes the decal node.
		/// </summary>
		private void InitializeNode()
		{
			AsDecal3D _Decal = new();
			Plugin.Singleton.GetInternalContainer().AddChild(_Decal);
			StatesUtils.Get().DecalSpawned = GlobalStates.SpawnStateEnum.Spawned;
		}

		/// <summary>
		/// Sets an initial position for the decal node.
		/// </summary>
		private void SetInitialPosition()
		{
			Transform3D Trans = GetNode().Transform;
			Trans.Origin = new Vector3(0, 0, 0);
			GetNode().Transform = Trans;
		}

		/// <summary>
		/// Checks if the message bus is valid.
		/// </summary>
		/// <returns>True if the message bus is valid, otherwise false.</returns>
		private bool Is_GlobalExplorerValid()
		{
			return null != _GlobalExplorer && null != _GlobalExplorer.Settings;
		}
	}
}
#endif
