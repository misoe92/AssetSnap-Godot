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
	/// Represents a component for offsetting the Z axis when snapping to objects.
	/// </summary>
	[Tool]
	public partial class SnapOffsetZ : LSObjectComponent
	{
		protected float Value
		{
			get => _Value;
			set
			{
				_Value = value;
				if (IsValid() && Trait<Spinboxable>().Select(0).IsValid())
				{
					Trait<Spinboxable>().Select(0).SetValue(value);
				}
			}
		}
		private readonly string _Title = "Offset Z: ";
		private readonly string _Tooltip = "Offsets the Z axis when snapping to object, enabling for precise operations.";
		private float _Value = 0.0f;

		/// <summary>
		/// Constructor of the SnapOffsetZ component.
		/// </summary>
		public SnapOffsetZ()
		{
			Name = "LSSnapOffsetZ";

			_UsingTraits = new()
			{
				{ typeof(Spinboxable).ToString() },
			};

			//_include = false;
		}

		/// <summary>
		/// Initializes the SnapOffsetZ component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			Callable _callable = Callable.From((double value) => { _OnSpinBoxValueChange((float)value); });

			_Initiated = true;

			Trait<Spinboxable>()
				.SetName("SnapObjectOffsetZ")
				.SetAction(_callable)
				.SetStep(0.01f)
				.SetPrefix(_Title + ": ")
				.SetMinValue(-200)
				.SetVisible(StatesUtils.Get().SnapToObject == GlobalStates.LibraryStateEnum.Enabled)
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(10, "bottom")
				.SetTooltipText(_Tooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer(this);

			Plugin.GetInstance().StatesChanged += (Godot.Collections.Array data) => { _MaybeUpdateValue(data); };
		}
		
		/// <summary>
		/// Synchronizes the SnapOffsetZ component's value with a global state controller.
		/// </summary>
		public override void Sync()
		{
			if (false == IsValid())
			{
				return;
			}

			StatesUtils.Get().SnapToObjectOffsetXValue = (float)Trait<Spinboxable>().Select(0).GetValue();
		}
		
		/// <summary>
		/// Resets the SnapOffsetZ component to its default value.
		/// </summary>
		public void Reset()
		{
			StatesUtils.Get().SnapToObjectOffsetZValue = 0.0f;
		}
		
		/// <summary>
		/// Sets the visibility state of the SnapOffsetZ component.
		/// </summary>
		/// <param name="state">The visibility state to set.</param>
		public override void SetVisible(bool state)
		{
			if (false == IsValid())
			{
				return;
			}

			Trait<Spinboxable>()
				.Select(0)
				.SetVisible(state);
		}
		
		/// <summary>
		/// Retrieves the current value of the SnapOffsetZ component.
		/// </summary>
		/// <returns>The current offset value.</returns>
		public float GetValue()
		{
			return StatesUtils.Get().SnapToObjectOffsetZValue;
		}

		/// <summary>
		/// Checks if the SnapOffsetZ component is valid.
		/// </summary>
		/// <returns>True if the component is valid, otherwise false.</returns>
		public bool IsValid()
		{
			return
				null != _GlobalExplorer &&
				null != StatesUtils.Get() &&
				false != _Initiated &&
				false != HasTrait<Spinboxable>() &&
				null != Trait<Spinboxable>();
		}
		
		/// <summary>
		/// Checks if the SnapOffsetZ component is currently visible.
		/// </summary>
		/// <returns>True if the component is visible, otherwise false.</returns>
		public override bool IsVisible()
		{
			if (false == IsValid())
			{
				return false;
			}

			return Trait<Spinboxable>()
				.Select(0)
				.IsVisible();
		}

		/// <summary>
		/// Handles updating the SnapOffsetZ component's value based on state changes.
		/// </summary>
		/// <param name="data">The array of data containing state change information.</param>
		private void _MaybeUpdateValue(Godot.Collections.Array data)
		{
			if (data[0].As<string>() == "SnapToObject" || data[0].As<string>() == "SnapToObjectOffsetZValue")
			{
				if (false == IsValid())
				{
					return;
				}

				if (data[0].As<string>() == "SnapToObject")
				{
					if (IsSnapToObject() && false == IsVisible() && data[1].As<bool>())
					{
						SetVisible(true);
					}
					else if ((false == IsSnapToObject() || false == data[1].As<bool>()) && true == IsVisible())
					{
						SetVisible(false);
					}
				}

				if (data[0].As<string>() == "SnapToObjectOffsetZValue")
				{
					Trait<Spinboxable>()
						.Select(0)
						.SetValue(_GlobalExplorer.States.SnapToObjectOffsetZValue);
				}
			}
		}

		/// <summary>
		/// Handles updating the value when the SpinBox value changes.
		/// </summary>
		/// <param name="value">The new value of the SpinBox.</param>
		private void _OnSpinBoxValueChange(float value)
		{
			StatesUtils.Get().SnapToObjectOffsetZValue = value;
			UpdateSpawnSettings("SnapToObjectOffsetZValue", value);
		}
	}
}

#endif