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
	public partial class Contribute : TraitableComponent
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
			
			base.Initialize();
			
			AddTrait(typeof(Titleable));
			AddTrait(typeof(Descriptionable));
			Initiated = true;
			
			Trait<Titleable>()
				.SetName( "ContributeTitle" )
				.SetType( Titleable.TitleType.HeaderLarge)
				.SetTitle( TitleText )
				.Initialize()
				.AddToContainer( Container );
			 
			Trait<Descriptionable>()
				.SetName( "ContributeDescription" )
				.SetTitle( DescriptionText )
				.Initialize()
				.AddToContainer( Container );
				
			_SetupContributors();
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
				
			base._ExitTree();
		}
	}
}