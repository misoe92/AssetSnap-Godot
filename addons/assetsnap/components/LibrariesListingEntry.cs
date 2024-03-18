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

	public partial class LibrariesListingEntry : BaseComponent
	{
		public string title;
		
		public PanelContainer _PanelContainer;
		public MarginContainer _MarginContainer;
		public HBoxContainer _OuterContainer;
		public HBoxContainer _BaseContainer;
		public HBoxContainer _ChoiceContainer;
		public Label _Title;
		public Button _RemoveButton;
		public Label _ChoiceTitle;
		public Button _Yes;
		public Button _No;
		
		public Callable? RemoveButtonCallable;
		public Callable? ConfirmRemoveButtonCallable;
		public Callable? CancelRemoveButtonCallable;

		
		/*
		** Constructor of the class
		**  
		** @return void
		*/
		public LibrariesListingEntry()
		{
			Name = "LibrariesListingEntry";
			// _include = false;
		}

		/*
		** Initialization of the component
		** 
		** @return void
		*/ 
		public override void Initialize()
		{
			_PanelContainer = new();
			_MarginContainer = new();
			_OuterContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};
			
			_BaseContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};
			
			_ChoiceContainer = new()
			{
				Visible = false,
				
				SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter,
				SizeFlagsVertical = Control.SizeFlags.ShrinkCenter,
			};
			
			_Title = new()
			{
				ThemeTypeVariation = "HeaderSmall",
				Text = title,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};

			_RemoveButton = new()
			{
				Name = "RemoveFolderButton",
				ThemeTypeVariation = "RemoveButton",
				Text = "Remove",
				TooltipText = "Click to remove the library",
				MouseDefaultCursorShape = Control.CursorShape.PointingHand
			};
			
			_ChoiceTitle = new()
			{
				ThemeTypeVariation = "HeaderSmall",
				Text = "Are you sure you wish to continue?",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			
			_Yes = new()
			{
				Name = "YesRemoveFolder",
				ThemeTypeVariation = "ConfirmButton",
				Text = "Confirm",
				TooltipText = "Click to remove the library",
				MouseDefaultCursorShape = Control.CursorShape.PointingHand
			};
			
			_No = new() 
			{
				Name = "NoRemoveFolder",
				ThemeTypeVariation = "RemoveButton",
				Text = "Cancel",
				TooltipText = "Cancel Removal",
				MouseDefaultCursorShape = Control.CursorShape.PointingHand
			};
			
			_MarginContainer.AddThemeConstantOverride("margin_left", 10);
			_MarginContainer.AddThemeConstantOverride("margin_right", 10);
			_MarginContainer.AddThemeConstantOverride("margin_top", 10);
			_MarginContainer.AddThemeConstantOverride("margin_bottom", 10);
			
			RemoveButtonCallable = new Callable(this, "_OnRemove");
			ConfirmRemoveButtonCallable = new Callable(this, "_OnConfirm");
			CancelRemoveButtonCallable = new Callable(this, "_OnReject");
 
			_ChoiceContainer.AddChild(_ChoiceTitle);
			_ChoiceContainer.AddChild(_Yes);
			_ChoiceContainer.AddChild(_No);
			_BaseContainer.AddChild(_Title);
			_BaseContainer.AddChild(_RemoveButton);
			_OuterContainer.AddChild(_ChoiceContainer);
			_OuterContainer.AddChild(_BaseContainer);
			
			_MarginContainer.AddChild(_OuterContainer);
			_PanelContainer.AddChild(_MarginContainer);
			
			_Container.AddChild(_PanelContainer);

			if( RemoveButtonCallable is Callable _callable ) 
			{
				_RemoveButton.Connect(Button.SignalName.Pressed, _callable);
			}
			
			if( ConfirmRemoveButtonCallable is Callable _ConfirmCallable ) 
			{
				_Yes.Connect(Button.SignalName.Pressed, _ConfirmCallable);
			}
			
			if( CancelRemoveButtonCallable is Callable _CancelCallable ) 
			{
				_No.Connect(Button.SignalName.Pressed, _CancelCallable);
			}
		}
		
		/*
		** Set visibility state and enables
		** the ability to remove a folder
		** 
		** @return void
		*/
		private void _OnRemove()
		{
			_ChoiceContainer.Visible = true;
			_BaseContainer.Visible = false;
		}
		
		/*
		** Confirms a removal of a folder
		** 
		** @return async<void>
		*/
		private void _OnConfirm()
		{
			_ChoiceContainer.Visible = false;
			_BaseContainer.Visible = true;

			_GlobalExplorer.Settings.RemoveFolder(title);

			if( null != _GlobalExplorer.Library ) 
			{
				_GlobalExplorer.Library.RemoveLibrary(title);
				_GlobalExplorer.Library.Refresh(_GlobalExplorer.BottomDock);
			}
			
			Godot.Collections.Array<Node> Children = _GlobalExplorer.BottomDock.Container.GetChildren();
			
			foreach( Node child in Children ) 
			{
				if( IsInstanceValid(child) && child.HasMeta("FolderPath") ) 
				{
					string childPath = child.GetMeta("FolderPath").As<string>();
					
					if( childPath == title ) 
					{
						if( null != child.GetParent() ) 
						{
							child.GetParent().RemoveChild(child);
						}
						
						child.QueueFree(); 
						
						if( IsInstanceValid(child) ) 
						{
							FreeAllChildrenRecursively(child);
						}				
					} 
				}
			}
		}
		
		private void FreeAllChildrenRecursively( Node child )
		{
			if( IsInstanceValid( child ) ) 
			{
				int childCount = child.GetChildCount();
				
				if( childCount == 0 ) 
				{
					return;
				}
				
				foreach( Node _child in child.GetChildren() ) 
				{
					if( IsInstanceValid( _child ) ) 
					{
						int _childCount = _child.GetChildCount();
						
						if( _childCount != 0 ) 
						{
							FreeAllChildrenRecursively(_child);
						}

						_child.QueueFree();
					}
				}
			}
		}
		
		/*
		** Rejects removal of a folder and
		** reverses the visibility
		** 
		** @param HBoxContainer _ChoiceContainer
		** @param HBoxContainer _Container
		** @return void
		*/
		private void _OnReject()
		{
			_ChoiceContainer.Visible = false;
			_BaseContainer.Visible = true;
		}
		
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			if( IsInstanceValid(_RemoveButton) && RemoveButtonCallable is Callable _RemoveCallable ) 
			{
				if( _RemoveButton.IsConnected(Button.SignalName.Pressed, _RemoveCallable) ) 
				{
					_RemoveButton.Disconnect(Button.SignalName.Pressed, _RemoveCallable);
				}
			}
			
			if( IsInstanceValid(_Yes) && ConfirmRemoveButtonCallable is Callable _ConfirmCallable ) 
			{
				if( _Yes.IsConnected(Button.SignalName.Pressed, _ConfirmCallable) ) 
				{
					_Yes.Disconnect(Button.SignalName.Pressed, _ConfirmCallable);
				}
			}
			
			if( IsInstanceValid(_No) && CancelRemoveButtonCallable is Callable _CancelCallable ) 
			{
				if( _No.IsConnected(Button.SignalName.Pressed, _CancelCallable) ) 
				{ 
					_No.Disconnect(Button.SignalName.Pressed, _CancelCallable);
				}
			}
				
			if( IsInstanceValid(_Title) ) 
			{
				_Title.QueueFree();
				_Title = null;
			}
				
			if( IsInstanceValid(_RemoveButton) ) 
			{
				_RemoveButton.QueueFree();
				_RemoveButton = null;
			}
				
			if( IsInstanceValid(_ChoiceTitle) ) 
			{
				_ChoiceTitle.QueueFree();
				_ChoiceTitle = null;
			}
				
			if( IsInstanceValid(_Yes) ) 
			{
				_Yes.QueueFree();
				_Yes = null;
			}	
				
			if( IsInstanceValid(_No) ) 
			{
				_No.QueueFree();
				_No = null;
			}	
					
			if( IsInstanceValid(_ChoiceContainer) ) 
			{
				_ChoiceContainer.QueueFree();
				_ChoiceContainer = null;
			}
				
			if( IsInstanceValid(_BaseContainer) ) 
			{
				_BaseContainer.QueueFree();
				_BaseContainer = null;
			}
				
			if( IsInstanceValid(_OuterContainer) ) 
			{
				_OuterContainer.QueueFree();
				_OuterContainer = null;
			}
			
			if( IsInstanceValid(_MarginContainer) ) 
			{
				_MarginContainer.QueueFree();
				_MarginContainer = null; 
			}
			
			if( IsInstanceValid(_PanelContainer) ) 
			{  
				_PanelContainer.QueueFree();
				_PanelContainer = null;
			}
			
			base._ExitTree();
		}
	}
}