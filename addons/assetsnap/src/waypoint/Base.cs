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
namespace AssetSnap.Waypoint
{
	using System;
	using AssetSnap.Front.Nodes;
	using AssetSnap.States;
	using AssetSnap.Static;
	using Godot;

	/// <summary>
	/// Provides functionality for managing waypoints in the scene.
	/// </summary>
	public partial class Base
	{
		private Node _ParentContainer;
		public WaypointList WaypointList;
		public float SnapDistance = 1.0f;
		public Node3D WorkingNode;
		private static Base _Instance;
		
		/// <summary>
		/// Gets the singleton instance of the waypoint system.
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
		/// Initializes the waypoint system.
		/// </summary>
		public void Initialize()
		{
			WaypointList = new();
		}

		/// <summary>
		/// Spawns a model and creates the collisions for said model if defined in settings.
		/// </summary>
		/// <param name="ModelInstance">The model instance to spawn.</param>
		/// <param name="Origin">The origin position of the model.</param>
		/// <param name="Rotation">The rotation of the model.</param>
		/// <param name="Scale">The scale of the model.</param>
		public void Spawn(Node3D ModelInstance, Vector3 Origin, Vector3 Rotation, Vector3 Scale)
		{
			Node3D _model = null;
			Node3D model = null;
			try
			{
				if (ModelInstance is AsMeshInstance3D _meshInstance)
				{
					_model = ModelInstance as AssetSnap.Front.Nodes.AsMeshInstance3D;
					_model.Name = ModelInstance.Name;
					_meshInstance.Floating = true;
				}
				else if (ModelInstance is AsGrouped3D)
				{
					_model = ModelInstance as AsGrouped3D;
				}
				else if (ModelInstance is AsGroup3D)
				{
					_model = ModelInstance as AsGroup3D;
				}
				else if (ModelInstance is AsNode3D)
				{
					_model = ModelInstance as AsNode3D;
				}

				if (null == _model)
				{
					return;
				}

				if (_model is AssetSnap.Front.Nodes.AsMeshInstance3D meshInstance3D)
				{
					meshInstance3D.SetLibraryName(StatesUtils.Get().CurrentLibrary.GetName());
				}

				if (_model is AsNode3D node3d)
				{
					node3d.SetLibraryName(StatesUtils.Get().CurrentLibrary.GetName());
				}
				
				model = _model.Duplicate() as Node3D;

				if (IsSimpleMode())
				{
					SimpleSpawn(model, Origin, Rotation, Scale);
				}
				else if (IsOptimizedMode())
				{
					OptimizedSpawn(model, Origin, Rotation, Scale);
				}
			}
			catch (Exception e)
			{
				GD.PushError(e);
			}

			WorkingNode = model;
			if (EditorPlugin.IsInstanceValid(_model))
			{
				if (EditorPlugin.IsInstanceValid(_model.GetParent()))
				{
					_model.GetParent().RemoveChild(_model);
				}

				_model.QueueFree();
			}
		}

		/// <summary>
		/// Checks if the current placing mode is set to Simple.
		/// </summary>
		/// <returns>True if the placing mode is Simple, otherwise false.</returns>
		private bool IsSimpleMode()
		{
			return StatesUtils.Get().PlacingType == GlobalStates.PlacingTypeEnum.Simple;
		}

		/// <summary>
		/// Checks if the current placing mode is set to Optimized.
		/// </summary>
		/// <returns>True if the placing mode is Optimized, otherwise false.</returns>
		private bool IsOptimizedMode()
		{
			return StatesUtils.Get().PlacingType == GlobalStates.PlacingTypeEnum.Optimized;
		}

		/// <summary>
		/// Spawns a model using the simple placement method.
		/// </summary>
		/// <param name="model">The model to spawn.</param>
		/// <param name="Origin">The origin position of the model.</param>
		/// <param name="Rotation">The rotation of the model.</param>
		/// <param name="Scale">The scale of the model.</param>
		private void SimpleSpawn(Node3D model, Vector3 Origin, Vector3 Rotation, Vector3 Scale)
		{
			_SpawnNode(model);
			_ConfigureNode(model, Origin, Rotation, Scale);
		}

		/// <summary>
		/// Spawns a model using the optimized placement method.
		/// </summary>
		/// <param name="model">The model to spawn.</param>
		/// <param name="Origin">The origin position of the model.</param>
		/// <param name="Rotation">The rotation of the model.</param>
		/// <param name="Scale">The scale of the model.</param>
		private void OptimizedSpawn(Node3D model, Vector3 Origin, Vector3 Rotation, Vector3 Scale)
		{
			if (model is AsMeshInstance3D meshInstance3D)
			{
				int InstanceId = _OptimizedSpawn(meshInstance3D, Origin, Rotation, Scale);
				StatesUtils.Get().EditingObject = null;
				meshInstance3D.Free();
			}

			if (model is AsNode3D node3D)
			{
				foreach (AsMeshInstance3D child in node3D.GetChildren())
				{
					int InstanceId = _OptimizedSpawn(child, Origin, Rotation, Scale);
					StatesUtils.Get().EditingObject = null;
				}
				node3D.Free();
			}

			if (model is AsGrouped3D grouped3D)
			{
				_SpawnNode(grouped3D);
				_ConfigureNode(grouped3D, Origin, Rotation, Scale);
				grouped3D.OptimizedSpawn = true;
				foreach (Node3D child in grouped3D.GetChildren())
				{
					if (child is AsNode3D)
					{
						foreach( AsMeshInstance3D asMeshInstance3D in child.GetChildren())
						{
							int InstanceId = _OptimizedSpawn(asMeshInstance3D, Origin + child.Transform.Origin, asMeshInstance3D.RotationDegrees, asMeshInstance3D.Scale);
							grouped3D.AddConnection(InstanceId, StatesUtils.Get().OptimizedGroups[asMeshInstance3D.Mesh][InstanceId], asMeshInstance3D.Mesh);
						}
					}
					else if (child is AsMeshInstance3D)
					{
						AsMeshInstance3D asMeshInstance3D = child as AsMeshInstance3D;
						int InstanceId = _OptimizedSpawn(asMeshInstance3D, Origin + asMeshInstance3D.Transform.Origin, asMeshInstance3D.RotationDegrees, asMeshInstance3D.Scale);
						grouped3D.AddConnection(InstanceId, StatesUtils.Get().OptimizedGroups[asMeshInstance3D.Mesh][InstanceId], asMeshInstance3D.Mesh);
					}
				}

				grouped3D.Clear();
				grouped3D.Update();
			}
		}

		/// <summary>
		/// Handles the optimized spawning of a single mesh instance.
		/// </summary>
		/// <param name="meshInstance3D">The mesh instance to spawn.</param>
		/// <param name="Origin">The origin position of the mesh instance.</param>
		/// <param name="Rotation">The rotation of the mesh instance.</param>
		/// <param name="Scale">The scale of the mesh instance.</param>
		/// <returns>The instance ID of the spawned mesh instance.</returns>
		private int _OptimizedSpawn(AsMeshInstance3D meshInstance3D, Vector3 Origin, Vector3 Rotation, Vector3 Scale)
		{
			Node3D AsChunks = null;
			
			if( false == Plugin.Singleton.GetTree().EditedSceneRoot.HasNode("AsChunks") ) 
			{
				AsChunks = new()
				{
					Name = "AsChunks"
				};
				Plugin.Singleton.GetTree().EditedSceneRoot.AddChild(AsChunks);
				AsChunks.Owner = Plugin.Singleton.GetTree().EditedSceneRoot;
				Plugin.Singleton.GetTree().EditedSceneRoot.MoveChild(AsChunks, 0);
			}
			else 
			{
				AsChunks = Plugin.Singleton.GetTree().EditedSceneRoot.GetNode("AsChunks") as Node3D;
			}
			
			int InstanceId = 0;
			Transform3D transform = meshInstance3D.GlobalTransform;
			Mesh mesh = meshInstance3D.Mesh;
			transform.Scaled(Scale);
			// Convert the rotation from degrees to radians
			float rotationRadiansX = Mathf.DegToRad(Rotation.X);
			float rotationRadiansY = Mathf.DegToRad(Rotation.Y);
			float rotationRadiansZ = Mathf.DegToRad(Rotation.Z);

			// Create a rotation basis around each axis
			Basis rotationBasisX = Basis.Identity.Rotated(Vector3.Right, rotationRadiansX);
			Basis rotationBasisY = Basis.Identity.Rotated(Vector3.Up, rotationRadiansY);
			Basis rotationBasisZ = Basis.Identity.Rotated(Vector3.Forward, rotationRadiansZ);

			// Combine the rotation around each axis
			Basis finalRotation = rotationBasisX * rotationBasisY * rotationBasisZ;

			// Assuming you have a transform called transform
			transform.Basis = finalRotation;
			transform.Origin = Origin;
			
			/*
			** Add rules
			*/
			Godot.Collections.Dictionary<string, Variant> rules = new();
			
			if( StatesUtils.Get().LevelOfDetailsState == GlobalStates.LibraryStateEnum.Enabled ) 
			{
				rules.Add("LevelOfDetails", StatesUtils.Get().LevelOfDetails);
			}
			
			if( StatesUtils.Get().VisibilityRangeBegin != 0 ) 
			{
				rules.Add("VisibilityRangeBegin", StatesUtils.Get().VisibilityRangeBegin);
			}
			
			if( StatesUtils.Get().VisibilityRangeBeginMargin != 0 ) 
			{
				rules.Add("VisibilityRangeBeginMargin", StatesUtils.Get().VisibilityRangeBeginMargin);
			}
			
			if( StatesUtils.Get().VisibilityRangeEnd != 0 ) 
			{
				rules.Add("VisibilityRangeEnd", StatesUtils.Get().VisibilityRangeEnd);
			}
			
			if( StatesUtils.Get().VisibilityRangeEndMargin != 0 ) 
			{
				rules.Add("VisibilityRangeEndMargin", StatesUtils.Get().VisibilityRangeEndMargin);
			}
			
			if( StatesUtils.Get().VisibilityFadeMode != "Use project default" ) 
			{
				rules.Add("VisibilityFadeMode", StatesUtils.Get().VisibilityFadeMode);
			}
			
			if (StatesUtils.Get().OptimizedGroups.ContainsKey(mesh))
			{
				bool found = false;
				int index = 0;
				// Already has a group
				foreach( AsOptimizedMultiMeshGroup3D multiMeshGroup in StatesUtils.Get().OptimizedGroups[mesh] ) 
				{
					if( multiMeshGroup.RulesEqual( rules ) ) 
					{
						InstanceId = multiMeshGroup.AddToBuffer(transform);
						multiMeshGroup.Update();
						found = true;
						break;
					}
					
					index += 1;
				}
				
				if( found == false ) 
				{
					// None found that have the same rules as we are working with, as such we should
					// create a new one.
					
					// Create our optimized multi mesh group
					AsOptimizedMultiMeshGroup3D newGroup = new()
					{
						Name = "AsMultimesh-" + meshInstance3D.Name,
						Object = mesh,
					};

					_ParentContainer = AsChunks;
					_SpawnNode(newGroup);
					_ParentContainer = null;
					InstanceId = newGroup.AddToBuffer(transform);
					newGroup.SetRules(rules);
					
					StatesUtils.Get().OptimizedGroups[mesh][ StatesUtils.Get().OptimizedGroups[mesh].Count - 1 ].Update();

					return StatesUtils.Get().OptimizedGroups[mesh].Count - 1;
				}

				return index;
			}
		
			// Create our optimized multi mesh group
			AsOptimizedMultiMeshGroup3D group = new()
			{
				Name = "AsMultimesh-" + meshInstance3D.Name,
				Object = mesh,
			};

			_ParentContainer = AsChunks;
			_SpawnNode(group);
			_ParentContainer = null;
			InstanceId = group.AddToBuffer(transform);
			
			group.SetRules(rules);
			
			StatesUtils.Get().OptimizedGroups[mesh][0].Update();

			return 0;
		}

		/// <summary>
		/// Removes a waypoint given its ModelInstance and its origin point.
		/// </summary>
		/// <param name="ModelInstance">The model instance to remove.</param>
		/// <param name="Origin">The origin position of the waypoint.</param>
		public void Remove(Node ModelInstance, Vector3 Origin)
		{
			if (null == WaypointList)
			{
				return;
			}

			if (EditorPlugin.IsInstanceValid(ModelInstance))
			{
				WaypointList.Remove(ModelInstance, Origin);
			}
		}

		/// <summary>
		/// Registers a new waypoint.
		/// </summary>
		/// <param name="node">The node to register.</param>
		/// <param name="Origin">The origin position of the node.</param>
		/// <param name="Rot">The rotation of the node.</param>
		/// <param name="Scale">The scale of the node.</param>
		public void Register(Node3D node, Vector3 Origin, Vector3 Rot, Vector3 Scale)
		{
			if (null == WaypointList)
			{
				return;
			}

			WaypointList.Add(node, Origin, Rot, Scale);
		}

		/// <summary>
		/// Checks if a node is already registered as a waypoint.
		/// </summary>
		/// <param name="node">The node to check.</param>
		/// <returns>True if the node is registered, otherwise false.</returns>
		public bool Has(Node3D node)
		{
			if (null == WaypointList)
			{
				return false;
			}

			return WaypointList.Has(node);
		}
		
		/// <summary>
		/// Updates the scale value on a waypoint positioned on a given origin point.
		/// </summary>
		/// <param name="Origin">The origin position of the waypoint.</param>
		/// <param name="Scale">The new scale value.</param>
		public void UpdateScaleOnPoint(Vector3 Origin, Vector3 Scale)
		{
			if (null == WaypointList)
			{
				return;
			}

			WaypointList.Update("Scale", Scale, Origin);
		}

		/// <summary>
		/// Retrieves the node that is currently being worked on.
		/// </summary>
		/// <returns>The working node.</returns>
		public Node3D GetWorkingNode()
		{
			return WorkingNode;
		}

		/// <summary>
		/// Spawns the specified Node3D and sets its owner and parent container if necessary.
		/// </summary>
		/// <param name="_model">The Node3D to spawn.</param>
		private void _SpawnNode(Node3D _model)
		{
			if (_model.GetParent() != null)
			{
				_model.GetParent().RemoveChild(_model);
			}

			SceneTree Tree = Plugin.Singleton.GetTree();
			if (SettingsStatic.ShouldPushToScene())
			{
				if (_ParentContainer != null)
				{
					Tree = _ParentContainer.GetTree();
					if (Tree != null)
					{
						_ParentContainer.AddChild(_model, true);
						_model.Owner = Tree.EditedSceneRoot;
					}
				}
				else
				{
					if (Tree != null)
					{
						Tree.EditedSceneRoot.AddChild(_model, true);
						_model.Owner = Tree.EditedSceneRoot;
					}
					else
					{
						GD.PushWarning("Tree not found");
					}
				}

				if (0 != _model.GetChildCount())
				{
					for (int i = 0; i < _model.GetChildCount(); i++)
					{
						_model.GetChild(i).Owner = Tree.EditedSceneRoot;
						
						if( 
							_model.GetChild(i) is Node3D node3d &&
							_model.GetChild(i).GetChildCount() > 0 && 
							EditorPlugin.IsInstanceValid(_model.GetChild(i).GetChild(0))
						) 
						{
							_model.GetChild(i).GetChild(0).Owner = Tree.EditedSceneRoot;
						}
					}
				}
			}
			else
			{
				if (_ParentContainer != null)
				{
					_ParentContainer.AddChild(_model);
				}
				else
				{
					Plugin.Singleton.AddChild(_model);
				}
			}

			if (_model is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D && true == asMeshInstance3D.Floating && false == SettingsStatic.ShouldAddCollision())
			{
				// AssetSnap.ASNode.MeshInstance.SpawnSettings SpawnSettings = asMeshInstance3D.SpawnSettings;
				// SpawnSettings.Update();
				asMeshInstance3D.SetIsFloating(false);
			}

			_model.NotifyPropertyListChanged();
		}

		/// <summary>
		/// Sets the parent container for spawning nodes.
		/// </summary>
		/// <param name="Container">The parent container node.</param>
		public void SetParentContainer(Node Container)
		{
			_ParentContainer = Container;
		}

		/// <summary>
		/// Configures the node's transform, scale, etc.
		/// </summary>
		/// <param name="_model">The node to configure.</param>
		/// <param name="Origin">The origin position of the node.</param>
		/// <param name="Rotation">The rotation of the node.</param>
		/// <param name="Scale">The scale of the node.</param>
		private void _ConfigureNode(Node3D _model, Vector3 Origin, Vector3 Rotation, Vector3 Scale)
		{
			Transform3D _Trans = _model.Transform;
			_Trans.Origin = Origin;
			_model.Transform = _Trans;
			_model.RotationDegrees = Rotation;
			_model.Scale = Scale;
			
			if( _model is AsMeshInstance3D asMeshInstance3D ) 
			{
				if( StatesUtils.Get().LevelOfDetailsState == GlobalStates.LibraryStateEnum.Enabled ) 
				{
					asMeshInstance3D.LodBias = StatesUtils.Get().LevelOfDetails; 
				}

				if (StatesUtils.Get().VisibilityRangeBegin != 0)
				{
					asMeshInstance3D.VisibilityRangeBegin = StatesUtils.Get().VisibilityRangeBegin;
				}
				
				if (StatesUtils.Get().VisibilityRangeBeginMargin != 0)
				{
					asMeshInstance3D.VisibilityRangeBeginMargin = StatesUtils.Get().VisibilityRangeBeginMargin;
				}
				
				if (StatesUtils.Get().VisibilityRangeEnd != 0)
				{
					asMeshInstance3D.VisibilityRangeEnd = StatesUtils.Get().VisibilityRangeEnd;
				}
				
				if (StatesUtils.Get().VisibilityRangeEndMargin != 0)
				{
					asMeshInstance3D.VisibilityRangeEndMargin = StatesUtils.Get().VisibilityRangeEndMargin;
				}
				
				if (StatesUtils.Get().VisibilityFadeMode != "Use project default")
				{
					switch( StatesUtils.Get().VisibilityFadeMode ) 
					{
						case "Disabled":
							asMeshInstance3D.VisibilityRangeFadeMode = GeometryInstance3D.VisibilityRangeFadeModeEnum.Disabled;
							break;
							
						case "Self":
							asMeshInstance3D.VisibilityRangeFadeMode = GeometryInstance3D.VisibilityRangeFadeModeEnum.Self;
							break;
							
							
						case "Dependencies":
							asMeshInstance3D.VisibilityRangeFadeMode = GeometryInstance3D.VisibilityRangeFadeModeEnum.Dependencies;
							break;
					}
				}
			}
			
			if( _model is AsNode3D asNode3D)
			{
				foreach( MeshInstance3D meshInstance3D in asNode3D.GetChildren() )
				{
					if( StatesUtils.Get().LevelOfDetailsState == GlobalStates.LibraryStateEnum.Enabled ) 
					{
						meshInstance3D.LodBias = StatesUtils.Get().LevelOfDetails;
					}

					if (StatesUtils.Get().VisibilityRangeBegin != 0)
					{
						meshInstance3D.VisibilityRangeBegin = StatesUtils.Get().VisibilityRangeBegin;
					}
					
					if (StatesUtils.Get().VisibilityRangeBeginMargin != 0)
					{
						meshInstance3D.VisibilityRangeBeginMargin = StatesUtils.Get().VisibilityRangeBeginMargin;
					}
					
					if (StatesUtils.Get().VisibilityRangeEnd != 0)
					{
						meshInstance3D.VisibilityRangeEnd = StatesUtils.Get().VisibilityRangeEnd;
					}
					
					if (StatesUtils.Get().VisibilityRangeEndMargin != 0)
					{
						meshInstance3D.VisibilityRangeEndMargin = StatesUtils.Get().VisibilityRangeEndMargin;
					}
					
					if (StatesUtils.Get().VisibilityFadeMode != "Use project default")
					{
						switch( StatesUtils.Get().VisibilityFadeMode ) 
						{
							case "Disabled":
								meshInstance3D.VisibilityRangeFadeMode = GeometryInstance3D.VisibilityRangeFadeModeEnum.Disabled;
								break;
								
							case "Self":
								meshInstance3D.VisibilityRangeFadeMode = GeometryInstance3D.VisibilityRangeFadeModeEnum.Self;
								break;
								
								
							case "Dependencies":
								meshInstance3D.VisibilityRangeFadeMode = GeometryInstance3D.VisibilityRangeFadeModeEnum.Dependencies;
								break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Checks if any waypoints are available.
		/// </summary>
		/// <returns>True if there are any waypoints, otherwise false.</returns>
		public bool HasAnyWaypoints()
		{
			return false == WaypointList.IsEmpty();
		}
	}
}
#endif