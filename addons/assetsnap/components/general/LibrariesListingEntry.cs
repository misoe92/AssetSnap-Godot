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
using Godot;

namespace AssetSnap.Front.Components
{
	/// <summary>
	/// Represents an entry in the libraries listing.
	/// </summary>
	[Tool]
	public partial class LibrariesListingEntry : TraitableComponent
	{
		/// <summary>
		/// The title of the entry.
		/// </summary>
		public string Title;
		
		/// <summary>
		/// Constructor of the LibrariesListingEntry class.
		/// </summary>
		public LibrariesListingEntry()
		{
			Name = "LibrariesListingEntry";
			
			_UsingTraits = new()
			{
				{ typeof(Labelable).ToString() },
				{ typeof(Buttonable).ToString() },
				{ typeof(Containerable).ToString() },
				{ typeof(Panelable).ToString() },
			};
		
			//_include = false;
		}

		/// <summary>
		/// Initialization of the component.
		/// </summary>
		public override void Initialize() 
		{
			base.Initialize();
		
			_Initiated = true;
			
			Trait<Panelable>()
				.SetType(Panelable.PanelType.RoundedPanelContainer)
				.SetName("EntryPanelContainer")
				.SetMargin(2, "top")
				.SetMargin(2, "bottom")
				.SetMargin(55, "right")
				.SetMargin(15, "left")
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
				.SetText(Title)
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
					this
				);
		
		}
		
		/// <summary>
		/// Set visibility state and enables the ability to remove a folder.
		/// </summary>
		private void _OnRemove()
		{
			Containerable baseContainer = Trait<Containerable>()
				.Select(1);
			baseContainer.Hide();

			Containerable choiceContainer = Trait<Containerable>()
				.Select(2);
			choiceContainer.Show();
		}
		
		/// <summary>
		/// Confirms a removal of a folder.
		/// </summary>
		private void _OnConfirm()
		{
			Plugin.Singleton.EmitSignal(Plugin.SignalName.OnRemoveFolder, Title);
		}
		
		/// <summary>
		/// Rejects removal of a folder and reverses the visibility.
		/// </summary>
		/// <param name="_ChoiceContainer">The choice container.</param>
		/// <param name="_Container">The base container.</param>
		private void _OnReject()
		{
			Containerable baseContainer = Trait<Containerable>()
				.Select(1);
			baseContainer.Show();

			Containerable choiceContainer = Trait<Containerable>()
				.Select(2);
			choiceContainer.Hide();
		}
	}
}

#endif