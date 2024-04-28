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
	using AssetSnap.Front.Nodes;
	using Godot;

	[Tool]
	public partial class Search : LibraryComponent
	{
		private readonly string Title = "Search Library";
		private string value = "";
		private string LastValue = "";
		private double ValueIntervalTimer = 0.0;
		
		private	Label _Label;
		public AsSearchInput _SearchInput { get; set; }

		private bool _Searching = false;
		private bool _Searched = false;

		private Callable? SearchCallable;
		
		/*
		** Component constructor
		**
		** @return void
		*/
		public Search()
		{
			Name = "LibrarySearch";
			
			UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
				{ typeof(Buttonable).ToString() },
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

			Trait<Containerable>()
				.SetName("SearchContainer")
				.SetMargin(4, "top")
				.SetMargin(0, "bottom")
				.SetOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetInnerOrientation(Containerable.ContainerOrientation.Horizontal)
				.Instantiate();
			
			_SearchInput = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				PlaceholderText = Title
			};
		
			SearchCallable = new Callable(this, "_OnSearchQuery");

			if( SearchCallable is Callable _callable ) 
			{
				_SearchInput.Connect(LineEdit.SignalName.TextChanged, _callable);
			}

			Trait<Buttonable>()
				.SetName("SearchClearQuery")
				.SetType(Buttonable.ButtonType.SmallFlatButton)
				.SetText("")
				.SetTooltipText("Clears your current search query")
				.SetVisible(false)
				.SetIcon(GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/x.svg"))
				.SetAction(() => { _ClearCurrentQuery(); } )
				.Instantiate();
			
			Trait<Containerable>()
				.Select(0)
				.GetInnerContainer()
				.AddChild(_SearchInput);

			Trait<Buttonable>()
				.Select(0)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				);

			Trait<Containerable>()
				.Select(0)
				.AddToContainer(this);
		}

		/*
		** Checks if a search has been performed
		** if it has it then queries an update on
		** the listing
		**
		** @return void
		*/
		public override void _Process(double delta) 
		{
			if( null == _SearchInput ) 
			{
				return;
			}
			
			if(
				_Searching == false ||
				null == Trait<Buttonable>() ||
				null == Trait<Buttonable>().Select(0)
			) 
			{
				return;
			}
			
			if(
				value != "" &&
				null != Trait<Buttonable>() &&
				false == Trait<Buttonable>()
					.Select(0)
					.IsVisible()
			) 
			{
				Trait<Buttonable>()
					.Select(0)
					.SetVisible(true);
			}
			else if(
				value == "" &&
				null != Trait<Buttonable>() &&
				true == Trait<Buttonable>()
					.Select(0)
					.IsVisible()
			)
			{
				Trait<Buttonable>()
					.Select(0)
					.SetVisible(false);
			}

			if(value != LastValue) 
			{
				LastValue = value;
				ValueIntervalTimer = GlobalExplorer.GetInstance().DeltaTime;
				_Searched = false;
			}
			else if(_Searched == false)
			{
				if( GlobalExplorer.GetInstance().DeltaTime - ValueIntervalTimer > 1) 
				{
					_Searched = true;
					ValueIntervalTimer = 0.0;
					Library._LibraryListing.Update();
				} 
			}
		}
		
		/*
		** Updates the search query and
		** sets the search state when input
		** is received
		**
		** @return void
		*/
		private void _OnSearchQuery(string text)
		{
			if (text != "" || text == "" && value != "")
			{
				_Searching = true;
			}
			else
			{
				_Searching = false;
			}

			value = text;
		}
		
		/*
		** Checks if search is ongoing
		**
		** @return bool
		*/
		public bool IsSearching()
		{
			return _Searching; 
		}
		
		/*
		** Checks if search text is valid
		**
		** @return bool
		*/
		public bool SearchValid( string text )
		{
			return text.ToLower().Contains(value.ToLower());
		}

		private void _ClearCurrentQuery()
		{
			_SearchInput.Clear();
			value = "";
		}
	}
}