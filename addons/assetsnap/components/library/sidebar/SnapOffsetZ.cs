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
	using AssetSnap.States;
	using Godot;

	[Tool]
	public partial class SnapOffsetZ : LSObjectComponent
	{
		private float _value = 0.0f;
		protected float value
		{
			get => _value;
			set
			{
				_value = value;
				if (IsValid() && Trait<Spinboxable>().Select(0).IsValid())
				{
					Trait<Spinboxable>().Select(0).SetValue(value);
				}
			}
		}

		private readonly string _Title = "Offset Z: ";
		private readonly string _Tooltip = "Offsets the Z axis when snapping to object, enabling for precise operations.";

		/*
		** Constructor of the component
		** 
		** @return void
		*/
		public SnapOffsetZ()
		{
			Name = "LSSnapOffsetZ";

			UsingTraits = new()
			{
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

			Callable _callable = Callable.From((double value) => { _OnSpinBoxValueChange((float)value); });

			Initiated = true;

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

			Plugin.GetInstance().StatesChanged += (Godot.Collections.Array data) => { MaybeUpdateValue(data); };
		}

		private void MaybeUpdateValue(Godot.Collections.Array data)
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

		/*
		** Updates the spawn settings of the
		** model when the component value
		** is changed
		** 
		** @return void
		*/
		private void _OnSpinBoxValueChange(float value)
		{
			_GlobalExplorer.States.SnapToObjectOffsetZValue = value;
			UpdateSpawnSettings("SnapToObjectOffsetZValue", value);
		}

		/*
		** Fetches the current value
		** of the component
		** 
		** @return float
		*/
		public float GetValue()
		{
			return _GlobalExplorer.States.SnapToObjectOffsetZValue;
		}

		/*
		** Resets the component
		** 
		** @return void
		*/
		public void Reset()
		{
			_GlobalExplorer.States.SnapToObjectOffsetZValue = 0.0f;
		}

		public bool IsValid()
		{
			return
				null != _GlobalExplorer &&
				null != _GlobalExplorer.States &&
				false != Initiated &&
				false != HasTrait<Spinboxable>() &&
				null != Trait<Spinboxable>();
		}

		/*
		** Syncronizes it's value to a global
		** central state controller
		**
		** @return void
		*/
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