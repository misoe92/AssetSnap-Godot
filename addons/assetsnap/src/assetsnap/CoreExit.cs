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

using AssetSnap.ASNode.Types;
using Godot;

namespace AssetSnap.Core 
{
	/// <summary>
    /// Handles all cleanup when the plugin exits the tree.
    /// </summary>
	public class CoreExit : Core 
	{
		/// <summary>
        /// General cleanup when the plugin exits the tree.
        /// </summary>
        /// <param name="plugin">The plugin instance.</param>
		public static void CleanArea( Plugin plugin )
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			
			if( null != _GlobalExplorer ) 
			{
				/** Removal of internal components **/ 
				// _RemoveEntry(_GlobalExplorer.BottomDock, plugin);

				// _GlobalExplorer.Modifiers._Exit();
				if( null != _GlobalExplorer._Plugin ) 
				{
					/** Dispose custom node types **/
					foreach( NodeType _NodeType in _GlobalExplorer._Plugin.NodeTypes ) 
					{
						_NodeType.Dispose(plugin);
					}
				}
			}
		}
		
		/// <summary>
        /// Removes an internal component given its instance.
        /// </summary>
        /// <param name="obj">The object to remove.</param>
        /// <param name="plugin">The plugin instance.</param>
		public static void _RemoveEntry(Node obj, Plugin plugin)
		{
			if( EditorPlugin.IsInstanceValid(obj) ) 
			{
				obj.QueueFree();
			}
		} 
	}
}

#endif