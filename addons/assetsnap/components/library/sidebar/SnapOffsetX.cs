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
	public partial class SnapOffsetX : LSObjectComponent
	{
		public float value 
		{
			get => IsValid() ? (float)Trait<Spinboxable>().GetValue() : 0;
			set 
			{
				if( false == IsValid() && null != Trait<Spinboxable>() ) 
				{
					Trait<Spinboxable>().SetValue(value);
				}
			}
		}
		
		private readonly string _Title = "Offset X: ";
		private readonly string _Tooltip = "Offsets the X axis when snapping to object, enabling for precise operations.";

		/*
		** Constructor of the component
		** 
		** @return void
		*/	
		public SnapOffsetX()
		{
			Name = "LSSnapOffsetX";
			
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
				.SetName("SnapObjectOffsetX")
				.SetAction(_callable)
				.SetStep(0.01f)
				.SetPrefix(_Title + ": ")
				.SetMinValue(-200)
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(0, "bottom")
				.SetTooltipText(_Tooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer( this );
				
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
				.SetValue(_GlobalExplorer.States.SnapToObjectOffsetXValue);
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
		** Updates spawn settings when
		** input is changed
		** 
		** @return void
		*/	
		private void _OnSpinBoxValueChange(float value)
		{
			_GlobalExplorer.States.SnapToObjectOffsetXValue = value;	
			UpdateSpawnSettings("SnapToObjectOffsetXValue", value);
		}
		
		/*
		** Fetches the current component value
		** 
		** @return float
		*/	
		public float GetValue()
		{
			return _GlobalExplorer.States.SnapToObjectOffsetXValue;
		}
		
		/*
		** Resets the component
		** 
		** @return void
		*/	
		public void Reset()
		{
			_GlobalExplorer.States.SnapToObjectOffsetXValue = 0.0f;
		}
		
		public bool IsValid()
		{
			return
				null != _GlobalExplorer &&
				null != _GlobalExplorer.States &&
				false != Initiated &&
				null != Trait<Spinboxable>() &&
				false != HasTrait<Spinboxable>();
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