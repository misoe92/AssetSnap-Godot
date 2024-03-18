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
	using System.Collections.Generic;
	using AssetSnap.Component;
	using Godot;

	public partial class LibrariesListing : BaseComponent
	{
		private readonly string TitleText = "Libraries";
		private readonly string NotFoundText = "No folder libraries was found, please add a folder first.";
		private int CurrentFolderCount = 0;
		 
		private MarginContainer _TitleMarginContainer;
		private VBoxContainer _TitleInnerContainer;
		private Label _Title;
		private MarginContainer _FolderListMarginContainer;
		private VBoxContainer _FolderListInnerContainer;
		private Godot.Collections.Array<Control> _Instances; 
		
		/*
		** Constructor of the class
		** 
		** @return void
		*/
		public LibrariesListing()
		{
			Name = "LibrariesListing";
			// _include = false;
		}

		public override void _EnterTree()
		{
			_Instances = new();
			
			base._EnterTree();
		}

		/*
		** Initialization of the component
		** 
		** @return void
		*/ 
		public override void Initialize()
		{
			CurrentFolderCount = _GlobalExplorer.Settings.FolderCount;
			
			_SetupListTitle(); 
			_SetupListTable(); 
		}
		
		/*
		** Forces the list to update
		** 
		** @return void
		*/
		public void ForceUpdate()
		{
			if( CurrentFolderCount != 0 ) 
			{
				_ClearList(); 
			}
			
			CurrentFolderCount = 0;
			_UpdateListTable();
		}
		
		/*
		** Set's up the list title
		** 
		** @return void
		*/
		private void _SetupListTitle()
		{
			_TitleMarginContainer = new();
			_TitleInnerContainer = new();
			_Title = new();
			
			_TitleMarginContainer.AddThemeConstantOverride("margin_left", 15);
			_TitleMarginContainer.AddThemeConstantOverride("margin_right", 15);
			_TitleMarginContainer.AddThemeConstantOverride("margin_top", 10);
			_TitleMarginContainer.AddThemeConstantOverride("margin_bottom", 4);

			_Title.ThemeTypeVariation = "HeaderLarge"; 
			_Title.Text = TitleText;

			_TitleInnerContainer.AddChild(_Title);
			_TitleMarginContainer.AddChild(_TitleInnerContainer);
			Container.AddChild(_TitleMarginContainer);
		}
		
		/*
		** Set's up the list table
		** 
		** @return void
		*/
		private void _SetupListTable()
		{
			if( _GlobalExplorer.Settings.FolderCount == 0 ) 
			{
				_SetupNoFoldersTable();
				return;
			}
			
			_SetupFolderListTable();
		}
		
		/*
		** Set's up the no folders list table
		** 
		** @return void
		*/
		private void _SetupNoFoldersTable()
		{
			_FolderListMarginContainer = new();
			_FolderListInnerContainer = new();
			
			_FolderListMarginContainer.AddThemeConstantOverride("margin_left", 15);
			_FolderListMarginContainer.AddThemeConstantOverride("margin_right", 15);
			_FolderListMarginContainer.AddThemeConstantOverride("margin_top", 3);
			_FolderListMarginContainer.AddThemeConstantOverride("margin_bottom", 0);

			Label Title = new()
			{
				Text = NotFoundText
			};

			_FolderListInnerContainer.AddChild(Title);
			_FolderListMarginContainer.AddChild(_FolderListInnerContainer);
			Container.AddChild(_FolderListMarginContainer);
		}
		
		/*
		** Set's up the folders list table
		** 
		** @return void
		*/
		private void _SetupFolderListTable()
		{
			_FolderListMarginContainer = new();
			_FolderListInnerContainer = new();
			
			_FolderListMarginContainer.AddThemeConstantOverride("margin_left", 15);
			_FolderListMarginContainer.AddThemeConstantOverride("margin_right", 15);
			_FolderListMarginContainer.AddThemeConstantOverride("margin_top", 0);
			_FolderListMarginContainer.AddThemeConstantOverride("margin_bottom", 0);

			for( int i = 0; i < _GlobalExplorer.Settings.FolderCount; i++) 
			{
				string title = _GlobalExplorer.Settings.Folders[i];
				List<string> Components = new()
				{
					"LibrariesListingEntry",
				};
				
				if (GlobalExplorer.GetInstance().Components.HasAll( Components.ToArray() )) 
				{
					LibrariesListingEntry SingleEntry = GlobalExplorer.GetInstance().Components.Single<LibrariesListingEntry>(true);
					
					SingleEntry.title = title;
					SingleEntry.Container = _FolderListInnerContainer;
					SingleEntry.Initialize(); 
				}
			}

			_FolderListMarginContainer.AddChild(_FolderListInnerContainer);
			Container.AddChild(_FolderListMarginContainer); 
		}
		
		/*
		** Clears the current list children
		** 
		** @return void
		*/
		private void _ClearList() 
		{
			var Children = Container.GetChildren();
			for( int i = 0; i < Children.Count; i++) 
			{
				var child = Children[i];

				// Container.RemoveChild(child);
				child.QueueFree();
			}
		}
		
		/*
		** Updates the list table, so it
		** shows the newest changes.
		** 
		** @return void
		*/
		private void _UpdateListTable()
		{
			if( _GlobalExplorer.Settings.FolderCount == CurrentFolderCount ) 
			{
				return; 
			}

			_ClearList();
			_SetupListTitle();
			
			if( _GlobalExplorer.Settings.FolderCount == 0 ) 
			{
				_SetupNoFoldersTable();
				return;
			}
			
			_SetupFolderListTable();
		}
			
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			// foreach( var _object in _Instances ) 
			// {
			// 	if( IsInstanceValid(_object) && _object is Node _Node ) 
			// 	{
			// 		_Node.QueueFree();
			// 	} 
			// }
			// _Instances = null; 
			
			if( IsInstanceValid(_TitleMarginContainer) ) 
			{
				_TitleMarginContainer.QueueFree();
				_TitleMarginContainer = null;
			}
			
			if( IsInstanceValid(_TitleInnerContainer) ) 
			{
				_TitleInnerContainer.QueueFree();
				_TitleInnerContainer = null;
			}
			
			if( IsInstanceValid(_Title) ) 
			{
				_Title.QueueFree();
				_Title = null;
			}

			if( IsInstanceValid(_FolderListMarginContainer) ) 
			{
				_FolderListMarginContainer.QueueFree();
				_FolderListMarginContainer = null;
			} 
			
			if( IsInstanceValid(_FolderListInnerContainer) ) 
			{
				_FolderListInnerContainer.QueueFree();
				_FolderListInnerContainer = null;
			}

			base._ExitTree();
		}
	}
}