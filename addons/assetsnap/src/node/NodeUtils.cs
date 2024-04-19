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

namespace AssetSnap.Nodes
{
	public static class NodeUtils
	{
		public static Aabb CalculateNodeAabb( Node node ) 
		{
			Aabb aabb = new Aabb();

			if( node is Node3D node3d && false == node3d.Visible) 
			{
				return aabb;
			}
			
			else if( node is MeshInstance3D meshInstance3D ) 
			{
				aabb = meshInstance3D.GlobalTransform * meshInstance3D.GetAabb();
			}
			
			for( int i = 0; i < node.GetChildCount(); i++ ) 
			{
				aabb = aabb.Merge(CalculateNodeAabb(node.GetChild(i)));
			}

			return aabb;
		}
	}
}