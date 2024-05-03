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

using AssetSnap.Front.Nodes;
using AssetSnap.States;
using Godot;

namespace AssetSnap.Nodes
{
	/// <summary>
	/// Represents a class responsible for managing collision generation for models.
	/// </summary>
	[Tool]
	public class ModelCollision
	{
		/// <summary>
		/// Defines the possible states of the collision generation.
		/// </summary>
		public enum State
		{
			False,
			True
		};

		/// <summary>
		/// Defines the possible types of collisions.
		/// </summary>
		public enum Type
		{
			Box,
			Sphere,
			Concave,
			Convex,
		};

		/// <summary>
		/// Defines the possible types of convex collisions.
		/// </summary>
		public enum ConvexType
		{
			None,
			Clean,
			Simplify,
			Both,
		};

		/// <summary>
		/// The type of collision.
		/// </summary>
		private Type CollisionType = Type.Box;
		
		/// <summary>
		/// The type of convex collision.
		/// </summary>
		private ConvexType ConvexCollisionType = ConvexType.None;
		
		/// <summary>
		/// Indicates whether the parent node has children.
		/// </summary>
		private State HasChildren = State.False;

		/// <summary>
		/// The axis-aligned bounding box of the model.
		/// </summary>
		private Aabb ModelAabb;

		/// <summary>
		/// The parent node.
		/// </summary>
		private Node3D Parent;

		/// <summary>
		/// The collision body.
		/// </summary>
		private AsStaticBody3D body;

		/// <summary>
		/// Determines the type of collision to create for the parent node.
		/// </summary>
		/// <param name="node">The parent node.</param>
		/// <remarks>
		/// This method defines what type of collision will be created for the parent node.
		/// </remarks>
		public void RegisterCollisionType(Node3D node)
		{
			Parent = node;
			ModelAabb = NodeUtils.CalculateNodeAabb(Parent);
			CollisionType = _CalculateCollisionType();

			if (CollisionType == Type.Convex)
			{
				ConvexCollisionType = _CalculateConvexCollisionType();
			}
			else
			{
				ConvexCollisionType = ConvexType.None;
			}

			if (Parent is IDriverableModel driverableModel)
			{
				if (driverableModel.GetModelType() == ModelDriver.ModelTypes.Simple)
				{
					HasChildren = State.False;
				}
				else if (driverableModel.GetModelType() == ModelDriver.ModelTypes.SceneBased)
				{
					HasChildren = State.True;
				}
			}
		}

		/// <summary>
        /// Renders the collision for the model.
        /// </summary>
		public void Render()
		{
			if (Parent is ICollisionableModel collisionableModel)
			{
				if (collisionableModel.HasCollisions())
				{
					// If collision already exist merely repair the connection to it
					body = collisionableModel.GetCollisionBody();
					return;
				}

				// Else we create the collisions and their connection
				if (HasChildren == State.False)
				{
					body = new()
					{
						Name = "CollisionBody",
						Parent = Parent,
						UsingMultiMesh = Parent is AsMultiMeshInstance3D,
						ModelAabb = ModelAabb,
						CollisionType = CollisionType,
						CollisionSubType = ConvexCollisionType
					};

					collisionableModel.ApplyCollision(body);
					collisionableModel.UpdateViewability();
				}
				else if (HasChildren == State.True)
				{
					foreach (Node3D node in Parent.GetChildren())
					{
						body = new()
						{
							Name = "CollisionBody",
							Parent = node,
							UsingMultiMesh = node is AsMultiMeshInstance3D,
							ModelAabb = NodeUtils.CalculateNodeAabb(node),
							CollisionType = CollisionType,
							CollisionSubType = ConvexCollisionType
						};
						node.AddChild(body);
					}
				}
			}
		}

		/// <summary>
        /// Calculates the type of collision to use for the model.
        /// </summary>
        /// <returns>The type of collision.</returns>
		private Type _CalculateCollisionType()
		{
			if (StatesUtils.Get().ConvexCollision == GlobalStates.LibraryStateEnum.Enabled)
			{
				return Type.Convex;
			}
			else if (StatesUtils.Get().ConcaveCollision == GlobalStates.LibraryStateEnum.Enabled)
			{
				return Type.Concave;
			}
			else if (StatesUtils.Get().SphereCollision == GlobalStates.LibraryStateEnum.Enabled)
			{
				return Type.Sphere;
			}

			return Type.Box;
		}

		/// <summary>
        /// Calculates the type of convex collision to use for the model.
        /// </summary>
        /// <returns>The type of convex collision.</returns>
		private ConvexType _CalculateConvexCollisionType()
		{
			GlobalStates states = StatesUtils.Get();

			if (states.ConvexClean == GlobalStates.LibraryStateEnum.Enabled && states.ConvexSimplify == GlobalStates.LibraryStateEnum.Enabled)
			{
				return ConvexType.Both;
			}
			else if (states.ConvexSimplify == GlobalStates.LibraryStateEnum.Enabled)
			{
				return ConvexType.Simplify;
			}
			else if (states.ConvexClean == GlobalStates.LibraryStateEnum.Enabled)
			{
				return ConvexType.Clean;
			}

			return ConvexType.None;
		}
	}
}