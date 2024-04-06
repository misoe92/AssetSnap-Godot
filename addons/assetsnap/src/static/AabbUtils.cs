using Godot;
using Godot.Collections;

public static class AabbUtils
{
	public static Aabb CalculateCombinedAABB(Array<Vector3> origins, Array<Mesh> meshes, Array<Vector3> rotations = null)
	{
		Aabb combinedAABB = new Aabb();

		// Iterate through each origin coordinate
		for (int i = 0; i < origins.Count; i++)
		{
			// Get the Aabb of the mesh at the current index
			Aabb meshAABB = GetMeshAABB(meshes[i]);

			// Apply rotation if provided
			// if (rotations != null && rotations.Count > i)
			// {
			// 	Basis rotationBasis = new Basis();
			// 	rotationBasis.SetEulerXYZ(rotations[i]);
			// 	meshAABB = meshAABB.Transformed(Transform.Identity.Rotated(rotationBasis));
			// }

			// Offset the Aabb to match the origin coordinate
			meshAABB.Position += origins[i];

			// Expand the combined Aabb to include the mesh Aabb
			combinedAABB = combinedAABB.Merge(meshAABB);
		}

		return combinedAABB;
	}

	private static Aabb GetMeshAABB(Mesh mesh)
	{
		// This function retrieves the Aabb of a given mesh
		// You may need to implement this based on your specific mesh setup
		// For demonstration purposes, let's assume a basic Aabb computation

		// Example implementation:
		// Aabb meshAABB = mesh.GetAABB();

		// For demonstration purposes, returning an empty Aabb
		return mesh.GetAabb();
	}
}