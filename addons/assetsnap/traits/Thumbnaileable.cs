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
using System;
using System.IO;
using Godot;

namespace AssetSnap.Component
{
	[Tool]
	public partial class Thumbnaileable : Trait.Base
	{
		
		public enum ContainerOrientation 
		{
			Horizontal,
			Vertical,
		};
		
		private Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		private Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		private TextureRect.ExpandModeEnum ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
		private TextureRect.StretchModeEnum StretchMode = TextureRect.StretchModeEnum.Keep;
		public ContainerOrientation Orientation = ContainerOrientation.Vertical;
		
		private VBoxContainer _InnerContainer;
		
		private MarginContainer _MarginContainer;

		private string FilePath;
		private Vector2 CustomMinimumSize;
		private Vector2 Size;
		
		public Thumbnaileable Instantiate()
		{
			try 
			{
				base._Instantiate( GetType().ToString() );
	
				// Margin Container 
				// VBox
				// Texture
				_MarginContainer = new()
				{
					Name = Name + "-Margin",
					CustomMinimumSize = CustomMinimumSize,
					SizeFlagsVertical = SizeFlagsVertical,
					SizeFlagsHorizontal = SizeFlagsHorizontal
				};
				
				foreach( (string side, int value ) in Margin ) 
				{
					_MarginContainer.AddThemeConstantOverride("margin_" + side, value);
				}
				
				_InnerContainer = new()
				{
					Name = Name + "-Inner",
					CustomMinimumSize = CustomMinimumSize,
					SizeFlagsVertical = SizeFlagsVertical,
					SizeFlagsHorizontal = SizeFlagsHorizontal
				};
				
				AsModelViewerRect _TextureRect = new()
				{
					Name = Name + "-Preview",
					ExpandMode = ExpandMode,
					StretchMode = StretchMode
				};
				
				var mesh_preview = EditorInterface.Singleton.GetResourcePreviewer();
				mesh_preview.QueueResourcePreview(FilePath, _TextureRect, "_MeshPreviewReady", _TextureRect);
				
				_InnerContainer.AddChild(_TextureRect);
				_MarginContainer.AddChild(_InnerContainer);

				Nodes.Add(_TextureRect);
				WorkingNode = _TextureRect;

				Reset();
			}
			catch(Exception e) 
			{
				GD.PushError(e.Message);
			}
			
			return this;
		}
		
		public void Show()
		{
			if( _MarginContainer is Control ControlNode ) 
			{
				ControlNode.Visible = true;
			}
		}
		
		public void Hide()
		{
			if( _MarginContainer is Control ControlNode ) 
			{
				ControlNode.Visible = false;
			}
		}
		
		public Thumbnaileable SetMargin( int value, string side = "" ) 
		{
			_SetMargin(value, side);
			
			return this;
		}
		
		public Thumbnaileable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		public Thumbnaileable SetFilePath( string path ) 
		{
			FilePath = path;
			
			return this;
		}
		
		public Thumbnaileable SetDimensions( int width, int height )
		{
			CustomMinimumSize = new Vector2( width, height);
			Size = new Vector2( width, height);

			return this;
		}
		
		public Thumbnaileable SetExpandMode(TextureRect.ExpandModeEnum mode )
		{
			ExpandMode = mode;
			
			return this;
		}
		
		public Thumbnaileable SetStretchMode( TextureRect.StretchModeEnum mode )
		{
			StretchMode = mode;

			return this;
		}
		
		public Thumbnaileable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;

			return this;
		}
		
		public Thumbnaileable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;

			return this;
		}
		
		public Thumbnaileable SetOrientation(ContainerOrientation orientation) 
		{
			Orientation = orientation;
			return this;
		}
		
		public Thumbnaileable SetVisible( bool state ) 
		{
			if( EditorPlugin.IsInstanceValid(_MarginContainer) && _MarginContainer is Control controlNode)  
			{
				controlNode.Visible = state;
			}
			else 
			{
				GD.PushError("MarginContainer is invalid");
			}

			return this;
		}
		
		public Thumbnaileable Select(int index)
		{
			base._Select(index);
			
			if( EditorPlugin.IsInstanceValid(WorkingNode) && EditorPlugin.IsInstanceValid(WorkingNode.GetParent()) ) 
			{
				_InnerContainer = WorkingNode.GetParent() as VBoxContainer;
			}
			else 
			{
				GD.PushError("Inner Container is invalid");
			}
			
			if( EditorPlugin.IsInstanceValid(_InnerContainer) && EditorPlugin.IsInstanceValid(_InnerContainer.GetParent()) ) 
			{
				_MarginContainer = _InnerContainer.GetParent() as MarginContainer;
			}
			else 
			{
				GD.PushError("Margin Container is invalid");
			}
			
			return this;
		}
		
		public Thumbnaileable SelectByName( string name ) 
		{
			foreach( Container container in Nodes ) 
			{
				if( container.Name == name ) 
				{
					WorkingNode = container;
					break;
				}
			}

			return this;
		}
		
		public Container GetInnerContainer( int index = 0)
		{
			if( null != WorkingNode ) 
			{
				// Single placement
				return _InnerContainer as Container;
			}
			else 
			{
				GD.PushWarning("Invalid inner container");
			}

			return null;
		}
		
		private void Reset()
		{
			WorkingNode = null;
			_InnerContainer = null;
			_MarginContainer = null;
		}

		public void AddToContainer( Node Container ) 
		{
			base._AddToContainer(Container, _MarginContainer);
		}
		
		public override void _ExitTree()
		{
			if( EditorPlugin.IsInstanceValid(WorkingNode) ) 
			{
				WorkingNode.QueueFree();
			}
		}
	}
}
#endif