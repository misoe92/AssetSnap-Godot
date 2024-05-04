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

using AssetSnap.Front.Nodes;

namespace AssetSnap.Abstracts
{
	/// <summary>
	/// Base class for explorer-related functionality.
	/// </summary>
	public abstract partial class AbstractExplorerBase
	{
		/// <summary>
		/// Represents the scroll state.
		/// </summary>
		public enum ScrollState 
		{
			SCROLL_DISABLED,
			SCROLL_ENABLED,
		}
		
		/// <summary>
		/// The current mouse input event.
		/// </summary>
		public EventMouse _CurrentMouseInput = EventMouse.EventNone;
		
		/// <summary>
		/// Gets the plugin main file instance.
		/// </summary>
		public Plugin _Plugin { get => AssetSnap.Plugin.GetInstance(); }
		
		/// <summary>
		/// Handles internal components.
		/// </summary>
		public Component.Base Components { get => AssetSnap.Component.Base.Singleton; } 
		
		
		/// <summary>
		/// Handles settings configurations.
		/// </summary>
		public Front.Configs.SettingsConfig Settings { get => Front.Configs.SettingsConfig.Singleton; } 
	
		/// <summary>
		/// Handles inspector functionalities.
		/// </summary>
		public Debug.Inspector Inspector { get => AssetSnap.Debug.Inspector.Singleton; } 
		
		/// <summary>
		/// Handles snap functionalities.
		/// </summary>
		public Snap.Base Snap { get => AssetSnap.Snap.Base.Singleton; } 
		
		/// <summary>
		/// Handles snappable functionalities.
		/// </summary>
		public Snap.SnappableBase Snappable { get => AssetSnap.Snap.SnappableBase.Singleton; } 
		
		/// <summary>
		/// Handles waypoint functionalities.
		/// </summary>
		public Waypoint.Base Waypoints { get => AssetSnap.Waypoint.Base.Singleton; } 
		
		/// <summary>
		/// Handles context menu functionalities.
		/// </summary>
		public ContextMenu.Base ContextMenu { get => AssetSnap.ContextMenu.Base.Singleton; } 
		 
		/// <summary>
		/// Handles library functionalities.
		/// </summary>
		public Library.Base Library { get => AssetSnap.Library.Base.Singleton; } 
		protected Library.Base _Library = null; 
		
		/* 
		** Handles spawn waypoints
		*/
		public Modifier.Base Modifiers { get => AssetSnap.Modifier.Base.Singleton; } 
		
		/// <summary>
		/// Handles modifier functionalities.
		/// </summary>
		public GroupBuilder.Base GroupBuilder { get => AssetSnap.GroupBuilder.Base.Singleton; } 
		
		/// <summary>
		/// Handles group builder functionalities.
		/// </summary>
		protected GroupBuilder.Base _GroupBuilder = null; 
		
		/// <summary>
		/// Represents the main screen of the group builder.
		/// </summary>
		public GroupBuilder.MainScreen GroupMainScreen { get; set; } 
		
		/// <summary>
		/// Handles bottom dock functionalities.
		/// </summary>
		public BottomDock.Base BottomDock { get => AssetSnap.BottomDock.Base.Singleton; } 

		/// <summary>
		/// Represents the raycast used for projection positions.
		/// </summary>
		public Raycast.Base Raycast { get => AssetSnap.Raycast.Base.Singleton; }
		
		/// <summary>
		/// Handles decal functionalities.
		/// </summary>
		public AssetSnap.Decal.Base Decal { get => AssetSnap.Decal.Base.Singleton; } 
		
		/// <summary>
		/// Delta time.
		/// </summary>
		protected float _DeltaTime = 0.0f;
		
		/// <summary>
		/// Collection of select lists.
		/// </summary>
		protected Godot.Collections.Array<AsSelectList> _SelectLists = new();
		
		/// <summary>
		/// Flag indicating whether the object has been disposed.
		/// </summary>
		protected bool _Disposed = false;
		
		/// <summary>
		/// The current state of scrolling.
		/// </summary>
		protected ScrollState _AllowScroll = ScrollState.SCROLL_ENABLED;
		
		protected Component.Base _Components = null; 
		protected Snap.Base _Snap = null; 
		protected Snap.SnappableBase _Snappable = null; 
		protected Waypoint.Base _Waypoints = null;
		protected ContextMenu.Base _ContextMenu = null;
		protected Modifier.Base _Modifiers = null; 
		protected BottomDock.Base _BottomDock = null; 
		protected Raycast.Base _Raycast = null; 
		protected Front.Configs.SettingsConfig _Settings = null; 
		protected AssetSnap.Decal.Base _Decal = null; 
		protected Debug.Inspector _Inspector = null;
	}
}