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

using System.Collections.Generic;
using AssetSnap.Component;
using AssetSnap.Front.Components.Groups.Builder.GroupObject;
using AssetSnap.Helpers;
using Godot;

namespace AssetSnap.Front.Components.Groups.Builder
{
	/// <summary>
	/// Represents an editor group object used in the GroupBuilder.
	/// </summary>
	[Tool]
	public partial class EditorGroupObject : LibraryComponent
	{
		/// <summary>
		/// Index of the group object.
		/// </summary>
		public int Index = 0;
		
		/// <summary>
		/// Path of the group object.
		/// </summary>
		public string Path = "";

		/// <summary>
		/// Origin of the group object.
		/// </summary>
		public Vector3 Origin { get; set; }

		/// <summary>
		/// Rotation of the group object.
		/// </summary>
		public Vector3 ObjectRotation { get; set; }

		/// <summary>
		/// Scale of the group object.
		/// </summary>
		public Vector3 ObjectScale { get; set; }

		/// <summary>
		/// Options of the group object.
		/// </summary>
		public Godot.Collections.Dictionary<string, Variant> Options { get; set; }

		/** Components **/
		private GroupObject.Advanced _GroupBuilderEditorGroupObjectAdvanced;
		private GroupObject.Actions _GroupBuilderEditorGroupObjectActions;
		public GroupObject.AdvancedContainer _GroupBuilderEditorGroupObjectAdvancedContainer;

		public GroupObject.Origin _GroupBuilderEditorGroupObjectOrigin;
		public GroupObject.Scale _GroupBuilderEditorGroupObjectScale;
		public GroupObject.Rotation _GroupBuilderEditorGroupObjectRotation;

		/// <summary>
		/// Initializes a new instance of the <see cref="EditorGroupObject"/> class.
		/// </summary>
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

		/// <summary>
		/// Initializes the editor group object.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			Initiated = true;

			_InitializeFields();

			Godot.Container RowInnerContainer = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer();

			Godot.Container RowOuterContainer = Trait<Containerable>()
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

		/// <summary>
		/// Formats the path to the filename.
		/// </summary>
		/// <param name="path">The path to format.</param>
		/// <returns>The formatted filename.</returns>
		public string FormatPathToFilename(string path)
		{
			string filename = StringHelper.FilePathToFileName(path);

			return filename;
		}

		/// <summary>
		/// Formats the path to the title.
		/// </summary>
		/// <param name="path">The path to format.</param>
		/// <returns>The formatted title.</returns>
		public string FormatPathToTitle(string path)
		{
			string filename = StringHelper.FilePathToFileName(path);
			string title = StringHelper.FileNameToTitle(filename);

			return title;
		}

		/// <summary>
		/// Initializes the origin container.
		/// </summary>
		/// <param name="RowInnerContainer">The inner container.</param>
		private void _InitializeOriginContainer(Godot.Container RowInnerContainer)
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

		/// <summary>
		/// Initializes the rotation container.
		/// </summary>
		/// <param name="RowInnerContainer">The inner container.</param>
		private void _InitializeRotationContainer(Godot.Container RowInnerContainer)
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

		/// <summary>
		/// Initializes the scale container.
		/// </summary>
		/// <param name="RowInnerContainer">The inner container.</param>
		private void _InitializeScaleContainer(Godot.Container RowInnerContainer)
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

		/// <summary>
		/// Initializes the actions container.
		/// </summary>
		/// <param name="RowInnerContainer">The inner container.</param>
		private void _InitializeActionsContainer(Godot.Container RowInnerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.Actions"
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				_GroupBuilderEditorGroupObjectActions = GlobalExplorer.GetInstance().Components.Single<Groups.Builder.GroupObject.Actions>(true);
				_GroupBuilderEditorGroupObjectActions.Index = Index;
				_GroupBuilderEditorGroupObjectActions.Path = Path;
				_GroupBuilderEditorGroupObjectActions.Initialize();
				RowInnerContainer.AddChild(_GroupBuilderEditorGroupObjectActions);
			}
		}

		/// <summary>
		/// Initializes the advanced container.
		/// </summary>
		/// <param name="RowInnerContainer">The inner container.</param>
		private void _InitializeAdvancedContainer(Godot.Container RowInnerContainer)
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

		/// <summary>
		/// Initializes the preview container.
		/// </summary>
		/// <param name="FileName">Name of the file to preview.</param>
		/// <param name="FolderPath">Path of the folder containing the file.</param>
		/// <param name="BoxContainer">The container to add the preview to.</param>
		private void _InitializePreviewContainer(string FileName, string FolderPath, Godot.Container BoxContainer)
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

		/// <summary>
		/// Initializes the advanced container control.
		/// </summary>
		/// <param name="container">The container to add the control to.</param>
		private void _InitializeAdvancedContainerControl(Godot.Container container)
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

		/// <summary>
		/// Initializes the fields of the editor group object.
		/// </summary>
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

		/// <summary>
        /// Finalizes the fields of the editor group object.
        /// </summary>
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

#endif