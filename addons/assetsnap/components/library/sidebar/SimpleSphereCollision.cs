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
using Godot;

namespace AssetSnap.Front.Components.Library.Sidebar
{
	/// <summary>
    /// Represents a simple sphere collision component.
    /// </summary>
	[Tool]
	public partial class SimpleSphereCollision : LSCollisionComponent
	{
		private readonly string _Title = "Simple Sphere";
		private readonly string _CheckboxTooltip = "Simple sphere collision, is fast.";

		/// <summary>
        /// Constructor of the component.
        /// </summary>
		public SimpleSphereCollision()
		{
			Name = "LSSimpleSphereCollision";
			
			UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
			
			// _include = false; 
		}
		
		/// <summary>
        /// Initializes the component.
        /// </summary>
		public override void Initialize()
		{
			base.Initialize();
			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });
			Initiated = true;
			
			Trait<Checkable>()
				.SetName("SnapObjectCheckbox")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(2, "top")
				.SetMargin(7, "bottom")
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
        /// <param name="data">An array containing the data to update.</param>
		public override void MaybeUpdateValue(Godot.Collections.Array data)
		{
			if (data[0].As<string>() == "SphereCollision")
			{
				base.MaybeUpdateValue(data);
			}
		}

		/// <summary>
        /// Updates collisions and spawn settings of the model.
        /// </summary>
		private void _OnCheckboxPressed()
		{
			if( false == Initiated ) 
			{
				return;
			}
			
			Node3D handle = _GlobalExplorer.GetHandle();
			
			if( false == IsActive() ) 
			{
				_GlobalExplorer.States.SphereCollision = GlobalStates.LibraryStateEnum.Enabled;
				_GlobalExplorer.States.ConcaveCollision = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.ConvexCollision = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.ConvexClean = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.ConvexSimplify = GlobalStates.LibraryStateEnum.Disabled;
				
				UpdateSpawnSettings("SphereCollision", true);
				UpdateSpawnSettings("ConcaveCollision", false);
				UpdateSpawnSettings("ConvexCollision", false);
				UpdateSpawnSettings("ConvexClean", false);
				UpdateSpawnSettings("ConvexSimplify", false);
			}
			else
			{
				_GlobalExplorer.States.SphereCollision = GlobalStates.LibraryStateEnum.Disabled;
				
				UpdateSpawnSettings("SphereCollision", false);
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
        /// <returns>True if the component state is active, otherwise false.</returns>
		public override bool IsActive() 
		{
			return _GlobalExplorer.States.SphereCollision == GlobalStates.LibraryStateEnum.Enabled;
		}
		
		/// <summary>
        /// Resets the state back to disabled.
        /// </summary>
		public void Reset()
		{
			_GlobalExplorer.States.SphereCollision = GlobalStates.LibraryStateEnum.Disabled;
		}
		
		/// <summary>
        /// Syncronizes its value to a global central state controller.
        /// </summary>
		public override void Sync() 
		{
			if(
				false == IsValid()
			) 
			{
				return;
			}
			
			_GlobalExplorer.States.SphereCollision = Trait<Checkable>().Select(0).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
		}
	}
}

#endif