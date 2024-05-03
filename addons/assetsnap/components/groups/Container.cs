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

namespace AssetSnap.Front.Components.Groups
{
	/// <summary>
	/// Partial class representing a container component within a library.
	/// </summary>
	[Tool]
	public partial class Container : LibraryComponent
	{
		/// <summary>
		/// Constructor for the Container class.
		/// </summary>
		public Container()
		{
			Name = "GroupContainer";
			
			UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
			};
			
			//_include = false;
			// _include = false;
		}
		
		/// <summary>
		/// Initializes the component.
		/// </summary>
		public override void Initialize()
		{
			SizeFlagsVertical = Control.SizeFlags.ExpandFill;
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
			
			base.Initialize();
			
			Initiated = true;

			_InitializeFields();
			_FinalizeFields();
		}
		
		/// <summary>
		/// Initializes the fields of the container.
		/// </summary>
		private void _InitializeFields()
		{
			Trait<Containerable>()
				.SetName("GroupContainer")
				.SetMargin(10, "bottom")
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetLayout( Containerable.ContainerLayout.TwoColumns )
				.Instantiate();
		}
		
		/// <summary>
        /// Finalizes the fields of the container.
        /// </summary>
		private void _FinalizeFields()
		{
			Godot.Container sidebar = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(0);

			sidebar.CustomMinimumSize = new Vector2(300, 0);
			sidebar.SizeFlagsHorizontal = 0;

			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					this
				);
		}
		
		/// <summary>
        /// Fetches the left inner container.
        /// </summary>
        /// <returns>The left inner container.</returns>
		public Godot.Container GetLeftInnerContainer() 
		{
			return Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(0);
		}
		
		/// <summary>
        /// Fetches the right inner container.
        /// </summary>
        /// <returns>The right inner container.</returns>
		public Godot.Container GetRightInnerContainer() 
		{
			return Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(1);
		}
	}
}

#endif