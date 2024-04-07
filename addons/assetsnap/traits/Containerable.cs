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
	public partial class Containerable : Trait.Base
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
		
		/*
		** Private
		*/
		private Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		private Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		private ContainerLayout Layout = ContainerLayout.OneColumn;
		private ContainerOrientation Orientation = ContainerOrientation.Vertical;
		private ContainerOrientation InnerOrientation = ContainerOrientation.Horizontal;
		private Vector2 CustomMinimumSize = Vector2.Zero;
		private Vector2 Size = Vector2.Zero;
		
		/*
		** Public methods
		*/
		
		/*
		** Instantiate an instance of the trait
		**
		** @return Containerable
		*/	
		public Containerable Instantiate()
		{
			try 
			{
				base._Instantiate( GetType().ToString() );
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
				
				VBoxContainer _WorkingNode = new()
				{
					Name = Name,
				};

				foreach( (string side, int value ) in Padding ) 
				{
					_PaddingContainer.AddThemeConstantOverride("margin_" + side, value);
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
				_WorkingNode.AddChild(_PaddingContainer);
				_MarginContainer.AddChild(_WorkingNode);			

				Nodes.Add(_WorkingNode);
				WorkingNode = _WorkingNode;

				Reset();
			}
			catch(Exception e) 
			{
				GD.PushError(e.Message);
			}
			
			return this;
		}
		
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
		public Containerable Select(int index)
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
		public Containerable SelectByName( string name ) 
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
		** Adds the currently chosen container
		** to a specified container
		**
		** @param Node Container
		** @param int|null index
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
		** Sets the name of the current container
		**
		** @param string text
		** @return Containerable
		*/
		public Containerable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/*
		** Sets the layout of the container
		**
		** @param ContainerLayout layout
		** @return Containerable
		*/
		public Containerable SetLayout( ContainerLayout layout ) 
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
		public Containerable SetVisible( bool state ) 
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
		public Containerable ToggleVisible() 
		{
			if( EditorPlugin.IsInstanceValid(_MarginContainer))  
			{
				_MarginContainer.Visible = !_MarginContainer.Visible;
			}
			else 
			{
				GD.PushError("MarginContainer is invalid");
			}

			return this;
		}
		
		/*
		** Sets the size of the container
		**
		** @param int width
		** @param int height
		** @return Containerable
		*/
		public Containerable SetDimensions( int width, int height )
		{
			CustomMinimumSize = new Vector2( width, height);
			Size = new Vector2( width, height);

			return this;
		}
		
		/*
		** Sets the orientation of the container
		**
		** @param ContainerOrientation orientation
		** @return Containerable
		*/
		public Containerable SetOrientation(ContainerOrientation orientation) 
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
		public Containerable SetInnerOrientation(ContainerOrientation orientation) 
		{
			InnerOrientation = orientation;
			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Containerable
		*/
		public Containerable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return Containerable
		*/
		public Containerable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;

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
		public Containerable SetMargin( int value, string side = "" ) 
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
		public Containerable SetPadding( int value, string side = "" ) 
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
		public Container GetOuterContainer()
		{
			if( null != WorkingNode && null != _InnerContainer) 
			{
				// Single placement
				return _InnerContainer;
			}
			else 
			{
				GD.PushWarning("Invalid outer container");
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
		public Container GetInnerContainer( int index = 0)
		{
			if( null != WorkingNode && null != _InnerContainer.GetChild( index )) 
			{
				// Single placement
				return _InnerContainer.GetChild( index ) as Container;
			}
			else 
			{
				GD.PushWarning("Invalid inner container");
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
		public bool IsVisible() 
		{
			if( EditorPlugin.IsInstanceValid(_MarginContainer))  
			{
				return _MarginContainer.Visible == true;
			}
			else 
			{
				GD.PushError("MarginContainer is invalid");
			}

			return false;
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