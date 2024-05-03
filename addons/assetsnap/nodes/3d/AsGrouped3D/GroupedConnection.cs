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

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// Partial class representing a grouped connection.
	/// </summary>
	[Tool]
	public partial class GroupedConnection
	{
		/// <summary>
		/// Gets or sets the instance ID.
		/// </summary>
		public int InstanceId { get; set; }
		
		/// <summary>
		/// Gets or sets the source of the grouped connection.
		/// </summary>
		public AsGrouped3D Source;
		
		/// <summary>
		/// Gets or sets the mesh of the instance.
		/// </summary>
		public Mesh InstanceMesh { get; set; }
		
		/// <summary>
		/// Updates the grouped connection using the given transform.
		/// </summary>
		/// <param name="transform">The transform to apply to the connection.</param>
		/// <returns>Void.</returns>
		public virtual void Update(Transform3D transform){}
		
		/// <summary>
        /// Updates the grouped connection using the given transform and options.
        /// </summary>
        /// <param name="transform">The transform to apply to the connection.</param>
        /// <param name="Options">The dictionary of options to apply.</param>
        /// <returns>Void.</returns>
		public virtual void UpdateUsing(Transform3D transform, Godot.Collections.Dictionary<string, Variant> Options){}
		
	}
}