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
	using AssetSnap.Front.Nodes;
	using Godot;

	public partial class LSConcaveCollision : LibraryComponent
	{
		private readonly string _Title = "Concave Polygon";
		private readonly string _CheckboxTooltip = "Use with caution, since this method is more expensive than a simple collision shape.";
		private bool Exited = false;

		private MarginContainer _MarginContainer;
		private VBoxContainer _InnerContainer;
		private CheckBox _Checkbox;
		private Callable? _CheckboxCallable;
	
		public bool state = false;
	
		/*
		** Constructor of component
		**
		** @return void
		*/
		public LSConcaveCollision()
		{
			Name = "LSConcaveCollision";
			// _include = false;
		}
		
		/*
		** Initialization of component
		**
		** @return void
		*/
		public override void Initialize()
		{
			if( Container is VBoxContainer BoxContainer ) 
			{
				_MarginContainer = new();
				_InnerContainer = new();
				
				_Checkbox = new()
				{
					Text = _Title,
					TooltipText = _CheckboxTooltip,
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

				_InnerContainer.AddChild(_Checkbox);
				_MarginContainer.AddChild(_InnerContainer);
				
				BoxContainer.AddChild(_MarginContainer);
			}
		}
				
		/*
		** Keeps the checkbox state synchronized
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
		** Updates spawn settings, collisions
		** and more on checkbox pressed event
		**
		** @return void
		*/
		private void _OnCheckboxPressed()
		{
			state = !state;
			
			Node3D handle = _GlobalExplorer.GetHandle();

			if( state == true ) 
			{
				Library._LibrarySettings._LSConvexPolygonCollision.state = false;
				Library._LibrarySettings._LSConvexPolygonCollision.GetCheckbox().ButtonPressed = false;
				Library._LibrarySettings._LSSimpleSphereCollision.state = false;
				Library._LibrarySettings._LSSimpleSphereCollision.GetCheckbox().ButtonPressed = false;
			}
			
			if( handle is AssetSnap.Front.Nodes.AsMeshInstance3D meshInstance3D ) 
			{
				if( meshInstance3D.GetParent() is AsStaticBody3D staticBody3D )
				{
					staticBody3D.UpdateCollision();
				}
			}
						
			string key = "_LSConcaveCollision.state";
			UpdateSpawnSettings(key, state);
		}
				
		/*
		** Fetches the checkbox
		**
		** @return CheckBox
		*/
		public CheckBox GetCheckbox()
		{
			return _Checkbox;
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
		** Resets the state back to disabled
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
				if( _Checkbox != null ) 
				{
					_Checkbox.QueueFree();
					_Checkbox = null;
				}
			}
			
			if( IsInstanceValid(_InnerContainer) ) 
			{
				_InnerContainer.QueueFree();
				_InnerContainer = null;
			}
			
			if( IsInstanceValid(_MarginContainer) ) 
			{
				_MarginContainer.QueueFree();
				_MarginContainer = null;
			}
		}
	}
}