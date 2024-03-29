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

	public partial class LibraryListTitle : LibraryComponent
	{
		private readonly string Title = "Library List";
		private	Label _Label;
		
		public LibraryListTitle()
		{
			Name = "LibraryListTitle";
			// _include = false;
		}
		
		/*
		** Initializes the component
		**
		** @return void
		*/ 
		public override void Initialize()
		{
			if( Container is HBoxContainer BoxContainer ) 
			{
				_Label = new()
				{
					ThemeTypeVariation = "HeaderMedium",
					Text = Title,
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
					SizeFlagsVertical = Control.SizeFlags.ShrinkCenter
				};
				
				BoxContainer.AddChild(_Label);
			}
		}
			
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			if( IsInstanceValid(_Label) ) 
			{
				_Label.QueueFree();
				_Label = null;
			}
		}
	}
}