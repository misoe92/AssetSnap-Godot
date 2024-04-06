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

[Tool]
public partial class OptimizedMultiMeshConnection : GroupedConnection
{
	public AsOptimizedMultiMeshGroup3D OptimizedMultiMesh { get; set; }
	
	/*
	** Will update based on the source object
	**
	** @return void
	*/
	public override void Update( Transform3D transform )
	{
		if( null == Source )
		{
			GD.PushWarning("No source was found to update through");
			return;
		}
		
		Godot.Collections.Dictionary<string, Variant> Options = new()
		{
			{ "ConcaveCollision", Source.ConcaveCollision ? Source.ConcaveCollision : false },
			{ "ConvexCollision", Source.ConvexCollision ? Source.ConvexCollision : false },
			{ "ConvexClean", Source.ConvexClean ? Source.ConvexClean : false },
			{ "ConvexSimplify", Source.ConvexSimplify ? Source.ConvexSimplify : false },
			{ "SphereCollision", Source.SphereCollision ? Source.SphereCollision : false },
		};

		OptimizedMultiMesh.UpdateBuffer(InstanceId, transform, Options);
	}
	
	/*
	** Updating using an collection of options
	**
	** @return void
	*/
	public override void UpdateUsing( Transform3D transform, Godot.Collections.Dictionary<string, Variant> Options )
	{
		OptimizedMultiMesh.UpdateBuffer(InstanceId, transform, Options);
	}
}