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

using Godot;
using AssetSnap.Component;
using AssetSnap.Nodes;

namespace AssetSnap.Front.Components
{
	/// <summary>
	/// Represents a component handling general actions.
	/// </summary>
	[Tool]
	public partial class Actions : TraitableComponent
	{
		private readonly string TitleText = "General actions";

		/// <summary>
		/// Constructor for the Actions component.
		/// </summary>
		public Actions()
		{
			Name = "Actions";

			_UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
				{ typeof(Labelable).ToString() },
				{ typeof(Buttonable).ToString() },
			};

			/* Debugging Purpose */
			// _include = false;  
			/* -- */
		}

		/// <summary>
		/// Initializes the Actions component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			_Initiated = true;

			Trait<Containerable>()
				.SetName("AddFolderContainer")
				.SetVerticalSizeFlags(SizeFlags.ShrinkCenter)
				.SetInnerOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetSeparation(15)
				.SetMargin(20, "left")
				.SetMargin(20, "bottom")
				.Instantiate();

			Trait<Labelable>()
				.SetName("AddFolderButtonTitle")
				.SetType(Labelable.TitleType.HeaderMedium)
				.SetText(TitleText)
				.SetMargin(0, "bottom")
				.Instantiate()
				.Select(0)
				.AddToContainer(
					this
				);

			Trait<Buttonable>()
				.SetName("AddFolderButton")
				.SetText("Add Library")
				.SetType(Buttonable.ButtonType.ActionButton)
				.SetAction(() => { _OnButtonPressed(); })
				.Instantiate()
				.Select(0)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				 );

			Trait<Buttonable>()
				.SetName("ClearPreviewImages")
				.SetText("Clear preview images")
				.SetTooltipText("Warning: This will remove all current preview images, and as such the images will need to be generated again")
				.SetType(Buttonable.ButtonType.SmallDangerButton)
				.SetAction(() => { _OnClearImages(); })
				.Instantiate()
				.Select(1)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer()
				 );

			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					this
				);
		}

		/// <summary>
		/// Handles folder selection when the "Add Library" button is clicked.
		/// </summary>
		/// <param name="FolderPath">The path of the selected folder.</param>
		/// <param name="fileDialog">The FileDialog instance.</param>
		/// <returns>Async void.</returns>
		private void _OnFolderSelected(string FolderPath, FileDialog fileDialog)
		{
			_GlobalExplorer.Settings.AddFolder(FolderPath);

			fileDialog.QueueFree();
		}

		/// <summary>
		/// Handles the button pressed event of the "Add Library" button.
		/// </summary>
		/// <returns>Void.</returns>
		private void _OnButtonPressed()
		{
			// Create a FileDialog instance
			FileDialog fileDialog = new FileDialog();
			_GlobalExplorer._Plugin.AddChild(fileDialog);

			// Set the dialog mode to "OpenDir" for folder selection
			fileDialog.FileMode = FileDialog.FileModeEnum.OpenDir;
			// Connect the "dir_selected" signal to another Callable function
			fileDialog.DirSelected += (string FolderPath) => { _OnFolderSelected(FolderPath, fileDialog); };
			fileDialog.MinSize = new Vector2I(768, 768);
			// Show the file dialog
			fileDialog.PopupCentered();
		}

		/// <summary>
        /// Handles clearing preview images.
        /// </summary>
        /// <returns>Void.</returns>
		private void _OnClearImages()
		{
			ModelPreviewer.ClearPreviewImages("res://assetsnap/previews");
		}
	}
}

#endif