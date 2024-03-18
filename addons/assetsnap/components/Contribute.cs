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

	public partial class Contribute : BaseComponent
	{
		private readonly string TitleText = "Contributions";
		private readonly string DescriptionText = "Below is a list over the people who have actively contributed to the development of the plugin.";
		private readonly string[] contributors = new string[]
		{
			"Mike Sørensen | https://github.com/misoe92",
		};
		
		private ScrollContainer _ContributorsScrollr;
		private MarginContainer _ContributorsContainer;
		private VBoxContainer _ContributorsInnerContainer;
		private MarginContainer _TitleContainer;
		private HBoxContainer _TitleInnerContainer;
		private Label _Title;
		private MarginContainer _DescriptionContainer;
		private Label _Description;
			
		/*
		** Class constructor
		** 
		** @return void
		*/
		public Contribute()
		{
			Name = "Contribute";
			/* Debugging Purpose */ 
			// _include = false;
			/* -- */  
		}

		/*
		** Initializing component
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

			_SetupContributors();
		}
		
		/*
		** Sets up the contribution title
		** 
		** @return void
		*/
		private void _SetupMainTitle()
		{
			_TitleContainer = new();
			_TitleInnerContainer = new();
			_Title = new();
			
			_TitleContainer.AddThemeConstantOverride("margin_left", 15);
			_TitleContainer.AddThemeConstantOverride("margin_right", 15);
			_TitleContainer.AddThemeConstantOverride("margin_top", 10);
			_TitleContainer.AddThemeConstantOverride("margin_bottom", 0);

			_Title.ThemeTypeVariation = "HeaderLarge";
			_Title.Text = TitleText;

			
			_TitleInnerContainer.AddChild(_Title);
			_TitleContainer.AddChild(_TitleInnerContainer);
			
			Container.AddChild(_TitleContainer);
		}
		
		/*
		** Sets up the contribution description
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
		** Sets up the contributor list
		** 
		** @return void
		*/
		private void _SetupContributors()
		{
			_ContributorsContainer = new();
			_ContributorsScrollr = new()
			{
				CustomMinimumSize = new Vector2(0, 150),
			};
			_ContributorsInnerContainer = new();
			
			_ContributorsContainer.AddThemeConstantOverride("margin_left", 15);
			_ContributorsContainer.AddThemeConstantOverride("margin_right", 15);
			_ContributorsContainer.AddThemeConstantOverride("margin_top", 10);
			_ContributorsContainer.AddThemeConstantOverride("margin_bottom", 0);

			for( int i = 0; i < contributors.Length; i++) 
			{
				string contributor = contributors[i];

				Label _Label = new()
				{
					Text = contributor
				};
				_ContributorsInnerContainer.AddChild(_Label);
			}

			_ContributorsScrollr.AddChild(_ContributorsInnerContainer);
			_ContributorsContainer.AddChild(_ContributorsScrollr);
			Container.AddChild(_ContributorsContainer);
		}
			
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			if( IsInstanceValid(_ContributorsInnerContainer) ) 
			{
				_ContributorsInnerContainer.QueueFree();
				_ContributorsInnerContainer = null; 
			}
			if( IsInstanceValid(_ContributorsScrollr) ) 
			{
				_ContributorsScrollr.QueueFree();
				_ContributorsScrollr = null;
			}
			if( IsInstanceValid(_ContributorsContainer) ) 
			{
				_ContributorsContainer.QueueFree();
				_ContributorsContainer = null;
			}
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