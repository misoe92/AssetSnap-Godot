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

#if TOOLS

using AssetSnap.Front.Nodes;

namespace AssetSnap.Snap
{
	/// <summary>
	/// Defines the base class for snap boundaries.
	/// </summary>
	public class BaseSnapBoundary
	{
		/// <summary>
		/// Gets or sets the snap boundary node.
		/// </summary>
		public AsSnapBoundary Node 
		{
			get => _Node;
			set 
			{
				_Node = value;
			}
		}

		/// <summary>
		/// Gets or sets the visibility of the snap boundary.
		/// </summary>
		public bool Visible 
		{
			get => _Node.Visible;
			set 
			{
				_Node.Visible = value;
			}
		}
		
		private AsSnapBoundary _Node;
		
		/// <summary>
		/// Determines whether the snap boundary is active.
		/// </summary>
		/// <returns>True if the snap boundary is active, otherwise false.</returns>
		public virtual bool IsActive()
		{
			return false;	
		}
		
		/// <summary>
		/// Gets the snap boundary.
		/// </summary>
		/// <returns>The snap boundary.</returns>
		public AsSnapBoundary GetBoundary()
		{
			return Node;
		}
	}
}

#endif