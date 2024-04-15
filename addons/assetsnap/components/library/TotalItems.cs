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
	using AssetSnap.Component;
	using Godot;
	using Godot.Collections;

	[Tool]
	public partial class TotalItems : LibraryComponent
	{
		private readonly string Title = "Total items";

		private int ItemCount = 0;

		/*
		** Component constructor
		**
		** @return void
		*/
		public TotalItems()
		{
			Name = "LibraryItems";
			
			UsingTraits = new()
			{
				{ typeof(Labelable).ToString() },
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
			Initiated = true;
			
			Trait<Labelable>()
				.SetName("LibraryItemsCount")
				.SetText(Title + ": " + ItemCount)
				.SetType(Labelable.TitleType.HeaderSmall)
				.SetMargin(3, "top")
				.SetMargin(0, "bottom")	
				.SetMargin(10, "right")
				.SetMargin(10, "left")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkCenter)
				.SetAlignment( Godot.HorizontalAlignment.Right )
				.Instantiate()
				.Select(0)
				.AddToContainer(Container);
		}
		
		public int GetItemCount() 
		{
			return ItemCount;
		}
		
		public void SetItemCount(int count ) 
		{
			ItemCount = count;
			
			if( 
				HasTrait<Labelable>() &&
				Trait<Labelable>().Select(0).IsValid()
			) 
			{
				Trait<Labelable>().SetText(Title + ": " + count);
			}
		}
	}
}