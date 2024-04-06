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

namespace AssetSnap.Front.Nodes
{
	using System;
	using Godot;

	[Tool]
	public partial class AsContextMenu : Control
	{
		[Signal]
		public delegate void VectorsChangedEventHandler(Godot.Collections.Dictionary which);
		
		[Signal]
		public delegate void QuickActionsChangedEventHandler(string which);
		
		public bool Active = false;
		
		private float ScaleValueX = 1.0f;
		private float ScaleValueY = 1.0f;
		private float ScaleValueZ = 1.0f;
		private float RotValueX = 0.0f;
		private float RotValueY = 0.0f;
		private float RotValueZ = 0.0f;
		
		/** Responsive Params **/
		private int largeScreenOffsetX = 135;
		private int mediumScreenOffsetX = 140;
		private int smallScreenOffsetX = 145;

		private int screenOffsetY = 123;

		private string BelongsToSceneName = "";

		public override void _EnterTree()
		{
			Name = "AsContextMenu";
			
			base._EnterTree();
		}
		
		public override void _Ready() 
		{
			 // block unloading with a strong handle
			var handle = System.Runtime.InteropServices.GCHandle.Alloc(this); 
			
			Action ResizeAction = () => { _OnResize(); };
			
			EditorInterface.Singleton.GetBaseControl().Connect(Control.SignalName.Resized, Callable.From(ResizeAction));
			EditorInterface.Singleton.GetBaseControl().GetNode<Control>("@VBoxContainer@14/@HSplitContainer@17/@HSplitContainer@25/@VSplitContainer@27").Connect(Control.SignalName.Resized, Callable.From(ResizeAction));

			// register cleanup code to prevent unloading issues
			System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(System.Reflection.Assembly.GetExecutingAssembly()).Unloading += alc =>
			{
				GD.Print("Cleaning Context"); 
				if( null != EditorInterface.Singleton.GetBaseControl() && EditorInterface.Singleton.GetBaseControl().IsConnected(Control.SignalName.Resized, Callable.From(ResizeAction)))
				{
					EditorInterface.Singleton.GetBaseControl().Disconnect(Control.SignalName.Resized, Callable.From(ResizeAction));
				}
				if( null != EditorInterface.Singleton.GetBaseControl() && EditorInterface.Singleton.GetBaseControl().GetNode<Control>("@VBoxContainer@14/@HSplitContainer@17/@HSplitContainer@25/@VSplitContainer@27").IsConnected(Control.SignalName.Resized, Callable.From(ResizeAction)))
				{
					EditorInterface.Singleton.GetBaseControl().GetNode<Control>("@VBoxContainer@14/@HSplitContainer@17/@HSplitContainer@25/@VSplitContainer@27").Disconnect(Control.SignalName.Resized, Callable.From(ResizeAction));
				}

				handle.Free();
			};
		}
		
		public override void _Input(InputEvent @Event) 
		{
			if( @Event is InputEventMouseMotion motionEvent ) 
			{
				Vector2 Position = motionEvent.Position;
				if( Position.Y < 90 ) 
				{
					Visible = false;
				}
				else if( Active == true )
				{
					Visible = true;
				}
			}

			if( Input.IsKeyPressed(Key.Shift) && Input.IsKeyPressed(Key.Alt) ) 
			{
				GlobalExplorer.GetInstance().AllowScroll = Abstracts.AbstractExplorerBase.ScrollState.SCROLL_DISABLED;	
			}
			
			if( @Event is InputEventKey KeyEvent && HasNode("HBoxContainer/QuickAction/ModifiersList")) 
			{
				if ( Input.IsKeyPressed(Key.Shift) && Input.IsKeyPressed(Key.Alt) && KeyEvent.Keycode == Key.Q) 
				{
					AsSelectList List = GetNode<AsSelectList>("HBoxContainer/QuickAction/ModifiersList");
					Control _Control = List.GetNode<MarginContainer>("None");
					
					List.SetActive( _Control );
				}
				
				if ( Input.IsKeyPressed(Key.Shift) && Input.IsKeyPressed(Key.Alt) && KeyEvent.Keycode == Key.R) 
				{
					AsSelectList List = GetNode<AsSelectList>("HBoxContainer/QuickAction/ModifiersList");
					Control _Control = List.GetNode<MarginContainer>("Rotate");
					
					List.SetActive(_Control); 
				}
				
				if ( Input.IsKeyPressed(Key.Shift) && Input.IsKeyPressed(Key.Alt) && KeyEvent.Keycode == Key.S) 
				{
					AsSelectList List = GetNode<AsSelectList>("HBoxContainer/QuickAction/ModifiersList");
					Control _Control = List.GetNode<MarginContainer>("Scale");

					List.SetActive(_Control);
				}
			}
		}
		
		public void _OnQuickActionChange(string which)
		{
			EmitSignal(SignalName.QuickActionsChanged, new Variant[] { which });
		}
		
		public void SetRotationX( float Rot ) 
		{
			RotValueX = Rot;
			UpdateRotate();
		}
		public void SetRotationY( float Rot ) 
		{
			RotValueY = Rot;
			UpdateRotate();
		}
		public void SetRotationZ( float Rot ) 
		{
			RotValueZ = Rot;
			UpdateRotate();
		}
		public void SetScaleX( float value ) 
		{
			ScaleValueX = value;
			UpdateScale();
		}
		public void SetScaleY( float value ) 
		{
			ScaleValueY = value;
			UpdateScale();
		}
		public void SetScaleZ( float value ) 
		{
			ScaleValueZ = value;
			UpdateScale();
		}
		public float GetRotationX() 
		{
			return RotValueX;
		}
		public float GetRotationY() 
		{
			return RotValueY;
		}
		public float GetRotationZ() 
		{
			return RotValueZ;
		}
		public float GetScaleX() 
		{
			return ScaleValueX;
		}
		public float GetScaleY() 
		{
			return ScaleValueY;
		}
		public float GetScaleZ() 
		{
			return ScaleValueZ;
		}
		public int GetAngleIndex()
		{
			return GetAngles()._ActiveIndex;
		}
		public void _OnRotateXChanged( float value )
		{
			if( RotValueX == value ) 
			{
				return;
			}
			
			RotValueX = value;
		}
		public void _OnRotateYChanged( float value )
		{
			if( RotValueY == value ) 
			{
				return;
			}
			
			RotValueY = value;
		}
		public void _OnRotateZChanged( float value )
		{
			if( RotValueZ == value ) 
			{
				return;
			}
			
			RotValueZ = value;
		}
		public void _OnScaleXChanged( float value )
		{
			if( ScaleValueX == value ) 
			{
				return;
			}
			
			ScaleValueX = value;
		}
		public void _OnScaleYChanged( float value )
		{
			if( ScaleValueY == value ) 
			{
				return;
			}
			
			ScaleValueY = value;
		}
		
		public void _OnScaleZChanged( float value )
		{
			if( ScaleValueZ == value ) 
			{
				return;
			}
			
			ScaleValueZ = value;
		}
		
		private void _OnResize()
		{
			Vector2 WindowSize = EditorInterface.Singleton.GetBaseControl().Size;
			Control DockOne = EditorInterface.Singleton.GetBaseControl().GetNode<Control>("@VBoxContainer@14/@HSplitContainer@17/@HSplitContainer@25/@VSplitContainer@27");
			Vector2 CurrentPosition = Position;
			
			if( WindowSize.X < 1300.0f ) 
			{
				CurrentPosition.X = DockOne.Size.X + smallScreenOffsetX;
			}
			else if( WindowSize.X < 1600.0f ) 
			{
				CurrentPosition.X = DockOne.Size.X + mediumScreenOffsetX;
			}
			else 
			{
				CurrentPosition.X = DockOne.Size.X + largeScreenOffsetX;
			}
			
			CurrentPosition.Y = screenOffsetY;
			Position = CurrentPosition;
		}

		private void UpdateScale()
		{
			ScaleNodeX().Value = GetScaleX();
			ScaleNodeY().Value = GetScaleY();
			ScaleNodeZ().Value = GetScaleZ();
		}
		private void UpdateRotate()
		{
			RotationNodeX().Value = GetRotationX();
			RotationNodeY().Value = GetRotationY();
			RotationNodeZ().Value = GetRotationZ();
		}
		public AsSelectList GetQuickActions()
		{
			return GetNode<AsSelectList>("HBoxContainer/QuickAction/ModifiersList");
		}
		public AsSelectList GetAngles()
		{
			return GetNode<AsSelectList>("HBoxContainer/Angle/ModifiersList");
		}
		
		private SpinBox RotationNodeX()
		{
			return GetNode<SpinBox>("HBoxContainer/RotateValues/RotateAngleX/SpinBox");
		}
		private SpinBox RotationNodeY()
		{
			return GetNode<SpinBox>("HBoxContainer/RotateValues/RotateAngleY/SpinBox");
		}
		private SpinBox RotationNodeZ()
		{
			return GetNode<SpinBox>("HBoxContainer/RotateValues/RotateAngleZ/SpinBox");
		}
		
		private SpinBox ScaleNodeX()
		{
			return GetNode<SpinBox>("HBoxContainer/ScaleValues/ScaleAngleX/SpinBox");
		}
		private SpinBox ScaleNodeY()
		{
			return GetNode<SpinBox>("HBoxContainer/ScaleValues/ScaleAngleY/SpinBox");
		}
		private SpinBox ScaleNodeZ()
		{
			return GetNode<SpinBox>("HBoxContainer/ScaleValues/ScaleAngleZ/SpinBox");
		}
		
	

		public override void _ExitTree()
		{
			base._ExitTree();
		}
	}
}
