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
	public partial class SimpleSphereCollision : LSCollisionComponent
	{
		private readonly string _Title = "Simple Sphere";
		private readonly string _CheckboxTooltip = "Simple sphere collision, is fast.";

		/*
		** Constructor of the component
		** 
		** @return void
		*/
		public SimpleSphereCollision()
		{
			Name = "LSSimpleSphereCollision";
			// _include = false; 
		}

		/*
		** Initializes the component
		**
		** @return void
		*/
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
				.AddToContainer( Container );
				
			Plugin.GetInstance().StatesChanged += () => { MaybeUpdateValue(); };
		}

		/*
		** Updates collisions and spawn settings
		** of the model
		**
		** @return void
		*/
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

		/*
		** Checks if the component state
		** is active
		** 
		** @return bool
		*/
		public override bool IsActive() 
		{
			return _GlobalExplorer.States.SphereCollision == GlobalStates.LibraryStateEnum.Enabled;
		}
		
		/*
		** Resets the state back to disabled
		**
		** @return void
		*/
		public void Reset()
		{
			_GlobalExplorer.States.SphereCollision = GlobalStates.LibraryStateEnum.Disabled;
		}
		
		/*
		** Syncronizes it's value to a global
		** central state controller
		**
		** @return void
		*/
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

		public override void _ExitTree()
		{
			Initiated = false;
			base._ExitTree();
		}
	}
}