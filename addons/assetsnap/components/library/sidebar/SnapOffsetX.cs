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
	/// Represents a component for setting the offset on the X-axis when snapping to objects.
	/// </summary>
	[Tool]
	public partial class SnapOffsetX : LSObjectComponent
	{
		/// <summary>
		/// Gets or sets the value of the X-axis offset.
		/// </summary>
		/// <remarks>If the component is not valid or the spin box trait is null, setting the value will not take effect.</remarks>
		public float value
		{
			get => IsValid() ? (float)Trait<Spinboxable>().GetValue() : 0;
			set
			{
				if (false == IsValid() && null != Trait<Spinboxable>())
				{
					Trait<Spinboxable>().SetValue(value);
				}
			}
		}

		private readonly string _Title = "Offset X: ";
		private readonly string _Tooltip = "Offsets the X axis when snapping to object, enabling for precise operations.";

		/// <summary>
		/// Constructor of the SnapOffsetX component.
		/// </summary>
		public SnapOffsetX()
		{
			Name = "LSSnapOffsetX";

			UsingTraits = new()
			{
				{ typeof(Spinboxable).ToString() },
			};

			//_include = false;
		}

		/// <summary>
		/// Initializes the SnapOffsetX component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			Callable _callable = Callable.From((double value) => { _OnSpinBoxValueChange((float)value); });

			Initiated = true;

			Trait<Spinboxable>()
				.SetName("SnapObjectOffsetX")
				.SetAction(_callable)
				.SetStep(0.01f)
				.SetPrefix(_Title + ": ")
				.SetMinValue(-200)
				.SetDimensions(140, 20)
				.SetVisible(StatesUtils.Get().SnapToObject == GlobalStates.LibraryStateEnum.Enabled)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(0, "bottom")
				.SetTooltipText(_Tooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer(this);

			Plugin.GetInstance().StatesChanged += (Godot.Collections.Array data) => { MaybeUpdateValue(data); };
		}

		/// <summary>
		/// Checks if the SnapOffsetX component needs to update its value based on the provided data.
		/// </summary>
		/// <param name="data">The array of data containing information about the update.</param>
		/// <remarks>
		/// This method is responsible for updating the SnapOffsetX component's visibility and value based on changes in the provided data.
		/// It specifically checks if the data indicates changes related to snapping to objects or changes in the offset X value.
		/// If the component is not valid or the data does not indicate relevant changes, the method returns without performing any updates.
		/// </remarks>
		private void MaybeUpdateValue(Godot.Collections.Array data)
		{
			if (data[0].As<string>() == "SnapToObject" || data[0].As<string>() == "SnapToObjectOffsetXValue")
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

				if (data[0].As<string>() == "SnapToObjectOffsetXValue")
				{
					Trait<Spinboxable>()
						.Select(0)
						.SetValue(data[1].As<double>());
				}
			}
		}

		/// <summary>
		/// Sets the visibility state of the SnapOffsetX component.
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
		/// Checks if the SnapOffsetX component is currently visible.
		/// </summary>
		/// <returns>True if visible, false otherwise.</returns>
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
		/// Updates spawn settings when the input value changes.
		/// </summary>
		/// <param name="value">The new value of the spin box.</param>
		private void _OnSpinBoxValueChange(float value)
		{
			_GlobalExplorer.States.SnapToObjectOffsetXValue = value;
			UpdateSpawnSettings("SnapToObjectOffsetXValue", value);
		}

		/// <summary>
		/// Retrieves the current value of the SnapOffsetX component.
		/// </summary>
		/// <returns>The current X-axis offset value.</returns>
		public float GetValue()
		{
			return _GlobalExplorer.States.SnapToObjectOffsetXValue;
		}

		/// <summary>
		/// Resets the SnapOffsetX component to its default state.
		/// </summary>
		public void Reset()
		{
			_GlobalExplorer.States.SnapToObjectOffsetXValue = 0.0f;
		}

		/// <summary>
		/// Checks if the SnapOffsetX component is valid.
		/// </summary>
		/// <returns>True if the component is valid, false otherwise.</returns>
		public bool IsValid()
		{
			return
				null != _GlobalExplorer &&
				null != _GlobalExplorer.States &&
				false != Initiated &&
				null != Trait<Spinboxable>() &&
				false != HasTrait<Spinboxable>();
		}

		/// <summary>
		/// Synchronizes the SnapOffsetX component's value with the global central state controller.
		/// </summary>
		public override void Sync()
		{
			if (false == IsValid())
			{
				return;
			}

			_GlobalExplorer.States.SnapToObjectOffsetXValue = (float)Trait<Spinboxable>().Select(0).GetValue();
		}
	}
}

#endif