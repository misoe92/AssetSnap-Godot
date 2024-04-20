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
using AssetSnap.Trait;
using Godot;
namespace AssetSnap.Component
{
	[Tool]
	public partial class Dropdownable : ContainerTrait
	{
		/*
		** Enums
		*/
		public enum DropdownState
		{
			Hidden,
			Shown
		};

		/*
		** Exports
		*/
		[Export]
		public Godot.Collections.Array<Button> Items = new();

		/*
		** Public
		*/
		public PanelContainer panelContainer;
		public MarginContainer _PanelPaddingContainer;
		public VBoxContainer panelInnerContainer;
		public DropdownButton SelectedBlock;
		public VBoxContainer ItemsInnerContainer;

		/*
		** Private
		*/
		private string DefaultValue = "";
		private string PrefixLabel = "";
		
		/*
		** Public methods
		*/
		public Dropdownable()
		{
			Name = "Dropdownable";
			TypeString = GetType().ToString();
		}
		
		/*
		** Instantiate an instance of the trait
		**
		** @return Dropdownable
		*/
		public Dropdownable Instantiate(int i)
		{
			UsePaddingContainer = false;
			base._Instantiate();
			base.Instantiate();

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
			
			// Setup the dropdown
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

			ItemsInnerContainer = new()
			{
				Name = "DropdownInnerContainer",
			};

			// Setup the layout
			_PanelPaddingContainer.AddChild(ItemsInnerContainer);
			panelInnerContainer.AddChild(SelectedBlock);
			panelInnerContainer.AddChild(_PanelPaddingContainer);
			panelContainer.AddChild(panelInnerContainer);
			base.GetInnerContainer(0).AddChild(panelContainer);
			
			Dependencies[TraitName + "_WorkingNode"] = panelContainer;
			Dependencies[TraitName + "_PanelPaddingContainer"] = _PanelPaddingContainer;
			Dependencies[TraitName + "_PanelInnerContainer"] = panelInnerContainer;
			Dependencies[TraitName + "_SelectedBlock"] = SelectedBlock;
			Dependencies[TraitName + "_ItemsInnerContainer"] = ItemsInnerContainer;

			// Add the node to the nodes array
			Plugin.Singleton.traitGlobal.AddInstance(Iteration, panelContainer, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.traitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);
 
			// Clear the trait
			Reset();
			Iteration += 1;
			Dependencies = new();

			return this;
		}
		
		/*
		** Selects an placed button in the
		** nodes array by index
		**
		** @param int index
		** @return Dropdownable
		*/
		public override Dropdownable Select(int index, bool debug = false)
		{
			base._Select(index);

			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode")) 
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.traitGlobal.GetDependencies(index, TypeString, OwnerName);
				Dependencies = dependencies;
			}

			return this; 
		}
		
		/*
		** Selects an placed button in the
		** nodes array by name
		**
		** @param string name
		** @return Dropdownable
		*/
		public override Dropdownable SelectByName(string name)
		{
			base.SelectByName(name);

			return this;
		}
		
		/*
		** Adds an button node to the dropdown
		**
		** @param Button button
		** @return Dropdownable
		*/
		public Dropdownable AddItem( Button button )
		{
			Items.Add(button);

			return this;
		}
		
		/*
		** Adds the currently chosen dropdown
		** to a specified container
		**
		** @param Node Container
		** @return void
		*/
		public void AddToContainer( Node Container ) 
		{
			if( null == Dependencies || false == Dependencies.ContainsKey(TraitName + "_MarginContainer") ) 
			{
				GD.PushError("Container was not found @ AddToContainer");
				
				if( null == Dependencies ) 
				{
					return;
				}
				
				GD.PushError("AddToContainer::Keys-> ", Dependencies.Keys);
				GD.PushError("AddToContainer::ADDTO-> ", TraitName + "_MarginContainer");
				return;
			}
			
			base._AddToContainer(Container, Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>(), 0);

			Reset();
		}
		
		/*
		** Setter Methods
		*/
		
		/*
		** Sets a name for the current dropdown
		**
		** @param string text
		** @return Dropdownable
		*/
		public Dropdownable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/*
		** Sets a prefix for the current dropdown
		**
		** @param string text
		** @return Dropdownable
		*/
		public Dropdownable SetPrefix( string text ) 
		{
			PrefixLabel = text;

			return this;
		}
		
		/*
		** Sets the default value for
		** the dropdown
		**
		** @param string value
		** @return Dropdownable
		*/
		public Dropdownable SetDefaultValue(string value) 
		{
			DefaultValue = value;
			
			return this;
		}
		
		/*
		** Sets margin values for 
		** the currently chosen dropdown
		**
		** @param int value
		** @param string side
		** @return Dropdownable
		*/
		public override Dropdownable SetMargin( int value, string side = "" ) 
		{
			base.SetMargin(value, side);
			
			return this;
		}
		
		/*
		** Sets padding values for 
		** the currently chosen dropdown
		**
		** @param int value
		** @param string side
		** @return Dropdownable
		*/
		public override Dropdownable SetPadding( int value, string side = "" ) 
		{
			base.SetPadding(value, side);
			
			return this;
		}
		
		/*
		** Getter Methods
		*/
		
		/*
		** Returns the inner container
		**
		** @return Container
		*/
		public Container GetDropdownContainer()
		{
			if( null == Dependencies || false == Dependencies.ContainsKey(TraitName + "_ItemsInnerContainer") ) 
			{
				return null;
			}
			
			return Dependencies[TraitName + "_ItemsInnerContainer"].As<Container>();
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
		protected override void Reset()
		{
			panelContainer = null;
			panelInnerContainer = null;
			SelectedBlock = null;
			ItemsInnerContainer = null;
			_PanelPaddingContainer = null;

			Dependencies = new();

			base.Reset();
		}
	}
}
#endif