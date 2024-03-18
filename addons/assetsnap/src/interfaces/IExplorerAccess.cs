namespace AssetSnap.Interfaces
{
	using Godot;

	public interface IExplorerAccess
	{
		EventMouse CurrentMouseInput();

		Node3D GetModel();
		Node3D GetHandle();

		float GetDeltaTime();
		bool HandleIsModel();
		bool HasModel();
		bool IsModelPlaced();
	}
}
