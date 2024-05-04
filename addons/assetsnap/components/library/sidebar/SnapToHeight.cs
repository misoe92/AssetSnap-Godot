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
	/// Partial class representing the SnapToHeight component.
	/// </summary>
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

		/// <summary>
		/// Constructor of the SnapToHeight component.
		/// </summary>
		public SnapToHeight()
		{
			Name = "LSSnapToHeight";
			Angle = GlobalStates.SnapAngleEnums.Y;

			_UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
				{ typeof(Spinboxable).ToString() },
			};

			//_include = false;
		}

		/// <summary>
		/// Initializes the SnapToHeight component.
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
		/// Handles updates to the component based on external changes.
		/// </summary>
		/// <param name="data">Data related to the update.</param>
		public override void MaybeUpdateValue(Godot.Collections.Array data)
		{
			if (data[0].As<string>() == "SnapToHeight" || data[0].As<string>() == "SnapToHeightValue")
			{
				if (
					false == IsValid()
				)
				{
					return;
				}

				if (data[0].As<string>() == "SnapToHeightValue")
				{
					Trait<Spinboxable>()
						.Select(0)
						.SetValue(data[1].As<double>());
				}


				base.MaybeUpdateValue(data);
			}
		}
		
		/// <summary>
        /// Synchronizes the state of the SnapToHeight component with the global state controller.
        /// </summary>
		public override void Sync()
		{
			if (false == IsValid())
			{
				return;
			}

			StatesUtils.Get().SnapToHeight = Trait<Checkable>().Select(0).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
			StatesUtils.Get().SnapToHeightGlue = Trait<Checkable>().Select(1).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
			StatesUtils.Get().SnapToHeightValue = (float)Trait<Spinboxable>().GetValue();
		}

		/// <summary>
		/// Initializes the checkbox UI element for SnapToHeight.
		/// </summary>
		/// <param name="BoxContainer">Container where the checkbox will be added.</param>
		private void _InitializeCheckBox(VBoxContainer BoxContainer)
		{
			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });

			Trait<Checkable>()
				.SetName("SnapHeightCheckbox")
				.SetAction(_callable)
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(1, "bottom")
				.SetText(_CheckboxTitle)
				.SetTooltipText(_CheckboxTooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer(BoxContainer);
		}

		/// <summary>
		/// Initializes the glue checkbox UI element for SnapToHeight.
		/// </summary>
		/// <param name="BoxContainer">Container where the checkbox will be added.</param>
		private void _InitializeGlue(VBoxContainer BoxContainer)
		{
			Callable _callable = Callable.From(() => { _OnGlueCheckboxPressed(); });

			Trait<Checkable>()
				.SetName("SnapHeightGlueCheckbox")
				.SetAction(_callable)
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(1, "bottom")
				.SetText(_GlueTitle)
				.SetTooltipText(_GlueTooltip)
				.Instantiate()
				.Select(1)
				.AddToContainer(BoxContainer);
		}

		/// <summary>
		/// Initializes the spin box UI element for SnapToHeight.
		/// </summary>
		/// <param name="BoxContainer">Container where the spin box will be added.</param>
		private void _InitializeSpinBox(VBoxContainer BoxContainer)
		{
			Callable _callable = Callable.From((double value) => { _OnSpinBoxValueChange((float)value); });

			Trait<Spinboxable>()
				.SetName("SnapHeightValue")
				.SetAction(_callable)
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
				.AddToContainer(BoxContainer);
		}

		/// <summary>
		/// Handles the action when the main checkbox is pressed.
		/// </summary>
		private void _OnCheckboxPressed()
		{
			bool state = false;
			if (false == _IsSnapTo())
			{
				StatesUtils.Get().SnapToHeight = GlobalStates.LibraryStateEnum.Enabled;
				StatesUtils.Get().SnapToHeightGlue = GlobalStates.LibraryStateEnum.Enabled;
				state = true;
			}
			else
			{
				StatesUtils.Get().SnapToHeight = GlobalStates.LibraryStateEnum.Disabled;
				StatesUtils.Get().SnapToHeightGlue = GlobalStates.LibraryStateEnum.Disabled;
			}

			UpdateSpawnSettings("SnapToHeight", state);
			UpdateSpawnSettings("SnapToHeightGlue", state);
		}

		/// <summary>
		/// Handles the action when the glue checkbox is pressed.
		/// </summary>
		private void _OnGlueCheckboxPressed()
		{
			bool state = false;

			if (false == IsSnapToGlue())
			{
				StatesUtils.Get().SnapToHeightGlue = GlobalStates.LibraryStateEnum.Enabled;
				state = true;
			}
			else
			{
				StatesUtils.Get().SnapToHeightGlue = GlobalStates.LibraryStateEnum.Disabled;
			}

			UpdateSpawnSettings("SnapToHeightGlue", state);
		}

		/// <summary>
		/// Handles the action when the value of the spin box changes.
		/// </summary>
		/// <param name="value">The new value of the spin box.</param>
		private void _OnSpinBoxValueChange(float value)
		{
			_value = value;
			StatesUtils.Get().SnapToHeightValue = value;

			UpdateSpawnSettings("SnapToHeightValue", state);
		}
	}
}

#endif