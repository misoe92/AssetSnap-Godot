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

using AssetSnap.Explorer;
using Godot;

namespace AssetSnap.Front.Nodes.Hoverable
{
	/// <summary>
    /// Partial class representing a margin container that responds to hover events and applies a Scatter modifier on click.
    /// </summary>
	[Tool]
	public partial class ScatterMarginContainer : HoverableMarginContainer
	{
		/// <summary>
        /// Handles the mouse click event to apply the Scatter modifier.
        /// </summary>
        /// <param name="event">The input event representing the mouse click.</param>
        /// <returns>Void.</returns>
		public override void _MouseClick( InputEvent @event )
		{
			if( @event is InputEventMouseButton EventMouseButton ) 
			{
				if( EventMouseButton.ButtonIndex == MouseButton.Left && EventMouseButton.Pressed ) 
				{
					ExplorerUtils.Get().Modifiers.ScatterModifier.Apply();
					var _parent = GetParent();
					if( _parent is AsSelectList _parentSelect ) 
					{
						_parentSelect.SetActive(null);
						_parentSelect.ResetListVisibility();
					}
				}
			}
		}
	}
}
