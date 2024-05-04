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

using System.Collections.Generic;
using AssetSnap.Explorer;
using AssetSnap.Front.Components.Library;
using AssetSnap.Front.Nodes;
using AssetSnap.Static;
using Godot;

namespace AssetSnap.ContextMenu
{
	/// <summary>
	/// Base class for managing the context menu in the AssetSnap addon.
	/// </summary>
	public partial class Base : Node, ISerializationListener
	{
		public static Base Singleton
		{
			get
			{
				return _Instance;
			}
		}
		
		private static Base _Instance;
		
		private readonly PackedScene _Scene = GD.Load<PackedScene>("res://addons/assetsnap/scenes/ContextMenu.tscn");
		private List<string> _Components = new()
		{
			"LibrarySnapRotate",
			"LibrarySnapScale",
		}; 
		
		/// <summary>
		/// Constructor for the Base class.
		/// </summary>
		/// <remarks>
		/// This constructor initializes the Base class.
		/// It sets the name of the context menu node to "AssetSnapContextMenu" and assigns the current instance to the static <see cref="_Instance"/> property.
		/// </remarks>
		/// <returns>Void.</returns>
		public Base()
		{
			Name = "AssetSnapContextMenu";
			_Instance = this;
		}
		
		/// <summary>
		/// Method called before serialization.
		/// </summary>
		public void OnBeforeSerialize()
		{
			//
		}

		/// <summary>
		/// Method called after deserialization.
		/// </summary>
		public void OnAfterDeserialize()
		{
			_Instance = this;
		}
		
		/// <summary>
		/// Initializes the context handler.
		/// </summary>
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

			if ( _HasComponents() )
			{
				_InitializeComponents();
			}
		}
		
		/// <summary>
		/// This method is called when the node and its children are ready.
		/// </summary>
		/// <remarks>
		/// It calls the <see cref="_Initialize"/> method to initialize the context menu.
		/// </remarks>
		/// <returns>Void.</returns>
		public override void _Ready()
		{
			_Initialize();
		}
	
		/// <summary>
		/// Shows the context menu.
		/// </summary>
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
		
		/// <summary>
		/// Hides the context menu.
		/// </summary>
		public void Hide() 
		{
			if( false == IsContextMenuValid() )
			{
				return;
			}

			SetVisible(false);
		}
		
		/// <summary>
		/// Sets the current rotation values based on a given vector3 parameter.
		/// </summary>
		/// <param name="Rotation">The rotation vector.</param>
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
		
		/// <summary>
		/// Sets the current scale values based on a given vector3 parameter.
		/// </summary>
		/// <param name="Scale">The scale vector.</param>
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
		
		/// <summary>
		/// Sets the visibility state of the context menu.
		/// </summary>
		/// <param name="state">The visibility state.</param>
		public void SetVisible(bool state)
		{
			if( false == HasInstance() ) 
			{
				return;
			}
			
			GetInstance().Visible = state;
			GetInstance().Active = state;
		}
		
		/// <summary>
		/// Gets the instance of the context menu.
		/// </summary>
		/// <returns>The instance of the context menu.</returns>
		public AsContextMenu GetInstance()
		{	
			if( false == HasInstance() )
			{
				return null;
			}

			return GetNode("AsContextMenu") as AsContextMenu;
		}
		
		/// <summary>
		/// Fetches the current rotation values.
		/// </summary>
		/// <returns>The rotation values as a Vector3.</returns>
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
		
		/// <summary>
		/// Fetches the current scale values.
		/// </summary>
		/// <returns>The scale values as a Vector3.</returns>
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
		
		/// <summary>
		/// Fetches the current angle value.
		/// </summary>
		/// <returns>The current angle as an integer.</returns>
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
		
		/// <summary>
		/// Checks if the context menu is hidden.
		/// </summary>
		/// <returns>True if the context menu is hidden, false otherwise.</returns>
		public bool IsHidden() 
		{
			if( false == IsContextMenuValid() || false == _ShouldUseOverlay()) 
			{
				return true;
			}
			
			return GetInstance().Visible == false;
		}
		
		
		/// <summary>
		/// Checks if an instance of the context menu exists.
		/// </summary>
		/// <returns>True if an instance exists, false otherwise.</returns>
		public bool HasInstance()
		{
			if( false == HasNode("AsContextMenu") || false == EditorPlugin.IsInstanceValid( GetNode("AsContextMenu") ) ) 
			{
				return false;
			}

			return true;
		}
		
		/// <summary>
		/// Checks if the instance of the context menu is valid.
		/// </summary>
		/// <returns>True if the instance is valid, false otherwise.</returns>
		public bool IsContextMenuValid()
		{
			return
				EditorPlugin.IsInstanceValid(Plugin.Singleton) && 
				Plugin.Singleton.HasInternalContainer() &&
				null != GetInstance() &&
				EditorPlugin.IsInstanceValid(GetInstance()) &&
				null != GetInstance().GetParent();
		}
		
		/// <summary>
		/// Initializes the scene and adds it to the tree.
		/// </summary>
		/// <remarks>
		/// This method initializes the scene for the context menu and adds it to the tree.
		/// It sets the visibility of the context menu to false by default.
		/// </remarks>
		/// <returns>Void.</returns>
		private void _Initialize() 
		{
			SetVisible(false);			
		}

		/// <summary>
		/// Initializes the components needed for the context menu.
		/// </summary>
		private void _InitializeComponents()
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
	
		/// <summary>
		/// Checks if the components needed for the context menu are available.
		/// </summary>
		/// <returns>True if all components are available, false otherwise.</returns>
		private bool _HasComponents() 
		{
			return ExplorerUtils.Get().Components.HasAll(_Components.ToArray());
		}
		
		/// <summary>
		/// Checks if the context menu should be used or not.
		/// </summary>
		/// <returns>True if the context menu should be used, false otherwise.</returns>
		private bool _ShouldUseOverlay()
		{
			return SettingsStatic.ShouldUseASOverlay();
		}
	}
}
#endif