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

namespace AssetSnap.Helpers
{
	/// <summary>
	/// A static helper class for string manipulation.
	/// </summary>
	public static class StringHelper
	{
		/// <summary>
		/// Converts a file name to a title format.
		/// </summary>
		/// <param name="filename">The file name to convert.</param>
		/// <returns>The file name converted to title format.</returns>
		public static string FileNameToTitle( string filename )
		{
			return filename.Split(".")[0].ToCamelCase();
		}
		
		/// <summary>
        /// Extracts the file name from a file path.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The extracted file name.</returns>
		public static string FilePathToFileName( string path )
		{
			string delimiter = "/";
			string[] pathSplit = path.Split(delimiter);

			return pathSplit[pathSplit.Length - 1];
		}
	}
}