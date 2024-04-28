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
	using System;
	using System.Reflection;
	using Godot;

	[Tool]
	public partial class GroupResource : Resource
	{
		[Export]
		public string Name { get; set; } = "";

		[Export]
		public string Title { get; set; } = "";

		[Export]
		public Godot.Collections.Array<string> _Paths { get; set; } = new();

		[Export]
		public Godot.Collections.Dictionary<int, Vector3> _Origins { get; set; } = new();

		[Export]
		public Godot.Collections.Dictionary<int, Vector3> _Rotations { get; set; } = new();

		[Export]
		public Godot.Collections.Dictionary<int, Vector3> _Scales { get; set; } = new();

		[Export]
		public Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> _Options { get; set; } = new();

		[Export]
		public int SnapLayer { get; set; } = 0;

		[Export]
		public float SnapToObjectOffsetXValue { get; set; } = 0.0f;

		[Export]
		public float SnapToObjectOffsetZValue { get; set; } = 0.0f;

		[Export]
		public float SnapToHeightValue { get; set; } = 0.0f;

		[Export]
		public float SnapToXValue { get; set; } = 0.0f;

		[Export]
		public float SnapToZValue { get; set; } = 0.0f;

		[Export]
		public bool SphereCollision { get; set; } = false;

		[Export]
		public bool ConvexCollision { get; set; } = false;

		[Export]
		public bool ConvexClean { get; set; } = false;

		[Export]
		public bool ConvexSimplify { get; set; } = false;

		[Export]
		public bool ConcaveCollision { get; set; } = false;

		[Export]
		public bool SnapToObject { get; set; } = false;

		[Export]
		public bool SnapToHeight { get; set; } = false;

		[Export]
		public bool SnapToX { get; set; } = false;

		[Export]
		public bool SnapToZ { get; set; } = false;

		public void EachProperty(Action<string, Variant> action)
		{
			Godot.Collections.Array<string> properties = new()
			{
				"SnapLayer",
				"SnapToObjectOffsetXValue",
				"SnapToObjectOffsetZValue",
				"SnapToHeightValue",
				"SnapToXValue",
				"SnapToZValue",
				"SphereCollision",
				"ConvexCollision",
				"ConvexClean",
				"ConvexSimplify",
				"ConcaveCollision",
				"SnapToObject",
				"SnapToHeight",
				"SnapToX",
				"SnapToZ",
			};
			Type type = GetType();

			foreach (string propertyName in properties)
			{
				// Get the property info using reflection
				PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (property != null)
				{
					// Get the value of the property
					object value = property.GetValue(this);

					if (value is bool boolVal)
					{
						// Call the provided action with the property name and its value
						action(propertyName, boolVal);
					}
					else if (value is float floatVal)
					{
						// Call the provided action with the property name and its value
						action(propertyName, floatVal);
					}
					else if (value is float intVal)
					{
						// Call the provided action with the property name and its value
						action(propertyName, intVal);
					}
				}
				else
				{
					GD.Print("Property " + propertyName + " not found");
				}
			}
		}

		public void AddChildren(AsGrouped3D group, Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> childOptions)
		{
			for (int i = 0; i < _Paths.Count; i++)
			{
				Vector3 Origin = _Origins[i];
				Vector3 Rotation = _Rotations[i];
				Vector3 Scale = _Scales[i];

				Transform3D transform = new(Basis.Identity, Vector3.Zero)
				{
					Origin = Origin,
				};

				GodotObject resource = GD.Load(_Paths[i]);

				if (resource is Mesh mesh)
				{
					AsMeshInstance3D asMeshInstance3D = new()
					{
						Name = _Paths[i] + "/" + i,
						Mesh = mesh,
						Transform = transform,
						RotationDegrees = Rotation,
						Scale = Scale,
						Floating = true,
						SpawnSettings = _Options[i]
					};

					group.AddChild(asMeshInstance3D);
					asMeshInstance3D.Owner = null != group.Owner ? group.Owner : null;
				}
				else if (resource is PackedScene _scene)
				{
					Node node = _scene.Instantiate();
					node.Owner = null;
					
					AsNode3D node3D = new()
					{
						Name = _Paths[i] + "/" + i,
						Transform = transform,
						RotationDegrees = Rotation,
						Scale = Scale,
						Floating = true,
						SpawnSettings = _Options[i]
					};
					
					group.AddChild(node3D);
					node3D.Owner = group.Owner;
					
					foreach (MeshInstance3D child in node.GetChildren())
					{
						child.Owner = null;
						node.RemoveChild(child);
						
						AsMeshInstance3D asMeshInstance3D = new()
						{
							Name = child.Name,
							Mesh = child.Mesh,
							Transform = child.Transform,
							RotationDegrees = child.Rotation,
							Scale = child.Scale,
							Floating = true,
							SpawnSettings = _Options[i],
						};
						
						node3D.AddChild(asMeshInstance3D);
						asMeshInstance3D.Owner = group.Owner;
					}

					node.QueueFree();
				}
				
			}
		}

		public void AddCollidingChildren(AsGrouped3D group, Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> childOptions)
		{
			// Add all children
			for (int i = 0; i < _Paths.Count; i++)
			{
				Vector3 Origin = _Origins[i];
				Vector3 Rotation = _Rotations[i];
				Vector3 Scale = _Scales[i];

				Transform3D transform = new(Basis.Identity, Vector3.Zero)
				{
					Origin = Origin,
				};

				GodotObject resource = GD.Load(_Paths[i]);

				if (resource is Mesh mesh)
				{
					AsMeshInstance3D asMeshInstance3D = new()
					{
						Name = _Paths[i] + "/" + i,
						Mesh = mesh,
						Transform = transform,
						RotationDegrees = Rotation,
						Scale = Scale,
						Floating = true,
						SpawnSettings = _Options[i]
					};

					group.AddChild(asMeshInstance3D);
				}
				else if (resource is PackedScene _scene)
				{
					Node node = _scene.Instantiate();
					node.Owner = null;
					
					AsNode3D node3D = new()
					{
						Name = _Paths[i] + "/" + i,
						Transform = transform,
						RotationDegrees = Rotation,
						Scale = Scale,
						Floating = true,
						SpawnSettings = _Options[i]
					};
					
					group.AddChild(node3D);
					
					foreach (MeshInstance3D child in node.GetChildren())
					{
						child.Owner = null;
						node.RemoveChild(child);
						
						AsMeshInstance3D asMeshInstance3D = new()
						{
							Name = child.Name,
							Mesh = child.Mesh,
							Transform = child.Transform,
							RotationDegrees = child.Rotation,
							Scale = child.Scale,
							Floating = true,
							SpawnSettings = _Options[i],
						};
						
						node3D.AddChild(asMeshInstance3D);
						asMeshInstance3D.Owner = group.Owner;
					}
				}
			}
		}

		public AsGrouped3D Build()
		{
			// Initialize variables to store distances
			float leftDistance = 0.0f;
			float rightDistance = 0.0f;
			float topDistance = 0.0f;
			float bottomDistance = 0.0f;

			// Iterate through each child node (assumed to be MeshInstance)
			foreach ((int i, Vector3 child) in _Origins)
			{
				if (child.X < leftDistance)
				{
					leftDistance = child.X;
				}

				if (child.X > rightDistance)
				{
					rightDistance = child.X;
				}

				if (child.Z < bottomDistance)
				{
					bottomDistance = child.Z;
				}

				if (child.Z > topDistance)
				{
					topDistance = child.Z;
				}
			}

			AsGrouped3D group = new()
			{
				Name = "GroupedObjects",
				GroupPath = "res://groups/" + Name + ".tres",
				ChildOptions = _Options,
				SnapLayer = SnapLayer,
				ObjectOffsetX = SnapToObjectOffsetXValue,
				ObjectOffsetZ = SnapToObjectOffsetZValue,
				SnapHeightValue = SnapToHeightValue,
				SnapXValue = SnapToXValue,
				SnapZValue = SnapToZValue,
				SphereCollision = SphereCollision,
				ConvexCollision = ConvexCollision,
				ConvexClean = ConvexClean,
				ConvexSimplify = ConvexSimplify,
				ConcaveCollision = ConcaveCollision,
				SnapToObject = SnapToObject,
				SnapToHeight = SnapToHeight,
				SnapToX = SnapToX,
				SnapToZ = SnapToZ,
				DistanceToLeft = leftDistance,
				DistanceToBottom = bottomDistance,
				DistanceToRight = rightDistance,
				DistanceToTop = topDistance,
			};

			// Add all children
			for (int i = 0; i < _Paths.Count; i++)
			{
				Vector3 Origin = _Origins[i];
				Vector3 Rotation = _Rotations[i];
				Vector3 Scale = _Scales[i];

				Transform3D transform = new(Basis.Identity, Vector3.Zero)
				{
					Origin = Origin,
				};

				GodotObject resource = GD.Load(_Paths[i]);

				if (resource is Mesh mesh)
				{
					AsMeshInstance3D asMeshInstance3D = new()
					{
						Name = _Paths[i] + "/" + i,
						Mesh = mesh,
						Transform = transform,
						RotationDegrees = Rotation,
						Scale = Scale,
						Floating = true,
						SpawnSettings = _Options[i]
					};

					group.AddChild(asMeshInstance3D);
				}
				else if (resource is PackedScene _scene)
				{
					Node node = _scene.Instantiate();
					AsNode3D node3D = new()
					{
						Name = _Paths[i] + "/" + i,
						Transform = transform,
						RotationDegrees = Rotation,
						Scale = Scale,
						Floating = true,
						SpawnSettings = _Options[i]
					};
					group.AddChild(node3D);

					foreach (MeshInstance3D child in node.GetChildren())
					{
						child.Owner = null;
						node.RemoveChild(child);
						
						AsMeshInstance3D asMeshInstance3D = new()
						{
							Name = child.Name,
							Mesh = child.Mesh,
							Floating = true,
							SpawnSettings = new()
						};
						
						node3D.AddChild(asMeshInstance3D);
						asMeshInstance3D.Owner = group.Owner;
					}

				}
			}

			return group;
		}
	}
}