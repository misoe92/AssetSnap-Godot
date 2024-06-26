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
	/// A component for snapping objects to the X axis.
	/// </summary>
	[Tool]
	public partial class SnapToX : LSSnapComponent
	{
		private readonly string _Title = "Snap X";
		private readonly string _CheckboxTitle = "Snap To X";
		private readonly string _CheckboxTooltip = "Creates a plane in the 3D world that you object will snap to on the X Axis";
		private readonly string _GlueTitle = "Use glue";
		private readonly string _GlueTooltip = "Will glue the model to the X axis set here, which will only make it able to move on 2 vectors. (Y,Z)";
		private readonly string _SpinBoxTooltip = "Sets the value on the X axis that the object will snap to";

		/// <summary>
		/// Constructor of the SnapToX component.
		/// </summary>
		public SnapToX()
		{
			Name = "LSSnapToX";
			Angle = GlobalStates.SnapAngleEnums.X;
			
			_UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
				{ typeof(Spinboxable).ToString() },
			};
			
			//_include = false;
		}
		
		/// <summary>
		/// Initializes the SnapToX component.
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
		/// Updates the value of the component if necessary.
		/// </summary>
		/// <param name="data">Data to update.</param>
		public override void MaybeUpdateValue(Godot.Collections.Array data)
		{
			if( data[0].As<string>() == "SnapToX" || data[0].As<string>() == "SnapToXValue" ) 
			{
				if(
					false == IsValid()
				) 
				{
					return;
				}

				if( data[0].As<string>() == "SnapToXValue" ) 
				{
					Trait<Spinboxable>()
						.Select(0)
						.SetValue(data[1].As<double>());
				}
			
				base.MaybeUpdateValue(data);
			}
		}
		
		/// <summary>
        /// Synchronizes the component's value with the global state controller.
        /// </summary>
		public override void Sync() 
		{
			if( IsValid() )
			{
				return;
			} 
			
			StatesUtils.Get().SnapToX = Trait<Checkable>().Select(0).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
			StatesUtils.Get().SnapToXGlue = Trait<Checkable>().Select(1).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
			StatesUtils.Get().SnapToXValue = (float)Trait<Spinboxable>().GetValue();
		}
		
		/// <summary>
		/// Initializes the checkbox for snapping to the X axis.
		/// </summary>
		/// <param name="BoxContainer">The VBoxContainer to add the checkbox to.</param>
		private void _InitializeCheckBox( VBoxContainer BoxContainer ) 
		{
			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });
			
			Trait<Checkable>()
				.SetName("SnapXCheckbox")
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
		/// Initializes the checkbox for snapping to the X axis glue.
		/// </summary>
		/// <param name="BoxContainer">The VBoxContainer to add the checkbox to.</param>
		private void _InitializeGlue( VBoxContainer BoxContainer ) 
		{
			Callable _callable = Callable.From(() => { _OnGlueCheckboxPressed(); });
			
			Trait<Checkable>()
				.SetName("SnapXGlueCheckbox")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(2, "bottom")
				.SetText(_GlueTitle)
				.SetTooltipText(_GlueTooltip)
				.SetVisible(false)
				.Instantiate()
				.Select(1)
				.AddToContainer( BoxContainer );
		}
		
		/// <summary>
		/// Initializes the spinbox for setting the value of the X axis.
		/// </summary>
		/// <param name="BoxContainer">The VBoxContainer to add the spinbox to.</param>
		private void _InitializeSpinBox( VBoxContainer BoxContainer ) 
		{
			Callable _callable = Callable.From((double value) => { _OnSpinBoxValueChange((float)value); });
			
			Trait<Spinboxable>()
				.SetName("SnapXValue")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(10, "bottom")
				.SetStep(0.01f)
				.SetMinValue(-200)
				.SetPrefix("X Axis: ")
				.SetTooltipText(_SpinBoxTooltip)
				.SetVisible(false)
				.Instantiate()
				.Select(0)
				.AddToContainer( BoxContainer );
		}
		
		/// <summary>
		/// Updates spawn settings and the internal value for snapping to the X axis.
		/// </summary>
		private void _OnCheckboxPressed()
		{
			bool state = false;
			if( false == _IsSnapTo() ) 
			{
				StatesUtils.Get().SnapToX = GlobalStates.LibraryStateEnum.Enabled;
				StatesUtils.Get().SnapToXGlue = GlobalStates.LibraryStateEnum.Enabled;
				state = true;
			}
			else 
			{
				StatesUtils.Get().SnapToX = GlobalStates.LibraryStateEnum.Disabled;
				StatesUtils.Get().SnapToXGlue = GlobalStates.LibraryStateEnum.Disabled;
			}

			UpdateSpawnSettings("SnapToX", state);
			UpdateSpawnSettings("SnapToXGlue", state);
		}
		
		/// <summary>
		/// Updates spawn settings and the internal value for using glue.
		/// </summary>
		private void _OnGlueCheckboxPressed()
		{
			bool state = false;
			
			if( false == IsSnapToGlue() ) 
			{
				StatesUtils.Get().SnapToXGlue = GlobalStates.LibraryStateEnum.Enabled;
				state = true;
			}
			else 
			{
				StatesUtils.Get().SnapToXGlue = GlobalStates.LibraryStateEnum.Disabled;
			}
		
			UpdateSpawnSettings("SnapToXGlue", state);
		}
		
		/// <summary>
		/// Updates SpawnSettings on the model when the component value is changed.
		/// </summary>
		/// <param name="value">The new value for the X axis.</param>
		private void _OnSpinBoxValueChange(float value)
		{
			_value = value;
			StatesUtils.Get().SnapToXValue = value;
						
			UpdateSpawnSettings("SnapXValue", value);
		}
	}
}

#endif