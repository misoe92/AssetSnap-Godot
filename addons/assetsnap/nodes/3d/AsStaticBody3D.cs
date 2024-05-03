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

using System;
using AssetSnap.Explorer;
using Godot;

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// Custom StaticBody3D node used for collision handling in AssetSnap.
	/// </summary>
	[Tool]
	public partial class AsStaticBody3D : StaticBody3D
	{
		private bool IsModelPlaced { get; set; } = false;
		private Vector3 InstanceOrigin;

		[ExportGroup("Settings")]

		/// <summary>
		/// The parent node for this static body.
		/// </summary>
		[Export]
		public Node3D Parent { get; set; }

		/// <summary>
		/// The axis-aligned bounding box of the model.
		/// </summary>
		[Export]
		public Aabb ModelAabb { get; set; }

		/// <summary>
		/// The type of collision for the model.
		/// </summary>
		[Export]
		public AssetSnap.Nodes.ModelCollision.Type CollisionType { get; set; }

		/// <summary>
		/// The convex type of collision for the model.
		/// </summary>
		[Export]
		public AssetSnap.Nodes.ModelCollision.ConvexType CollisionSubType { get; set; } = AssetSnap.Nodes.ModelCollision.ConvexType.None;

		/// <summary>
		/// Indicates whether multi-mesh is being used.
		/// </summary>
		[Export]
		public bool UsingMultiMesh { get; set; } = false;


		/// <summary>
		/// Constructor for AsStaticBody3D class.
		/// </summary>
		public AsStaticBody3D()
		{
			SetMeta("AsBody", true);
		}

		/// <summary>
		/// Checks if the node has been placed in the scene.
		/// </summary>
		/// <returns>True if the node has been placed, false otherwise.</returns>
		public bool IsPlaced()
		{
			return GetParent() != null;
		}

		/// <summary>
		/// Initializes the static body node.
		/// </summary>
		public void Initialize()
		{
			try
			{
				if (null == ExplorerUtils.Get() || null == Plugin.Singleton)
				{
					return;
				}

				if (UsingMultiMesh)
				{
					AsMultiMeshInstance3D asMultiMeshInstance3D = Parent as AsMultiMeshInstance3D;
					for (int i = 0; i < asMultiMeshInstance3D.Multimesh.InstanceCount; i++)
					{
						InstanceOrigin = asMultiMeshInstance3D.Multimesh.GetInstanceTransform(i).Origin;
						_InitializeCollisionInstance();
					}
				}
				else
				{
					_InitializeCollisionInstance();
				}
			}
			catch (Exception e)
			{
				GD.PushWarning(e.Message);
			}
		}

		/// <summary>
		/// Updates the static body node.
		/// </summary>
		/// <param name="Type">The type of update.</param>
		/// <param name="Value">The value of the update.</param>
		public void Update(string Type, Vector3 Value)
		{
			if ("Scale" == Type)
			{
				Scale = Value;
			}
			else if ("Rotation" == Type)
			{
				RotationDegrees = Value;
			}
		}

		/// <summary>
		/// Updates the collision of the static body node.
		/// </summary>
		public void UpdateCollision()
		{
			// Remove current collisions and free them
			foreach (Node child in GetChildren())
			{
				RemoveChild(child);
				child.QueueFree();
			}

			// Initialize the collisions again
			Initialize();
		}

		/// <summary>
		/// Initializes the collision instance based on the specified collision type.
		/// </summary>
		private void _InitializeCollisionInstance()
		{
			SceneTree Tree = Plugin.Singleton.GetTree();
			switch (CollisionType)
			{
				case AssetSnap.Nodes.ModelCollision.Type.Box:
					_SimpleCollisions(Tree, false);
					break;

				case AssetSnap.Nodes.ModelCollision.Type.Sphere:
					_SimpleCollisions(Tree, true);
					break;

				case AssetSnap.Nodes.ModelCollision.Type.Concave:
					_ConcaveCollisions(Tree);
					break;

				case AssetSnap.Nodes.ModelCollision.Type.Convex:
					_ConvexCollisions(Tree);
					break;

				default:
					GD.PushWarning("Invalid Collision type");
					return;
			}
		}

		/// <summary>
		/// Creates simple collision shapes and adds them to the node's children.
		/// </summary>
		/// <param name="Tree">The SceneTree instance.</param>
		/// <param name="IsSphere">Specifies whether to create a sphere shape (default: false).</param>
		private void _SimpleCollisions(SceneTree Tree, bool IsSphere = false)
		{
			try
			{
				Aabb _aabb = ModelAabb;
				Shape3D _Shape = CreateSimpleShape(IsSphere);
				CollisionShape3D _Collision = new()
				{
					Name = "Collision",
					Shape = _Shape,
					// RotationDegrees = InstanceRotation,
				};

				_ApplyCollisionMeta(_Collision);

				AddChild(_Collision, true);
				_ApplyCollisionTransform(_Collision);
				_Collision.Owner = Tree.EditedSceneRoot;

			}
			catch (Exception e)
			{
				GD.PushWarning(e.Message);
			}
		}

		/// <summary>
		/// Creates simple collision shapes.
		/// </summary>
		/// <param name="IsSphere">Specifies whether to create a sphere shape.</param>
		/// <returns>The created shape.</returns>
		private Shape3D CreateSimpleShape(bool IsSphere)
		{
			Shape3D _Shape = null;

			if (false == UsingMultiMesh)
			{
				Aabb aabb = ((AsMeshInstance3D)Parent).Mesh.GetAabb();

				_Shape = new BoxShape3D()
				{
					Size = new Vector3(aabb.Size.X, aabb.Size.Y, aabb.Size.Z),
				};

				if (IsSphere)
				{
					float radius = MathF.Max(Math.Max(aabb.Size.X, aabb.Size.Y), aabb.Size.Z) * 0.5f;
					_Shape = new SphereShape3D()
					{
						Radius = radius,
					};
				}
			}

			if (UsingMultiMesh)
			{
				Aabb aabb = ((AsMultiMeshInstance3D)Parent).Multimesh.Mesh.GetAabb();

				_Shape = new BoxShape3D()
				{
					Size = new Vector3(aabb.Size.X, aabb.Size.Y, aabb.Size.Z),
				};

				if (IsSphere)
				{
					float radius = MathF.Max(Math.Max(aabb.Size.X, aabb.Size.Y), aabb.Size.Z) * 0.5f;
					_Shape = new SphereShape3D()
					{
						Radius = radius,
					};
				}
			}

			return _Shape;
		}

		/// <summary>
		/// Creates convex collision shapes and adds them to the node's children.
		/// </summary>
		/// <param name="Tree">The SceneTree instance.</param>
		private void _ConvexCollisions(SceneTree Tree)
		{
			ConvexPolygonShape3D _Shape = new();

			if (false == UsingMultiMesh)
			{
				_Shape = ((MeshInstance3D)Parent).Mesh.CreateConvexShape(
					_IsClean(
						(int)CollisionSubType
					),
					_IsSimplify(
						(int)CollisionSubType
					)
				);
			}

			if (UsingMultiMesh)
			{
				_Shape = ((MultiMeshInstance3D)Parent).Multimesh.Mesh.CreateConvexShape(
					_IsClean(
						(int)CollisionSubType
					),
					_IsSimplify(
						(int)CollisionSubType
					)
				);
			}

			CollisionShape3D _Collision = new()
			{
				Name = "Collision",
				Shape = _Shape,
				// Scale = InstanceScale,
				// RotationDegrees = InstanceRotation,
			};

			_ApplyCollisionMeta(_Collision);

			Transform3D ColTrans = _Collision.Transform;

			if (UsingMultiMesh)
			{
				ColTrans.Origin.X = InstanceOrigin.X;
				ColTrans.Origin.Y = InstanceOrigin.Y;
				ColTrans.Origin.Z = InstanceOrigin.Z;
			}

			_Collision.Transform = ColTrans;

			AddChild(_Collision, true);
			_Collision.Owner = Tree.EditedSceneRoot;
		}

		/// <summary>
		/// Checks if the given state corresponds to a clean condition.
		/// </summary>
		/// <param name="state">The state value.</param>
		/// <returns>True if the state indicates a clean condition, false otherwise.</returns>
		private bool _IsClean(int state)
		{
			return state == 1 || state == 3;
		}

		/// <summary>
		/// Checks if the given state corresponds to a simplified condition.
		/// </summary>
		/// <param name="state">The state value.</param>
		/// <returns>True if the state indicates a simplified condition, false otherwise.</returns>
		private bool _IsSimplify(int state)
		{
			return state == 2 || state == 3;
		}

		/// <summary>
		/// Adds concave collision shapes to the node's children.
		/// </summary>
		/// <param name="Tree">The SceneTree instance.</param>
		private void _ConcaveCollisions(SceneTree Tree)
		{
			// We merely need to add it to our main node
			// since we have no children
			CollisionShape3D _Collision = CreateConcaveCollision();

			AddChild(_Collision, true);
			_Collision.Owner = Tree.EditedSceneRoot;

		}

		/// <summary>
		/// Creates a concave collision shape.
		/// </summary>
		/// <returns>The concave collision shape.</returns>
		private CollisionShape3D CreateConcaveCollision()
		{
			ConcavePolygonShape3D _Shape = new();

			if (false == UsingMultiMesh)
			{
				_Shape = ((MeshInstance3D)Parent).Mesh.CreateTrimeshShape();
			}

			if (UsingMultiMesh)
			{
				_Shape = ((MultiMeshInstance3D)Parent).Multimesh.Mesh.CreateTrimeshShape();
			}

			CollisionShape3D _Collision = new()
			{
				Name = "Collision",
				Shape = _Shape,
				// Scale = InstanceScale,
				// RotationDegrees = InstanceRotation,
			};

			_ApplyCollisionMeta(_Collision);

			Transform3D ColTrans = _Collision.Transform;

			if (UsingMultiMesh)
			{
				ColTrans.Origin.X = InstanceOrigin.X;
				ColTrans.Origin.Y = InstanceOrigin.Y;
				ColTrans.Origin.Z = InstanceOrigin.Z;
			}

			_Collision.Transform = ColTrans;

			return _Collision;
		}

		/// <summary>
		/// Applies transformation to the collision shape.
		/// </summary>
		/// <param name="_Collision">The collision shape to transform.</param>
		private void _ApplyCollisionTransform(CollisionShape3D _Collision)
		{
			Aabb aabb = new();
			if (!UsingMultiMesh)
			{
				aabb = ((AsMeshInstance3D)Parent).Mesh.GetAabb();
			}

			if (UsingMultiMesh)
			{
				aabb = ((AsMultiMeshInstance3D)Parent).Multimesh.Mesh.GetAabb();
			}

			Transform3D ColTrans = _Collision.Transform;

			if (UsingMultiMesh)
			{
				ColTrans.Origin.X = InstanceOrigin.X;
				ColTrans.Origin.Y = InstanceOrigin.Y;
				ColTrans.Origin.Z = InstanceOrigin.Z;
			}

			ColTrans.Origin.Y += aabb.Size.Y / 2;
			_Collision.Transform = ColTrans;
		}
		
		/// <summary>
		/// Applies meta information to the collision shape.
		/// </summary>
		/// <param name="_Collision">The collision shape to apply meta information.</param>
		private void _ApplyCollisionMeta(CollisionShape3D _Collision)
		{
			_Collision.SetMeta("AsCollision", true);
		}

		/// <summary>
		/// Called when the node is about to be removed from the scene tree.
		/// </summary>
		public override void _ExitTree()
		{
			base._ExitTree();
		}
	}
}
