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
	public partial class LibrariesListing : TraitableComponent
	{
		private readonly string TitleText = "Libraries";
		private readonly string NotFoundText = "No folder libraries was found, to start using the addon add a folder first by using the button on the left with the label 'Add Library'.";
		private int CurrentFolderCount = 0;

		private Godot.Collections.Array<BaseComponent> _Entries = new();
		/*
		** Constructor of the class
		** 
		** @return void
		*/
		public LibrariesListing()
		{
			Name = "LibrariesListing";
			//_include = false;
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
			AddTrait(typeof(Marginable));
			AddTrait(typeof(Panelable));
			AddTrait(typeof(Titleable));
			AddTrait(typeof(Listable));
			AddTrait(typeof(Labelable));
			Initiated = true;
			
			CurrentFolderCount = _GlobalExplorer.Settings.FolderCount;

			Trait<Titleable>()
				.SetName( "ListingTitle" )
				.SetType( Titleable.TitleType.HeaderLarge)
				.SetTitle( TitleText )
				.Initialize() 
				.AddToContainer( Container ) ;
			 
			_SetupListTable(); 
		}
		
		/*
		** Forces the list to update
		** 
		** @return void
		*/
		public void ForceUpdate()
		{
			if( Trait<Labelable>().ContainsIndex(0) )
			{
				if( false == ClearTrait<Labelable>() ) 
				{
					GD.PushError("Labelable was not cleared");
				}
				AddTrait(typeof(Labelable));
			}
			
			if( Trait<Listable>().ContainsIndex(0) )
			{
				if( false == ClearTrait<Listable>() ) 
				{
					GD.PushError("Listable was not cleared");
				}
				AddTrait(typeof(Listable));
			}
			
			CurrentFolderCount = 0;
			_UpdateListTable();
		}
		
		/*
		** Set's up the list table
		** 
		** @return void
		*/
		private void _SetupListTable()
		{
			if( _GlobalExplorer.Settings.FolderCount == 0 ) 
			{
				_SetupNoFoldersTable();
				return;
			}
			
			_SetupFolderListTable();
		}
		
		/*
		** Set's up the no folders list table
		** 
		** @return void
		*/
		private void _SetupNoFoldersTable()
		{
			Trait<Containerable>()
				.SetName( "ListingBoxContainer" )
				.SetMargin(15, "left")
				.SetMargin(15, "right")
				.SetMargin(0, "top")
				.SetMargin(0, "bottom")
				.Instantiate();

			_SetupNoFolderLabel();
						
			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					Container
				);
		}
		
		/*
		** Set's up the folders list table
		** 
		** @return void
		*/
		private void _SetupFolderListTable()
		{
			Trait<Containerable>()
				.SetName( "ListingBoxContainer" )
				.SetMargin(15, "left")
				.SetMargin(15, "right")
				.SetMargin(0, "top")
				.SetMargin(5, "bottom")
				.Instantiate();

			_SetupFolderListing();

			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					Container
				);
		}
		
		/*
		** Clears the current list children
		** 
		** @return void
		*/
		private void _ClearList() 
		{
			var Children = Container.GetChildren();
			for( int i = 0; i < Children.Count; i++) 
			{
				var child = Children[i];

				// Container.RemoveChild(child);
				child.QueueFree();
			}
		}
		
		/*
		** Updates the list table, so it
		** shows the newest changes.
		** 
		** @return void
		*/
		private void _UpdateListTable()
		{
			if( _GlobalExplorer.Settings.FolderCount == CurrentFolderCount ) 
			{
				return; 
			}
			
			if( _GlobalExplorer.Settings.FolderCount == 0 ) 
			{
				_SetupNoFolderLabel();
				return;
			}
			
			_SetupFolderListing();
		}
		
		/*
		** Setups the table with folders
		**
		** @return void
		*/
		private void _SetupFolderListing()
		{
			Container container = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer();
				
			Trait<Listable>()
				.SetName("LibrariesListing")
				.SetCount(_GlobalExplorer.Settings.FolderCount)
				.SetComponent("AssetSnap.Front.Components.LibrariesListingEntry")
				.SetDimensions(500, 0)
				.Each(
					(int index, BaseComponent component) =>
					{
						if (null != component && IsInstanceValid(component) && component is LibrariesListingEntry _component)
						{
							string title = _GlobalExplorer.Settings.Folders[index];
							_component.title = title;
							_component.Initialize();

							AddChild(_component, true);
							// _Entries.Add(_component);
						}
					}
				)
				.Instantiate()
				.Select(0)
				.AddToContainer(
					container	
				);
		}
		
		/*
		** Setups the label with no folders found.
		**
		** @return void
		*/
		private void _SetupNoFolderLabel()
		{
			Trait<Panelable>()
				.SetName("NoFolderPanelContainer")
				.Instantiate()
				.Select(0)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				);
			
			
			Trait<Labelable>()
				.SetName( "NotFoundText" )
				.SetText(NotFoundText)
				.SetAutoWrap( TextServer.AutowrapMode.Word )
				.SetDimensions(500, 0)
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.Instantiate()
				.Select(0)
				.AddToContainer(
					Trait<Panelable>()
						.Select(0)
						.GetNode()
				);
		}

		public override void _ExitTree()
		{
			// foreach( GodotObject _object in _Entries ) 
			// {
			// 	if( null != _object && IsInstanceValid(_object) && _object is Node _node ) 
			// 	{
			// 		GD.Print("Unloaded library listing entry: ", _node.Name);
			// 		_node.QueueFree();
			// 	}
			// }
			base._ExitTree();
		}
	}
}