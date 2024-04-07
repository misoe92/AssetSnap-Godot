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

	[Tool]
	public partial class AddFolderToLibrary : TraitableComponent
	{
		private readonly string TitleText = "General actions";
		// private FileDialog fileDialog;

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

			base.Initialize();
			
			AddTrait(typeof(Containerable));
			AddTrait(typeof(Labelable));
			AddTrait(typeof(Buttonable));
			
			Initiated = true;
			
			Trait<Containerable>()
				.SetInnerOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetOrientation(Containerable.ContainerOrientation.Vertical)
				.SetMargin(20, "left")
				.Instantiate();
			 
			Trait<Labelable>()
				.SetName("AddFolderButtonTitle")
				.SetType( Labelable.TitleType.HeaderMedium)
				.SetText( TitleText )
				.SetMargin(0, "bottom")
				.Instantiate() 
				.Select(0)
				.AddToContainer( 
					Container
				); 
				
			Trait<Buttonable>()
				.SetName("AddFolderButton")
				.SetText("Add Library")
				.SetType( Buttonable.ButtonType.ActionButton )
				.SetAction( () => { _OnButtonPressed(); } )
				.Instantiate()
				.Select(0)
				.AddToContainer( 
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				 );

			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					Container
				);
		}
		
		/*
		** Handles folder selection when Add Library
		** button is clicked.
		** 
		** @param string FolderPath
		** @return async<void>
		*/
		private void _OnFolderSelected( string FolderPath, FileDialog fileDialog )
		{
			_GlobalExplorer.Settings.AddFolder(FolderPath);
			
			if( null != _GlobalExplorer.Library ) 
			{
				_GlobalExplorer.Library.Refresh(_GlobalExplorer.BottomDock);
			}

			// fileDialog.DirSelected -= _OnFolderSelected;
			// fileDialog.QueueFree();
			// fileDialog = null; 
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
			FileDialog fileDialog = new FileDialog();
			_GlobalExplorer._Plugin.AddChild(fileDialog);

			// Set the dialog mode to "OpenDir" for folder selection
			fileDialog.FileMode = FileDialog.FileModeEnum.OpenDir;
			// Connect the "dir_selected" signal to another Callable function
			fileDialog.DirSelected += (string FolderPath) => { _OnFolderSelected(FolderPath,fileDialog); };
			fileDialog.MinSize = new Vector2I(768, 768);
			// Show the file dialog
			fileDialog.PopupCentered();
		}
		
	}
}