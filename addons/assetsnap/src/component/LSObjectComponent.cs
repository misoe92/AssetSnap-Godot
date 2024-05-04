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

using Godot;

namespace AssetSnap.Component
{
	/// <summary>
	/// Base class for components related to Library Object Snap.
	/// </summary>
	public partial class LSObjectComponent : LibraryComponent
	{
		protected MarginContainer _MarginContainer;
		protected VBoxContainer _InnerContainer;
		protected Label _Label;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LSObjectComponent"/> class.
		/// </summary>
		public LSObjectComponent()
		{
			Name = "LSObjectComponent";
			//_include = false;
		}
		
		/// <summary>
		/// Sets the visibility state of the component.
		/// </summary>
		/// <param name="state">The visibility state to set.</param>
		public virtual void SetVisible( bool state ) 
		{
			_MarginContainer.Visible = state;
		}
		
		/// <summary>
		/// Checks if the component is currently visible.
		/// </summary>
		/// <returns><c>true</c> if the component is visible; otherwise, <c>false</c>.</returns>
		public virtual bool IsVisible() 
		{
			return true == _MarginContainer.Visible;
		}
		
		/// <summary>
		/// Checks if Library Object Snap is enabled.
		/// </summary>
		/// <returns><c>true</c> if Library Object Snap is enabled; otherwise, <c>false</c>.</returns>
		public bool IsSnapToObject()
		{
			return _GlobalExplorer.States.SnapToObject == GlobalStates.LibraryStateEnum.Enabled;
		}

		/// <summary>
        /// Called when the node is about to be removed from the scene tree.
        /// </summary>
		public override void _ExitTree()
		{
			if( IsInstanceValid(_Label) ) 
			{
				_Label.QueueFree();
				_Label = null;
			}
			
			if( IsInstanceValid(_InnerContainer) ) 
			{
				_InnerContainer.QueueFree();
				_InnerContainer = null;
			}
			
			if( IsInstanceValid(_MarginContainer) ) 
			{
				_MarginContainer.QueueFree();
				_MarginContainer = null;
			}
			
			base._ExitTree();
		}
	}
}

#endif