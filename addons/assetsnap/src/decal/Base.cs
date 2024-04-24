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
	using AssetSnap.Instance.Input;
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
				if( null == _Instance ) 
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
			
			if( false == Is_GlobalExplorerValid() ) 
			{
				return;
			}
			
			InitializeNode();
			SetInitialPosition();
			AddNodeTo(GlobalExplorer.GetInstance()._Plugin);
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
		public void _UpdateDecalPreview( bool State ) 
		{
			if( false == EditorPlugin.IsInstanceValid( _GlobalExplorer.GetHandle() ) || false == EditorPlugin.IsInstanceValid(_Decal) ) 
			{
				return;
			}

			if( _GlobalExplorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Group ) 
			{
				AsGrouped3D group = _GlobalExplorer.States.GroupedObject;
				Node child = null;
				bool ChildFound = false;
							
				if (_Decal.GetChildCount() > 0)
				{
					child = _Decal.GetChild(0);
					ChildFound = true;
					ClearCurrentChildren(child, group);
				}

				PlaceDecalPreview( State, ChildFound, child, group );
			}
			
			if( _GlobalExplorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Model )
			{
				Node model = _GlobalExplorer.GetHandle();
				Node Model = null;
				bool ChildFound = false;

				if (_Decal.GetChildCount() > 0)
				{
					Model = _Decal.GetChild(0);
					ChildFound = true;
		
					ClearCurrentChildren(Model, model);
				}

				PlaceDecalPreview( State, ChildFound, Model, model );
			}
			
			if ( IsHidden() ) {
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
			if(
				ExplorerUtils.Get().GetHandle() == null ||
				ExplorerUtils.Get().GetHandle() is AsMeshInstance3D meshInstance3D &&
				meshInstance3D.IsPlaced() ||
				ExplorerUtils.Get().GetHandle() is AsGrouped3D grouped3d &&
				grouped3d.IsPlaced() ||
				ExplorerUtils.Get().GetHandle() is AsNode3D asnode3d && asnode3d.IsPlaced() ||
				ExplorerUtils.Get().InputDriver is DragAddInputDriver _input &&
				_input.IsDragging()
			)
			{
				return;
			}

			if( null == _Decal ) 
			{
				GD.PushError("Decal not instantiated");
				return;
			}
			
			_Decal.Visible = true;
			_GlobalExplorer.States.DecalVisible = GlobalStates.VisibilityStateEnum.Visible;
		}
		
		/*
		** Hides the decal
		**
		** @return void
		*/
		public void Hide()
		{
			if( _Decal == null || false == _Decal.Visible ) 
			{
				return;
			}
			
			_Decal.Visible = false;
			_GlobalExplorer.States.DecalVisible = GlobalStates.VisibilityStateEnum.Hidden;
		}
		
		/*
		** Fetches the Decal's node
		**
		** @return Node3D
		*/
		public Node3D GetNode()
		{
			return _Decal;
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
			return _Decal.Visible == false;
		}
		
		/*
		** Clears the models parent if it has one
		**
		** @param Node model
		** @return void
		*/
		private void MaybeClearModelParent( Node model )
		{
			if( model.GetParent() != null ) 
			{
				// Already placed
				model.GetParent().RemoveChild(model); 
			}
		}
		
		/*
		** Clears the current children of the decal as
		** long as it does not match the new child.
		**
		** @param AssetSnap.Front.Nodes.AsMeshInstance3D Model
		** @param AssetSnap.Front.Nodes.AsMeshInstance3D model
		** @return void
		*/
		private void ClearCurrentChildren( Node Model, Node model )
		{
			if( EditorPlugin.IsInstanceValid( Model ) && Model != model ) 
			{
				_Decal.RemoveChild(Model);			
			}
		}
		
		private void ClearChildren()
		{
			for( int i = 0; i < GlobalExplorer.GetInstance()._Plugin.GetChildCount(); i++) 
			{
				Node child = GlobalExplorer.GetInstance()._Plugin.GetChild(i);
				if( child is AsDecal3D ) 
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
		private void PlaceDecalPreview( bool State, bool ChildFound, Node Model, Node model )
		{
			if( true == State && false == ChildFound ) 
			{
				EditorInterface.Singleton.EditNode(model);
				_Decal.AddChild(model);
				if( model is AsMeshInstance3D meshInstance3D ) 
				{
					ModelMeshInstance = meshInstance3D;
					ModelMesh = ModelMeshInstance.Mesh;
				}
				
				if( model is AsGrouped3D group3D ) 
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
			_Decal = new();
		}
		
		/*
		** Set an initial position for our
		** decal node
		**
		** @return void
		*/
		private void SetInitialPosition()
		{
			Transform3D Trans = _Decal.Transform;
			Trans.Origin = new Vector3(0, 0, 0);
			_Decal.Transform = Trans;			
		}
		
		/*
		** Adds the decal node to a given node
		**
		** @return void
		*/
		private void AddNodeTo( Node To )
		{
			To.AddChild(_Decal);
			_GlobalExplorer.States.DecalSpawned = GlobalStates.SpawnStateEnum.Spawned;
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

		// public override void _ExitTree()
		// {
		// 	if( IsInstanceValid(Decal)) 
		// 	{
		// 		Decal.QueueFree();
		// 	}
			
		// 	base._ExitTree();
		// }
	}
}
#endif
