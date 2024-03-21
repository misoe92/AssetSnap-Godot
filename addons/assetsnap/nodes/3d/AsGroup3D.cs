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

using System;
using System.Collections.Generic;
using AssetSnap.Front.Nodes;
using Godot;

[Tool]
public partial class AsGroup3D : Node3D
{
	/** Private **/
	protected Node _SceneRoot;

	[ExportCategory("Rotation Randomnization")]
	private bool _RandomnizeRotation = false;
	[Export]
	public bool RandomnizeRotation
	{
		get => _RandomnizeRotation;
		set 
		{
			_RandomnizeRotation = value;
			Randomnize();
			NotifyPropertyListChanged();
		}
	}
		
	private bool _ExcludeRotationOnX = false;
	[Export]
	public bool ExcludeRotationOnX
	{
		get => _ExcludeRotationOnX;
		set 
		{
			_ExcludeRotationOnX = value;
			Randomnize();
		}
	}
		
	private bool _ExcludeRotationOnY = false;
	[Export]
	public bool ExcludeRotationOnY
	{
		get => _ExcludeRotationOnY;
		set 
		{
			_ExcludeRotationOnY = value;
			Randomnize();
		}
	}
	
	private bool _ExcludeRotationOnZ = false;
	[Export]
	public bool ExcludeRotationOnZ
	{
		get => _ExcludeRotationOnZ;
		set 
		{
			_ExcludeRotationOnZ = value;
			Randomnize();
		}
	}
	
	private int _MinRotationAngle = 0;
	[Export]
	public int MinRotationAngle 
	{
		get => _MinRotationAngle;
		set 
		{
			_MinRotationAngle = value;
			Randomnize();
		}
	}
	
	private int _MaxRotationAngle = 0;
	[Export]
	public int MaxRotationAngle 
	{
		get => _MaxRotationAngle;
		set 
		{
			_MaxRotationAngle = value;
			Randomnize();
		}
	}

	[ExportCategory("Scale Randomnization")]
	private bool _RandomnizeScale = false;
	[Export]
	public bool RandomnizeScale
	{
		get => _RandomnizeScale;
		set 
		{
			_RandomnizeScale = value;
			Randomnize();
			NotifyPropertyListChanged();
		}
	}
	
	private bool _ExcludeScaleOnX = false;
	[Export]
	public bool ExcludeScaleOnX
	{
		get => _ExcludeScaleOnX;
		set 
		{
			_ExcludeScaleOnX = value;
			Randomnize();
		}
	}
		
	private bool _ExcludeScaleOnY = false;
	[Export]
	public bool ExcludeScaleOnY
	{
		get => _ExcludeScaleOnY;
		set 
		{
			_ExcludeScaleOnY = value;
			Randomnize();
		}
	}
	
	private bool _ExcludeScaleOnZ = false;
	[Export]
	public bool ExcludeScaleOnZ
	{
		get => _ExcludeScaleOnZ;
		set 
		{
			_ExcludeScaleOnZ = value;
			Randomnize();
		}
	}
	
	private double _MinScale = 1.00;
	[Export]
	public double MinScale 
	{
		get => _MinScale;
		set 
		{
			_MinScale = value;
			Randomnize();
		}
	}
	
	private double _MaxScale = 1.00;
	[Export]
	public double MaxScale 
	{
		get => _MaxScale;
		set 
		{
			_MaxScale = value;
			Randomnize();
		}
	}

	protected Vector3[] _RotationBuffer;
	protected Vector3[] _ScaleBuffer;
	
	[Signal]
	public delegate void LoadedEventHandler();

	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		Godot.Collections.Array RotationProperties = new()
		{
			"ExcludeRotationOnX",
			"ExcludeRotationOnY",
			"ExcludeRotationOnZ",
			"MinRotationAngle",
			"MaxRotationAngle",
		};
		Godot.Collections.Array ScaleProperties = new()
		{
			"ExcludeScaleOnX",
			"ExcludeScaleOnY",
			"ExcludeScaleOnZ",
			"MinScale",
			"MaxScale",
		};
		
		if( _RandomnizeRotation == false && property.ContainsKey("name") && RotationProperties.Contains( property["name"].As<string>())) 
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if( _RandomnizeRotation == true && property.ContainsKey("name") && RotationProperties.Contains( property["name"].As<string>())) 
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
			property["usage"] = (int)usage;
		}
		
		if( _RandomnizeScale == false && property.ContainsKey("name") && ScaleProperties.Contains( property["name"].As<string>())) 
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
			property["usage"] = (int)usage;
		}
		else if( _RandomnizeScale == true && property.ContainsKey("name") && ScaleProperties.Contains( property["name"].As<string>())) 
		{
			var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ScriptVariable;
			property["usage"] = (int)usage;
		}
		
		base._ValidateProperty(property);
	}
	
	public AsGroup3D()
	{
		Name = "AsGroup";
		SetMeta("AsGroup", true);
	}
	
	protected void Randomnize() 
	{
		if( GetChildCount() == 0 ) 
		{
			return;
		}
		
		var _child = GetChild(0);
		_RotationBuffer = Array.Empty<Vector3>();
		_ScaleBuffer = Array.Empty<Vector3>();
		
		if (_child is MultiMeshInstance3D childMulti)
		{
			MultiMesh _MultiMesh = childMulti.Multimesh;
			int instanceCount = _MultiMesh.InstanceCount;
			
			if( _RandomnizeRotation == false && _RandomnizeScale == false ) 
			{
				for (int i = 0; i < instanceCount; i++)
				{
					// Get the transform of the current instance
					Transform3D transform = _MultiMesh.GetInstanceTransform(i);
					
					transform = Transform3D.Identity.Translated(transform.Origin)
										.Rotated(Vector3.Up, Mathf.DegToRad(0))
										.Rotated(Vector3.Right, Mathf.DegToRad(0))
										.Rotated(Vector3.Forward, Mathf.DegToRad(0))
										.Scaled(new Vector3(1, 1, 1));
					
					_MultiMesh.SetInstanceTransform(i, transform);
						
					AsStaticBody3D _body = GetChild<AsStaticBody3D>(i + 1);
					_body.Scale = new Vector3(1, 1, 1);
				}
				return;
			}

			for (int i = 0; i < instanceCount; i++)
			{
				float scale = (float)GD.RandRange(MinScale, MaxScale);
				Vector3 Scale = new Vector3(1,1,1);
				Vector3 Rot = new Vector3(0,0,0);
				// Get the transform of the current instance
				Transform3D transform = _MultiMesh.GetInstanceTransform(i);
				AsStaticBody3D _body = GetChild<AsStaticBody3D>(i + 1);
				
				if( _RandomnizeScale && false == _ExcludeScaleOnX ) 
				{
					Scale.X = scale;
				}
				
				if( _RandomnizeScale && false == _ExcludeScaleOnY ) 
				{
					Scale.Y = scale;
				}
				
				if( _RandomnizeScale && false == _ExcludeScaleOnZ ) 
				{
					Scale.Z = scale;
				}
				
				if( _RandomnizeRotation && false == _ExcludeRotationOnX ) 
				{
					float rotationX = (float)GD.RandRange(MinRotationAngle, MaxRotationAngle);
					Rot.X = rotationX;
				}
				
				if( _RandomnizeRotation && false == _ExcludeRotationOnY ) 
				{
					float rotationY = (float)GD.RandRange(MinRotationAngle, MaxRotationAngle);
					Rot.Y = rotationY;
				}
				
				if( _RandomnizeRotation && false == _ExcludeRotationOnZ ) 
				{
					float rotationZ = (float)GD.RandRange(MinRotationAngle, MaxRotationAngle);
					Rot.Z = rotationZ;
				}

				if( _RandomnizeRotation ) 
				{
					transform = UpdateRotation(transform, Rot);
					_body.RotationDegrees = Rot;
				}

				if( _RandomnizeScale ) 
				{
					transform = UpdateScale(transform, Scale);
					_body.Scale = Scale;
				}
				
				_MultiMesh.SetInstanceTransform(i, transform);
			}
		}
		else 
		{
			if( _RandomnizeRotation == false && _RandomnizeScale == false ) 
			{
				foreach( Node3D child in GetChildren() ) 
				{
					if( IsInstanceValid( child ) ) 
					{
						child.RotationDegrees = new Vector3(0,0,0);
						child.Scale = new Vector3(1,1,1);
					}
				}
				return;
			}
			
			foreach( Node3D child in GetChildren() ) 
			{
				if( IsInstanceValid( child ) ) 
				{
					float scale = (float)GD.RandRange(MinScale, MaxScale);
					Vector3 Rot = child.RotationDegrees;
					Vector3 Scale = child.Scale;
					
					if( false == _ExcludeRotationOnX ) 
					{
						float rotationX = (float)GD.RandRange(MinRotationAngle, MaxRotationAngle);
						Rot.X = rotationX;
					}
					
					if( false == _ExcludeRotationOnY ) 
					{
						float rotationY = (float)GD.RandRange(MinRotationAngle, MaxRotationAngle);
						Rot.Y = rotationY;
					}
					
					if( false == _ExcludeRotationOnZ ) 
					{
						float rotationZ = (float)GD.RandRange(MinRotationAngle, MaxRotationAngle);
						Rot.Z = rotationZ;
					}
					
					if( false == _ExcludeScaleOnX ) 
					{
						Scale.X = scale;
					}
					
					if( false == _ExcludeScaleOnY ) 
					{
						Scale.Y = scale;
					}
					
					if( false == _ExcludeScaleOnZ ) 
					{
						Scale.Z = scale;
					}
					
					if( _ExcludeRotationOnX && _ExcludeRotationOnY && _ExcludeRotationOnZ ) 
					{
						Rot.X = 0;
						Rot.Y = 0;
						Rot.Z = 0;
					}
					
					if( _ExcludeScaleOnX && _ExcludeScaleOnY && _ExcludeScaleOnZ ) 
					{
						Scale.X = 1;
						Scale.Y = 1;
						Scale.Z = 1;
					}

					child.RotationDegrees = Rot;
					child.Scale = Scale;
				}
			}
		}
	}
	
	private void _UpdateMultiCollisions(MultiMeshInstance3D _MultiMeshInstance)
	{
		int instanceCount = _MultiMeshInstance.Multimesh.InstanceCount;
		
		for( int i = 0; i < instanceCount; i++ )
		{
			if( GetChildCount() > i + 1 ) 
			{
				AsStaticBody3D _body = GetChild<AsStaticBody3D>(i + 1);
				_body.Scale = _MultiMeshInstance.Multimesh.GetInstanceTransform(i).Basis.Scale;
			}
		}
	}
	
	private Transform3D UpdateRotation( Transform3D transform, Vector3 RotationDegrees ) 
	{
		Vector3 currentTranslation = transform.Origin;
		Transform3D newTransform = new(Basis.Identity, Vector3.Up);

		// Save the original translation and scale
		newTransform = newTransform.Rotated(Vector3.Right, Mathf.DegToRad(RotationDegrees.X));
		newTransform = newTransform.Rotated(Vector3.Up, Mathf.DegToRad(RotationDegrees.Y));
		newTransform = newTransform.Rotated(Vector3.Forward, Mathf.DegToRad(RotationDegrees.Z));
		
		// Set back the original translation and scale
		newTransform.Origin = currentTranslation;

		List<Vector3> _List = new(_RotationBuffer)
		{
			RotationDegrees
		};
		_RotationBuffer = _List.ToArray();

		return newTransform;
	}
	
	private Transform3D UpdateScale(Transform3D transform, Vector3 Scale)
	{
		// Save the original translation
		Vector3 currentTranslation = transform.Origin;
		
		if( _RandomnizeRotation == false ) 
		{
			transform = new(Basis.Identity, Vector3.Up);
		}
		
		// Create a new basis with the desired scale
		transform = transform.Scaled(Scale);
		transform.Origin = currentTranslation;
	
		List<Vector3> _List = new(_ScaleBuffer)
		{
			Scale
		};
		_ScaleBuffer = _List.ToArray();

		return transform;
	}
	
	protected void RandomnizeChildrenRotation()
	{
		var _child = GetChild(0);

		if (_child is MultiMeshInstance3D childMulti ) 
		{
			MultiMesh _MultiMesh = childMulti.Multimesh;
			int instanceCount = _MultiMesh.InstanceCount;
			
			if( _RandomnizeRotation == false ) 
			{
				for (int i = 0; i < instanceCount; i++)
				{
					// Get the transform of the current instance
					Transform3D transform = _MultiMesh.GetInstanceTransform(i);
					Basis Basis = new Basis(Vector3.Up, Mathf.DegToRad(0)) *
							 new Basis(Vector3.Right, Mathf.DegToRad(0)) *
							 new Basis(Vector3.Forward, Mathf.DegToRad(0));
					
					transform.Basis = Basis;
					
					_MultiMesh.SetInstanceTransform(i, transform);
				}
				return;
			}
		
			// Iterate through all instances
			for (int i = 0; i < instanceCount; i++)
			{
				// Get the transform of the current instance
				Transform3D transform = _MultiMesh.GetInstanceTransform(i);
				Basis currentRotation = transform.Basis;
				Basis BasisX;
				Basis BasisY;
				Basis BasisZ;

				if( false == _ExcludeRotationOnX ) 
				{
					float rotationX = (float)GD.RandRange(MinRotationAngle, MaxRotationAngle);
					BasisX = new Basis(Vector3.Up, Mathf.DegToRad(rotationX));
					
					Vector3 XRot = currentRotation.X;
					Vector3 Rot = BasisX.X;

					Rot.X = XRot.X;
				}
				else 
				{
					BasisX = new Basis(Vector3.Up, Mathf.DegToRad(0));
				}
				
				if( false == _ExcludeRotationOnY ) 
				{
					float rotationY = (float)GD.RandRange(MinRotationAngle, MaxRotationAngle);
					BasisY = new Basis(Vector3.Right, Mathf.DegToRad(rotationY));
					
					Vector3 YRot = currentRotation.Y;
					Vector3 Rot = BasisY.Y;

					Rot.Y = YRot.Y;
				}
				else 
				{
					BasisY = new Basis(Vector3.Right, Mathf.DegToRad(0));
				}
				
				if( false == _ExcludeRotationOnZ ) 
				{
					float rotationZ = (float)GD.RandRange(MinRotationAngle, MaxRotationAngle);
					BasisZ = new Basis(Vector3.Forward, Mathf.DegToRad(rotationZ));
					
					Vector3 ZRot = currentRotation.Z;
					Vector3 Rot = BasisZ.Z;

					Rot.Z = ZRot.Z;
				}
				else 
				{
					BasisZ = new Basis(Vector3.Forward, Mathf.DegToRad(0));
				}
				
				Basis _Basis = BasisX *
					BasisY *
					BasisZ;
					
				transform.Basis = _Basis;

				// Apply the modified transform back to the instance
				_MultiMesh.SetInstanceTransform(i, transform);
			}
		}
		else 
		{
			if( _RandomnizeRotation == false ) 
			{
				foreach( Node3D child in GetChildren() ) 
				{
					if( IsInstanceValid( child ) ) 
					{
						child.RotationDegrees = new Vector3(0,0,0);
					}
				}
				return;
			}
			
			foreach( Node3D child in GetChildren() ) 
			{
				if( IsInstanceValid( child ) ) 
				{
					Vector3 Rot = child.RotationDegrees;
					
					if( false == _ExcludeRotationOnX ) 
					{
						float rotationX = (float)GD.RandRange(MinRotationAngle, MaxRotationAngle);
						Rot.X = rotationX;
					}
					
					if( false == _ExcludeRotationOnY ) 
					{
						float rotationY = (float)GD.RandRange(MinRotationAngle, MaxRotationAngle);
						Rot.Y = rotationY;
					}
					
					if( false == _ExcludeRotationOnZ ) 
					{
						float rotationZ = (float)GD.RandRange(MinRotationAngle, MaxRotationAngle);
						Rot.Z = rotationZ;
					}
					
					if( _ExcludeRotationOnX && _ExcludeRotationOnY && _ExcludeRotationOnZ ) 
					{
						Rot.X = 0;
						Rot.Y = 0;
						Rot.Z = 0;
					}

					child.RotationDegrees = Rot;
				}
			}
		}
	}
	
	protected void RandomnizeChildrenScale()
	{
		var _child = GetChild(0);

		if (_child is MultiMeshInstance3D childMulti ) 
		{
			MultiMesh _MultiMesh = childMulti.Multimesh;
			int instanceCount = _MultiMesh.InstanceCount;

			if( _RandomnizeScale == false ) 
			{
				for (int i = 0; i < instanceCount; i++)
				{
					Transform3D transform = _MultiMesh.GetInstanceTransform(i);
					transform.ScaledLocal(new Vector3(1, 1, 1));
					_MultiMesh.SetInstanceTransform(i, transform);
				}
				return;
			}
		
			// Iterate through all instances
			for (int i = 0; i < instanceCount; i++)
			{
				Godot.Collections.Dictionary<string, Vector3> _Scale = new();
				// Get the transform of the current instance
				Transform3D transform = _MultiMesh.GetInstanceTransform(i);
				float scale = (float)GD.RandRange(MinScale, MaxScale);
				Basis currentRotation = transform.Basis;

				if( false == _ExcludeScaleOnX ) 
				{
					_Scale.Add("X", new Vector3(scale,0,0));
					Vector3 Rot = currentRotation.X;
					Rot.X = 1;
					currentRotation.X = Rot;
				}
				
				if( false == _ExcludeScaleOnY ) 
				{
					_Scale.Add("Y", new Vector3(0,scale,0));
					Vector3 Rot = currentRotation.Y;
					Rot.Y = 1;
					currentRotation.Y = Rot;
				}
				
				if( false == _ExcludeScaleOnZ ) 
				{
					_Scale.Add("Z", new Vector3(0,0,scale));
					Vector3 Rot = currentRotation.Z;
					Rot.Z = 1;
					currentRotation.Z = Rot;
				}

				transform.Basis.X = currentRotation.X * _Scale["X"];
				transform.Basis.Y = currentRotation.Y * _Scale["Y"];
				transform.Basis.Z = currentRotation.Z * _Scale["Z"];

				// Apply the modified transform back to the instance
				_MultiMesh.SetInstanceTransform(i, transform);
			}
		}
		else 
		{
			if( _RandomnizeScale == false ) 
			{
				foreach (Node3D child in GetChildren())
				{
					child.Scale = new Vector3(1, 1, 1);
				}
				return;
			}
			
			foreach( Node3D child in GetChildren() ) 
			{
				if( IsInstanceValid( child ) ) 
				{
					float scale = (float)GD.RandRange(MinScale, MaxScale);

					Vector3 Scale = child.Scale;
					
					if( false == _ExcludeScaleOnX ) 
					{
						Scale.X = scale;
					}
					
					if( false == _ExcludeScaleOnY ) 
					{
						Scale.Y = scale;
					}
					
					if( false == _ExcludeScaleOnZ ) 
					{
						Scale.Z = scale;
					}

					if( _ExcludeScaleOnX && _ExcludeScaleOnY && _ExcludeScaleOnZ ) 
					{
						Scale.X = 1;
						Scale.Y = 1;
						Scale.Z = 1;
					}
					
					child.Scale = Scale;
				}
			}
		}
	}
	
	protected void ClearCurrentChildren()
	{
		foreach( Node child in GetChildren())
		{
			if( IsInstanceValid( child ) ) 
			{
				RemoveChild(child);
				child.QueueFree();
			}
		}
	}

	public override void _ExitTree()
	{
		if( null != _SceneRoot) 
		{
			_SceneRoot = null;
		}

		ClearCurrentChildren();
		
		base._ExitTree();
	}
}