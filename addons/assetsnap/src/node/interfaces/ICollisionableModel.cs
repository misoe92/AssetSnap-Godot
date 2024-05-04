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
using Godot;

namespace AssetSnap.Nodes
{
	/// <summary>
	/// Interface for collisionable models.
	/// </summary>
	public interface ICollisionableModel
	{
		/// <summary>
		/// Updates the viewability of the model.
		/// </summary>
		/// <param name="owner">The owner node.</param>
		public void UpdateViewability(Node owner = null);
		
		/// <summary>
		/// Applies collision to the model.
		/// </summary>
		/// <param name="body">The static body for collision.</param>
		public void ApplyCollision(AsStaticBody3D body);
			
		/// <summary>
		/// Gets the collision body of the model.
		/// </summary>
		/// <returns>The collision body as <see cref="AsStaticBody3D"/>.</returns>
		public AsStaticBody3D GetCollisionBody();
		
		/// <summary>
		/// Checks if the model has collisions.
		/// </summary>
		/// <returns>True if the model has collisions, false otherwise.</returns>
		public bool HasCollisions();
	}
}