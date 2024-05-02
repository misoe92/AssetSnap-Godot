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
	using System.Threading.Tasks;
	using AssetSnap.Component;
	using AssetSnap.Nodes;
	using AssetSnap.Settings;
	using Godot;

	[Tool]
	public partial class ListEntry : LibraryComponent
	{

		private readonly static Theme SnapTheme = GD.Load<Theme>("res://addons/assetsnap/assets/themes/SnapTheme.tres");
		private readonly static Texture2D ChevronLeft = GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/chevron-left.svg");
		private readonly static Texture2D ChevronRight = GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/chevron-right.svg");
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

		private Container leftInnerContainer;
		private Container middleInnerContainer;
		private Container rightInnerContainer;

		private Label xLabel;
		private Label yLabel;
		private Label zLabel;
		
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
			
			UsingTraits = new()
			{
				{ typeof(Buttonable).ToString() },
				{ typeof(Containerable).ToString() },
			};
		}

		/*
		** Initializes the component
		**
		** @return void
		*/
		public override void Initialize()
		{
			//
		}
		
		public void _Initialize()
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

			Trait<Containerable>()
				.SetName("ListEntryContainer-" + filename.Split("-").Join("_"))
				.SetLayout(AssetSnap.Trait.ContainerTrait.ContainerLayout.ThreeColumns)
				.SetHorizontalSizeFlags(SizeFlags.ExpandFill)
				.SetVerticalSizeFlags(SizeFlags.ShrinkCenter)
				.SetInnerOrientation(AssetSnap.Trait.ContainerTrait.ContainerOrientation.Horizontal)
				.Instantiate();

			leftInnerContainer = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(0);

			middleInnerContainer = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(1);
			
			rightInnerContainer = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(2);
				
			if( null == leftInnerContainer || null == middleInnerContainer || null == rightInnerContainer)	
			{
				return;
			}

			leftInnerContainer.SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
			rightInnerContainer.SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
			
			leftInnerContainer.CustomMinimumSize = new Vector2(30, 0);
			rightInnerContainer.CustomMinimumSize = new Vector2(30, 0);

			middleInnerContainer.TooltipText = Filename;

			middleInnerContainer.Connect(VBoxContainer.SignalName.MouseEntered, Callable.From(() => { _OnMiddleContainerMouseEnter(); }));
			middleInnerContainer.Connect(VBoxContainer.SignalName.MouseExited, Callable.From(() => { _OnMiddleContainerMouseLeave(); }));

			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					_PanelContainer
				);

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

			xLabel = new()
			{
				ThemeTypeVariation = Labelable.TitleType.TextTinyDiffused.ToString(),
				Text = "X: " + Mathf.Round(SettingsUtils.Get().GetModelSize(filename).X * 100) / 100
			};

			yLabel = new()
			{
				ThemeTypeVariation = Labelable.TitleType.TextTinyDiffused.ToString(),
				Text = "Y: " + Mathf.Round(SettingsUtils.Get().GetModelSize(filename).Y * 100) / 100
			};

			zLabel = new()
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

			Plugin.Singleton.ModelSizeCacheChanged += (string name, Vector3 value) => _OnModelSizeChanged( name, value );
		}
		
		private void _OnModelSizeChanged( string name, Vector3 value )
		{
			if (name == Filename)
			{
				xLabel.Text = "X: " + Mathf.Round(value.X * 100) / 100;
				yLabel.Text = "Y: " + Mathf.Round(value.Y * 100) / 100;
				zLabel.Text = "Z: " + Mathf.Round(value.Z * 100) / 100;
			}
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

		private void _InitializeLeftArrow(Container BoxContainer, string FileName, string FolderPath, string LibraryName)
		{
			Trait<Buttonable>()
				.SetName(FolderPath + "-" + FileName + "-Left")
				.SetType(Buttonable.ButtonType.SmallFlatButton)
				.SetIcon(ChevronLeft.Duplicate(true) as Texture2D)
				.SetTooltipText("Rotate preview by -90 degrees")
				.SetCursorShape(CursorShape.PointingHand)
				.SetMouseFilter(MouseFilterEnum.Stop)
				.SetTheme(SnapTheme)
				.SetAction( () => { _OnLeftArrowPressed(); })
				.Instantiate()
				.Select(0)
				.AddToContainer(
					BoxContainer
				);
		}

		private void _InitializeRightArrow(Container BoxContainer, string FileName, string FolderPath, string LibraryName)
		{
			Trait<Buttonable>()
				.SetName(FolderPath + "-" + FileName + "-Right")
				.SetType(Buttonable.ButtonType.SmallFlatButton)
				.SetIcon(ChevronRight.Duplicate(true) as Texture2D)
				.SetTooltipText("Rotate preview by 90 degrees")
				.SetCursorShape(CursorShape.PointingHand)
				.SetMouseFilter(MouseFilterEnum.Stop)
				.SetTheme(SnapTheme)
				.SetAction( () => { _OnRightArrowPressed(); })
				.Instantiate()
				.Select(1)
				.AddToContainer(
					BoxContainer
				);
		}
		
		private void _OnRightArrowPressed()
		{
			if( EditorPlugin.IsInstanceValid(_TextureRect) ) 
			{
				if (ImageRotation == 180)
				{
					ImageRotation = -90;
				}
				else
				{
					ImageRotation += 90;
				}

				Texture2D image = SetRotatedImage(Filename, Library.GetName());
				_TextureRect._MeshPreviewReady(Folder + "/" + Filename, image, image, _TextureRect);
			}
		}
		
		private void _OnLeftArrowPressed()
		{
			if( EditorPlugin.IsInstanceValid(_TextureRect) ) 
			{
				if (ImageRotation == -180)
				{
					ImageRotation = 90;
				}
				else
				{
					ImageRotation -= 90;
				}

				Texture2D image = SetRotatedImage(Filename, Library.GetName());
				_TextureRect._MeshPreviewReady(Folder + "/" + Filename, image, image, _TextureRect);
			}
		}
		
		private Texture2D SetRotatedImage(string FileName, string LibraryName)
		{
			Texture2D image = null;
			string BasePath = "res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0];
			if (ImageRotation == 0)
			{
				if( FileAccess.FileExists( BasePath + "/default.png" ) ) 
				{
					image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default.png");
				}
			}
			else if (ImageRotation < 0)
			{
				if( FileAccess.FileExists( BasePath + "/default-minus" + ImageRotation + ".png" ) ) 
				{
					image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default-minus" + ImageRotation + ".png");
				}
			}
			else
			{
				if( FileAccess.FileExists( BasePath + "/default-" + ImageRotation + ".png" ) ) 
				{
					image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default-" + ImageRotation + ".png");
				}
			}

			return image;
		}

		/*
		** Initializes model preview image and
		** it's container
		**
		** @return void
		*/
		private void _InitializePreviewContainer(string FileName, string FolderPath, Container BoxContainer)
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

		public override void _ExitTree()
		{
			
			base._ExitTree();
		}
	}
}