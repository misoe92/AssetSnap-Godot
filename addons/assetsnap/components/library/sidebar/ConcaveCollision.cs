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

using AssetSnap.Component;
using AssetSnap.Front.Nodes;
using AssetSnap.States;
using Godot;

namespace AssetSnap.Front.Components.Library.Sidebar
{
	/// <summary>
	/// A component representing a concave collision shape.
	/// </summary>
	[Tool]
	public partial class ConcaveCollision : LSCollisionComponent
	{
		private readonly string _Title = "Concave Polygon";
		private readonly string _CheckboxTooltip = "Use with caution, since this method is more expensive than a simple collision shape.";

		/// <summary>
		/// Constructor of the ConcaveCollision component.
		/// </summary>
		public ConcaveCollision()
		{
			Name = "LSConcaveCollision";
			
			_UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
			
			//_include = false;
		}
		
		/// <summary>
		/// Initializes the ConcaveCollision component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });
			_Initiated = true;
			
			Trait<Checkable>()
				.SetName("ConcaveCollision")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(7, "top")
				.SetMargin(10, "bottom")
				.SetText(_Title)
				.SetTooltipText(_CheckboxTooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer( this );
				
			Plugin.GetInstance().StatesChanged += (Godot.Collections.Array data) => { MaybeUpdateValue(data); };
		}

		/// <summary>
		/// Synchronizes the state of various checkboxes with their internal state.
		/// </summary>
		/// <param name="data">An array containing data related to the state change.</param>
		public override void MaybeUpdateValue(Godot.Collections.Array data)
		{
			if (data[0].As<string>() == "ConcaveCollision")
			{
				base.MaybeUpdateValue(data);
			}
		}
		
		/// <summary>
		/// Resets the state back to disabled.
		/// </summary>
		public void Reset()
		{
			StatesUtils.Get().ConcaveCollision = GlobalStates.LibraryStateEnum.Disabled;
		}
		
		/// <summary>
		/// Synchronizes its value to a global central state controller.
		/// </summary>
		public override void Sync() 
		{
			if( false == IsValid() )
			{
				return;
			}
			
			StatesUtils.Get().ConcaveCollision = Trait<Checkable>().Select(0).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
		}
		
		/// <summary>
		/// Overrides the _ExitTree method of the parent class.
		/// </summary>
		public override void _ExitTree()
		{
			_Initiated = false;
			
			base._ExitTree();
		}
				
		/// <summary>
		/// Updates spawn settings, collisions, and more on checkbox pressed event.
		/// </summary>
		private void _OnCheckboxPressed()
		{
			Node3D handle = _GlobalExplorer.GetHandle();

			if( false == _IsActive() ) 
			{
				StatesUtils.Get().ConcaveCollision = GlobalStates.LibraryStateEnum.Enabled;
				StatesUtils.Get().SphereCollision = GlobalStates.LibraryStateEnum.Disabled;
				StatesUtils.Get().ConvexCollision = GlobalStates.LibraryStateEnum.Disabled;
				StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Disabled;
				StatesUtils.Get().ConvexSimplify = GlobalStates.LibraryStateEnum.Disabled;
												
				UpdateSpawnSettings("ConcaveCollision", true);
				UpdateSpawnSettings("SphereCollision", false);
				UpdateSpawnSettings("ConvexCollision", false);
				UpdateSpawnSettings("ConvexClean", false);
				UpdateSpawnSettings("ConvexSimplify", false);
			}
			else 
			{
				StatesUtils.Get().ConcaveCollision = GlobalStates.LibraryStateEnum.Disabled;
				UpdateSpawnSettings("ConcaveCollision", false);
			}
			
			if( handle is AssetSnap.Front.Nodes.AsMeshInstance3D meshInstance3D ) 
			{
				if( meshInstance3D.GetParent() is AsStaticBody3D staticBody3D )
				{
					staticBody3D.UpdateCollision();
				}
			}
		}
				
		/// <summary>
		/// Checks if the component state is active.
		/// </summary>
		/// <returns>True if the component is active, otherwise false.</returns>
		protected override bool _IsActive() 
		{
			return StatesUtils.Get().ConcaveCollision == GlobalStates.LibraryStateEnum.Enabled;
		}
	}
}

#endif