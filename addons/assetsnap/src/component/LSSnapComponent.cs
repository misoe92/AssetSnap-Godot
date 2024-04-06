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

namespace AssetSnap.Component
{
	using Godot;

	public partial class LSSnapComponent : LibraryComponent
	{
		protected GlobalStates.SnapAngleEnums Angle;
		
		/** Callables **/
		protected Callable _SpinBoxCallable;
		
		public float _value = 0.0f;
		public bool state = false;
		public bool UsingGlue = false;
		
		public LSSnapComponent()
		{
			Name = "LSSnapComponent";
			//_include = false;
		}
		
		public virtual void MaybeUpdateValue()
		{
			if( IsSnapTo() && false == IsSnapToChecked() ) 
			{
				Trait<Checkable>().Select(0).SetValue(true);
			}
			else if( false == IsSnapTo() && true == IsSnapToChecked() )
			{
				Trait<Checkable>().Select(0).SetValue(false);	
			}
			
			if( IsSnapToGlue() && false == IsSnapToGlueChecked() ) 
			{
				Trait<Checkable>().Select(1).SetValue(true);
			}
			else if( false == IsSnapToGlue() && true == IsSnapToGlueChecked() )
			{
				Trait<Checkable>().Select(1).SetValue(false);	
			}
			
			if (IsSnapTo() && false == IsGlueCheckboxShown() ) 
			{
				Trait<Checkable>().Select(1).SetValue(true);
				Trait<Checkable>().Select(0).SetMargin( 2, "bottom" );
				Trait<Checkable>().Select(1).SetVisible(true);
				Trait<Spinboxable>().Select(0).SetVisible(true);
			}
			else if( false == IsSnapTo() && true == IsGlueCheckboxShown() ) 
			{
				Trait<Checkable>().Select(0).SetMargin( 10, "bottom" );
				Trait<Checkable>().Select(1).SetVisible(false);
				Trait<Checkable>().Select(1).SetValue(false);
				Trait<Spinboxable>().Select(0).SetVisible(false);
			}
		}
		
		/*
		** Sets the state of the main checkbox
		** 
		** @param bool state
		** @return void
		*/
		public void SetState( bool state )
		{
			if( IsValid() ) 
			{
				Trait<Checkable>().Select(0).SetValue(state);
				Trait<Checkable>().Select(1).SetValue(state);
			}
		}
			
		/*
		** Applies glue to the boundary's Y axis
		** 
		** @return Vector3
		*/
		public Vector3 ApplyGlue( Vector3 Origin )
		{
			Origin.Y = _GlobalExplorer.States.SnapToHeightValue;
			return Origin;
		}
				  
		/*
		** Checks if Snap to height is enabled
		**
		** @return bool
		*/
		public bool IsSnapTo()
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

		/*
		** Checks if Snap to height glue is enabled
		**
		** @return bool
		*/
		public bool IsSnapToGlue()
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
		
		/*
		** Checks if Snap to x is checked
		**
		** @return bool
		*/
		protected bool IsSnapToChecked()
		{
			if( IsValid() ) 
			{
				return Trait<Checkable>().Select(0).GetValue();
			}

			return false;
		}
		
		/*
		** Checks if Snap to x glue is checked
		**
		** @return bool
		*/
		protected bool IsSnapToGlueChecked()
		{
			if( IsValid() ) 
			{
				return Trait<Checkable>().Select(1).GetValue();
			}

			return false;
		}
				
		/*
		** Checks if glue checkbox is shown
		**
		** @return bool
		*/
		protected bool IsGlueCheckboxShown()
		{
			if( IsValid() ) 
			{
				return Trait<Checkable>().Select(1).IsVisible();
			}
			
			return false;
		}
				
		public bool IsValid()
		{
			return
				null != _GlobalExplorer &&
				null != _GlobalExplorer.States &&
				false != Initiated &&
				null != Trait<Checkable>() &&
				false != HasTrait<Checkable>() &&
				IsInstanceValid( Trait<Checkable>() );
		}
		
		/*
		** Resets the component
		** 
		** @return void
		*/
		public void Reset()
		{
			state = false;
			UsingGlue = false;
			_value = 0.0f;
		}
	}
}