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
	public partial class Titleable : Trait.Base
	{
		public enum TitleType 
		{
			HeaderSmall, 
			HeaderMedium,
			HeaderLarge,
		};
		
		public string Title = "";
		public string MinorTitle = "";
		public TitleType Type = TitleType.HeaderMedium;
		public MarginContainer _MarginContainer;
		public HBoxContainer _BoxContainer;
		public Label _Label; 
		public Label _MinorTitleLabel; 
		
		public Titleable Initialize()
		{
			base._Instantiate( GetType().ToString() );
			_InitializeFields();	
			
			Margin = new()
			{
				{"left", 15},
				{"right", 15},
				{"top", 10},
				{"bottom", 5},
			};
			
			if( Title == "" ) 
			{
				GD.PushWarning("Title not found");
				return this;
			}
			
			foreach( (string side, int value ) in Margin ) 
			{
				_MarginContainer.AddThemeConstantOverride("margin_" + side, value);
			}
			 
			_Label.Text = Title;
			_Label.ThemeTypeVariation = Type.ToString();
			
			_BoxContainer.AddChild(_Label);	
			
			if( MinorTitle != "" ) 
			{
				_MinorTitleLabel = new()
				{
					Text = MinorTitle,
				};
				_BoxContainer.AddChild(_MinorTitleLabel);	
			}
			
			_MarginContainer.AddChild(_BoxContainer);
			
			return this;
		}
				
		public Titleable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		public Titleable SetTitle( string title ) 
		{ 
			Title = title;
			
			return this;
		}
		 
		public Titleable SetMinorTitle( string title ) 
		{
			MinorTitle = title;
			
			return this;
		}
		
		public Titleable SetType(TitleType type) 
		{
			Type = type;

			return this;
		}
		
		public Titleable SetMargin( int value, string side = "" ) 
		{
			if( side == "" ) 
			{
				Margin["top"] = value;
				Margin["bottom"] = value;
				Margin["left"] = value;
				Margin["right"] = value;
			}
			else 
			{
				Margin[side] = value;
			}
			
			return this;
		}
		
		public string GetTitle()
		{
			return Title;
		}
		
		public HBoxContainer GetInnerContainer()
		{
			return _BoxContainer;
		}
		
		public void AddToContainer( Node Container ) 
		{
			base._AddToContainer(Container, _MarginContainer);
		}
		
		private void _InitializeFields()
		{
			_MarginContainer = new()
			{
				Name = "TitleMarginContainer"
			};
			_BoxContainer = new()
			{
				Name = "TitleBoxContainer"
			};
			_Label = new()
			{
				Name = "TitleLabel"
			};
		}
		
		public override void _ExitTree()
		{
			if( EditorPlugin.IsInstanceValid(_MinorTitleLabel) ) 
			{
				_MinorTitleLabel.QueueFree();
			}
			
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