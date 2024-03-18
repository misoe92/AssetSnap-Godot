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

namespace AssetSnap.Front.Components
{
	using Godot;
	using AssetSnap.Component;
	using AssetSnap.Front.Nodes;
	using AssetSnap.Instance.Input;

	public partial class LibrarySnapGrab : LibraryComponent
	{
		public LibrarySnapGrab()
		{
			Name = "LibrarySnapGrab";
			// _include = false;  
		}
	
		/*
		** Checks if rotation is currently active
		** and whether or not to apply it
		**
		** @return void
		*/
		public override void _Input(InputEvent @event)
		{
			if( false == _ShouldGrab() ) 
			{
				return;
			}
			
			if(
				null == _GlobalExplorer ||
				false == EditorPlugin.IsInstanceValid( _GlobalExplorer.Model )  &&
				false == EditorPlugin.IsInstanceValid( _GlobalExplorer.HandleNode )
			) 
			{ 
				return;
			}
			
			if( @event is InputEventKey keyEvent && keyEvent.Keycode == Key.A && Input.IsKeyPressed(Key.Shift) && Input.IsKeyPressed(Key.Alt) && keyEvent.IsPressed() == false) 
			{
				// Grab the currently chosen node.
				Node _Node = _GlobalExplorer.Model;
				if( null == _Node ) 
				{
					_Node = _GlobalExplorer.HandleNode;
				}
				
				if( _Node is not AsMeshInstance3D ) 
				{
					return;
				}

				AsMeshInstance3D _MeshInstance3D = _Node as AsMeshInstance3D;

				var CurrentLibrary = _GlobalExplorer.GetLibraryByName( _MeshInstance3D.GetLibraryName() );
				if( null == CurrentLibrary ) 
				{
					GD.PushWarning("No library");
					return;
				}
				
				Node Parent = _MeshInstance3D.GetParent();
				if( null != Parent ) 
				{
					Parent.RemoveChild(_MeshInstance3D);
				}
				else 
				{
					GD.PushWarning("No Parent");
					return;
				}
				
				_GlobalExplorer.CurrentLibrary = CurrentLibrary;
				_GlobalExplorer.HandleNode = _MeshInstance3D;
				_GlobalExplorer.Model = _MeshInstance3D;
			
				EditorInterface.Singleton.EditNode(_MeshInstance3D);
				_GlobalExplorer.CurrentLibrary._LibrarySettings._LSEditing.SetText(_MeshInstance3D.Name);
				
				if( _GlobalExplorer.InputDriver is DragAddInputDriver DraggableInputDriver ) 
				{
					DraggableInputDriver.CalculateObjectSize();
				}

				if ( null != Parent && Parent is AsStaticBody3D ) 
				{
					if( null != Parent.GetParent() ) 
					{
						Parent.GetParent().RemoveChild(Parent);
					}
				}
			}
		}
		
		/*
		** Returns the current state of
		** object grabbing
		**
		** @return bool
		*/
		private bool _ShouldGrab()
		{
			if( null == _GlobalExplorer || false == EditorPlugin.IsInstanceValid(_GlobalExplorer.Settings) ) 
			{
				return false;
			}
			
			bool ModelGrab = _GlobalExplorer.Settings.GetKey("allow_model_grab").As<bool>();
			return ModelGrab;
		}
	}
}