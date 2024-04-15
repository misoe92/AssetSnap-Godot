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
	using System;
	using AssetSnap.ASNode.Types;
	using AssetSnap.Core;
	using AssetSnap.Front.Nodes;
	using AssetSnap.Trait;
	using Godot;
	
	[Tool]
	public partial class Plugin : EditorPlugin
	{
		// ~Plugin()
		// {
		// 	// Code to run when object is being finalized (disposed)
		// 	// This will run automatically when the object is garbage collected
		// 	Dispose(true);
		// }
		
		[Signal]
		public delegate void FoldersLoadedEventHandler();
		
		[Signal]
		public delegate void SettingKeyChangedEventHandler();
		
		[Signal]
		public delegate void LibraryChangedEventHandler( string name );
		
		[Signal]
		public delegate void StatesChangedEventHandler();

		/** Internal data **/
		private bool disposed = false;
		private readonly string _Version = "0.1.2";
		private readonly string _Name = "Plugin";
		
		/* Bottom Dock */
		public TraitGlobal traitGlobal {
			get
			{
				return TraitGlobal.Singleton;
			}
			set 
			{
				GD.Print("Cant be set");
			}
		}
			
		[Export]
		public Node internalNode;
		
		[Export] 
		public AsBottomDock _dock;
	
		protected Callable? UpdateHandleCallable;
		private bool _initialized = false;
		
		/** Internal Components **/
		private CoreEnter _CoreEnter = new();
		private CoreInput _CoreInput = new();
		private CoreProcess _CoreProcess = new();
		private CoreHandles _CoreHandles = new();

		/** Editor Node Types **/ 
		public NodeType[] NodeTypes = Array.Empty<NodeType>();
		private static Plugin _Instance;
		
		public static Plugin Singleton 
		{
			get
			{
				return _Instance;
			}
		}
		
		public static Plugin GetInstance()
		{
			return Singleton;
		}
		
		public Plugin()
		{	
			_Instance = this;
		}
		
		/*
		** Initialization of our plugin
		** 
		** @return void
		*/ 
		public override void _EnterTree() 
		{
			Name = "AssetSnapPlugin";
			AddChild(traitGlobal);
			
			if( null == internalNode ) 
			{
				internalNode = new()
				{
					Name = "InternalNode"
				};
				
				AddChild(internalNode);
			}
			_dock = GD.Load<PackedScene>("res://addons/assetsnap/scenes/dock.tscn").Instantiate<AsBottomDock>();
			AddControlToBottomPanel(_dock, "Assets");
			
			
			if( null == GlobalExplorer.InitializeExplorer() ) 
			{
				GD.PushError("No explorer is available");
				return;
			}

			Connect(SignalName.SceneChanged, Callable.From( (Node scene) => { _OnSceneChanged(scene); } ) );
			// UpdateHandleCallable = new(this, "UpdateHandle");
			
			// if(false == IsUpdateHandleConnected()) 
			// {
			// 	EditorInterface.Singleton.GetInspector().Connect(EditorInspector.SignalName.EditedObjectChanged, UpdateCallable());
			// }
		}
		
		public override void _Ready()
		{
			// Finalize Initialize of plugin 
			_CoreEnter.InitializeCore();
		}
	
		public Node GetInternalContainer() 
		{
			return internalNode;
		} 
		
		private void _OnSceneChanged(Node Scene)
		{
			GlobalExplorer.GetInstance().States.CurrentScene = Scene;
		}
		
		/*
		** Destruction of our plugin
		**
		** @return void 
		*/
		public override void _ExitTree()
		{
			disposed = true;
			
			if( null != _dock ) 
			{
				RemoveControlFromBottomPanel(_dock);
				_dock.Free();
			}
		} 
		 
		/*
		** Handling of communication with editor handles
		** 
		** @return bool 
		*/
		public override bool _Handles( GodotObject _object ) 
		{
			if (_CoreHandles == null || disposed )
			{
				return true;
			}
			
			return _CoreHandles.Handle(_object);
		} 
		
		/*
		** Handling of GUI Input in the editor.
		**
		** @return int
		*/
		public override int _Forward3DGuiInput( Camera3D camera, InputEvent @event ) 
		{
			// If internal component is not set
			if( _CoreInput == null || disposed ) 
			{
				return (int)EditorPlugin.AfterGuiInput.Pass;
			}
			
			return _CoreInput.Handle(camera, @event);
		}
		
		/*
		** Handling process ticks
		**
		** @return void 
		*/
		public override void _Process( double delta ) 
		{
			if( _CoreProcess == null || disposed ) 
			{
				return;
			}
			 
			_CoreProcess.Tick(delta);

			return;
		}
		
		/*
		** Used to check whether or not HandleNode is in focus,
		** and if not perform an action
		**  
		** @return void
		*/
		protected void UpdateHandle()
		{
			GlobalExplorer explorer = GlobalExplorer.GetInstance();
			if( EditorInterface.Singleton.GetInspector().GetEditedObject() == null) 
			{
				explorer.HandleNode = null; 
				if( null != explorer.CurrentLibrary ) 
				{
					explorer.CurrentLibrary._LibrarySettings._LSEditing.SetText("None");
				} else 
				{
					if( null != explorer.Library && null != explorer.Library.Libraries ) 
					{
						// Goes through all libraries and resets it's data
						foreach( Library.Instance _Library in explorer.Library.Libraries ) 
						{
							if( null != _Library._LibrarySettings ) 
							{
								_Library._LibrarySettings.ClearAll();							
								_Library._LibrarySettings._LSEditing.SetText("None");
							}
						}
					}
				}

				if(explorer.ContextMenu == null )
				{
					return;	
				}
				
				explorer.ContextMenu.Hide();
			}
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
		 
		public TabContainer GetTabContainer()
		{
			return _dock._TabContainer; 
			// AsBottomDock; 
		}
		
		/*
		** Fetches the current plugin version
		**
		** @return double
		*/
		public string GetVersion()
		{
			return _Version;
		}
		
		/*
		** Fetches the current plugin version in a string
		**
		** @return string
		*/
		public string GetVersionString()
		{
			return _Version;
		}
		
		// public void OnBeforeSerialize()
		// {

		// }

		// public void OnAfterDeserialize()
		// {
		// 	GD.Print("We're ready now. Target sprite" + (_dock == null ? " is" : " is not") + " null");
		// }
	}
}
#endif 