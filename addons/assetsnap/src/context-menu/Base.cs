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
namespace AssetSnap.ContextMenu
{
	using System.Collections.Generic;
	using AssetSnap.Explorer;
	using AssetSnap.Front.Components.Library;
	using AssetSnap.Front.Nodes;
	using Godot;
	
	public partial class Base
	{
		public string Name = "";
		private readonly PackedScene _Scene = GD.Load<PackedScene>("res://addons/assetsnap/scenes/ContextMenu.tscn");
		private AsContextMenu _NodeInstance;
		private List<string> Components = new()
		{
			"LibrarySnapRotate",
			"LibrarySnapScale",
		}; 
		
		public Base()
		{
			Name = "AssetSnapContextMenu";
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
		** Initializes the context handler
		*/ 
		public void Initialize()
		{
			if( false == _ShouldUseOverlay()) 
			{
				return;
			}

			if ( HasComponents() ) 
			{
				InitializeComponents();
			}  
		}
		
		public void _Ready()
		{
			_Initialize();
		}
		
		/*
		** Handles viewability of the contextmenu based
		** on certain parameters.
		**
		** @param double delta
		** @return void
		*/
		public void _Process(double delta)
		{
			// if( false == Shown && _ShouldUseOverlay()) 
			// {
			// 	Shown = true; 
 
			// 	if( GlobalExplorer.GetInstance().HandleIsModel() ) 
			// 	{
			// 		SetVisible(true);
			// 	}
			// }
			// else if( true == Shown && false == _ShouldUseOverlay() ) 
			// { 
			// 	ClearOverlay();
			// }
		} 
		
		/*
		** Shows the context menu
		**
		** @return void
		*/
		public void Show()
		{
			if( false == IsContextMenuValid() )
			{
				return;
			}
			
			SetVisible(true);
			
			if( _NodeInstance is AsContextMenu ContextMenu ) 
			{
				Node3D Handle = GlobalExplorer.GetInstance().GetHandle();
				
				if( EditorPlugin.IsInstanceValid( Handle ) ) 
				{
					ContextMenu.SetRotationX(Handle.RotationDegrees.X);
					ContextMenu.SetRotationY(Handle.RotationDegrees.Y);
					ContextMenu.SetRotationZ(Handle.RotationDegrees.Z);
					
					ContextMenu.SetScaleX(Handle.Scale.Z);
					ContextMenu.SetScaleY(Handle.Scale.Z);
					ContextMenu.SetScaleZ(Handle.Scale.Z);
				}
			}
		}
		
		/*
		** Hides the context menu
		**
		** @return void
		*/
		public void Hide() 
		{
			if( false == IsContextMenuValid() )
			{
				return;
			}

			SetVisible(false);
		}
		
		/*
		** Sets the current rotation values based on a given vector3
		** parameter
		**
		** @param Vector3 Rotation
		** @return void
		*/
		public void SetRotationValues( Vector3 Rotation ) 
		{
			if(false == _ShouldUseOverlay() ||  false == IsContextMenuValid() ) 
			{
				return;
			}
			
			if( _NodeInstance is AsContextMenu ContextMenu ) 
			{
				ContextMenu.SetRotationX(Rotation.X);
				ContextMenu.SetRotationY(Rotation.Y);
				ContextMenu.SetRotationZ(Rotation.Z);
			}

			if( false == IsParentBody() ) 
			{
				Node3D Handle = GlobalExplorer.GetInstance().GetHandle();
				Handle.RotationDegrees = Rotation;
			}
			else if( GlobalExplorer.GetInstance().HandleIsModel() && IsParentBody() )
			{
				UpdateModelBody("Rotation", Rotation);
			}
		}
		
		/*
		** Sets the current scale values based on a given vector3
		** parameter
		**
		** @param Vector3 Scale
		** @return void
		*/
		public void SetScaleValues( Vector3 Scale ) 
		{
			if(false == _ShouldUseOverlay() || false == IsContextMenuValid() ) 
			{
				return;
			}
			
			if( _NodeInstance is AsContextMenu ContextMenu ) 
			{
				ContextMenu.SetScaleX(Scale.X);
				ContextMenu.SetScaleY(Scale.Y);
				ContextMenu.SetScaleZ(Scale.Z);
			}
			
			if( false == IsParentBody() ) 
			{
				Node3D Handle = GlobalExplorer.GetInstance().GetHandle();
				Handle.Scale = Scale;
			}
			else if( GlobalExplorer.GetInstance().HandleIsModel() && IsParentBody() )
			{
				UpdateModelBody("Scale", Scale);
			}
		}
		
		/*
		** If certain conditions are met it
		** will query a collision update
		** on the body
		**
		** @return void
		*/
		public void UpdateModelBody( string Type, Vector3 Value ) 
		{
			AsStaticBody3D _body = GetParentBody();
			_body.Update( Type, Value );
		}
		
		public AsContextMenu GetInstance()
		{	
			if(	null == _NodeInstance && null != Plugin.Singleton) 
			{
				_NodeInstance = _Scene.Instantiate<AsContextMenu>();
				Plugin.Singleton.GetInternalContainer().AddChild(_NodeInstance);
			}
			
			return _NodeInstance;
		}
		
		/*
		** Fetches the current rotation values
		**
		** @return Vector3
		*/
		public Vector3 GetRotateValues()
		{
			if(false == _ShouldUseOverlay() || false == IsContextMenuValid() ) 
			{
				return Vector3.Zero;
			}
			
			if( _NodeInstance is AsContextMenu ContextMenu ) 
			{
				return new Vector3(ContextMenu.GetRotationX(), ContextMenu.GetRotationY(), ContextMenu.GetRotationZ());
			}
			
			return Vector3.Zero;
		}
		
		/*
		** Fetches the current scale values
		**
		** @return Vector3
		*/
		public Vector3 GetScaleValues()
		{
			if(false == _ShouldUseOverlay() || false == IsContextMenuValid() ) 
			{
				return Vector3.Zero;
			}
			
			if( _NodeInstance is AsContextMenu ContextMenu ) 
			{
				return new Vector3(ContextMenu.GetScaleX(), ContextMenu.GetScaleY(), ContextMenu.GetScaleZ());
			}
			
			return Vector3.Zero;
		}
		
		/*
		** Fetches the current angle value
		**
		** @return int
		*/
		public int GetCurrentAngle()
		{
			if(false == _ShouldUseOverlay() || false == IsContextMenuValid() ) 
			{
				return 0;
			}
			
			if( GetInstance() is AsContextMenu ContextMenu ) 
			{
				return ContextMenu.GetAngleIndex();
			}

			return 0;
		}
		
		/*
		** Defines the visibility state of
		** the context menu
		*/
		public void SetVisible(bool state)
		{
			GetInstance().Visible = state;
			GetInstance().Active = state;
		}
		
		/*
		** Checks if context menu is hidden
		**
		** @return bool
		*/
		public bool IsHidden() 
		{
			if( false == IsContextMenuValid() || false == _ShouldUseOverlay()) 
			{
				return true;
			}
			
			return GetInstance().Visible == false;
		}
		
		/*
		** Checks if the instance of ContextMenu is valid or not
		**
		** @return bool
		*/
		public bool IsContextMenuValid()
		{
			return EditorPlugin.IsInstanceValid(GetInstance()) && null != GetInstance().GetParent();
		}
		
		/*
		** Checks if the instance of ContextMenu is valid or not
		**
		** @return bool
		*/
		public bool IsParentBody()
		{
			AsMeshInstance3D MeshInstance = GlobalExplorer.GetInstance().GetHandle() as AssetSnap.Front.Nodes.AsMeshInstance3D;
			Node3D parent = MeshInstance.GetParent() as Node3D;
			
			return parent is AsStaticBody3D;
		}
		/*
		** Checks if the instance of ContextMenu is valid or not
		**
		** @return bool
		*/
		public AsStaticBody3D GetParentBody()
		{
			AsMeshInstance3D MeshInstance = GlobalExplorer.GetInstance().GetHandle() as AssetSnap.Front.Nodes.AsMeshInstance3D;
			AsStaticBody3D parent = MeshInstance.GetParent() as AsStaticBody3D;
			
			return parent;
		}
		/*
		** Initialies the scene and adds
		** it to the tree
		**
		** @return void
		*/
		private void _Initialize() 
		{
			SetVisible(false);			
		}

		/*
		** Initialies the components needed
		** for the context menu
		**
		** @return void
		*/
		public void InitializeComponents()
		{
			SnapRotate _LibrarySnapRotate = ExplorerUtils.Get().Components.Single<AssetSnap.Front.Components.Library.SnapRotate>();
			SnapScale _LibrarySnapScale = ExplorerUtils.Get().Components.Single<AssetSnap.Front.Components.Library.SnapScale>();
			SnapGrab _LibrarySnapGrab = ExplorerUtils.Get().Components.Single<AssetSnap.Front.Components.Library.SnapGrab>();
			
			if( null != _LibrarySnapRotate ) 
			{
				_LibrarySnapRotate.Library = ExplorerUtils.Get().CurrentLibrary;
				_LibrarySnapRotate.Container = Plugin.Singleton.GetInternalContainer();
				_LibrarySnapRotate.Initialize();
			}
			
			if( null != _LibrarySnapScale ) 
			{
				_LibrarySnapScale.Library = ExplorerUtils.Get().CurrentLibrary;
				_LibrarySnapScale.Container = Plugin.Singleton.GetInternalContainer();
				_LibrarySnapScale.Initialize();
			}
			
			if( null != _LibrarySnapGrab ) 
			{
				_LibrarySnapGrab.Library = ExplorerUtils.Get().CurrentLibrary;
				_LibrarySnapGrab.Container = Plugin.Singleton.GetInternalContainer();
				_LibrarySnapGrab.Initialize();
			}
		}
		
		/*
		** Clears the instance and set's it's shown state to false
		**
		** @return void
		*/
		private void ClearOverlay() 
		{
			if( null != GetInstance() )  
			{
				GetInstance().QueueFree();
			}
		}
		
		/*
		** Checks if the message bus is connected
		**
		** @return bool
		*/
		private bool IsGlobalExplorerConnected()
		{
			return null != ExplorerUtils.Get() && null != ExplorerUtils.Get().Settings;
		}
		
		/*
		** Checks if the components needed are available
		** 
		** @return bool
		*/
		public bool HasComponents() 
		{
			return ExplorerUtils.Get().Components.HasAll(Components.ToArray());
		}
		
		/*
		** Checks if context menu should be used or not
		**
		** @return bool
		*/
		private bool _ShouldUseOverlay()
		{
			if( null == ExplorerUtils.Get() || null ==  ExplorerUtils.Get().Settings ) 
			{ 
				return false;
			}

			bool UseAsOverlay = ExplorerUtils.Get().Settings.GetKey("use_as_overlay").As<bool>();
			return UseAsOverlay;
		}
	}
}
#endif