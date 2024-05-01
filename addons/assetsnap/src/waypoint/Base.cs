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
	using System.Collections.Generic;
	using AssetSnap.Explorer;
	using AssetSnap.Front.Nodes;
	using AssetSnap.Nodes;
	using AssetSnap.States;
	using AssetSnap.Static;
	using Godot;

	public partial class Base
	{
		private GlobalExplorer _GlobalExplorer;
		private Node _ParentContainer;
		public WaypointList WaypointList;
		public float SnapDistance = 1.0f;
		public Node3D WorkingNode;
		private static Base _Instance;
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

		public void Initialize()
		{
			WaypointList = new();
			_GlobalExplorer = GlobalExplorer.GetInstance();
		}

		/*
		** Spawns a model and also creates the collisions for said model
		** if defined in settings
		**
		** @param Node3D ModelInstance
		** @param Vector3 Origin
		** @param Vector3 Rotation
		** @param Vector3 Scale
		** @return Node3D
		*/
		public void Spawn(Node3D ModelInstance, Vector3 Origin, Vector3 Rotation, Vector3 Scale)
		{
			Node3D _model = new();
			Node3D model = new();
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
					_model.QueueFree();
					_model = ModelInstance as AsGrouped3D;
				}
				else if (ModelInstance is AsGroup3D)
				{
					_model.QueueFree();
					_model = ModelInstance as AsGroup3D;
				}
				else if (ModelInstance is AsNode3D)
				{
					_model.QueueFree();
					_model = ModelInstance as AsNode3D;
				}

				if (null == _model)
				{
					return;
				}

				if (_model is AssetSnap.Front.Nodes.AsMeshInstance3D meshInstance3D)
				{
					meshInstance3D.SetLibraryName(GlobalExplorer.GetInstance().CurrentLibrary.GetName());
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

		private bool IsSimpleMode()
		{
			return GlobalExplorer.GetInstance().States.PlacingType == GlobalStates.PlacingTypeEnum.Simple;
		}

		private bool IsOptimizedMode()
		{
			return GlobalExplorer.GetInstance().States.PlacingType == GlobalStates.PlacingTypeEnum.Optimized;
		}

		private void SimpleSpawn(Node3D model, Vector3 Origin, Vector3 Rotation, Vector3 Scale)
		{
			_SpawnNode(model);
			_ConfigureNode(model, Origin, Rotation, Scale);
		}

		private void OptimizedSpawn(Node3D model, Vector3 Origin, Vector3 Rotation, Vector3 Scale)
		{
			if (model is AsMeshInstance3D meshInstance3D)
			{
				int InstanceId = _OptimizedSpawn(meshInstance3D, Origin, Rotation, Scale);
				GlobalExplorer.GetInstance().States.EditingObject = null;
				meshInstance3D.Free();
			}

			if (model is AsNode3D node3D)
			{
				foreach (AsMeshInstance3D child in node3D.GetChildren())
				{
					int InstanceId = _OptimizedSpawn(child, Origin, Rotation, Scale);
					GlobalExplorer.GetInstance().States.EditingObject = null;
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

		/*
		** Removes a waypoint given it's ModelInstance
		** and it's origin x,y,z point
		**
		** @param MeshInstance3D ModelInstance
		** @param Vector3 Origin
		** @return void
		*/
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

		/*
		** Registers a new waypoint
		**
		** @param Node3D node
		** @param Vector3 Origin
		** @param Vector3 Rot
		** @param Vector3 Scale
		** @return void
		*/
		public void Register(Node3D node, Vector3 Origin, Vector3 Rot, Vector3 Scale)
		{
			if (null == WaypointList)
			{
				return;
			}

			WaypointList.Add(node, Origin, Rot, Scale);
		}

		/*
		** Checks if a node is already registered
		**
		** @param Node3D node
		** @return bool
		*/
		public bool Has(Node3D node)
		{
			if (null == WaypointList)
			{
				return false;
			}

			return WaypointList.Has(node);
		}
		/*
		** Updates the scale value on a waypoint
		** positioned on a given origin x,y,z point
		**
		** @param Vector3 Origin
		** @param Vector3 Scale
		** @return void
		*/
		public void UpdateScaleOnPoint(Vector3 Origin, Vector3 Scale)
		{
			if (null == WaypointList)
			{
				return;
			}

			WaypointList.Update("Scale", Scale, Origin);
		}

		/*
		** Fetches the node that is currently
		** being worked on
		**
		** @return Node3D
		*/
		public Node3D GetWorkingNode()
		{
			return WorkingNode;
		}

		/*
		** Spawns the node
		**
		** @param Node3D _model
		** @return void
		*/
		private void _SpawnNode(Node3D _model)
		{
			if (_model.GetParent() != null)
			{
				_model.GetParent().RemoveChild(_model);
			}

			SceneTree Tree = _GlobalExplorer._Plugin.GetTree();
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
					_GlobalExplorer._Plugin.AddChild(_model);
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

		public void SetParentContainer(Node Container)
		{
			_ParentContainer = Container;
		}

		/*
		** Configures the node and it's
		** Transform, scale etc.
		**
		** @param Node3D _model
		** @param Vector3 Origin
		** @param Vector3 Rotation
		** @param Vector3 Scale
		** @return void
		*/
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

		/*
		** Checks if any waypoints is available
		**
		** @return bool
		*/
		public bool HasAnyWaypoints()
		{
			return false == WaypointList.IsEmpty();
		}
	}
}
#endif