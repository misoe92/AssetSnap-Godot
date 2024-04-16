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
	using Godot.Collections;

	[Tool]
	public partial class SnapToHeight : LSSnapComponent
	{
		/** Private **/
		private readonly string _Title = "Snap Height";
		private readonly string _CheckboxTitle = "Snap To Height";
		private readonly string _CheckboxTooltip = "Creates a plane in the 3D world that you object will snap to on the Y Axis";
		private readonly string _GlueTitle = "Use glue";
		private readonly string _GlueTooltip = "Will glue the model to the height set here, which will only make it able to move on 2 vectors. (X,Z)";
		private readonly string _NormalsTitle = "Align with normals";
		private readonly string _NormalsTooltip = "Will align the object with the normals of the snap point, rotating it to match the target rotation";
		private readonly string _SpinBoxTooltip = "Sets the value on the Y axis that the object will snap to";

		/*
		** Constructor of the component
		** 
		** @return void
		*/
		public SnapToHeight()
		{
			Name = "LSSnapToHeight";
			Angle = GlobalStates.SnapAngleEnums.Y;
			
			UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
				{ typeof(Spinboxable).ToString() },
			};
			
			//_include = false;
		}
		
		/*
		** Initializes the component
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
			
			Plugin.GetInstance().StatesChanged += () => { MaybeUpdateValue(); };
		}

		public override void MaybeUpdateValue()
		{
			if(
				false == IsValid()
			) 
			{
				return;
			}

			Trait<Spinboxable>()
				.Select(0)
				.SetValue(_GlobalExplorer.States.SnapToHeightValue);

			base.MaybeUpdateValue();
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
			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });
			
			Trait<Checkable>()
				.SetName("SnapHeightCheckbox")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(100, "top")
				.SetMargin(1, "bottom")
				.SetText(_CheckboxTitle)
				.SetTooltipText(_CheckboxTooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer( BoxContainer );
		}
		
		/*
		** Initializes the Checkbox that
		** holds the state of using glue
		** 
		** @return void
		*/
		private void _InitializeGlue( VBoxContainer BoxContainer ) 
		{
			Callable _callable = Callable.From(() => { _OnGlueCheckboxPressed(); });
			
			Trait<Checkable>()
				.SetName("SnapHeightGlueCheckbox")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(1, "bottom")
				.SetText(_GlueTitle)
				.SetTooltipText(_GlueTooltip)
				.Instantiate()
				.Select(1)
				.AddToContainer( BoxContainer );
		}
		
		/*
		** Initializes SpinBox that
		** holds the current SnapHeight
		** 
		** @return void
		*/
		private void _InitializeSpinBox( VBoxContainer BoxContainer ) 
		{
			Callable _callable = Callable.From((double value) => { _OnSpinBoxValueChange((float)value); });
			
			Trait<Spinboxable>()
				.SetName("SnapHeightValue")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(10, "bottom")
				.SetStep(0.01f)
				.SetMinValue(-200)
				.SetPrefix("Y Axis: ")
				.SetTooltipText(_SpinBoxTooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer( BoxContainer );
		}
		
	
		
		/*
		** Updates spawn settings and the internal
		** value for snapping to the Y Axis
		** 
		** @return void
		*/
		private void _OnCheckboxPressed()
		{
			bool state = false;
			if( false == IsSnapTo() ) 
			{
				_GlobalExplorer.States.SnapToHeight = GlobalStates.LibraryStateEnum.Enabled;
				_GlobalExplorer.States.SnapToHeightGlue = GlobalStates.LibraryStateEnum.Enabled;
				state = true;
			}
			else 
			{
				_GlobalExplorer.States.SnapToHeight = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.SnapToHeightGlue = GlobalStates.LibraryStateEnum.Disabled;
			}

			UpdateSpawnSettings("SnapToHeight", state);
			UpdateSpawnSettings("SnapToHeightGlue", state);
		}
		
		/*
		** Updates spawn settings and the internal
		** value for using glue
		** 
		** @return void
		*/
		private void _OnGlueCheckboxPressed()
		{
			bool state = false;
			
			if( false == IsSnapToGlue() ) 
			{
				_GlobalExplorer.States.SnapToHeightGlue = GlobalStates.LibraryStateEnum.Enabled;
				state = true;
			}
			else 
			{
				_GlobalExplorer.States.SnapToHeightGlue = GlobalStates.LibraryStateEnum.Disabled;
			}

			UpdateSpawnSettings("SnapToHeightGlue", state);
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
			_GlobalExplorer.States.SnapToHeightValue = value;
						
			UpdateSpawnSettings("SnapToHeightValue", state);
		}

		
		/*
		** Syncronizes it's value to a global
		** central state controller
		**
		** @return void
		*/
		public override void Sync() 
		{
			if( false == IsValid() )
			{
				return;
			}
			
			_GlobalExplorer.States.SnapToHeight = Trait<Checkable>().Select(0).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
			_GlobalExplorer.States.SnapToHeightGlue = Trait<Checkable>().Select(1).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
			_GlobalExplorer.States.SnapToHeightValue = (float)Trait<Spinboxable>().GetValue();
		}
	}
}