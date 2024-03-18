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

	public partial class Introduction : BaseComponent
	{
		private readonly string TitleText = "AssetSnap";
		private readonly string DescriptionText = "Add folders to start, when an folder has been added a tab with the folder name will appear, Then go to the folder tab and browse your assets and place them. \n\nIf you wish to remove a library all you will have to do is click the red button on the right. In the same column as the library you wish to remove.";
		private MarginContainer _TitleContainer;
		private HBoxContainer _TitleInnerContainer;
		private Label _Title;
		private Label _Version;
		private MarginContainer _DescriptionContainer;
		private Label _Description;
		
		/*
		** Constructor of the class
		** 
		** @return void
		*/ 
		public Introduction()
		{
			Name = "Introduction";
			// _include = false; 
		}
		
		/*
		** Initialization of component
		** 
		** @return void
		*/
		public override void Initialize()
		{
			if( Container == null ) 
			{
				return;
			}
			 
			_SetupMainTitle();
			_SetupMainDescription();
		}
		
		/*
		** Sets up the introduction title
		** 
		** @return void
		*/
		private void _SetupMainTitle()
		{
			_TitleContainer = new();
			_TitleInnerContainer = new();
			_Title = new();
			_Version = new();
			
			_TitleContainer.AddThemeConstantOverride("margin_left", 15);
			_TitleContainer.AddThemeConstantOverride("margin_right", 15);
			_TitleContainer.AddThemeConstantOverride("margin_top", 10);
			_TitleContainer.AddThemeConstantOverride("margin_bottom", 0);

			_Title.ThemeTypeVariation = "HeaderLarge"; 
			_Title.Text = TitleText;

			_Version.Text = _GlobalExplorer._Plugin.GetVersionString();
			
			_TitleInnerContainer.AddChild(_Title);
			_TitleInnerContainer.AddChild(_Version);
			_TitleContainer.AddChild(_TitleInnerContainer);
			
			Container.AddChild(_TitleContainer);
		}
		
		/*
		** Sets up the introduction description
		** 
		** @return void
		*/
		private void _SetupMainDescription()
		{
			_DescriptionContainer = new();
			_Description = new();
			
			_DescriptionContainer.AddThemeConstantOverride("margin_left", 15);
			_DescriptionContainer.AddThemeConstantOverride("margin_right", 15);
			_DescriptionContainer.AddThemeConstantOverride("margin_top", 5);
			_DescriptionContainer.AddThemeConstantOverride("margin_bottom", 5);

			_Description.Text = DescriptionText;
			_Description.AutowrapMode = TextServer.AutowrapMode.Word;
			
			_DescriptionContainer.AddChild(_Description);
			Container.AddChild(_DescriptionContainer);
		}
			
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			
			if( IsInstanceValid(_Title) ) 
			{
				_Title.QueueFree();
				_Title = null;
			}
			
			if( IsInstanceValid(_Description) ) 
			{
				_Description.QueueFree();
				_Description = null;
			}
			
			if( IsInstanceValid(_TitleInnerContainer) ) 
			{
				_TitleInnerContainer.QueueFree();
				_TitleInnerContainer = null;
			}
			
			if( IsInstanceValid(_TitleContainer) ) 
			{
				_TitleContainer.QueueFree();
				_TitleContainer = null;
			}

			if( IsInstanceValid(_DescriptionContainer) ) 
			{
				_DescriptionContainer.QueueFree();
				_DescriptionContainer = null;
			} 
		}
	}
}