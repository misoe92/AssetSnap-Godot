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

using System.Collections.Generic;
using AssetSnap.Component;
using Godot;
using AssetSnap.Front.Components.Groups.Builder.GroupObject.AdvancedContainers;

namespace AssetSnap.Front.Components.Groups.Builder.GroupObject
{
	/// <summary>
	/// Represents an advanced container for group objects in the builder.
	/// </summary>
	[Tool]
	public partial class AdvancedContainer : GroupObjectComponent
	{
		private SphereCollision _GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision;
		private ConvexCollision _GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision;
		private ConcaveCollision _GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision;
		private SnapLayer _GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer;
		private LevelOfDetails _GroupBuilderEditorGroupObjectAdvancedContainerLevelOfDetails;

		/// <summary>
		/// Initializes a new instance of the <see cref="AdvancedContainer"/> class.
		/// </summary>
		public AdvancedContainer()
		{
			Name = "GroupsBuilderGroupObjectAdvancedContainer";

			_UsingTraits = new()
			{
				{ typeof(Buttonable).ToString() },
				{ typeof(Spinboxable).ToString() },
				{ typeof(Labelable).ToString() },
				{ typeof(Containerable).ToString() },
			};
		}

		/// <summary>
		/// Toggles the visibility of the advanced container.
		/// </summary>
		public void ToggleVisibility()
		{
			Trait<Containerable>()
				.Select(0)
				.ToggleVisible();
		}
		
		/// <summary>
		/// Checks if the advanced container is visible.
		/// </summary>
		/// <returns>True if the advanced container is visible; otherwise, false.</returns>
		public bool IsVisible()
		{
			return Trait<Containerable>()
				.Select(0)
				.IsVisible();
		}

		/// <summary>
		/// Initializes the fields of the advanced container.
		/// </summary>
		protected override void _InitializeFields()
		{
			Trait<Containerable>()
				.SetName("AdvancedRowContainer")
				.SetMargin(0)
				.SetMargin(20, "bottom")
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
				.SetOrientation(Containerable.ContainerOrientation.Horizontal)
				.SetInnerOrientation(Containerable.ContainerOrientation.Vertical)
				.SetVisible(false)
				.Instantiate();

			Godot.Container BoxContainer = Trait<Containerable>().Select(0).GetInnerContainer();

			_InitializeSnapLayerControl(BoxContainer);
			_InitializeLevelOfDetailsControl(BoxContainer);
			_InitializeSphereCollisionControl(BoxContainer);
			_InitializeConvexCollisionControl(BoxContainer);
			_InitializeConcaveCollisionControl(BoxContainer);
		}

		/// <summary>
		/// Finalizes the fields of the advanced container.
		/// </summary>
		protected override void _FinalizeFields()
		{
			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					this
				);
		}

		/// <summary>
		/// Initializes the level of details control.
		/// </summary>
		/// <param name="innerContainer">The inner container where the control will be added.</param>
		private void _InitializeLevelOfDetailsControl(Godot.Container innerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.AdvancedContainers.LevelOfDetails"
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				_GroupBuilderEditorGroupObjectAdvancedContainerLevelOfDetails = GlobalExplorer.GetInstance().Components.Single<LevelOfDetails>(true);
				_GroupBuilderEditorGroupObjectAdvancedContainerLevelOfDetails.Index = Index;
				_GroupBuilderEditorGroupObjectAdvancedContainerLevelOfDetails.Options = Options;
				_GroupBuilderEditorGroupObjectAdvancedContainerLevelOfDetails.Path = Path;
				_GroupBuilderEditorGroupObjectAdvancedContainerLevelOfDetails.Initialize();

				innerContainer.AddChild(_GroupBuilderEditorGroupObjectAdvancedContainerLevelOfDetails);
			}
		}
		
		/// <summary>
		/// Initializes the snap layer control.
		/// </summary>
		/// <param name="innerContainer">The inner container where the control will be added.</param>
		private void _InitializeSnapLayerControl(Godot.Container innerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.AdvancedContainers.SnapLayer"
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				_GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer = GlobalExplorer.GetInstance().Components.Single<SnapLayer>(true);
				_GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer.Index = Index;
				_GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer.Options = Options;
				_GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer.Path = Path;
				_GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer.Initialize();
				innerContainer.AddChild(_GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer);
			}
		}

		/// <summary>
		/// Initializes the sphere collision control.
		/// </summary>
		/// <param name="innerContainer">The inner container where the control will be added.</param>
		private void _InitializeSphereCollisionControl(Godot.Container innerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.AdvancedContainers.SphereCollision"
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				_GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision = GlobalExplorer.GetInstance().Components.Single<SphereCollision>(true);
				_GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision.Index = Index;
				_GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision.Options = Options;
				_GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision.Path = Path;
				_GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision.Initialize();
				innerContainer.AddChild(_GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision);
			}
		}

		/// <summary>
		/// Initializes the convex collision control.
		/// </summary>
		/// <param name="innerContainer">The inner container where the control will be added.</param>
		private void _InitializeConvexCollisionControl(Godot.Container innerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.AdvancedContainers.ConvexCollision"
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				_GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision = GlobalExplorer.GetInstance().Components.Single<ConvexCollision>(true);
				_GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision.Index = Index;
				_GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision.Options = Options;
				_GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision.Path = Path;
				_GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision.Initialize();
				innerContainer.AddChild(_GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision);
			}
		}

		/// <summary>
		/// Initializes the concave collision control.
		/// </summary>
		/// <param name="innerContainer">The inner container where the control will be added.</param>
		private void _InitializeConcaveCollisionControl(Godot.Container innerContainer)
		{
			List<string> Components = new()
			{
				"Groups.Builder.GroupObject.AdvancedContainers.ConcaveCollision"
			};

			if (_GlobalExplorer.Components.HasAll(Components.ToArray()))
			{
				_GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision = GlobalExplorer.GetInstance().Components.Single<ConcaveCollision>(true);
				_GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision.Index = Index;
				_GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision.Options = Options;
				_GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision.Path = Path;
				_GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision.Initialize();
				innerContainer.AddChild(_GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision);
			}
		}
	}
}

#endif