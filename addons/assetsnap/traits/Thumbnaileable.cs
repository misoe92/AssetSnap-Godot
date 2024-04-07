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
using Godot;

namespace AssetSnap.Component
{
	[Tool]
	public partial class Thumbnaileable : Trait.Base
	{
		/*
		** Enums
		*/
		public enum ContainerOrientation 
		{
			Horizontal,
			Vertical,
		};
		
		/*
		** Public
		*/
		public VBoxContainer _InnerContainer;
		public MarginContainer _MarginContainer;
		
		/*
		** Private
		*/
		private Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		private Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		private TextureRect.ExpandModeEnum ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
		private TextureRect.StretchModeEnum StretchMode = TextureRect.StretchModeEnum.Keep;
		private ContainerOrientation Orientation = ContainerOrientation.Vertical;
		private Vector2 CustomMinimumSize;
		private Vector2 Size;
		private string FilePath;
		
		/*
		** Public methods
		*/
		
		/*
		** Instantiate an instance of the trait
		**
		** @return Thumbnaileable
		*/	
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
		
		/*
		** Selects an placed thumbnail in the
		** nodes array by index
		**
		** @param int index
		** @return Thumbnaileable
		*/
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
		
		/*
		** Selects an placed thumbnail in the
		** nodes array by name
		**
		** @param string name
		** @return Thumbnaileable
		*/
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
		
		/*
		** Show the current thumbnail
		**
		** @return void
		*/
		public void Show()
		{
			if( _MarginContainer is Control ControlNode ) 
			{
				ControlNode.Visible = true;
			}
		}
		
		/*
		** Hides the current thumbnail
		**
		** @return void
		*/
		public void Hide()
		{
			if( _MarginContainer is Control ControlNode ) 
			{
				ControlNode.Visible = false;
			}
		}
		
		/*
		** Adds the currently chosen thumbnail
		** to a specified container
		**
		** @param Node Container
		** @return void
		*/
		public void AddToContainer( Node Container ) 
		{
			base._AddToContainer(Container, _MarginContainer);
		}
		
		/*
		** Setter Methods
		*/
		
		/*
		** Sets the name of the current button
		**
		** @param string text
		** @return Buttonable
		*/
		public Thumbnaileable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/*
		** Sets the file path of the current thumbnail
		**
		** @param string path
		** @return Thumbnaileable
		*/
		public Thumbnaileable SetFilePath( string path ) 
		{
			FilePath = path;
			
			return this;
		}
		
		/*
		** Sets the visibility state of the
		** currently chosen thumbnail
		**
		** @param bool state
		** @return Thumbnaileable
		*/
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
		
		/*
		** Sets the dimensions of the current thumbnail
		**
		** @param int width
		** @param int height
		** @return Thumbnaileable
		*/
		public Thumbnaileable SetDimensions( int width, int height )
		{
			CustomMinimumSize = new Vector2( width, height);
			Size = new Vector2( width, height);

			return this;
		}
		
		/*
		** Sets the expand mode of the texture rect
		**
		** @param TextureRect.ExpandModeEnum mode
		** @return Thumbnaileable
		*/
		public Thumbnaileable SetExpandMode(TextureRect.ExpandModeEnum mode )
		{
			ExpandMode = mode;
			
			return this;
		}
		
		/*
		** Sets the stretch mode of the texture rect
		**
		** @param TextureRect.StretchModeEnum mode
		** @return Thumbnaileable
		*/
		public Thumbnaileable SetStretchMode( TextureRect.StretchModeEnum mode )
		{
			StretchMode = mode;

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Thumbnaileable
		*/
		public Thumbnaileable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Thumbnaileable
		*/
		public Thumbnaileable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;

			return this;
		}
		
		/*
		** Sets the orientation of the container
		**
		** @param ContainerOrientation orientation
		** @return Thumbnaileable
		*/
		public Thumbnaileable SetOrientation(ContainerOrientation orientation) 
		{
			Orientation = orientation;
			return this;
		}
		
		/*
		** Sets margin values for 
		** the currently chosen thumbnail
		**
		** @param int value
		** @param string side
		** @return Thumbnaileable
		*/
		public Thumbnaileable SetMargin( int value, string side = "" ) 
		{
			_SetMargin(value, side);
			
			return this;
		}
		
		/*
		** Getter Methods
		*/
		
		/*
		** Retrieves the inner container of the thumbnail
		**
		** @return Container
		*/
		public Container GetInnerContainer()
		{
			if( null != WorkingNode ) 
			{
				// Single placement
				return _InnerContainer as Container;
			}

			return null;
		}
		
		/*
		** Private Methods
		*/
		
		/*
		** Resets the trait to
		** a cleared state
		**
		** @return void
		*/
		private void Reset()
		{
			WorkingNode = null;
			_InnerContainer = null;
			_MarginContainer = null;
		}

		/*
		** Cleanup
		*/
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