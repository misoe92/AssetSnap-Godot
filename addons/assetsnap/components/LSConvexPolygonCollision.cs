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
	using System.Transactions;
	using AssetSnap.Component;
	using AssetSnap.Front.Nodes;
	using Godot;

	public partial class LSConvexPolygonCollision : LibraryComponent
	{
		private readonly string _Title = "Convex Polygon";
		private readonly string _cleanTitle = "Clean";
		private readonly string _simplifyTitle = "Simplify";
		private readonly string _CheckboxTooltip = "Use with caution, since this method is more expensive than a simple collision shape.";
		private readonly string _cleanCheckboxTooltip = "If clean is true (default), duplicate and interior vertices are removed automatically. You can set it to false to make the process faster if not needed.";
		private readonly string _simplifyCheckboxTooltip = "If simplify is true, the geometry can be further simplified to reduce the number of vertices. Disabled by default.";
		private bool Exited = false;
		
		private MarginContainer _MarginContainer;
		private MarginContainer _ValuesMarginContainer;
		private VBoxContainer _InnerContainer;
		private VBoxContainer _ValuesInnerContainer;
		private CheckBox _Checkbox;
		private CheckBox _cleanCheckbox;
		private CheckBox _simplifyCheckbox;
		
		private Callable? _CheckboxCallable;
		private Callable? _cleanCheckboxCallable;
		private Callable? _simplifyCheckboxCallable;
		
		public bool state = false;
		public bool clean = false;
		public bool simplify = false;
		 
		/*
		** Constructor of the component
		**
		** @return void
		*/
		public LSConvexPolygonCollision()
		{
			Name = "LSConvexPolygonCollision";
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
				_ValuesMarginContainer = new();
				_ValuesInnerContainer = new();
				
				
				_Checkbox = new()
				{
					Text = _Title,
					TooltipText = _CheckboxTooltip,
				};
				
				_cleanCheckbox = new()
				{
					Text = _cleanTitle,
					TooltipText = _cleanCheckboxTooltip,
				};
				
				_simplifyCheckbox = new()
				{
					Text = _simplifyTitle,
					TooltipText = _simplifyCheckboxTooltip,
				};
				
				_CheckboxCallable = new(this, "_OnCheckboxPressed");
				_cleanCheckboxCallable = new(this, "_OnCleanCheckboxPressed");
				_simplifyCheckboxCallable = new(this, "_OnSimplifyCheckboxPressed");
				
				_MarginContainer.AddThemeConstantOverride("margin_left", 10); 
				_MarginContainer.AddThemeConstantOverride("margin_right", 10);
				_MarginContainer.AddThemeConstantOverride("margin_top", 2);
				_MarginContainer.AddThemeConstantOverride("margin_bottom", 2);
				
				_ValuesMarginContainer.AddThemeConstantOverride("margin_left", 20); 
				_ValuesMarginContainer.AddThemeConstantOverride("margin_right", 20);
				_ValuesMarginContainer.AddThemeConstantOverride("margin_top", 0);
				_ValuesMarginContainer.AddThemeConstantOverride("margin_bottom", 0);
				
				if( _CheckboxCallable is Callable _checkboxCallable) 
				{
					_Checkbox.Connect(CheckBox.SignalName.Pressed,_checkboxCallable);
				}
				
				if( _cleanCheckboxCallable is Callable _CleanCheckboxCallable) 
				{
					_cleanCheckbox.Connect(CheckBox.SignalName.Pressed,_CleanCheckboxCallable);
				}
				
				if( _simplifyCheckboxCallable is Callable _SimplifyCheckboxCallable) 
				{
					_simplifyCheckbox.Connect(CheckBox.SignalName.Pressed,_SimplifyCheckboxCallable);
				}
				
				_InnerContainer.AddChild(_Checkbox);
				_ValuesInnerContainer.AddChild(_cleanCheckbox);
				_ValuesInnerContainer.AddChild(_simplifyCheckbox);
				_MarginContainer.AddChild(_InnerContainer);
				_ValuesMarginContainer.AddChild(_ValuesInnerContainer);
				
				BoxContainer.AddChild(_MarginContainer);
				BoxContainer.AddChild(_ValuesMarginContainer);
			}
		}

		/*
		** Synchronizes the state of various checkboxes with
		** their internal state.
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
			
			if( _cleanCheckbox != null && clean && _cleanCheckbox.ButtonPressed == false ) 
			{
				_cleanCheckbox.ButtonPressed = true;
			}
			else if( _cleanCheckbox != null && false == clean && _cleanCheckbox.ButtonPressed == true ) 
			{
				_cleanCheckbox.ButtonPressed = false;
			}
			
			if( _simplifyCheckbox != null && simplify && _simplifyCheckbox.ButtonPressed == false ) 
			{
				_simplifyCheckbox.ButtonPressed = true;
			}
			else if( _simplifyCheckbox != null && false == simplify && _simplifyCheckbox.ButtonPressed == true ) 
			{
				_simplifyCheckbox.ButtonPressed = false;
			}
			
			if( _cleanCheckbox != null && state && _cleanCheckbox.Visible == false )  
			{
				_cleanCheckbox.Visible = true;
			}
			else if( _cleanCheckbox != null && false == state && _cleanCheckbox.Visible == true )  
			{
				_cleanCheckbox.Visible = false;
			}
			
			if( null != _simplifyCheckbox && state && false ==_simplifyCheckbox.Visible )  
			{
				_simplifyCheckbox.Visible = true;
			}
			else if( null != _simplifyCheckbox && false == state && true == _simplifyCheckbox.Visible )  
			{
				_simplifyCheckbox.Visible = false;
			}
			
			if( null != _ValuesMarginContainer && state && false == _ValuesMarginContainer.Visible ) 
			{
				_ValuesMarginContainer.Visible = true;
			}
			else if( null != _ValuesMarginContainer && false == state && true == _ValuesMarginContainer.Visible ) 
			{
				_ValuesMarginContainer.Visible = false;
			}
			
			base._Process(delta);
		}

		/*
		** Updates the state 
		** and updates staticbody collision
		**
		** @return void
		*/
		private void _OnCheckboxPressed()
		{
			state = !state;

			Node3D handle = _GlobalExplorer.GetHandle();
			
			if( state == true ) 
			{
				Library._LibrarySettings._LSConcaveCollision.GetCheckbox().ButtonPressed = false;
				Library._LibrarySettings._LSConcaveCollision.state = false;
				Library._LibrarySettings._LSSimpleSphereCollision.GetCheckbox().ButtonPressed = false;
				Library._LibrarySettings._LSSimpleSphereCollision.state = false;
			}
			
			if( handle is AssetSnap.Front.Nodes.AsMeshInstance3D meshInstance3D ) 
			{
				if( meshInstance3D.GetParent() is AsStaticBody3D staticBody3D )
				{
					staticBody3D.UpdateCollision();
				}
			}
			
			string key = "_LSConvexPolygonCollision.state";
			UpdateSpawnSettings(key, state);
		}
		
		/*
		** Updates the clean state 
		** and updates staticbody collision
		**
		** @return void
		*/
		private void _OnCleanCheckboxPressed()
		{
			clean = !clean;
			
			Node3D handle = _GlobalExplorer.GetHandle();
			if( handle is AssetSnap.Front.Nodes.AsMeshInstance3D meshInstance3D ) 
			{
				if( meshInstance3D.GetParent() is AsStaticBody3D staticBody3D )
				{
					staticBody3D.UpdateCollision();
				}
			}
		}
							
		/*
		** Updates the simplify state 
		** and updates staticbody collision
		**
		** @return void
		*/
		private void _OnSimplifyCheckboxPressed()
		{
			simplify = !simplify;
			
			Node3D handle = _GlobalExplorer.GetHandle();
			if( handle is AssetSnap.Front.Nodes.AsMeshInstance3D meshInstance3D ) 
			{
				if( meshInstance3D.GetParent() is AsStaticBody3D staticBody3D )
				{
					staticBody3D.UpdateCollision();
				}
			}
		}
					
		/*
		** Fetches the main checkbox
		**
		** @return CheckBox
		*/
		public CheckBox GetCheckbox()
		{
			return _Checkbox;
		}
					
		/*
		** Checks if the collision should use
		** the ConvexPolygon collision
		**
		** @return bool
		*/
		public bool IsActive() 
		{
			return state == true;
		}
					
		/*
		** Checks if the collision should use
		** the internal extension "Clean"
		**
		** @return bool
		*/
		public bool ShouldClean() 
		{
			return clean == true;
		}
					
		/*
		** Checks if the collision should use
		** the internal extension "Simplify"
		**
		** @return bool
		*/
		public bool ShouldSimplify() 
		{
			return simplify == true;
		}
					
		/*
		** Resets the state back to disabled
		**
		** @return void
		*/
		public void Reset()
		{
			state = false;
			clean = false;
			simplify = false;
		}
			
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			Exited = true;
			
			if( IsInstanceValid(_Checkbox) && _Checkbox != null && _CheckboxCallable is Callable _checkboxCallable) 
			{
				if( _Checkbox.IsConnected(CheckBox.SignalName.Pressed, _checkboxCallable)) 
				{
					_Checkbox.Disconnect(CheckBox.SignalName.Pressed,_checkboxCallable);
				}
			}
			
			if( IsInstanceValid(_cleanCheckbox) && _cleanCheckbox != null && _cleanCheckboxCallable is Callable _CleanCheckboxCallable) 
			{
				if( _cleanCheckbox.IsConnected(CheckBox.SignalName.Pressed, _CleanCheckboxCallable)) 
				{
					_cleanCheckbox.Disconnect(CheckBox.SignalName.Pressed,_CleanCheckboxCallable);
				}
			}
			
			if( IsInstanceValid(_simplifyCheckbox) && _simplifyCheckbox != null && _simplifyCheckboxCallable is Callable _SimplifyCheckboxCallable) 
			{
				if( _simplifyCheckbox.IsConnected(CheckBox.SignalName.Pressed, _SimplifyCheckboxCallable)) 
				{
					_simplifyCheckbox.Disconnect(CheckBox.SignalName.Pressed,_SimplifyCheckboxCallable);
				}
			}
			
			if( IsInstanceValid(_Checkbox)) 
			{
				_Checkbox.QueueFree();
				_Checkbox = null;
			}
			
			if( IsInstanceValid(_cleanCheckbox) ) 
			{
				_cleanCheckbox.QueueFree();
				_cleanCheckbox = null;
			}
			
			if( IsInstanceValid(_simplifyCheckbox) ) 
			{
				_simplifyCheckbox.QueueFree();
				_simplifyCheckbox = null;
			}
			
			if( IsInstanceValid(_ValuesInnerContainer) ) 
			{
				_ValuesInnerContainer.QueueFree();
				_ValuesInnerContainer = null;
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
			
			if( IsInstanceValid(_ValuesMarginContainer ) ) 
			{
				_ValuesMarginContainer.QueueFree();
				_ValuesMarginContainer = null;
			}
		}
	}
}