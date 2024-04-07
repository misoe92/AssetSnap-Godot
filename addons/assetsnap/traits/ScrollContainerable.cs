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
	public partial class ScrollContainerable : Trait.Base
	{
		/*
		** Enums
		*/
		public enum ContainerOrientation 
		{
			Horizontal,
			Vertical,
		};
		
		/*
		** Public
		*/
		private HBoxContainer _InnerContainer;
		private MarginContainer _PaddingContainer;
		
		/*
		** Private
		*/
		private Control.SizeFlags SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		private Control.SizeFlags SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
		public ContainerOrientation Orientation = ContainerOrientation.Vertical;
		
		/*
		** Public methods
		*/
		
		/*
		** Instantiate an instance of the trait
		**
		** @return ScrollContainerable
		*/	
		public ScrollContainerable Instantiate()
		{
			try 
			{
				base._Instantiate( GetType().ToString() );
				
				ScrollContainer _WorkingNode = new()
				{
					Name="Scroll",
					SizeFlagsVertical = Control.SizeFlags.ExpandFill,
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				};
				
				_PaddingContainer = new()
				{
					Name="ScrollPadding",
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
					SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				};

				_InnerContainer = new()
				{
					Name="ScrollInner",
					SizeFlagsVertical = Control.SizeFlags.ExpandFill,
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				};
				
				foreach( (string side, int value ) in Padding ) 
				{
					_PaddingContainer.AddThemeConstantOverride("margin_" + side, value);
				}

				_PaddingContainer.AddChild(_InnerContainer);
				_WorkingNode.AddChild(_PaddingContainer);

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
			if( WorkingNode is Control ControlNode ) 
			{
				ControlNode.Visible = true;
			}
		}
		
		public void Hide()
		{
			if( WorkingNode is Control ControlNode ) 
			{
				ControlNode.Visible = false;
			}
		}
		
		/*
		** Selects an placed scroll container
		** in the nodes array by index
		**
		** @param int index
		** @return ScrollContainerable
		*/
		public ScrollContainerable Select(int index)
		{
			base._Select(index);
			
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
				_InnerContainer = _PaddingContainer.GetChild(0) as HBoxContainer;
			}
			else 
			{
				GD.PushError("InnerContainer is invalid");
			}
			
			return this;
		}
		
		/*
		** Selects an placed scroll container
		** in the nodes array by name
		**
		** @param string name
		** @return ScrollContainerable
		*/
		public ScrollContainerable SelectByName( string name ) 
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
		** Adds the currently chosen scroll
		** container to a specified container
		**
		** @param Node Container
		** @return void
		*/
		public void AddToContainer( Node Container ) 
		{
			base._AddToContainer(Container, WorkingNode);
		}
		
		/*
		** Setter Methods
		*/
		
		/*
		** Sets the name of the current scroll container
		**
		** @param string text
		** @return ScrollContainerable
		*/
		public ScrollContainerable SetName( string text ) 
		{
			base._SetName(text);
			
			return this;
		}
		
		/*
		** Sets the visibility state of the
		** currently chosen scroll container
		**
		** @param bool state
		** @return ScrollContainerable
		*/
		public ScrollContainerable SetVisible( bool state ) 
		{
			if( EditorPlugin.IsInstanceValid(WorkingNode) && WorkingNode is Control controlNode)  
			{
				controlNode.Visible = state;
			}
			else 
			{
				GD.PushError("MarginContainer is invalid");
			}

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the x
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return ScrollContainerable
		*/
		public ScrollContainerable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsHorizontal = flag;

			return this;
		}
		
		/*
		** Sets the horizontal size flag, which controls the y
		** axis, and how it should act.
		**
		** @param Control.SizeFlags flag
		** @return ScrollContainerable
		*/
		public ScrollContainerable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			SizeFlagsVertical = flag;

			return this;
		}
		
		/*
		** Sets the orientation of the scroll container
		**
		** @param ContainerOrientation orientation
		** @return ScrollContainerable
		*/
		public ScrollContainerable SetOrientation(ContainerOrientation orientation) 
		{
			Orientation = orientation;
			return this;
		}
		
		/*
		** Sets margin values for 
		** the currently chosen scroll container
		**
		** @param int value
		** @param string side
		** @return ScrollContainerable
		*/
		public ScrollContainerable SetMargin( int value, string side = "" ) 
		{
			_SetMargin(value, side);
			
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
		public Container GetInnerContainer()
		{
			if( null != WorkingNode ) 
			{
				// Single placement
				return _InnerContainer as Container;
			}

			return null;
		}
		
		/*
		** Private Methods
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
			_PaddingContainer = null;
			Orientation = ContainerOrientation.Vertical;
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
		}
	}
}
#endif