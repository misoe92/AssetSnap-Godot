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

	public partial class LSSnapObject : LibraryComponent
	{
		private readonly string _Title = "Snap Object";
		private readonly string _CheckboxTitle = "Snap to objects";
		private bool Exited = false;
		
		private MarginContainer _MarginContainer;
		private VBoxContainer _InnerContainer;
		private	Label _Label;
		private CheckBox _Checkbox;
		private Callable? _CheckboxCallable;
		
		public bool state = false;
		
		/*
		** Constructor for the component
		** 
		** @return void
		*/	
		public LSSnapObject()
		{
			Name = "LSSnapObject";
			// _include = false;
		}
		
		/*
		** Initializes the component
		** 
		** @return void
		*/	
		public override void Initialize()
		{
			if( Container is VBoxContainer BoxContainer ) 
			{
				_MarginContainer = new();
				_InnerContainer = new();
					
				_Label = new()
				{
					ThemeTypeVariation = "HeaderSmall",
					Text = _Title
				};
				
				_Checkbox = new()
				{
					Text = _CheckboxTitle
				};
				
				_CheckboxCallable = new(this, "_OnCheckboxPressed");
				
				_MarginContainer.AddThemeConstantOverride("margin_left", 10); 
				_MarginContainer.AddThemeConstantOverride("margin_right", 10);
				_MarginContainer.AddThemeConstantOverride("margin_top", 2);
				_MarginContainer.AddThemeConstantOverride("margin_bottom", 2);
				
				if( _CheckboxCallable is Callable _callable ) 
				{
					_Checkbox.Connect(CheckBox.SignalName.Pressed,_callable);
				}
				
				_InnerContainer.AddChild(_Label);
				_InnerContainer.AddChild(_Checkbox);
				_MarginContainer.AddChild(_InnerContainer);
				BoxContainer.AddChild(_MarginContainer);
			}
		}
		
		/*
		** Handles synchronization of the checkboxes
		** so it matches the state of the model
		** 
		** @return void
		*/	
		public override void _Process(double delta)
		{
			if( false == IsInstanceValid(_Checkbox) ) 
			{
				return;
			}
			
			if( _GlobalExplorer == null || _GlobalExplorer._Plugin == null ) 
			{
				return;
			}
			
			if( _Checkbox != null && state && _Checkbox.ButtonPressed == false ) 
			{
				_Checkbox.ButtonPressed = true;
			}
			else if( _Checkbox != null && false == state && _Checkbox.ButtonPressed == true ) 
			{
				_Checkbox.ButtonPressed = false;
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
			state = !state;
									
			string key = "_LSSnapObject.state";
			UpdateSpawnSettings(key, state);
		}
		
		/*
		** Checks if the component is active
		** 
		** @return bool
		*/	
		public bool IsActive() 
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
			state = false;
			
		}
			
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			Exited = true;
							
			if( IsInstanceValid(_Checkbox) && _Checkbox != null && _CheckboxCallable is Callable _callable ) 
			{
				if( _Checkbox.IsConnected(CheckBox.SignalName.Pressed, _callable)) 
				{
					_Checkbox.Disconnect(CheckBox.SignalName.Pressed,_callable);
				}
			}
			
			if( IsInstanceValid(_Checkbox) ) 
			{
				_Checkbox.QueueFree();
				_Checkbox = null;
			}
			
			if( IsInstanceValid(_MarginContainer) ) 
			{
				_MarginContainer.QueueFree();
				_MarginContainer = null;
			}
		}
	}
}