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

using AssetSnap.Explorer;
using AssetSnap.Settings;
using Godot;

namespace AssetSnap.Core 
{
	public class CoreEnter : Core 
	{ 
		/*
		** Initializes our plugin
		**
		** @return void
		*/
		public void InitializeCore()
		{	 
			/** Initialize custom node types **/  
			new ASNode.Types.AsNodeType().Initialize();
			new ASNode.Types.AsGroupType().Initialize();
			new ASNode.Types.AsGroupedType().Initialize();
			new ASNode.Types.AsArrayModifierType().Initialize();
			new ASNode.Types.AsScatterModifierType().Initialize();
			new ASNode.Types.AsStaticBodyType().Initialize();
			new ASNode.Types.AsListSelectType().Initialize();
			new ASNode.Types.AsMeshInstanceType().Initialize();
			new ASNode.Types.AsMultiMeshInstanceType().Initialize();
			new ASNode.Types.AsOptimizedMultiMeshGroupType().Initialize();
			new ASNode.Types.AsMultiMeshType().Initialize();

			new Library.Base();
			new ContextMenu.Base();
			new Debug.Inspector();
			
			_GlobalExplorer._Plugin.FoldersLoaded += () => { _OnLoadContainers(); };

			/** Initialize **/
			_GlobalExplorer.Waypoints.Initialize();
			_GlobalExplorer.ContextMenu.Initialize();
			_GlobalExplorer.Snap.Initialize();
			
			_GlobalExplorer.Decal.Initialize();  
			_GlobalExplorer.Raycast.Initialize();
			
			// Finalize Group builder container 
			_GlobalExplorer.GroupBuilder.Initialize(); 
			 
			_GlobalExplorer.Inspector.Initialize();
			_GlobalExplorer.Inspector.AddToDock();
			
			Plugin.Singleton.AddChild(ExplorerUtils.Get().Library);
			Plugin.Singleton.AddChild(ExplorerUtils.Get().Components);
			Plugin.Singleton.AddChild(ExplorerUtils.Get().ContextMenu);
			
			_GlobalExplorer.Settings.MaybeEmitFoldersLoaded();
		} 
		
		private void _OnLoadContainers() 
		{
			if( _GlobalExplorer.Settings.Initialized ) 
			{
				_GlobalExplorer.Settings.Reset();
			}
			
			if( _GlobalExplorer.GroupBuilder.Initialized ) 
			{
				_GlobalExplorer.GroupBuilder.ClearContainer();
			}
			
			if( SettingsUtils.Get().FolderCount != 0 ) 
			{
				_GlobalExplorer.GroupBuilder.InitializeContainer();
				
				if(
					null == _GlobalExplorer.GroupBuilder.Container ||
					false == EditorPlugin.IsInstanceValid( _GlobalExplorer.GroupBuilder.Container )
				) 
				{
					GD.PushError("Invalid Group Container");
				}
			
				_GlobalExplorer.BottomDock.Add(_GlobalExplorer.GroupBuilder.Container); 
			}
			
			_GlobalExplorer.Library.Initialize();
			if( SettingsUtils.Get().FolderCount != 0 ) 
			{
				_GlobalExplorer.Settings.InitializeContainer();
				if(
					null == _GlobalExplorer.Settings.Container ||
					false == EditorPlugin.IsInstanceValid( _GlobalExplorer.Settings.Container )
				) 
				{
					GD.PushError("Invalid Settings Container");
				}
			
				_GlobalExplorer.BottomDock.Add(_GlobalExplorer.Settings.Container);
			}
		}
		
		public void dispose()
		{
			_GlobalExplorer._Plugin.FoldersLoaded -= () => { _OnLoadContainers(); };
		}
	}
}