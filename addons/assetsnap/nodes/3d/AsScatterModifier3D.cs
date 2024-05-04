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
using AssetSnap.States;
using AssetSnap.Static;
using Godot;
using System;
using System.Collections.Generic;

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// A tool script for scattering instances in 3D space.
	/// </summary>
	[Tool]
	public partial class AsScatterModifier3D : AsGroup3D
	{
		[ExportGroup("Settings")]
		[ExportSubgroup("Modifier")]
		/// <summary>
		/// Gets or sets the name of the scatter modifier.
		/// </summary>
		/// <value>The name of the scatter modifier.</value>
		[Export]
		public string ScatterName
		{
			get => _Name;
			set
			{
				base.Name = value;
				_Name = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of duplication to be used.
		/// </summary>
		/// <value>The type of duplication.</value>
		[Export]
		public string DuplicateType
		{
			get => _DuplicateType;
			set
			{
				_DuplicateType = value;
			}
		}

		/// <summary>
		/// Property representing the node for duplicates.
		/// </summary>
		[Export]
		public Node Duplicates
		{
			get => _Duplicates;
			set
			{
				_Duplicates = value;
			}
		}
		
		[ExportSubgroup("Debug")]
		
		/// <summary>
		/// Property to show or hide the boundary box.
		/// </summary>
		[Export]
		public bool ShowBoundaryBox
		{
			get => _ShowBoundaryBox;
			set
			{
				_ShowBoundaryBox = value;
			}
		}

		/// <summary>
		/// Property representing the color of the boundary box.
		/// </summary>
		[Export]
		public Color BoundaryBoxColor
		{
			get => _BoundaryBoxColor;
			set
			{
				if (IsInstanceValid(BoundaryNode))
				{
					RemoveChild(BoundaryNode);
					BoundaryNode.QueueFree();
					BoundaryNode = null;
				}

				_BoundaryBoxColor = value;
			}
		}

		/// <summary>
		/// Property representing the height of the boundary box.
		/// </summary>
		[Export]
		public float BoundaryBoxHeight
		{
			get => _BoundaryBoxHeight;
			set
			{
				if (IsInstanceValid(BoundaryNode))
				{
					RemoveChild(BoundaryNode);
					BoundaryNode.QueueFree();
					BoundaryNode = null;
				}

				_BoundaryBoxHeight = value;
			}
		}

		[ExportSubgroup("Optimization")]

		/// <summary>
		/// Property to determine whether to use multi-mesh.
		/// </summary>
		[Export]
		public bool UseMultiMesh
		{
			get => _UseMultiMesh;
			set
			{
				_UseMultiMesh = value;
				UpdateScatter();
			}
		}
	
		[ExportSubgroup("Collisions")]

		/// <summary>
		/// Property to force collisions.
		/// </summary>
		[Export]
		public bool ForceCollisions
		{
			get => _ForceCollisions;
			set
			{
				_ForceCollisions = value;
				UpdateScatter();
			}
		}

		/// <summary>
		/// Property representing whether to allow collisions.
		/// </summary>
		[Export]
		public bool NoCollisions
		{
			get => _NoCollisions;
			set
			{
				_NoCollisions = value;
				UpdateScatter();
			}
		}

		/// <summary>
		/// Property representing whether to use a sphere.
		/// </summary>
		[Export]
		public bool UseSphere
		{
			get => _UseSphere;
			set
			{
				_UseSphere = value;
				UpdateScatter();
				NotifyPropertyListChanged();
			}
		}

		/// <summary>
		/// Property representing whether to use a convex polygon.
		/// </summary>
		[Export]
		public bool UseConvexPolygon
		{
			get => _UseConvexPolygon;
			set
			{
				_UseConvexPolygon = value;
				UpdateScatter();
				NotifyPropertyListChanged();
			}
		}

		/// <summary>
		/// Property representing whether to clean a convex shape.
		/// </summary>
		[Export]
		public bool UseConvexClean
		{
			get => _UseConvexClean;
			set
			{
				_UseConvexClean = value;
				UpdateScatter();
				NotifyPropertyListChanged();
			}
		}

		/// <summary>
		/// Property representing whether to simplify a convex shape.
		/// </summary>
		[Export]
		public bool UseConvexSimplify
		{
			get => _UseConvexSimplify;
			set
			{
				_UseConvexSimplify = value;
				UpdateScatter();
				NotifyPropertyListChanged();
			}
		}

		/// <summary>
		/// Property representing whether to use a concave polygon.
		/// </summary>
		[Export]
		public bool UseConcavePolygon
		{
			get => _UseConcavePolygon;
			set
			{
				_UseConcavePolygon = value;
				UpdateScatter();
				NotifyPropertyListChanged();
			}
		}

		[ExportSubgroup("Mesh")]

		/// <summary>
		/// Property representing the instance library.
		/// </summary>
		[Export]
		public string InstanceLibrary
		{
			get => _InstanceLibrary;
			set
			{
				_InstanceLibrary = value;
			}
		}

		[ExportCategory("General")]

		/// <summary>
		/// Property representing the scatter radius.
		/// </summary>
		[Export]
		public int ScatterRadius
		{
			get => _ScatterRadius;
			set
			{
				_ScatterRadius = value;
				UpdateScatter();
			}
		}

		/// <summary>
		/// Property representing the scatter count.
		/// </summary>
		[Export]
		public int ScatterCount
		{
			get => _ScatterCount;
			set
			{
				_ScatterCount = value;
				UpdateScatter();
			}
		}

		/// <summary>
		/// Property representing the minimum distance.
		/// </summary>
		[Export]
		public float MinDistance
		{
			get => _MinDistance;
			set
			{
				_MinDistance = value;
				UpdateScatter();
			}
		}

		/// <summary>
		/// Property representing the noise.
		/// </summary>
		[Export]
		public FastNoiseLite Noise
		{
			get => _Noise;
			set
			{
				_Noise = value;
				UpdateScatter();
			}
		}
	
		[ExportCategory("Scatter Height")]

		/// <summary>
		/// Property representing whether the height is fixed.
		/// </summary>
		[Export]
		public bool FixedHeight
		{
			get => _FixedHeight;
			set
			{
				if (value)
				{
					_RayCastHeight = false;
				}

				_FixedHeight = value;
				UpdateScatter();
			}
		}

		/// <summary>
		/// Property representing the fixed height value.
		/// </summary>
		[Export]
		public float FixedHeightValue
		{
			get => _FixedHeightValue;
			set
			{
				_FixedHeightValue = value;
				UpdateScatter();
			}
		}

		/// <summary>
		/// Property representing whether to use raycast height.
		/// </summary>
		[Export]
		public bool RayCastHeight
		{
			get => _RayCastHeight;
			set
			{
				if (value)
				{
					_FixedHeight = false;
					_FixedHeightValue = 0.0f;
				}

				_RayCastHeight = value;
				UpdateScatter();
			}
		}
		
		
		public AsMultiMeshInstance3D MultiMeshInstance;
		public MultiMesh ScatterMultiMesh;
		public Node3D BoundaryNode;
		
		private Node _Duplicates;
		private Color _BoundaryBoxColor;
		private FastNoiseLite _Noise;
		private string _InstanceLibrary;
		private string _Name;
		private string _DuplicateType;
		private int _ScatterRadius = 1;
		private int _ScatterCount = 1;
		private int _WorkingCount;
		private int _PositionFail = 0;
		private float _MinDistance = 1.0f;
		private float _FixedHeightValue = 0.0f;
		private float _BoundaryBoxHeight = 1;
		private bool _FixedHeight = false;
		private bool _PositionFailed = false;
		private bool _Initialized = false;
		private bool _ShowBoundaryBox = false;
		private bool _UseConcavePolygon = false;
		private bool _UseConvexSimplify = false;
		private bool _UseConvexClean = false;
		private bool _UseConvexPolygon = false;
		private bool _UseSphere = false;
		private bool _NoCollisions = false;
		private bool _ForceCollisions = false;
		private bool _UseMultiMesh = false;
		private bool _RayCastHeight = false;

		/// <summary>
		/// Initializes the scatter modifier when the scene tree is ready.
		/// </summary>
		public override void _Ready()
		{
			Noise = new();
			_SceneRoot = GlobalExplorer.GetInstance()._Plugin.GetTree().EditedSceneRoot;

			_BoundaryBoxColor = new Color(
				(float)GD.RandRange(0.0, 0.5),
				(float)GD.RandRange(0.0, 0.5),
				(float)GD.RandRange(0.0, 0.5),
				(float)GD.RandRange(0.2, 0.5)
			);

			Noise.Connect(FastNoiseLite.SignalName.Changed, new Callable(this, "_OnNoiseChange"));
			_Initialized = true;
			base._Ready();

			UpdateScatter();
		}

		/// <summary>
		/// Validates the property based on the current settings.
		/// </summary>
		/// <param name="property">The property to be validated.</param>
		public override void _ValidateProperty(Godot.Collections.Dictionary property)
		{
			if ((UseConvexPolygon == true || UseConcavePolygon == true) && property.ContainsKey("name") && (property["name"].As<string>() == "UseSphere" || property["name"].As<string>() == "UseSphere"))
			{
				var usage = PropertyUsageFlags.ReadOnly;
				property["usage"] = (int)usage;
			}
			else if (UseConvexPolygon == false && UseConcavePolygon == false && property.ContainsKey("name") && (property["name"].As<string>() == "UseSphere" || property["name"].As<string>() == "UseSphere"))
			{
				var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
				property["usage"] = (int)usage;
			}

			if (UseConvexPolygon == false && property.ContainsKey("name") && (property["name"].As<string>() == "UseConvexClean" || property["name"].As<string>() == "UseConvexClean"))
			{
				var usage = PropertyUsageFlags.ReadOnly;
				property["usage"] = (int)usage;
			}
			else if (UseConvexPolygon == true && property.ContainsKey("name") && (property["name"].As<string>() == "UseConvexClean" || property["name"].As<string>() == "UseConvexClean"))
			{
				var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
				property["usage"] = (int)usage;
			}

			if (UseConvexPolygon == false && property.ContainsKey("name") && (property["name"].As<string>() == "UseConvexSimplify" || property["name"].As<string>() == "UseConvexSimplify"))
			{
				var usage = PropertyUsageFlags.ReadOnly;
				property["usage"] = (int)usage;
			}
			else if (UseConvexPolygon == true && property.ContainsKey("name") && (property["name"].As<string>() == "UseConvexSimplify" || property["name"].As<string>() == "UseConvexSimplify"))
			{
				var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
				property["usage"] = (int)usage;
			}

			if ((UseSphere == true || UseConcavePolygon == true) && property.ContainsKey("name") && (property["name"].As<string>() == "UseConvexPolygon" || property["name"].As<string>() == "UseConvexPolygon"))
			{
				var usage = PropertyUsageFlags.ReadOnly;
				property["usage"] = (int)usage;
			}
			else if (UseSphere == false && UseConcavePolygon == false && property.ContainsKey("name") && (property["name"].As<string>() == "UseConvexPolygon" || property["name"].As<string>() == "UseConvexPolygon"))
			{
				var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
				property["usage"] = (int)usage;
			}

			if ((UseSphere == true || UseConvexPolygon == true) && property.ContainsKey("name") && (property["name"].As<string>() == "UseConcavePolygon" || property["name"].As<string>() == "UseConcavePolygon"))
			{
				var usage = PropertyUsageFlags.ReadOnly;
				property["usage"] = (int)usage;
			}
			else if (UseSphere == false && UseConvexPolygon == false && property.ContainsKey("name") && (property["name"].As<string>() == "UseConcavePolygon" || property["name"].As<string>() == "UseConcavePolygon"))
			{
				var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
				property["usage"] = (int)usage;
			}

			base._ValidateProperty(property);
		}

		/// <summary>
		/// Processes the scatter modifier on each frame.
		/// </summary>
		/// <param name="delta">The time in seconds since the last frame.</param>
		public override void _Process(double delta)
		{
			if (null == GetParent() || false == IsInstanceValid(GetParent()))
			{
				return;
			}

			if (ShowBoundaryBox && false == IsInstanceValid(BoundaryNode))
			{
				_SetupBoundaryBox();
			}
			else if (false == ShowBoundaryBox)
			{
				if (HasNode("BoundaryNode"))
				{
					var node = GetNode("BoundaryNode");
					RemoveChild(node);
					node.QueueFree();
				}
			}

			base._Process(delta);
		}

		/// <summary>
		/// Sets up the boundary box for visual debugging.
		/// </summary>
		private void _SetupBoundaryBox()
		{
			if (HasNode("BoundaryNode"))
			{
				var node = GetNode("BoundaryNode");
				RemoveChild(node);
				node.QueueFree();
			}

			// Create a new transparent material
			StandardMaterial3D transparentMaterial = new StandardMaterial3D()
			{
				AlbedoColor = _BoundaryBoxColor,
				Transparency = BaseMaterial3D.TransparencyEnum.Alpha
			};

			BoundaryNode = new()
			{
				Name = "BoundaryNode"
			};

			SphereMesh BoundaryMesh = new()
			{
				Radius = _ScatterRadius,
				Height = _BoundaryBoxHeight,
			};

			MeshInstance3D _BoundaryBody = new()
			{
				Name = "BoundaryBody",
				Mesh = BoundaryMesh,
				MaterialOverride = transparentMaterial
			};

			BoundaryNode.AddChild(_BoundaryBody);
			AddChild(BoundaryNode);
		}

		/// <summary>
		/// Handles the noise change event and updates the scatter accordingly.
		/// </summary>
		private void _OnNoiseChange()
		{
			UpdateScatter();
		}

		/// <summary>
		/// Updates the scatter based on the current settings.
		/// </summary>
		public void UpdateScatter()
		{
			if (false == _Initialized || null == Duplicates)
			{
				return;
			}

			bool HasNoise = Noise is FastNoiseLite;

			if (HasNoise == false)
			{
				GD.PushWarning("Cannot scatter without a noise pattern");
				return;
			}

			if (0 != GetChildCount())
			{
				ClearCurrentChildren();
			}

			StatesUtils.Get().ConcaveCollision = GlobalStates.LibraryStateEnum.Disabled;
			StatesUtils.Get().ConvexCollision = GlobalStates.LibraryStateEnum.Disabled;
			StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Disabled;
			StatesUtils.Get().ConvexSimplify = GlobalStates.LibraryStateEnum.Disabled;
			StatesUtils.Get().SphereCollision = GlobalStates.LibraryStateEnum.Disabled;

			// Apply options
			if (UseSphere)
			{
				StatesUtils.Get().SphereCollision = GlobalStates.LibraryStateEnum.Enabled;
			}

			if (UseConcavePolygon)
			{
				StatesUtils.Get().ConcaveCollision = GlobalStates.LibraryStateEnum.Enabled;
			}

			if (UseConvexPolygon)
			{
				StatesUtils.Get().ConvexCollision = GlobalStates.LibraryStateEnum.Enabled;

				if (UseConvexClean)
				{
					StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Enabled;
				}

				if (UseConvexSimplify)
				{
					StatesUtils.Get().ConvexSimplify = GlobalStates.LibraryStateEnum.Enabled;
				}
			}

			if (_UseMultiMesh)
			{
				CreateMultiMeshScatter();
			}
			else
			{
				CreateSimpleScatter();
			}
		}

		/// <summary>
		/// Creates a simple scatter of instances in the scene.
		/// </summary>
		private void CreateSimpleScatter()
		{
			List<Vector3> positions = new List<Vector3>();
			for (int i = 0; i < _ScatterCount; i++)
			{
				Vector2 newPosition;
				do
				{
					Aabb _aabb = NodeUtils.CalculateNodeAabb(Duplicates);
					// Generate random angle
					float angle = (float)GD.RandRange(0, 2 * Mathf.Pi);

					// Generate random distance within the radius
					float maxDistance = _ScatterRadius - _aabb.Size.X - 0.5f; // Adjusted to ensure the entire mesh stays within the radius
					float distance = (float)GD.RandRange(0, maxDistance);

					// Use FastNoiseLite to perturb the position
					float noiseValue = _Noise.GetNoise2D(distance, angle);

					// Adjust the distance based on noise
					distance += noiseValue * _ScatterRadius;

					// Calculate x and y coordinates based on polar coordinates
					float x = distance * Mathf.Cos(angle);
					float y = distance * Mathf.Sin(angle);

					newPosition = new Vector2(x, y);

				} while (!IsPositionValid(newPosition, positions, i));

				if (_PositionFailed == false)
				{
					// Create a new instance of the MeshInstance
					Node3D _Model = Duplicates.Duplicate() as Node3D;

					if (_Model == null)
					{
						throw new Exception("Model is invalid");
					}

					// Set the position of the MeshInstance
					Transform3D transform = new Transform3D(Basis.Identity, Vector3.Zero);
					transform.Origin = new Vector3(newPosition.X, 0, newPosition.Y);

					if (FixedHeight)
					{
						transform.Origin.Y = FixedHeightValue;
					}

					if (RayCastHeight)
					{
						transform.Origin.Y = CastHeight(transform.Origin);
					}

					if (_Model is AsMeshInstance3D)
					{
						ApplyModelMeta(_Model);
					}

					if (_Model is AsNode3D)
					{
						foreach (Node child in _Model.GetChildren())
						{
							ApplyModelMeta(child);
						}
					}

					// Add the MeshInstance to the scene
					AddChild(_Model, true);
					_Model.Owner = _SceneRoot;
					_Model.Transform = transform;

					if (_Model is AsMeshInstance3D meshInstance3D)
					{
						meshInstance3D.SetLibraryName(InstanceLibrary);
						meshInstance3D.SetIsFloating(false);
						meshInstance3D.GetCollisionBody().Initialize();
						meshInstance3D.UpdateViewability();
					}

					if (_Model is AsNode3D node3d)
					{
						node3d.SetLibraryName(InstanceLibrary);
						node3d.SetIsFloating(false);

						foreach (Node child in node3d.GetChildren())
						{
							child.Owner = GetTree().EditedSceneRoot;

							if (child is AsMeshInstance3D childMeshInstance3D)
							{
								childMeshInstance3D.SetLibraryName(InstanceLibrary);
								childMeshInstance3D.SetIsFloating(false);
								childMeshInstance3D.GetCollisionBody().Initialize();
								childMeshInstance3D.UpdateViewability();
							}
						}
					}

					// Add the position to the list for future checks
					positions.Add(new Vector3(newPosition.X, transform.Origin.Y, newPosition.Y));
				}
				else
				{
					_ScatterCount = i;
					GD.PushWarning("Not enough space in the radius");
					break;
				}
			}

			// Randomnize();
		}

		/// <summary>
		/// Creates a scatter using the MultiMeshInstance node.
		/// </summary>
		private void CreateMultiMeshScatter()
		{
			if (Duplicates is AsMeshInstance3D asMeshInstance3D)
			{
				_CreateSimpleMultiMeshScatter(asMeshInstance3D);
			}
			else
			{
				_CreateAdvancedMultiMeshScatter((AsNode3D)Duplicates);
			}
			// Randomnize();
		}

		/// <summary>
		/// Creates a simple scatter using MultiMeshInstance.
		/// </summary>
		/// <param name="asMeshInstance3D">The MeshInstance3D to scatter.</param>
		private void _CreateSimpleMultiMeshScatter(AsMeshInstance3D asMeshInstance3D)
		{
			ScatterMultiMesh = new()
			{
				TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
				Mesh = asMeshInstance3D.Mesh,
				InstanceCount = _ScatterCount,
			};

			MultiMeshInstance = new()
			{
				Multimesh = ScatterMultiMesh,
			};
			ApplyModelMeta(MultiMeshInstance);

			List<Vector3> positions = new List<Vector3>();

			for (int i = 0; i < _ScatterCount; i++)
			{
				Vector2 newPosition;

				do
				{
					Aabb _aabb = asMeshInstance3D.GetAabb();
					// Generate random angle
					float angle = (float)GD.RandRange(0, 2 * Mathf.Pi);

					// Generate random distance within the radius
					float maxDistance = _ScatterRadius - _aabb.Size.X - 0.5f; // Adjusted to ensure the entire mesh stays within the radius
					float distance = (float)GD.RandRange(0, maxDistance);

					// Use FastNoiseLite to perturb the position
					float noiseValue = _Noise.GetNoise2D(distance, angle);

					// Adjust the distance based on noise
					distance += noiseValue * _ScatterRadius;

					// Calculate x and y coordinates based on polar coordinates
					float x = distance * Mathf.Cos(angle);
					float y = distance * Mathf.Sin(angle);

					newPosition = new Vector2(x, y);

				} while (!IsPositionValid(newPosition, positions, i));

				if (_PositionFailed == false)
				{
					Transform3D Trans = Transform3D.Identity;
					Trans.Origin = new Vector3(newPosition.X, 0, newPosition.Y);

					if (FixedHeight)
					{
						Trans.Origin.Y = FixedHeightValue;
					}

					if (RayCastHeight)
					{
						Trans.Origin.Y = CastHeight(Trans.Origin);
					}

					ScatterMultiMesh.SetInstanceTransform(i, Trans);

					// Add the position to the list for future checks
					positions.Add(new Vector3(newPosition.X, Trans.Origin.Y, newPosition.Y));
				}
				else
				{
					_ScatterCount = i;
					GD.PushWarning("Not enough space in the radius");
					break;
				}
			}

			// Add the MeshInstance to the scene
			AddChild(MultiMeshInstance);
			MultiMeshInstance.Owner = _SceneRoot;
		}

		/// <summary>
		/// Creates an advanced scatter using MultiMeshInstance.
		/// </summary>
		/// <param name="node3d">The parent node containing MeshInstance3D children.</param>
		private void _CreateAdvancedMultiMeshScatter(AsNode3D node3d)
		{
			// Childable
			foreach (MeshInstance3D child in node3d.GetChildren())
			{
				ScatterMultiMesh = new()
				{

					TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
					Mesh = child.Mesh,
					InstanceCount = _ScatterCount,
				};

				MultiMeshInstance = new()
				{
					Multimesh = ScatterMultiMesh,
				};
				ApplyModelMeta(MultiMeshInstance);

				List<Vector3> positions = new List<Vector3>();

				for (int i = 0; i < _ScatterCount; i++)
				{
					Vector2 newPosition;

					do
					{
						Aabb _aabb = node3d.GetAabb();
						// Generate random angle
						float angle = (float)GD.RandRange(0, 2 * Mathf.Pi);

						// Generate random distance within the radius
						float maxDistance = _ScatterRadius - _aabb.Size.X - 0.5f; // Adjusted to ensure the entire mesh stays within the radius
						float distance = (float)GD.RandRange(0, maxDistance);

						// Use FastNoiseLite to perturb the position
						float noiseValue = _Noise.GetNoise2D(distance, angle);

						// Adjust the distance based on noise
						distance += noiseValue * _ScatterRadius;

						// Calculate x and y coordinates based on polar coordinates
						float x = distance * Mathf.Cos(angle);
						float y = distance * Mathf.Sin(angle);

						newPosition = new Vector2(x, y);

					} while (!IsPositionValid(newPosition, positions, i));

					if (_PositionFailed == false)
					{
						Transform3D Trans = Transform3D.Identity;
						Trans.Origin = new Vector3(newPosition.X, 0, newPosition.Y);

						if (FixedHeight)
						{
							Trans.Origin.Y = FixedHeightValue;
						}

						if (RayCastHeight)
						{
							Trans.Origin.Y = CastHeight(Trans.Origin);
						}

						ScatterMultiMesh.SetInstanceTransform(i, Trans);

						// Add the position to the list for future checks
						positions.Add(new Vector3(newPosition.X, Trans.Origin.Y, newPosition.Y));
					}
					else
					{
						_ScatterCount = i;
						GD.PushWarning("Not enough space in the radius");
						break;
					}
				}

				// Add the MeshInstance to the scene
				AddChild(MultiMeshInstance);
				MultiMeshInstance.Owner = _SceneRoot;
				MultiMeshInstance.Name = _Name + "/multiMesh";
			}
		}

		/// <summary>
		/// Checks if a position is valid for scattering.
		/// </summary>
		/// <param name="newPosition">The position to check.</param>
		/// <param name="existingPositions">The list of existing positions.</param>
		/// <param name="count">The count of positions checked.</param>
		/// <returns><c>true</c> if the position is valid; otherwise, <c>false</c>.</returns>
		private bool IsPositionValid(Vector2 newPosition, List<Vector3> existingPositions, int count)
		{
			if (_WorkingCount != count)
			{
				_PositionFailed = false;
				_PositionFail = 0;
				_WorkingCount = count;
			}

			if (_WorkingCount == count && _PositionFail > 10)
			{
				_PositionFailed = true;
				return true;
			}

			// Check if the new position is at least MinDistance away from existing positions
			foreach (Vector3 existingPosition in existingPositions)
			{
				float distance = newPosition.DistanceTo(new Vector2(existingPosition.X, existingPosition.Z));
				if (distance < MinDistance)
				{
					_PositionFail += 1;
					return false; // Invalid position, too close to an existing position
				}
			}

			return true; // Valid position
		}

		/// <summary>
		/// Casts a ray to determine the height at a given position.
		/// </summary>
		/// <param name="Origin">The origin of the ray.</param>
		/// <returns>The height at the given position.</returns>
		private float CastHeight(Vector3 Origin)
		{
			float height = 0.0f;

			// Replace this with the actual position for which you want to find the height
			Vector3 targetPosition = Origin;

			// Cast a ray from the target position straight down
			RayCast3D _Ray = new RayCast3D();
			AddChild(_Ray);

			Transform3D Trans = _Ray.Transform;
			Trans.Origin = targetPosition;
			_Ray.Transform = Trans;

			_Ray.TargetPosition = new Vector3(0, -1000, 0); // Adjust the length of the ray based on your scene's dimensions

			// Perform the raycast
			_Ray.ForceRaycastUpdate(); // This is necessary to perform the initial raycast

			// Check for collision
			if (_Ray.IsColliding())
			{
				// Get the collision point, which contains the height
				height = _Ray.GetCollisionPoint().Y;
			}
			else
			{
				_Ray.TargetPosition = new Vector3(0, 1000, 0); // Adjust the length of the ray based on your scene's dimensions

				// Perform the raycast
				_Ray.ForceRaycastUpdate(); // This is necessary to perform the initial raycast
			}

			// Remove the ray after use
			RemoveChild(_Ray);
			_Ray.QueueFree();

			return height - Transform.Origin.Y;
		}

		/// <summary>
		/// Applies metadata to the model based on current settings.
		/// </summary>
		/// <param name="_Model">The model node to apply metadata to.</param>
		private void ApplyModelMeta(Node _Model)
		{
			if (ForceCollisions)
			{
				_Model.SetMeta("Collision", true);
			}

			if (NoCollisions)
			{
				_Model.SetMeta("Collision", false);
			}
		}
		
		/// <summary>
		/// Determines whether collisions should be added based on current settings.
		/// </summary>
		/// <returns><c>true</c> if collisions should be added; otherwise, <c>false</c>.</returns>
		private bool ShouldAddCollision()
		{
			return SettingsStatic.ShouldAddCollision();
		}
	}
}