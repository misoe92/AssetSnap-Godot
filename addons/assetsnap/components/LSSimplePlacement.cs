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
	public partial class LSSimplePlacement : CheckableComponent
	{
		private readonly string _Title = "Simple placement";
		private readonly string _CheckboxTooltip = "Can be used for single object scripts and when you only need few objects";

		/*
		** Constructor of the component
		**
		** @return void
		*/
		public LSSimplePlacement()
		{
			Name = "LSSimplePlacement";
			//_include = false;
		}

		/*
		** Initializes the component
		**
		** @return void
		*/
		public override void Initialize()
		{
			AddTrait(typeof(Checkable));
			base.Initialize();
			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });
			
			Initiated = true;

			Trait<Checkable>()
				.SetName("SimplePlacementCheckbox")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(2, "top")
				.SetMargin(7, "bottom")
				.SetText(_Title)
				.SetTooltipText(_CheckboxTooltip)
				.SetValue( IsActive() )
				.Instantiate()
				.Select(0)
				.AddToContainer( Container );

			Plugin.GetInstance().StatesChanged += () => { MaybeUpdateValue(); };
		}

		public void MaybeUpdateValue()
		{
			if(
				false == IsValid()
			) 
			{
				return;
			}

			if( IsActive() && false == Trait<Checkable>().Select(0).GetValue() ) 
			{
				Trait<Checkable>().Select(0).SetValue(true);
			}
			else if( IsOptimized() && true == Trait<Checkable>().Select(0).GetValue() )
			{
				Trait<Checkable>().Select(0).SetValue(false);	
			}
		}

		/*
		** Updates collisions and spawn settings
		** of the model
		**
		** @return void
		*/
		private void _OnCheckboxPressed()
		{
			if (
				false == IsValid()
			)
			{
				return;
			}
			
			if( Trait<Checkable>().Select(0).GetValue() ) 
			{
				_GlobalExplorer.States.PlacingType = GlobalStates.PlacingTypeEnum.Simple;
				UpdateSpawnSettings("SimplePlacement", true);
			}
			else
			{
				UpdateSpawnSettings("SimplePlacement", false);
			}
		}

		/*
		** Checks if the component state
		** is active
		** 
		** @return bool
		*/
		public bool IsActive() 
		{
			return GlobalExplorer.GetInstance().States.PlacingType == GlobalStates.PlacingTypeEnum.Simple;
		}
		
		/*
		** Checks if the component state
		** is active
		** 
		** @return bool
		*/
		public bool IsOptimized() 
		{
			return _GlobalExplorer.States.PlacingType == GlobalStates.PlacingTypeEnum.Optimized;
		}
		
		/*
		** Syncronizes it's value to a global
		** central state controller
		**
		** @return void
		*/
		public override void Sync() 
		{
			if(
				IsValid() &&
				Trait<Checkable>().Select(0).GetValue()
			) 
			{
				_GlobalExplorer.States.PlacingType = GlobalStates.PlacingTypeEnum.Simple;
			}
		}

		public override void _ExitTree()
		{
			Plugin.GetInstance().StatesChanged -= () => { MaybeUpdateValue(); };
			Initiated = false;
			base._ExitTree();
		}
	}
}