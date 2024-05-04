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
	/// Represents a title component for a library list.
	/// </summary>
	[Tool]
	public partial class ListTitle : LibraryComponent
	{
		private readonly string _Title = "Library List";
		private	Label _Label;
		
		/// <summary>
        /// Constructs a new instance of ListTitle.
        /// </summary>
		public ListTitle()
		{
			Name = "LibraryListTitle";
			
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
			
			Trait<Labelable>()
				.SetName("LibraryListTitle")
				.SetText(_Title)
				.SetType(Labelable.TitleType.HeaderMedium)
				.SetMargin(2, "top")
				.SetMargin(0, "bottom")	
				.SetMargin(10, "right")
				.SetMargin(12, "left")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkCenter)
				.Instantiate()
				.Select(0)
				.AddToContainer(this);
		}
	}
}

#endif