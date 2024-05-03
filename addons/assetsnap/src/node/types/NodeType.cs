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

using System.Collections.Generic;
using Godot;
	
namespace AssetSnap.ASNode.Types
{
	/// <summary>
	/// Represents a base class for custom node types.
	/// </summary>
	[Tool]
	public class NodeType
	{
		/// <summary>
		/// The name of the custom node type.
		/// </summary>
		public string Name;

		/// <summary>
		/// The parent class that the custom node type inherits from.
		/// </summary>
		public string Inherits;

		/// <summary>
		/// The file path of the script associated with the custom node type.
		/// </summary>
		public string ScriptPath;

		/// <summary>
        /// The file path of the icon associated with the custom node type.
        /// </summary>
		public string IconPath;
		
		/// <summary>
        /// Initializes the custom node type by adding it to the editor.
        /// </summary>
        /// <returns>void</returns>
		public void Initialize()
		{
			Plugin _Plugin = GlobalExplorer.GetInstance()._Plugin; 
			_Plugin.AddCustomType(Name, Inherits, GD.Load<Script>(ScriptPath), GD.Load<Texture2D>(IconPath));

			List<NodeType> _List = new(_Plugin.NodeTypes)
			{
				this
			};

			_Plugin.NodeTypes = _List.ToArray();
		}
			
		/// <summary>
        /// Disposes of the custom node type by removing it from the editor.
        /// </summary>
        /// <param name="plugin">The plugin instance to remove the custom type from.</param>
        /// <returns>void</returns>
		public void Dispose(Plugin plugin)
		{
			plugin.RemoveCustomType(Name);
			
			List<NodeType> _List = new(plugin.NodeTypes);
			_List.Remove(this);
			
			plugin.NodeTypes = _List.ToArray();
		}
	}
}