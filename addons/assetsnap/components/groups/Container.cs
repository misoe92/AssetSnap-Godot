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
	using AssetSnap.Component;
	using Godot;

	[Tool]
	public partial class GroupContainer : LibraryComponent
	{
		/*
		** Constructor of component
		** 
		** @return void
		*/
		public GroupContainer()
		{
			Name = "GroupContainer"; 
			//_include = false;
			// _include = false;
		}
		
		/*
		** Initialization of component
		** 
		** @return void
		*/
		public override void Initialize()
		{
			AddTrait(typeof(Containerable));
			Initiated = true;

			_InitializeFields();
			_FinalizeFields();
		}
		
		private void _InitializeFields()
		{
			Trait<Containerable>()
				.SetName("GroupContainer")
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetLayout( Containerable.ContainerLayout.TwoColumns )
				.Instantiate();

			
		}
		
		private void _FinalizeFields()
		{
			Container sidebar = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(0);

			sidebar.CustomMinimumSize = new Vector2(300, 0);
			sidebar.SizeFlagsHorizontal = 0;

			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					Container
				);
		}
		
		/*
		** Fetches the left inner container
		**
		** @return VBoxContainer
		*/
		public Container GetLeftInnerContainer() 
		{
			return Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(0);
		}
		
		/*
		** Fetches the right inner container
		**
		** @return VBoxContainer
		*/
		public Container GetRightInnerContainer() 
		{
			return Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(1);
		}
	}
}