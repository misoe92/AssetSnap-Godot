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
namespace AssetSnap.Snap
{
	using AssetSnap.Explorer;
	using AssetSnap.States;
	using Godot;
	public class Boundary
	{
		private GlobalExplorer _GlobalExplorer;

		public GlobalStates.SnapAngleEnums Angle;

		private StaticBody3D _BoundaryBody;
		private MeshInstance3D _BoundaryMeshInstance;
		private PlaneMesh _BoundaryBoxMesh;
		private CollisionShape3D _BoundaryCollision;
		private BoxShape3D _BoundaryCollisionBox;
		private readonly Shader _BoundaryGrid = GD.Load<Shader>("res://addons/assetsnap/shaders/snap-grid.gdshader");
		private ShaderMaterial _BoundaryMaterial;

		public Boundary(GlobalStates.SnapAngleEnums angle)
		{
			_GlobalExplorer = GlobalExplorer.GetInstance();
			Angle = angle;

			_BoundaryMaterial = new();
			_BoundaryCollisionBox = new();
			_BoundaryBoxMesh = new();

			_BoundaryBody = new()
			{
				Name = "SnapToHeightBoundaryBody",
				CollisionLayer = 100,
				CollisionMask = 100,
			};

			_BoundaryMeshInstance = new()
			{
				Name = "SnapToHeightBoundaryMeshInstance"
			};

			_BoundaryCollision = new()
			{
				Name = "SnapToHeightBoundaryCollision"
			};

			AddCollisionBox(_BoundaryBody);
			if (true == ShouldShowBoundaryBox())
			{
				AddBoundaryBox(_BoundaryBody);
			}
		}

		public void Spawn(Node ToContainer)
		{
			ToContainer.AddChild(_BoundaryBody);
		}

		/*
		** Updates the opacity of the boundary
		** 
		** @param float value
		** @return void
		*/
		public void UpdateOpacity(float value)
		{
			if (_BoundaryMaterial != null)
			{
				_BoundaryMaterial.SetShaderParameter("opacity", value);
			}
		}

		/*
		** Updates the opacity of the boundary
		** 
		** @param float value
		** @return void
		*/
		public void UpdateFlat(float value)
		{
			if (_BoundaryMaterial != null)
			{
				_BoundaryMaterial.SetShaderParameter("flatten", value);
			}
		}

		/*
		** Updates the transform of the boundary
		** per rules set in the active library
		** 
		** @return void
		*/
		public void UpdateTransform()
		{
			Transform3D transform = _BoundaryBody.Transform;
			switch (Angle)
			{
				case GlobalStates.SnapAngleEnums.X:
					transform.Origin.X = StatesUtils.Get().SnapToXValue;

					// Y and Z should follow the mouse position to ensure the boundary is always available
					transform.Origin.Y = ExplorerUtils.Get().Decal.GetPosition().Y;
					transform.Origin.Z = ExplorerUtils.Get().Decal.GetPosition().Z;
					break;

				case GlobalStates.SnapAngleEnums.Y:
					transform.Origin.Y = StatesUtils.Get().SnapToHeightValue;

					// Y and Z should follow the mouse position to ensure the boundary is always available
					transform.Origin.X = ExplorerUtils.Get().Decal.GetPosition().X;
					transform.Origin.Z = ExplorerUtils.Get().Decal.GetPosition().Z;
					break;

				case GlobalStates.SnapAngleEnums.Z:
					transform.Origin.Z = StatesUtils.Get().SnapToZValue;

					// Y and Z should follow the mouse position to ensure the boundary is always available
					transform.Origin.Y = ExplorerUtils.Get().Decal.GetPosition().Y;
					transform.Origin.X = ExplorerUtils.Get().Decal.GetPosition().X;
					break;
			}

			_BoundaryBody.Transform = transform;
		}

		/*
		** Adds the collision box
		** to the scene
		** 
		** @param StaticBody3D to
		** @return void
		*/
		private void AddCollisionBox(StaticBody3D to)
		{
			switch (Angle)
			{
				case GlobalStates.SnapAngleEnums.Y:
					_BoundaryCollisionBox.Size = new Vector3(25, 0.1f, 25);

					break;

				case GlobalStates.SnapAngleEnums.X:
					_BoundaryCollisionBox.Size = new Vector3(0.1f, 25, 25);

					break;

				case GlobalStates.SnapAngleEnums.Z:
					_BoundaryCollisionBox.Size = new Vector3(25, 25, 0.1f);
					break;
			}

			_BoundaryCollision.Shape = _BoundaryCollisionBox;
			to.AddChild(_BoundaryCollision);
		}

		/*
		** Adds the boundary box
		** to the scene
		** 
		** @param StaticBody3D to
		** @return void
		*/
		private void AddBoundaryBox(StaticBody3D to)
		{
			switch (Angle)
			{
				case GlobalStates.SnapAngleEnums.Y:
					_BoundaryBoxMesh.Size = new Vector2(25, 25);

					break;

				case GlobalStates.SnapAngleEnums.X:
					_BoundaryBoxMesh.Size = new Vector2(25, 25);

					Vector3 RotX = _BoundaryMeshInstance.RotationDegrees;
					RotX.X = 270;
					// RotX.Y = 90;
					RotX.Z = 270;
					_BoundaryMeshInstance.RotationDegrees = RotX;

					break;

				case GlobalStates.SnapAngleEnums.Z:
					_BoundaryBoxMesh.Size = new Vector2(25, 25);

					Vector3 RotZ = _BoundaryMeshInstance.RotationDegrees;
					RotZ.X = 270;
					_BoundaryMeshInstance.RotationDegrees = RotZ;

					break;
			}
			_BoundaryBoxMesh.Material = GetBoundaryMaterial();
			_BoundaryMeshInstance.Mesh = _BoundaryBoxMesh;

			to.AddChild(_BoundaryMeshInstance);
		}

		private ShaderMaterial GetBoundaryMaterial()
		{
			_BoundaryMaterial.Shader = _BoundaryGrid;

			_BoundaryMaterial.SetShaderParameter("scale_0", 25);
			_BoundaryMaterial.SetShaderParameter("scale_1", 25);

			_BoundaryMaterial.SetShaderParameter("line_scale_0", 0.02f);
			_BoundaryMaterial.SetShaderParameter("line_scale_1", 0.01f);

			_BoundaryMaterial.SetShaderParameter("color_0", new Color(Colors.Black.R, Colors.Black.G, Colors.Black.B, 0.1f));
			_BoundaryMaterial.SetShaderParameter("color_1", new Color(Colors.White));

			_BoundaryMaterial.SetShaderParameter("opacity", GetBoundaryOpacity());
			_BoundaryMaterial.SetShaderParameter("flatten", GetBoundaryFlat());

			return _BoundaryMaterial;
		}

		private float GetBoundaryOpacity()
		{
			return _GlobalExplorer.Settings.GetKey("boundary_box_opacity").As<float>();
		}

		private bool GetBoundaryFlat()
		{
			return _GlobalExplorer.Settings.GetKey("boundary_box_flat").As<bool>();
		}

		private bool ShouldShowBoundaryBox()
		{
			return _GlobalExplorer.Settings.GetKey("show_snap_boundary_box").As<bool>();
		}

		public void ExitTree()
		{
			if (EditorPlugin.IsInstanceValid(_BoundaryCollision))
			{
				_BoundaryCollision.QueueFree();
			}

			if (EditorPlugin.IsInstanceValid(_BoundaryMeshInstance))
			{
				_BoundaryMeshInstance.QueueFree();
			}

			if (EditorPlugin.IsInstanceValid(_BoundaryBody))
			{
				_BoundaryBody.QueueFree();
			}
		}
	}
}
#endif