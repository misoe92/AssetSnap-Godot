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

namespace AssetSnap.Front.Nodes
{
	using Godot;

	[Tool]
	public partial class AsSelectList : VBoxContainer
	{
		private Node3D _Handle { get; set; }
	
		[Export]
		public int _ActiveIndex { get; set; }
		[Export]
		public string _Label { get; set; }
		[Export]
		public Control _ActiveItem { get; set; }
		[Export]
		public Button _Button { get; set; }
		[Export]
		public Godot.Collections.Array<Control> _Items { get; set; }
		[Export]
		public bool ListVisible { get; set; }
		
		[Signal]
		public delegate void StateChangedEventHandler(string which);
		
		/*
		** Configuration of the select list
		** 
		** @return void
		*/
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
		
		/*
		** Handles visibility changes of the list
		** 
		** @param double delta
		** @return void
		*/
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

		/*
		** Creates the main button of the list
		** 
		** @return void
		*/
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
		
		/*
		** Sets the active list item
		** 
		** @param Control which
		** @return void
		*/
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
				
		/*
		** Sets the handle
		** 
		** @param Node3D Handle
		** @return void
		*/
		public void SetHandle( Node3D Handle ) 
		{
			_Handle = Handle; 
		}
		
		/*
		** Fetches the active item
		** 
		** @return Control
		*/
		public Control GetActive()
		{
			return _ActiveItem;
		}
					
		/*
		** Handles toggling of the select menu
		** 
		** @return void
		*/
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

		/*
		** Resets the list visibility
		** 
		** @return void
		*/
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

		/*
		** Cleans up references, parameters and fields
		** 
		** @return void
		*/	
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