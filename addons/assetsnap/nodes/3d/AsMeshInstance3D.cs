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
	using AssetSnap.ASNode.MeshInstance;
	using AssetSnap.Nodes;
	using AssetSnap.Static;

	[Tool]
	public partial class AsMeshInstance3D : Base, ICollisionableModel
	{
		private float fadeDuration = SettingsStatic.TransparencyFadeDuration(); // Fade duration in seconds
		private float fadeTimer = 0.0f;

		private ModelCollision Collision;

		public AsMeshInstance3D()
		{
			SetMeta("AsModel", true);
			SetMeta("Collision", true);
		}

		public override void _EnterTree()
		{
			if (SettingsStatic.ModelTransparencyActive())
			{
				Transparency = 1 - SettingsStatic.TransparencyLevel();
			}

			Collision = new();

			base._EnterTree();
		}

		public async override void _Ready()
		{
			base._Ready();
			Collision.RegisterCollisionType(this);

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
					Collision.Render();
				}
			}
		}

		public override void _Process(double delta)
		{
			if (
				fadeTimer != fadeDuration &&
				IsPlaced() &&
				SettingsStatic.ModelTransparencyActive()
			)
			{
				// Increment the timer
				fadeTimer += (float)delta;

				// Calculate the alpha value based on the timer
				float alpha = Mathf.Clamp(fadeTimer / fadeDuration, 0.25f, 1.0f);

				// Update the alpha property of the shader material
				Transparency = 1 - alpha;

				// Check if the fade-in is complete
				if (fadeTimer >= fadeDuration)
				{
					// Reset the timer or stop the fade-in effect
					fadeTimer = fadeDuration;
				}
			}

			base._Process(delta);
		}

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

			// Body
			GetCollisionBody().Owner = owner;
			// Shape
			GetCollisionBody().GetChild(0).Owner = owner;
		}
	}
}
