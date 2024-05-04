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

using System;
using System.Collections.Generic;
using System.Reflection;
using AssetSnap.Component;
using AssetSnap.Front.Components.Library.Sidebar;
using Godot;

namespace AssetSnap.Front.Components.Library
{
	/// <summary>
	/// Partial class for managing settings in the library component.
	/// </summary>
	[Tool]
	public partial class Settings : LibraryComponent
	{
		public SnapObject SnapObject;
		public Editing Editing;
		public SnapLayer SnapLayer;
		public SimpleSphereCollision SimpleSphereCollision;
		public ConvexPolygonCollision ConvexPolygonCollision;
		public ConcaveCollision ConcaveCollision;
		public SnapOffsetX SnapOffsetX;
		public SnapOffsetZ SnapOffsetZ;
		public SnapToHeight SnapToHeight;
		public SnapToX SnapToX;
		public SnapToZ SnapToZ;
		public OptimizedPlacement OptimizedPlacement;
		public SimplePlacement SimplePlacement;
		public LevelOfDetails LevelOfDetails;
		public VisibilityRange VisibilityRange;
		
		private ScrollContainer _ScrollContainer;
		private VBoxContainer _BoxContainer;
		private Label _SnapTitle;
		
		/// <summary>
		/// Constructor for the Settings class.
		/// </summary>
		public Settings()
		{
			Name = "LibrarySettings";
			
			_UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
				{ typeof(Labelable).ToString() },
				{ typeof(ScrollContainerable).ToString() },
				{ typeof(Dropdownable).ToString() },
			};
			
			//_include = false;
		} 
			
		/// <summary>
		/// Initializes the settings component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			_Initiated = true;
			
			SizeFlagsHorizontal = SizeFlags.ExpandFill;
			SizeFlagsVertical = SizeFlags.ExpandFill;

			Trait<Containerable>()
				.SetName("LibrarySettingsMain")
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetMargin(0, "top")
				.SetMargin(5, "right")
				.SetMargin(5, "left")
				.Instantiate();
				
			Trait<Dropdownable>()
				.SetName("Placing Modes")
				.SetDefaultValue("Placing Modes")
				.Instantiate(0);
				
			Trait<Dropdownable>()
				.SetName("Object Snapping")
				.SetDefaultValue("Object Snapping")
				.Instantiate(1);
				
			Trait<Dropdownable>()
				.SetName("Plane Snapping")
				.SetPadding(5, "top")
				.SetDefaultValue("Plane Snapping")
				.Instantiate(2);
				
			Trait<Dropdownable>()
				.SetName("Collisions")
				.SetDefaultValue("Collisions")
				.Instantiate(3);
				
			Trait<Dropdownable>()
				.SetName("Level of detail")
				.SetDefaultValue("Level of detail")
				.Instantiate(4);
							
			Trait<Dropdownable>()
				.SetName("Visibility Range")
				.SetDefaultValue("Visibility Range")
				.Instantiate(5);
				
			Trait<Labelable>()
				.SetName("LibrarySettingsTitle")
				.SetText("Library Controls")
				.SetType(Labelable.TitleType.HeaderMedium)
				.SetMargin(3, "top")
				.SetMargin(6, "bottom")
				.SetMargin(10, "right")
				.SetMargin(2, "left")
				.Instantiate();
				
			Trait<ScrollContainerable>()
				.SetName("LibrarySettingsScroll")
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetMinimumDimension(0, 200)
				.Instantiate();	
			
			Trait<Containerable>()
				.SetName("LibrarySettingsInner")
				.SetMargin(5)
				.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
				.Instantiate();

			Container container = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer();

			Container scrollContainerInner = Trait<ScrollContainerable>()
				.Select(0)
				.GetScrollContainer();

			Container componentContainer = Trait<Containerable>()
				.Select(1)
				.GetInnerContainer();
				
			Trait<Labelable>()
				.Select(0)
				.AddToContainer(container); 
				
			
			List<string> Components = new()
			{
				"Library.Sidebar.SnapObject",
				"Library.Sidebar.Editing",
				"Library.Sidebar.SnapLayer",
				"Library.Sidebar.SnapOffsetX",
				"Library.Sidebar.SnapOffsetZ",
				"Library.Sidebar.SnapToHeight",
				"Library.Sidebar.SnapToX",
				"Library.Sidebar.SnapToZ",
				"Library.Sidebar.SimplePlacement",
				"Library.Sidebar.OptimizedPlacement",
				"Library.Sidebar.SimpleSphereCollision",
				"Library.Sidebar.ConvexPolygonCollision",
				"Library.Sidebar.ConcaveCollision"
			};
			
			if (GlobalExplorer.GetInstance().Components.HasAll( Components.ToArray() )) 
			{
				Editing = GlobalExplorer.GetInstance().Components.Single<Editing>(true);
				SimplePlacement = GlobalExplorer.GetInstance().Components.Single<SimplePlacement>(true);
				OptimizedPlacement = GlobalExplorer.GetInstance().Components.Single<OptimizedPlacement>(true);
				
				SnapLayer = GlobalExplorer.GetInstance().Components.Single<SnapLayer>(true);
				SnapObject = GlobalExplorer.GetInstance().Components.Single<SnapObject>(true);
				SnapOffsetX = GlobalExplorer.GetInstance().Components.Single<SnapOffsetX>(true);
				SnapOffsetZ = GlobalExplorer.GetInstance().Components.Single<SnapOffsetZ>(true);
			
				SnapToHeight = GlobalExplorer.GetInstance().Components.Single<SnapToHeight>(true);
				SnapToX = GlobalExplorer.GetInstance().Components.Single<SnapToX>(true);
				SnapToZ = GlobalExplorer.GetInstance().Components.Single<SnapToZ>(true);
			
				SimpleSphereCollision = GlobalExplorer.GetInstance().Components.Single<SimpleSphereCollision>(true);
				ConvexPolygonCollision = GlobalExplorer.GetInstance().Components.Single<ConvexPolygonCollision>(true);
				ConcaveCollision = GlobalExplorer.GetInstance().Components.Single<ConcaveCollision>(true);
				
				LevelOfDetails = GlobalExplorer.GetInstance().Components.Single<LevelOfDetails>(true);
				VisibilityRange = GlobalExplorer.GetInstance().Components.Single<VisibilityRange>(true);
				
				if( Editing != null ) 
				{
					Editing.LibraryName = LibraryName;
					Editing.Initialize();
					container.AddChild(Editing);
				}
				
				if( SimplePlacement != null ) 
				{
					SimplePlacement.LibraryName = LibraryName;
					SimplePlacement.Initialize();
					Trait<Dropdownable>()
						.Select(0)
						.GetDropdownContainer()
						.AddChild(SimplePlacement);
				}
				
				if( OptimizedPlacement != null ) 
				{
					OptimizedPlacement.LibraryName = LibraryName;
					OptimizedPlacement.Initialize();

					Trait<Dropdownable>()
						.Select(0)
						.GetDropdownContainer()
						.AddChild(OptimizedPlacement);
				}
				
				
				if( SnapLayer != null ) 
				{
					SnapLayer.LibraryName = LibraryName;
					SnapLayer.Initialize();
					
					Trait<Dropdownable>()
						.Select(1)
						.GetDropdownContainer()
						.AddChild(SnapLayer);
				}
				
				if( SnapObject != null ) 
				{
					SnapObject.LibraryName = LibraryName;
					SnapObject.Initialize();
					
					Trait<Dropdownable>()
						.Select(1)
						.GetDropdownContainer()
						.AddChild(SnapObject);
				} 
				
				if( SnapOffsetX != null ) 
				{
					SnapOffsetX.LibraryName = LibraryName;
					SnapOffsetX.Initialize();
					
					Trait<Dropdownable>()
						.Select(1)
						.GetDropdownContainer()
						.AddChild(SnapOffsetX);
				}
				
				if( SnapOffsetZ != null ) 
				{
					SnapOffsetZ.LibraryName = LibraryName;
					SnapOffsetZ.Initialize();
					
					Trait<Dropdownable>()
						.Select(1)
						.GetDropdownContainer()
						.AddChild(SnapOffsetZ);
				}
				
					
				if( SnapToHeight != null ) 
				{
					SnapToHeight.LibraryName = LibraryName;
					SnapToHeight.Initialize();
					
					Trait<Dropdownable>()
						.Select(2)
						.GetDropdownContainer()
						.AddChild(SnapToHeight);
				}
				
				if( SnapToX != null ) 
				{
					SnapToX.LibraryName = LibraryName;
					SnapToX.Initialize();
					Trait<Dropdownable>()
						.Select(2)
						.GetDropdownContainer()
						.AddChild(SnapToX);
				}
				
				if( SnapToZ != null ) 
				{
					SnapToZ.LibraryName = LibraryName;
					SnapToZ.Initialize();
					Trait<Dropdownable>()
						.Select(2)
						.GetDropdownContainer()
						.AddChild(SnapToZ);
				}
				
				if( SimpleSphereCollision != null )
				{
					SimpleSphereCollision.LibraryName = LibraryName;
					SimpleSphereCollision.Initialize();
					Trait<Dropdownable>()
						.Select(3)
						.GetDropdownContainer()
						.AddChild(SimpleSphereCollision);
				}
				
				if( ConvexPolygonCollision != null ) 
				{
					ConvexPolygonCollision.LibraryName = LibraryName;
					ConvexPolygonCollision.Initialize();
					Trait<Dropdownable>()
						.Select(3)
						.GetDropdownContainer()
						.AddChild(ConvexPolygonCollision);
				}
				
				if( ConcaveCollision != null )
				{
					ConcaveCollision.LibraryName = LibraryName;
					ConcaveCollision.Initialize();
					Trait<Dropdownable>()
						.Select(3)
						.GetDropdownContainer()
						.AddChild(ConcaveCollision);
				}
				
				if( LevelOfDetails != null ) 
				{
					LevelOfDetails.LibraryName = LibraryName;
					LevelOfDetails.Initialize();
					Trait<Dropdownable>()
						.Select(4)
						.GetDropdownContainer()
						.AddChild(LevelOfDetails);
				}
				
				if( VisibilityRange != null ) 
				{
					VisibilityRange.LibraryName = LibraryName;
					VisibilityRange.Initialize();
					Trait<Dropdownable>()
						.Select(5)
						.GetDropdownContainer()
						.AddChild(VisibilityRange);
				}
			}
			
			Trait<Dropdownable>()
				.Select(5)
				.AddToContainer(componentContainer);
				
			Trait<Dropdownable>()
				.Select(4)
				.AddToContainer(componentContainer);
				
			Trait<Dropdownable>()
				.Select(3)
				.AddToContainer(componentContainer);
				
			Trait<Dropdownable>()
				.Select(2)
				.AddToContainer(componentContainer);
						
			Trait<Dropdownable>()
				.Select(1)
				.AddToContainer(componentContainer);
				
			Trait<Dropdownable>()
				.Select(0)
				.AddToContainer(componentContainer);

			Trait<Containerable>()
				.Select(1)
				.AddToContainer(scrollContainerInner);
				
			Trait<ScrollContainerable>()
				.Select(0)
				.AddToContainer(container);
				
			Trait<Containerable>()
				.Select(0)
				.AddToContainer(this);
		}
		
		/// <summary>
		/// Synchronizes the settings with their corresponding components.
		/// </summary>
		public override void Sync()
		{
			if( null != SnapObject && IsInstanceValid(SnapObject)) 
			{
				SnapObject.Sync();
			}
			if( null != Editing && IsInstanceValid(Editing)) 
			{
				Editing.Sync();
			}
			if( null != SnapLayer && IsInstanceValid(SnapLayer)) 
			{
				SnapLayer.Sync();
			}
			if( null != SnapOffsetX && IsInstanceValid(SnapOffsetX)) 
			{
				SnapOffsetX.Sync();
			}
			if( null != SnapOffsetZ && IsInstanceValid(SnapOffsetZ)) 
			{
				SnapOffsetZ.Sync();
			}
			if( null != SnapToHeight && IsInstanceValid(SnapToHeight)) 
			{
				SnapToHeight.Sync();
			}
			if( null != SnapToX && IsInstanceValid(SnapToX))
			{
				SnapToX.Sync();
			}
			if( null != SnapToZ && IsInstanceValid(SnapToZ)) 
			{
				SnapToZ.Sync();
			}
			if( null != SimpleSphereCollision && IsInstanceValid(SimpleSphereCollision)) 
			{
				SimpleSphereCollision.Sync();
			}
			if( null != ConvexPolygonCollision && IsInstanceValid(ConvexPolygonCollision)) 
			{
				ConvexPolygonCollision.Sync();
			}
			if( null != ConcaveCollision && IsInstanceValid(ConcaveCollision)) 
			{
				ConcaveCollision.Sync();
			}
		}
			
		/// <summary>
		/// Fetches a component by its key name.
		/// </summary>
		/// <param name="key">The key name of the component.</param>
		/// <returns>The fetched component.</returns>
		public BaseComponent AccessField( string key )
		{
			Type type = GetType();
			FieldInfo field = type.GetField(key);
			
			if (field != null)
			{
				object value = field.GetValue(this);
				return (BaseComponent)value;
			}

			GD.PushWarning("Field not found: ", key);
			
			return default(BaseComponent);
		}
		
		/// <summary>
		/// Clears all current setting values.
		/// </summary>
		public void ClearAll()
		{
			if( null != SnapObject && IsInstanceValid(SnapObject)) 
			{
				SnapObject.Reset();
			}
			if( null != SnapLayer && IsInstanceValid(SnapLayer)) 
			{
				SnapLayer.Reset();
			}
			
			if( null != SnapOffsetX && IsInstanceValid(SnapOffsetX)) 
			{
				SnapOffsetX.Reset();
			}
			
			if( null != SnapOffsetZ && IsInstanceValid(SnapOffsetZ)) 
			{
				SnapOffsetZ.Reset();
			}
			
			if( null != SnapToHeight && IsInstanceValid(SnapToHeight)) 
			{
				SnapToHeight.Reset(); 
			}
			
			if( null != SnapToX && IsInstanceValid(SnapToX)) 
			{
				SnapToX.Reset();
			}
			
			if( null != SnapToZ && IsInstanceValid(SnapToZ)) 
			{
				SnapToZ.Reset();
			}
			
			if( null != SimpleSphereCollision && IsInstanceValid(SimpleSphereCollision)) 
			{
				SimpleSphereCollision.Reset();
			}
			
			if( null != ConvexPolygonCollision && IsInstanceValid(ConvexPolygonCollision)) 
			{
				ConvexPolygonCollision.Reset();
			}
			
			if( null != ConcaveCollision && IsInstanceValid(ConcaveCollision)) 
			{
				ConcaveCollision.Reset();
			}
		}
		
		/// <summary>
		/// Checks if a field exists in the Settings class.
		/// </summary>
		/// <param name="fieldName">The name of the field to check.</param>
		/// <returns>True if the field exists, otherwise false.</returns>
		public bool FieldExists(string fieldName)
		{
			Type type = typeof(Settings);
			
			// Use BindingFlags.Public to include public fields
			// You can modify the BindingFlags as needed based on your requirements
			FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

			return fieldInfo != null;
		}
	}
}

#endif