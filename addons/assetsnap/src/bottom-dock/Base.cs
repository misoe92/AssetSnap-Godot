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

namespace AssetSnap.BottomDock
{
	using AssetSnap.Explorer;
	using AssetSnap.Front.Nodes;
	using Godot;

	[Tool]
	public partial class Base
	{	
		/*
		** Public
		*/
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
		
		public AsBottomDock Container
		{
			get => _Container;
			set
			{
				_Container = value;
			}
		}
		
		public string Name = "";

		/*
		** Private
		*/
		private AsBottomDock _Container;
		private static bool initial = true;
		private static Base _Instance = null;
		
		/*
		** Initializes the bottom dock tab
		**
		** @return void
		*/
		public void Initialize()
		{
		}
		
		public void SetTab( Library.Instance library ) 
		{
			// ExplorerUtils.Get()._Plugin.GetTabContainer().CurrentTab = library.Index + 2;
		}
		public void SetTabByIndex( int index ) 
		{
			// ExplorerUtils.Get()._Plugin.GetTabContainer().CurrentTab = index;
		} 
		
		/* 
		** Adds a child to our tab container
		**
		** @return void
		*/
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
		
		/*
		** Check if a child exists in our tab container 
		**
		** @return void
		*/
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
		
		private bool IsValid( Container _Container )
		{
			return null != _Container && true == EditorPlugin.IsInstanceValid(_Container); 
		}
	}
} 