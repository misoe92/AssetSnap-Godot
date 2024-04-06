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
	using AssetSnap.Front.Nodes;
	using AssetSnap.Static;
	using Godot;
	
	public partial class Base : Node
	{
		private GlobalExplorer _GlobalExplorer;
		private Node _ParentContainer;
		public WaypointList WaypointList;
		public float SnapDistance = 1.0f;
		
		public Node3D WorkingNode;
		
		public Base()
		{
			Name = "AssetSnapWaypoint";
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

				if (null == _model)
				{
					return;
				}

				if (_model is AssetSnap.Front.Nodes.AsMeshInstance3D meshInstance3D)
				{
					meshInstance3D.SetLibraryName(GlobalExplorer.GetInstance().CurrentLibrary.GetName());
				}

				model = _model.Duplicate() as Node3D;

				if( IsSimpleMode() )
				{
					SimpleSpawn(model, Origin, Rotation, Scale);
				}
				else if( IsOptimizedMode() ) 
				{
					OptimizedSpawn(model, Origin, Rotation, Scale);
				}

			}
			catch (Exception e)
			{
				GD.PushError(e);
			}

			WorkingNode = model;
			if (IsInstanceValid(_model))
			{
				if (IsInstanceValid(_model.GetParent()))
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
			_SpawnNode( model );
			_ConfigureNode( model, Origin, Rotation, Scale );
			
			if( SettingsStatic.ShouldAddCollision() && model is AssetSnap.Front.Nodes.AsMeshInstance3D ) 
			{
				model = _AddCollisions(model);
			}
			else if ( SettingsStatic.ShouldAddCollision() && model is AsGrouped3D asGrouped3D )
			{
				asGrouped3D.Update();
			}
			
			// Focus the selected node in the editor
			// EditorInterface.Singleton.GetSelection().Clear();
			if( SettingsStatic.ShouldFocusAsset() && false == _GlobalExplorer.InputDriver.IsMulti ) 
			{
				_GlobalExplorer.Model = null;
				_GlobalExplorer.HandleNode = null;
				_GlobalExplorer.States.EditingObject = null;
				_GlobalExplorer.States.GroupedObject = null;
				_GlobalExplorer.InputDriver.FocusAsset(model);	
			}	
		}
		
		private void OptimizedSpawn(Node3D model, Vector3 Origin, Vector3 Rotation, Vector3 Scale)
		{
			if( model is AsMeshInstance3D meshInstance3D ) 
			{
				int InstanceId = _OptimizedSpawn(meshInstance3D, Origin, Rotation, Scale);

				GlobalExplorer.GetInstance().States.EditingObject = null;
				AddMultiCollisions(
					new List<Vector3>() { Origin },
					new List<Vector3>() { Scale },
					new List<Vector3>() { Rotation },
					new Godot.Collections.Array<Mesh>() { meshInstance3D.Mesh },
					meshInstance3D
				);
				// GlobalExplorer.GetInstance().States. = null;
				meshInstance3D.Free();
			}

			if (model is AsGrouped3D grouped3D)
			{
				Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> ChildOptions = grouped3D.ChildOptions;
				Node InitialChild = grouped3D.GetChild(0);
				grouped3D.OptimizedSpawn = true;
				List<Vector3> positions = new List<Vector3>();
				List<Vector3> rotations = new List<Vector3>();
				List<Vector3> scales = new List<Vector3>();
				Godot.Collections.Array<Mesh> meshes = new Godot.Collections.Array<Mesh>();
				
				foreach( Node3D child in grouped3D.GetChildren() )
				{
					if( InitialChild is AsStaticBody3D )
					{
						// With collisions
						AsMeshInstance3D asMeshInstance3D = child.GetChild(0) as AsMeshInstance3D;
						int InstanceId = _OptimizedSpawn(asMeshInstance3D, Origin + child.Transform.Origin, asMeshInstance3D.RotationDegrees, asMeshInstance3D.Scale);
						positions.Add(child.Transform.Origin);
						meshes.Add(asMeshInstance3D.Mesh);
						rotations.Add(asMeshInstance3D.RotationDegrees);
						scales.Add(asMeshInstance3D.Scale);
					}
					else if( InitialChild is AsMeshInstance3D) 
					{
						AsMeshInstance3D asMeshInstance3D = child as AsMeshInstance3D;
						// Without collisions
						int InstanceId = _OptimizedSpawn(asMeshInstance3D, Origin + asMeshInstance3D.Transform.Origin, asMeshInstance3D.RotationDegrees, asMeshInstance3D.Scale);
						
						positions.Add(asMeshInstance3D.Transform.Origin);
						meshes.Add(asMeshInstance3D.Mesh);
						rotations.Add(asMeshInstance3D.RotationDegrees);
						scales.Add(asMeshInstance3D.Scale);

						grouped3D.AddConnection(InstanceId, GlobalExplorer.GetInstance().States.OptimizedGroups[asMeshInstance3D.Mesh], asMeshInstance3D.Mesh );
					}
				}
				
				grouped3D.Clear();
				_SpawnNode(grouped3D);
				_ConfigureNode( grouped3D, Origin, Rotation, Scale );
								
				if( SettingsStatic.ShouldAddCollision() ) 
				{
					grouped3D.AddMultiCollisions( positions, scales, rotations, meshes, grouped3D, ChildOptions );	
				}
				
				// Focus the selected node in the editor
				// EditorInterface.Singleton.GetSelection().Clear();
				if( SettingsStatic.ShouldFocusAsset() && false == _GlobalExplorer.InputDriver.IsMulti ) 
				{
					_GlobalExplorer.Model = null;
					_GlobalExplorer.HandleNode = null;
					_GlobalExplorer.States.EditingObject = null;
					_GlobalExplorer.States.GroupedObject = null;
					_GlobalExplorer.InputDriver.FocusAsset(model);	
				}		
			}
		}
		
		public void AddMultiCollisions(List<Vector3> _Positions, List<Vector3> _Scales, List<Vector3> _Rotations, Godot.Collections.Array<Mesh> _Meshes, AsMeshInstance3D meshInstance3D )
		{
			Node _SceneRoot = GlobalExplorer.GetInstance()._Plugin.GetTree().EditedSceneRoot;

			for (int i = 0; i < _Positions.Count; i++)
			{
				Vector3 _Pos = _Positions[i];
				Transform3D _Trans = Transform3D.Identity;
				_Trans.Origin = new Vector3(_Pos.X, _Pos.Y, _Pos.Z);
				AsStaticBody3D _Body = new()
				{
					Transform = _Trans,
					UsingMultiMesh = true,
					Mesh = _Meshes[i],
					MeshName = _Meshes[i].ResourceName,
					InstanceTransform = _Trans,
					InstanceScale = _Scales[i],
					InstanceRotation = _Rotations[i],
				};

				_SpawnNode(_Body);
				_Body.Owner = _SceneRoot;

				int typeState = 0;
				int argState = 0;

				bool IsChildConvex = meshInstance3D.HasSetting("ConvexCollision") ? meshInstance3D.GetSetting("ConvexCollision").As<bool>() : false;
				bool IsChildConvexClean = meshInstance3D.HasSetting("ConvexClean") ? meshInstance3D.GetSetting("ConvexClean").As<bool>() : false;
				bool IsChildConvexSimplify = meshInstance3D.HasSetting("ConvexSimplify") ? meshInstance3D.GetSetting("ConvexSimplify").As<bool>() : false;
				bool IsChildConcave = meshInstance3D.HasSetting("ConcaveCollision") ? meshInstance3D.GetSetting("ConcaveCollision").As<bool>() : false;
				bool IsChildSphere = meshInstance3D.HasSetting("SphereCollision") ? meshInstance3D.GetSetting("SphereCollision").As<bool>() : false;

				if (
					GlobalStates.LibraryStateEnum.Enabled == GlobalExplorer.GetInstance().States.ConvexCollision &&
					false == IsChildConvex &&
					false == IsChildConcave &&
					false == IsChildSphere ||
					true == IsChildConvex	
				) 
				{
					typeState = 1;
					
					if(
						GlobalStates.LibraryStateEnum.Enabled == GlobalExplorer.GetInstance().States.ConvexClean &&
						GlobalStates.LibraryStateEnum.Disabled == GlobalExplorer.GetInstance().States.ConvexSimplify &&
						false == IsChildConvex ||
						true == IsChildConvexClean &&
						true == IsChildConvexSimplify
					) 
					{
						argState = 1;	
					}
					else if( 
						GlobalStates.LibraryStateEnum.Disabled == GlobalExplorer.GetInstance().States.ConvexClean &&
						GlobalStates.LibraryStateEnum.Enabled == GlobalExplorer.GetInstance().States.ConvexSimplify &&
						false == IsChildConvex ||
						false == IsChildConvexClean &&
						true == IsChildConvexSimplify
					) 
					{
						argState = 2;	
					}
					else if( 
						GlobalStates.LibraryStateEnum.Enabled == GlobalExplorer.GetInstance().States.ConvexClean &&
						GlobalStates.LibraryStateEnum.Enabled == GlobalExplorer.GetInstance().States.ConvexSimplify &&
						false == IsChildConvex ||
						true == IsChildConvexClean &&
						true == IsChildConvexSimplify
					) 
					{
						argState = 3;	
					}
				}
				else if( 
					GlobalStates.LibraryStateEnum.Enabled == GlobalExplorer.GetInstance().States.ConcaveCollision &&
					false == IsChildConvex &&
					false == IsChildConcave &&
					false == IsChildSphere ||
					true == IsChildConcave	
				) 
				{
					typeState = 2;
				}
				else if(
					GlobalStates.LibraryStateEnum.Enabled == GlobalExplorer.GetInstance().States.SphereCollision &&
					false == IsChildConvex &&
					false == IsChildConcave &&
					false == IsChildSphere ||
					true == IsChildSphere	
				) 
				{
					typeState = 3;
				}

				_Body.Initialize(typeState, argState);
			}
		}
		
		private int _OptimizedSpawn( AsMeshInstance3D meshInstance3D, Vector3 Origin, Vector3 Rotation, Vector3 Scale)
		{
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
			
			if( GlobalExplorer.GetInstance().States.OptimizedGroups.ContainsKey( mesh ) ) 
			{
				// Already has a group
				InstanceId = GlobalExplorer.GetInstance().States.OptimizedGroups[mesh].AddToBuffer(transform);
				GlobalExplorer.GetInstance().States.OptimizedGroups[mesh].Update();
			}
			else 
			{
				// Create our optimized multi mesh group
				AsOptimizedMultiMeshGroup3D group = new()
				{
					Name = "AsMultimesh-" + meshInstance3D.Name,
					Object = mesh,
				};
				
				_SpawnNode( group );
				InstanceId = group.AddToBuffer(transform);
				GlobalExplorer.GetInstance().States.OptimizedGroups[mesh].Update();
			}

			return InstanceId;
		}
		
		/*
		** Removes a waypoint given it's ModelInstance
		** and it's origin x,y,z point
		**
		** @param MeshInstance3D ModelInstance
		** @param Vector3 Origin
		** @return void
		*/
		public void Remove(MeshInstance3D ModelInstance, Vector3 Origin)
		{
			if( null == WaypointList ) 
			{
				return;
			}
			
			if( IsInstanceValid( ModelInstance ) ) 
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
		public void Register( Node3D node, Vector3 Origin, Vector3 Rot, Vector3 Scale ) 
		{
			if( null == WaypointList ) 
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
		public bool Has( Node3D node ) 
		{
			if( null == WaypointList ) 
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
			if( null == WaypointList ) 
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
		private void _SpawnNode( Node3D _model)
		{						
			if( _model.GetParent() != null ) 
			{
				_model.GetParent().RemoveChild(_model);
			}
			
			SceneTree Tree = _GlobalExplorer._Plugin.GetTree();
			if (SettingsStatic.ShouldPushToScene())
			{
				if( _ParentContainer != null ) 
				{ 
					Tree = _ParentContainer.GetTree();
					if( Tree != null ) 
					{
						_ParentContainer.AddChild(_model, true);
						_model.Owner = Tree.EditedSceneRoot;
					}
				}
				else 
				{
					if( Tree != null ) 
					{
						Tree.EditedSceneRoot.AddChild(_model, true);
						_model.Owner = Tree.EditedSceneRoot;
					}
					else 
					{
						GD.PushWarning("Tree not found");
					}
				}

				if( 0 != _model.GetChildCount() ) 
				{
					for( int i = 0; i < _model.GetChildCount(); i++ ) 
					{
						_model.GetChild(i).Owner = Tree.EditedSceneRoot;
					}
				}
			}
			else 
			{
				if( _ParentContainer != null ) 
				{
					_ParentContainer.AddChild(_model);
				}
				else 
				{
					_GlobalExplorer._Plugin.AddChild(_model);
				}
			}

			if( _model is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D && true == asMeshInstance3D.Floating && false == SettingsStatic.ShouldAddCollision() ) 
			{
				// AssetSnap.ASNode.MeshInstance.SpawnSettings SpawnSettings = asMeshInstance3D.SpawnSettings;
				// SpawnSettings.Update();
				asMeshInstance3D.SetIsFloating(false);
			}
			
			_model.NotifyPropertyListChanged();
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
		private void _ConfigureNode( Node3D _model, Vector3 Origin, Vector3 Rotation, Vector3 Scale )
		{
			Transform3D _Trans = _model.Transform;
			_Trans.Origin = Origin;

			_model.Transform = _Trans;
			_model.RotationDegrees = Rotation;
			_model.Scale = Scale;
		}
			
		/*
		** Adds collisions to a given model
		**
		** @param Node3D _model
		** @return void
		*/
		private Node3D _AddCollisions(Node3D _model, Node3D _ParentContainer = null)
		{
			try 
			{
				// Only add collisions to models
				if( _model.HasMeta("AsModel") == false ) 
				{
					return _model;
				}
				
				if( null == _GlobalExplorer ) 
				{
					return _model;
				}

				SceneTree Tree = _GlobalExplorer._Plugin.GetTree();
				
				if( Tree == null ) 
				{
					GD.PushWarning("Tree is not set");
				}
				
				AssetSnap.Front.Nodes.AsMeshInstance3D model = _model as AssetSnap.Front.Nodes.AsMeshInstance3D;
				
				AsStaticBody3D _Body = new()
				{
					MeshName = model.Name,
					Mesh = model.Mesh,
					Transform = model.Transform,
					InstanceTransform = model.Transform,
					InstanceScale = new Vector3(1, 1, 1),
					InstanceRotation = new Vector3(0, 0, 0),
					InstanceLibrary = model.GetLibraryName(),
					InstanceSpawnSettings = model.SpawnSettings,
				};

				if (SettingsStatic.ShouldPushToScene())
				{
					if( _ParentContainer != null ) 
					{
						Tree = _ParentContainer.GetTree();
						if( Tree != null ) 
						{
							_ParentContainer.AddChild(_Body, true);
							_Body.Owner = Tree.EditedSceneRoot;
						}
					}
					else 
					{
						if( Tree != null ) 
						{
							Tree.EditedSceneRoot.AddChild(_Body, true);
							_Body.Owner = Tree.EditedSceneRoot;
						}
						else 
						{
							GD.PushWarning("Tree not found");
						}
					}
				}
				else 
				{
					if( _ParentContainer != null ) 
					{
						_ParentContainer.AddChild(_Body);
					}
					else 
					{
						_GlobalExplorer._Plugin.AddChild(_Body);
					}
				}

				_Body.Initialize();
				model.QueueFree();
			
				return _Body.GetInstance();
			}
			catch(Exception e ) 
			{
				GD.PushWarning(e.Message);
			}
			
			return _model;
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