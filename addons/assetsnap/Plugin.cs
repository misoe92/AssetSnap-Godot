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
	using System.Collections.Generic;
	using AssetSnap.ASNode.Types;
	using AssetSnap.Core;
	using AssetSnap.Explorer;
	using AssetSnap.Front.Nodes;
	using AssetSnap.Nodes;
	using AssetSnap.States;
	using AssetSnap.Trait;
	using Godot;

	[Tool]
	public partial class Plugin : EditorPlugin, ISerializationListener
	{
		[Signal]
		public delegate void FoldersLoadedEventHandler();

		[Signal]
		public delegate void SettingKeyChangedEventHandler(Godot.Collections.Array data);

		[Signal]
		public delegate void LibraryChangedEventHandler(string name);

		[Signal]
		public delegate void StatesChangedEventHandler(Godot.Collections.Array data);

		[Signal]
		public delegate void OnRemoveFolderEventHandler(string folder);

		[Signal]
		public delegate void OnLibraryPopulizedEventHandler();

		[Signal]
		public delegate void ModelSizeCacheChangedEventHandler(string model, Vector3 value);

		/** Internal data **/
		private List<string> ignoredExtensions = new List<string> { ".png", ".jpg", ".jpeg", ".tga", ".bmp" }; // Add the extensions you want to ignore
		private bool disposed = false;
		private readonly string _Version = "0.1.2";
		private readonly string _Name = "Plugin";

		/* Bottom Dock */
		public TraitGlobal traitGlobal
		{
			get
			{
				if( false == IsInstanceValid( TraitGlobal.Singleton ) ) 
				{
					return null;
				}
				
				return TraitGlobal.Singleton;
			}
			set
			{
				// GD.Print("Cant be set");
			}
		}

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
			Name = "AssetSnapPlugin";
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
		** Initialization of our plugin
		** 
		** @return void
		*/
		public override void _EnterTree()
		{
			_Instance = this;

			AddChild(traitGlobal);
			if ( false == HasInternalContainer() )
			{
				Node internalNode = new()
				{
					Name = "InternalNode"
				};

				AddChild(internalNode);
			}
			
			_dock = new AsBottomDock();
			AddControlToBottomPanel(_dock, "Assets");
			Connect(EditorPlugin.SignalName.SceneChanged, Callable.From((Node scene) => { _OnSceneChanged(scene); }));

			if (null == GlobalExplorer.InitializeExplorer())
			{
				GD.PushError("No explorer is available");
				return;
			}

			UpdateHandleCallable = new(this, "UpdateHandle");
			if(false == IsUpdateHandleConnected()) 
			{
				EditorInterface.Singleton.GetInspector().Connect(EditorInspector.SignalName.EditedObjectChanged, UpdateCallable());
			}
			
			_CoreEnter.InitializeCore();
		}

		public override void _Ready()
		{
			// Finalize Initialize of plugin 

			OnRemoveFolder += (string title) => { CallDeferred("DoRemoveFolder", title); };

			ModelPreviewer.Singleton.Enter();
			ModelPreviewer.Singleton.GeneratePreviews();
		}

		private void DoRemoveFolder(string title)
		{
			ExplorerUtils.Get().Settings.RemoveFolder(title);
		}

		public bool HasInternalContainer()
		{
			return HasNode("InternalNode") && IsInstanceValid(GetNode("InternalNode"));
		}
		
		public Node GetInternalContainer()
		{
			if( false == HasInternalContainer() )
			{
				return null;
			}
			
			return GetNode("InternalNode");
		}

		private void _OnSceneChanged(Node Scene)
		{
			StatesUtils.Get().CurrentScene = Scene;

			if (StatesUtils.Get().CurrentScene == null)
			{
				_dock._TabContainer.Visible = false;
				_dock._LoadingContainer.Visible = true;
			}
			else
			{
				_dock._TabContainer.Visible = true;
				_dock._LoadingContainer.Visible = false;
			}
		}

		/*
		** Destruction of our plugin
		**
		** @return void 
		*/
		public override void _ExitTree()
		{
			disposed = true;
			
			/** Initialize custom node types **/  
			new ASNode.Types.AsNodeType().Dispose( this );
			new ASNode.Types.AsGroupType().Dispose( this );
			new ASNode.Types.AsGroupedType().Dispose( this );
			new ASNode.Types.AsArrayModifierType().Dispose( this );
			new ASNode.Types.AsScatterModifierType().Dispose( this );
			new ASNode.Types.AsStaticBodyType().Dispose( this );
			new ASNode.Types.AsListSelectType().Dispose( this );
			new ASNode.Types.AsMeshInstanceType().Dispose( this );
			new ASNode.Types.AsMultiMeshInstanceType().Dispose( this );
			new ASNode.Types.AsOptimizedMultiMeshGroupType().Dispose( this );
			new ASNode.Types.AsMultiMeshType().Dispose( this );
			
			foreach (GodotObject _object in traitGlobal.DisposeQueue)
			{
				if (IsInstanceValid(_object) && _object is Node node)
				{
					if (IsInstanceValid(node) && node.HasMethod(Node.MethodName.Free))
					{
						if (null != node.GetParent())
						{
							node.GetParent().RemoveChild(node);
						}

						node.Free();
					}
					else
					{
						GD.Print("Could not free", node.Name);
					}
				}
				else
				{
					GD.Print("Not valid");
				}
			}

			if (ExplorerUtils.Get().Components.DisposeQueue.Count != 0)
			{
				foreach (GodotObject gobject in ExplorerUtils.Get().Components.DisposeQueue)
				{
					if (IsInstanceValid(gobject) && gobject is Node node)
					{
						node.Free();
					}
				}
			}

			if (null != _dock)
			{
				RemoveControlFromBottomPanel(_dock);
				_dock.Free();
			}
			
			if ( HasInternalContainer() && null != GetInternalContainer())
			{
				GetInternalContainer().QueueFree();
			}

			if (null != traitGlobal)
			{
				traitGlobal.Free();
			}
		}

		/*
		** Handling of communication with editor handles
		** 
		** @return bool 
		*/
		public override bool _Handles(GodotObject _object)
		{
			if (_CoreHandles == null || disposed)
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
		public override int _Forward3DGuiInput(Camera3D camera, InputEvent @event)
		{
			// If internal component is not set
			if (_CoreInput == null || disposed)
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
		public override void _Process(double delta)
		{
			if (_CoreProcess == null || disposed)
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
			if (EditorInterface.Singleton.GetInspector().GetEditedObject() == null)
			{
				explorer.HandleNode = null;
				if (null != explorer.CurrentLibrary)
				{
					explorer.CurrentLibrary._LibrarySettings._LSEditing.SetText("None");
				}
				else
				{
					if (null != explorer.Library && null != explorer.Library.Libraries)
					{
						// Goes through all libraries and resets it's data
						foreach (Library.Instance _Library in explorer.Library.Libraries)
						{
							if (null != _Library._LibrarySettings)
							{
								_Library._LibrarySettings.ClearAll();
								_Library._LibrarySettings._LSEditing.SetText("None");
							}
						}
					}
				}

				if (explorer.ContextMenu == null)
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
	}
}
#endif