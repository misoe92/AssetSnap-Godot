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
	using Godot;
	using AssetSnap.Component;

	public partial class AddFolderToLibrary : BaseComponent
	{
		private readonly string TitleText = "General actions";
		private Callable? _ButtonPressed;
		private MarginContainer _TitleContainer;
		private HBoxContainer _InnerContainer;
		private Label _Title;
		private MarginContainer _ButtonContainer;
		private Button _Button;
		private FileDialog fileDialog;
			
		/*
		** Class Constructor
		** 
		** @return void  
		*/ 
		public AddFolderToLibrary()
		{
			Name = "AddFolderToLibrary";
			/* Debugging Purpose */ 
			// _include = false; 
			/* -- */ 
		}

		/* 
		** Initializes component
		** 
		** @return void 
		*/
		public override void Initialize()
		{
			if( Container == null ) 
			{
				return;
			}
			
			_ButtonPressed = new Callable(this, "_OnButtonPressed");
			_SetupTitle();
			_SetupButton();
		}
		
		/*
		** Setup title container and label
		** 
		** @return void
		*/
		private void _SetupTitle()
		{
			_TitleContainer = new();
			_InnerContainer = new();
			_Title = new();
			
			_TitleContainer.AddThemeConstantOverride("margin_left", 15);
			_TitleContainer.AddThemeConstantOverride("margin_right", 15);
			_TitleContainer.AddThemeConstantOverride("margin_top", 0);
			_TitleContainer.AddThemeConstantOverride("margin_bottom", 0);

			_Title.ThemeTypeVariation = "HeaderMedium"; 
			_Title.Text = TitleText;

			_InnerContainer.AddChild(_Title);
			_TitleContainer.AddChild(_InnerContainer);
			
			Container.AddChild(_TitleContainer);
		}
		
		/*
		** Setup add library button + it's container
		** 
		** @return void
		*/
		private void _SetupButton()
		{
			_ButtonContainer = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
				SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin,
			};

			_Button = new()
			{
				Name = "AddFolderButton",
				Text = "Add Library"
			};
			
			_ButtonContainer.AddThemeConstantOverride("margin_left", 20);
			_ButtonContainer.AddThemeConstantOverride("margin_right", 20);
			_ButtonContainer.AddThemeConstantOverride("margin_top", 0);
			_ButtonContainer.AddThemeConstantOverride("margin_bottom", 24);
			
			_Button.MouseDefaultCursorShape = Godot.Control.CursorShape.PointingHand;
			
			if( _ButtonPressed is Callable callable ) 
			{
				_Button.Connect("pressed", callable);
			}

			_ButtonContainer.AddChild(_Button);
			
			Container.AddChild(_ButtonContainer);
		}
		
		/*
		** Handles folder selection when Add Library
		** button is clicked.
		** 
		** @param string FolderPath
		** @return async<void>
		*/
		private void _OnFolderSelected( string FolderPath )
		{
			_GlobalExplorer.Settings.AddFolder(FolderPath);
			
			if( null != _GlobalExplorer.Library ) 
			{
				_GlobalExplorer.Library.Refresh(_GlobalExplorer.BottomDock);
			}

			fileDialog.DirSelected -= _OnFolderSelected;
			fileDialog.QueueFree();
			fileDialog = null; 
		}
		
		/* 
		** Handles the button pressed event 
		** of the button
		** 
		** @return void
		*/
		private void _OnButtonPressed()
		{
			// Create a FileDialog instance
			fileDialog = new FileDialog();
			_GlobalExplorer._Plugin.AddChild(fileDialog);

			fileDialog.Mode = Godot.Window.ModeEnum.Maximized;
			// Set the dialog mode to "OpenDir" for folder selection
			fileDialog.FileMode = FileDialog.FileModeEnum.OpenDir;
			// Connect the "dir_selected" signal to another Callable function
			fileDialog.DirSelected += _OnFolderSelected;
			// Show the file dialog
			fileDialog.PopupCentered();
		}
			
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			if( IsInstanceValid(_Button) ) 
			{
				if( _ButtonPressed is Callable callable ) 
				{
					_Button.Disconnect("pressed", callable);
				}
				
				_ButtonPressed = null;
				_Button.QueueFree();
				_Button = null;
			}
			
			if( IsInstanceValid(_Title) ) 
			{
				_Title.QueueFree();
				_Title = null;
			}

			if( IsInstanceValid(_ButtonContainer) ) 
			{
				_ButtonContainer.QueueFree();
				_ButtonContainer = null;
			} 
			
			if( IsInstanceValid(_TitleContainer) ) 
			{
				_TitleContainer.QueueFree();
				_TitleContainer = null;
			}
			
			if( IsInstanceValid(_InnerContainer) ) 
			{
				_InnerContainer.QueueFree();
				_InnerContainer = null;
			}
		}
	}
}