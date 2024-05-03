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
	/// Represents a TextureRect for displaying preview textures of a model.
	/// </summary>
	public partial class AsModelViewerRect : TextureRect
	{
		/// <summary>
        /// Sets the preview texture after it has fully loaded.
        /// </summary>
        /// <param name="Path">The path to the preview texture.</param>
        /// <param name="preview">The preview texture.</param>
        /// <param name="texture_preview">The texture preview.</param>
        /// <param name="textureRect">The TextureRect to set the texture on.</param>
		public void _MeshPreviewReady(string Path, Texture2D preview, Texture2D texture_preview, TextureRect textureRect)
		{
			if( null == preview && null == texture_preview ) 
			{
				GD.Print("Invalid preview: ", Path);
				return;
			}
			
			textureRect.Texture = preview;
		}
	}
}

#endif