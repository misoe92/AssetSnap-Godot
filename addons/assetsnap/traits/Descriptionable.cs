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
using Godot;

namespace AssetSnap.Component
{
	[Tool]
	public partial class Descriptionable : Trait.Base
	{
		public enum DescriptionType 
		{
			Small,
			Medium,
			Large,
		};
		
		public string Title = "";
		public DescriptionType Type = DescriptionType.Medium;
		
		public MarginContainer _MarginContainer;
		public VBoxContainer _BoxContainer;
		public Label _Label;
		
		public Descriptionable Initialize()
		{
			base._Instantiate( GetType().ToString() );
			if( Title == "" ) 
			{
				GD.PushWarning("Title not found");
				return this;
			}

			_InitializeFields();
			
			_MarginContainer.AddThemeConstantOverride("margin_left", 15);
			_MarginContainer.AddThemeConstantOverride("margin_right", 15);
			_MarginContainer.AddThemeConstantOverride("margin_top", 0);
			_MarginContainer.AddThemeConstantOverride("margin_bottom", 5);
			
			_Label.Text = Title;
			_Label.ThemeTypeVariation = Type.ToString();
			_Label.AutowrapMode = TextServer.AutowrapMode.Word;

			_BoxContainer.AddChild(_Label);	
			_MarginContainer.AddChild(_BoxContainer);
			
			return this;
		}
		
		public Descriptionable SetTitle( string title ) 
		{
			Title = title;
			
			return this;
		}
		
		public Descriptionable SetType(DescriptionType type) 
		{
			Type = type;

			return this;
		}
		
		public Descriptionable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		public string GetTitle()
		{
			return Title;
		}
		
		public void AddToContainer( Node Container ) 
		{
			base._AddToContainer(Container, _MarginContainer);
		}
		
		private void _InitializeFields()
		{
			_MarginContainer = new()
			{
				Name = "DescriptionMarginContainer"
			};
			_BoxContainer = new()
			{
				Name = "DescriptionBoxContainer"
			};
			_Label = new()
			{
				Name = "DescriptionLabel"
			};
		}
		
		public override void _ExitTree()
		{
			if( EditorPlugin.IsInstanceValid(_Label) ) 
			{
				_Label.QueueFree();
			}
			
			if( EditorPlugin.IsInstanceValid(_BoxContainer) ) 
			{
				_BoxContainer.QueueFree();
			}
			
			if( EditorPlugin.IsInstanceValid(_MarginContainer) ) 
			{
				_MarginContainer.QueueFree();
			}
		}
	}
}
#endif