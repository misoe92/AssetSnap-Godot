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
	using System;
	using AssetSnap.Component;
	using Godot;

	public partial class LibraryBody : LibraryComponent
	{
		private	HBoxContainer _MainContainer;
		
		private VBoxContainer LeftContainer;
		private MarginContainer LeftMarginContainer;
		private VBoxContainer LeftInnerContainer;
		
		private VBoxContainer RightContainer;
		private MarginContainer RightMarginContainer;
		private VBoxContainer RightInnerContainer;
		
		
		/*
		** Constructor of component
		** 
		** @return void
		*/
		public LibraryBody()
		{
			Name = "LibraryBody";
			// _include = false;
		}
		
		/*
		** Initialization of component
		** 
		** @return void
		*/
		public override void Initialize()
		{
			if( Container is PanelContainer PanelContainer ) 
			{
				_MainContainer = new()
				{
					Name = "MainContainer",
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
					SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				};
				
				_SetupLeftSide();
				_SetupRightSide();

				PanelContainer.AddChild(_MainContainer);
			}
		}
			
		/*
		** Set's up the left side of the container body
		** 
		** @return void
		*/
		public void _SetupLeftSide()
		{
			LeftContainer = new()
			{
				Name = "LeftContainer",
				SizeFlagsHorizontal = 0,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				CustomMinimumSize = new Vector2(200, 0)
			};
			LeftMarginContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill, 
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};
			LeftInnerContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};
			
			LeftMarginContainer.AddThemeConstantOverride("margin_left", 5);
			LeftMarginContainer.AddThemeConstantOverride("margin_right", 5);
			LeftMarginContainer.AddThemeConstantOverride("margin_top", 5);
			LeftMarginContainer.AddThemeConstantOverride("margin_bottom", 5);

			LeftMarginContainer.AddChild(LeftInnerContainer);
			LeftContainer.AddChild(LeftMarginContainer);
			_MainContainer.AddChild(LeftContainer);
		}
		
			
		/*
		** Set's up the right side of the container body
		** 
		** @return void
		*/
		public void _SetupRightSide()
		{
			RightContainer = new()
			{
				Name = "RightContainer",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};
			
			RightMarginContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};
			RightInnerContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};
			
			RightMarginContainer.AddThemeConstantOverride("margin_left", 5);
			RightMarginContainer.AddThemeConstantOverride("margin_right", 5);
			RightMarginContainer.AddThemeConstantOverride("margin_top", 5);
			RightMarginContainer.AddThemeConstantOverride("margin_bottom", 5);
			
			RightMarginContainer.AddChild(RightInnerContainer);
			RightContainer.AddChild(RightMarginContainer);
			_MainContainer.AddChild(RightContainer);
		}
		
		/*
		** Fetches the left inner container
		**
		** @return VBoxContainer
		*/
		public VBoxContainer GetLeftInnerContainer() 
		{
			return LeftInnerContainer;
		}
		
		/*
		** Fetches the right inner container
		**
		** @return VBoxContainer
		*/
		public VBoxContainer GetRightInnerContainer() 
		{
			return RightInnerContainer;
		}
			
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			if( IsInstanceValid(RightInnerContainer) ) 
			{
				RightInnerContainer.QueueFree();
				RightInnerContainer = null;
			}
			
			if( IsInstanceValid(RightMarginContainer) ) 
			{
				RightMarginContainer.QueueFree();
				RightMarginContainer = null;
			}
			
			if( IsInstanceValid(RightContainer) ) 
			{
				RightContainer.QueueFree();
				RightContainer = null;
			}

			if( IsInstanceValid(LeftInnerContainer) ) 
			{
				LeftInnerContainer.QueueFree();
				LeftInnerContainer = null;
			} 
			
			if( IsInstanceValid(LeftMarginContainer) ) 
			{
				LeftMarginContainer.QueueFree();
				LeftMarginContainer = null;
			}
			
			if( IsInstanceValid(LeftContainer) ) 
			{
				LeftContainer.QueueFree();
				LeftContainer = null;
			}
			
			if( IsInstanceValid(_MainContainer) ) 
			{
				// _MainContainer.GetParent().RemoveChild(_MainContainer);
				_MainContainer.QueueFree(); 
				_MainContainer = null;
			}
		}
	}
}