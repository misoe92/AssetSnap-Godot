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
	using AssetSnap.Explorer;
	using AssetSnap.Settings;
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
			
			UsingTraits = new()
			{
				{ typeof(Labelable).ToString() },
				{ typeof(Listable).ToString() },
				{ typeof(Panelable).ToString() },
				{ typeof(Containerable).ToString() },
			};
		
			//_include = false;
		}
		
		/*
		** Initialization of the component
		** 
		** @return void
		*/ 
		public override void Initialize()
		{		
			LoadInEntries();
		}
		
		public void LoadInEntries()
		{
			base.Initialize();
			
			if( null == Trait<Labelable>() || null == Trait<Listable>() ) 
			{
				GD.PushError("Failed to update since traits wasn't loaded");
				return;
			}
			
			CurrentFolderCount = _GlobalExplorer.Settings.FolderCount;
				
			if( null == Trait<Labelable>().Select(0).GetNode() ) 
			{
				Trait<Labelable>()
					.SetMargin(0, "bottom")
					.SetName( "ListingTitle" )
					.SetType( Labelable.TitleType.HeaderLarge)
					.SetText( TitleText )
					.Instantiate()
					.Select(0)
					.AddToContainer( this ) ;
			}
			
			_SetupListTable(); 
		}
		
		/*
		** Set's up the list table
		** 
		** @return void
		*/
		private void _SetupListTable()
		{
			if(
				ExplorerUtils.Get()
					.Settings
					.FolderCount == 0
			) 
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
			if (null == Trait<Containerable>().Select(0).GetNode())
			{
				Trait<Containerable>()
					.SetName("ListingBoxContainer")
					.SetMargin(5, "left")
					.SetMargin(5, "right")
					.SetMargin(0, "top")
					.SetMargin(0, "bottom")
					.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
					.Instantiate();
			}

			_SetupNoFolderLabel();

			if (
				null != Trait<Containerable>().Select(0).GetNode() &&
				null == Trait<Containerable>().Select(0).GetParentContainer()
			)
			{		
				Trait<Containerable>()
					.Select(0)
					.AddToContainer(
						this
					);
			}
		}
		
		/*
		** Set's up the folders list table
		** 
		** @return void
		*/
		private void _SetupFolderListTable()
		{
			if (null == Trait<Containerable>().Select(0).GetNode())
			{
				Trait<Containerable>()
					.SetName( "ListingBoxContainer" )
					.SetMargin(5, "left")
					.SetMargin(5, "right")
					.SetMargin(0, "top")
					.SetMargin(0, "bottom")
					.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
					.Instantiate();
			}

			_SetupFolderListing();

			if (
				null != Trait<Containerable>().Select(0).GetNode() &&
				null == Trait<Containerable>().Select(0).GetParentContainer()
			)
			{
				Trait<Containerable>()
					.Select(0)
					.AddToContainer(
						this
					);
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
			if(
				ExplorerUtils.Get().Settings.FolderCount == CurrentFolderCount
			) 
			{
				return; 
			}

			CurrentFolderCount = ExplorerUtils.Get().Settings.FolderCount;
			
			if( 
				ExplorerUtils.Get()
					.Settings
					.FolderCount == 0
			) 
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
			if(
				null == Trait<Containerable>().Select(0).GetInnerContainer()
			) 
			{
				GD.PushError("No container was found");
				return;	
			}
			
			Container container = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer();
				
			Trait<Listable>()
				.SetName("LibrariesListing") 
				.SetCount(SettingsUtils.Get().FolderCount)
				.SetComponent("AssetSnap.Front.Components.LibrariesListingEntry")
				.SetDimensions(400, 0)
				.Each(
					(int index, GodotObject component) =>
					{
						if (null != component && IsInstanceValid(component) && component is LibrariesListingEntry _component)
						{
							string title = _GlobalExplorer.Settings.Folders[index];
							_component.title = title;
							_component.Initialize();
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
			Container outerContainer = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(0);
				
			Trait<Panelable>()
				.SetName("NoFolderPanelContainer")
				.Instantiate()
				.Select(0)
				.AddToContainer(
					outerContainer
				);

			Node panel = Trait<Panelable>()
				.Select(0)
				.GetContainer();

			Trait<Labelable>()
				.SetName( "NotFoundText" )
				.SetType(Labelable.TitleType.TextMedium)
				.SetText(NotFoundText)
				.SetAutoWrap( TextServer.AutowrapMode.Word )
				.SetDimensions(400, 0)
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.Instantiate()
				.Select(1)
				.AddToContainer(
					panel
				);
		}
	}
}