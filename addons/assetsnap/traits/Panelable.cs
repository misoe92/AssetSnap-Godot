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
	public partial class Panelable : Trait.Base
	{
		/*
		** Enums
		*/
		public enum PanelType
		{
			DefaultPanelContainer,
			RoundedPanelContainer,
			LightPanelContainer
		}
		
		/*
		** Private
		*/
		private new Godot.Collections.Dictionary<string, int> Margin = new()
		{
			{"left", 0},
			{"right", 0},
			{"top", 5},
			{"bottom", 5},
		};
		private new Godot.Collections.Dictionary<string, int> Padding = new()
		{
			{"left", 10},
			{"right", 10},
			{"top", 5},
			{"bottom", 5},
		};
		private PanelType Type = PanelType.DefaultPanelContainer;
		private MarginContainer _MarginContainer;
		private MarginContainer _PaddingContainer;

		/*
		** Public methods
		*/
		public Panelable()
		{
			TypeString = GetType().ToString();
		}
		
		/*
		** Instantiate an instance of the trait
		**
		** @return Panelable
		*/	
		public Panelable Instantiate()
		{
			base._Instantiate();

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
				Name = TraitName,
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

			Dependencies[TraitName + "_WorkingNode"] = WorkingPanel;
			Dependencies[TraitName + "_PanelPaddingContainer"] = _PaddingContainer;
			Dependencies[TraitName + "_MarginContainer"] = _MarginContainer;

			Plugin.Singleton.traitGlobal.AddInstance(Iteration, WorkingPanel, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.traitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);

			// GD.Print(Iteration);
		
			Reset();
			Iteration += 1;
			Dependencies = new();
			
			return this;
		}
		
		/*
		** Selects an placed panel container
		** in the nodes array by index
		**
		** @param int index
		** @return Panelable
		*/
		public Panelable Select(int index)
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
		** Selects an placed panel container
		** in the nodes array by name
		**
		** @param string name
		** @return Panelable
		*/
		public Panelable SelectByName( string name ) 
		{
			foreach( Button button in Nodes ) 
			{
				if( button.Name == name ) 
				{
					Dependencies["WorkingNode"] = button;
					break;
				}
			}

			return this;
		}
		
		/*
		** Adds the currently chosen panel
		** container to a specified container
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
		** Sets the name of the current panel container
		**
		** @param string text
		** @return Panelable
		*/
		public Panelable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/*
		** Sets the theme type of the panel continer,
		** which lays out a set of specified rules
		** from the theme that the panel container follows
		**
		** @param PanelType type
		** @return Panelable
		*/
		public Panelable SetType( PanelType type ) 
		{
			Type = type;
			return this;
		}
		
		/*
		** Sets the visible state
		** of the current panel container
		**
		** @param bool state
		** @return Panelable
		*/
		public Panelable SetVisible( bool state ) 
		{
			base._SetVisible(state);
			
			if(
				null != Dependencies &&
				false != Dependencies.ContainsKey(TraitName + "WorkingNode") &&
				Dependencies["WorkingNode"].As<GodotObject>() is PanelContainer panel
			) 
			{
				Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible = state;
			}
			
			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Panelable
		*/
		public override Panelable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Panelable
		*/
		public override Panelable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}
		
		/*
		** Sets margin values for the 
		** currently chosen panel container
		**
		** @param int value
		** @param string side
		** @return Panelable
		*/
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
		
		/*
		** Sets padding values for the 
		** currently chosen panel container
		**
		** @param int value
		** @param string side
		** @return Panelable
		*/
		public Panelable SetPadding( int value, string side ) 
		{
			base._SetPadding(value, side);
			
			return this;
		}
		
		/*
		** Getter Methods
		*/
		
		public MarginContainer GetContainer()
		{
			if( false == Dependencies.ContainsKey(TraitName + "_PaddingContainer") ) 
			{
				return null;
			}
			
			return Dependencies[TraitName + "_PaddingContainer"].As<MarginContainer>();
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
			_MarginContainer = null;
			_PaddingContainer = null;
			Type = PanelType.DefaultPanelContainer;
		}
	}
}
#endif