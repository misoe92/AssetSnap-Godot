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
	using AssetSnap.Component;
	using Godot;

	[Tool]
	public partial class LSSnapOffsetZ : LSObjectComponent
	{
		public float value 
		{
			get => IsValid() ? (float)Trait<Spinboxable>().Select(0).GetValue() : 0;
			set 
			{
				if( IsValid() ) 
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
		public LSSnapOffsetZ()
		{
			Name = "LSSnapOffsetZ";
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
			AddTrait(typeof(Spinboxable));

			Callable _callable = Callable.From((double value) => { _OnSpinBoxValueChange((float)value); });
			
			Initiated = true;
			
			Trait<Spinboxable>()
				.SetName("SnapObjectOffsetZ")
				.SetAction(_callable)
				.SetStep(0.01f)
				.SetPrefix(_Title + ": ")
				.SetMinValue(-200)
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(10, "bottom")
				.SetTooltipText(_Tooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer( Container );
				
			Plugin.GetInstance().StatesChanged += () => { MaybeUpdateValue(); };
		}
		
		private void MaybeUpdateValue()
		{
			if( false == IsValid() ) 
			{
				return;
			}
			
			if( IsSnapToObject() && false == IsVisible()) 
			{
				SetVisible(true);
			}
			else if( false == IsSnapToObject() && true == IsVisible())
			{
				SetVisible(false);
			}
			
			Trait<Spinboxable>()
				.Select(0)
				.SetValue(_GlobalExplorer.States.SnapToObjectOffsetZValue);
		}
		
		public override void SetVisible(bool state) 
		{
			if( false == IsValid() ) 
			{
				return;
			}
			
			Trait<Spinboxable>()
				.Select(0)
				.SetVisible( state );
		}
		
		public override bool IsVisible() 
		{
			if( false == IsValid() ) 
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
				null != Trait<Spinboxable>() &&
				false != HasTrait<Spinboxable>() &&
				IsInstanceValid( Trait<Spinboxable>() );
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
			
			_GlobalExplorer.States.SnapToObjectOffsetXValue = (float)Trait<Spinboxable>().Select(0).GetValue();
		}
	}
}