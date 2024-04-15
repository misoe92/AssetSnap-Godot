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
	using AssetSnap.Trait;
	using Godot;
	using Godot.Collections;

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
				{ typeof(Listable).ToString() },
				{ typeof(Labelable).ToString() },
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
			base.Initialize();
			
			_GlobalExplorer._Plugin.FoldersLoaded += () => { _OnLoadEntries(); };
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
		** Forces the list to update
		** 
		** @return void
		*/
		public void ForceUpdate()
		{
			// if(
			// 	null != Trait<Labelable>() &&
			// 	Trait<Labelable>().ContainsIndex(0)
			// )
			// {
			// 	if( false == ClearTrait<Labelable>() ) 
			// 	{
			// 		GD.PushError("Container was not cleared");
			// 	}
			// }
			// AddTrait(typeof(Labelable));
			// if(
			// 	null != Trait<Panelable>() &&
			// 	Trait<Panelable>().ContainsIndex(0)
			// )
			// {
			// 	if( false == ClearTrait<Panelable>() ) 
			// 	{
			// 		GD.PushError("Container was not cleared");
			// 	}
			// }
			// AddTrait(typeof(Panelable));
			// if(
			// 	null != Trait<Containerable>() &&
			// 	Trait<Containerable>().ContainsIndex(0)
			// ) 
			// {
			// 	if( false == ClearTrait<Containerable>() ) 
			// 	{ 
			// 		GD.PushError("Container was not cleared");
			// 	}
			// }
			// AddTrait(typeof(Containerable));
			
			// if(
			// 	null != Trait<Listable>() &&
			// 	Trait<Listable>().ContainsIndex(0)
			// )
			// {
			// 	if( false == ClearTrait<Listable>() ) 
			// 	{
			// 		GD.PushError("Listable was not cleared");
			// 	}
			// }
			 
			// Trait<Containerable>()
			// 	.SetName( "ListingBoxContainer" )
			// 	.SetMargin(115, "left")
			// 	.SetMargin(35, "right")
			// 	.SetMargin(0, "top")
			// 	.SetMargin(0, "bottom")
			// 	.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
			// 	.Instantiate();
			
			// _UpdateListTable();
			
			// Trait<Containerable>()
			// 	.Select(0)
			// 	.AddToContainer(
			// 		Container
			// 	);
				
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
			
			if (null == Trait<Containerable>().Select(0).GetNode())
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
			if ( null == Trait<Containerable>().Select(0).GetInnerContainer() )
			{
				Trait<Containerable>()
					.SetName( "ListingBoxContainer" )
					.SetMargin(5, "left")
					.SetMargin(5, "right")
					.SetMargin(0, "top")
					.SetMargin(5, "bottom")
					.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
					.Instantiate();
			}

			_SetupFolderListing();

			if ( null != Trait<Containerable>().Select(0).GetInnerContainer() &&  null == Trait<Containerable>().Select(0).GetContainerParent() )
			{
				Trait<Containerable>()
					.Select(0)
					.AddToContainer(
						this
					);
			}
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
				
			if( null != Trait<Listable>().Select(0).GetNode() ) 
			{
				GD.Print("Runs");
				Trait<Listable>().Iteration = 0;
				Trait<Listable>().Clear(0);
			}
				
			Trait<Listable>()
				.SetName("LibrariesListing") 
				.SetCount(SettingsUtils.Get().FolderCount)
				.SetComponent("AssetSnap.Front.Components.LibrariesListingEntry")
				.SetDimensions(400, 0)
				.Each(
					(int index, BaseComponent component) =>
					{
						if (null != component && IsInstanceValid(component) && component is LibrariesListingEntry _component)
						{
							string title = _GlobalExplorer.Settings.Folders[index];
							_component.title = title;
							_component.Initialize();

							// AddChild(_component, true);
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
				.SetDimensions(500, 0)
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.Instantiate()
				.Select(1)
				.AddToContainer(
					panel
				);
		}
		
		public void _OnLoadEntries()
		{
			if( Initiated ) 
			{
				// GetParent().RemoveChild(this);
				// Clear();
			}
			
			Initiated = true;
			LoadInEntries();	
		}

		public override void _ExitTree()
		{
			_GlobalExplorer._Plugin.FoldersLoaded -= () => { _OnLoadEntries(); };
			
			base._ExitTree();
		}
	}
}