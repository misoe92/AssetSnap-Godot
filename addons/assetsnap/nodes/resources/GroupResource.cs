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
		public float ObjectOffsetX { get; set; } = 0.0f;
		
		[Export]
		public float ObjectOffsetZ { get; set; } = 0.0f;
		
		[Export]
		public float SnapHeightValue { get; set; } = 0.0f;
		
		[Export]
		public float SnapXValue { get; set; } = 0.0f;
		
		[Export]
		public float SnapZValue { get; set; } = 0.0f;
		
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
		
		public void EachProperty( Action<string, Variant> action ) 
		{
			Godot.Collections.Array<string> properties = new()
			{
				"SnapLayer",
				"ObjectOffsetX",
				"ObjectOffsetZ",
				"SnapHeightValue",
				"SnapXValue",
				"SnapZValue",
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
			
			foreach( string propertyName in properties ) 
			{
				// Get the property info using reflection
				PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (property != null)
				{
					// Get the value of the property
					object value = property.GetValue(this);
					
					if( value is bool boolVal ) 
					{
						// Call the provided action with the property name and its value
						action(propertyName, boolVal);
					}

					if( value is float floatVal ) 
					{
						// Call the provided action with the property name and its value
						action(propertyName, floatVal);
					}
					
					if( value is float intVal ) 
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
		
		public void AddChildren( AsGrouped3D group, Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> childOptions ) 
		{
			for( int i = 0; i < _Paths.Count; i++ ) 
			{
				Vector3 Origin = _Origins[i];
				Vector3 Rotation = _Rotations[i];
				Vector3 Scale = _Scales[i];

				Transform3D transform = new(Basis.Identity, Vector3.Zero)
				{
					Origin = Origin,
				};
				
				AsMeshInstance3D asMeshInstance3D = new()
				{
					Name = _Paths[i] + "/" + i,
					Mesh = GD.Load<Mesh>(_Paths[i]),
					Transform = transform,
					RotationDegrees = Rotation,
					Scale = Scale,
					Floating = true,
					SpawnSettings = childOptions[i]
				};

				group.AddChild(asMeshInstance3D);
				asMeshInstance3D.Owner = null != group.Owner ? group.Owner : null;
			}
		}
		
		public void AddCollidingChildren( AsGrouped3D group, Godot.Collections.Array<Godot.Collections.Dictionary<string, Variant>> childOptions) 
		{
			for( int i = 0; i < _Paths.Count; i++ ) 
			{
				Vector3 Origin = _Origins[i];
				Vector3 Rotation = _Rotations[i];
				Vector3 Scale = _Scales[i];

				Transform3D transform = new(Basis.Identity, Vector3.Zero)
				{
					Origin = Origin,
				};

				Mesh mesh = GD.Load<Mesh>(_Paths[i]);
				
				AsStaticBody3D staticBody3D = new()
				{
					Name = _Paths[i] + "/" + i,
					// Mesh = mesh,
					// MeshName = mesh.ResourceName,
					Transform = transform,
					// InstanceTransform = transform,
					// InstanceRotation = Rotation,
					// InstanceScale = Scale,
					// InstanceOwner = null != group.Owner ? group.Owner : null,
					// InstanceSpawnSettings = childOptions[i]
				};

				group.AddChild(staticBody3D);
				staticBody3D.Owner = null != group.Owner ? group.Owner : null;
				int TypeState = 0;
				int CollisionType = 0;
				
				bool IsChildConvex = childOptions[i].ContainsKey("ConvexCollision") ? childOptions[i]["ConvexCollision"].As<bool>() : false;
				bool IsChildConvexClean = childOptions[i].ContainsKey("ConvexClean") ? childOptions[i]["ConvexClean"].As<bool>() : false;
				bool IsChildConvexSimplify = childOptions[i].ContainsKey("ConvexSimplify") ? childOptions[i]["ConvexSimplify"].As<bool>() : false;
				bool IsChildConcave = childOptions[i].ContainsKey("ConcaveCollision") ? childOptions[i]["ConcaveCollision"].As<bool>() : false;
				bool IsChildSphere = childOptions[i].ContainsKey("SphereCollision") ? childOptions[i]["SphereCollision"].As<bool>() : false;

				if (
					true == group.ConvexCollision &&
					false == IsChildConvex &&
					false == IsChildConcave &&
					false == IsChildSphere ||
					true == IsChildConvex	
				) 
				{
					TypeState = 1;
					
					if(
						true == group.ConvexClean &&
						false == group.ConvexSimplify &&
						false == IsChildConvex ||
						true == IsChildConvexClean &&
						true == IsChildConvexSimplify
					) 
					{
						CollisionType = 1;	
					}
					else if( 
						false == group.ConvexClean &&
						true == group.ConvexSimplify &&
						false == IsChildConvex ||
						false == IsChildConvexClean &&
						true == IsChildConvexSimplify
					) 
					{
						CollisionType = 2;	
					}
					else if( 
						true == group.ConvexClean &&
						true == group.ConvexSimplify &&
						false == IsChildConvex ||
						true == IsChildConvexClean &&
						true == IsChildConvexSimplify
					) 
					{
						CollisionType = 3;	
					}
				}
				else if( 
					true == group.ConcaveCollision &&
					false == IsChildConvex &&
					false == IsChildConcave &&
					false == IsChildSphere ||
					true == IsChildConcave	
				) 
				{
					TypeState = 2;
				}
				else if(
					true == group.SphereCollision &&
					false == IsChildConvex &&
					false == IsChildConcave &&
					false == IsChildSphere ||
					true == IsChildSphere	
				) 
				{
					TypeState = 3;
				}
				
				staticBody3D.Initialize();
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
			foreach ( ( int i, Vector3 child ) in _Origins)
			{
				if( child.X < leftDistance ) 
				{
					leftDistance = child.X;
				}
				
				if( child.X > rightDistance ) 
				{
					rightDistance = child.X;
				}
				
				if( child.Z < bottomDistance ) 
				{
					bottomDistance = child.Z;
				}
				
				if( child.Z > topDistance ) 
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
				ObjectOffsetX = ObjectOffsetX,
				ObjectOffsetZ = ObjectOffsetZ,
				SnapHeightValue = SnapHeightValue,
				SnapXValue = SnapXValue,
				SnapZValue = SnapZValue,
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
			for( int i = 0; i < _Paths.Count; i++ ) 
			{
				Vector3 Origin = _Origins[i];
				Vector3 Rotation = _Rotations[i];
				Vector3 Scale = _Scales[i];

				Transform3D transform = new(Basis.Identity, Vector3.Zero)
				{
					Origin = Origin,
				};
				
				AsMeshInstance3D asMeshInstance3D = new()
				{
					Name = _Paths[i] + "/" + i,
					Mesh = GD.Load<Mesh>(_Paths[i]),
					Transform = transform,
					RotationDegrees = Rotation,
					Scale = Scale,
					Floating = true,
					SpawnSettings = _Options[i]
				};

				group.AddChild(asMeshInstance3D);
			}

			return group;
		}
	}
}