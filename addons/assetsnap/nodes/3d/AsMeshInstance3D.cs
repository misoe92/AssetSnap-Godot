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
using AssetSnap.ASNode.MeshInstance;
using AssetSnap.Nodes;
using AssetSnap.Static;

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// Represents a 3D mesh instance with collision capabilities.
	/// </summary>
	[Tool]
	public partial class AsMeshInstance3D : Base, ICollisionableModel
	{
		private float _FadeDuration = SettingsStatic.TransparencyFadeDuration(); // Fade duration in seconds
		private float _FadeTimer = 0.0f;
		private ModelCollision _Collision;

		/// <summary>
		/// Constructor for AsMeshInstance3D class.
		/// </summary>
		public AsMeshInstance3D()
		{
			SetMeta("AsModel", true);
			SetMeta("Collision", true);
		}

		/// <summary>
		/// Called when the node enters the scene tree.
		/// </summary>
		public override void _EnterTree()
		{
			if (SettingsStatic.ModelTransparencyActive())
			{
				Transparency = 1 - SettingsStatic.TransparencyLevel();
			}

			_Collision = new();

			base._EnterTree();
		}

		/// <summary>
		/// Called when the node is ready.
		/// </summary>
		public async override void _Ready()
		{
			base._Ready();
			_Collision.RegisterCollisionType(this);

			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

			if (GetChildCount() > 0)
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
					_Collision.Render();
				}
			}
		}

		/// <summary>
		/// Called every frame.
		/// </summary>
		public override void _Process(double delta)
		{
			if (
				_FadeTimer != _FadeDuration &&
				IsPlaced() &&
				SettingsStatic.ModelTransparencyActive()
			)
			{
				// Increment the timer
				_FadeTimer += (float)delta;

				// Calculate the alpha value based on the timer
				float alpha = Mathf.Clamp(_FadeTimer / _FadeDuration, 0.25f, 1.0f);

				// Update the alpha property of the shader material
				Transparency = 1 - alpha;

				// Check if the fade-in is complete
				if (_FadeTimer >= _FadeDuration)
				{
					// Reset the timer or stop the fade-in effect
					_FadeTimer = _FadeDuration;
				}
			}

			base._Process(delta);
		}

		/// <summary>
		/// Applies collision to the mesh instance.
		/// </summary>
		/// <param name="body">The collision body to apply.</param>
		public void ApplyCollision(AsStaticBody3D body)
		{
			AddChild(body);
			body.Initialize();
		}

		/// <summary>
		/// Updates the viewability of the mesh instance.
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

			// Body
			GetCollisionBody().Owner = owner;
			// Shape
			GetCollisionBody().GetChild(0).Owner = owner;
		}
		
		/// <summary>
		/// Gets the collision body associated with this mesh instance.
		/// </summary>
		/// <returns>The collision body as AsStaticBody3D, or null if not found.</returns>
		public AsStaticBody3D GetCollisionBody()
		{
			if (GetChildCount() > 0)
			{
				Node child = GetChild(0);
				if (null != child && child is AsStaticBody3D body)
				{
					return body;
				}
			}

			return null;
		}
		
		/// <summary>
		/// Checks if the mesh instance has collision nodes.
		/// </summary>
		/// <returns>True if the mesh instance has collision nodes, otherwise false.</returns>
		public bool HasCollisions()
		{
			return GetChildCount() != 0;
		}
	}
}
