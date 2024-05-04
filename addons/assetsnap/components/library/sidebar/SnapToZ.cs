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

using AssetSnap.Component;
using AssetSnap.States;
using Godot;
	
namespace AssetSnap.Front.Components.Library.Sidebar
{
	/// <summary>
	/// Partial class for handling snapping functionality on the Z axis.
	/// </summary>
	[Tool]
	public partial class SnapToZ : LSSnapComponent
	{
		private readonly string _Title = "Snap Z";
		private readonly string _CheckboxTitle = "Snap To Z";
		private readonly string _CheckboxTooltip = "Creates a plane in the 3D world that you object will snap to on the Z Axis";
		private readonly string _GlueTitle = "Use glue";
		private readonly string _GlueTooltip = "Will glue the model to the Z axis set here, which will only make it able to move on 2 vectors. (Y,Z)";
		private readonly string _SpinBoxTooltip = "Sets the value on the Z axis that the object will snap to";

		/// <summary>
		/// Constructor of the component.
		/// </summary>
		public SnapToZ()
		{
			Name = "LSSnapToZ";
			Angle = GlobalStates.SnapAngleEnums.Z;
			
			_UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
				{ typeof(Spinboxable).ToString() },
			};
			
			//_include = false; 
		}
		
		/// <summary>
		/// Initializes the component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			_Initiated = true;
			
			_InitializeCheckBox(this);
			_InitializeGlue(this);
			_InitializeSpinBox(this);
			Plugin.GetInstance().StatesChanged += (Godot.Collections.Array data) => { MaybeUpdateValue(data); };
		}

		/// <summary>
		/// Updates the component value if necessary based on incoming data.
		/// </summary>
		/// <param name="data">The incoming data.</param>
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
				
				
				base.MaybeUpdateValue(data);
			}
		}
		
		/// <summary>
        /// Syncronizes its value to a global central state controller.
        /// </summary>
		public override void Sync() 
		{
			if( IsValid() )
			{
				return;
			} 
			
			StatesUtils.Get().SnapToZ = Trait<Checkable>().Select(0).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
			StatesUtils.Get().SnapToZGlue = Trait<Checkable>().Select(1).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
			StatesUtils.Get().SnapToZValue = (float)Trait<Spinboxable>().GetValue();
		}
	
		/// <summary>
		/// Initializes the checkbox that holds the state value of the component.
		/// </summary>
		/// <param name="BoxContainer">The container to add the checkbox to.</param>
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
		
		/// <summary>
		/// Initializes the checkbox that holds the state value of the using glue option.
		/// </summary>
		/// <param name="BoxContainer">The container to add the checkbox to.</param>
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
				.SetVisible(false)
				.SetTooltipText(_GlueTooltip)
				.Instantiate()
				.Select(1)
				.AddToContainer( BoxContainer );
		}
		
		/// <summary>
		/// Initializes the spinbox that holds the value of the component.
		/// </summary>
		/// <param name="BoxContainer">The container to add the spinbox to.</param>
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
				.SetVisible(false)
				.SetTooltipText(_SpinBoxTooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer( BoxContainer );
		}
		
		/// <summary>
		/// Updates spawn settings and the internal value for snapping to the X Axis.
		/// </summary>
		private void _OnCheckboxPressed()
		{
			bool state = false;
			
			if( false == _IsSnapTo() ) 
			{
				StatesUtils.Get().SnapToZ = GlobalStates.LibraryStateEnum.Enabled;
				StatesUtils.Get().SnapToZGlue = GlobalStates.LibraryStateEnum.Enabled;
				state = true;
			}
			else 
			{
				StatesUtils.Get().SnapToZ = GlobalStates.LibraryStateEnum.Disabled;
				StatesUtils.Get().SnapToZGlue = GlobalStates.LibraryStateEnum.Disabled;
			}
			
			string key = "SnapToZ";
			UpdateSpawnSettings(key, state);
		}
		
		/// <summary>
		/// Updates spawn settings and the internal value for using glue.
		/// </summary>
		private void _OnGlueCheckboxPressed()
		{
			state = false;
			
			if( false == IsSnapToGlue() ) 
			{
				StatesUtils.Get().SnapToZGlue = GlobalStates.LibraryStateEnum.Enabled;
				state = true;
			}
			else 
			{
				StatesUtils.Get().SnapToZGlue = GlobalStates.LibraryStateEnum.Disabled;
			}
			
			string key = "SnapToZGlue";
			UpdateSpawnSettings(key, state);
		}
		
		/// <summary>
		/// Updates SpawnSettings on the model when component value is changed.
		/// </summary>
		/// <param name="value">The new value of the spinbox.</param>
		private void _OnSpinBoxValueChange(float value)
		{
			_value = value;
			StatesUtils.Get().SnapToZValue = value;
			string key = "SnapToZValue";
			UpdateSpawnSettings(key, _value);
		}
	}
}

#endif