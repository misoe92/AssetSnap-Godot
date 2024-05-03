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

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// Represents a control for conditional visibility based on the presence of certain values in a list.
	/// </summary>
	[Tool]
	public partial class AsConditional : Control
	{
		private Control Control_;
		private Callable _Callable;
		
		/// <summary>
		/// Gets or sets the control to be observed for conditional visibility.
		/// </summary>
		[Export]
		public Control _Control 
		{
			get => Control_;
			set 
			{
				Control_ = value;
			}
		}

		/// <summary>
        /// Gets or sets an array of values to check for in the associated control.
        /// </summary>
		[Export]
		public string[] values;

		/// <summary>
        /// Handles the visibility state of the node based on the presence of certain values in the associated control.
        /// </summary>
        /// <param name="delta">The time elapsed since the last frame, in seconds.</param>
        /// <returns>void</returns>
		public override void _Process(double delta)
		{
			if(Control_ == null || _Control.GetParent() == null || GetParent<Control>().Visible == false) 
			{
				return;
			}

			if( Control_ is AsSelectList _List && _List._Button != null) 
			{
				bool found = false;
				for( int i = 0; i < values.Length; i++ ) 
				{
					var value = values[i];
					if (_List._Button.Text.Contains(value) )
					{
						Visible = true;
						found = true;
						break;
					}
				}

				if( false == found ) 
				{
					Visible = false;
				}
			}
		}
	} 
}

#endif