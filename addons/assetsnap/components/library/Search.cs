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
using AssetSnap.Front.Nodes;
using Godot;

namespace AssetSnap.Front.Components.Library
{
	/// <summary>
	/// Component representing a search functionality within a library.
	/// </summary>
	[Tool]
	public partial class Search : LibraryComponent
	{
		public AsSearchInput SearchInput { get; set; }
		
		private readonly string _Title = "Search Library";
		private string _Value = "";
		private string _LastValue = "";
		private double _ValueIntervalTimer = 0.0;
		private	Label _Label;
		private bool _Searching = false;
		private bool _Searched = false;
		private Callable? _SearchCallable;
		
		/// <summary>
		/// Constructor for the Search component.
		/// </summary>
		public Search()
		{
			Name = "LibrarySearch";
			
			_UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
				{ typeof(Buttonable).ToString() },
			};
			
			//_include = false;
		}
		
		/// <summary>
		/// Initializes the Search component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			_Initiated = true;

			Trait<Containerable>()
				.SetName("SearchContainer")
				.SetMargin(4, "top")
				.SetMargin(0, "bottom")
				.SetOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetInnerOrientation(Containerable.ContainerOrientation.Horizontal)
				.Instantiate();
			
			SearchInput = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				PlaceholderText = _Title
			};
		
			_SearchCallable = new Callable(this, "_OnSearchQuery");

			if( _SearchCallable is Callable _callable ) 
			{
				SearchInput.Connect(LineEdit.SignalName.TextChanged, _callable);
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
				.AddChild(SearchInput);

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

		/// <summary>
		/// Process method for handling continuous updates in the Search component.
		/// </summary>
		/// <param name="delta">Time elapsed since the last frame.</param>
		public override void _Process(double delta) 
		{
			if( null == SearchInput ) 
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
				_Value != "" &&
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
				_Value == "" &&
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

			if(_Value != _LastValue) 
			{
				_LastValue = _Value;
				_ValueIntervalTimer = GlobalExplorer.GetInstance().DeltaTime;
				_Searched = false;
			}
			else if(_Searched == false)
			{
				if( GlobalExplorer.GetInstance().DeltaTime - _ValueIntervalTimer > 1) 
				{
					_Searched = true;
					_ValueIntervalTimer = 0.0;
					Library._LibraryListing.Update();
				} 
			}
		}
		
		/// <summary>
		/// Checks if a search is ongoing.
		/// </summary>
		/// <returns>True if a search is ongoing, otherwise false.</returns>
		public bool IsSearching()
		{
			return _Searching; 
		}
		
		/// <summary>
		/// Checks if the provided search text is valid.
		/// </summary>
		/// <param name="text">The search text to validate.</param>
		/// <returns>True if the search text is valid, otherwise false.</returns>
		public bool SearchValid( string text )
		{
			return text.ToLower().Contains(_Value.ToLower());
		}
		
		/// <summary>
		/// Event handler for updating the search query and setting the search state when input is received.
		/// </summary>
		/// <param name="text">The new search query text.</param>
		private void _OnSearchQuery(string text)
		{
			if (text != "" || text == "" && _Value != "")
			{
				_Searching = true;
			}
			else
			{
				_Searching = false;
			}

			_Value = text;
		}

		/// <summary>
		/// Clears the current search query.
		/// </summary>
		private void _ClearCurrentQuery()
		{
			SearchInput.Clear();
			_Value = "";
		}
	}
}

#endif