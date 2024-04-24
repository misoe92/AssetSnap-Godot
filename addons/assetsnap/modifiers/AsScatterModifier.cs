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
	using System;
	using AssetSnap.Explorer;
	using AssetSnap.Front.Nodes;
	using AssetSnap.States;

	using Godot;

	public partial class AsScatterModifier
	{
		public string Name = "AsScatterModifier";

		/*
		** Applies the modifier
		** 
		** @return async Task<bool>
		*/
		public bool Apply()
		{
			if (HasHandle() == false)
			{
				return false;
			}

			ApplySimple();

			return true;
		}

		/*
		** Applies simple modifier, without a parenting
		** static body
		** 
		** @return async Task<bool>
		*/
		public bool ApplySimple()
		{
			Node3D Handle = GetHandle() as Node3D;

			if (null == Handle)
			{
				GD.PushWarning("No handle was found");
				return false;
			}

			if (null == Handle.GetTree())
			{
				GD.PushWarning("No tree was found");
				return false;
			}

			if (null == Handle.GetTree().EditedSceneRoot)
			{
				GD.PushWarning("No scene root was found");
				return false;
			}

			AsScatterModifier3D Group = new()
			{
				ScatterName = "ScatterModifier-" + Handle.GetTree().EditedSceneRoot.GetChildCount(),
				DuplicateType = Handle is AsMeshInstance3D ? "AsMeshInstance3D" : "AsNode3D",
				Duplicates = Handle,
			};

			Handle.GetParent().RemoveChild(Handle);
			ExplorerUtils.Get().Waypoints.Remove(Handle, Handle.Transform.Origin);
			ExplorerUtils.Get().Waypoints.Spawn(Group, Handle.Transform.Origin, Handle.RotationDegrees, Handle.Scale);

			Node3D GroupInstance = ExplorerUtils.Get().Waypoints.GetWorkingNode();
			ExplorerUtils.Get().SetFocusToNode(GroupInstance);

			return true;
		}

		/*
		** Fetches the current handle
		** 
		** @return Node
		*/
		public Node GetHandle()
		{
			return StatesUtils.Get().EditingObject;
		}

		/*
		** Checks if handle exists
		** 
		** @return bool
		*/
		public bool HasHandle()
		{
			Node3D Handle = StatesUtils.Get().EditingObject;

			if (Handle == null)
			{
				return false;
			}

			return true;
		}
	}
}