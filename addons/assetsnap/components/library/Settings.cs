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
		private ScrollContainer _ScrollContainer;
		private VBoxContainer _BoxContainer;
		private Label _SnapTitle;
		public SnapObject _LSSnapObject;
		public Editing _LSEditing;
		public SnapLayer _LSSnapLayer;
		public SimpleSphereCollision _LSSimpleSphereCollision;
		public ConvexPolygonCollision _LSConvexPolygonCollision;
		public ConcaveCollision _LSConcaveCollision;
		public SnapOffsetX _LSSnapOffsetX;
		public SnapOffsetZ _LSSnapOffsetZ;
		public SnapToHeight _LSSnapToHeight;
		public SnapToX _LSSnapToX;
		public SnapToZ _LSSnapToZ;
		public OptimizedPlacement _LSOptimizedPlacement;
		public SimplePlacement _LSSimplePlacement;
		public LevelOfDetails _LSLevelOfDetails;
		public VisibilityRange _LSVisibilityRange;
		
		/// <summary>
		/// Constructor for the Settings class.
		/// </summary>
		public Settings()
		{
			Name = "LibrarySettings";
			
			UsingTraits = new()
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
			Initiated = true;
			
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
				_LSEditing = GlobalExplorer.GetInstance().Components.Single<Editing>(true);
				_LSSimplePlacement = GlobalExplorer.GetInstance().Components.Single<SimplePlacement>(true);
				_LSOptimizedPlacement = GlobalExplorer.GetInstance().Components.Single<OptimizedPlacement>(true);
				
				_LSSnapLayer = GlobalExplorer.GetInstance().Components.Single<SnapLayer>(true);
				_LSSnapObject = GlobalExplorer.GetInstance().Components.Single<SnapObject>(true);
				_LSSnapOffsetX = GlobalExplorer.GetInstance().Components.Single<SnapOffsetX>(true);
				_LSSnapOffsetZ = GlobalExplorer.GetInstance().Components.Single<SnapOffsetZ>(true);
			
				_LSSnapToHeight = GlobalExplorer.GetInstance().Components.Single<SnapToHeight>(true);
				_LSSnapToX = GlobalExplorer.GetInstance().Components.Single<SnapToX>(true);
				_LSSnapToZ = GlobalExplorer.GetInstance().Components.Single<SnapToZ>(true);
			
				_LSSimpleSphereCollision = GlobalExplorer.GetInstance().Components.Single<SimpleSphereCollision>(true);
				_LSConvexPolygonCollision = GlobalExplorer.GetInstance().Components.Single<ConvexPolygonCollision>(true);
				_LSConcaveCollision = GlobalExplorer.GetInstance().Components.Single<ConcaveCollision>(true);
				
				_LSLevelOfDetails = GlobalExplorer.GetInstance().Components.Single<LevelOfDetails>(true);
				_LSVisibilityRange = GlobalExplorer.GetInstance().Components.Single<VisibilityRange>(true);
				
				if( _LSEditing != null ) 
				{
					_LSEditing.LibraryName = LibraryName;
					_LSEditing.Initialize();
					container.AddChild(_LSEditing);
				}
				
				if( _LSSimplePlacement != null ) 
				{
					_LSSimplePlacement.LibraryName = LibraryName;
					_LSSimplePlacement.Initialize();
					Trait<Dropdownable>()
						.Select(0)
						.GetDropdownContainer()
						.AddChild(_LSSimplePlacement);
				}
				
				if( _LSOptimizedPlacement != null ) 
				{
					_LSOptimizedPlacement.LibraryName = LibraryName;
					_LSOptimizedPlacement.Initialize();

					Trait<Dropdownable>()
						.Select(0)
						.GetDropdownContainer()
						.AddChild(_LSOptimizedPlacement);
				}
				
				
				if( _LSSnapLayer != null ) 
				{
					_LSSnapLayer.LibraryName = LibraryName;
					_LSSnapLayer.Initialize();
					
					Trait<Dropdownable>()
						.Select(1)
						.GetDropdownContainer()
						.AddChild(_LSSnapLayer);
				}
				
				if( _LSSnapObject != null ) 
				{
					_LSSnapObject.LibraryName = LibraryName;
					_LSSnapObject.Initialize();
					
					Trait<Dropdownable>()
						.Select(1)
						.GetDropdownContainer()
						.AddChild(_LSSnapObject);
				} 
				
				if( _LSSnapOffsetX != null ) 
				{
					_LSSnapOffsetX.LibraryName = LibraryName;
					_LSSnapOffsetX.Initialize();
					
					Trait<Dropdownable>()
						.Select(1)
						.GetDropdownContainer()
						.AddChild(_LSSnapOffsetX);
				}
				
				if( _LSSnapOffsetZ != null ) 
				{
					_LSSnapOffsetZ.LibraryName = LibraryName;
					_LSSnapOffsetZ.Initialize();
					
					Trait<Dropdownable>()
						.Select(1)
						.GetDropdownContainer()
						.AddChild(_LSSnapOffsetZ);
				}
				
					
				if( _LSSnapToHeight != null ) 
				{
					_LSSnapToHeight.LibraryName = LibraryName;
					_LSSnapToHeight.Initialize();
					
					Trait<Dropdownable>()
						.Select(2)
						.GetDropdownContainer()
						.AddChild(_LSSnapToHeight);
				}
				
				if( _LSSnapToX != null ) 
				{
					_LSSnapToX.LibraryName = LibraryName;
					_LSSnapToX.Initialize();
					Trait<Dropdownable>()
						.Select(2)
						.GetDropdownContainer()
						.AddChild(_LSSnapToX);
				}
				
				if( _LSSnapToZ != null ) 
				{
					_LSSnapToZ.LibraryName = LibraryName;
					_LSSnapToZ.Initialize();
					Trait<Dropdownable>()
						.Select(2)
						.GetDropdownContainer()
						.AddChild(_LSSnapToZ);
				}
				
				if( _LSSimpleSphereCollision != null )
				{
					_LSSimpleSphereCollision.LibraryName = LibraryName;
					_LSSimpleSphereCollision.Initialize();
					Trait<Dropdownable>()
						.Select(3)
						.GetDropdownContainer()
						.AddChild(_LSSimpleSphereCollision);
				}
				
				if( _LSConvexPolygonCollision != null ) 
				{
					_LSConvexPolygonCollision.LibraryName = LibraryName;
					_LSConvexPolygonCollision.Initialize();
					Trait<Dropdownable>()
						.Select(3)
						.GetDropdownContainer()
						.AddChild(_LSConvexPolygonCollision);
				}
				
				if( _LSConcaveCollision != null )
				{
					_LSConcaveCollision.LibraryName = LibraryName;
					_LSConcaveCollision.Initialize();
					Trait<Dropdownable>()
						.Select(3)
						.GetDropdownContainer()
						.AddChild(_LSConcaveCollision);
				}
				
				if( _LSLevelOfDetails != null ) 
				{
					_LSLevelOfDetails.LibraryName = LibraryName;
					_LSLevelOfDetails.Initialize();
					Trait<Dropdownable>()
						.Select(4)
						.GetDropdownContainer()
						.AddChild(_LSLevelOfDetails);
				}
				
				if( _LSVisibilityRange != null ) 
				{
					_LSVisibilityRange.LibraryName = LibraryName;
					_LSVisibilityRange.Initialize();
					Trait<Dropdownable>()
						.Select(5)
						.GetDropdownContainer()
						.AddChild(_LSVisibilityRange);
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
			if( null != _LSSnapObject && IsInstanceValid(_LSSnapObject)) 
			{
				_LSSnapObject.Sync();
			}
			if( null != _LSEditing && IsInstanceValid(_LSEditing)) 
			{
				_LSEditing.Sync();
			}
			if( null != _LSSnapLayer && IsInstanceValid(_LSSnapLayer)) 
			{
				_LSSnapLayer.Sync();
			}
			if( null != _LSSnapOffsetX && IsInstanceValid(_LSSnapOffsetX)) 
			{
				_LSSnapOffsetX.Sync();
			}
			if( null != _LSSnapOffsetZ && IsInstanceValid(_LSSnapOffsetZ)) 
			{
				_LSSnapOffsetZ.Sync();
			}
			if( null != _LSSnapToHeight && IsInstanceValid(_LSSnapToHeight)) 
			{
				_LSSnapToHeight.Sync();
			}
			if( null != _LSSnapToX && IsInstanceValid(_LSSnapToX))
			{
				_LSSnapToX.Sync();
			}
			if( null != _LSSnapToZ && IsInstanceValid(_LSSnapToZ)) 
			{
				_LSSnapToZ.Sync();
			}
			if( null != _LSSimpleSphereCollision && IsInstanceValid(_LSSimpleSphereCollision)) 
			{
				_LSSimpleSphereCollision.Sync();
			}
			if( null != _LSConvexPolygonCollision && IsInstanceValid(_LSConvexPolygonCollision)) 
			{
				_LSConvexPolygonCollision.Sync();
			}
			if( null != _LSConcaveCollision && IsInstanceValid(_LSConcaveCollision)) 
			{
				_LSConcaveCollision.Sync();
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
			if( null != _LSSnapObject && IsInstanceValid(_LSSnapObject)) 
			{
				_LSSnapObject.Reset();
			}
			if( null != _LSSnapObject && IsInstanceValid(_LSSnapLayer)) 
			{
				_LSSnapLayer.Reset();
			}
			
			if( null != _LSSnapObject && IsInstanceValid(_LSSnapOffsetX)) 
			{
				_LSSnapOffsetX.Reset();
			}
			
			if( null != _LSSnapObject && IsInstanceValid(_LSSnapOffsetZ)) 
			{
				_LSSnapOffsetZ.Reset();
			}
			
			if( null != _LSSnapObject && IsInstanceValid(_LSSnapToHeight)) 
			{
				_LSSnapToHeight.Reset(); 
			}
			
			if( null != _LSSnapObject && IsInstanceValid(_LSSnapToX)) 
			{
				_LSSnapToX.Reset();
			}
			
			if( null != _LSSnapObject && IsInstanceValid(_LSSnapToZ)) 
			{
				_LSSnapToZ.Reset();
			}
			
			if( null != _LSSnapObject && IsInstanceValid(_LSSimpleSphereCollision)) 
			{
				_LSSimpleSphereCollision.Reset();
			}
			
			if( null != _LSSnapObject && IsInstanceValid(_LSConvexPolygonCollision)) 
			{
				_LSConvexPolygonCollision.Reset();
			}
			
			if( null != _LSSnapObject && IsInstanceValid(_LSConcaveCollision)) 
			{
				_LSConcaveCollision.Reset();
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