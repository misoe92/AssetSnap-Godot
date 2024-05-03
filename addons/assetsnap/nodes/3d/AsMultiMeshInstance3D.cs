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

using AssetSnap.Nodes;
using Godot;

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// Represents a 3D multi-mesh instance with collision capabilities.
	/// </summary>
	[Tool]
	public partial class AsMultiMeshInstance3D : MultiMeshInstance3D, ICollisionableModel
	{
		private bool _NoCollisions = false;
		private bool _ForceCollisions = false;
		private ModelCollision Collision;

		[ExportCategory("Collisions")]

		[Export]
		public bool ForceCollisions { get => _ForceCollisions; set { _ForceCollisions = value; } }
		[Export]
		public bool NoCollisions { get => _NoCollisions; set { _NoCollisions = value; } }

		/// <summary>
		/// Constructor for AsMultiMeshInstance3D class.
		/// </summary>
		public AsMultiMeshInstance3D()
		{
			SetMeta("AsModel", true);
			SetMeta("Collision", true);
		}

		/// <summary>
		/// Called when the node enters the scene tree.
		/// </summary>
		public override void _EnterTree()
		{
			Collision = new();

			base._EnterTree();
		}

		/// <summary>
		/// Called when the node is ready.
		/// </summary>
		public async override void _Ready()
		{
			base._Ready();
			Collision.RegisterCollisionType(this);
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

			if ( GetChildCount() > 0 )
			{
				foreach (Node child in GetChildren())
				{
					RemoveChild(child);
					child.QueueFree();
				}
			}

			if (HasMeta("Collision") && GetMeta("Collision").As<bool>() == true)
			{
				if (null != Owner)
				{
					Collision.Render();
				}
			}
		}

		/// <summary>
		/// Gets the model type of the multi-mesh instance.
		/// </summary>
		/// <returns>The model type as ModelTypes.</returns>
		public ModelDriver.ModelTypes GetModelType()
		{
			return ModelDriver.ModelTypes.Simple;
		}

		/// <summary>
		/// Gets the collision body associated with this multi-mesh instance.
		/// </summary>
		/// <returns>The collision body as AsStaticBody3D, or null if not found.</returns>
		public AsStaticBody3D GetCollisionBody()
		{
			Node child = GetChild(0);
			if (null != child && child is AsStaticBody3D body)
			{
				return body;
			}

			return null;
		}

		/// <summary>
		/// Checks if the multi-mesh instance has collision nodes.
		/// </summary>
		/// <returns>True if the multi-mesh instance has collision nodes, otherwise false.</returns>
		public bool HasCollisions()
		{
			return GetChildCount() != 0;
		}

		/// <summary>
		/// Applies collision to the multi-mesh instance.
		/// </summary>
		/// <param name="body">The collision body to apply.</param>
		public void ApplyCollision(AsStaticBody3D body)
		{
			AddChild(body);
			body.Initialize();
		}

		/// <summary>
        /// Updates the viewability of the multi-mesh instance.
        /// </summary>
        /// <param name="owner">The owner node.</param>
		public void UpdateViewability(Node owner = null)
		{
			if (null == GetParent())
			{
				GD.PushWarning("MeshInstance has not yet been placed");
				return;
			}

			if (owner == null)
			{
				owner = GetTree().EditedSceneRoot;
			}

			if (null != GetCollisionBody())
			{
				// Body
				GetCollisionBody().Owner = owner;

				// Shape
				if (GetCollisionBody().GetChildCount() > 0)
				{
					GetCollisionBody().GetChild(0).Owner = owner;
				}
				else
				{
					GD.PushWarning("No collision shape found @ AsMultiMeshInstance3D->UpdateViewability");
				}
			}
			else
			{
				GD.PushWarning("No collision body found @ AsMultiMeshInstance3D->UpdateViewability");
			}
		}
	}
}
