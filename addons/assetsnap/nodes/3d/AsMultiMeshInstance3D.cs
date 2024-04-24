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
	using AssetSnap.Nodes;
	using Godot;

	[Tool]
	public partial class AsMultiMeshInstance3D : MultiMeshInstance3D, ICollisionableModel
	{
		private bool _NoCollisions = false;
		private bool _ForceCollisions = false;

		[ExportCategory("Collisions")]

		[Export]
		public bool ForceCollisions { get => _ForceCollisions; set { _ForceCollisions = value; } }

		[Export]
		public bool NoCollisions { get => _NoCollisions; set { _NoCollisions = value; } }

		private ModelCollision Collision;

		public AsMultiMeshInstance3D()
		{
			SetMeta("AsModel", true);
			GD.Print("SET1");
			SetMeta("Collision", true);
		}

		public override void _EnterTree()
		{
			Collision = new();

			base._EnterTree();
		}

		public async override void _Ready()
		{
			base._Ready();
			Collision.RegisterCollisionType(this);
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

			if (null != GetChild(0))
			{
				foreach (Node child in GetChildren())
				{
					RemoveChild(child);
					child.QueueFree();
				}
			}

			GD.Print("her::", GetMeta("Collision").As<bool>());

			if (HasMeta("Collision") && GetMeta("Collision").As<bool>() == true)
			{
				if (null != Owner)
				{
					Collision.Render();
				}
			}
		}

		public ModelDriver.ModelTypes GetModelType()
		{
			return ModelDriver.ModelTypes.Simple;
		}

		public AsStaticBody3D GetCollisionBody()
		{
			Node child = GetChild(0);
			if (null != child && child is AsStaticBody3D body)
			{
				return body;
			}

			return null;
		}

		public bool HasCollisions()
		{
			return GetChildCount() != 0;
		}

		public void ApplyCollision(AsStaticBody3D body)
		{
			AddChild(body);
			body.Initialize();
		}

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
