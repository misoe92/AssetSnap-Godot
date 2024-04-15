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

namespace AssetSnap.Front.Components.Groups.Builder.GroupObject
{
	using System.Collections.Generic;
	using AssetSnap.Component;
	using Godot;
	using AssetSnap.Front.Components.Groups.Builder.GroupObject.AdvancedContainers;
	using Godot.Collections;

	[Tool]
	public partial class AdvancedContainer : GroupObjectComponent
	{
		private SphereCollision _GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision;
		private ConvexCollision _GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision;
		private ConcaveCollision _GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision;
		private SnapLayer _GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer;
		
		public AdvancedContainer()
		{
			UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
				{ typeof(Buttonable).ToString() },
				{ typeof(Spinboxable).ToString() },
				{ typeof(Labelable).ToString() },
			};
		}
		
		public bool IsVisible()
		{
			return Trait<Containerable>()
				.Select(0)
				.IsVisible();
		}
		
		public void ToggleVisibility()
		{
			Trait<Containerable>()
				.Select(0)
				.ToggleVisible();
		}
		
		protected override void _InitializeFields()
		{
			Trait<Containerable>()
				.SetName("AdvancedRowContainer")
				.SetMargin(0)
				.SetMargin(20, "bottom")
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetOrientation( Containerable.ContainerOrientation.Horizontal )
				.SetInnerOrientation( Containerable.ContainerOrientation.Vertical )
				.SetVisible( false )
				.Instantiate();

			Container BoxContainer = Trait<Containerable>().Select(0).GetInnerContainer();
		
			_InitializeSnapLayerControl(BoxContainer);
			_InitializeSphereCollisionControl(BoxContainer);
			_InitializeConvexCollisionControl(BoxContainer);
			_InitializeConcaveCollisionControl(BoxContainer);
		}
		
		protected override void _FinalizeFields()
		{
			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					this
				);
		}
		
		private void _InitializeSnapLayerControl(Container innerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObjects.AdvancedContainers.SnapLayer"
			};
			
			if (_GlobalExplorer.Components.HasAll( Components.ToArray() )) 
			{
				_GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer = GlobalExplorer.GetInstance().Components.Single<SnapLayer>(true);
				_GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer.Container = innerContainer;
				_GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer.Index = Index;
				_GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer.Options = Options;
				_GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer.Path = Path;
				_GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer.Initialize();
			}
		}
		
		private void _InitializeSphereCollisionControl(Container innerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObjects.AdvancedContainers.SphereCollision"
			};
			
			if (_GlobalExplorer.Components.HasAll( Components.ToArray() )) 
			{
				_GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision = GlobalExplorer.GetInstance().Components.Single<SphereCollision>(true);
				_GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision.Container = innerContainer;
				_GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision.Index = Index;
				_GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision.Options = Options;
				_GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision.Path = Path;
				_GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision.Initialize();
			}
		}
		
		private void _InitializeConvexCollisionControl(Container innerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObjects.AdvancedContainers.ConvexCollision"
			};
			
			if (_GlobalExplorer.Components.HasAll( Components.ToArray() )) 
			{
				_GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision = GlobalExplorer.GetInstance().Components.Single<ConvexCollision>(true);
				_GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision.Container = innerContainer;
				_GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision.Index = Index;
				_GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision.Options = Options;
				_GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision.Path = Path;
				_GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision.Initialize();
			}
		}
		
		private void _InitializeConcaveCollisionControl(Container innerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObjects.AdvancedContainers.ConcaveCollision"
			};
			
			if (_GlobalExplorer.Components.HasAll( Components.ToArray() )) 
			{
				_GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision = GlobalExplorer.GetInstance().Components.Single<ConcaveCollision>(true);
				_GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision.Container = innerContainer;
				_GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision.Index = Index;
				_GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision.Options = Options;
				_GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision.Path = Path;
				_GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision.Initialize();
			}
		}
	}
}