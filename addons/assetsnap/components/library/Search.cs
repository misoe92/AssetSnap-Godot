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
	using AssetSnap.Front.Nodes;
	using Godot;

	[Tool]
	public partial class LibrarySearch : LibraryComponent
	{
		private readonly string Title = "Search Library";
		private string value = "";
		private string LastValue = "";
		private double ValueIntervalTimer = 0.0;
		
		private MarginContainer _MarginContainer;
		private	HBoxContainer _InnerContainer;
		private	Label _Label;
		private	AsSearchInput _SearchInput;

		private bool _Searching = false;
		private bool _Searched = false;

		private Callable? SearchCallable;
		
		/*
		** Component constructor
		**
		** @return void
		*/
		public LibrarySearch()
		{
			Name = "LibrarySearch";
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
			AddTrait(typeof(Containerable));
			AddTrait(typeof(Buttonable));
			Initiated = true;

			Trait<Containerable>()
				.SetName("SearchContainer")
				.SetMargin(4, "top")
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
				.AddToContainer(Container);
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
	
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			
			if( IsInstanceValid(_SearchInput) && _SearchInput != null && SearchCallable is Callable _callable ) 
			{
				if( _SearchInput.IsConnected(LineEdit.SignalName.TextChanged, _callable)) 
				{
					_SearchInput.Disconnect(LineEdit.SignalName.TextChanged, _callable);				
				}
			}
		 
			if( IsInstanceValid(_SearchInput) ) 
			{
				_SearchInput.QueueFree();
				_SearchInput = null;
			}
			
			if( IsInstanceValid(_Label) ) 
			{
				_Label.QueueFree();
				_Label = null;
			} 
		}
	}
}