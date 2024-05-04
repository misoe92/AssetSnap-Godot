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
using AssetSnap.Component;
using Godot;

namespace AssetSnap.Front.Components.Library
{
	/// <summary>
	/// Represents the top bar component of the library.
	/// </summary>
	[Tool]
	public partial class Topbar : LibraryComponent
	{
		/// <summary>
		/// The title of the library list.
		/// </summary>
		public ListTitle LibraryListTitle;
		
		/// <summary>
		/// The search component of the library.
		/// </summary>
		public Search LibrarySearch;
		
		/// <summary>
		/// The total number of items in the library.
		/// </summary>
		public TotalItems LibraryItems;
		
		/// <summary>
		/// Constructor for Topbar class.
		/// </summary>
		public Topbar()
		{
			Name = "LibraryTopbar";
			SizeFlagsHorizontal = SizeFlags.ExpandFill;
			SizeFlagsVertical = SizeFlags.ShrinkBegin;
			
			_UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
			};
			
			//_include = false; 
		}

		/// <summary>
		/// Initializes the top bar component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			_Initiated = true;
			
			// Container 3 way
			// TitleComponent (spot 1)
			// Search ( Spot 2)
			Trait<Containerable>()
				.SetName("TopbarContainer")
				.SetLayout(Containerable.ContainerLayout.ThreeColumns)
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.Instantiate();

			Container ContainerOne = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(0);
				
			Container ContainerTwo = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(1);
							
			Container ContainerThree = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(2);
				
			List<string> Components = new()
			{
				"LibraryListTitle",
				"LibrarySearch",
			};

			if (GlobalExplorer.GetInstance().Components.HasAll( Components.ToArray() )) 
			{
				LibraryListTitle = GlobalExplorer.GetInstance().Components.Single<ListTitle>(true);
				LibrarySearch = GlobalExplorer.GetInstance().Components.Single<Search>(true);
				LibraryItems = GlobalExplorer.GetInstance().Components.Single<TotalItems>(true);

				if( LibraryListTitle != null ) 
				{
					LibraryListTitle.LibraryName = LibraryName;
					LibraryListTitle.Initialize();
					ContainerOne.AddChild(LibraryListTitle);
				}
				
				if( LibrarySearch != null ) 
				{
					LibrarySearch.LibraryName = LibraryName;
					LibrarySearch.Initialize();
					ContainerTwo.AddChild(LibrarySearch);
				}
				
				if( LibraryItems != null ) 
				{
					LibraryItems.LibraryName = LibraryName;
					LibraryItems.Initialize();
					ContainerThree.AddChild(LibraryItems);
				}
			}

			Trait<Containerable>()
				.Select(0)
				.AddToContainer(this);
		}
		
		/// <summary>
        /// Retrieves the container associated with the top bar.
        /// </summary>
        /// <returns>The container node.</returns>
		public Container GetContainer()
		{
			return Trait<Containerable>()
				.Select(0)
				.GetNode() as Container;
		}
	}
}

#endif