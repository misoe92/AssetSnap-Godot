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

namespace AssetSnap.Front.Components.Library
{
	using AssetSnap.Component;
	using AssetSnap.Explorer;
	using AssetSnap.Nodes;
	using AssetSnap.Settings;
	using Godot;

	[Tool]
	public partial class ListEntry : LibraryComponent
	{
		private string _Folder;
		private string _Filename;
		private string _FormattedFileName;
		private AsLibraryPanelContainer _PanelContainer;
		private MarginContainer _MarginContainer;
		private MarginContainer _LabelMarginContainer;
		private Label _Label;
		private VBoxContainer _InnerContainer;
		private AsModelViewerRect _TextureRect;
		private Control absoluteContainer;
		private int ImageRotation = 0;

		private VBoxContainer leftInnerContainer;
		private VBoxContainer middleInnerContainer;
		private VBoxContainer rightInnerContainer;

		public string Folder
		{
			get => _Folder;
			set
			{
				_Folder = value;
			}
		}

		public string Filename
		{
			get => _Filename;
			set
			{
				_Filename = value;
			}
		}


		/*
		** Class constructor
		**
		** @return void
		*/
		public ListEntry()
		{
			Name = "LibraryListEntry";
			//_include = false;
		}

		/*
		** Initializes the component
		**
		** @return void
		*/
		public override void Initialize()
		{
			SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
			SizeFlagsVertical = SizeFlags.ExpandFill;

			base.Initialize();
			Initiated = true;

			if (Filename == null || Folder == null)
			{
				return;
			}

			PrepareFilenameTitles();

			string filename = Filename;
			if (Filename.Contains("."))
			{
				filename = filename.Split(".")[0];
			}

			_PanelContainer = new()
			{
				Name = _Filename,
				Library = Library,
				MouseFilter = MouseFilterEnum.Stop,
			};

			HBoxContainer outerPanelContainer = new();
			leftInnerContainer = new()
			{
				SizeFlagsHorizontal = SizeFlags.ShrinkBegin,
				SizeFlagsVertical = SizeFlags.ShrinkCenter,
				CustomMinimumSize = new Vector2(30, 0),
			};

			middleInnerContainer = new()
			{
				Name = "ClickableContainer",
				TooltipText = Filename,
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				SizeFlagsVertical = SizeFlags.ShrinkCenter,
			};

			middleInnerContainer.Connect(VBoxContainer.SignalName.MouseEntered, Callable.From(() => { _OnMiddleContainerMouseEnter(); }));
			middleInnerContainer.Connect(VBoxContainer.SignalName.MouseExited, Callable.From(() => { _OnMiddleContainerMouseLeave(); }));

			rightInnerContainer = new()
			{
				SizeFlagsHorizontal = SizeFlags.ShrinkBegin,
				SizeFlagsVertical = SizeFlags.ShrinkCenter,
				CustomMinimumSize = new Vector2(30, 0),
			};

			outerPanelContainer.AddChild(leftInnerContainer);
			outerPanelContainer.AddChild(middleInnerContainer);
			outerPanelContainer.AddChild(rightInnerContainer);
			_PanelContainer.AddChild(outerPanelContainer);

			// Now you can use the previewImage as needed
			// For example, you could display it in your UI or store it for later use
			_PanelContainer.SetFilePath(Folder + "/" + Filename);

			absoluteContainer = new();
			// Set the Rect Min and Max sizes for the Control
			absoluteContainer.Size = new Vector2(200, 200);
			absoluteContainer.Visible = false;

			VBoxContainer absoluteVboxcontainer = new();
			absoluteVboxcontainer.Position = new Vector2(-25, 0);
			absoluteVboxcontainer.AddThemeConstantOverride("separation", 0);

			Label sizeLabel = new()
			{
				ThemeTypeVariation = Labelable.TitleType.TextTinyDiffused.ToString(),
				Text = "Size"
			};

			Label xLabel = new()
			{
				ThemeTypeVariation = Labelable.TitleType.TextTinyDiffused.ToString(),
				Text = "X: " + Mathf.Round(SettingsUtils.Get().GetModelSize(filename).X * 100) / 100
			};

			Label yLabel = new()
			{
				ThemeTypeVariation = Labelable.TitleType.TextTinyDiffused.ToString(),
				Text = "Y: " + Mathf.Round(SettingsUtils.Get().GetModelSize(filename).Y * 100) / 100
			};

			Label zLabel = new()
			{
				ThemeTypeVariation = Labelable.TitleType.TextTinyDiffused.ToString(),
				Text = "Z: " + Mathf.Round(SettingsUtils.Get().GetModelSize(filename).Z * 100) / 100
			};

			absoluteVboxcontainer.AddChild(sizeLabel);
			absoluteVboxcontainer.AddChild(xLabel);
			absoluteVboxcontainer.AddChild(yLabel);
			absoluteVboxcontainer.AddChild(zLabel);
			absoluteContainer.AddChild(absoluteVboxcontainer);
			middleInnerContainer.AddChild(absoluteContainer);
			_InitializeLeftArrow(leftInnerContainer, Filename, Folder, Library.GetName());
			_InitializeRightArrow(rightInnerContainer, Filename, Folder, Library.GetName());
			_InitializePreviewContainer(Filename, Folder, middleInnerContainer);
			_InitializeLabelContainer(_PanelContainer);

			// Add container to parent container
			AddChild(_PanelContainer);

			Library.AddPanel(_PanelContainer);

			Plugin.Singleton.ModelSizeCacheChanged += (string name, Vector3 value) =>
			{
				if (name == filename)
				{
					xLabel.Text = "X: " + Mathf.Round(value.X * 100) / 100;
					yLabel.Text = "Y: " + Mathf.Round(value.Y * 100) / 100;
					zLabel.Text = "Z: " + Mathf.Round(value.Z * 100) / 100;
				}

			};
		}

		private void _OnMiddleContainerMouseEnter()
		{
			leftInnerContainer.GetChild<Control>(0).Visible = false;
			rightInnerContainer.GetChild<Control>(0).Visible = false;
			absoluteContainer.Visible = true;
		}

		private void _OnMiddleContainerMouseLeave()
		{
			leftInnerContainer.GetChild<Control>(0).Visible = true;
			rightInnerContainer.GetChild<Control>(0).Visible = true;
			absoluteContainer.Visible = false;
		}

		private void _InitializeLeftArrow(VBoxContainer BoxContainer, string FileName, string FolderPath, string LibraryName)
		{
			Texture2D Icon = GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/chevron-left.svg");
			Button LeftArrowButton = new()
			{
				ThemeTypeVariation = "SmallFlatButton",
				Icon = Icon,
				Flat = true,
				TooltipText = "Rotate preview by -90 Degrees",
				Theme = GD.Load<Theme>("res://addons/assetsnap/assets/themes/SnapTheme.tres"),
				MouseDefaultCursorShape = CursorShape.PointingHand,
				MouseFilter = MouseFilterEnum.Stop
			};

			LeftArrowButton.Connect(Button.SignalName.Pressed,
				Callable.From(
					() =>
					{
						if (ImageRotation == -180)
						{
							ImageRotation = 90;
						}
						else
						{
							ImageRotation -= 90;
						}

						Texture2D image = null;
						if (ImageRotation == 0)
						{
							image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default.png");
						}
						else if (ImageRotation < 0)
						{
							image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default-minus" + ImageRotation + ".png");
						}
						else
						{
							image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default-" + ImageRotation + ".png");
						}

						_TextureRect._MeshPreviewReady(FolderPath + "/" + FileName, image, image, _TextureRect);
					}
				)
			);

			BoxContainer.AddChild(LeftArrowButton);
		}

		private void _InitializeRightArrow(VBoxContainer BoxContainer, string FileName, string FolderPath, string LibraryName)
		{
			Texture2D Icon = GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/chevron-right.svg");
			Button RightArrowButton = new()
			{
				ThemeTypeVariation = "SmallFlatButton",
				Icon = Icon,
				Flat = true,
				TooltipText = "Rotate preview by 90 Degrees",
				Theme = GD.Load<Theme>("res://addons/assetsnap/assets/themes/SnapTheme.tres"),
				MouseDefaultCursorShape = CursorShape.PointingHand,
				MouseFilter = MouseFilterEnum.Stop
			};

			RightArrowButton.Connect(Button.SignalName.Pressed,
				Callable.From(
					() =>
					{
						if (ImageRotation == 180)
						{
							ImageRotation = -90;
						}
						else
						{
							ImageRotation += 90;
						}

						Texture2D image = null;
						if (ImageRotation == 0)
						{
							image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default.png");
						}
						else if (ImageRotation < 0)
						{
							image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default-minus" + ImageRotation + ".png");
						}
						else
						{
							image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default-" + ImageRotation + ".png");
						}

						_TextureRect._MeshPreviewReady(FolderPath + "/" + FileName, image, image, _TextureRect);
					}
				)
			);

			BoxContainer.AddChild(RightArrowButton);
		}

		/*
		** Initializes model preview image and
		** it's container
		**
		** @return void
		*/
		private void _InitializePreviewContainer(string FileName, string FolderPath, VBoxContainer BoxContainer)
		{
			_MarginContainer = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			_InnerContainer = new()
			{
				// CustomMinimumSize = new Vector2(0, 125),
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};

			_TextureRect = new()
			{
				ExpandMode = TextureRect.ExpandModeEnum.FitHeightProportional,
				StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
				CustomMinimumSize = new Vector2(120, 120),
				SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};

			_MarginContainer.AddThemeConstantOverride("margin_left", 5);
			_MarginContainer.AddThemeConstantOverride("margin_right", 5);
			_MarginContainer.AddThemeConstantOverride("margin_top", 0);
			_MarginContainer.AddThemeConstantOverride("margin_bottom", 5);

			if (true == FileAccess.FileExists("res://assetsnap/previews/" + Library.GetName() + "/" + FileName.Split(".")[0] + "/default.png"))
			{
				Texture2D image = GD.Load<Texture2D>("res://assetsnap/previews/" + Library.GetName() + "/" + FileName.Split(".")[0] + "/default.png");
				_TextureRect._MeshPreviewReady(FolderPath + "/" + FileName, image, image, _TextureRect);
			}
			else
			{
				ModelPreviewer.Singleton.AddToQueue(FolderPath + "/" + FileName, _TextureRect, Library.GetName());
			}

			_MarginContainer.AddChild(_TextureRect);
			_InnerContainer.AddChild(_MarginContainer);
			BoxContainer.AddChild(_InnerContainer);
		}

		/*
		** Initializes label and its container
		** readable label
		**
		** @return void
		*/
		private void _InitializeLabelContainer(PanelContainer BoxContainer)
		{
			// Label
			if (_FormattedFileName.Length > 18)
			{
				_FormattedFileName += "...";
			}

			MarginContainer _PanelMarginContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter,
				SizeFlagsVertical = Control.SizeFlags.ShrinkEnd,
			};

			_LabelMarginContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter,
				SizeFlagsVertical = Control.SizeFlags.ShrinkEnd,
			};

			_Label = new()
			{
				ThemeTypeVariation = Labelable.TitleType.TextSmall.ToString(),
				Text = _FormattedFileName.Capitalize(),
				HorizontalAlignment = HorizontalAlignment.Center,
			};

			PanelContainer _panel = new()
			{
				ThemeTypeVariation = Panelable.PanelType.RoundedPanelContainer.ToString(),
				SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter,
				SizeFlagsVertical = Control.SizeFlags.ShrinkEnd,
			};

			_LabelMarginContainer.AddThemeConstantOverride("margin_left", 5);
			_LabelMarginContainer.AddThemeConstantOverride("margin_right", 5);
			_LabelMarginContainer.AddThemeConstantOverride("margin_top", 1);
			_LabelMarginContainer.AddThemeConstantOverride("margin_bottom", 1);

			_PanelMarginContainer.AddThemeConstantOverride("margin_top", 0);
			_PanelMarginContainer.AddThemeConstantOverride("margin_bottom", 10);

			// Add children
			_LabelMarginContainer.AddChild(_Label);
			_panel.AddChild(_LabelMarginContainer);
			_PanelMarginContainer.AddChild(_panel);
			BoxContainer.AddChild(_PanelMarginContainer);
		}

		/*
		** Converts the bound filename into a more
		** readable label
		**
		** @return void
		*/
		private void PrepareFilenameTitles()
		{
			string filename = Filename;
			if (Filename.Contains("."))
			{
				filename = filename.Split(".")[0];
			}

			_FormattedFileName = filename.Substring(0, filename.Length > 18 ? 19 : filename.Length).Split("_").Join(" ").Split("-").Join(" ");
		}
	}
}