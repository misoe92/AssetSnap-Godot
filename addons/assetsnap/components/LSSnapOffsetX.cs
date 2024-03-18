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

	public partial class LSSnapOffsetX : LibraryComponent
	{
		private readonly string _Title = "Object Snap Offset X";
		private bool Exited = false;
		
		private MarginContainer _MarginContainer;
		private	VBoxContainer _InnerContainer;
		private	Label _Label;
		private	SpinBox _SpinBox;
		private Callable? _SpinBoxCallable;
		
		public float value = 0.0f;
			
		/*
		** Constructor of the component
		** 
		** @return void
		*/	
		public LSSnapOffsetX()
		{
			Name = "LSSnapOffsetX";
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
				_SpinBoxCallable = new(this, "_OnSpinBoxValueChange");
				
				_Label = new()
				{
					ThemeTypeVariation = "HeaderSmall",
					Text = _Title
				};
				
				_SpinBox = new()
				{
					CustomMinimumSize = new Vector2(140, 20),
					MinValue = -200,
					Step = 0.01f
				};
				
				_MarginContainer.AddThemeConstantOverride("margin_left", 10); 
				_MarginContainer.AddThemeConstantOverride("margin_right", 10);
				_MarginContainer.AddThemeConstantOverride("margin_top", 2);
				_MarginContainer.AddThemeConstantOverride("margin_bottom", 2);
				
				if( _SpinBoxCallable is Callable _callable ) 
				{
					_SpinBox.Connect(SpinBox.SignalName.ValueChanged,_callable);
				}
				
				_InnerContainer.AddChild(_Label);
				_InnerContainer.AddChild(_SpinBox);
				_MarginContainer.AddChild(_InnerContainer);
				
				BoxContainer.AddChild(_MarginContainer);
			}
		}
			
		/*
		** Updates spawn settings when
		** input is changed
		** 
		** @return void
		*/	
		private void _OnSpinBoxValueChange(float _value)
		{
			value = _value;
									
			string key = "_LSSnapOffsetX.value";
			UpdateSpawnSettings(key, value);
		}
		
		/*
		** Handles visibility based on the
		** state of object snapping
		** 
		** @return void
		*/	
		public override void _Process(double delta)
		{
			if( false == IsInstanceValid(_Label) ) 
			{
				return;
			}
			
			if( _GlobalExplorer == null || _GlobalExplorer._Plugin == null ) 
			{
				return;
			}
			
			if( Library == null ) 
			{
				return;
			}

			LibrarySettings _Settings = Library._LibrarySettings;
			
			if( _Settings == null ) 
			{
				GD.PushWarning("Settings is null");
				return;
			}

			LSSnapObject _SnapObject = Library._LibrarySettings._LSSnapObject;
			
			if( _SnapObject == null ) 
			{
				GD.PushWarning("Snapobject is null");
				return;
			}
			
			bool SnapToObject = _SnapObject.IsActive();
			
			if( SnapToObject && false == _MarginContainer.Visible) 
			{
				_MarginContainer.Visible = true;
			}
			else if( false == SnapToObject && true == _MarginContainer.Visible)
			{
				_MarginContainer.Visible = false;
			}
		}
		
		/*
		** Fetches the current component value
		** 
		** @return float
		*/	
		public float GetValue()
		{
			return value;
		}
		
		/*
		** Resets the component
		** 
		** @return void
		*/	
		public void Reset()
		{
			value = 0.0f;
		}
			
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			Exited = true;
			
			if( IsInstanceValid(_SpinBox) && _SpinBox != null && _SpinBoxCallable is Callable _callable ) 
			{
				if(  _SpinBox.IsConnected(SpinBox.SignalName.ValueChanged, _callable)) 
				{
					_SpinBox.Disconnect(SpinBox.SignalName.ValueChanged, _callable);
				}
			}
			
			if( IsInstanceValid(_SpinBox) ) 
			{
				_SpinBox.QueueFree();
				_SpinBox = null;
			}
			
			if( IsInstanceValid(_Label) ) 
			{
				_Label.QueueFree();
				_Label = null;
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