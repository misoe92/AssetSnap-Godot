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
	using System.Collections.Generic;
	using AssetSnap.Component;
    using AssetSnap.Front.Components.Groups.Builder.GroupObject;
    using AssetSnap.Helpers;
	using Godot;

	[Tool]
	public partial class EditorGroupObject : LibraryComponent
	{
		public EditorGroupObject()
		{
			Name = "GroupBuilderEditorGroupObject";
			//_include = false;
		}
		
		public int Index = 0;
		public string Path = "";

		public Vector3 Origin { get; set; }
		
		public Vector3 Rotation { get; set; }
		
		public Vector3 Scale { get; set; }
		
		public Godot.Collections.Dictionary<string, Variant> Options { get; set; }

		/** Components **/
		private Advanced _GroupBuilderEditorGroupObjectAdvanced;
		private Actions _GroupBuilderEditorGroupObjectActions;
		public AdvancedContainer _GroupBuilderEditorGroupObjectAdvancedContainer;
		
		public Origin _GroupBuilderEditorGroupObjectOrigin;
		public Scale _GroupBuilderEditorGroupObjectScale;
		public Rotation _GroupBuilderEditorGroupObjectRotation;
		
		public override void Initialize()
		{
			AddTrait(typeof(Containerable));
			AddTrait(typeof(Panelable));
			AddTrait(typeof(Labelable));
			AddTrait(typeof(Spinboxable));
			AddTrait(typeof(Buttonable));
			AddTrait(typeof(Thumbnaileable));
			
			Initiated = true;
			
			_InitializeFields();

			Container RowInnerContainer = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer();
				
			Container RowOuterContainer = Trait<Containerable>()
				.Select(0)
				.GetOuterContainer();
			
			_InitializePreviewContainer(FormatPathToFilename(Path), Path.Split("\\")[0], RowInnerContainer);

			_InitializeOriginContainer(RowInnerContainer);
			_InitializeRotationContainer(RowInnerContainer);
			_InitializeScaleContainer(RowInnerContainer);
			_InitializeActionsContainer(RowInnerContainer);
			_InitializeAdvancedContainer(RowInnerContainer);
			
			/** Advanced section of object **/
			_InitializeAdvancedContainerControl(RowOuterContainer);
			
			_FinalizeFields();
		}
		
		public string FormatPathToFilename( string path ) 
		{
			string filename = StringHelper.FilePathToFileName(path).Split("\\")[1];

			return filename;
		}
		
		public string FormatPathToTitle( string path ) 
		{
			string filename = StringHelper.FilePathToFileName(path).Split("\\")[1];
			string title = StringHelper.FileNameToTitle(filename);

			return title;
		}
		
		private void _InitializeOriginContainer(Container RowInnerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.Origin"
			};
			
			if (_GlobalExplorer.Components.HasAll( Components.ToArray() )) 
			{
				_GroupBuilderEditorGroupObjectOrigin = GlobalExplorer.GetInstance().Components.Single<Origin>(true);
				_GroupBuilderEditorGroupObjectOrigin.Container = RowInnerContainer;
				_GroupBuilderEditorGroupObjectOrigin.Parent = this;
				_GroupBuilderEditorGroupObjectOrigin.Index = Index;
				_GroupBuilderEditorGroupObjectOrigin.Path = Path;
				_GroupBuilderEditorGroupObjectOrigin.Initialize();
			}
		}
		
		private void _InitializeRotationContainer(Container RowInnerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.Rotation"
			};
			
			if (_GlobalExplorer.Components.HasAll( Components.ToArray() )) 
			{
				_GroupBuilderEditorGroupObjectRotation = GlobalExplorer.GetInstance().Components.Single<Rotation>(true);
				_GroupBuilderEditorGroupObjectRotation.Container = RowInnerContainer;
				_GroupBuilderEditorGroupObjectRotation.Index = Index;
				_GroupBuilderEditorGroupObjectRotation.Parent = this;
				_GroupBuilderEditorGroupObjectRotation.Path = Path;
				_GroupBuilderEditorGroupObjectRotation.Initialize();
			}
		}
		
		private void _InitializeScaleContainer(Container RowInnerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.Scale"
			};
			
			if (_GlobalExplorer.Components.HasAll( Components.ToArray() )) 
			{
				_GroupBuilderEditorGroupObjectScale = GlobalExplorer.GetInstance().Components.Single<Scale>(true);
				_GroupBuilderEditorGroupObjectScale.Container = RowInnerContainer;
				_GroupBuilderEditorGroupObjectScale.Index = Index;
				_GroupBuilderEditorGroupObjectScale.Parent = this;
				_GroupBuilderEditorGroupObjectScale.Path = Path;
				_GroupBuilderEditorGroupObjectScale.Initialize();
			}
		}
		
		private void _InitializeActionsContainer(Container RowInnerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.Actions"
			};
			
			if (_GlobalExplorer.Components.HasAll( Components.ToArray() )) 
			{
				_GroupBuilderEditorGroupObjectActions = GlobalExplorer.GetInstance().Components.Single<Actions>(true);
				_GroupBuilderEditorGroupObjectActions.Container = RowInnerContainer;
				_GroupBuilderEditorGroupObjectActions.Index = Index;
				_GroupBuilderEditorGroupObjectActions.Path = Path;
				_GroupBuilderEditorGroupObjectActions.Initialize();
			}
		}
		
		private void _InitializeAdvancedContainer(Container RowInnerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.Advanced"
			};
			
			if (_GlobalExplorer.Components.HasAll( Components.ToArray() )) 
			{
				_GroupBuilderEditorGroupObjectAdvanced = GlobalExplorer.GetInstance().Components.Single<Advanced>(true);
				_GroupBuilderEditorGroupObjectAdvanced.Container = RowInnerContainer;
				_GroupBuilderEditorGroupObjectAdvanced.Parent = this;
				_GroupBuilderEditorGroupObjectAdvanced.Initialize();
			}
		}
		
		/*
		** Initializes model preview image and
		** it's container
		**
		** @return void
		*/
		private void _InitializePreviewContainer(string FileName, string FolderPath, Container BoxContainer) 
		{
			Trait<Thumbnaileable>()
				.SetName("GroupObjectsPreviewRect")
				.SetMargin(0, "left")
				.SetMargin(20, "right")
				.SetMargin(10, "top")
				.SetMargin(10, "bottom")
				.SetFilePath(FolderPath + "/" + FileName )
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetExpandMode(TextureRect.ExpandModeEnum.FitHeightProportional)
				.SetStretchMode(TextureRect.StretchModeEnum.KeepAspectCentered)
				.SetDimensions(75, 75)
				.Instantiate()
				.Select(0)
				.AddToContainer(
					BoxContainer
				);
		}
		
		private void _InitializeAdvancedContainerControl( Container container )
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.AdvancedContainer"
			};
			
			if (_GlobalExplorer.Components.HasAll( Components.ToArray() )) 
			{
				_GroupBuilderEditorGroupObjectAdvancedContainer = GlobalExplorer.GetInstance().Components.Single<AdvancedContainer>(true);
				_GroupBuilderEditorGroupObjectAdvancedContainer.Container = container;
				_GroupBuilderEditorGroupObjectAdvancedContainer.Index = Index;
				_GroupBuilderEditorGroupObjectAdvancedContainer.Options = Options;
				_GroupBuilderEditorGroupObjectAdvancedContainer.Path = Path;
				_GroupBuilderEditorGroupObjectAdvancedContainer.Initialize();
			}
		}
		
		private void _InitializeFields()
		{
			Trait<Panelable>()
				.SetName("RowPanel")
				.SetMargin(0)
				.SetMargin(5, "bottom")
				.SetType(Panelable.PanelType.LightPanelContainer)
				.Instantiate();

			Trait<Containerable>()
				.SetName("RowContainer")
				.SetOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetInnerOrientation(Containerable.ContainerOrientation.Vertical)
				.Instantiate();

			Trait<Labelable>()
				.SetName("IndexLabel")
				.SetMargin(20, "left")
				.SetMargin(20, "right")
				.SetMargin(40, "top")
				.SetMargin(10, "bottom")
				.SetText(Index.ToString())
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.Instantiate();
		}
		
		private void _FinalizeFields()
		{
			Trait<Labelable>()
				.Select(0)
				.AddToContainer(
					Trait<Containerable>()
						.Select(0)
						.GetInnerContainer(),
					0
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
				.AddToContainer(Container);
		}
	}
}