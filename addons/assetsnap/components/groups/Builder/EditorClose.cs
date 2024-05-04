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

namespace AssetSnap.Front.Components.Groups.Builder
{
	/// <summary>
	/// Represents a component for closing the editor in the group builder.
	/// </summary>
	[Tool]
	public partial class EditorClose : LibraryComponent
	{
		private static readonly string Text = "Close"; 

		/// <summary>
		/// Initializes a new instance of the <see cref="EditorClose"/> class.
		/// </summary>
		public EditorClose()
		{
			Name = "GroupBuilderEditorClose";
			TooltipText = "Close the current group and stop editing it.";
			MouseDefaultCursorShape = Control.CursorShape.PointingHand;
			
			_UsingTraits = new()
			{
				{ typeof(Buttonable).ToString() },
			};
			
			// _include = false;
		}

		/// <summary>
		/// Initializes the component.
		/// </summary>
		public override void Initialize()
		{
			if( _Initiated ) 
			{
				return;
			}
			
			base.Initialize();
		
			_Initiated = true;

			_InitializeFields();
			_FinalizeFields();
		}

		/// <summary>
		/// Shows the component.
		/// </summary
		public void DoShow()
		{
			Trait<Buttonable>()
				.SetVisible(true);
		}
		
		/// <summary>
		/// Hides the component.
		/// </summary>
		public void DoHide()
		{
			Trait<Buttonable>()
				.SetVisible(false);
		}
		
		/// <summary>
		/// Event handler for closing the group.
		/// </summary>
		private void _OnCloseGroup()
		{
			string GroupPath = _GlobalExplorer.GroupBuilder._Editor.GroupPath;
			Sidebar sidebar = _GlobalExplorer.GroupBuilder._Sidebar;
			EditorListing listing = _GlobalExplorer.GroupBuilder._Editor.Listing;
			_GlobalExplorer.GroupBuilder._Editor.GroupPath = "";
			
			sidebar.Update(); 
			
			listing.Reset();
			listing.Update();
		}
		
		/// <summary>
		/// Initializes the fields of the component.
		/// </summary>
		private void _InitializeFields()
		{
			Trait<Buttonable>()
				.SetName("GroupBuilderEditorCloseButton")
				.SetType(Buttonable.ButtonType.DefaultButton)
				.SetText(Text)
				.SetTooltipText(TooltipText)
				.SetCursorShape(MouseDefaultCursorShape)
				.SetVisible(false)
				.SetAction(() => { _OnCloseGroup(); })
				.Instantiate();
		}
		
		/// <summary>
        /// Finalizes the fields of the component.
        /// </summary>
		private void _FinalizeFields()
		{
			Trait<Buttonable>()
				.Select(0)
				.AddToContainer(
					this
				);
		}
	}
}

#endif