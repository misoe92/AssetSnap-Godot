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
	using Godot;

	public partial class AsArrayModifier
	{
		public string Name = "AsArrayModifier";
		
		public bool Apply()
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
				ApplySimple();
			}

			return true;
		}
		
		
		public bool ApplySimple()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			AssetSnap.Front.Nodes.AsMeshInstance3D Handle = _GlobalExplorer.HandleNode as AssetSnap.Front.Nodes.AsMeshInstance3D;
			AsArrayModifier3D Group = new()
			{
				Name = "ArrayModifier-" + Handle.GetTree().EditedSceneRoot.GetChildCount(),
				Mesh = Handle.Mesh,
				InstanceLibrary = Handle.GetLibraryName(),
				InstanceScale = Handle.Scale,
				InstanceTransform = Handle.Transform,
				InstanceRotation = Handle.RotationDegrees,
			};
			
			_GlobalExplorer.Waypoints.Spawn(Group, Handle.Transform.Origin, Handle.RotationDegrees, Handle.Scale);
			Node3D GroupInstance = _GlobalExplorer.Waypoints.GetWorkingNode();
			_GlobalExplorer.Waypoints.Remove(Handle, Handle.Transform.Origin);
			
			Handle.GetParent().RemoveChild(Handle);
			
			_GlobalExplorer.SetFocusToNode(GroupInstance);
			
			return true;
		}
		
		public bool ApplyWithBody()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			AssetSnap.Front.Nodes.AsMeshInstance3D Handle = _GlobalExplorer.HandleNode as AssetSnap.Front.Nodes.AsMeshInstance3D;
			
			if( Handle == null || Handle.GetParent() == null ) 
			{
				GD.PushWarning("No valid handle to apply with");
				return false;
			}
			
			AsArrayModifier3D Group = new()
			{
				Name = "ArrayModifier-" + Handle.GetTree().EditedSceneRoot.GetChildCount(),
				Mesh = Handle.Mesh,
				InstanceLibrary = Handle.GetLibraryName(),
				InstanceScale = Handle.Scale,
				InstanceTransform = Handle.Transform,
				InstanceRotation = Handle.RotationDegrees,
			};
			
			Transform3D Transform = Handle.GetParent<StaticBody3D>().Transform;
			_GlobalExplorer.Waypoints.Remove(Handle, Handle.Transform.Origin);
			_GlobalExplorer.Waypoints.Spawn(Group, Handle.Transform.Origin, Handle.RotationDegrees, Handle.Scale);
			Node3D GroupInstance = _GlobalExplorer.Waypoints.GetWorkingNode();
			Handle.GetParent().GetParent().RemoveChild(Handle.GetParent());
			
			GroupInstance.Transform = Transform;
			GroupInstance.ForceUpdateTransform();
			
			_GlobalExplorer.SetFocusToNode(GroupInstance);

			Group.Update();
			
			return true;
		}
		
		
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
		
		public Node GetHandle()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			AssetSnap.Front.Nodes.AsMeshInstance3D Handle = _GlobalExplorer.HandleNode as AssetSnap.Front.Nodes.AsMeshInstance3D;
			
			return Handle;
		}
		
		public bool IsAsBody( Node Node)
		{
			return Node.HasMeta( "AsBody" ) == true;
		}
	}
}