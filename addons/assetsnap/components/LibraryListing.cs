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
	using System.Collections.Generic;
	using AssetSnap.Component;
	using Godot;

	public partial class LibraryListing : LibraryComponent
	{
		private string _Folder;
	
		private ScrollContainer _ScrollContainer;
		private	MarginContainer _MarginContainer;
		private	VBoxContainer _InnerContainer;
		private Godot.Collections.Array<HBoxContainer> Containers = new();
		private Godot.Collections.Array<LibraryListEntry> Items = new();
		
		public string Folder 
		{
			get => _Folder;
			set 
			{
				_Folder = value;
			}
		}
		
		/*
		** Constructor
		**
		** @return void
		*/
		public LibraryListing()
		{
			Name = "LibraryListing";
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
				if( BoxContainer == null || Folder == null ) 
				{
					return;
				}
				
				_ScrollContainer = new()
				{
					SizeFlagsVertical = Control.SizeFlags.ExpandFill,
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
					CustomMinimumSize = new Vector2(0, 170),
				};

				_MarginContainer = new()
				{
					SizeFlagsVertical = Control.SizeFlags.ExpandFill,
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				};
				_InnerContainer = new()
				{
					SizeFlagsVertical = Control.SizeFlags.ExpandFill,
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				};
				
				_MarginContainer.AddThemeConstantOverride("margin_left", 5);
				_MarginContainer.AddThemeConstantOverride("margin_right", 5);
				_MarginContainer.AddThemeConstantOverride("margin_top", 5);
				_MarginContainer.AddThemeConstantOverride("margin_bottom", 5);
				
				// Get the bottom dock
				IterateFiles(_Folder, _InnerContainer); 
				
				_MarginContainer.AddChild(_InnerContainer);
				_ScrollContainer.AddChild(_MarginContainer);
				
				Container.AddChild(_ScrollContainer);
			}
		}
		
		/*
		** Updates the list 
		**
		** @return void
		*/
		public void Update()
		{
			// _Library.RemoveAllPanelState();
			foreach(HBoxContainer child in _InnerContainer.GetChildren()) 
			{
				if( IsInstanceValid( child ) ) 
				{
					_Library.RemoveAllPanelState();
					_InnerContainer.RemoveChild(child);	
					child.QueueFree();	
				}
			}

			IterateFiles(Folder, _InnerContainer);
		}
	
		/*
		** Iterates through the files inside the current folder
		** and adds them as entries to the list
		**
		** @return void
		*/
		private void IterateFiles(string folderPath, VBoxContainer BoxContainer)
		{		
			List<string> Components = new()
			{
				"LibraryListEntry",
			};
			
			if (GlobalExplorer.GetInstance().Components.HasAll( Components.ToArray() )) 
			{
				int iteration = 0;
				int rows = 0;
				int max_iteration = 4;

				HBoxContainer CurrentBoxContainer = _SetupListContainer(BoxContainer);
				string[] fileNames = System.IO.Directory.GetFiles(folderPath.Split("res://").Join(""));
				foreach (string fileName in fileNames)
				{
					if(fileName.Contains(".import")) 
					{
						continue; 
					}
					
					string extension = System.IO.Path.GetExtension(fileName).ToLower();
					string file_name = System.IO.Path.GetFileName(fileName);

					
					bool IsSearching = Library._LibrarySearch.IsSearching();
					bool SearchValid = Library._LibrarySearch.SearchValid(file_name);
					
					if( IsSearching && false == SearchValid)
					{
						continue;
					}

					// Check if the file has a supported extension 
					if ( IsValidExtension( extension ) )
					{
						LibraryListEntry SingleEntry = GlobalExplorer.GetInstance().Components.Single<LibraryListEntry>(true);
						SingleEntry.Name = "Entry-" + ((rows + 1) * iteration);
						
						SingleEntry.Container = CurrentBoxContainer;
						SingleEntry.Folder = folderPath;
						SingleEntry.Filename = file_name;
						SingleEntry.Library = Library;
						SingleEntry.Initialize();
						
						Items.Add(SingleEntry); 
						  
						if( iteration == max_iteration ) 
						{
							iteration = 0;
							rows += 1; 
							
							CurrentBoxContainer = _SetupListContainer(BoxContainer);
						}
						else 
						{
							iteration += 1;
						}
					}
				}
			}
		}
		
		/*
		** Check's if a extension is valid
		** to be used as a model
		**
		** @TODO Move check to a better section/class
		** @return HBoxContainer
		*/
		private bool IsValidExtension( string Extension ) 
		{
			return Extension == ".obj" || Extension == ".fbx" || Extension == ".glb" || Extension == ".gltf";
		}
			
		/*
		** Set's up the list container
		**
		** @return HBoxContainer
		*/
		private HBoxContainer _SetupListContainer(VBoxContainer BoxContainer)
		{
			HBoxContainer _Con = new()
			{
				// SizeFlagsVertical = Control.SizeFlags.ExpandFill,  // Uncheck VSize Flags for fixed height
				// SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,  // Uncheck HSize Flags for fixed width
			};
			
			BoxContainer.AddChild(_Con);
			Containers.Add( _Con );

			return _Con;
		}
			
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			Items = null;
			Containers = null;
			// if( Items != null)  
			// {
			// 	foreach( LibraryListEntry _item in Items ) 
			// 	{
			// 		if( IsInstanceValid(_item) ) 
			// 		{
			// 			// if( IsInstanceValid( item.GetParent() ) ) 
			// 			// {
			// 			// 	item.GetParent().RemoveChild(item);
			// 			// }
						
			// 			_item.QueueFree();
			// 		}
			// 	}

			// 	Items = null;
			// }
			
			// if( Containers != null ) 
			// {
			// 	foreach( HBoxContainer _container in Containers ) 
			// 	{
			// 		if( IsInstanceValid(_container) ) 
			// 		{
			// 			// if( IsInstanceValid( item.GetParent() ) ) 
			// 			// {
			// 			// 	item.GetParent().RemoveChild(item);
			// 			// }
						
			// 			_container.QueueFree();
			// 		}
			// 	}
	
			// 	Containers = null;
			// }
			
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
			
			if( IsInstanceValid(_ScrollContainer) ) 
			{
				_ScrollContainer.QueueFree();
				_ScrollContainer = null;
			}
		}
	}
}