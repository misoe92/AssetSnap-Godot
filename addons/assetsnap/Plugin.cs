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

using System;
using AssetSnap.ASNode.Types;
using AssetSnap.Core;
using AssetSnap.Explorer;
using AssetSnap.Front.Nodes;
using AssetSnap.Nodes;
using AssetSnap.States;
using AssetSnap.Trait;
using Godot;

namespace AssetSnap
{
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

		/* Bottom Dock */
		[Export]
		public AsBottomDock Dock;
		
		/* The `public static Plugin Singleton` property is providing a way to access the singleton instance of
		the `Plugin` class. By defining this property as static, it allows other parts of the code to access
		the single instance of the `Plugin` class that is stored in the `_Instance` variable. This ensures
		that there is only one instance of the `Plugin` class throughout the application, promoting a
		singleton design pattern. */
		public static Plugin Singleton
		{
			get
			{
				return _Instance;
			}
		}
		
		public TraitGlobal TraitGlobal
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
				//
			}
		}
		
		/** Editor Node Types **/
		public NodeType[] NodeTypes = Array.Empty<NodeType>();
		
		/** Internal data **/
		protected Callable? _UpdateHandleCallable;

		private static Plugin _Instance;
		
		private readonly string _Version = "0.1.2";
		private readonly string _Name = "Plugin";
		private bool _Disposed = false;
		private CoreEnter _CoreEnter = new();
		private CoreInput _CoreInput = new();
		private CoreProcess _CoreProcess = new();
		private CoreHandles _CoreHandles = new();

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

			AddChild(TraitGlobal);
			
			if ( false == HasInternalContainer() )
			{
				Node InternalNode = new()
				{
					Name = "InternalNode"
				};

				AddChild(InternalNode);
			}
			
			Dock = new AsBottomDock();
			AddControlToBottomPanel(Dock, "Assets");
			
			Connect(EditorPlugin.SignalName.SceneChanged, 
				Callable.From(
					(Node scene) =>
					{
						_OnSceneChanged(scene);
					}
				)
			);

			if (null == GlobalExplorer.InitializeExplorer())
			{
				GD.PushError("No explorer is available");
				return;
			}

			_UpdateHandleCallable = new(this, "UpdateHandle");
			if(false == _IsUpdateHandleConnected()) 
			{
				EditorInterface.Singleton.GetInspector().Connect(EditorInspector.SignalName.EditedObjectChanged, UpdateCallable());
			}
			
			_CoreEnter.InitializeCore();
		}

		public override void _Ready()
		{
			// Finalize Initialize of plugin 

			OnRemoveFolder += (string title) => { CallDeferred("_DoRemoveFolder", title); };

			ModelPreviewer.Singleton.Enter();
			ModelPreviewer.Singleton.GeneratePreviews();
		}
		
		/// <summary>
		/// This C# function overrides the _Process method to tick the _CoreProcess object if it is not null and
		/// not disposed.
		/// </summary>
		/// <param name="delta">The `delta` parameter in the `_Process` method represents the time elapsed in
		/// seconds since the last frame. It is typically used to update the game logic and perform actions that
		/// need to be executed every frame, such as moving objects, updating animations, or handling
		/// input.</param>
		/// <returns>
		/// In the provided code snippet, the `return` statement is being used to exit the method `_Process`
		/// early under certain conditions. If either `_CoreProcess` is `null` or the variable `disposed` is
		/// `true`, the method will return without executing the remaining code.
		/// </returns>
		public override void _Process(double delta)
		{
			if (_CoreProcess == null || _Disposed)
			{
				return;
			}

			_CoreProcess.Tick(delta);

			return;
		}
		
		/// <summary>
		/// The _ExitTree function disposes of various custom node types, frees nodes in dispose queues,
		/// removes and frees controls, and queues free the internal container and traitGlobal.
		/// </summary>
		public override void _ExitTree()
		{
			_Disposed = true;
			
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
			
			Plugin.Singleton.RemoveChild(ExplorerUtils.Get().Library);
			Plugin.Singleton.RemoveChild(ExplorerUtils.Get().Components);
			Plugin.Singleton.RemoveChild(ExplorerUtils.Get().ContextMenu);
			Plugin.Singleton.RemoveChild(ExplorerUtils.Get().Inspector);

			ExplorerUtils.Get().Library.Free();
			ExplorerUtils.Get().Components.Free();
			ExplorerUtils.Get().ContextMenu.Free();
			ExplorerUtils.Get().Inspector.Free();
			
			foreach (GodotObject _object in TraitGlobal.DisposeQueue)
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

			if (null != Dock)
			{
				RemoveControlFromBottomPanel(Dock);
				Dock.Free();
			}
			
			if ( HasInternalContainer() && null != GetInternalContainer())
			{
				GetInternalContainer().QueueFree();
			}

			if (null != TraitGlobal)
			{
				TraitGlobal.Free();
			}
		}
		
		/// <summary>
		/// This C# function overrides a method to handle 3D GUI input using a specified camera and input event.
		/// </summary>
		/// <param name="Camera3D">The `Camera3D` parameter represents a 3D camera used for rendering and
		/// displaying the scene in a 3D environment. It typically contains information such as the camera's
		/// position, orientation, field of view, and projection matrix. This parameter is used in the
		/// `_Forward3DGuiInput</param>
		/// <param name="InputEvent">An object representing an input event, such as a mouse click or keyboard
		/// press.</param>
		/// <returns>
		/// The method is returning the result of calling the `Handle` method on the `_CoreInput` object with
		/// the `camera` and `@event` parameters.
		/// </returns>
		public override int _Forward3DGuiInput(Camera3D camera, InputEvent @event)
		{
			// If internal component is not set
			if (_CoreInput == null || _Disposed)
			{
				return (int)EditorPlugin.AfterGuiInput.Pass;
			}

			return _CoreInput.Handle(camera, @event);
		}
		
		/// <summary>
		/// The UpdateCallable function returns a Callable object.
		/// </summary>
		/// <returns>
		/// A Callable object is being returned.
		/// </returns>
		public Callable UpdateCallable()
		{
			return (Callable)_UpdateHandleCallable;
		}
		
		/// <summary>
		/// This C# function returns the internal container node named "InternalNode" if it exists, otherwise
		/// it returns null.
		/// </summary>
		/// <returns>
		/// The method `GetInternalContainer` is returning a `Node` object. If `HasInternalContainer()`
		/// returns false, then it will return null. Otherwise, it will return the node with the name
		/// "InternalNode".
		/// </returns>
		public Node GetInternalContainer()
		{
			if( false == HasInternalContainer() )
			{
				return null;
			}
			
			return GetNode("InternalNode");
		}
		
		/// <summary>
		/// The GetTabContainer function returns the TabContainer from the _dock object.
		/// </summary>
		/// <returns>
		/// The method `GetTabContainer` is returning the `_TabContainer` property of the `_dock` object.
		/// </returns>
		public TabContainer GetTabContainer()
		{
			return Dock._TabContainer;
		}
		
		/// <summary>
		/// The GetVersion function in C# returns the version of the software.
		/// </summary>
		/// <returns>
		/// The method GetVersion() is returning the value stored in the variable _Version.
		/// </returns>
		public string GetVersion()
		{
			return _Version;
		}

		/// <summary>
		/// The GetVersionString function in C# returns the version string.
		/// </summary>
		/// <returns>
		/// The method `GetVersionString()` is returning the `_Version` string.
		/// </returns>
		public string GetVersionString()
		{
			return _Version;
		}
		
		/// <summary>
		/// The _Handles function checks if _CoreHandles is valid and not disposed before calling its Handle
		/// method on a GodotObject.
		/// </summary>
		/// <param name="GodotObject">the object currently being handled</param>
		/// <returns>
		/// The method is returning a boolean value. If `_CoreHandles` is null or the object has been
		/// disposed, it returns `true`. Otherwise, it calls the `Handle` method on `_CoreHandles` passing
		/// `_object` as a parameter and returns the result of that method call.
		/// </returns>
		public override bool _Handles(GodotObject _object)
		{
			if (_CoreHandles == null || _Disposed)
			{
				return true;
			}

			return _CoreHandles.Handle(_object);
		}
		
		/// <summary>
		/// The function `HasInternalContainer` checks if an internal node exists and is valid.
		/// </summary>
		/// <returns>
		/// The method `HasInternalContainer()` is returning a boolean value. It returns `true` if the node
		/// named "InternalNode" exists and is a valid instance, otherwise it returns `false`.
		/// </returns>
		public bool HasInternalContainer()
		{
			return HasNode("InternalNode") && IsInstanceValid(GetNode("InternalNode"));
		}

		/// <summary>
		/// The function DoRemoveFolder removes a folder with the specified title from the settings using
		/// ExplorerUtils in C#.
		/// </summary>
		/// <param name="title">The `title` parameter is a string that represents the title of the folder that
		/// needs to be removed.</param>
		private void _DoRemoveFolder(string title)
		{
			ExplorerUtils.Get().Settings.RemoveFolder(title);
		}

		/// <summary>
		/// The _OnSceneChanged function updates the visibility of UI elements based on the current scene in
		/// C#.
		/// </summary>
		/// <param name="Node">In the provided code snippet, the `_OnSceneChanged` method takes a parameter
		/// `Node Scene`. This parameter represents a Node object that is passed to the method when the scene
		/// is changed. The method then updates the visibility of `_dock._TabContainer` and
		/// `_dock._LoadingContainer` based on</param>
		private void _OnSceneChanged(Node Scene)
		{
			StatesUtils.Get().CurrentScene = Scene;

			if (StatesUtils.Get().CurrentScene == null)
			{
				Dock._TabContainer.Visible = false;
				Dock._LoadingContainer.Visible = true;
			}
			else
			{
				Dock._TabContainer.Visible = true;
				Dock._LoadingContainer.Visible = false;
			}
		}

		/// <summary>
		/// The UpdateHandle function checks if an object is being edited and updates the handle accordingly.
		/// </summary>
		/// <returns>
		/// If the condition `EditorInterface.Singleton.GetInspector().GetEditedObject() == null` is true, then
		/// the method will return without performing any further actions.
		/// </returns>
		protected void _UpdateHandle()
		{
			GlobalExplorer explorer = GlobalExplorer.GetInstance();
			if (EditorInterface.Singleton.GetInspector().GetEditedObject() == null)
			{
				explorer.HandleNode = null;
				if (null != StatesUtils.Get().CurrentLibrary)
				{
					StatesUtils.Get().CurrentLibrary._LibrarySettings.Editing.SetText("None");
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
								_Library._LibrarySettings.Editing.SetText("None");
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

		/// <summary>
		/// The function checks if the update handle is connected to the editor inspector for edited object
		/// changes.
		/// </summary>
		/// <returns>
		/// The method `IsUpdateHandleConnected` returns a boolean value indicating whether the update handle
		/// is connected. If the `UpdateHandleCallable` is an instance of `Callable`, it checks if the
		/// `EditorInspector` is connected to the `EditedObjectChanged` signal with the callable object. If
		/// the conditions are met, it returns `true`, otherwise it returns `false`.
		/// </returns>
		protected bool _IsUpdateHandleConnected()
		{
			if (_UpdateHandleCallable is Callable callable)
			{
				EditorInspector Inspector = EditorInterface.Singleton.GetInspector();
				return Inspector.IsConnected(EditorInspector.SignalName.EditedObjectChanged, callable);
			}

			return false;
		}
	}
}
#endif