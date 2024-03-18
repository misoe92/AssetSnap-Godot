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
	using System.Reflection;
	using AssetSnap.Component;
	using Godot;

	public partial class LSSnapToHeight : LibraryComponent
	{
		/** Private **/
		private readonly string _Title = "Snap Height";
		private readonly string _CheckboxTitle = "Snap To Height";
		private readonly string _GlueTitle = "Use glue";
		private readonly string _GlueTooltip = "Will glue the model to the height set here, which will only make it able to move on 2 vectors. (X,Z)";
		private readonly string _NormalsTitle = "Align with normals";
		private readonly string _NormalsTooltip = "Will align the object with the normals of the snap point, rotating it to match the target rotation";
		private float CurrentOpacity;

		/** Nodes **/
		private MarginContainer _MarginContainer;
		private VBoxContainer _InnerContainer;
		private	Label _Label;
		private CheckBox _Checkbox;
		private MarginContainer _ValueMarginContainer;
		private VBoxContainer _ValueContainer;
		private SpinBox _ValueSpinBox;
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
		
		public float SnapHeight = 0.0f;
		public bool state = false;
		public bool UsingGlue = false;
		
		/*
		** Constructor of the component
		** 
		** @return void
		*/
		public LSSnapToHeight()
		{
			Name = "LSSnapToHeight";
			// _include = false;
		}
		
		/*
		** Initializes the component
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
		** Synchronizes states and values for various
		** of fields, and updates internal states if
		** its needed
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
			
			Variant snapBoundaryVariant = _GlobalExplorer.Settings.GetKey("show_snap_boundary_box");
			bool ShowSnapBoundary = snapBoundaryVariant.As<bool>();

			if( true == ShowSnapBoundary && false == IsInstanceValid(_BoundaryBody) ) 
			{
				_BoundaryBody = new();
				_BoundaryMeshInstance = new();
				_BoundaryBoxMesh = new();
				AddBoundaryBox(_BoundaryBody);
			}
			else if( false == ShowSnapBoundary && true == IsInstanceValid(_BoundaryBody) )
			{
				_BoundaryBody.QueueFree();	
			}
			
			if( _Checkbox != null &&  state && _Checkbox.ButtonPressed == false ) 
			{
				_Checkbox.ButtonPressed = true;
			}
			else if( _Checkbox != null &&  false == state && _Checkbox.ButtonPressed == true ) 
			{
				_Checkbox.ButtonPressed = false;
			}
			
			if( _GlueCheckbox != null &&  UsingGlue && _GlueCheckbox.ButtonPressed == false ) 
			{
				_GlueCheckbox.ButtonPressed = true;
			}
			else if( _GlueCheckbox != null &&  false == UsingGlue && _GlueCheckbox.ButtonPressed == true ) 
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
			
			if( true == IsInstanceValid(_BoundaryBody) ) 
			{
				if(true == state && null == _BoundaryBody.GetParent() ) 
				{
					_GlobalExplorer._Plugin.AddChild(_BoundaryBody);
				}
				else if(false == state && null != _BoundaryBody.GetParent() ) 
				{
					_GlobalExplorer._Plugin.RemoveChild(_BoundaryBody);
				}
				
				Transform3D Trans = _BoundaryBody.Transform;
				Trans.Origin.Y = SnapHeight;
				_BoundaryBody.Transform = Trans;
			}
		}

		/*
		** Initializes the Checkbox that
		** holds the Enabled/Disabled
		** state of the component
		** 
		** @return void
		*/
		private void _InitializeCheckBox( VBoxContainer BoxContainer ) 
		{
			_MarginContainer = new()
			{
				Name = "SnapToHeightMarginContainer",				
			};
			_InnerContainer = new()
			{
				Name = "SnapToHeightInnerContainer",				
			};
			_CheckboxCallable = new(this, "_OnCheckboxPressed");
			
			_Checkbox = new()
			{
				Name = "SnapToHeightCheckbox",
				Text = _CheckboxTitle
			};
			
			_Label = new()
			{
				Name = "SnapToHeightLabel",
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
		** Initializes the Checkbox that
		** holds the state of using glue
		** 
		** @return void
		*/
		private void _InitializeGlue( VBoxContainer BoxContainer ) 
		{
			_GlueCheckboxCallable = new(this, "_OnGlueCheckboxPressed");
			
			_GlueCheckbox = new()
			{
				Name = "SnapToHeightGlue",
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
		** Initializes SpinBox that
		** holds the current SnapHeight
		** 
		** @return void
		*/
		private void _InitializeSpinBox( VBoxContainer BoxContainer ) 
		{
			_ValueMarginContainer = new()
			{
				Name = "SnapToHeightSpinBoxMargin",
			};

			_ValueContainer = new()
			{
				Name = "SnapToHeightSpinBoxContainer",
			};
			
			_SpinBoxCallable = new(this, "_OnSpinBoxValueChange");
			
			_ValueSpinBox = new() 
			{
				Name = "SnapToHeightSpinBox",
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
		** Initializes snap boundary
		** 
		** @return void
		*/
		public void _InitializeSnapBoundary()
		{
			try 
			{
				_BoundaryBody = new()
				{
					Name = "SnapToHeightBoundaryBody"
				};
				_BoundaryMeshInstance = new()
				{
					Name = "SnapToHeightBoundaryMeshInstance"
				};
				_BoundaryBoxMesh = new();
				_BoundaryCollision = new()
				{
					Name = "SnapToHeightBoundaryCollision"
				};
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
		** Applies glue to the boundary's Y axis
		** 
		** @return Vector3
		*/
		public Vector3 ApplyGlue( Vector3 Origin )
		{
			Origin.Y = SnapHeight;
			return Origin;
		}
			  
		/*
		** Checks if glue is being used on this axis
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
		** Fetches the StaticBody3D
		** 
		** @return StaticBody3D
		*/
		public StaticBody3D GetNode()
		{
			return _BoundaryBody;
		}
				  
		/*
		** Adds the collision box
		** to the scene
		** 
		** @param StaticBody3D to
		** @return void
		*/
		private void AddCollisionBox( StaticBody3D to )
		{
			_BoundaryCollisionBox.Size = new Vector3(1000, 0.1f, 1000);
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
		private void AddBoundaryBox( StaticBody3D to )
		{
			_BoundaryBoxMesh.Size = new Vector3(1000, 0.3f, 1000);

			_BoundaryMaterial.Shader = _BoundaryGrid;
			_BoundaryMaterial.SetShaderParameter("scale_0", 1024);
			_BoundaryMaterial.SetShaderParameter("scale_1", 1024);
			_BoundaryMaterial.SetShaderParameter("line_scale_0", 0.02f);
			_BoundaryMaterial.SetShaderParameter("line_scale_1", 0.01f);
			
			float BoundaryOpacity = _GlobalExplorer.Settings.GetKey("boundary_box_opacity").As<float>();
			_BoundaryMaterial.SetShaderParameter("opacity", BoundaryOpacity);
			
			_BoundaryMaterial.SetShaderParameter("color_0", new Color(Colors.Black.R, Colors.Black.G, Colors.Black.B, 0.1f));
			_BoundaryMaterial.SetShaderParameter("color_1", new Color(Colors.White)); 
			
			_BoundaryBoxMesh.Material = _BoundaryMaterial;
			_BoundaryMeshInstance.Mesh = _BoundaryBoxMesh;

			to.AddChild(_BoundaryMeshInstance);
		}
			  
		/*
		** Updates the opacity of the boundary
		** 
		** @param float value
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
		** value for snapping to the Y Axis
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
			
			string key = "_LSSnapToHeight.state";
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
			
			string key = "_LSSnapToHeight.UsingGlue";
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
			SnapHeight = value;
						
			string key = "_LSSnapToHeight.SnapHeight";
			UpdateSpawnSettings(key, SnapHeight);
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
			SnapHeight = 0.0f;
			
		}
		
		public void PrintFields()
		{
			Type type = GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			Console.WriteLine($"Fields for class {type.Name}:");

			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(this);
				Console.WriteLine($"{field.Name}: {value}");
			}
		}
			
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/ 
		public override void _ExitTree()
		{
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
			}
			
			if( IsInstanceValid(_ValueContainer) ) 
			{
				_ValueContainer.QueueFree();
			}
			
			if( IsInstanceValid(_ValueMarginContainer) ) 
			{
				_ValueMarginContainer.QueueFree();
			}
			
			if( IsInstanceValid(_Checkbox) ) 
			{
				_Checkbox.QueueFree();
			}
			
			if( IsInstanceValid(_GlueCheckbox) ) 
			{
				_GlueCheckbox.QueueFree();
			}
		
			if( IsInstanceValid(_Label) ) 
			{
				_Label.QueueFree();
			}
			
			if( IsInstanceValid(_MarginContainer) ) 
			{
				_MarginContainer.QueueFree();
			}
			
			if( IsInstanceValid(_BoundaryCollision) )  
			{
				_BoundaryCollision.QueueFree();
			}
			
			if( IsInstanceValid(_BoundaryMeshInstance) ) 
			{  
				_BoundaryMeshInstance.QueueFree();
			}
			
			if( IsInstanceValid(_BoundaryBody) ) 
			{
				_BoundaryBody.QueueFree();
			}
		}
	}
}