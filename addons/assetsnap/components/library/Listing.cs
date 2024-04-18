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

namespace AssetSnap.Front.Components.Library
{
	using System.Collections.Generic;
    using System.Linq;
    using AssetSnap.Component;
	using Godot;

	[Tool]
	public partial class Listing : LibraryComponent
	{
		private string _Folder;
	
		private Godot.Collections.Array<HBoxContainer> Containers = new();
		private Godot.Collections.Array<ListEntry> Items = new();
		
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
		public Listing()
		{
			Name = "LibraryListing";
			
			UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
				{ typeof(ScrollContainerable).ToString() },
			};
			
			//_include = false; 
		}
		
		/*
		** Initializes the component 
		**
		** @return void
		*/
		public override void Initialize()
		{
			base.Initialize();
			
			Containers = new();
			Items = new();
			Initiated = true;
			
			SizeFlagsHorizontal = SizeFlags.ExpandFill;
			SizeFlagsVertical = SizeFlags.ExpandFill;
		
			if( Folder == null ) 
			{
				return;
			}
						
			Trait<Containerable>()
				.SetName("LibraryListingInner")
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetMargin(2, "top")
				.SetMargin(5, "left")
				.SetMargin(5, "right")
				.Instantiate();
				
			Trait<ScrollContainerable>()
				.SetName("LibraryListingMain")
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetMargin(5, "right")
				.SetMargin(5, "left")
				.Instantiate();
				
			Trait<Containerable>()
				.SetName("LibraryListingInner")
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetMargin(5, "left")
				.SetMargin(5, "right")
				.Instantiate();
			
			// Get the bottom dock
			IterateFiles(
				_Folder,
				Trait<Containerable>()
					.Select(1)
					.GetInnerContainer()
			);

			Trait<Containerable>()
				.Select(1)
				.AddToContainer(
					Trait<ScrollContainerable>()
						.Select(0)
						.GetScrollContainer()
				);

			Trait<ScrollContainerable>()
				.Select(0)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				);
				
			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					this
				);
		}
		
		/*
		** Updates the list 
		**
		** @return void
		*/
		public void Update()
		{
			// _Library.RemoveAllPanelState();
			foreach(HBoxContainer child in Trait<Containerable>().Select(0).GetInnerContainer().GetChildren()) 
			{
				if( IsInstanceValid( child ) ) 
				{
					_Library.RemoveAllPanelState();
					Trait<Containerable>().Select(0).GetInnerContainer().RemoveChild(child);	
					child.QueueFree();
				}
			}

			IterateFiles(Folder, Trait<Containerable>().Select(0).GetInnerContainer());
		}
	
		/*
		** Iterates through the files inside the current folder
		** and adds them as entries to the list
		**
		** @return void
		*/
		private void IterateFiles(string folderPath, Container BoxContainer)
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

				Library.ItemCount = fileNames.Length;
				foreach (string fileName in fileNames)
				{
					if(fileName.Contains(".import")) 
					{
						continue; 
					}
					
					string extension = System.IO.Path.GetExtension(fileName).ToLower();
					string file_name = System.IO.Path.GetFileName(fileName);

					
					if( IsInstanceValid(Library._LibraryTopbar) && IsInstanceValid(Library._LibraryTopbar._LibrarySearch)) 
					{
						bool IsSearching = Library._LibraryTopbar._LibrarySearch.IsSearching();
						bool SearchValid = Library._LibraryTopbar._LibrarySearch.SearchValid(file_name);
						
						if( IsSearching && false == SearchValid)
						{
							continue;
						}
					}
				

					// Check if the file has a supported extension 
					if ( IsValidExtension( extension ) )
					{
						ListEntry SingleEntry = GlobalExplorer.GetInstance().Components.Single<ListEntry>(true);
						SingleEntry.Name = "LibraryEntry-" + ((rows + 1) * iteration);
						
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
		private HBoxContainer _SetupListContainer(Container BoxContainer)
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
	}
}