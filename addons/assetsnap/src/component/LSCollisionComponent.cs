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

	public partial class LSCollisionComponent : LibraryComponent
	{
		protected string Type = "";
		public bool state 
		{
			get => GetState();
			set 
			{
				SetState(value);
			}
		}
		
		public LSCollisionComponent()
		{
			Name = "LSCollisionComponent";
			//_include = false;
		}

		public override void Initialize()
		{
			AddTrait(typeof(Checkable));
			
			base.Initialize();
		}

		public virtual void MaybeUpdateValue()
		{
			if(
				false == IsValid()
			) 
			{
				return;
			}

			if( ShouldUpdateState() ) 
			{
				UpdateState();
			}
		}
		
		public bool ShouldUpdateState()
		{
			return
				IsActive() && false == CheckboxPressed() ||
				false == IsActive() && CheckboxPressed();
		}
		
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
		
		public void SetState( bool state ) 
		{
			if( false == IsValid() ) 
			{
				return;
			}
			
			Trait<Checkable>().Select(0).SetValue( state );
		}
		
		public bool GetState() 
		{
			if( false == IsValid() ) 
			{
				return false;
			}
			
			return Trait<Checkable>().Select(0).GetValue();
		}
		
		protected bool CheckboxPressed()
		{
			return GetState();
		}
		
		/*
		** Fetches the component checkbox
		** 
		** @return CheckBox
		*/
		public CheckBox GetCheckbox()
		{
			if( false == IsValid() ) 
			{
				return null;
			}
			
			return Trait<Checkable>().Select(0).GetNode() as CheckBox;
		}
		
		public virtual bool IsActive()
		{
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

		public override void _ExitTree()
		{
			Initiated = false;
			base._ExitTree();
		}
	}
}