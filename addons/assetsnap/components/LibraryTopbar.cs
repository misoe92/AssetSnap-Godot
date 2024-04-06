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

	[Tool]
	public partial class LibraryTopbar : LibraryComponent
	{
		public LibraryListTitle _LibraryListTitle;
		public LibrarySearch _LibrarySearch;
		public LibraryItems _LibraryItems;

		public int ItemCount 
		{
			get => _ItemCount;
			set 
			{
				_ItemCount = value;
				 
				if(
					null != _LibraryItems &&
					ItemCount != _LibraryItems.GetItemCount() &&
					null != _LibraryItems && IsInstanceValid(_LibraryItems)
				)
				{
					_LibraryItems.SetItemCount( ItemCount );
				}
			}
		}

		private int _ItemCount = 0;
		
		/*
		** Constructor
		**
		** @return void
		*/
		public LibraryTopbar()
		{
			Name = "LibraryTopbar";
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
			AddTrait(typeof(Containerable));
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
				_LibraryListTitle = GlobalExplorer.GetInstance().Components.Single<LibraryListTitle>(true);
				_LibrarySearch = GlobalExplorer.GetInstance().Components.Single<LibrarySearch>(true);
				_LibraryItems = GlobalExplorer.GetInstance().Components.Single<LibraryItems>(true);

				if( _LibraryListTitle != null ) 
				{
					_LibraryListTitle.Container = ContainerOne;
					_LibraryListTitle.Library = Library;
					_LibraryListTitle.Initialize();
				}
				
				if( _LibrarySearch != null ) 
				{
					_LibrarySearch.Container = ContainerTwo;
					_LibrarySearch.Library = Library;
					_LibrarySearch.Initialize();
				}
				
				if( _LibraryItems != null ) 
				{
					_LibraryItems.Container = ContainerThree;
					_LibraryItems.SetItemCount( ItemCount );
					_LibraryItems.Library = Library;
					_LibraryItems.Initialize();
				}
			}

			Trait<Containerable>()
				.Select(0)
				.AddToContainer(Container);	
		}
	
		public Container GetContainer()
		{
			return Trait<Containerable>()
				.Select(0)
				.GetNode() as Container;
		}
	}
}