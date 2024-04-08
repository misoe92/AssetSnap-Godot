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
	using System;
	using AssetSnap.Component;
	using Godot;

	[Tool]
	public partial class LibrariesListingEntry : TraitableComponent
	{
		public string title;

		/*
		** Constructor of the class
		**  
		** @return void
		*/
		public LibrariesListingEntry()
		{
			Name = "LibrariesListingEntry";
			//_include = false;
		}

		public override void _EnterTree()
		{
			base._EnterTree();
		}

		/*
		** Initialization of the component
		** 
		** @return void
		*/
		public override void Initialize()
		{
			base.Initialize();
			
			AddTrait(typeof(Containerable));
			AddTrait(typeof(Panelable));
			AddTrait(typeof(Labelable));
			AddTrait(typeof(Buttonable));

			// Callable _removeFolderCallable = Callable.From();
			// Callable _yesRemoveFolderCallable = Callable.From();
			// Callable _noRemoveFolderCallable = Callable.From();
			
			Initiated = true;
			
			try 
			{
				Trait<Panelable>()
					.SetType(Panelable.PanelType.RoundedPanelContainer)
					.SetName("EntryPanelContainer")
					.SetMargin(2, "top")
					.SetMargin(2, "bottom")
					.Instantiate();
				
				Trait<Containerable>()
					.SetName("OuterContainer")
					.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
					.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
					.Instantiate();
					
				Trait<Containerable>()
					.SetName("BaseContainer")
					.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
					.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
					.SetMargin(0, "left")
					.SetMargin(15, "right")
					.SetMargin(2, "top")
					.SetMargin(2, "bottom")
					.SetOrientation(Containerable.ContainerOrientation.Horizontal)
					.Instantiate();
						
				Trait<Labelable>()
					.SetName("EntryTitle")
					.SetType(Labelable.TitleType.HeaderSmall)
					.SetText(title)
					.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
					.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
					.Instantiate();
					
				Trait<Buttonable>()
					.SetName("RemoveFolderButton")
					.SetType(Buttonable.ButtonType.SmallDangerButton)
					.SetText("Remove")
					.SetTooltipText("Click to remove the library")
					.SetAction( () => { this._OnRemove(); } )
					.Instantiate();
				
				Trait<Containerable>()
					.SetName("ChoiceContainer")
					.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
					.SetVerticalSizeFlags(Control.SizeFlags.ShrinkCenter)
					.SetMargin(0, "left")
					.SetMargin(15, "right")
					.SetMargin(2, "top")
					.SetMargin(2, "bottom")
					.SetOrientation(Containerable.ContainerOrientation.Horizontal)
					.Instantiate();
					
				Trait<Labelable>()
					.SetName("ChoiceTitle")
					.SetType(Labelable.TitleType.HeaderSmall)
					.SetText("Are you sure you wish to continue?")
					.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
					.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
					.Instantiate();
					
				Trait<Buttonable>()
					.SetName("YesRemoveFolder")
					.SetType(Buttonable.ButtonType.SmallSuccesButton)
					.SetText("Confirm")
					.SetTooltipText("Click to remove the library")
					.SetAction( () => { this._OnConfirm(); } )
					.Instantiate();
					
				Trait<Buttonable>()
					.SetName("NoRemoveFolder")
					.SetType(Buttonable.ButtonType.SmallDefaultButton)
					.SetText("Cancel")
					.SetTooltipText("Cancel Removal")
					.SetAction( () => { this._OnReject(); } )
					.Instantiate();
	
				var container = Trait<Containerable>()
					.Select(0)
					.GetInnerContainer();
									
				var baseContainer = Trait<Containerable>()
					.Select(1)
					.GetInnerContainer();
								
				var choiceContainer = Trait<Containerable>()
					.Select(2)
					.SetVisible(false)
					.GetInnerContainer();

					
				Trait<Labelable>()
					.Select(1)
					.AddToContainer(
						choiceContainer
					);
						
				Trait<Buttonable>()
					.Select(2)
					.AddToContainer(
						choiceContainer
					);
					
				Trait<Buttonable>()
					.Select(1)
					.AddToContainer(
						choiceContainer
					);
				
				Trait<Labelable>()
					.Select(0)
					.AddToContainer(
						baseContainer
					);
					
				Trait<Buttonable>()
					.Select(0)
					.AddToContainer(
						baseContainer
					);
				
				// Choice
				Trait<Containerable>()
					.Select(2)
					.AddToContainer(
						container
					);
					
				// Base
				Trait<Containerable>()
					.Select(1)
					.AddToContainer(
						container
					);

				Trait<Containerable>()
					.Select(0)
					.AddToContainer(
						Trait<Panelable>()
							.Select(0)
							.GetNode()
					);

				Trait<Panelable>()
					.Select(0)
					.AddToContainer(
						_Container
					);
			}
			catch( Exception e ) 
			{	
				GD.PushWarning(e.Message);
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
			Containerable baseContainer = Trait<Containerable>()
				.Select(1);
			baseContainer.Hide();

			Containerable choiceContainer = Trait<Containerable>()
				.Select(2);
			choiceContainer.Show();
		}
		
		/*
		** Confirms a removal of a folder
		** 
		** @return async<void>
		*/
		private void _OnConfirm()
		{
			Containerable baseContainer = Trait<Containerable>()
				.Select(1);
			baseContainer.Show();

			Containerable choiceContainer = Trait<Containerable>()
				.Select(2);
			choiceContainer.Hide();
			
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
						
						// child.Free();

						// FreeAllChildrenRecursively(child);
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
			Containerable baseContainer = Trait<Containerable>()
				.Select(1);
			baseContainer.Show();

			Containerable choiceContainer = Trait<Containerable>()
				.Select(2);
			choiceContainer.Hide();
		}

		public override void _ExitTree()
		{
			// Callable _removeFolderCallable = Callable.From(() => { this._OnRemove(); });
			// Callable _yesRemoveFolderCallable = Callable.From(() => { this._OnConfirm(); });
			// Callable _noRemoveFolderCallable = Callable.From(() => { this._OnReject(); });
			// if( Trait<Buttonable>().Select(0).IsValid() ) 
			// {
			// 	GD.Print("Exiting now");
			// 	GodotObject _object = Trait<Buttonable>()
			// 		.Select(0)
			// 		.GetNode();
					
			// 	GodotObject _objectTwo = Trait<Buttonable>()
			// 		.Select(1)
			// 		.GetNode();
					
			// 	GodotObject _objectThree = Trait<Buttonable>()
			// 		.Select(2)
			// 		.GetNode();
					
			// 	if ( _object is Button button && button.IsConnected(Button.SignalName.Pressed, _removeFolderCallable))
			// 	{
			// 		button.Disconnect(Button.SignalName.Pressed, _removeFolderCallable); 
			// 	}
				
			// 	if ( _objectTwo is Button buttonTwo && buttonTwo.IsConnected(Button.SignalName.Pressed, _yesRemoveFolderCallable))
			// 	{
			// 		buttonTwo.Disconnect(Button.SignalName.Pressed, _yesRemoveFolderCallable); 
			// 	}
				
			// 	if ( _objectThree is Button buttonThree && buttonThree.IsConnected(Button.SignalName.Pressed, _noRemoveFolderCallable))
			// 	{
			// 		buttonThree.Disconnect(Button.SignalName.Pressed, _noRemoveFolderCallable); 
			// 	}
			// }
			
			base._ExitTree();
		}
	}
}