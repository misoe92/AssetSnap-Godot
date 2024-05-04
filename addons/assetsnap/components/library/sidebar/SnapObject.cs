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
	/// Partial class representing a SnapObject component, used in the sidebar of the library.
	/// </summary>
	[Tool]
	public partial class SnapObject : LSObjectComponent
	{
		/// <summary>
		/// Gets or sets the state of the SnapObject.
		/// </summary>
		/// <value>The state of the SnapObject.</value>
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
		
		private readonly string _Title = "Snap Object";
		private readonly string _CheckboxTitle = "Snap to objects";
		private readonly string _CheckboxTooltip = "When enabled the object you spawn will snap to other objects close by";

		/// <summary>
		/// Constructor for the SnapObject component.
		/// </summary>
		public SnapObject()
		{
			Name = "LSSnapObject";
			
			_UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
			
			//_include = false;
		}
		
		/// <summary>
		/// Gets the state of the SnapObject.
		/// </summary>
		/// <returns>The state of the SnapObject.</returns>
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
		/// Initializes the SnapObject component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });
			
			_Initiated = true;
		
			Trait<Checkable>()
				.SetName("SnapObjectCheckbox")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(2, "top")
				.SetMargin(10, "bottom")
				.SetText(_CheckboxTitle)
				.SetTooltipText(_CheckboxTooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer( this );
	
			Plugin.GetInstance().StatesChanged += (Godot.Collections.Array data) => { _MaybeUpdateValue(); };
		}
		
		/// <summary>
		/// Handles synchronization of the checkboxes to match the state of the model.
		/// </summary>	
		public void _MaybeUpdateValue()
		{
			if( 
				false == IsValid()
			) 
			{
				return;
			}
			
			if( IsSnapToObject() && false == IsCheckboxChecked() ) 
			{
				State = true;
			}
			else if( false == IsSnapToObject() && true == IsCheckboxChecked() ) 
			{
				State = false;
			}
		}
		
		/// <summary>
		/// Resets the SnapObject component.
		/// </summary>
		public void Reset()
		{
			StatesUtils.Get().SnapToObject = GlobalStates.LibraryStateEnum.Disabled;
			State = false;
		}
			
		/// <summary>
		/// Synchronizes the SnapObject's value to a global central state controller.
		/// </summary>
		public override void Sync() 
		{
			StatesUtils.Get().SnapToObject = State ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
		}
		
		/// <summary>
		/// Checks if the SnapObject component is valid.
		/// </summary>
		/// <returns>True if the SnapObject component is valid, otherwise false.</returns>
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
		/// Updates spawn settings values on change.
		/// </summary>
		private void _OnCheckboxPressed()
		{
			bool state = false;
			
			if( false == IsSnapToObject() ) 
			{
				StatesUtils.Get().SnapToObject = GlobalStates.LibraryStateEnum.Enabled;
				
				Trait<Checkable>()
					.Select(0)
					.SetMargin(0, "bottom");
					
				state = true;
			}
			else 
			{
				Trait<Checkable>()
					.Select(0)
					.SetMargin(10, "bottom");
					
				StatesUtils.Get().SnapToObject = GlobalStates.LibraryStateEnum.Disabled;
			}
									
			UpdateSpawnSettings("SnapToObject", state);
		}
		
		/// <summary>
		/// Checks if the SnapObject's checkbox is checked.
		/// </summary>
		/// <returns>True if the checkbox is checked, otherwise false.</returns>
		private bool IsCheckboxChecked()
		{
			return State == true;
		}
	}
}

#endif