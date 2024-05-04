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
		private Type _CollisionType = Type.Box;
		
		/// <summary>
		/// The type of convex collision.
		/// </summary>
		private ConvexType _ConvexCollisionType = ConvexType.None;
		
		/// <summary>
		/// Indicates whether the parent node has children.
		/// </summary>
		private State _HasChildren = State.False;

		/// <summary>
		/// The axis-aligned bounding box of the model.
		/// </summary>
		private Aabb _ModelAabb;

		/// <summary>
		/// The parent node.
		/// </summary>
		private Node3D _Parent;

		/// <summary>
		/// The collision body.
		/// </summary>
		private AsStaticBody3D _Body;

		/// <summary>
		/// Determines the type of collision to create for the parent node.
		/// </summary>
		/// <param name="node">The parent node.</param>
		/// <remarks>
		/// This method defines what type of collision will be created for the parent node.
		/// </remarks>
		public void RegisterCollisionType(Node3D node)
		{
			_Parent = node;
			_ModelAabb = NodeUtils.CalculateNodeAabb(_Parent);
			_CollisionType = _CalculateCollisionType();

			if (_CollisionType == Type.Convex)
			{
				_ConvexCollisionType = _CalculateConvexCollisionType();
			}
			else
			{
				_ConvexCollisionType = ConvexType.None;
			}

			if (_Parent is IDriverableModel driverableModel)
			{
				if (driverableModel.GetModelType() == ModelDriver.ModelTypes.Simple)
				{
					_HasChildren = State.False;
				}
				else if (driverableModel.GetModelType() == ModelDriver.ModelTypes.SceneBased)
				{
					_HasChildren = State.True;
				}
			}
		}

		/// <summary>
        /// Renders the collision for the model.
        /// </summary>
		public void Render()
		{
			if (_Parent is ICollisionableModel collisionableModel)
			{
				if (collisionableModel.HasCollisions())
				{
					// If collision already exist merely repair the connection to it
					_Body = collisionableModel.GetCollisionBody();
					return;
				}

				// Else we create the collisions and their connection
				if (_HasChildren == State.False)
				{
					_Body = new()
					{
						Name = "CollisionBody",
						Parent = _Parent,
						UsingMultiMesh = _Parent is AsMultiMeshInstance3D,
						ModelAabb = _ModelAabb,
						CollisionType = _CollisionType,
						CollisionSubType = _ConvexCollisionType
					};

					collisionableModel.ApplyCollision(_Body);
					collisionableModel.UpdateViewability();
				}
				else if (_HasChildren == State.True)
				{
					foreach (Node3D node in _Parent.GetChildren())
					{
						_Body = new()
						{
							Name = "CollisionBody",
							Parent = node,
							UsingMultiMesh = node is AsMultiMeshInstance3D,
							ModelAabb = NodeUtils.CalculateNodeAabb(node),
							CollisionType = _CollisionType,
							CollisionSubType = _ConvexCollisionType
						};
						node.AddChild(_Body);
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