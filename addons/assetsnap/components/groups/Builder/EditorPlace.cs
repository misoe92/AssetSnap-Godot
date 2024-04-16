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
	using AssetSnap.Instance.Input;
	using Godot;

	[Tool]
	public partial class EditorPlace : LibraryComponent
	{
		private static readonly string Text = "Place"; 
		private static readonly string TooltipText = "Will let you place the group of objects in the 3D world"; 
		private static readonly Control.CursorShape MouseDefaultCursorShape = Control.CursorShape.PointingHand; 
		
		public EditorPlace()
		{
			Name = "GroupBuilderEditorPlace";
			
			UsingTraits = new()
			{
				{ typeof(Buttonable).ToString() },
			};
			
			//_include = false;
		}
		
		public override void Initialize()
		{
			base.Initialize();
			
			Initiated = true;

			_InitializeFields();
			_FinalizeFields();
		}
		
		public void Show()
		{
			Trait<Buttonable>()
				.Select(0)
				.SetVisible( true );
		}
			
		public void Hide()
		{
			Trait<Buttonable>()
				.Select(0)
				.SetVisible( false );
		}
		
		private void _OnPlaceGroup()
		{
			_GlobalExplorer.GroupBuilder._Editor.OpenGroupOptions();
			_GlobalExplorer.States.PlacingMode = GlobalStates.PlacingModeEnum.Group;
			_GlobalExplorer.States.Group = _GlobalExplorer.GroupBuilder._Editor.Group;
			_GlobalExplorer.States.GroupedObject = _GlobalExplorer.GroupBuilder._Editor.Group.Build();

			_GlobalExplorer.GroupBuilder._Editor.Group.EachProperty(
				( string key, Variant value ) =>
				{
					_GlobalExplorer.States.Set(key, value);
				}
			);
			
			EditorInterface.Singleton.EditNode(_GlobalExplorer.States.GroupedObject);
			
			if( _GlobalExplorer.InputDriver is DragAddInputDriver DraggableInputDriver ) 
			{
				DraggableInputDriver.CalculateObjectSize();
			}
		}
		
		private void _InitializeFields()
		{
			Trait<Buttonable>()
				.SetName("GroupBuilderEditorPlace")
				.SetType( Buttonable.ButtonType.ActionButton )
				.SetText(Text)
				.SetTooltipText(TooltipText)
				.SetCursorShape(MouseDefaultCursorShape)
				.SetVisible(false)
				.SetAction( () => { _OnPlaceGroup(); } )
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