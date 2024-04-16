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

namespace AssetSnap.Front.Components.Library.Sidebar
{
	using AssetSnap.Component;
	using AssetSnap.Front.Nodes;
	using Godot;

	[Tool]
	public partial class ConcaveCollision : LSCollisionComponent
	{
		private readonly string _Title = "Concave Polygon";
		private readonly string _CheckboxTooltip = "Use with caution, since this method is more expensive than a simple collision shape.";

		/*
		** Constructor of component
		**
		** @return void
		*/
		public ConcaveCollision()
		{
			Name = "LSConcaveCollision";
			
			UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
			
			//_include = false;
		}
		
		/*
		** Initialization of component
		**
		** @return void
		*/
		public override void Initialize()
		{
			base.Initialize();
			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });
			Initiated = true;
			
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
				
			Plugin.GetInstance().StatesChanged += () => { MaybeUpdateValue(); };
		}
				
		/*
		** Updates spawn settings, collisions
		** and more on checkbox pressed event
		**
		** @return void
		*/
		private void _OnCheckboxPressed()
		{
			Node3D handle = _GlobalExplorer.GetHandle();

			if( false == IsActive() ) 
			{
				_GlobalExplorer.States.ConcaveCollision = GlobalStates.LibraryStateEnum.Enabled;
				_GlobalExplorer.States.SphereCollision = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.ConvexCollision = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.ConvexClean = GlobalStates.LibraryStateEnum.Disabled;
				_GlobalExplorer.States.ConvexSimplify = GlobalStates.LibraryStateEnum.Disabled;
												
				UpdateSpawnSettings("ConcaveCollision", true);
				UpdateSpawnSettings("SphereCollision", false);
				UpdateSpawnSettings("ConvexCollision", false);
				UpdateSpawnSettings("ConvexClean", false);
				UpdateSpawnSettings("ConvexSimplify", false);
			}
			else 
			{
				_GlobalExplorer.States.ConcaveCollision = GlobalStates.LibraryStateEnum.Disabled;
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
				
		/*
		** Checks if the component state
		** is active
		** 
		** @return bool
		*/
		public override bool IsActive() 
		{
			return _GlobalExplorer.States.ConcaveCollision == GlobalStates.LibraryStateEnum.Enabled;
		}
				
		/*
		** Resets the state back to disabled
		**
		** @return void
		*/
		public void Reset()
		{
			_GlobalExplorer.States.ConcaveCollision = GlobalStates.LibraryStateEnum.Disabled;
		}
		
		/*
		** Syncronizes it's value to a global
		** central state controller
		**
		** @return void
		*/
		public override void Sync() 
		{
			if( false == IsValid() )
			{
				return;
			}
			
			_GlobalExplorer.States.ConcaveCollision = Trait<Checkable>().Select(0).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
		}
		
		public override void _ExitTree()
		{
			Initiated = false;
			
			base._ExitTree();
		}
	}
}