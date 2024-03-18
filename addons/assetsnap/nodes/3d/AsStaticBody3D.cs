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

namespace AssetSnap.Front.Nodes
{
	using System;
	using AssetSnap.Front.Components;
	using Godot;

	[Tool]
	public partial class AsStaticBody3D : StaticBody3D
	{
		private bool IsModelPlaced { get; set; } = false;

		[ExportGroup("Settings")]
		[ExportSubgroup("Mesh")]
		[Export]
		public string InstanceLibrary { get; set; }
	
		[Export]
		public string MeshName { get; set; }
		
		[Export]
		public Mesh Mesh { get; set; }
		
		[Export]
		public Transform3D InstanceTransform { get; set; }
		
		[Export]
		public Vector3 InstanceRotation { get; set; }
		
		[Export]
		public Vector3 InstanceScale { get; set; }
		
		[Export]
		public Godot.Collections.Dictionary<string, Variant> InstanceSpawnSettings { get; set; }

		[Export]
		public bool UsingMultiMesh { get; set; } = false;

		public AsStaticBody3D()
		{
			SetMeta("AsBody", true);
		}
		
		public bool IsPlaced()
		{
			return GetParent() != null;
		}

		public void Initialize(int TypeState = 0, int ArgState = 0)
		{
			try 
			{
				GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();

				if( _GlobalExplorer == null ) 
				{
					return;
				}
				
				if( null == Mesh ) 
				{
					throw new Exception("No mesh available");
				}
				
				SceneTree Tree = _GlobalExplorer._Plugin.GetTree();
				
				if( false == UsingMultiMesh ) 
				{
					Transform3D Trans = InstanceTransform;
					Trans.Origin = new Vector3(0.0f, 0.0f, 0.0f);

					AsMeshInstance3D _Instance = new()
					{
						Name = MeshName,
						LibraryName = InstanceLibrary,
						Transform = Trans,
						Mesh = Mesh,
						Scale = InstanceScale,
						RotationDegrees = InstanceRotation,
						SpawnSettings = InstanceSpawnSettings,
					};
	
					AddChild(_Instance);
					_Instance.Owner = Tree.EditedSceneRoot;
				}

				var CurrentLibrary = _GlobalExplorer.GetLibraryByName(InstanceLibrary);
				if( CurrentLibrary != null ) 
				{
					LSConvexPolygonCollision ConvexComponent = CurrentLibrary._LibrarySettings._LSConvexPolygonCollision;
					bool IsConvexCollision = ConvexComponent.IsActive();
					
					LSConcaveCollision ConcaveComponent = CurrentLibrary._LibrarySettings._LSConcaveCollision;
					bool IsConcaveCollision = ConcaveComponent.IsActive();
					
					if( IsConvexCollision || TypeState == 1) 
					{
						int state = 0;
						
						if( ConvexComponent.ShouldClean() && false == ConvexComponent.ShouldSimplify() || ArgState == 1) 
						{
							state = 1;
						} 
						
						if( false == ConvexComponent.ShouldClean() && ConvexComponent.ShouldSimplify() || ArgState == 2 ) 
						{
							state = 2;
						} 
						
						if(ConvexComponent.ShouldClean() && ConvexComponent.ShouldSimplify() || ArgState == 3 ) 
						{
							state = 3;
						} 
						
						_ConvexCollisions(Mesh, this, Tree, state);	
					}
					else if( IsConcaveCollision || TypeState == 2 ) 
					{
						_ConcaveCollisions(Mesh, this, Tree);	
					}
					else 
					{
						_SimpleCollisions(Mesh, this, Tree, TypeState == 3 );	
					}
				}
			}
			catch(Exception e ) 
			{
				GD.PushWarning(e.Message);
			}
		}
		
		public AsMeshInstance3D GetInstance() 
		{
			return GetChild(0) as AsMeshInstance3D;
		}
		
		public void Update( string Type, Vector3 Value )
		{
			if( "Scale" == Type ) 
			{
				Scale = Value;
			}
			else if( "Rotation" == Type ) 
			{
				RotationDegrees = Value;
			}
		}
		 
		public void UpdateCollision(int TypeState = 0, int ArgState = 0)
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();

			if( _GlobalExplorer == null ) 
			{
				return;
			}
			
			if( HasNode("AsCollision") ) 
			{
				CollisionShape3D OldCol = GetNode("AsCollision") as CollisionShape3D;
				
				if( IsInstanceValid( OldCol ) ) 
				{
					RemoveChild(OldCol);
					OldCol.QueueFree();
				}
			}
 
			SceneTree Tree = _GlobalExplorer._Plugin.GetTree();
			var CurrentLibrary = _GlobalExplorer.GetLibraryByName(InstanceLibrary);
			if( CurrentLibrary != null ) 
			{
				LSConvexPolygonCollision ConvexComponent = CurrentLibrary._LibrarySettings._LSConvexPolygonCollision;
				bool IsConvexCollision = ConvexComponent.IsActive();
				
				LSConcaveCollision ConcaveComponent = CurrentLibrary._LibrarySettings._LSConcaveCollision;
				bool IsConcaveCollision = ConcaveComponent.IsActive();
				
				if( IsConvexCollision || TypeState == 1) 
				{
					int state = 0;
					
					if( ConvexComponent.ShouldClean() && false == ConvexComponent.ShouldSimplify() || ArgState == 1 ) 
					{
						state = 1;
					} 
					
					if( false == ConvexComponent.ShouldClean() && ConvexComponent.ShouldSimplify() || ArgState == 2 ) 
					{
						state = 2;
					} 
					
					if(ConvexComponent.ShouldClean() && ConvexComponent.ShouldSimplify() || ArgState == 3 ) 
					{
						state = 3;
					} 
					
					_ConvexCollisions(Mesh, this, Tree, state);	
				}
				else if( IsConcaveCollision || TypeState == 2 ) 
				{
					_ConcaveCollisions(Mesh, this, Tree);	
				}
				else 
				{
					_SimpleCollisions(Mesh, this, Tree, TypeState == 3 );	
				}
			}
		}
			
		private void _SimpleCollisions(Mesh model, StaticBody3D _Body, SceneTree Tree, bool IsSphere = false )
		{
			try 
			{
				GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();

				if( _GlobalExplorer == null ) 
				{
					throw new Exception("GlobalExplorer not set");
				}
				
				Aabb _aabb = model.GetAabb();
			
				var CurrentLibrary = _GlobalExplorer.GetLibraryByName(InstanceLibrary);
				LSSimpleSphereCollision ConcaveComponent = CurrentLibrary._LibrarySettings._LSSimpleSphereCollision;
				
				if( IsSphere == false ) 
				{
					IsSphere = ConcaveComponent.IsActive();
				}
				
				Shape3D _Shape = new BoxShape3D()
				{
					Size = new Vector3(_aabb.Size.X * InstanceScale.X, _aabb.Size.Y * InstanceScale.Y, _aabb.Size.Z * InstanceScale.Z),
				};
			
				if( IsSphere ) 
				{
					float radius = MathF.Max(Math.Max(_aabb.Size.X * InstanceScale.X, _aabb.Size.Y * InstanceScale.Y), _aabb.Size.Z * InstanceScale.Z) * 0.5f;
					_Shape = new SphereShape3D()
					{
						Radius = radius,
					};
				}
				
				CollisionShape3D _Collision = new()
				{
					Name = "AsCollision",
					Shape = _Shape,
					RotationDegrees = InstanceRotation,
				};

				Transform3D ColTrans = _Collision.Transform;
				ColTrans.Origin.Y += _aabb.Size.Y * InstanceScale.Y / 2;
				_Collision.Transform = ColTrans;

				_Collision.SetMeta("AsCollision", true);
					
				_Body.AddChild(_Collision, true);
				_Collision.Owner = Tree.EditedSceneRoot;
			}
			catch( Exception e ) 
			{
				GD.PushWarning(e.Message);
			}
		}
		
		private void _ConvexCollisions( Mesh model, StaticBody3D _Body, SceneTree Tree, int state = 0) 
		{
			Aabb _aabb = model.GetAabb();
		
			bool clean = false;
			bool simplify = false;
			
			if( state == 1 ) 
			{
				clean = true;
			}
			
			if( state == 2 ) 
			{
				simplify = true;
			}
			
			if( state == 3 ) 
			{
				clean = true;
				simplify = true;
			}
			
			ConvexPolygonShape3D _Shape = model.CreateConvexShape(clean, simplify);
			CollisionShape3D _Collision = new()
			{
				Name = "AsCollision",
				Shape = _Shape,
				Scale = InstanceScale,
				RotationDegrees = InstanceRotation,
			};
			
			Transform3D ColTrans = _Collision.Transform;
			// ColTrans.Origin.Y += _aabb.Size.Y / 2;
			_Collision.Transform = ColTrans;

			_Collision.SetMeta("AsCollision", true);
	
			_Body.AddChild(_Collision, true);
			_Collision.Owner = Tree.EditedSceneRoot;
		}
		
		private void _ConcaveCollisions(Mesh model, StaticBody3D _Body, SceneTree Tree)
		{
			ConcavePolygonShape3D _Shape = model.CreateTrimeshShape();
			CollisionShape3D _Collision = new()
			{
				Name = "AsCollision",
				Shape = _Shape,
				Scale = InstanceScale,
				RotationDegrees = InstanceRotation,
			};
			
			// Transform3D ColTrans = _Collision.Transform;
			// ColTrans.Origin.Y += _aabb.Size.Y / 2; 
			// _Collision.Transform = ColTrans;

			_Collision.SetMeta("AsCollision", true);
				
			_Body.AddChild(_Collision, true);
			_Collision.Owner = Tree.EditedSceneRoot;
		}

		public override void _ExitTree()
		{
			base._ExitTree();
		}
	}
}
