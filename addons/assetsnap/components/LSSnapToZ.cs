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

namespace AssetSnap.Front.Components
{
	using System;
	using AssetSnap.Component;
	using AssetSnap.Front.Nodes;
	using Godot;

	public partial class LSSnapToZ : LibraryComponent
	{
		private readonly string _Title = "Snap Z";
		private readonly string _CheckboxTitle = "Snap To Z";
		private readonly string _GlueTitle = "Use glue";
		private readonly string _GlueTooltip = "Will glue the model to the X axis set here, which will only make it able to move on 2 vectors. (Y,Z)";
		private float CurrentOpacity;
		private bool Exited = false;
		
		private MarginContainer _MarginContainer;
		private VBoxContainer _InnerContainer;
		private	Label _Label;
		private CheckBox _Checkbox;
		private SpinBox _ValueSpinBox;
		private MarginContainer _ValueMarginContainer;
		private VBoxContainer _ValueContainer;
		private CheckBox _GlueCheckbox;
					
		/** Boundary Box **/
		private StaticBody3D _BoundaryBody;
		private MeshInstance3D _BoundaryMeshInstance;
		private BoxMesh _BoundaryBoxMesh;
		private CollisionShape3D _BoundaryCollision;
		private BoxShape3D _BoundaryCollisionBox;
		private Shader _BoundaryGrid;
		private ShaderMaterial _BoundaryMaterial;
		
		/** Callables **/
		private Callable? _CheckboxCallable;
		private Callable? _GlueCheckboxCallable;
		private Callable? _SpinBoxCallable;
		
		public bool state = false;
		public float SnapZValue = 0.0f;
		public bool UsingGlue = false;
		
		/*
		** Constructor of the component
		** 
		** @return void
		*/
		public LSSnapToZ()
		{
			Name = "LSSnapToZ";
			// _include = false; 
		}
		
		/*
		** Initializing the component
		** 
		** @return void
		*/
		public override void Initialize()
		{ 
			if( Container is VBoxContainer BoxContainer ) 
			{
				_InitializeCheckBox(BoxContainer);
				_InitializeGlue(BoxContainer);
				_InitializeSpinBox(BoxContainer);
			}
		
			_InitializeSnapBoundary();
		}
		
		/*
		** Updates states of various of internal fields
		** based on certain scenario
		** 
		** @return void
		*/
		public override void _Process(double delta)
		{
			if( false == IsInstanceValid(_Checkbox) ) 
			{
				return;
			}
			
			if( null == _GlobalExplorer || null == _GlobalExplorer._Plugin || null == _GlobalExplorer.Settings) 
			{
				return;
			}
			
			if( _Checkbox != null && state && _Checkbox.ButtonPressed == false ) 
			{
				_Checkbox.ButtonPressed = true;
			}
			else if( _Checkbox != null && false == state && _Checkbox.ButtonPressed == true ) 
			{
				_Checkbox.ButtonPressed = false;
			}

			if( _GlueCheckbox != null && UsingGlue && _GlueCheckbox.ButtonPressed == false ) 
			{
				_GlueCheckbox.ButtonPressed = true;
			}
			else if( _GlueCheckbox != null && false == UsingGlue && _GlueCheckbox.ButtonPressed == true ) 
			{
				_GlueCheckbox.ButtonPressed = false;
			}
			
			if( _ValueMarginContainer != null ) 
			{
				if( state && false == _ValueMarginContainer.Visible ) 
				{
					_ValueMarginContainer.Visible = true;
				}
				else if( false == state && true == _ValueMarginContainer.Visible ) 
				{
					_ValueMarginContainer.Visible = false;
				}
			}
			if( _GlueCheckbox != null ) 
			{
				if( state && false == _GlueCheckbox.Visible ) 
				{
					_GlueCheckbox.Visible = true;
				}
				else if( false == state && true == _GlueCheckbox.Visible ) 
				{
					_GlueCheckbox.Visible = false;
				}
			}
		
			float BoundaryOpacity = _GlobalExplorer.Settings.GetKey("boundary_box_opacity").As<float>();
			if( BoundaryOpacity != CurrentOpacity ) 
			{
				UpdateOpacity(BoundaryOpacity);				
			}
			
			if( null == _BoundaryBody ) 
			{
				return;
			}
			
			if( true == state && null == _BoundaryBody.GetParent() ) 
			{
				_GlobalExplorer._Plugin.AddChild(_BoundaryBody);
			}
			else if( false == state && null != _BoundaryBody.GetParent() ) 
			{
				_GlobalExplorer._Plugin.RemoveChild(_BoundaryBody);
			}

			Transform3D Trans = _BoundaryBody.Transform;
			Trans.Origin.Z = SnapZValue;
			_BoundaryBody.Transform = Trans;
		}
		
		/*
		** Initializing the checkbox that holds
		** the state value of the component
		** 
		** @return void
		*/
		private void _InitializeCheckBox( VBoxContainer BoxContainer ) 
		{
			_MarginContainer = new();
			_InnerContainer = new();
			_CheckboxCallable = new(this, "_OnCheckboxPressed");
			
			_Checkbox = new()
			{
				Text = _CheckboxTitle
			};
			
			_Label = new()
			{
				ThemeTypeVariation = "HeaderSmall",
				Text = _Title
			};
			
			_MarginContainer.AddThemeConstantOverride("margin_left", 10); 
			_MarginContainer.AddThemeConstantOverride("margin_right", 10);
			_MarginContainer.AddThemeConstantOverride("margin_top", 2);
			_MarginContainer.AddThemeConstantOverride("margin_bottom", 2);
			 
			if( _CheckboxCallable is Callable _callable ) 
			{
				_Checkbox.Connect(CheckBox.SignalName.Pressed,_callable);
			}
			
			_InnerContainer.AddChild(_Label);
			_InnerContainer.AddChild(_Checkbox);
			_MarginContainer.AddChild(_InnerContainer);
			BoxContainer.AddChild(_MarginContainer);
		}
		
		/*
		** Initializing the checkbox that holds
		** the state value of the using glue option
		** 
		** @return void
		*/
		private void _InitializeGlue( VBoxContainer BoxContainer ) 
		{
			_GlueCheckboxCallable = new(this, "_OnGlueCheckboxPressed");
			
			_GlueCheckbox = new()
			{
				Text = _GlueTitle,
				TooltipText = _GlueTooltip,
			};
			
			if( _GlueCheckboxCallable is Callable _callable) 
			{
				_GlueCheckbox.Connect(CheckBox.SignalName.Pressed,_callable);
			}
			
			_InnerContainer.AddChild(_GlueCheckbox);
		}
		
		/*
		** Initializing the spinbox that holds
		** the value of the component
		** 
		** @return void
		*/
		private void _InitializeSpinBox( VBoxContainer BoxContainer ) 
		{
			_ValueMarginContainer = new();
			_ValueContainer = new();
			
			_SpinBoxCallable = new(this, "_OnSpinBoxValueChange");
			
			_ValueSpinBox = new() 
			{
				CustomMinimumSize = new Vector2(140, 20),
				MinValue = -200,
				Step = 0.01f
			};
			
			_ValueMarginContainer.AddThemeConstantOverride("margin_left", 10); 
			_ValueMarginContainer.AddThemeConstantOverride("margin_right", 10);
			_ValueMarginContainer.AddThemeConstantOverride("margin_top", 2);
			_ValueMarginContainer.AddThemeConstantOverride("margin_bottom", 2);
			
			if( _SpinBoxCallable is Callable _callable ) 
			{
				_ValueSpinBox.Connect(SpinBox.SignalName.ValueChanged,_callable);
			}
			
			_ValueContainer.AddChild(_ValueSpinBox);
			_ValueMarginContainer.AddChild(_ValueContainer);
			BoxContainer.AddChild(_ValueMarginContainer);
		}
		
		/*
		** Initializing the snap boundary
		** 
		** @return void
		*/
		public void _InitializeSnapBoundary()
		{
			try 
			{
				_BoundaryBody = new();
				_BoundaryMeshInstance = new();
				_BoundaryBoxMesh = new();
				_BoundaryCollision = new();
				_BoundaryCollisionBox = new();
				
				_BoundaryGrid = GD.Load<Shader>("res://addons/assetsnap/shaders/snap-grid.gdshader");
				_BoundaryMaterial = new();

				bool ShowSnapBoundary = _GlobalExplorer.Settings.GetKey("show_snap_boundary_box").As<bool>();
				AddCollisionBox(_BoundaryBody);
				if( true == ShowSnapBoundary ) 
				{
					AddBoundaryBox(_BoundaryBody);
				}
			}
			catch(Exception e ) 
			{
				GD.PushError(e.Message);	
			}
		}
		
		/*
		** Set the Z axis on the given Vector3 value
		** to that of our component value
		** 
		** @return Vector3
		*/
		public Vector3 ApplyGlue( Vector3 Origin )
		{
			Origin.Z = SnapZValue;
			return Origin;
		}
		
		/*
		** Checks if glue is being used
		** 
		** @return bool
		*/
		public bool IsUsingGlue()
		{
			return UsingGlue;
		}
		
		/*
		** Checks if the component is active
		** 
		** @return bool
		*/
		public bool IsActive()
		{
			return state == true;
		}
		
		/*
		** Sets up the collision box
		** 
		** @return void
		*/
		private void AddCollisionBox( StaticBody3D to )
		{
			_BoundaryCollisionBox.Size = new Vector3(1000.0f, 1000.0f, 0.3f);
			_BoundaryCollision.Shape = _BoundaryCollisionBox;
			
			to.AddChild(_BoundaryCollision); 
		}
		
		/*
		** Sets up the boundary box
		** 
		** @return void
		*/
		private void AddBoundaryBox( StaticBody3D to )
		{
			_BoundaryBoxMesh.Size = new Vector3(1000.0f, 1000.0f, 0.3f);

			_BoundaryMaterial.Shader = _BoundaryGrid;
			_BoundaryMaterial.SetShaderParameter("scale_0", 1024);
			_BoundaryMaterial.SetShaderParameter("scale_1", 1024);
			_BoundaryMaterial.SetShaderParameter("line_scale_0", 0.02f);
			_BoundaryMaterial.SetShaderParameter("line_scale_1", 0.01f);
			
			float ShowSnapBoundary = _GlobalExplorer.Settings.GetKey("boundary_box_opacity").As<float>();
			_BoundaryMaterial.SetShaderParameter("opacity", ShowSnapBoundary);
			
			_BoundaryMaterial.SetShaderParameter("color_0", new Color(Colors.Black.R, Colors.Black.G, Colors.Black.B, 0.1f));
			_BoundaryMaterial.SetShaderParameter("color_1", new Color(Colors.White)); 
			
			_BoundaryBoxMesh.Material = _BoundaryMaterial;
			_BoundaryMeshInstance.Mesh = _BoundaryBoxMesh;

			to.AddChild(_BoundaryMeshInstance);
		}
		
		/*
		** Updates the opacity of the boundary
		** 
		** @return void
		*/
		private void UpdateOpacity( float value )
		{
			if( _BoundaryMaterial != null) 
			{
				_BoundaryMaterial.SetShaderParameter("opacity", value);
				CurrentOpacity = value;
			}
		}
		
		/*
		** Updates spawn settings and the internal
		** value for snapping to the X Axis
		** 
		** @return void
		*/
		private void _OnCheckboxPressed()
		{
			state = !state;
			
			if( state ) 
			{
				UsingGlue = true;				
			}
			
			string key = "_LSSnapToZ.state";
			UpdateSpawnSettings(key, state);
		}
		
		/*
		** Updates spawn settings and the internal
		** value for using glue
		** 
		** @return void
		*/
		private void _OnGlueCheckboxPressed()
		{
			UsingGlue = !UsingGlue;
			
			string key = "_LSSnapToZ.UsingGlue";
			UpdateSpawnSettings(key, UsingGlue);
		}
		
		/*
		** Updates SpawnSettings on the model
		** when component value is changed
		** 
		** @return void
		*/
		private void _OnSpinBoxValueChange(float value)
		{
			SnapZValue = value;

			string key = "_LSSnapToZ.SnapZValue";
			UpdateSpawnSettings(key, SnapZValue);
		}
		
		/*
		** Resets the component
		** 
		** @return void
		*/
		public void Reset()
		{
			state = false;
			UsingGlue = false;
			SnapZValue = 0.0f;
		}
			
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			Exited = true;
			
			if( IsInstanceValid(_GlueCheckbox) && _GlueCheckbox != null && _GlueCheckboxCallable is Callable _callable ) 
			{
				if( _GlueCheckbox.IsConnected(CheckBox.SignalName.Pressed, _callable)) 
				{
					_GlueCheckbox.Disconnect(CheckBox.SignalName.Pressed, _callable);
				}
			}
			
			if( IsInstanceValid(_ValueSpinBox) && _ValueSpinBox != null && _SpinBoxCallable is Callable _SpinCallable ) 
			{
				if( _ValueSpinBox.IsConnected(SpinBox.SignalName.ValueChanged, _SpinCallable)) 
				{
					_ValueSpinBox.Disconnect(SpinBox.SignalName.ValueChanged, _SpinCallable);
				}
			}
			
			if( IsInstanceValid(_Checkbox) && _Checkbox != null && _CheckboxCallable is Callable _BoxCallable ) 
			{
				if( _Checkbox.IsConnected(CheckBox.SignalName.Pressed, _BoxCallable)) 
				{
					_Checkbox.Disconnect(CheckBox.SignalName.Pressed, _BoxCallable);
				}
			}
			
			if( IsInstanceValid(_ValueSpinBox) ) 
			{
				_ValueSpinBox.QueueFree();
				_ValueSpinBox = null;
			}
			
			if( IsInstanceValid(_ValueContainer) ) 
			{
				_ValueContainer.QueueFree();
				_ValueContainer = null; 
			}
			
			if( IsInstanceValid(_ValueMarginContainer) ) 
			{
				_ValueMarginContainer.QueueFree();
				_ValueMarginContainer = null;
			}
			
			if( IsInstanceValid(_GlueCheckbox) ) 
			{
				_GlueCheckbox.QueueFree();
				_GlueCheckbox = null;
			}
		
			if( IsInstanceValid(_Checkbox) ) 
			{
				_Checkbox.QueueFree();
				_Checkbox = null;
			}
			
			if( IsInstanceValid(_Label) ) 
			{
				_Label.QueueFree();
				_Label = null;
			}
			
			if( IsInstanceValid(_MarginContainer) ) 
			{
				_MarginContainer.QueueFree();
				_MarginContainer = null;
			}	
						
			if( IsInstanceValid(_BoundaryCollision) ) 
			{
				_BoundaryCollision.QueueFree();
				_BoundaryCollision = null;
			}
			
			if( IsInstanceValid(_BoundaryMeshInstance) ) 
			{
				_BoundaryMeshInstance.QueueFree();
				_BoundaryMeshInstance = null;
			}
			
			if( IsInstanceValid(_BoundaryBody) ) 
			{
				_BoundaryBody.QueueFree();
				_BoundaryBody = null;
			}
		}
	}
}