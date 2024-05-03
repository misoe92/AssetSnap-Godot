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

using Godot;

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// Represents a selectable list with an associated button for toggling visibility.
	/// </summary>
	[Tool]
	public partial class AsSelectList : VBoxContainer
	{
		private Node3D _Handle { get; set; }
	
		/// <summary>
		/// The index of the active item in the list.
		/// </summary>
		[Export]
		public int _ActiveIndex { get; set; }
		
		/// <summary>
		/// The label associated with the list.
		/// </summary>
		[Export]
		public string _Label { get; set; }
		
		/// <summary>
		/// The currently active item in the list.
		/// </summary>
		[Export]
		public Control _ActiveItem { get; set; }
		
		/// <summary>
		/// The button associated with the list.
		/// </summary>
		[Export]
		public Button _Button { get; set; }
		
		/// <summary>
		/// The items contained within the list.
		/// </summary>
		[Export]
		public Godot.Collections.Array<Control> _Items { get; set; }
		
		/// <summary>
		/// Indicates whether the list is currently visible.
		/// </summary>
		[Export]
		public bool ListVisible { get; set; }
		
		/// <summary>
        /// Signals that the state of the list has changed.
        /// </summary>
		[Signal]
		public delegate void StateChangedEventHandler(string which);
		
		/// <summary>
        /// Configures the select list when entering the scene tree.
        /// </summary>
		public override void _EnterTree()
		{
			Name = "SelectList";
			ListVisible = false;
			_Items = new();
			
			if( _Button == null && HasNode("ButtonContainer/Button") ) 
			{
				_Button = GetNode<Button>("ButtonContainer/Button");
			}
			
			for( int i = 2; i < GetChildCount(); i++) 
			{
				Control item = GetChild<Control>(i);
				_Items.Add(item);
			}

			if( false == _Button.IsConnected(Button.SignalName.Pressed, new Callable(this, "_OnButtonPressed")) ) 
			{
				_Button.Connect(Button.SignalName.Pressed, new Callable(this, "_OnButtonPressed"));
			}
			
			GlobalExplorer.GetInstance().SelectLists.Add( this );
			
			base._EnterTree();
		}
		
		/// <summary>
        /// Handles processing logic for the select list.
        /// </summary>
        /// <param name="delta">The time elapsed since the last frame.</param>
		public override void _Process(double delta)
		{
			if( _Label == null || _Label == "" || _Items == null || _Items.Count == 0) 
			{
				return;
			}
			
			if( _Button == null && HasNode("ButtonContainer/Button") ) 
			{
				_Button = GetNode<Button>("ButtonContainer/Button");
			}
			
			if( _ActiveItem == null ) 
			{
				if( _Button.Text != _Label ) 
				{
					_Button.Text = _Label;
					ResetListVisibility();
				}
				
				return;
			}
			
			if( _Button.Text != _ActiveItem.Name ) 
			{
				_Button.Text = _ActiveItem.Name;
				ResetListVisibility();
			}

			return;
		}

		/// <summary>
        /// Creates the main button for the list.
        /// </summary>
		public void CreateButton()
		{
			Button _button = new()
			{
				Text = _Label,
				Flat = true,
				CustomMinimumSize = new Vector2(80, 0),
				TooltipText = "View actions available",
			};
			
			GetNode("ButtonContainer").AddChild(_button);

			_Button = _button;
		}
		
		/// <summary>
        /// Sets the active item in the list.
        /// </summary>
        /// <param name="which">The item to set as active.</param>
		public void SetActive( Control which ) 
		{
			if( null == which || "none" == which.Name.ToString().ToLower() ) 
			{
				_ActiveItem = null;
				_ActiveIndex = 0;
				EmitSignal(SignalName.StateChanged, "None");
				return;
			}

			_ActiveIndex = which.GetIndex() - 2;
			_ActiveItem = which;
			EmitSignal(SignalName.StateChanged, which.Name);
		}
				
		/// <summary>
        /// Sets the handle for the list.
        /// </summary>
        /// <param name="Handle">The handle to set.</param>
		public void SetHandle( Node3D Handle ) 
		{
			_Handle = Handle; 
		}
		
		/// <summary>
        /// Retrieves the currently active item in the list.
        /// </summary>
        /// <returns>The currently active item.</returns>
		public Control GetActive()
		{
			return _ActiveItem;
		}
					
		/// <summary>
        /// Handles toggling of the select menu.
        /// </summary>
		private void _OnButtonPressed() 
		{
			if( false == ListVisible ) 
			{
				foreach( AsSelectList List in GlobalExplorer.GetInstance().SelectLists ) 
				{
					if( List != this && List.ListVisible == true) 
					{
						List.ListVisible = false;
						for( int c = 1; c < List.GetChildCount(); c++) 
						{
							Control item = List.GetChild<Control>(c);
							
							if( item.Visible == true ) 
							{
								item.Visible = false;
							}
							
						}
					}	
				}
				
				for( int i = 1; i < GetChildCount(); i++) 
				{
					Control item = GetChild<Control>(i);
					
					if( item.Visible == false ) 
					{
						item.Visible = true;
					}
					
				}
			}
			else 
			{
				for( int i = 1; i < GetChildCount(); i++) 
				{
					Control item = GetChild<Control>(i);
					
					if( item.Visible == true ) 
					{
						item.Visible = false;
					}
				}
			}
			ListVisible = !ListVisible;
		}

		/// <summary>
        /// Resets the visibility of the list.
        /// </summary>
		public void ResetListVisibility()
		{
			ListVisible = false;
			for( int i = 1; i < GetChildCount(); i++) 
			{
				Control item = GetChild<Control>(i);
				
				if( item.Visible == true ) 
				{
					item.Visible = false;
				}
				
			}
		}

		/// <summary>
        /// Cleans up references and fields when exiting the scene tree.
        /// </summary>	
		public override void _ExitTree()
		{
			if( _Button.IsConnected(Button.SignalName.Pressed, new Callable(this, "_OnButtonPressed")) ) 
			{
				_Button.Disconnect(Button.SignalName.Pressed, new Callable(this, "_OnButtonPressed"));
			}
			
			ListVisible = false;
			_Button = null;
			_ActiveItem = null;
			_Handle = null;
			_Items = null;
		}
	}
}