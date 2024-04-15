namespace AssetSnap.Abstracts
{
	using AssetSnap.Front.Nodes;
	using Godot;

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
		public Component.Base Components { get => AssetSnap.Component.Base.Singleton; } 
		protected Component.Base _Components = null; 
		
		/*
		** Handles spawn waypoints
		*/
		public Front.Configs.SettingsConfig Settings { get => Front.Configs.SettingsConfig.Singleton; } 
		protected Front.Configs.SettingsConfig _Settings = null; 
	
		/*
		** Handles spawn waypoints 
		*/
		public Debug.Inspector Inspector { get => AssetSnap.Debug.Inspector.Singleton; } 
		protected Debug.Inspector _Inspector = null;
		
		/*
		** Handles spawn waypoints
		*/
		public Snap.Base Snap { get => AssetSnap.Snap.Base.Singleton; } 
		protected Snap.Base _Snap = null; 
		
		/*
		** Handles spawn waypoints
		*/
		public Snap.SnappableBase Snappable { get => AssetSnap.Snap.SnappableBase.Singleton; } 
		protected Snap.SnappableBase _Snappable = null; 
		
		/*
		** Handles spawn waypoints
		*/ 
		public Waypoint.Base Waypoints { get => AssetSnap.Waypoint.Base.Singleton; } 
		protected Waypoint.Base _Waypoints = null;
		
		/*
		** Handles spawn waypoints
		*/ 
		public ContextMenu.Base ContextMenu { get => AssetSnap.ContextMenu.Base.Singleton; } 
		protected ContextMenu.Base _ContextMenu = null;
		 
		/*
		** Handles spawn waypoints 
		*/ 
		public Library.Base Library { get => AssetSnap.Library.Base.Singleton; } 
		protected Library.Base _Library = null; 
		
		/* 
		** Handles spawn waypoints
		*/
		public Modifier.Base Modifiers { get => AssetSnap.Modifier.Base.Singleton; } 
		protected Modifier.Base _Modifiers = null; 
		
		/* 
		** Handles spawn waypoints
		*/
		public GroupBuilder.Base GroupBuilder { get => AssetSnap.GroupBuilder.Base.Singleton; } 
		protected GroupBuilder.Base _GroupBuilder = null; 
		
		public GroupBuilder.MainScreen GroupMainScreen { get; set; } 
		
		/*
		** Handles spawn waypoints
		*/
		public BottomDock.Base BottomDock { get => AssetSnap.BottomDock.Base.Singleton; } 
		protected BottomDock.Base _BottomDock = null; 

		/*
		** The raycast which are being used for projection positions
		*/
		public Raycast.Base Raycast { get => AssetSnap.Raycast.Base.Singleton; } 
		protected Raycast.Base _Raycast = null; 
		
		/*
		** Decal handler
		*/
		public AssetSnap.Decal.Base Decal { get => AssetSnap.Decal.Base.Singleton; } 
		protected AssetSnap.Decal.Base _Decal = null; 
	}
}