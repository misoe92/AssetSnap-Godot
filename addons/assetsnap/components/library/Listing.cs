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

#if TOOLS

using System.Collections.Generic;
using System.Linq;
using AssetSnap.Component;
using Godot;

namespace AssetSnap.Front.Components.Library
{
	/// <summary>
	/// Partial class for managing a listing of assets in the library.
	/// </summary>
	[Tool]
	public partial class Listing : LibraryComponent
	{
		/// <summary>
		/// Gets or sets the folder path for the listing.
		/// </summary>
		public string Folder
		{
			get => _Folder;
			set
			{
				_Folder = value;
			}
		}
		
		private string _Folder;
		private Godot.Collections.Array<HBoxContainer> _Containers = new();

		/// <summary>
		/// Constructor for the Listing class.
		/// </summary>
		public Listing()
		{
			Name = "LibraryListing";

			_UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
				{ typeof(ScrollContainerable).ToString() },
			};

			//_include = false; 
		}

		/// <summary>
		/// Initializes the component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			_Containers = new();
			_Initiated = true;

			SizeFlagsHorizontal = SizeFlags.ExpandFill;
			SizeFlagsVertical = SizeFlags.ExpandFill;

			if (Folder == null)
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
			_IterateFiles(
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

		/// <summary>
		/// Updates the list.
		/// </summary>
		public void Update()
		{
			// _Library.RemoveAllPanelState();
			foreach (HBoxContainer child in Trait<Containerable>().Select(1).GetInnerContainer().GetChildren())
			{
				if (IsInstanceValid(child))
				{
					// _Library.RemoveAllPanelState();
					Trait<Containerable>().Select(1).GetInnerContainer().RemoveChild(child);
					child.QueueFree();
				}
			}

			 _IterateFiles(Folder, Trait<Containerable>().Select(1).GetInnerContainer());
		}

		/// <summary>
		/// Iterates through the files inside the current folder and adds them as entries to the list.
		/// </summary>
		/// <param name="folderPath">The path of the folder to iterate through.</param>
		/// <param name="BoxContainer">The container to add the entries to.</param>
		private void _IterateFiles(string folderPath, Container BoxContainer)
		{
			List<string> Components = new()
			{
				"Library.ListEntry",
			};

			if (GlobalExplorer.GetInstance().Components.HasAll(Components.ToArray()))
			{
				int iteration = 0;
				int total_iteration = 0;
				int rows = 0;
				int max_iteration = 4;

				HBoxContainer CurrentBoxContainer = _SetupListContainer(BoxContainer);

				// Build array of compatible models
				string[] fileNames = System.IO.Directory.GetFiles(
					folderPath.Split("res://").Join("")).Where(
						t => t.Contains(".glb") || t.Contains(".gltf") || t.Contains(".fbx") || t.Contains(".obj")
					).ToArray();

				// Remove .import files from the array
				fileNames = fileNames.Except(fileNames.Where(t => t.Contains(".import"))).ToArray();

				Library.ItemCount = fileNames.Length;
				foreach (string fileName in fileNames)
				{
					if (fileName.Contains(".import"))
					{
						continue;
					}

					string extension = System.IO.Path.GetExtension(fileName).ToLower();
					string file_name = System.IO.Path.GetFileName(fileName);

					if (IsInstanceValid(Library._LibraryTopbar) && IsInstanceValid(Library._LibraryTopbar.LibrarySearch))
					{
						bool IsSearching = Library._LibraryTopbar.LibrarySearch.IsSearching();
						bool SearchValid = Library._LibraryTopbar.LibrarySearch.SearchValid(file_name);

						if (IsSearching && false == SearchValid)
						{
							continue;
						}
					}

					// Check if the file has a supported extension 
					if (_IsValidExtension(extension))
					{
						ListEntry SingleEntry = GlobalExplorer.GetInstance().Components.Single<ListEntry>(true);
						SingleEntry.Name = "LibraryEntry-" + file_name + "-" + total_iteration;

						SingleEntry.Folder = folderPath;
						SingleEntry.Filename = file_name;
						SingleEntry.LibraryName = LibraryName;
						SingleEntry._Initialize();
						CurrentBoxContainer.AddChild(SingleEntry);

						if (iteration == max_iteration)
						{
							iteration = 0;
							rows += 1;
							total_iteration += 1;

							CurrentBoxContainer = _SetupListContainer(BoxContainer);
						}
						else
						{
							total_iteration += 1;
							iteration += 1;
						}
					}
				}
			}
		}

		/// <summary>
		/// Sets up the list container.
		/// </summary>
		/// <param name="BoxContainer">The container to set up the list within.</param>
		/// <returns>The initialized HBoxContainer.</returns>
		private HBoxContainer _SetupListContainer(Container BoxContainer)
		{
			HBoxContainer _Con = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,  // Uncheck VSize Flags for fixed height
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,  // Uncheck HSize Flags for fixed width
			};

			BoxContainer.AddChild(_Con);
			_Containers.Add(_Con);

			return _Con;
		}
		
		/// <summary>
		/// Checks if an extension is valid to be used as a model.
		/// </summary>
		/// <param name="Extension">The file extension to check.</param>
		/// <returns>True if the extension is valid, otherwise false.</returns>
		private bool _IsValidExtension(string Extension)
		{
			return Extension == ".obj" || Extension == ".fbx" || Extension == ".glb" || Extension == ".gltf";
		}
	}
}

#endif