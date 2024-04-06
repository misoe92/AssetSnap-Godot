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
	using AssetSnap.Static;
	using Godot;
	
	public class BaseInputDriver 
	{
		/*
		** Holds a reference to the current instance
		*/
		private static BaseInputDriver _Instance;
		
		/*
		** Checks if Is Multi is currently enabled
		*/
		public bool IsMulti = false;
		
		/*
		** Fetches an instance of the input driver
		*/
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
			if( false == ShouldListen() ) 
			{
				return (int)EditorPlugin.AfterGuiInput.Pass;
			}

			_ListenForReset( Event );
			
			int _result = _ListenForMouseButtons(Event);
			if( _result != 0 ) 
			{
				return _result;
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
		
		/*
		** Listens to for reset events
		**
		** @return void
		*/
		private void _ListenForReset( InputEvent Event )
		{
			if( Event is InputEventKey _KeyEvent ) 
			{
				if( IsKeyReset(_KeyEvent) ) 
				{
					// _Parent.CurrentLibrary.ClearAllPanelState();
					// CoreHandles.GetInstance<CoreHandles>().ResetHandle();
				}
			}
		}
		
		/*
		** Listens to mouse button events
		**
		** @return void
		*/
		private int _ListenForMouseButtons( InputEvent Event )
		{
			GlobalExplorer explorer = GlobalExplorer.GetInstance();
			
			if(Event is InputEventMouseButton _MouseButtonEvent)
			{
				if ( InputsStatic.HasMouseLeftPressed(_MouseButtonEvent) )
				{
					Node3D Node = explorer.GetHandle();
				
					if (
						SettingsStatic.CanMultiDrop() &&
						InputsStatic.ShiftInputPressed(_MouseButtonEvent) &&
						InputsStatic.AltInputPressed(_MouseButtonEvent)
					)
					{
						IsMulti = true;
					}
					else 
					{
						IsMulti = false;
					}
					
					explorer.Waypoints.Spawn(Node, explorer.Decal.GetNode().Transform.Origin, Node.RotationDegrees, Node.Scale);
					
					if( false == SettingsStatic.ShouldFocusAsset() ) 
					{
						explorer.CurrentLibrary._LibrarySettings._LSEditing.SetText("None");
						explorer.HandleNode = null;
						explorer.Model = null;
						explorer.States.EditingObject = null;
					}
				
					if( SettingsStatic.CanMultiDrop() && InputsStatic.ShiftInputPressed(_MouseButtonEvent) && InputsStatic.AltInputPressed(_MouseButtonEvent) ) 
					{
						if( Node is AsMeshInstance3D meshInstance3D ) 
						{
							AssetSnap.Front.Nodes.AsMeshInstance3D Duplicate = new()
							{
								Name = Node.Name,
								Mesh = meshInstance3D.Mesh,
								Transform = new Transform3D(Basis.Identity, new Vector3(0,0,0)),
								Scale = Node.Scale,
								RotationDegrees = Node.RotationDegrees,
								Floating = true,
								SpawnSettings = meshInstance3D.SpawnSettings.Duplicate(true),
							};
					
							Duplicate.SetLibraryName(meshInstance3D.GetLibraryName());
							explorer.CurrentLibrary = Duplicate.GetLibrary();
							explorer.Model = Duplicate;
							explorer.HandleNode = Duplicate;
							explorer.States.EditingObject = Duplicate;
						}
						
						if( Node is AsGrouped3D grouped3D ) 
						{
							AsGrouped3D newGrouped3D = grouped3D.Duplicate() as AsGrouped3D;
							newGrouped3D.GroupPath = grouped3D.GroupPath;

							explorer.States.EditingObject = newGrouped3D;
							explorer.States.GroupedObject = newGrouped3D;
						}
					}
					else 
					{
						if( explorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Model ) 
						{
							explorer.CurrentLibrary.ClearAllPanelState();
							explorer.CurrentLibrary = null;
							explorer.States.Group = null;
							explorer.States.GroupedObject = null;
						}
						
						if( explorer.States.PlacingMode == GlobalStates.PlacingModeEnum.Group ) 
						{
							explorer.CurrentLibrary = null;
							explorer.States.PlacingMode = GlobalStates.PlacingModeEnum.Model;
							explorer.States.Group = null;
							explorer.States.GroupedObject = null;
						}
					}
					
					return (int)EditorPlugin.AfterGuiInput.Stop;
				}
			}
			return (int)EditorPlugin.AfterGuiInput.Pass;
		}
		
		/*
		** Checks if input should be listened to.
		*/
		private bool ShouldListen()
		{
			return 
				null != GlobalExplorer.GetInstance().CurrentLibrary ||
				GlobalExplorer.GetInstance().States.PlacingMode == GlobalStates.PlacingModeEnum.Group;
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
	}
}