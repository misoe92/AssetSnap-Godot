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

namespace AssetSnap.Front.Nodes
{
	using Godot;

	[Tool]
	public partial class AsOptimizedMultiMeshGroup3D : Node3D
	{

		bool disposed = false;
		bool Initiated = false;
		
		[Export]
		public int CurrentItemCount { get; set; } = 0;
		
		[ExportCategory("Mesh")]
		/*
		** The object that will be spawned in this container
		*/
		[Export]
		public Mesh Object
		{
			get => _Object;
			set 
			{
				_Object = value;
				if( Initiated ) 
				{
					_Update();
				}
			}
		}
		
		[ExportCategory("MultiMesh Configuration")]
		/*
		** Chunk size of our child multi meshes.
		** 
		** Too many entries per chunk might cause issues
		*/
		[Export]
		public int ChunkSize
		{
			get => _ChunkSize;
			set 
			{
				_ChunkSize = value;
				if( Initiated ) 
				{
					_Update();
				}
			}
		}

		
		[Export]
		public Godot.Collections.Array<Transform3D> TransformBuffer
		{
			get => _TransformBuffer;
			set 
			{
				_TransformBuffer = value;
				if( Initiated ) 
				{
					_Update();
				}
			}
		}

		private Mesh _Object;
		private int _ChunkSize = 20;
		private Godot.Collections.Array<Transform3D> _TransformBuffer = new();

		public override void _EnterTree()
		{
			if( null != GetParent() && IsInstanceValid( GetParent() ) ) 
			{
				GlobalExplorer.GetInstance().States.OptimizedGroups.Add(_Object, this);
			}
			
			ClearChildren();
			Update();
			
			Initiated = true;
		}
		
		public int AddToBuffer(Transform3D transform ) 
		{
			_TransformBuffer.Add(transform);
			Update();

			return _TransformBuffer.Count - 1;
		}
		
		public void UpdateBuffer( int InstanceId, Transform3D transform, Godot.Collections.Dictionary<string, Variant> Options)
		{
			_TransformBuffer[InstanceId] = transform;
			Update();
		}
		
		public void Update()
		{
			_Update();
		}
		
		private void _ClearBuffer()
		{
			_TransformBuffer.Clear();
		}
		
		private void _Update()
		{
			if( disposed ) 
			{
				return;
			}
			// Clear the current children of the node
			ClearChildren();
		
			if( _TransformBuffer.Count == 0 ) 
			{
				return;
			}

			CurrentItemCount = _TransformBuffer.Count;

			// Create MultiMeshInstances with chunk size and transform buffer
			int numChunks = Mathf.Max(1, Mathf.CeilToInt((float)_TransformBuffer.Count / ChunkSize));
			for (int i = 0; i < numChunks; i++)
			{
				// Create a MultiMeshInstance for each chunk
				MultiMeshInstance3D multiMeshInstance = new MultiMeshInstance3D()
				{
					Name = "Chunk-" + ( i + 1)
				};
				AddChild(multiMeshInstance); // Add the MultiMeshInstance as a child of this node
				multiMeshInstance.Owner = Owner;
				
				// Set the mesh and transform data for the MultiMeshInstance
				MultiMesh multiMesh = new MultiMesh()
				{
					TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
					Mesh = _Object,
					InstanceCount = Mathf.Min(ChunkSize, _TransformBuffer.Count - i * ChunkSize)
				};

				for (int j = 0; j < multiMesh.InstanceCount; j++)
				{
					// Set the transform data for each instance
					multiMesh.SetInstanceTransform(j, _TransformBuffer[i * ChunkSize + j]);
				}

				// Assign the MultiMesh to the MultiMeshInstance
				multiMeshInstance.Multimesh = multiMesh;
			}
		}
		
		private void ClearChildren()
		{
			foreach (Node child in GetChildren())
			{
				if( IsInstanceValid(child)) 
				{
					RemoveChild(child);
					child.QueueFree(); // Free the child node
				}
			}
		}

		public override void _ExitTree()
		{
			GlobalExplorer.GetInstance().States.OptimizedGroups.Remove(_Object);
			disposed = true;
			ClearChildren();
			base._ExitTree();
		}
	}
}
