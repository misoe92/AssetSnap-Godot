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
using System.Drawing;
using Godot;

namespace AssetSnap.Component
{
	[Tool]
	public partial class Labelable : Trait.Base
	{
		/*
		** Enums
		*/
		public enum TitleType 
		{
			HeaderSmall,
			HeaderMedium,
			HeaderLarge,
			TextSmall,
		};
		
		/*
		** Public
		*/
		public MarginContainer _MarginContainer;
		public HBoxContainer _BoxContainer;
		
		/*
		** Private
		*/
		private new Godot.Collections.Dictionary<string, int> Margin = new()
		{
			{"left", 15},
			{"right", 15},
			{"top", 10},
			{"bottom", 10},
		};
		private string Title = "";
		private TitleType Type = TitleType.HeaderMedium;
		private Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		private Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		private TextServer.AutowrapMode AutowrapMode = TextServer.AutowrapMode.Off;
		private Vector2 CustomMinimumSize;
		private Vector2 Size;
		private HorizontalAlignment _HorizontalAlignment;
		
		/*
		** Public methods
		*/
		
		/*
		** Instantiate an instance of the trait
		**
		** @return Labelable
		*/	
		public Labelable Instantiate()
		{
			base._Instantiate( GetType().ToString() );
			
			if( Title == "" ) 
			{
				GD.PushWarning("Title not found");
				return this;
			}

			_MarginContainer = new()
			{
				Name = "LabelMargin",
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
			};
			_BoxContainer = new()
			{
				Name = "LabelBox",
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
			};
			
			foreach( (string side, int value ) in Margin ) 
			{
				_MarginContainer.AddThemeConstantOverride("margin_" + side, value);
			}
			
			Label _WorkingNode = new() 
			{
				Name = Name,	
				Text = Title,
				CustomMinimumSize = CustomMinimumSize,
				Size = Size,
				ThemeTypeVariation = Type.ToString(),
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
				AutowrapMode = AutowrapMode,
				HorizontalAlignment = _HorizontalAlignment
			};
			
			_BoxContainer.AddChild(_WorkingNode);	
			_MarginContainer.AddChild(_BoxContainer);

			Nodes.Add(_WorkingNode);
			WorkingNode = _WorkingNode;

			Reset();
			
			return this;
		}
		
		/*
		** Selects an placed label in the
		** nodes array by index
		**
		** @param int index
		** @return Labelable
		*/
		public Labelable Select(int index)
		{
			base._Select(index);

			if( null != WorkingNode ) 
			{
				_BoxContainer = WorkingNode.GetParent() as HBoxContainer;
				if( EditorPlugin.IsInstanceValid(_BoxContainer) )
				{
					_MarginContainer = _BoxContainer.GetParent() as MarginContainer;		
				}
			}

			return this;
		}
		
		/*
		** Selects an placed label in the
		** nodes array by name
		**
		** @param string name
		** @return Labelable
		*/
		public Labelable SelectByName(string name)
		{
			base._SelectByName(name);

			return this;
		}
		
		/*
		** Adds the currently chosen button
		** to a specified container
		**
		** @param Node Container
		** @return void
		*/
		public void AddToContainer( Node Container, int? index = null ) 
		{
			base._AddToContainer(Container, _MarginContainer, index);
		}
		
		/*
		** Setter Methods
		*/
		
		/*
		** Sets the name of the current label
		**
		** @param string text
		** @return Labelable
		*/
		public Labelable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/*
		** Sets the text of the current button
		**
		** @param string text
		** @return Labelable
		*/
		public Labelable SetText( string text ) 
		{
			Title = text;
			
			if( EditorPlugin.IsInstanceValid(WorkingNode) && WorkingNode is Label labelNode ) 
			{
				labelNode.Text =text;
			}
			
			return this;
		}
		
		/*
		** Sets the theme type of the button,
		** which lays out a set of specified rules
		** from the theme that the button follows
		**
		** @param TitleType type
		** @return Labelable
		*/
		public Labelable SetType(TitleType type) 
		{
			Type = type;

			return this;
		}
		
		
		/*
		** Sets the alignment for the text
		** of the current label
		**
		** @param HorizontalAlignment alignment
		** @return Labelable
		*/
		public Labelable SetAlignment( HorizontalAlignment alignment ) 
		{
			_HorizontalAlignment = alignment;

			return this;
		}
		
		/*
		** Sets the dimensions for 
		** the current label
		**
		** @param int width
		** @param int height
		** @return Labelable
		*/
		public Labelable SetDimensions( int width, int height )
		{
			CustomMinimumSize = new Vector2( width, height);
			Size = new Vector2( width, height);

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Labelable
		*/
		public Labelable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Labelable
		*/
		public Labelable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;

			return this;
		}
		
		/*
		** Sets the auto wrap of the label
		** allowing it to break lines based
		** on rules
		**
		** @param TextServer.AutowrapMode mode
		** @return Labelable
		*/
		public Labelable SetAutoWrap( TextServer.AutowrapMode mode )
		{
			AutowrapMode = mode;

			return this;
		}
		
		/*
		** Sets margin values for 
		** the currently chosen label
		**
		** @param int value
		** @param string side
		** @return Labelable
		*/
		public Labelable SetMargin( int value, string side = "" ) 
		{
			if( side == "" ) 
			{
				Margin["top"] = value;
				Margin["bottom"] = value;
				Margin["left"] = value;
				Margin["right"] = value;
				
				if( null != WorkingNode ) 
				{
					foreach( (string marginSide, int marginValue ) in Margin ) 
					{
						_MarginContainer.AddThemeConstantOverride("margin_" + marginSide, marginValue);
					}
				}
			}
			else 
			{
				Margin[side] = value;
				
				if( null != WorkingNode ) 
				{
					_MarginContainer.AddThemeConstantOverride("margin_" + side, value);
				}
			}
			
			return this;
		}
		
		/*
		** Getter Methods
		*/
		
		/*
		** Fetches the text from the label
		**
		** @return string
		*/
		public string GetTitle()
		{
			return Title;
		}
		
		/*
		** Fetches the inner container
		** of the label
		**
		** @return HBoxContainer
		*/
		public HBoxContainer GetInnerContainer()
		{
			return _BoxContainer;
		}
		
		/*
		** Booleans
		*/
		
		/*
		** Checks if the label is valid
		**
		** @return HBoxContainer
		*/
		public override bool IsValid()
		{
			if( Nodes.Count > 0 && EditorPlugin.IsInstanceValid(_MarginContainer) ) 
			{
				return true;
			}
			
			return base.IsValid();
		}
		
		/*
		** Private
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
			_BoxContainer = null;
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