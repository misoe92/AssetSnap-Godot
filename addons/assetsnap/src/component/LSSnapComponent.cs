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

using AssetSnap.States;
using Godot;

namespace AssetSnap.Component
{
	/// <summary>
	/// Base class for components related to Library Snap.
	/// </summary>	
	public partial class LSSnapComponent : LibraryComponent
	{
		public float _value = 0.0f;
		public bool state = false;
		public bool UsingGlue = false;
		
		protected GlobalStates.SnapAngleEnums Angle;
		protected Callable _SpinBoxCallable;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LSSnapComponent"/> class.
		/// </summary>
		public LSSnapComponent()
		{
			Name = "LSSnapComponent";
			//_include = false;
		}
		
		/// <summary>
		/// Updates the values based on the snap settings.
		/// </summary>
		/// <param name="data">Array of data.</param>
		public virtual void MaybeUpdateValue(Godot.Collections.Array data)
		{
			if( _IsSnapTo() && false == _IsSnapToChecked() ) 
			{
				Trait<Checkable>().Select(0).SetValue(true);
			}
			else if( false == _IsSnapTo() && true == _IsSnapToChecked() )
			{
				Trait<Checkable>().Select(0).SetValue(false);	
			}
			
			if( _IsSnapToGlue() && false == _IsSnapToGlueChecked() ) 
			{
				Trait<Checkable>().Select(1).SetValue(true);
			}
			else if( false == _IsSnapToGlue() && true == _IsSnapToGlueChecked() )
			{
				Trait<Checkable>().Select(1).SetValue(false);	
			}
			
			if (_IsSnapTo() && false == _IsGlueCheckboxShown() ) 
			{
				Trait<Checkable>().Select(1).SetValue(true);
				Trait<Checkable>().Select(0).SetMargin( 2, "bottom" );
				Trait<Checkable>().Select(1).SetVisible(true);
				Trait<Spinboxable>().Select(0).SetVisible(true);
			}
			else if( false == _IsSnapTo() && true == _IsGlueCheckboxShown() ) 
			{
				Trait<Checkable>().Select(0).SetMargin( 10, "bottom" );
				Trait<Checkable>().Select(1).SetVisible(false);
				Trait<Checkable>().Select(1).SetValue(false);
				Trait<Spinboxable>().Select(0).SetVisible(false);
			}
		}
		
		/// <summary>
		/// Sets the state of the main checkbox.
		/// </summary>
		/// <param name="state">The state to set.</param>
		public void SetState( bool state )
		{
			if( IsValid() ) 
			{
				Trait<Checkable>().Select(0).SetValue(state);
				Trait<Checkable>().Select(1).SetValue(state);
			}
		}
			
		/// <summary>
		/// Applies glue to the boundary's Y axis.
		/// </summary>
		/// <param name="Origin">The original vector.</param>
		/// <returns>The modified vector with glue applied.</returns>
		public Vector3 ApplyGlue( Vector3 Origin )
		{
			Origin.Y = _GlobalExplorer.States.SnapToHeightValue;
			return Origin;
		}
		
		/// <summary>
		/// Checks if the component is valid.
		/// </summary>
		/// <returns><c>true</c> if the component is valid; otherwise, <c>false</c>.</returns>	
		public bool IsValid()
		{
			return
				null != StatesUtils.Get() &&
				false != _Initiated &&
				null != Trait<Checkable>() &&
				false != HasTrait<Checkable>();
		}
		
		/// <summary>
		/// Resets the component to its default state.
		/// </summary>
		public void Reset()
		{
			state = false;
			UsingGlue = false;
			_value = 0.0f;
		}
		
		/// <summary>
		/// Checks if Snap to height is enabled.
		/// </summary>
		/// <returns><c>true</c> if Snap to height is enabled; otherwise, <c>false</c>.</returns>
		protected bool _IsSnapTo()
		{
			switch( Angle ) 
			{
				case GlobalStates.SnapAngleEnums.X:
					return _GlobalExplorer.States.SnapToX == GlobalStates.LibraryStateEnum.Enabled;
					
				case GlobalStates.SnapAngleEnums.Y:
					return _GlobalExplorer.States.SnapToHeight == GlobalStates.LibraryStateEnum.Enabled;
					
				case GlobalStates.SnapAngleEnums.Z:
					return _GlobalExplorer.States.SnapToZ == GlobalStates.LibraryStateEnum.Enabled;
			}

			return false;
		}

		/// <summary>
		/// Checks if Snap to height glue is enabled.
		/// </summary>
		/// <returns><c>true</c> if Snap to height glue is enabled; otherwise, <c>false</c>.</returns>
		protected bool _IsSnapToGlue()
		{
			switch( Angle ) 
			{
				case GlobalStates.SnapAngleEnums.X:
					return _GlobalExplorer.States.SnapToXGlue == GlobalStates.LibraryStateEnum.Enabled;
					
				case GlobalStates.SnapAngleEnums.Y:
					return _GlobalExplorer.States.SnapToHeightGlue == GlobalStates.LibraryStateEnum.Enabled;
					
				case GlobalStates.SnapAngleEnums.Z:
					return _GlobalExplorer.States.SnapToZGlue == GlobalStates.LibraryStateEnum.Enabled;
			}

			return false;
		}
		
		/// <summary>
		/// Checks if Snap to x is checked.
		/// </summary>
		/// <returns><c>true</c> if Snap to x is checked; otherwise, <c>false</c>.</returns>
		protected bool _IsSnapToChecked()
		{
			if( IsValid() ) 
			{
				return Trait<Checkable>().Select(0).GetValue();
			}

			return false;
		}
		
		/// <summary>
		/// Checks if Snap to x glue is checked.
		/// </summary>
		/// <returns><c>true</c> if Snap to x glue is checked; otherwise, <c>false</c>.</returns>
		protected bool _IsSnapToGlueChecked()
		{
			if( IsValid() ) 
			{
				return Trait<Checkable>().Select(1).GetValue();
			}

			return false;
		}
				
		/// <summary>
		/// Checks if glue checkbox is shown.
		/// </summary>
		/// <returns><c>true</c> if glue checkbox is shown; otherwise, <c>false</c>.</returns>
		protected bool _IsGlueCheckboxShown()
		{
			if( IsValid() ) 
			{
				return Trait<Checkable>().Select(1).IsVisible();
			}
			
			return false;
		}
	}
}

#endif