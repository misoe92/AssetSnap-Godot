using AssetSnap.Front.Nodes;
using Godot;

namespace AssetSnap.Nodes
{
	public interface ICollisionableModel
	{
		public void UpdateViewability(Node owner = null);
		public void ApplyCollision(AsStaticBody3D body);
		public bool HasCollisions();
		public AsStaticBody3D GetCollisionBody();
	}
}