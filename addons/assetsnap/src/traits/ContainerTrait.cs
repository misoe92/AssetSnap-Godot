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
namespace AssetSnap.Trait
{
	using System;
	using Godot;

	[Tool]
	public partial class ContainerTrait : Base
	{
		/*
		** Enums
		*/
		public enum ContainerLayout 
		{
			OneColumn,
			TwoColumns,
			ThreeColumns,
			FourColumns,
		};
		
		public enum ContainerOrientation 
		{
			Horizontal,
			Vertical,
		};
		
		/*
		** Protected
		*/
		protected ContainerLayout Layout = ContainerLayout.OneColumn;
		protected ContainerOrientation Orientation = ContainerOrientation.Vertical;
		protected ContainerOrientation InnerOrientation = ContainerOrientation.Horizontal;
		
		protected bool UsePaddingContainer = true;

		/*
		** Public Methods
		*/
		
		/*
		** Shows the current container
		**
		** @return void
		*/
		public void Show()
		{
			Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible = true;
		}
		
		/*
		** Hides the current container
		**
		** @return void
		*/
		public void Hide()
		{
			Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible = false;
		}
		
		/*
		** Selects an placed container in the
		** nodes array by index
		**
		** @param int index
		** @return Containerable
		*/
		public virtual ContainerTrait Select(int index, bool debug = false)
		{
			base._Select(index, debug);
			
			if( false == Dependencies.ContainsKey(TraitName + "_WorkingNode")) 
			{
				GD.PushError("Node was false @ ContainerTrait -> @", OwnerName, "::", TraitName, "::", TypeString);
				GD.PushError("KEYS::", Dependencies.Keys);
				return this;
			}
			
			Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.traitGlobal.GetDependencies(index, TypeString, OwnerName);
			Dependencies = dependencies;
			
			return this;
		}
		
		/*
		** Selects an placed container in the
		** nodes array by name
		**
		** @param string name
		** @return Containerable
		*/
		public virtual ContainerTrait SelectByName( string name ) 
		{
			foreach( Container container in Nodes ) 
			{
				if( container.Name == name ) 
				{
					break;
				}
			}

			return this;
		}
		
		/*
		** Sets the layout of the container
		**
		** @param ContainerLayout layout
		** @return Containerable
		*/
		public virtual ContainerTrait SetLayout( ContainerLayout layout ) 
		{
			Layout = layout;
			
			return this;
		}
		
		/*
		** Sets the visibility state of the
		** currently chosen container
		**
		** @param bool state
		** @return Containerable
		*/
		public virtual ContainerTrait SetVisible( bool state ) 
		{
			Visible = state;
			
			if(
				null != Dependencies &&
				false != Dependencies.ContainsKey( TraitName + "_MarginContainer" )
			)  
			{
				Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible = state;
			}

			return this;
		}
		
		/*
		** Toggles the visibility state of the
		** currently chosen container
		**
		** @return Containerable
		*/
		public virtual ContainerTrait ToggleVisible() 
		{
			if(
				null != Dependencies &&
				false != Dependencies.ContainsKey( TraitName + "_MarginContainer" )
			)  
			{
				Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible = !Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible;
			}

			return this;
		}
		
		/*
		** Sets the orientation of the container
		**
		** @param ContainerOrientation orientation
		** @return Containerable
		*/
		public virtual ContainerTrait SetOrientation(ContainerOrientation orientation) 
		{
			Orientation = orientation;
			return this;
		}
		
		/*
		** Sets the inner orientation of the container
		**
		** @param ContainerOrientation orientation
		** @return Containerable
		*/
		public virtual ContainerTrait SetInnerOrientation(ContainerOrientation orientation) 
		{
			InnerOrientation = orientation;
			return this;
		}
		
		
		/*
		** Sets margin values for 
		** the currently chosen container
		**
		** @param int value
		** @param string side
		** @return Containerable
		*/
		public virtual ContainerTrait SetMargin( int value, string side = "" ) 
		{
			_SetMargin(value, side);
			
			if( side == "" ) 
			{
				if( false != Dependencies.ContainsKey(TraitName + "_MarginContainer") ) 
				{
					foreach( (string marginSide, int marginValue ) in Margin ) 
					{
						Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().AddThemeConstantOverride("margin_" + marginSide, marginValue);
					}
				}
			}
			else 
			{
				if( false != Dependencies.ContainsKey(TraitName + "_MarginContainer") ) 
				{
					Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().AddThemeConstantOverride("margin_" + side, value);
				}
			}
			
			return this;
		}
		
		/*
		** Sets padding values for 
		** the currently chosen container
		**
		** @param int value
		** @param string side
		** @return Containerable
		*/
		public virtual ContainerTrait SetPadding( int value, string side = "" ) 
		{
			_SetPadding(value, side);
			
			if( side == "" ) 
			{
				if( false != Dependencies.ContainsKey(TraitName + "_PaddingContainer") ) 
				{
					foreach( (string marginSide, int marginValue ) in Margin ) 
					{
						Dependencies[TraitName + "_PaddingContainer"].As<MarginContainer>().AddThemeConstantOverride("margin_" + marginSide, marginValue);
					}
				}
			}
			else 
			{
				if( false != Dependencies.ContainsKey(TraitName + "_PaddingContainer") ) 
				{
					Dependencies[TraitName + "_PaddingContainer"].As<MarginContainer>().AddThemeConstantOverride("margin_" + side, value);
				}
			}
			
			return this;
		}
		
		/*
		** Getter Methods
		*/
		
		public virtual Node GetParentContainer()
		{
			if( false != Dependencies.ContainsKey(TraitName + "_MarginContainer") ) 
			{
				// Single placement
				return Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().GetParent();
			}

			return null;
		}
		
		/*
		** Returns the outer container
		** of the container layout
		**
		** @return Container
		*/
		public virtual Container GetOuterContainer()
		{
			if( false != Dependencies.ContainsKey(TraitName + "_InnerContainer") ) 
			{
				// Single placement
				return Dependencies[TraitName + "_InnerContainer"].As<Container>();
			}

			return null;
		}
		
		/*
		** Returns a inner container
		** depending on a specified index
		**
		** @param int(0) index
		** @return Container
		*/
		public virtual Container GetInnerContainer( int index )
		{
			if( null == Dependencies ) 
			{
				return null;
			}
			
			if(
				false != Dependencies.ContainsKey(TraitName + "_InnerContainer")  &&
				null != Dependencies[TraitName + "_InnerContainer"].As<Container>().GetChild( index )
			) 
			{
				// Single placement
				return Dependencies[TraitName + "_InnerContainer"].As<Container>().GetChild( index ) as Container;
			}
			else 
			{
				GD.Print("Not found @ Inner Container ", TraitName + "_InnerContainer", " ", Dependencies.Keys);
			}

			return null;
		}
		
		/*
		** Booleans
		*/
		
		/*
		** Checks if the container is visible
		**
		** @return bool
		*/
		public virtual bool IsVisible() 
		{
			if( false != Dependencies.ContainsKey(TraitName + "_MarginContainer") )  
			{
				return Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>().Visible == true;
			}

			return false;
		}
		
		/*
		** Protected
		*/
		public virtual ContainerTrait Instantiate()
		{
			int ColumnCount = (int)Layout + 1;
			string prefix = TraitName ;
			
			MarginContainer _MarginContainer = new()
			{
				Name = prefix + "-ContainerMargin",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = SizeFlagsVertical,
				Visible = Visible,
			};

			if( Size != Vector2.Zero ) 
			{
				_MarginContainer.Size = Size;	
			}
			
			if( CustomMinimumSize != Vector2.Zero ) 
			{
				_MarginContainer.CustomMinimumSize = CustomMinimumSize;	
			}
			
			MarginContainer _PaddingContainer = new()
			{
				Name = prefix + "-ContainerPadding",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = SizeFlagsVertical,
			};

			Container _InnerContainer;
			
			if( InnerOrientation == ContainerOrientation.Vertical ) 
			{
				_InnerContainer = new VBoxContainer()
				{
					Name = prefix + "-ContainerInner",
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
					SizeFlagsVertical = SizeFlagsVertical,
				};
			}
			else 
			{
				_InnerContainer = new HBoxContainer()
				{
					Name = prefix + "-ContainerInner",
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
					SizeFlagsVertical = SizeFlagsVertical,
				};
			}

			foreach( (string side, int value ) in Margin ) 
			{
				_MarginContainer.AddThemeConstantOverride("margin_" + side, value);
			}
			
			VBoxContainer _ContainerNode = new()
			{
				SizeFlagsHorizontal = SizeFlagsHorizontal,
				SizeFlagsVertical = SizeFlagsVertical,
				Name = prefix + "_Container",
			};
			
			// if( TypeString == "AssetSnap.Component.Containerable") 
			// {
			// 	GD.Print(SizeFlagsVertical, Name );
			// }

			if( UsePaddingContainer ) 
			{
				foreach( (string side, int value ) in Padding ) 
				{
					_PaddingContainer.AddThemeConstantOverride("margin_" + side, value);
				}
			}
			
			for( int i = 0; i < ColumnCount; i++ ) 
			{
				Container innerContainer = Orientation == ContainerOrientation.Horizontal ? new HBoxContainer() : new VBoxContainer();
				innerContainer.SizeFlagsHorizontal = SizeFlagsHorizontal;
				innerContainer.SizeFlagsVertical = SizeFlagsVertical;
				innerContainer.Name = prefix + "-inner-" + i;
				
				_InnerContainer.AddChild(innerContainer);
			}

			if( Dependencies.ContainsKey(prefix + "_InnerContainer") ) 
			{
				GD.Print(prefix + "_InnerContainer already exists" );
			}
			
			Dependencies.Add(prefix + "_InnerContainer", _InnerContainer);
			Dependencies.Add(prefix + "_PaddingContainer", _PaddingContainer);
			Dependencies.Add(prefix + "_Container", _ContainerNode);
			Dependencies.Add(prefix + "_MarginContainer", _MarginContainer);
			
			_PaddingContainer.AddChild(_InnerContainer);
			_ContainerNode.AddChild(_PaddingContainer);
			_MarginContainer.AddChild(_ContainerNode);

			return this;
		}
		
		/*
		** Resets the trait to
		** a cleared state
		**
		** @return void
		*/
		protected virtual void Reset()
		{
			Layout = ContainerLayout.OneColumn;
			Orientation = ContainerOrientation.Vertical;
			InnerOrientation = ContainerOrientation.Vertical;
			Size = Vector2.Zero;
			CustomMinimumSize = Vector2.Zero;
			Dependencies = new();
			TraitName = "";
		}
	}
}
#endif