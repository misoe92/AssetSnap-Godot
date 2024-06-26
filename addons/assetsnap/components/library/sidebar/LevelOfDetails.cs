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
	/// A component representing the level of details settings in the library sidebar.
	/// </summary>
	[Tool]
	public partial class LevelOfDetails : LibraryComponent
	{
		/// <summary>
		/// Gets or sets the state of the level of details settings.
		/// </summary>
		public bool State 
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

		/// <summary>
		/// Constructor for the LevelOfDetails component.
		/// </summary>
		public LevelOfDetails()
		{
			Name = "LSLevelOfDetails";
			
			_UsingTraits = new()
			{
				{ typeof(Labelable).ToString() },
				{ typeof(Checkable).ToString() },
				{ typeof(Spinboxable).ToString() },
			};
			
			//_include = false;
		}
		
		/// <summary>
		/// Gets the state of the level of details settings.
		/// </summary>
		/// <returns>True if the level of details settings are enabled, otherwise false.</returns>
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
		
		/// <summary>
		/// Initializes the LevelOfDetails component.
		/// </summary>	
		public override void Initialize()
		{
			base.Initialize();

			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });
			
			_Initiated = true;
			
			Trait<Labelable>()
				.SetName("LevelOfDetailsText")
				.SetType(Labelable.TitleType.TextSmall)
				.SetText( "If no Level of detail value is specified, models will follow the project settings for LOD" )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(2, "top")
				.SetMargin(2, "bottom")
				.SetAutoWrap(TextServer.AutowrapMode.Word)
				.Instantiate()
				.Select(0)
				.AddToContainer( this );
				
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
				.SetName("LevelOfDetailsValue")
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
				.SetAction(Callable.From((double value) => { _OnLevelOfDetailsChanged(value); }))
				.Instantiate()
				.Select(0)
				.AddToContainer( this );
	
			Plugin.GetInstance().StatesChanged += (Godot.Collections.Array data) => { MaybeUpdateValue(); };
		}
		
		/// <summary>
		/// Handles synchronization of the checkboxes so it matches the state of the model.
		/// </summary>
		public void MaybeUpdateValue()
		{
			if( 
				false == IsValid()
			) 
			{
				return;
			}
		}
		
		/// <summary>
		/// Resets the component to its default state.
		/// </summary>	
		public void Reset()
		{
			StatesUtils.Get().LevelOfDetailsState = GlobalStates.LibraryStateEnum.Disabled;
			StatesUtils.Get().LevelOfDetails = 0.0f;
			State = false;
		}
		
		/// <summary>
		/// Synchronizes the component's value to a global central state controller.
		/// </summary>
		public override void Sync() 
		{
			if( StatesUtils.Get().LevelOfDetailsState == GlobalStates.LibraryStateEnum.Enabled ) 
			{
				StatesUtils.Get().LevelOfDetails = (float)Trait<Spinboxable>().Select(0).GetValue();
			}
		}
		
		/// <summary>
		/// Checks if the component is in a valid state.
		/// </summary>
		/// <returns>True if the component is valid, otherwise false.</returns>
		public bool IsValid()
		{
			return
				null != _GlobalExplorer &&
				null != StatesUtils.Get() &&
				false != _Initiated &&
				null != Trait<Checkable>() &&
				false != HasTrait<Checkable>();
		}
		
		/// <summary>
		/// Handles the checkbox press event.
		/// </summary>
		private void _OnCheckboxPressed()
		{
			if( StatesUtils.Get().LevelOfDetailsState == GlobalStates.LibraryStateEnum.Disabled ) 
			{
				StatesUtils.Get().LevelOfDetailsState = GlobalStates.LibraryStateEnum.Enabled;
			}
			else
			{
				StatesUtils.Get().LevelOfDetailsState = GlobalStates.LibraryStateEnum.Disabled;	
			}
		}
		
		/// <summary>
		/// Handles the change in level of details value.
		/// </summary>
		/// <param name="value">The new value for the level of details.</param>
		private void _OnLevelOfDetailsChanged( double value ) 
		{
			StatesUtils.Get().LevelOfDetails = (float)value;
		}
		
		/// <summary>
		/// Checks if the level of details checkbox is checked.
		/// </summary>
		/// <returns>True if the checkbox is checked, otherwise false.</returns>
		private bool IsCheckboxChecked()
		{
			return State == true;
		}
	}
}

#endif