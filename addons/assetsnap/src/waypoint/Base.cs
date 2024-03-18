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
	using AssetSnap.Library;
	using Godot;
	
	public partial class Base : Node
	{
		private GlobalExplorer _GlobalExplorer;
		private Node _ParentContainer;
		private WaypointList _WaypointList;
		public float SnapDistance = 1.0f;
		
		public Node3D WorkingNode;
		
		public Base()
		{
			Name = "AssetSnapWaypoint";
		}
		
		public void Initialize()
		{
			_WaypointList = new();
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
			try 
			{
				if( ModelInstance.HasMeta("AsModel") ) 
				{
					_model.QueueFree();
					_model = ModelInstance as AssetSnap.Front.Nodes.AsMeshInstance3D;
					_model.Name = ModelInstance.Name;
				}
				else if ( ModelInstance.HasMeta("AsGroup") ) 
				{
					_model.QueueFree();
					_model = ModelInstance as AsGroup3D;
				}
				
				if( null == _model ) 
				{
					return;
				} 
				 
				if( _model is AssetSnap.Front.Nodes.AsMeshInstance3D meshInstance3D ) 
				{
					meshInstance3D.SetLibraryName( GlobalExplorer.GetInstance().CurrentLibrary.GetName() );
				}

				_SpawnNode( _model );
				_ConfigureNode( _model, Origin, Rotation, Scale );
				
				if( _ShouldAddCollision() && _model is AssetSnap.Front.Nodes.AsMeshInstance3D ) 
				{
					_model = _AddCollisions(_model);
				}
				
				// Focus the selected node in the editor
				// EditorInterface.Singleton.GetSelection().Clear();
				if( _GlobalExplorer.InputDriver.ShouldFocusAsset() && false == _GlobalExplorer.InputDriver.IsMulti ) 
				{
					_GlobalExplorer.Model = null;
					_GlobalExplorer.HandleNode = null;
					_GlobalExplorer.InputDriver.FocusAsset(_model);	
				}
			}
			catch( Exception e ) 
			{
				GD.PushError(e);
			}

			WorkingNode = _model;
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
			if( IsInstanceValid( ModelInstance ) ) 
			{
				_WaypointList.Remove(ModelInstance, Origin);
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
			_WaypointList.Add(node, Origin, Rot, Scale);
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
			_WaypointList.Update("Scale", Scale, Origin);
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
		** Returns snap coordinates based on a given
		** vector 3 coordinate set and a snap layer.
		**
		** @param Vector3 Coordinates
		** @param int Layer
		** @return Vector3
		*/
		public Vector3 Snap(Vector3 Coordinates, int Layer = 0)
		{
			if( false == _HasAnyWaypoints() ) 
			{
				return Vector3.Zero;
			}
			
			bool SnapToX = IsSnapX();
			bool SnapToZ = IsSnapZ();
			
			float ObjectSnapOffsetX = 0.0f;
			float ObjectSnapOffsetZ = 0.0f;
			
			if( HasObjectSnapOffsetX() ) 
			{
				ObjectSnapOffsetX = GetObjectSnapOffsetX();
			}
			
			if( HasObjectSnapOffsetZ() ) 
			{
				ObjectSnapOffsetZ = GetObjectSnapOffsetZ();
			}
			
			Vector3 snappedCoordinates = Coordinates;
			
			_WaypointList.Each(
				( BaseWaypoint Point ) =>
				{
					AsMeshInstance3D model = Point.GetModel() as AssetSnap.Front.Nodes.AsMeshInstance3D;
					Aabb AABB = Point.GetAabb();

					Vector3 MeshSize = AABB.Size * ( Point.GetScale() * new Vector3(0.75f, 0.75f, 0.75f));
					Vector3 spawnPointGlobal = Point.GetModel().GlobalTransform.Origin;

					int SnapLayer = model.GetSetting<int>("_LSSnapLayer.value"); 
					if( SnapLayer != Layer ) 
					{
						return;
					}
					
					if (!SnapToX &&
						(Coordinates.X > spawnPointGlobal.X - ( MeshSize.X / 2)) &&
						(Coordinates.X < spawnPointGlobal.X + ( MeshSize.X / 2)) &&
						(
							(Coordinates.Z > spawnPointGlobal.Z - ( MeshSize.Z / 2) - SnapDistance) &&
							(Coordinates.Z < spawnPointGlobal.Z + ( MeshSize.Z / 2) + SnapDistance)
						)
					)
					{
						if(Coordinates.Z - spawnPointGlobal.Z < (MeshSize.Z / 2)) 
						{
							snappedCoordinates.Z = spawnPointGlobal.Z - MeshSize.Z;
							if( ObjectSnapOffsetZ != 0 ) 
							{
								snappedCoordinates.Z -= ObjectSnapOffsetZ;
							}
						}
						else 
						{
							snappedCoordinates.Z = spawnPointGlobal.Z + MeshSize.Z;
							if( ObjectSnapOffsetZ != 0 ) 
							{
								snappedCoordinates.Z += ObjectSnapOffsetZ;
							}
						}
						snappedCoordinates.X = spawnPointGlobal.X;
						
						return;
					}

					// Check if the coordinates are within the snap distance on the X-axis
					if (!SnapToZ &&
						(Coordinates.Z > spawnPointGlobal.Z - ( MeshSize.Z / 2)) &&
						(Coordinates.Z < spawnPointGlobal.Z + ( MeshSize.Z / 2)) &&
						(
							(Coordinates.X > spawnPointGlobal.X - ( MeshSize.X / 2) - SnapDistance) &&
							(Coordinates.X < spawnPointGlobal.X + ( MeshSize.X / 2) + SnapDistance)
						)
					)
					{
						if(Coordinates.X - spawnPointGlobal.X < (MeshSize.X / 2)) 
						{
							snappedCoordinates.X = spawnPointGlobal.X - MeshSize.X;
							
							if( ObjectSnapOffsetX != 0 ) 
							{
								snappedCoordinates.X -= ObjectSnapOffsetX;
							}
						}
						else 
						{
							snappedCoordinates.X = spawnPointGlobal.X + MeshSize.X;
							
							if( ObjectSnapOffsetX != 0 ) 
							{
								snappedCoordinates.X += ObjectSnapOffsetX;
							}
						}
						
						snappedCoordinates.Z = spawnPointGlobal.Z;
						return;
					}
				}
			);
			
			return snappedCoordinates;
		}
		
		/*
		** Checks whether or not the model can
		** snap to another model
		**
		** @param Vector3 Coordinates
		** @param int Layer
		** @return bool
		*/
		public bool CanSnap(Vector3 Coordinates, int Layer = 0)
		{
			if( false == _HasAnyWaypoints() ) 
			{
				GD.Print("Houston, We got a problem.");
				return false;
			}

			bool Snapping = false;
			bool SnapToX = IsSnapX();
			bool SnapToZ = IsSnapZ();

			_WaypointList.Each(
				( BaseWaypoint Point ) =>
				{
					AsMeshInstance3D model = Point.GetModel() as AssetSnap.Front.Nodes.AsMeshInstance3D;
					Aabb AABB = Point.GetAabb();

					Vector3 MeshSize = AABB.Size * ( Point.GetScale() * new Vector3(0.75f, 0.75f, 0.75f));
					Vector3 spawnPointGlobal = Point.GetModel().GlobalTransform.Origin;

					int SnapLayer = model.GetSetting<int>("_LSSnapLayer.value");
					if( SnapLayer != Layer ) 
					{
						return;
					}
										
					// Check if the coordinates are within the snap distance on the X-axis
					if (!SnapToX &&
						(Coordinates.X > spawnPointGlobal.X - ( MeshSize.X / 2)) &&
						(Coordinates.X < spawnPointGlobal.X + ( MeshSize.X / 2)) &&
						(
							(Coordinates.Z > spawnPointGlobal.Z - ( MeshSize.Z / 2) - SnapDistance) &&
							(Coordinates.Z < spawnPointGlobal.Z + ( MeshSize.Z / 2) + SnapDistance)
						)
					)
					{
						Snapping = true;
						return;
					}

					// Check if the coordinates are within the snap distance on the X-axis
					if (!SnapToZ &&
						(Coordinates.Z > spawnPointGlobal.Z - ( MeshSize.Z / 2)) &&
						(Coordinates.Z < spawnPointGlobal.Z + ( MeshSize.Z / 2)) &&
						(
							(Coordinates.X > spawnPointGlobal.X - ( MeshSize.X / 2) - SnapDistance) &&
							(Coordinates.X < spawnPointGlobal.X + ( MeshSize.X / 2) + SnapDistance)
						)
					)
					{
						Snapping = true;
						return;
					}
				}
			);

			return Snapping;
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
			if (_ShouldPushToScene())
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

			if( _model is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D ) 
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
		private Node3D _AddCollisions(Node3D _model)
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

				if (_ShouldPushToScene())
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
		** Fetches the snap offset on the x axis
		**
		** @return float
		*/
		private float GetObjectSnapOffsetX()
		{			
			if( null == _GlobalExplorer ) 
			{
				return 0.0f;
			}

			Instance CurrentLibrary = _GlobalExplorer.CurrentLibrary;

			return CurrentLibrary._LibrarySettings._LSSnapOffsetX.GetValue();
		}
			
		/*
		** Fetches the snap offset on the z axis
		**
		** @return float
		*/
		private float GetObjectSnapOffsetZ()
		{		
			if( null == _GlobalExplorer ) 
			{
				return 0.0f;
			}

			Instance CurrentLibrary = _GlobalExplorer.CurrentLibrary;

			return CurrentLibrary._LibrarySettings._LSSnapOffsetZ.GetValue();
		}
		
		/*
		** Checks if object snap offset on the x axis
		** is enabled
		**
		** @return bool
		*/
		private bool HasObjectSnapOffsetX()
		{			
			if( null == _GlobalExplorer ) 
			{
				return false;
			}

			Instance CurrentLibrary = _GlobalExplorer.CurrentLibrary;
			
			if( 0.0f == CurrentLibrary._LibrarySettings._LSSnapOffsetX.GetValue() ) 
			{
				return false;
			}

			return true;
		}
		
		/*
		** Checks if object snap offset on the z axis
		** is enabled
		**
		** @return bool
		*/
		private bool HasObjectSnapOffsetZ()
		{	
			if( null == _GlobalExplorer ) 
			{
				return false;
			}

			Instance CurrentLibrary = _GlobalExplorer.CurrentLibrary;
			
			if( 0.0f == CurrentLibrary._LibrarySettings._LSSnapOffsetZ.GetValue() ) 
			{
				return false;
			}

			return true;
		}
		
		/*
		** Checks if any waypoints is available
		**
		** @return bool
		*/
		private bool _HasAnyWaypoints() 
		{
			return false == _WaypointList.IsEmpty();
		}
		
		/*
		** Checks if any snapping is enabled on the x
		** axis
		**
		** @return bool
		*/
		private bool IsSnapX()
		{
			if( null == _GlobalExplorer ) 
			{
				return false;
			}

			Instance CurrentLibrary = _GlobalExplorer.CurrentLibrary;
			
			if( false == CurrentLibrary._LibrarySettings._LSSnapToX.IsActive() ) 
			{
				return false;
			}

			return true;
		}
		
		/*
		** Checks if any snapping is enabled on the z
		** axis
		**
		** @return bool
		*/
		private bool IsSnapZ()
		{		
			if( null == _GlobalExplorer ) 
			{
				return false;
			}

			Instance CurrentLibrary = _GlobalExplorer.CurrentLibrary;
			
			if( false == CurrentLibrary._LibrarySettings._LSSnapToZ.IsActive() ) 
			{
				return false;
			}

			return true;
		}
		
		/*
		** Checks if collisions should be added
		**
		** @return bool
		*/
		private bool _ShouldAddCollision()
		{
			bool AddCollisions = _GlobalExplorer.Settings.GetKey("add_collisions").As<bool>();
			return AddCollisions;
		}
		
		/*
		** Checks if new children should be pushed to the current scene
		**
		** @return bool
		*/
		private bool _ShouldPushToScene()
		{
			bool PushToScene = _GlobalExplorer.Settings.GetKey("push_to_scene").As<bool>();
			return PushToScene;
		}
	}
}
#endif