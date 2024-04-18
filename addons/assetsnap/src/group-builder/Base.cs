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
namespace AssetSnap.GroupBuilder
{
	using System.Collections.Generic;
	using AssetSnap.Explorer;
	using AssetSnap.Front.Components.Groups.Builder;
	using AssetSnap.Front.Nodes;
	using AssetSnap.States;
	using Godot;
	
	[Tool]
	public partial class Base
	{
		public bool Initialized = false;
		
		/*
		** Public
		*/
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
		
		public PanelContainer Container;
		public Front.Components.Groups.Container _GroupContainer;
		public Sidebar _Sidebar;
		public Editor _Editor;
		
		
		/*
		** Private
		*/
		private readonly List<string> OuterComponents = new()
		{
			"Groups.Container",
		};
		private readonly List<string> InnerComponents = new()
		{
			"Groups.Builder.Sidebar",
			"Groups.Builder.Editor",
		};
		private static Base _Instance;
		
		/*
		** Initializes the group builder
		*/
		public void Initialize()
		{
			StatesUtils.SetLoad("GroupBuilder", true);
		}
		
		public bool HasMenu()
		{
			if(
				null == Plugin.Singleton ||
				null == Plugin.Singleton.GetInternalContainer()
			)
			{
				return false;				
			}

			return Plugin.Singleton
				.GetInternalContainer()
				.HasNode("GroupContextMenu");
		}
		
		public AsGroupContextMenu GetMenu()
		{
			return ExplorerUtils.Get()._Plugin
				.GetInternalContainer()
				.GetNode("GroupContextMenu") as AsGroupContextMenu;
		}
		
		public void CreateMenu()
		{
			AsGroupContextMenu menu = new();
			menu.Initialize();
			
			ExplorerUtils.Get()._Plugin 
				.GetInternalContainer()
				.AddChild(menu);
		}

		public void ShowMenu( Vector2 coordinates, string path )
		{
			if( ! HasMenu() ) 
			{
				CreateMenu();
			}
			
			GetMenu().ShowMenu( coordinates, path );
		}
		
		public void MaybeHideMenu( Vector2 coordinates )
		{
			if (!HasMenu())
			{
				return;
			}

			GetMenu().MaybeHideMenu( coordinates );
		}
		
		/*
		** Initializes the container in the assets tab
		*/
		public void InitializeContainer()
		{
			Component.Base Components = ExplorerUtils.Get().Components;
			
			if( Initialized ) 
			{
				// Clear the instances first
				Components.Clear<Sidebar>();
				Components.Clear<Editor>();
				Components.Clear<AssetSnap.Front.Components.Groups.Container>();
			}
			
			Initialized = true;
			
			Container = new()
			{
				Name = "GroupBuilder",
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
			};

			/** Add the tab item if settings is turned on **/
			if ( Components.HasAll( OuterComponents.ToArray() )) 
			{
				_GroupContainer = Components.Single<AssetSnap.Front.Components.Groups.Container>();
				
				if( HasGroupContainer() ) 
				{
					_GroupContainer.Container = Container;
					_GroupContainer.Initialize();
			
					if ( Components.HasAll( InnerComponents.ToArray() )) 
					{
						_Sidebar = Components.Single<Sidebar>();
						_Editor = Components.Single<Editor>();
						
						if( HasSidebar() ) 
						{
							_Sidebar.Container = _GroupContainer.GetLeftInnerContainer();
							_Sidebar.Initialize();
						}
						
						if( HasListing() ) 
						{
							_Editor.Container = _GroupContainer.GetRightInnerContainer();
							_Editor.Initialize();
						}
						
						StatesUtils.SetLoad("GroupBuilderContainer", true);
					}
					else
					{
						GD.PushError("Components was not found @ GroupBuilder -> Inner");
					}
				}
				else 
				{
					GD.PushError("Could not spawn group container");
				}
			}
			else
			{
				GD.PushError("Components was not found @ GroupBuilder -> Outer");
			}
		} 
		
		public void ClearContainer()
		{
			_Sidebar.Clear();
			_Editor.Clear();
			_GroupContainer.Clear();
			
			if( null != Container ) 
			{
				if( null != Container.GetParent() ) 
				{
					Container.GetParent().RemoveChild(Container);
				}

				Container.Free();
			}
			else 
			{
				GD.PushError("Invalid container found");
			}
		}
		
		public bool HasSidebar()
		{
			return null != _Sidebar;
		}
		
		public bool HasListing()
		{
			return null != _Editor;
		}
		private bool HasGroupContainer()
		{
			return null != _GroupContainer;
		}
	}
}
#endif