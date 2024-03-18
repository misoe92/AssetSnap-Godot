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

namespace AssetSnap.Front.Components
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using AssetSnap.Component;
	using Godot;

	public partial class LibrarySettings : LibraryComponent
	{
		private ScrollContainer _ScrollContainer;
		private VBoxContainer _BoxContainer;
		private Label _SnapTitle;
		
		public LSSnapObject _LSSnapObject;
		public LSEditing _LSEditing;
		public LSSnapLayer _LSSnapLayer;
		public LSCollisionTitle _LSCollisionTitle;
		public LSSimpleSphereCollision _LSSimpleSphereCollision;
		public LSConvexPolygonCollision _LSConvexPolygonCollision;
		public LSConcaveCollision _LSConcaveCollision;
		public LSSnapOffsetX _LSSnapOffsetX;
		public LSSnapOffsetZ _LSSnapOffsetZ;
		public LSSnapToHeight _LSSnapToHeight;
		public LSSnapToX _LSSnapToX;
		public LSSnapToZ _LSSnapToZ;

			
		/*
		** Component constructor
		**
		** @return void
		*/
		public LibrarySettings()
		{
			Name = "LibrarySettings";
			// _include = false;
		} 
			
		/*
		** Initializes the settings component
		**
		** @return void
		*/
		public override void Initialize()
		{
			if( Container is VBoxContainer OuterContainer ) 
			{
				_ScrollContainer = new()
				{
					SizeFlagsVertical = Control.SizeFlags.ExpandFill,
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				};
				
				_BoxContainer = new()
				{
					SizeFlagsVertical = Control.SizeFlags.ExpandFill,
					SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				};

				_SnapTitle = new()
				{
					ThemeTypeVariation = "HeaderMedium",
					Text = "Object Snapping",
				};

				_BoxContainer.AddChild(_SnapTitle);
				_ScrollContainer.AddChild(_BoxContainer);
				OuterContainer.AddChild(_ScrollContainer);
				
				List<string> Components = new()
				{
					"LSSnapObject",
					"LSEditing",
					"LSSnapLayer",
					"LSSnapOffsetX",
					"LSSnapOffsetZ",
					"LSSnapToHeight",
					"LSSnapToX",
					"LSSnapToZ",
					"LSCollisionTitle",
					"LSSimpleSphereCollision",
					"LSConvexPolygonCollision",
					"LSConcaveCollision"
				};
				
				if (GlobalExplorer.GetInstance().Components.HasAll( Components.ToArray() )) 
				{
					_LSSnapObject = GlobalExplorer.GetInstance().Components.Single<LSSnapObject>(true);
					_LSSnapLayer = GlobalExplorer.GetInstance().Components.Single<LSSnapLayer>(true);
					_LSEditing = GlobalExplorer.GetInstance().Components.Single<LSEditing>(true);
					_LSSnapOffsetX = GlobalExplorer.GetInstance().Components.Single<LSSnapOffsetX>(true);
					_LSSnapOffsetZ = GlobalExplorer.GetInstance().Components.Single<LSSnapOffsetZ>(true);
					_LSSnapToHeight = GlobalExplorer.GetInstance().Components.Single<LSSnapToHeight>(true);
					_LSSnapToX = GlobalExplorer.GetInstance().Components.Single<LSSnapToX>(true);
					_LSSnapToZ = GlobalExplorer.GetInstance().Components.Single<LSSnapToZ>(true);
					_LSCollisionTitle = GlobalExplorer.GetInstance().Components.Single<LSCollisionTitle>(true);
					_LSSimpleSphereCollision = GlobalExplorer.GetInstance().Components.Single<LSSimpleSphereCollision>(true);
					_LSConvexPolygonCollision = GlobalExplorer.GetInstance().Components.Single<LSConvexPolygonCollision>(true);
					_LSConcaveCollision = GlobalExplorer.GetInstance().Components.Single<LSConcaveCollision>(true);
					
					if( _LSEditing != null ) 
					{
						_LSEditing.Container = _BoxContainer;
						_LSEditing.Library = Library;
						_LSEditing.Initialize();
					}
					
					if( _LSSnapLayer != null ) 
					{
						_LSSnapLayer.Container = _BoxContainer;
						_LSSnapLayer.Library = Library;
						_LSSnapLayer.Initialize();
					}
					
					if( _LSSnapObject != null ) 
					{
						_LSSnapObject.Container = _BoxContainer;
						_LSSnapObject.Library = Library;
						_LSSnapObject.Initialize();
					}
					
					if( _LSSnapOffsetX != null ) 
					{
						_LSSnapOffsetX.Container = _BoxContainer;
						_LSSnapOffsetX.Library = Library;
						_LSSnapOffsetX.Initialize();
					}
					
					if( _LSSnapOffsetZ != null ) 
					{
						_LSSnapOffsetZ.Container = _BoxContainer;
						_LSSnapOffsetZ.Library = Library;
						_LSSnapOffsetZ.Initialize();
					}
					
					if( _LSSnapToHeight != null ) 
					{
						_LSSnapToHeight.Container = _BoxContainer;
						_LSSnapToHeight.Library = Library;
						_LSSnapToHeight.Initialize();
					}
					
					if( _LSSnapToX != null ) 
					{
						_LSSnapToX.Container = _BoxContainer;
						_LSSnapToX.Library = Library;
						_LSSnapToX.Initialize();
					}
					
					if( _LSSnapToZ != null ) 
					{
						_LSSnapToZ.Container = _BoxContainer;
						_LSSnapToZ.Library = Library;
						_LSSnapToZ.Initialize();
					}
					
					if( _LSCollisionTitle != null )
					{
						_LSCollisionTitle.Container = _BoxContainer;
						_LSCollisionTitle.Library = Library;
						_LSCollisionTitle.Initialize();
					}
					
					if( _LSSimpleSphereCollision != null )
					{
						_LSSimpleSphereCollision.Container = _BoxContainer;
						_LSSimpleSphereCollision.Library = Library;
						_LSSimpleSphereCollision.Initialize();
					}
					
					if( _LSConvexPolygonCollision != null ) 
					{
						_LSConvexPolygonCollision.Container = _BoxContainer;
						_LSConvexPolygonCollision.Library = Library;
						_LSConvexPolygonCollision.Initialize();
					}
					
					if( _LSConcaveCollision != null )
					{
						_LSConcaveCollision.Container = _BoxContainer;
						_LSConcaveCollision.Library = Library;
						_LSConcaveCollision.Initialize();
					}
				}
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
			
			return default(BaseComponent);
		}
		
		/*
		** Clears all current setting values
		**
		** @return void
		*/
		public void ClearAll()
		{
			if( IsInstanceValid(_LSSnapObject)) 
			{
				_LSSnapObject.Reset();
			}
			if( IsInstanceValid(_LSSnapLayer)) 
			{
				_LSSnapLayer.Reset();
			}
			
			if( IsInstanceValid(_LSSnapOffsetX)) 
			{
				_LSSnapOffsetX.Reset();
			}
			
			if( IsInstanceValid(_LSSnapOffsetZ)) 
			{
				_LSSnapOffsetZ.Reset();
			}
			
			if( IsInstanceValid(_LSSnapToHeight)) 
			{
				_LSSnapToHeight.Reset(); 
			}
			
			if( IsInstanceValid(_LSSnapToX)) 
			{
				_LSSnapToX.Reset();
			}
			
			if( IsInstanceValid(_LSSnapToZ)) 
			{
				_LSSnapToZ.Reset();
			}
			
			if( IsInstanceValid(_LSSimpleSphereCollision)) 
			{
				_LSSimpleSphereCollision.Reset();
			}
			
			if( IsInstanceValid(_LSConvexPolygonCollision)) 
			{
				_LSConvexPolygonCollision.Reset();
			}
			
			if( IsInstanceValid(_LSConcaveCollision)) 
			{
				_LSConcaveCollision.Reset();
			}
		}
		
	
		public bool FieldExists(string fieldName)
		{
			Type type = typeof(LibrarySettings);
			
			// Use BindingFlags.Public to include public fields
			// You can modify the BindingFlags as needed based on your requirements
			FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

			return fieldInfo != null;
		}
		
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			if( IsInstanceValid(_SnapTitle) ) 
			{
				_SnapTitle.QueueFree();
				_SnapTitle = null;
			}
			
			if( IsInstanceValid(_BoxContainer) ) 
			{
				_BoxContainer.QueueFree();
				_BoxContainer = null;
			}
			
			if( IsInstanceValid(_ScrollContainer) ) 
			{
				_ScrollContainer.QueueFree();
				_ScrollContainer = null;
			}
		}
	}
}