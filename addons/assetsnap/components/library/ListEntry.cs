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
using AssetSnap.Front.Nodes;
using AssetSnap.Nodes;
using AssetSnap.Settings;
using Godot;

namespace AssetSnap.Front.Components.Library
{
	/// <summary>
	/// Represents an entry in the library list.
	/// </summary>
	[Tool]
	public partial class ListEntry : LibraryComponent
	{
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

		private readonly static Theme _SnapTheme = GD.Load<Theme>("res://addons/assetsnap/assets/themes/SnapTheme.tres");
		private readonly static Texture2D _ChevronLeft = GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/chevron-left.svg");
		private readonly static Texture2D _ChevronRight = GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/chevron-right.svg");
		private string _Folder;
		private string _Filename;
		private string _FormattedFileName;
		private int _ImageRotation = 0;
		
		private AsLibraryPanelContainer _PanelContainer;
		private MarginContainer _MarginContainer;
		private MarginContainer _LabelMarginContainer;
		private Label _Label;
		private VBoxContainer _InnerContainer;
		private AsModelViewerRect _TextureRect;
		private Control _AbsoluteContainer;

		private Container _LeftInnerContainer;
		private Container _MiddleInnerContainer;
		private Container _RightInnerContainer;

		private Label _XLabel;
		private Label _YLabel;
		private Label _ZLabel;

		/// <summary>
		/// Class constructor.
		/// </summary>
		public ListEntry()
		{
			Name = "LibraryListEntry";
			//_include = false;
			
			_UsingTraits = new()
			{
				{ typeof(Buttonable).ToString() },
				{ typeof(Containerable).ToString() },
			};
		}

		/// <summary>
		/// Initializes the component.
		/// </summary>
		public override void Initialize()
		{
			//
		}
		
		/// <summary>
		/// Initializes the component.
		/// </summary>
		public void _Initialize()
		{
			SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
			SizeFlagsVertical = SizeFlags.ExpandFill;

			base.Initialize();
			_Initiated = true;

			if (Filename == null || Folder == null)
			{
				return;
			}

			_PrepareFilenameTitles();

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

			_LeftInnerContainer = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(0);

			_MiddleInnerContainer = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(1);
			
			_RightInnerContainer = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(2);
				
			if( null == _LeftInnerContainer || null == _MiddleInnerContainer || null == _RightInnerContainer)	
			{
				return;
			}

			_LeftInnerContainer.SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
			_RightInnerContainer.SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
			
			_LeftInnerContainer.CustomMinimumSize = new Vector2(30, 0);
			_RightInnerContainer.CustomMinimumSize = new Vector2(30, 0);

			_MiddleInnerContainer.TooltipText = Filename;

			_MiddleInnerContainer.Connect(VBoxContainer.SignalName.MouseEntered, Callable.From(() => { _OnMiddleContainerMouseEnter(); }));
			_MiddleInnerContainer.Connect(VBoxContainer.SignalName.MouseExited, Callable.From(() => { _OnMiddleContainerMouseLeave(); }));

			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					_PanelContainer
				);

			// Now you can use the previewImage as needed
			// For example, you could display it in your UI or store it for later use
			_PanelContainer.SetFilePath(Folder + "/" + Filename);

			_AbsoluteContainer = new()
			{
				Size = new Vector2(200, 200),
				Visible = false
			};

			VBoxContainer absoluteVboxcontainer = new()
			{
				Position = new Vector2(-25, 0),
			};
			absoluteVboxcontainer.AddThemeConstantOverride("separation", 0);

			Label sizeLabel = new()
			{
				ThemeTypeVariation = Labelable.TitleType.TextTinyDiffused.ToString(),
				Text = "Size"
			};

			_XLabel = new()
			{
				ThemeTypeVariation = Labelable.TitleType.TextTinyDiffused.ToString(),
				Text = "X: " + Mathf.Round(SettingsUtils.Get().GetModelSize(filename).X * 100) / 100
			};

			_YLabel = new()
			{
				ThemeTypeVariation = Labelable.TitleType.TextTinyDiffused.ToString(),
				Text = "Y: " + Mathf.Round(SettingsUtils.Get().GetModelSize(filename).Y * 100) / 100
			};

			_ZLabel = new()
			{
				ThemeTypeVariation = Labelable.TitleType.TextTinyDiffused.ToString(),
				Text = "Z: " + Mathf.Round(SettingsUtils.Get().GetModelSize(filename).Z * 100) / 100
			};

			absoluteVboxcontainer.AddChild(sizeLabel);
			absoluteVboxcontainer.AddChild(_XLabel);
			absoluteVboxcontainer.AddChild(_YLabel);
			absoluteVboxcontainer.AddChild(_ZLabel);
			_AbsoluteContainer.AddChild(absoluteVboxcontainer);
			_MiddleInnerContainer.AddChild(_AbsoluteContainer);
			
			_InitializeLeftArrow(_LeftInnerContainer, Filename, Folder, Library.GetName());
			_InitializeRightArrow(_RightInnerContainer, Filename, Folder, Library.GetName());
			_InitializePreviewContainer(Filename, Folder, _MiddleInnerContainer);
			_InitializeLabelContainer(_PanelContainer);

			// Add container to parent container
			AddChild(_PanelContainer);
			Library.AddPanel(_PanelContainer);

			Plugin.Singleton.ModelSizeCacheChanged += (string name, Vector3 value) => _OnModelSizeChanged( name, value );
		}
		
		/// <summary>
		/// Handles the event when the model size changes.
		/// </summary>
		/// <param name="name">The name of the model.</param>
		/// <param name="value">The new size of the model.</param>
		private void _OnModelSizeChanged( string name, Vector3 value )
		{
			if (name == Filename)
			{
				_XLabel.Text = "X: " + Mathf.Round(value.X * 100) / 100;
				_YLabel.Text = "Y: " + Mathf.Round(value.Y * 100) / 100;
				_ZLabel.Text = "Z: " + Mathf.Round(value.Z * 100) / 100;
			}
		}

		/// <summary>
		/// Handles the event when the mouse enters the middle container.
		/// </summary>
		private void _OnMiddleContainerMouseEnter()
		{
			_LeftInnerContainer.GetChild<Control>(0).Visible = false;
			_RightInnerContainer.GetChild<Control>(0).Visible = false;
			_AbsoluteContainer.Visible = true;
		}

		/// <summary>
		/// Handles the event when the mouse leaves the middle container.
		/// </summary>
		private void _OnMiddleContainerMouseLeave()
		{
			_LeftInnerContainer.GetChild<Control>(0).Visible = true;
			_RightInnerContainer.GetChild<Control>(0).Visible = true;
			_AbsoluteContainer.Visible = false;
		}

		/// <summary>
		/// Initializes the left arrow button.
		/// </summary>
		/// <param name="BoxContainer">The container to which the button will be added.</param>
		/// <param name="FileName">The name of the file.</param>
		/// <param name="FolderPath">The path of the folder.</param>
		/// <param name="LibraryName">The name of the library.</param>
		private void _InitializeLeftArrow(Container BoxContainer, string FileName, string FolderPath, string LibraryName)
		{
			Trait<Buttonable>()
				.SetName(FolderPath + "-" + FileName + "-Left")
				.SetType(Buttonable.ButtonType.SmallFlatButton)
				.SetIcon(_ChevronLeft.Duplicate(true) as Texture2D)
				.SetTooltipText("Rotate preview by -90 degrees")
				.SetCursorShape(CursorShape.PointingHand)
				.SetMouseFilter(MouseFilterEnum.Stop)
				.SetTheme(_SnapTheme)
				.SetAction( () => { _OnLeftArrowPressed(); })
				.Instantiate()
				.Select(0)
				.AddToContainer(
					BoxContainer
				);
		}

		/// <summary>
		/// Initializes the right arrow button.
		/// </summary>
		/// <param name="BoxContainer">The container to which the button will be added.</param>
		/// <param name="FileName">The name of the file.</param>
		/// <param name="FolderPath">The path of the folder.</param>
		/// <param name="LibraryName">The name of the library.</param>
		private void _InitializeRightArrow(Container BoxContainer, string FileName, string FolderPath, string LibraryName)
		{
			Trait<Buttonable>()
				.SetName(FolderPath + "-" + FileName + "-Right")
				.SetType(Buttonable.ButtonType.SmallFlatButton)
				.SetIcon(_ChevronRight.Duplicate(true) as Texture2D)
				.SetTooltipText("Rotate preview by 90 degrees")
				.SetCursorShape(CursorShape.PointingHand)
				.SetMouseFilter(MouseFilterEnum.Stop)
				.SetTheme(_SnapTheme)
				.SetAction( () => { _OnRightArrowPressed(); })
				.Instantiate()
				.Select(1)
				.AddToContainer(
					BoxContainer
				);
		}
		
		/// <summary>
		/// Handles the event when the right arrow button is pressed.
		/// </summary>
		private void _OnRightArrowPressed()
		{
			if( EditorPlugin.IsInstanceValid(_TextureRect) ) 
			{
				if (_ImageRotation == 180)
				{
					_ImageRotation = -90;
				}
				else
				{
					_ImageRotation += 90;
				}

				Texture2D image = _SetRotatedImage(Filename, Library.GetName());
				_TextureRect._MeshPreviewReady(Folder + "/" + Filename, image, image, _TextureRect);
			}
		}
		
		/// <summary>
		/// Handles the event when the left arrow button is pressed.
		/// </summary>
		private void _OnLeftArrowPressed()
		{
			if( EditorPlugin.IsInstanceValid(_TextureRect) ) 
			{
				if (_ImageRotation == -180)
				{
					_ImageRotation = 90;
				}
				else
				{
					_ImageRotation -= 90;
				}

				Texture2D image = _SetRotatedImage(Filename, Library.GetName());
				_TextureRect._MeshPreviewReady(Folder + "/" + Filename, image, image, _TextureRect);
			}
		}
		

		/// <summary>
		/// Initializes the preview container.
		/// </summary>
		/// <param name="FileName">The name of the file.</param>
		/// <param name="FolderPath">The path of the folder.</param>
		/// <param name="BoxContainer">The container to which the preview container will be added.</param>
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

		/// <summary>
		/// Initializes the label container.
		/// </summary>
		/// <param name="BoxContainer">The container to which the label container will be added.</param>
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

		/// <summary>
		/// Prepares the filename for display as a label.
		/// </summary>
		private void _PrepareFilenameTitles()
		{
			string filename = Filename;
			if (Filename.Contains("."))
			{
				filename = filename.Split(".")[0];
			}

			_FormattedFileName = filename.Substring(0, filename.Length > 18 ? 19 : filename.Length).Split("_").Join(" ").Split("-").Join(" ");
		}
		
		/// <summary>
		/// Sets the rotated image based on the current rotation angle.
		/// </summary>
		/// <param name="FileName">The name of the file.</param>
		/// <param name="LibraryName">The name of the library.</param>
		/// <returns>The rotated image.</returns>
		private Texture2D _SetRotatedImage(string FileName, string LibraryName)
		{
			Texture2D image = null;
			string BasePath = "res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0];
			if (_ImageRotation == 0)
			{
				if( FileAccess.FileExists( BasePath + "/default.png" ) ) 
				{
					image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default.png");
				}
			}
			else if (_ImageRotation < 0)
			{
				if( FileAccess.FileExists( BasePath + "/default-minus" + _ImageRotation + ".png" ) ) 
				{
					image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default-minus" + _ImageRotation + ".png");
				}
			}
			else
			{
				if( FileAccess.FileExists( BasePath + "/default-" + _ImageRotation + ".png" ) ) 
				{
					image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default-" + _ImageRotation + ".png");
				}
			}

			return image;
		}
	}
}

#endif