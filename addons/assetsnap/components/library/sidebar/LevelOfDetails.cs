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
	public partial class LevelOfDetails : LibraryComponent
	{
		public bool state 
		{
			get => GetState();
			set 
			{
				if(
					IsValid()
				) 
				{
					Trait<Checkable>().SetValue(value);
				}
			}
		}
		
		private readonly string _Title = "Level of details";
		private readonly string _CheckboxTitle = "Use specified level";
		private readonly string _CheckboxTooltip = "When enabled you will be able to choose a Level of details level for all components spawned.";

		/*
		** Constructor for the component
		** 
		** @return void
		*/	
		public LevelOfDetails()
		{
			Name = "LSLevelOfDetails";
			
			UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
				{ typeof(Spinboxable).ToString() },
			};
			
			//_include = false;
		}
		
		public bool GetState()
		{
			if(
				false == IsValid()
			) 
			{
				return false;
			}
			
			return HasTrait<Checkable>() && false != Trait<Checkable>().IsValid() && false != Trait<Checkable>().Select(0).IsValid() ? Trait<Checkable>().Select(0).GetValue() : false;	
		}
		
		/*
		** Initializes the component
		** 
		** @return void
		*/	
		public override void Initialize()
		{
			base.Initialize();

			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });
			
			Initiated = true;
		
			Trait<Checkable>()
				.SetName("LevelOfDetailsCheckbox")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(2, "top")
				.SetMargin(2, "bottom")
				.SetText(_CheckboxTitle)
				.SetTooltipText(_CheckboxTooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer( this );
				
			Trait<Spinboxable>()
				.SetName("LevelOfDetailsScale")
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(10, "bottom")
				.SetPrefix("LoD Level: ")
				.SetStep(0.1f)
				.SetMaxValue(5)
				.SetMinValue(0)
				.SetTooltipText(_CheckboxTooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer( this );
	
			Plugin.GetInstance().StatesChanged += (Godot.Collections.Array data) => { MaybeUpdateValue(); };
		}
		
		/*
		** Handles synchronization of the checkboxes
		** so it matches the state of the model
		** 
		** @return void
		*/	
		public void MaybeUpdateValue()
		{
			if( 
				false == IsValid()
			) 
			{
				return;
			}
		}
		
		/*
		** Updates spawn settings values on
		** change
		** 
		** @return void
		*/	
		private void _OnCheckboxPressed()
		{
			bool state = false;
		}
		
		private bool IsCheckboxChecked()
		{
			return state == true;
		}
		
		/*
		** Resets the component
		** 
		** @return void
		*/	
		public void Reset()
		{
			_GlobalExplorer.States.SnapToObject = GlobalStates.LibraryStateEnum.Disabled;
			state = false;
		}
		
		public bool IsValid()
		{
			return
				null != _GlobalExplorer &&
				null != _GlobalExplorer.States &&
				false != Initiated &&
				null != Trait<Checkable>() &&
				false != HasTrait<Checkable>();
		}
		
		/*
		** Syncronizes it's value to a global
		** central state controller
		**
		** @return void
		*/
		public override void Sync() 
		{
			// _GlobalExplorer.States.LevelOfDetails = state ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
		}
	}
}