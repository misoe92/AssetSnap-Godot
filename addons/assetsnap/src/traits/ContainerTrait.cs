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
		** Public
		*/
		public MarginContainer _MarginContainer;
		public Container _InnerContainer;
		public MarginContainer _PaddingContainer;
		public VBoxContainer _ContainerNode;
		
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
			_MarginContainer.Visible = true;
		}
		
		/*
		** Hides the current container
		**
		** @return void
		*/
		public void Hide()
		{
			_MarginContainer.Visible = false;
		}
		
		/*
		** Selects an placed container in the
		** nodes array by index
		**
		** @param int index
		** @return Containerable
		*/
		public virtual ContainerTrait Select(int index)
		{
			base._Select(index);
			
			if( EditorPlugin.IsInstanceValid(WorkingNode) && EditorPlugin.IsInstanceValid(WorkingNode.GetParent()) ) 
			{
				_MarginContainer = WorkingNode.GetParent() as MarginContainer;
			}
			
			if( EditorPlugin.IsInstanceValid(WorkingNode) && EditorPlugin.IsInstanceValid(WorkingNode.GetChild(0)) ) 
			{
				_PaddingContainer = WorkingNode.GetChild(0) as MarginContainer;
			}
			
			if( EditorPlugin.IsInstanceValid(_PaddingContainer) && EditorPlugin.IsInstanceValid(_PaddingContainer.GetChild(0)) ) 
			{
				_InnerContainer = _PaddingContainer.GetChild(0) as Container;
			}
			
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
					WorkingNode = container;
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
			
			if( EditorPlugin.IsInstanceValid(_MarginContainer))  
			{
				_MarginContainer.Visible = state;
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
			if( EditorPlugin.IsInstanceValid(_MarginContainer))  
			{
				_MarginContainer.Visible = !_MarginContainer.Visible;
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
				if( null != WorkingNode ) 
				{
					_MarginContainer.AddThemeConstantOverride("margin_" + side, value);
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
				if( null != WorkingNode ) 
				{
					foreach( (string marginSide, int marginValue ) in Margin ) 
					{
						_PaddingContainer.AddThemeConstantOverride("margin_" + marginSide, marginValue);
					}
				}
			}
			else 
			{
				if( null != WorkingNode ) 
				{
					_PaddingContainer.AddThemeConstantOverride("margin_" + side, value);
				}
			}
			
			return this;
		}
		
		/*
		** Getter Methods
		*/
		
		/*
		** Returns the outer container
		** of the container layout
		**
		** @return Container
		*/
		public virtual Container GetOuterContainer()
		{
			if( null != WorkingNode && null != _InnerContainer) 
			{
				// Single placement
				return _InnerContainer;
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
			if(
				null != _InnerContainer &&
				null != _InnerContainer.GetChild( index )
			) 
			{
				// Single placement
				return _InnerContainer.GetChild( index ) as Container;
			}
			else 
			{
				GD.Print("Not found @ Inner Container");
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
			if( EditorPlugin.IsInstanceValid(_MarginContainer))  
			{
				return _MarginContainer.Visible == true;
			}

			return false;
		}
		
		/*
		** Protected
		*/
		public virtual ContainerTrait Instantiate()
		{
			int ColumnCount = (int)Layout + 1;
			_MarginContainer = new()
			{
				Name = "ContainerMargin",
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
			
			_PaddingContainer = new()
			{
				Name = "ContainerPadding",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = SizeFlagsVertical,
			};
			
			if( InnerOrientation == ContainerOrientation.Vertical ) 
			{
				_InnerContainer = new VBoxContainer()
				{
					Name = "ContainerInner",
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
					SizeFlagsVertical = SizeFlagsVertical,
				};
			}
			else 
			{
				_InnerContainer = new HBoxContainer()
				{
					Name = "ContainerInner",
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
					SizeFlagsVertical = SizeFlagsVertical,
				};
			}

			foreach( (string side, int value ) in Margin ) 
			{
				_MarginContainer.AddThemeConstantOverride("margin_" + side, value);
			}
			
			_ContainerNode = new()
			{
				Name = Name,
			};

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
				innerContainer.Name = "inner-" + i;
				
				_InnerContainer.AddChild(innerContainer);
			}
			
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
			WorkingNode = null;
			_InnerContainer = null;
			_MarginContainer = null;
			_PaddingContainer = null;
			Layout = ContainerLayout.OneColumn;
			Orientation = ContainerOrientation.Vertical;
			InnerOrientation = ContainerOrientation.Vertical;
			Size = Vector2.Zero;
			CustomMinimumSize = Vector2.Zero;
		}
		
		/*
		** Cleanup
		*/
		public override void _ExitTree()
		{
			if( null != _InnerContainer && EditorPlugin.IsInstanceValid( _InnerContainer ) ) 
			{
				_InnerContainer.QueueFree();
			}
			
			if( null != _PaddingContainer && EditorPlugin.IsInstanceValid( _PaddingContainer ) ) 
			{
				_PaddingContainer.QueueFree();
			}
			 
			if( null != WorkingNode && EditorPlugin.IsInstanceValid( WorkingNode ) ) 
			{ 
				WorkingNode.QueueFree();
			}
			
			if( null != _MarginContainer && EditorPlugin.IsInstanceValid( _MarginContainer ) ) 
			{
				_MarginContainer.QueueFree();
			}

			Reset();
		}
	}
}
#endif