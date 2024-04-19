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
	using AssetSnap.Nodes;
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
		private	Label _Label;
		private VBoxContainer _InnerContainer;
		private	AsModelViewerRect _TextureRect;

		private int ImageRotation = 0;
		
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
		
			if( Filename == null || Folder == null) 
			{
				return;
			}
			
			PrepareFilenameTitles();
			
			_PanelContainer = new()
			{
				TooltipText = Filename,
				Name = _Filename,
				Library = Library,
			};

			HBoxContainer outerPanelContainer = new();

			VBoxContainer leftInnerContainer = new()
			{
				SizeFlagsHorizontal = SizeFlags.ShrinkBegin,
				SizeFlagsVertical = SizeFlags.ShrinkCenter,
			};
			VBoxContainer middleInnerContainer = new()
			{
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				SizeFlagsVertical = SizeFlags.ShrinkCenter,
			};
			VBoxContainer rightInnerContainer = new()
			{
				SizeFlagsHorizontal = SizeFlags.ShrinkBegin,
				SizeFlagsVertical = SizeFlags.ShrinkCenter,
			};

			outerPanelContainer.AddChild(leftInnerContainer);
			outerPanelContainer.AddChild(middleInnerContainer);
			outerPanelContainer.AddChild(rightInnerContainer);

			_PanelContainer.AddChild(outerPanelContainer);
			
			// Now you can use the previewImage as needed
			// For example, you could display it in your UI or store it for later use
			_PanelContainer.SetFilePath(Folder + "\\" + Filename);

			_InitializeLeftArrow(leftInnerContainer, Filename, Folder, Library.GetName());
			_InitializeRightArrow(rightInnerContainer, Filename, Folder, Library.GetName());
			_InitializePreviewContainer(Filename, Folder, middleInnerContainer);
			_InitializeLabelContainer(_PanelContainer);
			
			// Add container to parent container
			AddChild(_PanelContainer);

			Library.AddPanel(_PanelContainer);
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
			};

			LeftArrowButton.Connect(Button.SignalName.Pressed,
				Callable.From(
					() => 
					{
						if( ImageRotation == -180 ) 
						{
							ImageRotation = 90;
						}
						else 
						{
							ImageRotation -= 90;
						}
						
						Texture2D image = null;
						if( ImageRotation == 0 ) 
						{
							image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default.png");
						}
						else if( ImageRotation < 0 ) 
						{
							image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default-minus" + ImageRotation + ".png");
						}
						else 
						{
							image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default-" + ImageRotation + ".png");
						}

						GD.Print(ImageRotation);
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
			};
			
			RightArrowButton.Connect(Button.SignalName.Pressed,
				Callable.From(
					() => 
					{
						if( ImageRotation == 180 ) 
						{
							ImageRotation = -90;
						}
						else 
						{
							ImageRotation += 90;
						}
						
						Texture2D image = null;
						if( ImageRotation == 0 ) 
						{
							image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default.png");
						}
						else if( ImageRotation < 0 ) 
						{
							image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default-minus" + ImageRotation + ".png");
						}
						else 
						{
							image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default-" + ImageRotation + ".png");
						}

						GD.Print(ImageRotation);
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
				CustomMinimumSize = new Vector2(0, 125),
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

			if( true == FileAccess.FileExists("res://assetsnap/previews/"  + Library.GetName() + "/" + FileName.Split(".")[0] + "/default.png") ) 
			{
				Texture2D image = GD.Load<Texture2D>("res://assetsnap/previews/" + Library.GetName() + "/" + FileName.Split(".")[0] + "/default.png");
				_TextureRect._MeshPreviewReady(FolderPath + "/" + FileName, image, image, _TextureRect);
			}
			else 
			{
				ModelPreviewer.Singleton.AddToQueue(FolderPath + "/" + FileName, _TextureRect, Library.GetName());
			}
			
			// var mesh_preview = EditorInterface.Singleton.GetResourcePreviewer();
			// mesh_preview.QueueResourcePreview(FolderPath + "/" + FileName, _TextureRect, "_MeshPreviewReady", _TextureRect);	
			
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
			if(_FormattedFileName.Length > 18) 
			{
				_FormattedFileName += "...";
			}
			
			_LabelMarginContainer = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ShrinkEnd,
			};
			
			_Label = new()
			{
				Text = _FormattedFileName,
				HorizontalAlignment = HorizontalAlignment.Center,
			};

			_LabelMarginContainer.AddThemeConstantOverride("margin_left", 10);
			_LabelMarginContainer.AddThemeConstantOverride("margin_right", 10);
			_LabelMarginContainer.AddThemeConstantOverride("margin_top", 5);
			_LabelMarginContainer.AddThemeConstantOverride("margin_bottom", 15);

			// Add children
			_LabelMarginContainer.AddChild(_Label);
			BoxContainer.AddChild(_LabelMarginContainer);
		} 
		
		/*
		** Converts the bound filename into a more
		** readable label
		**
		** @return void
		*/
		private void PrepareFilenameTitles() 
		{
			_FormattedFileName = Filename.Substring(0, Filename.Length > 18 ? 19 : Filename.Length).Split("_").Join(" ");
		}
	}
}