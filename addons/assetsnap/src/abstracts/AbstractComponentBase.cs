namespace AssetSnap.Abstracts
{
	using Godot;

	[Tool]
	public abstract partial class AbstractComponentBase : Node 
	{
		protected GlobalExplorer _GlobalExplorer;
		// protected Plugin _Plugin { get => _GlobalExplorer._Plugin; }
		// protected Component.Base Components { get => _GlobalExplorer.Components; }
		// protected Waypoint.Base Waypoints { get => _GlobalExplorer.Waypoints; }
		// protected ContextMenu.Base ContextMenu { get => _GlobalExplorer.ContextMenu; } 
		// protected Library.Base Libraries { get => _GlobalExplorer.Library; }
		// protected Modifier.Base Modifiers { get => _GlobalExplorer.Modifiers; }
		// protected BottomDock.Base BottomDock { get => _GlobalExplorer.BottomDock; }
		// protected Raycast.Base Raycast { get => _GlobalExplorer.Raycast; } 
		// protected AssetSnap.Decal.Base Decal { get => _GlobalExplorer.Decal; }
		// protected BaseInputDriver InputDriver { get => _GlobalExplorer.InputDriver; }
		// protected SettingsConfig Settings { get => _GlobalExplorer.Settings; }
		
		/*
		** Fetches handle from the global class
		**
		** @return Node3D
		*/
		public Node3D GetHandle() 
		{
			return _GlobalExplorer.GetHandle();
		}
	} 
}