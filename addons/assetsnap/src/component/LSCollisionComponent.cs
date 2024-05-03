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
	/// Base class for components related to Library Collision Snap.
	/// </summary>
	public partial class LSCollisionComponent : LibraryComponent
	{
		protected string Type = "";
		private bool _state = false;
		protected bool state 
		{
			get => _state;
			set 
			{
				_state = value;
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LSCollisionComponent"/> class.
		/// </summary>
		public LSCollisionComponent()
		{
			Name = "LSCollisionComponent";
			//_include = false;
		}

		/// <summary>
		/// Initializes the component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
		}

		/// <summary>
		/// Updates the value of the component based on the provided data.
		/// </summary>
		/// <param name="data">The data to update the component with.</param>
		public virtual void MaybeUpdateValue(Godot.Collections.Array data)
		{
			if( ShouldUpdateState() ) 
			{
				UpdateState();
			}
		}
		
		/// <summary>
		/// Determines whether the state of the component should be updated.
		/// </summary>
		/// <returns><c>true</c> if the state should be updated; otherwise, <c>false</c>.</returns>
		public bool ShouldUpdateState()
		{
			return
				IsActive() && false == CheckboxPressed() ||
				false == IsActive() && CheckboxPressed();
		}
		
		/// <summary>
		/// Updates the state of the component.
		/// </summary>
		public void UpdateState()
		{
			if(IsActive() && false == CheckboxPressed() ) 
			{
				SetState(true);
			}
			else if( false == IsActive() && CheckboxPressed() ) 
			{
				SetState(false);
			}
		}
		
		/// <summary>
		/// Sets the state of the component.
		/// </summary>
		/// <param name="state">The state to set.</param>
		public void SetState( bool state ) 
		{
			if( false == IsValid() ) 
			{
				return;
			}
			
			Trait<Checkable>().Select(0).SetValue( state );
		}
		
		/// <summary>
		/// Gets the state of the component.
		/// </summary>
		/// <returns>The state of the component.</returns>
		public bool GetState() 
		{
			if( false == IsValid() ) 
			{
				return false;
			}
			
			return Trait<Checkable>().Select(0).GetValue();
		}
		
		/// <summary>
		/// Checks if the checkbox associated with the component is pressed.
		/// </summary>
		/// <returns><c>true</c> if the checkbox is pressed; otherwise, <c>false</c>.</returns>
		protected bool CheckboxPressed()
		{
			return GetState();
		}
		
		/// <summary>
		/// Fetches the component checkbox.
		/// </summary>
		/// <returns>The checkbox associated with the component.</returns>
		public CheckBox GetCheckbox()
		{
			if( false == IsValid() ) 
			{
				return null;
			}
			
			return Trait<Checkable>().Select(0).GetNode() as CheckBox;
		}
		
		/// <summary>
		/// Determines whether the component is active.
		/// </summary>
		/// <returns><c>true</c> if the component is active; otherwise, <c>false</c>.</returns>
		public virtual bool IsActive()
		{
			return false;
		}
		
		/// <summary>
		/// Checks if the component is valid.
		/// </summary>
		/// <returns><c>true</c> if the component is valid; otherwise, <c>false</c>.</returns>
		public bool IsValid()
		{
			return
				null != StatesUtils.Get() &&
				false != Initiated &&
				null != Trait<Checkable>() &&
				false != HasTrait<Checkable>();
		}		

		/// <summary>
        /// Called when the node is about to be removed from the scene tree.
        /// </summary>
		public override void _ExitTree()
		{
			Initiated = false;
			base._ExitTree();
		}
	}
}

#endif