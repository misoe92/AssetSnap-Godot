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
	public partial class Panelable : Trait.Base
	{
		public enum PanelType
		{
			DefaultPanelContainer,
			RoundedPanelContainer,
			LightPanelContainer
		}
		public new Godot.Collections.Dictionary<string, int> Margin = new()
		{
			{"left", 0},
			{"right", 0},
			{"top", 5},
			{"bottom", 5},
		};
		
		public new Godot.Collections.Dictionary<string, int> Padding = new()
		{
			{"left", 10},
			{"right", 10},
			{"top", 5},
			{"bottom", 5},
		};
		
		private Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		private Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		
		private PanelType Type = PanelType.DefaultPanelContainer;
		private MarginContainer _MarginContainer;
		
		private MarginContainer _PaddingContainer;

		public Panelable Instantiate()
		{
			base._Instantiate( GetType().ToString() );

			_MarginContainer = new()
			{
				Name="PanelMarginContainer",
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
				Visible = Visible,
			};
			
			foreach( (string side, int value ) in Margin ) 
			{
				_MarginContainer.AddThemeConstantOverride("margin_" + side, value);
			}
			
			PanelContainer WorkingPanel = new()
			{
				Name = Name,
				ThemeTypeVariation = Type.ToString(),
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
			};
		
			_PaddingContainer = new()
			{
				Name="PanelPaddingContainer",
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
			};
			
			foreach( (string side, int value ) in Padding ) 
			{
				_PaddingContainer.AddThemeConstantOverride("margin_" + side, value);
			}

			WorkingPanel.AddChild(_PaddingContainer);
			_MarginContainer.AddChild(WorkingPanel);

			Nodes.Add(WorkingPanel);
			WorkingNode = WorkingPanel;

			Reset();
			
			return this;
		}
		
		public Panelable Select(int index)
		{
			base._Select(index);

			return this;
		}
		
		public Panelable SelectByName( string name ) 
		{
			foreach( Button button in Nodes ) 
			{
				if( button.Name == name ) 
				{
					WorkingNode = button;
					break;
				}
			}

			return this;
		}
		
		public Panelable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		public Panelable SetVisible( bool state ) 
		{
			base._SetVisible(state);
			
			if( null != WorkingNode && WorkingNode is PanelContainer panel ) 
			{
				_MarginContainer.Visible = state;
			}
			
			return this;
		}
		public Panelable SetPadding( int value, string side ) 
		{
			base._SetPadding(value, side);
			
			return this;
		}
			
		public Panelable SetType( PanelType type ) 
		{
			Type = type;
			return this;
		}
		
		public Panelable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;

			return this;
		}
		
		public Panelable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;

			return this;
		}
		
		public Panelable SetMargin( int value, string side = "" ) 
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
		
		public void AddToContainer( Node Container )
		{
			if( null != WorkingNode ) 
			{
				// Single placement
				Container.AddChild(WorkingNode.GetParent());
			}
			else 
			{
				// Multi placement
				foreach( Node node in Nodes ) 
				{
					base._AddToContainer(Container, node);
				}
			}
		}
		
		private void Reset()
		{
			WorkingNode = null;
		}
		
		public override void _ExitTree()
		{
			Reset();
			
			// for( int i = 0; i < Nodes.Count; i++)
			// {
			// 	if( EditorPlugin.IsInstanceValid( Nodes[i] ) ) 
			// 	{
			// 		PanelContainer panel = Nodes[i] as PanelContainer;
			// 		Node container = panel.GetParent();
			// 		panel.QueueFree();

			// 		if( EditorPlugin.IsInstanceValid( container ) ) 
			// 		{
			// 			container.QueueFree();
			// 		}
			// 	}
				
			// }
		}
	}
}
#endif