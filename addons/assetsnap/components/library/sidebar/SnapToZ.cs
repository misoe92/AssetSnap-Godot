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

namespace AssetSnap.Front.Components.Library.Sidebar
{
	using AssetSnap.Component;
	using Godot;

	[Tool]
	public partial class SnapToZ : LSSnapComponent
	{
		private readonly string _Title = "Snap Z";
		private readonly string _CheckboxTitle = "Snap To Z";
		private readonly string _CheckboxTooltip = "Creates a plane in the 3D world that you object will snap to on the Z Axis";
		private readonly string _GlueTitle = "Use glue";
		private readonly string _GlueTooltip = "Will glue the model to the Z axis set here, which will only make it able to move on 2 vectors. (Y,Z)";
		private readonly string _SpinBoxTooltip = "Sets the value on the Z axis that the object will snap to";

		/*
		** Constructor of the component
		** 
		** @return void
		*/
		public SnapToZ()
		{
			Name = "LSSnapToZ";
			Angle = GlobalStates.SnapAngleEnums.Z;
			
			UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
				{ typeof(Spinboxable).ToString() },
			};
			
			//_include = false; 
		}
		
		/*
		** Initializing the component
		** 
		** @return void
		*/
		public override void Initialize()
		{
			base.Initialize();

			Initiated = true;
			
			_InitializeCheckBox(this);
			_InitializeGlue(this);
			_InitializeSpinBox(this);
			Plugin.GetInstance().StatesChanged += (Godot.Collections.Array data) => { MaybeUpdateValue(data); };
		}

		public override void MaybeUpdateValue(Godot.Collections.Array data)
		{
			if( data[0].As<string>() == "SnapToZ" || data[0].As<string>() == "SnapToZValue" ) 
			{
				if(
					false == IsValid()
				) 
				{
					return;
				}

				if( data[0].As<string>() == "SnapToZValue" ) 
				{
					Trait<Spinboxable>()
						.Select(0)
						.SetValue(data[1].As<double>());

				}
				
				if( data[0].As<string>() == "SnapToZ" ) 
				{
					base.MaybeUpdateValue(data);
				}
			}
		}
	
		/*
		** Initializing the checkbox that holds
		** the state value of the component
		** 
		** @return void
		*/
		private void _InitializeCheckBox( VBoxContainer BoxContainer ) 
		{
			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });
			
			Trait<Checkable>()
				.SetName("SnapZCheckbox")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(2, "top")
				.SetMargin(10, "bottom")
				.SetText(_CheckboxTitle)
				.SetTooltipText(_CheckboxTooltip) 
				.Instantiate()
				.Select(0)
				.AddToContainer( BoxContainer );
		}
		
		/*
		** Initializing the checkbox that holds
		** the state value of the using glue option
		** 
		** @return void
		*/
		private void _InitializeGlue( VBoxContainer BoxContainer ) 
		{
			Callable _callable = Callable.From(() => { _OnGlueCheckboxPressed(); });
		
			Trait<Checkable>()
				.SetName("SnapZGlueCheckbox")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(2, "bottom")
				.SetText(_GlueTitle)
				.SetTooltipText(_GlueTooltip)
				.Instantiate()
				.Select(1)
				.AddToContainer( BoxContainer );
		}
		
		/*
		** Initializing the spinbox that holds
		** the value of the component
		** 
		** @return void
		*/
		private void _InitializeSpinBox( VBoxContainer BoxContainer ) 
		{
			Callable _callable = Callable.From((double value) => { _OnSpinBoxValueChange((float)value); });
			
			Trait<Spinboxable>()
				.SetName("SnapZValue")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(10, "bottom")
				.SetStep(0.01f)
				.SetMinValue(-200)
				.SetPrefix("Z Axis: ")
				.SetTooltipText(_SpinBoxTooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer( BoxContainer );
		}
		
		/*
		** Updates spawn settings and the internal
		** value for snapping to the X Axis
		** 
		** @return void
		*/
		private void _OnCheckboxPressed()
		{
			bool state = false;
			
			if( false == IsSnapTo() ) 
			{
				_GlobalExplorer.States.SnapToZ = GlobalStates.LibraryStateEnum.Enabled;
				_GlobalExplorer.States.SnapToZGlue = GlobalStates.LibraryStateEnum.Enabled;
				state = true;
			}
			else 
			{
				_GlobalExplorer.States.SnapToZ = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.SnapToZGlue = GlobalStates.LibraryStateEnum.Disabled;
			}
			
			string key = "SnapToZ";
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
			state = false;
			
			if( false == IsSnapToGlue() ) 
			{
				_GlobalExplorer.States.SnapToZGlue = GlobalStates.LibraryStateEnum.Enabled;
				state = true;
			}
			else 
			{
				_GlobalExplorer.States.SnapToZGlue = GlobalStates.LibraryStateEnum.Disabled;
			}
			
			string key = "SnapToZGlue";
			UpdateSpawnSettings(key, state);
		}
		
		/*
		** Updates SpawnSettings on the model
		** when component value is changed
		** 
		** @return void
		*/
		private void _OnSpinBoxValueChange(float value)
		{
			_value = value;
			_GlobalExplorer.States.SnapToZValue = value;
			string key = "SnapToZValue";
			UpdateSpawnSettings(key, _value);
		}
		
		/*
		** Syncronizes it's value to a global
		** central state controller
		**
		** @return void
		*/
		public override void Sync() 
		{
			if( IsValid() )
			{
				return;
			} 
			
			_GlobalExplorer.States.SnapToZ = Trait<Checkable>().Select(0).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
			_GlobalExplorer.States.SnapToZGlue = Trait<Checkable>().Select(1).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
			_GlobalExplorer.States.SnapToZValue = (float)Trait<Spinboxable>().GetValue();
		}
	}
}