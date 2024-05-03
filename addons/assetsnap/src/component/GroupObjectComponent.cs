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

using AssetSnap.Front.Nodes;
using Godot;

namespace AssetSnap.Component
{
	/// <summary>
	/// Base class for components associated with grouped objects.
	/// </summary>
	[Tool]
	public partial class GroupObjectComponent : TraitableComponent
	{
		/// <summary>
		/// The path associated with the group object.
		/// </summary>
		public string Path = "";
		
		/// <summary>
		/// The text associated with the group object.
		/// </summary>
		protected string Text = "";

		/// <summary>
		/// The index of the group object.
		/// </summary>
		public int Index = 0;
		
		/// <summary>
		/// The options associated with the group object.
		/// </summary>
		public Godot.Collections.Dictionary<string, Variant> Options { get; set; }
	   
	   	/// <summary>
		/// Initializes the component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
		
			_RegisterTraits();
			Initiated = true;
			_InitializeFields();
			_FinalizeFields();
		}
		
		/// <summary>
		/// Registers traits associated with the component.
		/// </summary>
		protected virtual void _RegisterTraits(){}
		
		/// <summary>
		/// Initializes the fields for the component.
		/// </summary>
		protected virtual void _InitializeFields(){}
		
		/// <summary>
		/// Finalizes the fields for the component.
		/// </summary>
		protected virtual void _FinalizeFields(){}
		
		/// <summary>
        /// Triggers an update for grouped objects.
        /// </summary>
		protected void _TriggerGroupedUpdate()
		{
			string GroupPath = _GlobalExplorer.GroupBuilder._Editor.GroupPath;
			
			if( "" != GroupPath && _GlobalExplorer.States.GroupedObjects.ContainsKey(GroupPath) ) 
			{
				foreach( Node3D node in _GlobalExplorer.States.GroupedObjects[GroupPath] ) 
				{
					if( node is AsGrouped3D asGrouped3D ) 
					{
						asGrouped3D.Update();
					}
				}
			}
		}
	}
}

#endif