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

using AssetSnap.Front.Modifiers;

namespace AssetSnap.Modifier
{
	/// <summary>
	/// Base class for modifiers.
	/// </summary>
	public partial class Base
	{
		/// <summary>
		/// The array modifier instance.
		/// </summary>
		public AsArrayModifier ArrayModifier = new();
		
		/// <summary>
		/// The scatter modifier instance.
		/// </summary>
		public AsScatterModifier ScatterModifier = new();
		
		private static Base _Instance;
		
		/// <summary>
		/// Gets the singleton instance of the Base class.
		/// </summary>
		public static Base Singleton 
		{
			get
			{
				if( null == _Instance ) 
				{
					_Instance = new();
				}
				
				return _Instance;
			}
		}
		
		/// <summary>
        /// Called when the Base instance is exiting.
        /// </summary>
		public void _Exit()
		{
			ArrayModifier = null;
			ScatterModifier = null;
		}
	}
}
#endif