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
	/// Represents a component for convex polygon collision handling.
	/// </summary>
	[Tool]
	public partial class ConvexPolygonCollision : LSCollisionComponent
	{
		private readonly string _Title = "Convex Polygon";
		private readonly string _cleanTitle = "Clean";
		private readonly string _simplifyTitle = "Simplify";
		private readonly string _CheckboxTooltip = "Use with caution, since this method is more expensive than a simple collision shape.";
		private readonly string _cleanCheckboxTooltip = "If clean is true (default), duplicate and interior vertices are removed automatically. You can set it to false to make the process faster if not needed.";
		private readonly string _simplifyCheckboxTooltip = "If simplify is true, the geometry can be further simplified to reduce the number of vertices. Disabled by default.";
		private bool _Exited = false;

		/// <summary>
		/// Constructor of the component.
		/// </summary>
		public ConvexPolygonCollision()
		{
			Name = "LSConvexPolygonCollision";
			
			_UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};
			
			//_include = false;
		} 
		
		/// <summary>
		/// Initializes the component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });
			Callable _cleanCallable = Callable.From(() => { _OnCleanCheckboxPressed(); });
			Callable _simplifyCallable = Callable.From(() => { _OnSimplifyCheckboxPressed(); });
			
			_Initiated = true;
			
			Trait<Checkable>()
				.SetName("ConvexCollision")
				.SetAction( _callable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(2, "top")
				.SetMargin(0, "bottom")
				.SetText(_Title)
				.SetTooltipText(_CheckboxTooltip)
				.Instantiate();
				
			Trait<Checkable>()
				.SetName("ConvexClean")
				.SetAction( _cleanCallable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(2, "top")
				.SetMargin(0, "bottom")
				.SetText(_cleanTitle)
				.SetTooltipText(_cleanCheckboxTooltip)
				.SetVisible(false)
				.Instantiate();
					
			Trait<Checkable>()
				.SetName("ConvexSimplify")
				.SetAction( _simplifyCallable )
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(2, "top")
				.SetMargin(0, "bottom")
				.SetText(_simplifyTitle)
				.SetTooltipText(_simplifyCheckboxTooltip)
				.SetVisible(false)
				.Instantiate();

			Trait<Checkable>()
				.Select(0)
				.AddToContainer( this );
								
			Trait<Checkable>()
				.Select(1)
				.AddToContainer( this );
								
			Trait<Checkable>()
				.Select(2)
				.AddToContainer( this );
				
			Plugin.GetInstance().StatesChanged += (Godot.Collections.Array data) => { MaybeUpdateValue(data); };
		}

		/// <summary>
		/// Synchronizes the state of various checkboxes with their internal state.
		/// </summary>
		public override void MaybeUpdateValue(Godot.Collections.Array data)
		{
			if( data[0].As<string>() == "ConvexCollision" || data[0].As<string>() == "ConvexClean" || data[0].As<string>() == "ConvexSimplify" ) 
			{
				if(
					false == IsValid()
				) 
				{
					return;
				}
				
				if( _IsActive() ) 
				{
					SetCleanCheckboxVisibility(true);
					SetSimplifyCheckboxVisibility(true);
				}
				else
				{
					SetCleanCheckboxVisibility(false);
					SetSimplifyCheckboxVisibility(false);
				}
				
				if( _IsCleanActive() && false == _CleanCheckboxChecked() ) 
				{
					SetCleanCheckboxState(true);
				}
				else if( false == _IsCleanActive() && true == _CleanCheckboxChecked() ) 
				{
					SetCleanCheckboxState(false);
				}
				
				if( _IsSimplifyActive() && false == _SimplifyCheckboxChecked() ) 
				{
					SetSimplifyCheckboxState(true);
				}
				else if( false == _IsSimplifyActive() && true == _SimplifyCheckboxChecked() ) 
				{
					SetSimplifyCheckboxState(false);
				}
				
				base.MaybeUpdateValue(data);
			}
		}
		
		/// <summary>
		/// Sets the state of the clean checkbox.
		/// </summary>
		public void SetCleanCheckboxState( bool state ) 
		{
			if( false == IsValid() )
			{
				return;
			}
			
			Trait<Checkable>().Select(1).SetValue( state );
		}
		
		/// <summary>
		/// Sets the state of the simplify checkbox.
		/// </summary>
		public void SetSimplifyCheckboxState( bool state ) 
		{
			if( false == IsValid() )
			{
				return;
			}
			
			Trait<Checkable>().Select(2).SetValue( state );
		}
		
		/// <summary>
		/// Sets the visibility of the clean checkbox.
		/// </summary>
		public void SetCleanCheckboxVisibility( bool state ) 
		{
			if( false == IsValid() )
			{
				return;
			}
			
			Trait<Checkable>().Select(1).SetVisible( state );
		}
		
		/// <summary>
		/// Sets the visibility of the simplify checkbox.
		/// </summary>
		public void SetSimplifyCheckboxVisibility( bool state )
		{
			if( false == IsValid() )
			{
				return;
			}
			
			Trait<Checkable>().Select(2).SetVisible( state );
		}
		
		/// <summary>
        /// Resets the state back to disabled.
        /// </summary>
		public void Reset()
		{
			StatesUtils.Get().ConvexCollision = GlobalStates.LibraryStateEnum.Disabled; 
			StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Disabled; 
			StatesUtils.Get().ConvexSimplify = GlobalStates.LibraryStateEnum.Disabled; 
		}
			
		/// <summary>
        /// Syncronizes its value to a global central state controller.
        /// </summary>
		public override void Sync() 
		{
			if( true == _Initiated && Trait<Checkable>().Select(0).IsValid() )
			{
				StatesUtils.Get().ConvexCollision = Trait<Checkable>().Select(0).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
			}
			
			if( true == _Initiated && Trait<Checkable>().Select(1).IsValid() )
			{
				StatesUtils.Get().ConvexClean = Trait<Checkable>().Select(1).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
			}
			
			if( true == _Initiated && Trait<Checkable>().Select(2).IsValid() )
			{
				StatesUtils.Get().ConvexSimplify = Trait<Checkable>().Select(2).GetValue() ? GlobalStates.LibraryStateEnum.Enabled : GlobalStates.LibraryStateEnum.Disabled;
			}
		}
		
		/// <summary>
		/// Handles checkbox press event.
		/// </summary>
		private void _OnCheckboxPressed()
		{
			Node3D handle = _GlobalExplorer.GetHandle();

			if( false == _IsActive() ) 
			{
				StatesUtils.Get().ConvexCollision = GlobalStates.LibraryStateEnum.Enabled;
				StatesUtils.Get().SphereCollision = GlobalStates.LibraryStateEnum.Disabled;
				StatesUtils.Get().ConcaveCollision = GlobalStates.LibraryStateEnum.Disabled;
								
				UpdateSpawnSettings("ConvexCollision", true);
				UpdateSpawnSettings("SphereCollision", false);
				UpdateSpawnSettings("ConcaveCollision", false);
				UpdateSpawnSettings("ConvexClean", false);
				UpdateSpawnSettings("ConvexSimplify", false);
			}
			else
			{
				StatesUtils.Get().ConvexCollision = GlobalStates.LibraryStateEnum.Disabled;
				StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Disabled;
				StatesUtils.Get().ConvexSimplify = GlobalStates.LibraryStateEnum.Disabled;
										
				UpdateSpawnSettings("ConvexCollision", false);
				UpdateSpawnSettings("ConvexClean", false);
				UpdateSpawnSettings("ConvexSimplify", false);
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
		/// Handles clean checkbox press event.
		/// </summary>
		private void _OnCleanCheckboxPressed()
		{
			bool state = false; 
			
			if( false == _IsCleanActive() ) 
			{
				StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Enabled;
				state = true;
			}
			else 
			{
				StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Disabled;
			}
			
			Node3D handle = _GlobalExplorer.GetHandle();
			if( handle is AssetSnap.Front.Nodes.AsMeshInstance3D meshInstance3D ) 
			{
				if( meshInstance3D.GetParent() is AsStaticBody3D staticBody3D )
				{
					staticBody3D.UpdateCollision();
				}
			}
			
			string key = "_LSConvexPolygonCollisionClean.state";
			UpdateSpawnSettings(key, state);
		}
							
		/// <summary>
		/// Handles simplify checkbox press event.
		/// </summary>
		private void _OnSimplifyCheckboxPressed()
		{
			bool state = false; 
			
			if( false == _IsSimplifyActive() ) 
			{
				StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Enabled;
				state = true;
			}
			else 
			{
				StatesUtils.Get().ConvexClean = GlobalStates.LibraryStateEnum.Disabled;
			}
			
			Node3D handle = _GlobalExplorer.GetHandle();
			if( handle is AssetSnap.Front.Nodes.AsMeshInstance3D meshInstance3D ) 
			{
				if( meshInstance3D.GetParent() is AsStaticBody3D staticBody3D )
				{
					staticBody3D.UpdateCollision();
				}
			}
			
			string key = "_LSConvexPolygonCollisionSimplify.state";
			UpdateSpawnSettings(key, state);
		}
		
		/// <summary>
		/// Checks if the simplify checkbox is active.
		/// </summary>
		private bool _IsSimplifyActive()
		{
			return StatesUtils.Get().ConvexSimplify == GlobalStates.LibraryStateEnum.Enabled;
		}
		
		/// <summary>
		/// Checks if the clean checkbox is active.
		/// </summary>
		private bool _IsCleanActive()
		{
			return StatesUtils.Get().ConvexClean == GlobalStates.LibraryStateEnum.Enabled;
		}
		
		/// <summary>
		/// Checks if the collision should use the ConvexPolygon collision.
		/// </summary>
		protected override bool _IsActive() 
		{
			return StatesUtils.Get().ConvexCollision == GlobalStates.LibraryStateEnum.Enabled;
		}
		
		/// <summary>
		/// Checks if the clean checkbox is checked.
		/// </summary>
		private bool _CleanCheckboxChecked()
		{
			if( false == IsValid() )
			{
				return false;
			}
			
			return Trait<Checkable>().Select(1).GetValue();
		}
		
		/// <summary>
		/// Checks if the clean checkbox is visible.
		/// </summary>	
		private bool _CleanCheckboxVisible()
		{
			if( false == IsValid() )
			{
				return false;
			}
			
			return Trait<Checkable>().Select(1).IsVisible();
		}
		
		/// <summary>
		/// Checks if the simplify checkbox is checked.
		/// </summary>
		private bool _SimplifyCheckboxChecked()
		{
			if( false == IsValid() )
			{
				return false;
			}

			return Trait<Checkable>().Select(2).GetValue();
		}
		
		/// <summary>
		/// Checks if the simplify checkbox is visible.
		/// </summary>
		private bool _SimplifyCheckboxVisible()
		{
			if( false == IsValid() )
			{
				return false;
			}
			
			return Trait<Checkable>().Select(2).IsVisible();
		}
	}
}

#endif