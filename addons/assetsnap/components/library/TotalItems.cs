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

using AssetSnap.Component;
using Godot;

namespace AssetSnap.Front.Components.Library
{
	/// <summary>
	/// A component that displays the total number of items in the library.
	/// </summary>
	[Tool]
	public partial class TotalItems : LibraryComponent
	{
		private readonly string _Title = "Total items";

		private int _ItemCount = 0;

		/// <summary>
		/// Component constructor.
		/// </summary>
		public TotalItems()
		{
			Name = "LibraryItems";
			
			_UsingTraits = new()
			{
				{ typeof(Labelable).ToString() },
			};
			
			//_include = false;
		}

		/// <summary>
		/// Initializes the component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			_Initiated = true;
			Library.ItemCountUpdated += _OnItemCountUpdated; 
			_ItemCount = Library.ItemCount;
			
			Trait<Labelable>()
				.SetName("LibraryItemsCount")
				.SetText(_Title + ": " + _ItemCount)
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
				.AddToContainer(this);
		}
		
		/// <summary>
		/// Sets the total item count.
		/// </summary>
		/// <param name="count">The new total item count.</param>
		public void SetItemCount(int count ) 
		{
			_ItemCount = count;
			
			if( 
				HasTrait<Labelable>() &&
				Trait<Labelable>().Select(0).IsValid()
			) 
			{
				Trait<Labelable>().SetText(_Title + ": " + count);
			}
			else 
			{
				GD.PushError("Total items label not available", HasTrait<Labelable>());
			}
		}
		
		/// <summary>
		/// Overrides the _ExitTree method to detach from events before being removed from the scene tree.
		/// </summary>
		public override void _ExitTree()
		{
			Library.ItemCountUpdated -= _OnItemCountUpdated; 
			base._ExitTree();
		}
		
		/// <summary>
		/// Gets the total item count.
		/// </summary>
		/// <returns>The total item count.</returns>
		public int GetItemCount() 
		{
			return _ItemCount;
		}
		
		/// <summary>
		/// Handles the event when the item count is updated.
		/// </summary>
		/// <param name="count">The new item count.</param>
		private void _OnItemCountUpdated(int count)
		{
			SetItemCount(count);
		}
	}
}

#endif