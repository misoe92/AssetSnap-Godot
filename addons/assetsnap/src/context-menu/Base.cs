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
	using AssetSnap.Static;
	using Godot;
	
	public partial class Base : Node, ISerializationListener
	{
		private readonly PackedScene _Scene = GD.Load<PackedScene>("res://addons/assetsnap/scenes/ContextMenu.tscn");
		public static bool _Instantiated = false;
		private List<string> Components = new()
		{
			"LibrarySnapRotate",
			"LibrarySnapScale",
		}; 
		
		private static Base _Instance;
		public static Base Singleton
		{
			get
			{
				return _Instance;
			}
		}
		
		public Base()
		{
			Name = "AssetSnapContextMenu";
			_Instance = this;
		}
		
		public void OnBeforeSerialize()
		{
			//
		}

		public void OnAfterDeserialize()
		{
			_Instance = this;
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
			
			if(	null != Plugin.Singleton && false == HasNode("AsContextMenu") ) 
			{
				AsContextMenu _NodeInstance = _Scene.Instantiate<AsContextMenu>();
				AddChild(_NodeInstance);
			}

			if ( HasComponents() )
			{
				InitializeComponents();
			}
		}
		
		public override void _Ready()
		{
			_Initialize();
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
			
			if( GetInstance() is AsContextMenu ContextMenu ) 
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
			
			if( GetInstance() is AsContextMenu ContextMenu ) 
			{
				ContextMenu.SetRotationX(Rotation.X);
				ContextMenu.SetRotationY(Rotation.Y);
				ContextMenu.SetRotationZ(Rotation.Z);
			}
			
			Node3D Handle = GlobalExplorer.GetInstance().GetHandle();
			Handle.RotationDegrees = Rotation;
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
			
			if( GetInstance() is AsContextMenu ContextMenu ) 
			{
				ContextMenu.SetScaleX(Scale.X);
				ContextMenu.SetScaleY(Scale.Y);
				ContextMenu.SetScaleZ(Scale.Z);
			}
			
			Node3D Handle = GlobalExplorer.GetInstance().GetHandle();
			Handle.Scale = Scale;
		}
		
		public AsContextMenu GetInstance()
		{	
			if( false == HasInstance() )
			{
				return null;
			}

			return GetNode("AsContextMenu") as AsContextMenu;
		}
		
		public bool HasInstance()
		{
			if( false == HasNode("AsContextMenu") || false == EditorPlugin.IsInstanceValid( GetNode("AsContextMenu") ) ) 
			{
				return false;
			}

			return true;
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
			
			if( GetInstance() is AsContextMenu ContextMenu ) 
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
			
			if( GetInstance() is AsContextMenu ContextMenu ) 
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
			if( false == HasInstance() ) 
			{
				return;
			}
			
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
			return
				EditorPlugin.IsInstanceValid(Plugin.Singleton) && 
				Plugin.Singleton.HasInternalContainer() &&
				null != GetInstance() &&
				EditorPlugin.IsInstanceValid(GetInstance()) &&
				null != GetInstance().GetParent();
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
			if( false == Plugin.Singleton.HasInternalContainer() ) 
			{
				return;
			}
			
			SnapRotate _LibrarySnapRotate = ExplorerUtils.Get().Components.Single<AssetSnap.Front.Components.Library.SnapRotate>();
			SnapScale _LibrarySnapScale = ExplorerUtils.Get().Components.Single<AssetSnap.Front.Components.Library.SnapScale>();
			SnapGrab _LibrarySnapGrab = ExplorerUtils.Get().Components.Single<AssetSnap.Front.Components.Library.SnapGrab>();
			
			if( null != _LibrarySnapRotate ) 
			{
				_LibrarySnapRotate.Initialize();
				AddChild(_LibrarySnapRotate);
			}
			
			if( null != _LibrarySnapScale ) 
			{
				_LibrarySnapScale.Initialize();
				AddChild(_LibrarySnapScale);
			}
			
			if( null != _LibrarySnapGrab ) 
			{
				_LibrarySnapGrab.Initialize();
				AddChild(_LibrarySnapGrab);
			}
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
			return SettingsStatic.ShouldUseASOverlay();
		}
	}
}
#endif