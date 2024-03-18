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
	using Godot;

	[Tool]
	public partial class Plugin : EditorPlugin
	{
		[Signal]
		public delegate void SettingKeyChangedEventHandler();
	
		/** Internal data **/
		private readonly double _Version = 0.1;
		private readonly string _Name = "Plugin";
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

		public Node CurrentScene;
		
		public static Plugin GetInstance()
		{
			return _Instance;
		} 
		
		/*
		** Initialization of our plugin
		** 
		** @return void
		*/ 
		public override void _EnterTree() 
		{
			Name = "AssetSnapPlugin";
			Plugin._Instance = this; 

			if( null == GlobalExplorer.InitializeExplorer() ) 
			{
				GD.PushError("No explorer is available");
				return;
			}
			
			GlobalExplorer.GetInstance()._Plugin.SceneChanged += (scene) => { _OnSceneChanged(scene); };
			
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
		
		/*
		** Destruction of our plugin
		**
		** @return void 
		*/ 
		public override void _ExitTree()
		{
			_CoreEnter = null;
			_CoreInput = null;
			_CoreProcess = null;
			_CoreHandles = null;

			_Instance = null;
			UpdateHandleCallable = null; 
		} 

		/*
		** Handling of communication with editor handles
		** 
		** @return bool 
		*/
		public override bool _Handles( GodotObject _object ) 
		{
			if (_CoreHandles == null)
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
			if( _CoreInput == null ) 
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
			if( _CoreProcess == null ) 
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
		
		private void _OnSceneChanged(Node Scene)
		{
			CurrentScene = Scene;
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
		** Fetches the current plugin version
		**
		** @return double
		*/
		public double GetVersion()
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
			return _Version.ToString().Split(",").Join(".");
		}
	}
}
#endif
