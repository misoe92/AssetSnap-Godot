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

using AssetSnap.Config;
using AssetSnap.Front.Nodes;
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
			
			// Adding base components to the tree
			_GlobalExplorer._Plugin.AddChild(_GlobalExplorer.Settings);
			_GlobalExplorer._Plugin.AddChild(_GlobalExplorer.Waypoints);
			_GlobalExplorer._Plugin.AddChild(_GlobalExplorer.ContextMenu);
			_GlobalExplorer._Plugin.AddChild(_GlobalExplorer.BottomDock);
			_GlobalExplorer._Plugin.AddChild(_GlobalExplorer.Inspector);
 
			_GlobalExplorer._Plugin.AddChild(_GlobalExplorer.Decal);
			_GlobalExplorer._Plugin.AddChild(_GlobalExplorer.Raycast);

			_GlobalExplorer._Plugin.AddChild(_GlobalExplorer.Library); 
			_GlobalExplorer._Plugin.AddChild(_GlobalExplorer.Modifiers);
 
			_GlobalExplorer._Plugin.AddChild(_GlobalExplorer.GroupBuilder);

			_GlobalExplorer.Settings.FoldersLoaded += () => { LoadContainers(); };
 
			/** Initialize **/  
			_GlobalExplorer.Settings.Initialize();
			_GlobalExplorer.Components.Initialize();
			_GlobalExplorer.Waypoints.Initialize();
			_GlobalExplorer.BottomDock.Initialize();  
			_GlobalExplorer.ContextMenu.Initialize(); 
			_GlobalExplorer.Snap.Initialize();
			 
			_GlobalExplorer.Decal.Initialize();  
			_GlobalExplorer.Raycast.Initialize();  
			
			// Finalize Group builder container  
			_GlobalExplorer.GroupBuilder.Initialize();
			
			_GlobalExplorer.Inspector.Initialize();
			_GlobalExplorer.Inspector.AddToDock();
			
			_GlobalExplorer.BottomDock.AddToBottomPanel();

			_GlobalExplorer.Settings.MaybeEmitFoldersLoaded();
		}
		
		private void LoadContainers()
		{
			GD.Print("Loading containers");
			_GlobalExplorer.GroupBuilder.InitializeContainer();
			if(
				null == _GlobalExplorer.GroupBuilder.Container ||
				false == EditorPlugin.IsInstanceValid( _GlobalExplorer.GroupBuilder.Container )
			) 
			{
				GD.PushError("Invalid Group Container");
			}
			
			_GlobalExplorer.BottomDock.Add(_GlobalExplorer.GroupBuilder.Container); 
		
			_GlobalExplorer.Library.Initialize();
		
			_GlobalExplorer.Settings.InitializeContainer();
			if(
				null == _GlobalExplorer.Settings.Container ||
				false == EditorPlugin.IsInstanceValid( _GlobalExplorer.Settings.Container )
			) 
			{
				GD.PushError("Invalid Group Container");
			}
			
			_GlobalExplorer.BottomDock.Add(_GlobalExplorer.Settings.Container);
		}
	}
}