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

using Godot;
using Godot.Collections;

/// <summary>
/// Utility class for calculating combined Axis-Aligned Bounding Boxes (AABB).
/// </summary>
public static class AabbUtils
{
	/// <summary>
	/// Calculates the combined AABB of multiple meshes at specified origin coordinates.
	/// </summary>
	/// <param name="origins">Array of origin coordinates for each mesh.</param>
	/// <param name="meshes">Array of mesh instances.</param>
	/// <param name="rotations">Optional array of rotation vectors for each mesh.</param>
	/// <returns>The combined AABB of all meshes.</returns>
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

	/// <summary>
    /// Retrieves the AABB of a given mesh.
    /// </summary>
    /// <param name="mesh">The mesh instance.</param>
    /// <returns>The AABB of the mesh.</returns>
	private static Aabb GetMeshAABB(Mesh mesh)
	{
		return mesh.GetAabb();
	}
}