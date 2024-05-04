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

using AssetSnap.Explorer;
using AssetSnap.Front.Nodes;
using Godot;

namespace AssetSnap.BottomDock
{
	/// <summary>
	/// Base class for the bottom dock functionality.
	/// </summary>
	[Tool]
	public partial class Base
	{	
		/// <summary>
		/// Singleton instance of the Base class.
		/// </summary>
		public static Base Singleton
		{
			get
			{
				if( null == _Instance ) 
				{
					_Instance = new()
					{
						Name = "AssetSnapBottomDock"
					}; 
					_Instance.Initialize();
				}


				return _Instance;
			}
		}
		
		/// <summary>
		/// The container for the bottom dock.
		/// </summary>
		public AsBottomDock Container
		{
			get => _Container;
			set
			{
				_Container = value;
			}
		}
		
		/// <summary>
		/// Name of the Base instance.
		/// </summary>
		public string Name = "";

		private AsBottomDock _Container;
		private static Base _Instance = null;
		
		/// <summary>
		/// Initializes the bottom dock tab.
		/// </summary>
		public void Initialize()
		{
			//
		}
		
		/// <summary>
		/// Sets the tab in the bottom dock.
		/// </summary>
		/// <param name="library">The library instance to set.</param>
		public void SetTab( Library.Instance library ) 
		{
			// ExplorerUtils.Get()._Plugin.GetTabContainer().CurrentTab = library.Index + 2;
		}
		
		/// <summary>
		/// Sets the tab in the bottom dock by index.
		/// </summary>
		/// <param name="index">The index of the tab to set.</param>
		public void SetTabByIndex( int index ) 
		{
			// ExplorerUtils.Get()._Plugin.GetTabContainer().CurrentTab = index;
		} 
		
		/// <summary>
		/// Adds a child to the tab container.
		/// </summary>
		/// <param name="container">The container to add.</param>
		public void Add( Container container )
		{
			if( false == IsValid(container) || null == ExplorerUtils.Get()._Plugin ) 
			{
				return;
			}
			
			if( Has( container ) ) 
			{
				Remove(container);
			}

			ExplorerUtils.Get()._Plugin.GetTabContainer().AddChild(container, true);
		}
		
		/// <summary>
		/// Checks if a child exists in the tab container.
		/// </summary>
		/// <param name="container">The container to check.</param>
		/// <returns>True if the container exists, otherwise false.</returns>
		public bool Has( Container container )
		{
			if( null == ExplorerUtils.Get()._Plugin.GetTabContainer() || false == EditorPlugin.IsInstanceValid(ExplorerUtils.Get()._Plugin.GetTabContainer()) ) 
			{
				GD.PushError("Invalid container @ Bottom Dock: ", container.Name);
				return false;
			}
			
			foreach( Node child in ExplorerUtils.Get()._Plugin.GetTabContainer().GetChildren() ) 
			{
				if( EditorPlugin.IsInstanceValid(child) && child.Name == container.Name ) 
				{
					return true;
				}
			}
 
			return false;
		}

		/// <summary>
		/// Removes a child from the tab container.
		/// </summary>
		/// <param name="container">The container to remove.</param>
		public void Remove( Container container ) 
		{
			foreach( Node child in _Container.GetChildren() ) 
			{
				if(  EditorPlugin.IsInstanceValid(child) && child.Name == container.Name ) 
				{
					// _Container.RemoveChild(child);
					child.QueueFree();
				}
			}
		}
		
		/// <summary>
		/// Removes a child from the tab container by name.
		/// </summary>
		/// <param name="name">The name of the child to remove.</param>
		public void RemoveByName( string name ) 
		{
			foreach( Node child in _Container.GetChildren() ) 
			{
				if(  EditorPlugin.IsInstanceValid(child) && child.Name == name ) 
				{
					// _Container.RemoveChild(child);
					child.QueueFree();
				}
			}
		}
		
		/// <summary>
        /// Checks if a container is valid.
        /// </summary>
        /// <param name="_Container">The container to check.</param>
        /// <returns>True if the container is valid, otherwise false.</returns>
		private bool IsValid( Container _Container )
		{
			return null != _Container && true == EditorPlugin.IsInstanceValid(_Container); 
		}
	}
}

#endif