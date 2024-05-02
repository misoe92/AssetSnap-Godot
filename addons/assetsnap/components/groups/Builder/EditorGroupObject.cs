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
		public int Index = 0;
		public string Path = "";

		public Vector3 Origin { get; set; }

		public Vector3 ObjectRotation { get; set; }

		public Vector3 ObjectScale { get; set; }

		public Godot.Collections.Dictionary<string, Variant> Options { get; set; }

		/** Components **/
		private Advanced _GroupBuilderEditorGroupObjectAdvanced;
		private Actions _GroupBuilderEditorGroupObjectActions;
		public AdvancedContainer _GroupBuilderEditorGroupObjectAdvancedContainer;

		public Origin _GroupBuilderEditorGroupObjectOrigin;
		public Scale _GroupBuilderEditorGroupObjectScale;
		public Rotation _GroupBuilderEditorGroupObjectRotation;

		public EditorGroupObject()
		{
			Name = "GroupBuilderEditorGroupObject";

			UsingTraits = new()
			{
				{ typeof(Labelable).ToString() },
				{ typeof(Thumbnaileable).ToString() },
				{ typeof(Containerable).ToString() },
				{ typeof(Panelable).ToString() },
			};

			//_include = false;
		}

		public override void Initialize()
		{
			base.Initialize();

			Initiated = true;

			_InitializeFields();

			Container RowInnerContainer = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer();

			Container RowOuterContainer = Trait<Containerable>()
				.Select(0)
				.GetOuterContainer();

			_FinalizeFields();

			_InitializePreviewContainer(FormatPathToFilename(Path), Path.Replace("/" + FormatPathToFilename(Path), "") , RowInnerContainer);

			_InitializeOriginContainer(RowInnerContainer);
			_InitializeRotationContainer(RowInnerContainer);
			_InitializeScaleContainer(RowInnerContainer);
			_InitializeActionsContainer(RowInnerContainer);
			_InitializeAdvancedContainer(RowInnerContainer);

			/** Advanced section of object **/
			_InitializeAdvancedContainerControl(RowOuterContainer);

		}

		public string FormatPathToFilename(string path)
		{
			string filename = StringHelper.FilePathToFileName(path);

			return filename;
		}

		public string FormatPathToTitle(string path)
		{
			string filename = StringHelper.FilePathToFileName(path);
			string title = StringHelper.FileNameToTitle(filename);

			return title;
		}

		private void _InitializeOriginContainer(Container RowInnerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.Origin"
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				_GroupBuilderEditorGroupObjectOrigin = GlobalExplorer.GetInstance().Components.Single<Origin>(true);
				_GroupBuilderEditorGroupObjectOrigin.Parent = this;
				_GroupBuilderEditorGroupObjectOrigin.Index = Index;
				_GroupBuilderEditorGroupObjectOrigin.Path = Path;
				_GroupBuilderEditorGroupObjectOrigin.Initialize();
				RowInnerContainer.AddChild(_GroupBuilderEditorGroupObjectOrigin);
			}
		}

		private void _InitializeRotationContainer(Container RowInnerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.Rotation"
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				_GroupBuilderEditorGroupObjectRotation = GlobalExplorer.GetInstance().Components.Single<Rotation>(true);
				_GroupBuilderEditorGroupObjectRotation.Index = Index;
				_GroupBuilderEditorGroupObjectRotation.Parent = this;
				_GroupBuilderEditorGroupObjectRotation.Path = Path;
				_GroupBuilderEditorGroupObjectRotation.Initialize();
				RowInnerContainer.AddChild(_GroupBuilderEditorGroupObjectRotation);
			}
		}

		private void _InitializeScaleContainer(Container RowInnerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.Scale"
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				_GroupBuilderEditorGroupObjectScale = GlobalExplorer.GetInstance().Components.Single<Scale>(true);
				_GroupBuilderEditorGroupObjectScale.Index = Index;
				_GroupBuilderEditorGroupObjectScale.Parent = this;
				_GroupBuilderEditorGroupObjectScale.Path = Path;
				_GroupBuilderEditorGroupObjectScale.Initialize();
				RowInnerContainer.AddChild(_GroupBuilderEditorGroupObjectScale);
			}
		}

		private void _InitializeActionsContainer(Container RowInnerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.Actions"
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				_GroupBuilderEditorGroupObjectActions = GlobalExplorer.GetInstance().Components.Single<Actions>(true);
				_GroupBuilderEditorGroupObjectActions.Index = Index;
				_GroupBuilderEditorGroupObjectActions.Path = Path;
				_GroupBuilderEditorGroupObjectActions.Initialize();
				RowInnerContainer.AddChild(_GroupBuilderEditorGroupObjectActions);
			}
		}

		private void _InitializeAdvancedContainer(Container RowInnerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.Advanced"
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				_GroupBuilderEditorGroupObjectAdvanced = GlobalExplorer.GetInstance().Components.Single<Advanced>(true);
				_GroupBuilderEditorGroupObjectAdvanced.Parent = this;
				_GroupBuilderEditorGroupObjectAdvanced.Initialize();
				RowInnerContainer.AddChild(_GroupBuilderEditorGroupObjectAdvanced);
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
				.SetMargin(0, "right")
				.SetMargin(15, "top")
				.SetMargin(15, "bottom")
				.SetFilePath(FolderPath + "/" + FileName)
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetExpandMode(TextureRect.ExpandModeEnum.FitHeightProportional)
				.SetStretchMode(TextureRect.StretchModeEnum.KeepAspectCentered)
				.SetContainerHorizontalSizeFlag(SizeFlags.ShrinkBegin)
				.Instantiate()
				.Select(0)
				.AddToContainer(
					BoxContainer
				);
		}

		private void _InitializeAdvancedContainerControl(Container container)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.AdvancedContainer"
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				_GroupBuilderEditorGroupObjectAdvancedContainer = GlobalExplorer.GetInstance().Components.Single<AdvancedContainer>(true);
				_GroupBuilderEditorGroupObjectAdvancedContainer.Index = Index;
				_GroupBuilderEditorGroupObjectAdvancedContainer.Options = Options;
				_GroupBuilderEditorGroupObjectAdvancedContainer.Path = Path;
				_GroupBuilderEditorGroupObjectAdvancedContainer.Initialize();
				container.AddChild(_GroupBuilderEditorGroupObjectAdvancedContainer);
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
				.SetMargin(30, "top")
				.SetMargin(10, "bottom")
				.SetText(Index.ToString())
				.SetContainerHorizontalSizeFlag(SizeFlags.ShrinkBegin)
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
				.AddToContainer(this);
		}
	}
}