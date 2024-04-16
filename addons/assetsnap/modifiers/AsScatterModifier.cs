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

namespace AssetSnap.Front.Modifiers
{
	using System.Threading.Tasks;
	using Godot;

	public partial class AsScatterModifier
	{
		public string Name = "AsScatterModifier";
		
		/*
		** Applies the modifier
		** 
		** @return async Task<bool>
		*/
		public async Task<bool> Apply()
		{
			if( HasHandle() == false )
			{
				return false;
			}

			if( IsAsBody(GetHandle().GetParent())) 
			{
				ApplyWithBody();
			}
			else 
			{
				await ApplySimple();
			}
			
			return true;
		}
		
		/*
		** Applies simple modifier, without a parenting
		** static body
		** 
		** @return async Task<bool>
		*/
		public async Task<bool> ApplySimple()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			AssetSnap.Front.Nodes.AsMeshInstance3D Handle = _GlobalExplorer.HandleNode as AssetSnap.Front.Nodes.AsMeshInstance3D;
			
			AssetSnap.Front.Nodes.AsMeshInstance3D Duplication = Handle.Duplicate(15) as AssetSnap.Front.Nodes.AsMeshInstance3D;
			
			AsScatterModifier3D Group = new()
			{
				Name = "ScatterModifier-" + Handle.GetTree().EditedSceneRoot.GetChildCount(),
				Mesh = Handle.Mesh,
				InstanceLibrary = Handle.GetLibraryName(),
				InstanceScale = Handle.Scale,
				InstanceTransform = Handle.Transform,
				InstanceRotation = Handle.RotationDegrees,
			};
			
			if( Duplication == null ) 
			{
				return false;
			}

			Group.Transform = Duplication.Transform;

			Transform3D Trans = Duplication.Transform;
			Trans.Origin = new Vector3(0, 0, 0);
			Duplication.Transform = Trans;

			Group.Name = "ScatterModifier-" + Handle.GetTree().EditedSceneRoot.GetChildCount();
			
			_GlobalExplorer.Waypoints.Spawn(Group, Handle.Transform.Origin, Handle.RotationDegrees, Handle.Scale);
			_GlobalExplorer.Waypoints.Remove(Handle, Handle.Transform.Origin);
			
			Handle.GetParent().RemoveChild(Handle);
			
			await _GlobalExplorer._Plugin.ToSignal(_GlobalExplorer._Plugin.GetTree(), SceneTree.SignalName.ProcessFrame);
			_GlobalExplorer.SetFocusToNode(Group);
			
			return true;
		}
		
		/*
		** Applies modifier, with a parenting
		** static body
		** 
		** @return bool
		*/
		public bool ApplyWithBody()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			AssetSnap.Front.Nodes.AsMeshInstance3D Handle = _GlobalExplorer.HandleNode as AssetSnap.Front.Nodes.AsMeshInstance3D;
			
			AsScatterModifier3D Group = new()
			{
				Name = "ScatterModifier-" + Handle.GetTree().EditedSceneRoot.GetChildCount(),
				Mesh = Handle.Mesh,
				InstanceLibrary = Handle.GetLibraryName(),
				InstanceScale = Handle.Scale,
				InstanceTransform = Handle.Transform,
				InstanceRotation = Handle.RotationDegrees,
			};
			
			Transform3D Transform = Handle.GetParent<StaticBody3D>().Transform;
			_GlobalExplorer.Waypoints.Spawn(Group, Handle.Transform.Origin, Handle.RotationDegrees, Handle.Scale);
			Node3D InstancedGroup = _GlobalExplorer.Waypoints.GetWorkingNode();
			
			_GlobalExplorer.Waypoints.Remove(Handle, Handle.Transform.Origin);
			Handle.GetParent().GetParent().RemoveChild(Handle.GetParent());
			
			_GlobalExplorer.SetFocusToNode(InstancedGroup);
			
			InstancedGroup.Transform = Transform;
			InstancedGroup.ForceUpdateTransform();
			
			return true;
		}
		
		/*
		** Fetches the current handle
		** 
		** @return Node
		*/
		public Node GetHandle()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			AssetSnap.Front.Nodes.AsMeshInstance3D Handle = _GlobalExplorer.HandleNode as AssetSnap.Front.Nodes.AsMeshInstance3D;
			
			return Handle;
		}
		
		/*
		** Checks if handle exists
		** 
		** @return bool
		*/
		public bool HasHandle()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			AssetSnap.Front.Nodes.AsMeshInstance3D Handle = _GlobalExplorer.HandleNode as AssetSnap.Front.Nodes.AsMeshInstance3D;
			
			if( Handle == null ) 
			{
				return false;
			}

			return true;
		}
		
		/*
		** Checks if given node is a AsStaticBody3D
		** 
		** @return bool
		*/
		public bool IsAsBody( Node Node)
		{
			return Node.HasMeta( "AsBody" ) == true;
		}
	}
}