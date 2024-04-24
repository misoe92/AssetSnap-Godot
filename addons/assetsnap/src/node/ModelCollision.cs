namespace AssetSnap.Nodes
{
	using AssetSnap.Front.Nodes;
	using AssetSnap.States;
	using Godot;

	public class ModelCollision
	{
		public enum State
		{
			False,
			True
		};

		public enum Type
		{
			Box,
			Sphere,
			Concave,
			Convex,
		};

		public enum ConvexType
		{
			None,
			Clean,
			Simplify,
			Both,
		};

		/*
		** Collision Options
		*/
		private Type CollisionType = Type.Box;
		private ConvexType ConvexCollisionType = ConvexType.None;
		private State HasChildren = State.False;

		private Aabb ModelAabb;

		private Node3D Parent;

		private AsStaticBody3D body;

		/*
		** Defines what type of collision which will be
		** created for the parent node.
		**
		** @param Node3D node
		** @return void
		*/
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

		/*
		** Calculates which collision type should
		** be used for the model collisions
		**
		** @return ModelCollision.Type
		*/
		private Type _CalculateCollisionType()
		{
			GD.Print(StatesUtils.Get().ConcaveCollision);
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

		/*
		** Calculates which Convex collision type should
		** be used for the model collisions
		**
		** @return ModelCollision.ConvexType
		*/
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