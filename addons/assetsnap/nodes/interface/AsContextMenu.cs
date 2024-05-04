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

using System;
using AssetSnap.Explorer;
using Godot;

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// Represents a context menu control for handling quick actions, rotations, and scaling.
	/// </summary>
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

		/// <summary>
		/// Called when the node enters the scene tree.
		/// </summary>
		public override void _EnterTree()
		{
			Name = "AsContextMenu";
			
			base._EnterTree();
		}
		
		/// <summary>
		/// Called when the node is ready.
		/// </summary>
		public override void _Ready() 
		{
			Action ResizeAction = () => { _OnResize(); };
			
			EditorInterface.Singleton.GetBaseControl().Connect(Control.SignalName.Resized, Callable.From(ResizeAction));
			EditorInterface.Singleton.GetFileSystemDock().GetParent().Connect(Control.SignalName.Resized, Callable.From(ResizeAction));

			ExplorerUtils.Get().ContextMenu.GetInstance().Connect(
				SignalName.VectorsChanged,
				new Callable(this, "_OnUpdateVectors")
			);
		}
		
		/// <summary>
		/// Called when the node receives input events.
		/// </summary>
		/// <param name="event">The input event received by the node.</param>
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
			
			if( @Event is InputEventKey KeyEvent && HasNode("HBoxContainer/QuickAction/SelectList")) 
			{
				if ( Input.IsKeyPressed(Key.Shift) && Input.IsKeyPressed(Key.Alt) && KeyEvent.Keycode == Key.Q) 
				{
					AsSelectList List = GetNode<AsSelectList>("HBoxContainer/QuickAction/SelectList");
					Control _Control = List.GetNode<MarginContainer>("None");
					
					List.SetActive( _Control );
				}
				
				if ( Input.IsKeyPressed(Key.Shift) && Input.IsKeyPressed(Key.Alt) && KeyEvent.Keycode == Key.R) 
				{
					AsSelectList List = GetNode<AsSelectList>("HBoxContainer/QuickAction/SelectList");
					Control _Control = List.GetNode<MarginContainer>("Rotate");
					
					List.SetActive(_Control); 
				}
				
				if ( Input.IsKeyPressed(Key.Shift) && Input.IsKeyPressed(Key.Alt) && KeyEvent.Keycode == Key.S) 
				{
					AsSelectList List = GetNode<AsSelectList>("HBoxContainer/QuickAction/SelectList");
					Control _Control = List.GetNode<MarginContainer>("Scale");

					List.SetActive(_Control);
				}
			}
		}
		
		/// <summary>
		/// Emits the QuickActionsChanged signal with the specified action.
		/// </summary>
		/// <param name="which">The action to be emitted.</param>
		public void _OnQuickActionChange(string which)
		{
			EmitSignal(SignalName.QuickActionsChanged, new Variant[] { which });
		}
		
		/// <summary>
		/// Sets the rotation value around the X-axis.
		/// </summary>
		/// <param name="Rot">The rotation value to set.</param>
		public void SetRotationX( float Rot ) 
		{
			RotValueX = Rot;
			UpdateRotate();
		}
		
		/// <summary>
		/// Sets the rotation value around the Y-axis.
		/// </summary>
		/// <param name="Rot">The rotation value to set.</param>
		public void SetRotationY( float Rot ) 
		{
			RotValueY = Rot;
			UpdateRotate();
		}
		
		/// <summary>
		/// Sets the rotation value around the Z-axis.
		/// </summary>
		/// <param name="Rot">The rotation value to set.</param>
		public void SetRotationZ( float Rot ) 
		{
			RotValueZ = Rot;
			UpdateRotate();
		}
		
		/// <summary>
		/// Sets the scaling value along the X-axis.
		/// </summary>
		/// <param name="value">The scaling value to set.</param>
		public void SetScaleX( float value ) 
		{
			ScaleValueX = value;
			UpdateScale();
		}
		
		/// <summary>
		/// Sets the scaling value along the Y-axis.
		/// </summary>
		/// <param name="value">The scaling value to set.</param>
		public void SetScaleY( float value ) 
		{
			ScaleValueY = value;
			UpdateScale();
		}
		
		/// <summary>
		/// Sets the scaling value along the Z-axis.
		/// </summary>
		/// <param name="value">The scaling value to set.</param>
		public void SetScaleZ( float value ) 
		{
			ScaleValueZ = value;
			UpdateScale();
		}
		
		/// <summary>
		/// Retrieves the rotation value around the X-axis.
		/// </summary>
		/// <returns>The rotation value around the X-axis.</returns>
		public float GetRotationX() 
		{
			return RotValueX;
		}
		
		/// <summary>
		/// Retrieves the rotation value around the Y-axis.
		/// </summary>
		/// <returns>The rotation value around the Y-axis.</returns>
		public float GetRotationY() 
		{
			return RotValueY;
		}
		
		/// <summary>
		/// Retrieves the rotation value around the Z-axis.
		/// </summary>
		/// <returns>The rotation value around the Z-axis.</returns>
		public float GetRotationZ() 
		{
			return RotValueZ;
		}
		
		/// <summary>
		/// Retrieves the scaling value along the X-axis.
		/// </summary>
		/// <returns>The scaling value along the X-axis.</returns>
		public float GetScaleX() 
		{
			return ScaleValueX;
		}
		
		/// <summary>
		/// Retrieves the scaling value along the Y-axis.
		/// </summary>
		/// <returns>The scaling value along the Y-axis.</returns>
		public float GetScaleY() 
		{
			return ScaleValueY;
		}
		
		/// <summary>
		/// Retrieves the scaling value along the Z-axis.
		/// </summary>
		/// <returns>The scaling value along the Z-axis.</returns>
		public float GetScaleZ() 
		{
			return ScaleValueZ;
		}
		
		/// <summary>
		/// Retrieves the index of the angle.
		/// </summary>
		/// <returns>The index of the angle.</returns>
		public int GetAngleIndex()
		{
			return GetAngles()._ActiveIndex;
		}
		
		/// <summary>
		/// Called when the X-axis rotation value is changed.
		/// </summary>
		/// <param name="value">The new X-axis rotation value.</param>
		public void _OnRotateXChanged( float value )
		{
			if( RotValueX == value ) 
			{
				return;
			}
			
			RotValueX = value;
		}
		
		/// <summary>
		/// Called when the Y-axis rotation value is changed.
		/// </summary>
		/// <param name="value">The new Y-axis rotation value.</param>
		public void _OnRotateYChanged( float value )
		{
			if( RotValueY == value ) 
			{
				return;
			}
			
			RotValueY = value;
		}
		
		/// <summary>
		/// Called when the node receives a change in rotation around the X-axis.
		/// </summary>
		/// <param name="value">The new rotation value around the X-axis.</param>
		public void _OnRotateZChanged( float value )
		{
			if( RotValueZ == value ) 
			{
				return;
			}
			
			RotValueZ = value;
		}
		
		/// <summary>
		/// Called when the X-axis scaling value is changed.
		/// </summary>
		/// <param name="value">The new X-axis scaling value.</param>
		public void _OnScaleXChanged( float value )
		{
			if( ScaleValueX == value ) 
			{
				return;
			}
			
			ScaleValueX = value;
		}
		
		/// <summary>
		/// Called when the Y-axis scaling value is changed.
		/// </summary>
		/// <param name="value">The new Y-axis scaling value.</param>
		public void _OnScaleYChanged( float value )
		{
			if( ScaleValueY == value ) 
			{
				return;
			}
			
			ScaleValueY = value;
		}
		
		/// <summary>
		/// Called when the Z-axis scaling value is changed.
		/// </summary>
		/// <param name="value">The new Z-axis scaling value.</param>
		public void _OnScaleZChanged( float value )
		{
			if( ScaleValueZ == value ) 
			{
				return;
			}
			
			ScaleValueZ = value;
		}
		
		/// <summary>
		/// Adjusts the position of the context menu based on the current window size.
		/// </summary>
		private void _OnResize()
		{
			Vector2 WindowSize = EditorInterface.Singleton.GetBaseControl().Size;
			Control DockOne = EditorInterface.Singleton.GetFileSystemDock().GetParent<Control>();
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

		/// <summary>
		/// Updates the scale values displayed in the UI.
		/// </summary>
		private void UpdateScale()
		{
			ScaleNodeX().Value = GetScaleX();
			ScaleNodeY().Value = GetScaleY();
			ScaleNodeZ().Value = GetScaleZ();
		}
		
		/// <summary>
		/// Updates the rotation values displayed in the UI.
		/// </summary>
		private void UpdateRotate()
		{
			RotationNodeX().Value = GetRotationX();
			RotationNodeY().Value = GetRotationY();
			RotationNodeZ().Value = GetRotationZ();
		}
		
		/// <summary>
		/// Retrieves the list of quick actions.
		/// </summary>
		/// <returns>The list of quick actions.</returns>
		public AsSelectList GetQuickActions()
		{
			return GetNode<AsSelectList>("HBoxContainer/QuickAction/SelectList");
		}
		
		/// <summary>
		/// Retrieves the list of angle options.
		/// </summary>
		/// <returns>The list of angle options.</returns>
		public AsSelectList GetAngles()
		{
			return GetNode<AsSelectList>("HBoxContainer/Angle/SelectList");
		}
		
		/// <summary>
		/// Retrieves the SpinBox node for X-axis rotation.
		/// </summary>
		/// <returns>The SpinBox node for X-axis rotation.</returns>
		private SpinBox RotationNodeX()
		{
			return GetNode<SpinBox>("HBoxContainer/RotateValues/RotateAngleX/SpinBox");
		}
		
		/// <summary>
		/// Retrieves the SpinBox node for Y-axis rotation.
		/// </summary>
		/// <returns>The SpinBox node for Y-axis rotation.</returns>
		private SpinBox RotationNodeY()
		{
			return GetNode<SpinBox>("HBoxContainer/RotateValues/RotateAngleY/SpinBox");
		}
		
		/// <summary>
		/// Retrieves the SpinBox node for Z-axis rotation.
		/// </summary>
		/// <returns>The SpinBox node for Z-axis rotation.</returns>
		private SpinBox RotationNodeZ()
		{
			return GetNode<SpinBox>("HBoxContainer/RotateValues/RotateAngleZ/SpinBox");
		}
		
		/// <summary>
		/// Retrieves the SpinBox node for X-axis scaling.
		/// </summary>
		/// <returns>The SpinBox node for X-axis scaling.</returns>
		private SpinBox ScaleNodeX()
		{
			return GetNode<SpinBox>("HBoxContainer/ScaleValues/ScaleAngleX/SpinBox");
		}
		
		/// <summary>
		/// Retrieves the SpinBox node for Y-axis scaling.
		/// </summary>
		/// <returns>The SpinBox node for Y-axis scaling.</returns>
		private SpinBox ScaleNodeY()
		{
			return GetNode<SpinBox>("HBoxContainer/ScaleValues/ScaleAngleY/SpinBox");
		}
		
		/// <summary>
        /// Retrieves the SpinBox node for Z-axis scaling.
        /// </summary>
        /// <returns>The SpinBox node for Z-axis scaling.</returns>
		private SpinBox ScaleNodeZ()
		{
			return GetNode<SpinBox>("HBoxContainer/ScaleValues/ScaleAngleZ/SpinBox");
		}
		
		/// <summary>
		/// Updates rotation and scale values on the current handle when a change is received from the context menu.
		/// </summary>
		/// <param name="package">The dictionary package containing rotation and scale values.</param>
		private void _OnUpdateVectors(Godot.Collections.Dictionary package)
		{
			Node3D Handle = ExplorerUtils.Get().GetHandle();
			Handle.RotationDegrees = package["Rotation"].As<Vector3>();
			Handle.Scale = package["Scale"].As<Vector3>();
		}
	}
}
