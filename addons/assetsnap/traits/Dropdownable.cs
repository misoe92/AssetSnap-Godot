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
	/// <summary>
	/// A dropdownable component that extends ContainerTrait, allowing the creation of dropdown menus.
	/// </summary>
	[Tool]
	public partial class Dropdownable : ContainerTrait
	{
		public enum DropdownState
		{
			Hidden,
			Shown
		};

		[Export]
		public Godot.Collections.Array<Button> Items = new();

		
		public PanelContainer PanelContainer;
		public MarginContainer PanelPaddingContainer;
		public VBoxContainer PanelInnerContainer;
		public DropdownButton SelectedBlock;
		public VBoxContainer ItemsInnerContainer;

		private string _DefaultValue = "";
		private string _PrefixLabel = "";
		
		/// <summary>
		/// Constructor for the Dropdownable class.
		/// </summary>
		public Dropdownable()
		{
			Name = "Dropdownable";
			TypeString = GetType().ToString();
		}
		
		/// <summary>
		/// Adds the currently chosen dropdown to a specified container.
		/// </summary>
		/// <param name="Container">The container to which the dropdown will be added.</param>
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
		
		/// <summary>
		/// Instantiate an instance of the trait.
		/// </summary>
		/// <param name="i">An integer value.</param>
		/// <returns>Returns the instantiated Dropdownable instance.</returns>
		public Dropdownable Instantiate(int i)
		{
			UsePaddingContainer = false;
			base._Instantiate();
			base.Instantiate();

			PanelContainer = new()
			{
				Name = "DropdownPanel",
				ThemeTypeVariation = "DropdownPanel",
			};
			
			PanelPaddingContainer = new()
			{
				Name = "DropdownPanelPaddingContainer",
				Visible = false,
			};

			foreach( (string side, int value ) in Padding ) 
			{ 
				PanelPaddingContainer.AddThemeConstantOverride("margin_" + side, value);
			}
			
			// Setup the dropdown
			PanelInnerContainer = new()
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
			
			if( "" != _DefaultValue ) 
			{
				SelectedBlock.Text = _DefaultValue;
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
			PanelPaddingContainer.AddChild(ItemsInnerContainer);
			PanelInnerContainer.AddChild(SelectedBlock);
			PanelInnerContainer.AddChild(PanelPaddingContainer);
			PanelContainer.AddChild(PanelInnerContainer);
			base.GetInnerContainer(0).AddChild(PanelContainer);
			
			Dependencies[TraitName + "_WorkingNode"] = PanelContainer;
			Dependencies[TraitName + "_PanelPaddingContainer"] = PanelPaddingContainer;
			Dependencies[TraitName + "_PanelInnerContainer"] = PanelInnerContainer;
			Dependencies[TraitName + "_SelectedBlock"] = SelectedBlock;
			Dependencies[TraitName + "_ItemsInnerContainer"] = ItemsInnerContainer;

			// Add the node to the nodes array
			Plugin.Singleton.TraitGlobal.AddInstance(Iteration, PanelContainer, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.TraitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);
 
			// Clear the trait
			Reset();
			Iteration += 1;
			Dependencies = new();

			return this;
		}
		
		/// <summary>
		/// Selects a placed button in the nodes array by index.
		/// </summary>
		/// <param name="index">The index of the button to select.</param>
		/// <param name="debug">Optional parameter to enable debugging.</param>
		/// <returns>Returns the updated Dropdownable instance.</returns>
		public override Dropdownable Select(int index, bool debug = false)
		{
			base._Select(index);

			if( false != Dependencies.ContainsKey(TraitName + "_WorkingNode")) 
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.TraitGlobal.GetDependencies(index, TypeString, OwnerName);
				Dependencies = dependencies;
			}

			return this; 
		}
		
		/// <summary>
		/// Selects a placed button in the nodes array by name.
		/// </summary>
		/// <param name="name">The name of the button to select.</param>
		/// <returns>Returns the updated Dropdownable instance.</returns>
		public override Dropdownable SelectByName(string name)
		{
			base.SelectByName(name);

			return this;
		}
		
		/// <summary>
		/// Adds a button node to the dropdown.
		/// </summary>
		/// <param name="button">The button to add.</param>
		/// <returns>Returns the updated Dropdownable instance.</returns>
		public Dropdownable AddItem( Button button )
		{
			Items.Add(button);

			return this;
		}
		
		/// <summary>
		/// Sets a name for the current dropdown.
		/// </summary>
		/// <param name="text">The name to set.</param>
		/// <returns>Returns the updated Dropdownable instance.</returns>
		public Dropdownable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/// <summary>
		/// Sets a prefix for the current dropdown.
		/// </summary>
		/// <param name="text">The prefix to set.</param>
		/// <returns>Returns the updated Dropdownable instance.</returns>
		public Dropdownable SetPrefix( string text ) 
		{
			_PrefixLabel = text;

			return this;
		}
		
		/// <summary>
		/// Sets the default value for the dropdown.
		/// </summary>
		/// <param name="value">The default value to set.</param>
		/// <returns>Returns the updated Dropdownable instance.</returns>
		public Dropdownable SetDefaultValue(string value) 
		{
			_DefaultValue = value;
			
			return this;
		}
		
		/// <summary>
		/// Sets margin values for the currently chosen dropdown.
		/// </summary>
		/// <param name="value">The margin value.</param>
		/// <param name="side">The side for which to set the margin.</param>
		/// <returns>Returns the updated Dropdownable instance.</returns>
		public override Dropdownable SetMargin( int value, string side = "" ) 
		{
			base.SetMargin(value, side);
			
			return this;
		}
		
		/// <summary>
		/// Sets padding values for the currently chosen dropdown.
		/// </summary>
		/// <param name="value">The padding value.</param>
		/// <param name="side">The side for which to set the padding.</param>
		/// <returns>Returns the updated Dropdownable instance.</returns>
		public override Dropdownable SetPadding( int value, string side = "" ) 
		{
			base.SetPadding(value, side);
			
			return this;
		}
		
		/// <summary>
		/// Returns the inner container of the dropdown.
		/// </summary>
		/// <returns>Returns the inner container.</returns>
		public Container GetDropdownContainer()
		{
			if( null == Dependencies || false == Dependencies.ContainsKey(TraitName + "_ItemsInnerContainer") ) 
			{
				return null;
			}
			
			return Dependencies[TraitName + "_ItemsInnerContainer"].As<Container>();
		}
				
		/// <summary>
		/// Resets the trait to a cleared state.
		/// </summary>
		protected override void Reset()
		{
			PanelContainer = null;
			PanelInnerContainer = null;
			SelectedBlock = null;
			ItemsInnerContainer = null;
			PanelPaddingContainer = null;

			Dependencies = new();

			base.Reset();
		}
	}
}
#endif