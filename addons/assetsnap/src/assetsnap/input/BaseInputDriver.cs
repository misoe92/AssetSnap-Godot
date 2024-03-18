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

namespace AssetSnap.Instance.Input
{
	using AssetSnap.Front.Nodes;
	using Godot;
	
	public class BaseInputDriver 
	{
		private static BaseInputDriver _Instance;
		public bool IsMulti = false;
		
		public static BaseInputDriver GetInstance()
		{
			if( null == _Instance ) 
			{
				_Instance = new();
			}

			return _Instance;
		}
		
		/*
		** Handles base input events
		** 
		** @param Camera3D Camera
		** @param InputEvent Event
		** @return int
		*/
		public virtual int _Input( Camera3D Camera, InputEvent Event) 
		{
			GlobalExplorer messageBus = GlobalExplorer.GetInstance();
			
			if( messageBus.CurrentLibrary == null ) 
			{
				return (int)EditorPlugin.AfterGuiInput.Pass;
			}
			
			if( Event is InputEventKey _KeyEvent ) 
			{
				if( IsKeyReset(_KeyEvent) ) 
				{
					// _Parent.CurrentLibrary.ClearAllPanelState();
					// CoreHandles.GetInstance<CoreHandles>().ResetHandle();
				}
			}
			if(Event is InputEventMouseButton _MouseButtonEvent)
			{
				if (HasMouseLeftPressed(_MouseButtonEvent))
				{
					MeshInstance3D ModelInstance = messageBus.Decal.GetMeshInstance();
				
					if (
						CanMultiDrop() &&
						ShiftInputPressed(_MouseButtonEvent) &&
						AltInputPressed(_MouseButtonEvent) &&
						ModelInstance is AssetSnap.Front.Nodes.AsMeshInstance3D
					)
					{
						IsMulti = true;
					}
					else 
					{
						IsMulti = false;
					}
					
					messageBus.Waypoints.Spawn(ModelInstance, messageBus.Decal.GetNode().Transform.Origin, ModelInstance.RotationDegrees, ModelInstance.Scale);
					
					if( false == ShouldFocusAsset() ) 
					{
						messageBus.CurrentLibrary._LibrarySettings._LSEditing.SetText("None");
						messageBus.HandleNode = null;
						messageBus.Model = null;
					}
				
					if( CanMultiDrop() && ShiftInputPressed(_MouseButtonEvent) && AltInputPressed(_MouseButtonEvent) && ModelInstance is AssetSnap.Front.Nodes.AsMeshInstance3D AsModelInstance ) 
					{
						AssetSnap.Front.Nodes.AsMeshInstance3D Duplicate = new()
						{
							Name = AsModelInstance.Name,
							Mesh = AsModelInstance.Mesh,
							Transform = new Transform3D(Basis.Identity, new Vector3(0,0,0)),
							Scale = AsModelInstance.Scale,
							RotationDegrees = AsModelInstance.RotationDegrees,
							Floating = true,
							SpawnSettings = AsModelInstance.SpawnSettings,
						};

						Duplicate.SetLibraryName(AsModelInstance.GetLibraryName());
						
						messageBus.CurrentLibrary = Duplicate.GetLibrary();
						messageBus.Model = Duplicate;
						messageBus.HandleNode = Duplicate;
					}
					else 
					{
						messageBus.CurrentLibrary.ClearAllPanelState();
						messageBus.CurrentLibrary = null;
					}
					
					return (int)EditorPlugin.AfterGuiInput.Stop;
				}
			}
			
			return (int)EditorPlugin.AfterGuiInput.Pass; 
		}
		
		/*
		** Forces focus to a model
		**
		** @return void
		*/
		public void FocusAsset( Node3D _model )
		{
			GlobalExplorer.GetInstance()._ForceFocus = _model;
		}
		
		public bool CanMultiDrop( )
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			return _GlobalExplorer.Settings.GetKey("allow_multi_drop").As<bool>();
		}
		
		public bool ShiftInputPressed(InputEventMouseButton _MouseButtonEvent)
		{
			return _MouseButtonEvent.ShiftPressed;
		}
		
		public bool AltInputPressed(InputEventMouseButton _MouseButtonEvent)
		{
			return _MouseButtonEvent.AltPressed;
		}
		/*
		** Checks if a asset should be focused after placement
		**
		** @return bool
		*/
		public bool ShouldFocusAsset()
		{
			bool value = GlobalExplorer.GetInstance().Settings.GetKey("focus_placed_asset").As<bool>();
			
			if( value is bool valueBool ) 
			{
				return valueBool;
			}

			return false;
		}
		
		/*
		** Checks if current keycode is escape
		**
		** @param InputEventKey _KeyEvent
		** @return bool
		*/
		private bool IsKeyReset(InputEventKey _KeyEvent)
		{
			return _KeyEvent.Keycode == Key.Escape;
		}
		 
		/*
		** Checks if left mouse button has been pressed
		**
		** @param InputEventMouseButton _MouseButtonEvent
		** @return bool
		*/
		private bool HasMouseLeftPressed(InputEventMouseButton _MouseButtonEvent)
		{
			return _MouseButtonEvent.ButtonIndex == MouseButton.Left && false == _MouseButtonEvent.Pressed;
		}
		
		/*
		** Checks if right mouse button has been pressed
		**
		** @param InputEventMouseButton _MouseButtonEvent
		** @return bool
		*/
		private bool HasMouseRightPressed(InputEventMouseButton _MouseButtonEvent)
		{
			return _MouseButtonEvent.ButtonIndex == MouseButton.Right && false == _MouseButtonEvent.Pressed;
		}
	}
}