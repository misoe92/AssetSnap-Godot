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
namespace AssetSnap.Front.Components.Library
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using AssetSnap.Component;
    using AssetSnap.Front.Components.Library.Sidebar;
    using Godot;
	
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
			
		/*
		** Component constructor
		**
		** @return void
		*/
		public Settings()
		{
			Name = "LibrarySettings";
			//_include = false;
		} 
			
		/*
		** Initializes the settings component
		**
		** @return void
		*/
		public override void Initialize()
		{
			base.Initialize();
			AddTrait(typeof(Containerable));
			AddTrait(typeof(Labelable));
			AddTrait(typeof(ScrollContainerable));
			AddTrait(typeof(Dropdownable));
			Initiated = true;
			
			if( Container is VBoxContainer OuterContainer ) 
			{
				Trait<Containerable>()
					.SetName("LibrarySettingsMain")
					.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
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
			
				Trait<Labelable>()
					.SetName("LibrarySettingsTitle")
					.SetText("Library Controls")
					.SetType(Labelable.TitleType.HeaderMedium)
					.SetMargin(6, "top")
					.SetMargin(7, "bottom")
					.SetMargin(10, "right")
					.SetMargin(10, "left")
					.Instantiate();
					
				Trait<ScrollContainerable>()
					.SetName("LibrarySettingsScroll")
					.SetVerticalSizeFlags(Control.SizeFlags.ExpandFill)
					.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
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
					
					if( _LSEditing != null ) 
					{
						_LSEditing.Container = container;
						_LSEditing.Library = Library;
						_LSEditing.Initialize();
					}
					
					if( _LSSimplePlacement != null ) 
					{
						_LSSimplePlacement.Container = Trait<Dropdownable>()
							.Select(0)
							.GetDropdownContainer();
							
						_LSSimplePlacement.Library = Library;
						_LSSimplePlacement.Initialize();
					}
					
					if( _LSOptimizedPlacement != null ) 
					{
						_LSOptimizedPlacement.Container = Trait<Dropdownable>()
							.Select(0)
							.GetDropdownContainer();
							
						_LSOptimizedPlacement.Library = Library;
						_LSOptimizedPlacement.Initialize();
					}
					
					
					if( _LSSnapLayer != null ) 
					{
						_LSSnapLayer.Container = Trait<Dropdownable>()
							.Select(1)
							.GetDropdownContainer();
							
						_LSSnapLayer.Library = Library;
						_LSSnapLayer.Initialize();
					}
					
					if( _LSSnapObject != null ) 
					{
						_LSSnapObject.Container = Trait<Dropdownable>()
							.Select(1)
							.GetDropdownContainer();
							
						_LSSnapObject.Library = Library;
						_LSSnapObject.Initialize();
					} 
					
					if( _LSSnapOffsetX != null ) 
					{
						_LSSnapOffsetX.Container = Trait<Dropdownable>()
							.Select(1)
							.GetDropdownContainer();
							
						_LSSnapOffsetX.Library = Library;
						_LSSnapOffsetX.Initialize();
					}
					
					if( _LSSnapOffsetZ != null ) 
					{
						_LSSnapOffsetZ.Container = Trait<Dropdownable>()
							.Select(1)
							.GetDropdownContainer();
							
						_LSSnapOffsetZ.Library = Library;
						_LSSnapOffsetZ.Initialize();
					}
					
						
					if( _LSSnapToHeight != null ) 
					{
						_LSSnapToHeight.Container = Trait<Dropdownable>()
							.Select(2)
							.GetDropdownContainer();
							
						_LSSnapToHeight.Library = Library;
						_LSSnapToHeight.Initialize();
					}
					
					if( _LSSnapToX != null ) 
					{
						_LSSnapToX.Container = Trait<Dropdownable>()
							.Select(2)
							.GetDropdownContainer();
							
						_LSSnapToX.Library = Library;
						_LSSnapToX.Initialize();
					}
					
					if( _LSSnapToZ != null ) 
					{
						_LSSnapToZ.Container = Trait<Dropdownable>()
							.Select(2)
							.GetDropdownContainer();
							
						_LSSnapToZ.Library = Library;
						_LSSnapToZ.Initialize();
					}
					
					if( _LSSimpleSphereCollision != null )
					{
						_LSSimpleSphereCollision.Container = Trait<Dropdownable>()
							.Select(3)
							.GetDropdownContainer();

						_LSSimpleSphereCollision.Library = Library;
						_LSSimpleSphereCollision.Initialize();
					}
					
					if( _LSConvexPolygonCollision != null ) 
					{
						_LSConvexPolygonCollision.Container = Trait<Dropdownable>()
							.Select(3)
							.GetDropdownContainer();
							
						_LSConvexPolygonCollision.Library = Library;
						_LSConvexPolygonCollision.Initialize();
					}
					
					if( _LSConcaveCollision != null )
					{
						_LSConcaveCollision.Container = Trait<Dropdownable>()
							.Select(3)
							.GetDropdownContainer();
							
						_LSConcaveCollision.Library = Library;
						_LSConcaveCollision.Initialize();
					}
				}
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
					.AddToContainer(OuterContainer);
			}
		}
		
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
			
		/*
		** Fetches a component by it's key name
		**
		** @return BaseComponent
		*/
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
		
		/*
		** Clears all current setting values
		**
		** @return void
		*/
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
		
	
		public bool FieldExists(string fieldName)
		{
			Type type = typeof(Settings);
			
			// Use BindingFlags.Public to include public fields
			// You can modify the BindingFlags as needed based on your requirements
			FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

			return fieldInfo != null;
		}

		public override void _ExitTree()
		{
			// if(null != _LSSnapObject && IsInstanceValid(_LSSnapObject)) 
			// {
			// 	_LSSnapObject.Free();
			// }
			// if(null != _LSEditing && IsInstanceValid(_LSEditing)) 
			// {
			// 	_LSEditing.Free();
			// }
			// if(null != _LSSnapLayer && IsInstanceValid(_LSSnapLayer)) 
			// {
			// 	_LSSnapLayer.Free();
			// }
			// if(null != _LSSimpleSphereCollision && IsInstanceValid(_LSSimpleSphereCollision)) 
			// {
			// 	_LSSimpleSphereCollision.Free();
			// }
			// if(null != _LSConvexPolygonCollision && IsInstanceValid(_LSConvexPolygonCollision)) 
			// {
			// 	_LSConvexPolygonCollision.Free();
			// }
			// if(null != _LSConcaveCollision && IsInstanceValid(_LSConcaveCollision)) 
			// {
			// 	_LSConcaveCollision.Free();
			// }
			// if(null != _LSSnapOffsetX && IsInstanceValid(_LSSnapOffsetX)) 
			// {
			// 	_LSSnapOffsetX.Free();
			// }
			// if(null != _LSSnapOffsetZ && IsInstanceValid(_LSSnapOffsetZ)) 
			// {
			// 	_LSSnapOffsetZ.Free();
			// }
			// if(null != _LSSnapToHeight && IsInstanceValid(_LSSnapToHeight)) 
			// {
			// 	_LSSnapToHeight.Free();
			// }
			// if(null != _LSSnapToX && IsInstanceValid(_LSSnapToX)) 
			// {
			// 	_LSSnapToX.Free();
			// }
			// if(null != _LSSnapToZ && IsInstanceValid(_LSSnapToZ)) 
			// {
			// 	_LSSnapToZ.Free();
			// }
			// if(null != _LSOptimizedPlacement && IsInstanceValid(_LSOptimizedPlacement)) 
			// {
			// 	_LSOptimizedPlacement.Free();
			// }
			// if(null != _LSSimplePlacement && IsInstanceValid(_LSSimplePlacement)) 
			// {
			// 	_LSSimplePlacement.Free();
			// }
			
			base._ExitTree();
		}
	}
}
#endif