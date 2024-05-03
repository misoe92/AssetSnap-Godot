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

using Godot;

namespace AssetSnap.Static
{
	/// <summary>
	/// Utility class for handling input events.
	/// </summary>
	public static class InputsStatic
	{
		/// <summary>
		/// Checks if the Shift key is pressed.
		/// </summary>
		/// <param name="_MouseButtonEvent">The mouse button input event.</param>
		/// <returns>True if the Shift key is pressed; otherwise, false.</returns>
		public static bool ShiftInputPressed(InputEventMouseButton _MouseButtonEvent)
		{
			return _MouseButtonEvent.ShiftPressed;
		}
		
		/// <summary>
        /// Checks if the Alt key is pressed.
        /// </summary>
        /// <param name="_MouseButtonEvent">The mouse button input event.</param>
        /// <returns>True if the Alt key is pressed; otherwise, false.</returns>
		public static bool AltInputPressed(InputEventMouseButton _MouseButtonEvent)
		{
			return _MouseButtonEvent.AltPressed;
		}
		
		/// <summary>
        /// Checks if the left mouse button is pressed.
        /// </summary>
        /// <param name="_MouseButtonEvent">The mouse button input event.</param>
        /// <returns>True if the left mouse button is pressed; otherwise, false.</returns>
		public static bool HasMouseLeftPressed(InputEventMouseButton _MouseButtonEvent)
		{
			return _MouseButtonEvent.ButtonIndex == MouseButton.Left && false == _MouseButtonEvent.Pressed;
		}
		
		/// <summary>
        /// Checks if the right mouse button is pressed.
        /// </summary>
        /// <param name="_MouseButtonEvent">The mouse button input event.</param>
        /// <returns>True if the right mouse button is pressed; otherwise, false.</returns>
		public static bool HasMouseRightPressed(InputEventMouseButton _MouseButtonEvent)
		{
			return _MouseButtonEvent.ButtonIndex == MouseButton.Right && false == _MouseButtonEvent.Pressed;
		}
	}
}