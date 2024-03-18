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
	using AssetSnap.Front.Nodes;
	using Godot;

	public partial class Base : Node
	{
		private AsDecal3D _Decal;
		private Mesh _ModelMesh;
		private MeshInstance3D _ModelMeshInstance;
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

		private static Base _Instance;
		
		public static Base GetInstance()
		{
			if( null == _Instance ) 
			{
				_Instance = new()
				{
					Name = "AssetSnapDecal",
				};
			}

			return _Instance;
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
		
		/*
		** Updates the preview model shown in the decal
		**
		** @param bool State
		** @return void
		*/
		public void _UpdateDecalPreview( bool State ) 
		{
			if( false == _GlobalExplorer.HandleIsModel() && IsInstanceValid(_Decal) ) 
			{
				return;
			}

			AsMeshInstance3D model = _GlobalExplorer.GetHandle() as AssetSnap.Front.Nodes.AsMeshInstance3D;
			AsMeshInstance3D Model = null;
			bool ChildFound = false;
						
			if (_Decal.GetChildCount() > 0)
			{
				Model = _Decal.GetChild(0) as AssetSnap.Front.Nodes.AsMeshInstance3D;
				ChildFound = true;
	
				ClearCurrentChildren(Model, model);
			}

			PlaceDecalPreview( State, ChildFound, Model, model );
			
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
			if( null == ModelMeshInstance || null == ModelMesh )
			{
				return;
			}
			
			_Decal.Visible = true;
		}
		
		/*
		** Hides the decal
		**
		** @return void
		*/
		public void Hide()
		{
			if( _Decal == null ) 
			{
				return;
			}
			
			_Decal.Visible = false;
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
		private void ClearCurrentChildren( AssetSnap.Front.Nodes.AsMeshInstance3D Model, AssetSnap.Front.Nodes.AsMeshInstance3D model )
		{
			if( IsInstanceValid( Model ) && Model != model ) 
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
		private void PlaceDecalPreview( bool State, bool ChildFound, AssetSnap.Front.Nodes.AsMeshInstance3D Model, AssetSnap.Front.Nodes.AsMeshInstance3D model )
		{
			if( true == State && false == ChildFound) 
			{
				EditorInterface.Singleton.EditNode(model);
				_Decal.AddChild(model);
				ModelMeshInstance = model;
				ModelMesh = ModelMeshInstance.Mesh;
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

		public override void _ExitTree()
		{
			if( IsInstanceValid(Decal)) 
			{
				Decal.QueueFree();
			}
			
			base._ExitTree();
		}
	}
}
#endif
