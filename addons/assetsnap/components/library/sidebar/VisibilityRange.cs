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
	/// Represents the visibility range component in the library sidebar.
	/// </summary>
	[Tool]
	public partial class VisibilityRange : LibraryComponent
	{
		/// <summary>
		/// Gets or sets the state of the visibility range component.
		/// </summary>
		/// <remarks>
		/// This property represents whether the visibility range component is enabled or disabled.
		/// </remarks>
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
		
		private readonly string _Title = "Visibility Range";
		private readonly string _Tooltip = "Value is specified in meters. And is calculated from the Camera position.";
		private readonly string _MarginTooltip = "The margin will be taken into consideration when the calculation is made.";

		/// <summary>
		/// Constructor for the visibility range component.
		/// </summary>
		public VisibilityRange()
		{
			Name = "LSVisibilityRange";
			
			_UsingTraits = new()
			{
				{ typeof(Labelable).ToString() },
				{ typeof(Checkable).ToString() },
				{ typeof(Spinboxable).ToString() },
				{ typeof(Selectable).ToString() },
			};
			
			//_include = false;
		}
		
		/// <summary>
		/// Gets the state of the visibility range component.
		/// </summary>
		/// <returns>A boolean value indicating the state of the component.</returns>
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
		/// Initializes the visibility range component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			_Initiated = true;
			
			Trait<Labelable>()
				.SetName("VisibilityRangeText")
				.SetType(Labelable.TitleType.TextSmall)
				.SetText( "Control when objects should be visible, and if they should fade out or simply hide. Keep 0 for default values." )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(2, "top")
				.SetMargin(2, "bottom")
				.SetAutoWrap(TextServer.AutowrapMode.Word)
				.Instantiate()
				.Select(0)
				.AddToContainer( this );
				
			Trait<Spinboxable>()
				.SetName("VisibilityRangeBegin")
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(2, "bottom")
				.SetPrefix("Begin: ")
				.SetStep(0.1f)
				.SetMinValue(0)
				.SetTooltipText(_Tooltip)
				.SetAction(Callable.From((double value) => { _OnVisibilityRangeBeginChanged(value); }))
				.Instantiate()
				.Select(0)
				.AddToContainer( this );
				
			Trait<Spinboxable>()
				.SetName("VisibilityRangeBeginMargin")
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(10, "bottom")
				.SetPrefix("Begin Margin: ")
				.SetStep(0.1f)
				.SetMinValue(0)
				.SetTooltipText(_MarginTooltip)
				.SetAction(Callable.From((double value) => { _OnVisibilityRangeBeginMarginChanged(value); }))
				.Instantiate()
				.Select(1)
				.AddToContainer( this );
				
			Trait<Spinboxable>()
				.SetName("VisibilityRangeEnd")
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(2, "bottom")
				.SetPrefix("End: ")
				.SetStep(0.1f)
				.SetMinValue(0)
				.SetTooltipText(_Tooltip)
				.SetAction(Callable.From((double value) => { _OnVisibilityRangeEndChanged(value); }))
				.Instantiate()
				.Select(2)
				.AddToContainer( this );
				
			Trait<Spinboxable>()
				.SetName("VisibilityRangeEndMargin")
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(10, "bottom")
				.SetPrefix("End Margin: ")
				.SetStep(0.1f)
				.SetMinValue(0)
				.SetTooltipText(_MarginTooltip)
				.SetAction(Callable.From((double value) => { _OnVisibilityRangeEndMarginChanged(value); }))
				.Instantiate()
				.Select(3)
				.AddToContainer( this );
						
			Trait<Labelable>()
				.SetName("VisibilityRangeFadeModeText")
				.SetType(Labelable.TitleType.TextSmall)
				.SetText( "Fade mode" )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(2, "top")
				.SetMargin(0, "bottom")
				.SetAutoWrap(TextServer.AutowrapMode.Word)
				.Instantiate()
				.Select(1)
				.AddToContainer( this );
					
			Trait<Selectable>()
				.SetType(Selectable.Type.SelectableSmall)
				.SetName("VisibilityRangeFadeMode")
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(0, "top")
				.SetMargin(10, "bottom")
				.SetAction(( int index ) => { _OnVisibilityFadeModeChanged(index); })
				.SetText( "Fade mode" )
				.AddItem("Use project default")
				.AddItem("Disabled")
				.AddItem("Self")
				.AddItem("Dependencies")
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
		/// Resets the visibility range component.
		/// </summary>
		public void Reset()
		{
			// StatesUtils.Get().VisibilityRangeState = GlobalStates.LibraryStateEnum.Disabled;
			// StatesUtils.Get().VisibilityRange = 0.0f;
			State = false;
		}
		
		/// <summary>
		/// Checks whether the visibility range component is valid.
		/// </summary>
		/// <returns>A boolean value indicating whether the component is valid.</returns>
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
		/// Synchronizes the value of the visibility range component to a global central state controller.
		/// </summary>
		public override void Sync() 
		{
			// if( StatesUtils.Get().VisibilityRangeState == GlobalStates.LibraryStateEnum.Enabled ) 
			// {
			// 	StatesUtils.Get().VisibilityRange = (float)Trait<Spinboxable>().Select(0).GetValue();
			// }
		}
		
		/// <summary>
		/// Handles the event when the visibility fade mode changes.
		/// </summary>
		/// <param name="index">The index of the selected item.</param>
		private void _OnVisibilityFadeModeChanged( int index )
		{
			OptionButton selectable = Trait<Selectable>().Select(0).GetNode() as OptionButton;
			StatesUtils.Get().VisibilityFadeMode = selectable.GetItemText(index);
		}
		
		/// <summary>
		/// Handles the event when the visibility range begin value changes.
		/// </summary>
		/// <param name="value">The new value of the visibility range begin.</param>
		private void _OnVisibilityRangeBeginChanged( double value ) 
		{
			StatesUtils.Get().VisibilityRangeBegin = (float)value;
		}
		
		/// <summary>
		/// Handles the event when the visibility range begin margin value changes.
		/// </summary>
		/// <param name="value">The new value of the visibility range begin margin.</param>
		private void _OnVisibilityRangeBeginMarginChanged( double value ) 
		{
			StatesUtils.Get().VisibilityRangeBeginMargin = (float)value;
		}
		
		/// <summary>
		/// Handles the event when the visibility range end value changes.
		/// </summary>
		/// <param name="value">The new value of the visibility range end.</param>
		private void _OnVisibilityRangeEndChanged( double value ) 
		{
			StatesUtils.Get().VisibilityRangeEnd = (float)value;
		}
		
		/// <summary>
		/// Handles the event when the visibility range end margin value changes.
		/// </summary>
		/// <param name="value">The new value of the visibility range end margin.</param>
		private void _OnVisibilityRangeEndMarginChanged( double value ) 
		{
			StatesUtils.Get().VisibilityRangeEndMargin = (float)value;
		}
		
		/// <summary>
		/// Checks if the checkbox is checked.
		/// </summary>
		/// <returns>A boolean value indicating whether the checkbox is checked.</returns>
		private bool _IsCheckboxChecked()
		{
			return State == true;
		}
	}
}

#endif