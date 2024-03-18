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

namespace AssetSnap.ASNode.Types
{
	using System.Collections.Generic;
	using Godot;
	
	[Tool]
	public class NodeType
	{
		public string Name;

		public string Inherits;

		public string ScriptPath;

		public string IconPath;
		
		/*
		** Adds the custom type to the editor
		**
		** @return void 
		*/
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
			
		/*
		** Disposes of the custom type from the editor
		**
		** @return void 
		*/
		public void Dispose(Plugin plugin)
		{
			plugin.RemoveCustomType(Name);
			
			List<NodeType> _List = new(plugin.NodeTypes);
			_List.Remove(this);
			
			plugin.NodeTypes = _List.ToArray();
		}
	}
}