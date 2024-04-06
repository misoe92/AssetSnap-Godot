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
using AssetSnap.Front.Nodes;
using Godot;
namespace AssetSnap.Component
{
	[Tool]
	public partial class Dropdownable : Trait.Base
	{
		public enum DropdownState
		{
			Hidden,
			Shown
		};

		[Export]
		public Godot.Collections.Array<Button> Items = new();

		public string DefaultValue = "";
		public string PrefixLabel = "";

		public MarginContainer _MarginContainer;
		public PanelContainer panelContainer;
		public MarginContainer _PanelPaddingContainer;
		public VBoxContainer panelInnerContainer;
		public DropdownButton SelectedBlock;
		public VBoxContainer ItemsInnerContainer;

		public Dropdownable Instantiate(int i)
		{
			base._Instantiate( GetType().ToString() );
			
			_MarginContainer = new() 
			{
				Name = "DropdownMarginPanel",
			};
			
			foreach( (string side, int value ) in Margin ) 
			{
				_MarginContainer.AddThemeConstantOverride("margin_" + side, value);
			}
			
			panelContainer = new()
			{
				Name = "DropdownPanel",
				ThemeTypeVariation = "DropdownPanel",
			};
			
			_PanelPaddingContainer = new()
			{
				Name = "DropdownPanelPaddingContainer",
				Visible = false,
			};

			foreach( (string side, int value ) in Padding ) 
			{ 
				_PanelPaddingContainer.AddThemeConstantOverride("margin_" + side, value);
			}
			
			panelInnerContainer = new()
			{
				Name = "DropdownPanelContainer",
			};
			
			SelectedBlock = new()
			{
				ThemeTypeVariation = "DropdownButton",
				Name = "DropdownSelected",
				MouseDefaultCursorShape = Control.CursorShape.PointingHand,
				Icon = GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/chevron-down.svg"),
				IconAlignment = HorizontalAlignment.Right,
				Alignment = HorizontalAlignment.Left,
				Margin = Margin,
			};
			
			if( "" != DefaultValue ) 
			{
				SelectedBlock.Text = DefaultValue;
			}
			else 
			{
				SelectedBlock.Text = "None";
			}

			// Action action = () => { _OnToggleVisibility(i); };
			// SelectedBlock.Pressed += action;
			
			ItemsInnerContainer = new()
			{
				Name = "DropdownInnerContainer",
			};

			WorkingNode = panelContainer; 

			_PanelPaddingContainer.AddChild(ItemsInnerContainer);
			panelInnerContainer.AddChild(SelectedBlock);
			panelInnerContainer.AddChild(_PanelPaddingContainer);
			panelContainer.AddChild(panelInnerContainer);
			_MarginContainer.AddChild(panelContainer);

			// Callable _callable = Callable.From(() => { _OnToggleVisibility(i); });
			// SelectedBlock.Connect( Button.SignalName.Pressed, _callable );
			
			Nodes.Add(WorkingNode);
			// _Actions.Add(action);
 
			Reset();

			return this;
		}
		
		public Dropdownable Select(int index)
		{
			base._Select(index);

			if( null != WorkingNode ) 
			{
				_MarginContainer = WorkingNode.GetParent() as MarginContainer;
				panelInnerContainer = WorkingNode.GetChild(0) as VBoxContainer;
				SelectedBlock = panelInnerContainer.GetChild(0) as DropdownButton; 
				_PanelPaddingContainer = panelInnerContainer.GetChild(1) as MarginContainer;
				ItemsInnerContainer = _PanelPaddingContainer.GetChild(0) as VBoxContainer;
			}

			return this; 
		}
		
		public Dropdownable SelectByName(string name)
		{
			base._SelectByName(name);

			return this;
		}
		
		public Dropdownable AddItem( Button button )
		{
			Items.Add(button);

			return this;
		}
		
		public Dropdownable SetMargin( int value, string side = "" ) 
		{
			_SetMargin(value, side);
			
			return this;
		}
		
		public Dropdownable SetPadding( int value, string side = "" ) 
		{
			_SetPadding(value, side);
			
			return this;
		}
		
		public Dropdownable SetPrefix( string text ) 
		{
			PrefixLabel = text;

			return this;
		}
			
		public Dropdownable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		public Dropdownable SetDefaultValue(string value) 
		{
			DefaultValue = value;
			
			return this;
		}
			
		public void AddToContainer( Node Container ) 
		{
			base._AddToContainer(Container, _MarginContainer);
		}
		
		public Container GetInnerContainer()
		{
			return ItemsInnerContainer;
		}
				
		private void Reset()
		{
			WorkingNode = null;
			panelContainer = null;
			panelInnerContainer = null;
			SelectedBlock = null;
			ItemsInnerContainer = null;
			_PanelPaddingContainer = null;
		}
		
		public void ExitTree()
		{
			Items = null;
		}
	}
}
#endif