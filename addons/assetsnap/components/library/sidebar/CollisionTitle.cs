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

namespace AssetSnap.Front.Components.Library.Sidebar
{
	/// <summary>
    /// Represents the title component for collision shapes in the library sidebar.
    /// </summary>
	public partial class CollisionTitle : LibraryComponent
	{
		private readonly string _MainTitle = "Collision shapes";
		private bool _Exited = false;
	
		private MarginContainer _MarginContainer;
		private VBoxContainer _InnerContainer;
		private Label _Label;

		/// <summary>
        /// Constructor for the CollisionTitle component.
        /// </summary>
		public CollisionTitle()
		{
			Name = "LSCollisionTitle";
			// _include = false;
		}
		
		/// <summary>
        /// Initializes the CollisionTitle component.
        /// </summary>
		public override void Initialize()
		{
			base.Initialize();
		
			_MarginContainer = new();
			_InnerContainer = new();
			
			_Label = new()
			{
				ThemeTypeVariation = "HeaderMedium",
				Text = _MainTitle,
			};
			
			_MarginContainer.AddThemeConstantOverride("margin_left", 10); 
			_MarginContainer.AddThemeConstantOverride("margin_right", 10);
			_MarginContainer.AddThemeConstantOverride("margin_top", 2);
			_MarginContainer.AddThemeConstantOverride("margin_bottom", 2);
			
			_InnerContainer.AddChild(_Label);
			_MarginContainer.AddChild(_InnerContainer);
			
			AddChild(_MarginContainer);
		}
	}
}

#endif