namespace AssetSnap.Abstracts
{
	using AssetSnap.Front.Nodes;
	
	public abstract partial class AbstractExplorerBase
	{
		public enum ScrollState 
		{
			SCROLL_DISABLED,
			SCROLL_ENABLED,
		}
		
		protected bool _Disposed = false;
		protected Godot.Collections.Array<AsSelectList> _SelectLists = new();
		protected float _DeltaTime = 0.0f;
		protected ScrollState _AllowScroll = ScrollState.SCROLL_ENABLED;
		public EventMouse _CurrentMouseInput = EventMouse.EventNone;
		
		/*
		** Plugin main file
		*/
		public Plugin _Plugin { get => AssetSnap.Plugin.GetInstance(); }
		
		/*
		** Handles internal components
		*/
		public Component.Base Components { get => _Components; } 
		protected Component.Base _Components = null; 
		
		/*
		** Handles spawn waypoints
		*/
		public Front.Configs.SettingsConfig Settings { get => _Settings; } 
		protected Front.Configs.SettingsConfig _Settings = null; 
		
		/*
		** Handles spawn waypoints
		*/ 
		public Waypoint.Base Waypoints { get => _Waypoints; } 
		protected Waypoint.Base _Waypoints = null;
		
		/*
		** Handles spawn waypoints
		*/ 
		public ContextMenu.Base ContextMenu { get => _ContextMenu; } 
		protected ContextMenu.Base _ContextMenu = null;
		 
		/*
		** Handles spawn waypoints 
		*/ 
		public Library.Base Library { get => _Library; } 
		protected Library.Base _Library = null; 
		
		/* 
		** Handles spawn waypoints
		*/
		public Modifier.Base Modifiers { get => _Modifiers; } 
		protected Modifier.Base _Modifiers = null; 
		
		/*
		** Handles spawn waypoints
		*/
		public BottomDock.Base BottomDock { get => _BottomDock; } 
		protected BottomDock.Base _BottomDock = null; 

		/*
		** The raycast which are being used for projection positions
		*/
		public Raycast.Base Raycast { get => _Raycast; } 
		protected Raycast.Base _Raycast = null; 
		
		/*
		** Decal handler
		*/
		public Decal.Base Decal { get => _Decal; } 
		protected Decal.Base _Decal = null; 
	}
}