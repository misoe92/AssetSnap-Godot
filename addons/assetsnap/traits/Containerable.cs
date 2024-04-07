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
		
		private Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		private Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		
		public ContainerLayout Layout = ContainerLayout.OneColumn;
		
		public ContainerOrientation Orientation = ContainerOrientation.Vertical;
		
		public ContainerOrientation InnerOrientation = ContainerOrientation.Horizontal;
		
		private MarginContainer _MarginContainer;
		
		private Container _InnerContainer;
		
		private MarginContainer _PaddingContainer;
		
		private Vector2 CustomMinimumSize = Vector2.Zero;
		private Vector2 Size = Vector2.Zero;
		
		public Containerable Instantiate()
		{
			try 
			{
				base._Instantiate( GetType().ToString() );
				int ColumnCount = (int)Layout + 1;
	
				// Margin Container 
				// VBox
				// Padding(Margin) Container
				// HBox
				// Inner HBox / VBox
				
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
		
		public void Show()
		{
			_MarginContainer.Visible = true;
		}
		
		public void Hide()
		{
			_MarginContainer.Visible = false;
		}
		
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
		
		public Containerable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		public Containerable SetLayout( ContainerLayout layout ) 
		{
			Layout = layout;
			
			return this;
		}
		
		public Containerable SetDimensions( int width, int height )
		{
			CustomMinimumSize = new Vector2( width, height);
			Size = new Vector2( width, height);

			return this;
		}
		
		public Containerable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;

			return this;
		}
		
		public Containerable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;

			return this;
		}
		
		public Containerable SetOrientation(ContainerOrientation orientation) 
		{
			Orientation = orientation;
			return this;
		}
		
		public Containerable SetInnerOrientation(ContainerOrientation orientation) 
		{
			InnerOrientation = orientation;
			return this;
		}
		
		public Containerable SetVisible( bool state ) 
		{
			Visible = state;
			
			if( EditorPlugin.IsInstanceValid(_MarginContainer))  
			{
				_MarginContainer.Visible = state;
			}

			return this;
		}
		
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
		
		public Containerable Select(int index)
		{
			base._Select(index);
			
			if( EditorPlugin.IsInstanceValid(WorkingNode) && EditorPlugin.IsInstanceValid(WorkingNode.GetParent()) ) 
			{
				_MarginContainer = WorkingNode.GetParent() as MarginContainer;
			}
			else 
			{
				GD.PushError("MarginContainer is invalid");
			}
			
			if( EditorPlugin.IsInstanceValid(WorkingNode) && EditorPlugin.IsInstanceValid(WorkingNode.GetChild(0)) ) 
			{
				_PaddingContainer = WorkingNode.GetChild(0) as MarginContainer;
			}
			else 
			{
				GD.PushError("PaddingContainer is invalid");
			}
			
			if( EditorPlugin.IsInstanceValid(_PaddingContainer) && EditorPlugin.IsInstanceValid(_PaddingContainer.GetChild(0)) ) 
			{
				_InnerContainer = _PaddingContainer.GetChild(0) as Container;
			}
			else 
			{
				GD.PushError("InnerContainer is invalid");
			}
			
			return this;
		}
		
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

		public void AddToContainer( Node Container, int? index = null ) 
		{
			base._AddToContainer(Container, _MarginContainer, index);
		}
		
		
		public override void _ExitTree()
		{
			WorkingNode.QueueFree();
			Reset();
		}
	}
}
#endif