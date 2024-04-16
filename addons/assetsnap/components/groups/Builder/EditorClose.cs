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

namespace AssetSnap.Front.Components.Groups.Builder
{
	using AssetSnap.Component;
	using Godot;

	[Tool]
	public partial class EditorClose : LibraryComponent
	{
		private static readonly string Text = "Close"; 
	
		public EditorClose()
		{
			Name = "GroupBuilderEditorClose";
			TooltipText = "Close the current group and stop editing it.";
			MouseDefaultCursorShape = Control.CursorShape.PointingHand;
			
			UsingTraits = new()
			{
				{ typeof(Buttonable).ToString() },
			};
			
			// _include = false;
		}

		
		public override void Initialize()
		{
			base.Initialize();
		
			Initiated = true;

			_InitializeFields();
			_FinalizeFields();
		}
	
		public void DoShow()
		{
			Trait<Buttonable>()
				.SetVisible(true);
		}
			
		public void DoHide()
		{
			Trait<Buttonable>()
				.SetVisible(false);
		}
		
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