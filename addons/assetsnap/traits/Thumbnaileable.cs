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
using AssetSnap.Trait;
using Godot;

namespace AssetSnap.Component
{
	[Tool]
	public partial class Thumbnaileable : ContainerTrait
	{
		/*
		** Private
		*/
		private TextureRect.ExpandModeEnum ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
		private TextureRect.StretchModeEnum StretchMode = TextureRect.StretchModeEnum.Keep;
		private string FilePath;
		
		/*
		** Public methods
		*/
		public Thumbnaileable()
		{
			Name = "Thumbnaileable";
			TypeString = GetType().ToString();
		}
		
		/*
		** Instantiate an instance of the trait
		**
		** @return Thumbnaileable
		*/	
		public override Thumbnaileable Instantiate()
		{
			base._Instantiate();
			base.Instantiate();
			
			AsModelViewerRect _TextureRect = new()
			{
				Name = TraitName + "-Preview",
				ExpandMode = ExpandMode,
				StretchMode = StretchMode
			};
			
			var mesh_preview = EditorInterface.Singleton.GetResourcePreviewer();
			mesh_preview.QueueResourcePreview(FilePath, _TextureRect, "_MeshPreviewReady", _TextureRect);
			
			GetInnerContainer(0).AddChild(_TextureRect);

			Dependencies[TraitName + "_WorkingNode"] = _TextureRect;
		
			Plugin.Singleton.traitGlobal.AddInstance(Iteration, _TextureRect, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.traitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);
			
			Reset();
			Iteration += 1;
			Dependencies = new();
			
			return this;
		}
		
		/*
		** Selects an placed thumbnail in the
		** nodes array by index
		**
		** @param int index
		** @return Thumbnaileable
		*/
		public override Thumbnaileable Select(int index, bool debug = false)
		{
			base._Select(index);
			
			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode") ) 
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.traitGlobal.GetDependencies(index, TypeString, OwnerName);
				Dependencies = dependencies;
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
		public override Thumbnaileable SelectByName( string name ) 
		{
			foreach( Container container in Nodes ) 
			{
				if( container.Name == name ) 
				{
					Dependencies[TraitName + "_WorkingNode"] = container;
					break;
				}
			}

			return this;
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
			if( false == Dependencies.ContainsKey(TraitName + "_MarginContainer") ) 
			{
				GD.PushError("Container was not found @ AddToContainer");
				GD.PushError("AddToContainer::Keys-> ", Dependencies.Keys);
				GD.PushError("AddToContainer::ADDTO-> ", TraitName + "_MarginContainer");
				return;
			}

			base._AddToContainer(Container, Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>());
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
		public override Thumbnaileable SetVisible( bool state ) 
		{
			base.SetVisible(state);

			return this;
		}
		
		/*
		** Sets the dimensions of the current thumbnail
		**
		** @param int width
		** @param int height
		** @return Thumbnaileable
		*/
		public override Thumbnaileable SetDimensions( int width, int height )
		{
			base.SetDimensions(width, height);
			
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
		public override Thumbnaileable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Thumbnaileable
		*/
		public override Thumbnaileable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}
		
		/*
		** Sets the orientation of the container
		**
		** @param ContainerOrientation orientation
		** @return Thumbnaileable
		*/
		public override Thumbnaileable SetOrientation(ContainerOrientation orientation) 
		{
			base.SetOrientation(orientation);

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
		public override Thumbnaileable SetMargin( int value, string side = "" ) 
		{
			base.SetMargin(value, side);
			
			return this;
		}
	}
}
#endif