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
		public ListTitle _LibraryListTitle;
		
		/// <summary>
		/// The search component of the library.
		/// </summary>
		public Search _LibrarySearch;
		
		/// <summary>
		/// The total number of items in the library.
		/// </summary>
		public TotalItems _LibraryItems;
		
		/// <summary>
		/// Constructor for Topbar class.
		/// </summary>
		public Topbar()
		{
			Name = "LibraryTopbar";
			SizeFlagsHorizontal = SizeFlags.ExpandFill;
			SizeFlagsVertical = SizeFlags.ShrinkBegin;
			
			UsingTraits = new()
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
			Initiated = true;
			
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
				_LibraryListTitle = GlobalExplorer.GetInstance().Components.Single<ListTitle>(true);
				_LibrarySearch = GlobalExplorer.GetInstance().Components.Single<Search>(true);
				_LibraryItems = GlobalExplorer.GetInstance().Components.Single<TotalItems>(true);

				if( _LibraryListTitle != null ) 
				{
					_LibraryListTitle.LibraryName = LibraryName;
					_LibraryListTitle.Initialize();
					ContainerOne.AddChild(_LibraryListTitle);
				}
				
				if( _LibrarySearch != null ) 
				{
					_LibrarySearch.LibraryName = LibraryName;
					_LibrarySearch.Initialize();
					ContainerTwo.AddChild(_LibrarySearch);
				}
				
				if( _LibraryItems != null ) 
				{
					_LibraryItems.LibraryName = LibraryName;
					_LibraryItems.Initialize();
					ContainerThree.AddChild(_LibraryItems);
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