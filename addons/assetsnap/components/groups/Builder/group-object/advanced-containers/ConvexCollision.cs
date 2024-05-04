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
using AssetSnap.Static;
using Godot;

namespace AssetSnap.Front.Components.Groups.Builder.GroupObject.AdvancedContainers
{
	/// <summary>
	/// Component representing convex collision options within a group object.
	/// </summary>
	[Tool]
	public partial class ConvexCollision : AdvancedGroupComponent
	{
		/// <summary>
		/// Constructor for ConvexCollision class.
		/// </summary>
		public ConvexCollision()
		{
			Text = "Convex Collision";
			
			_UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
				{ typeof(Labelable).ToString() },
				{ typeof(Checkable).ToString() },
			};
		}

		/// <summary>
		/// Registers traits for the ConvexCollision component.
		/// </summary>
		protected override void _RegisterTraits()
		{
			base._RegisterTraits();
		}

		/// <summary>
		/// Initializes fields and UI elements for the ConvexCollision component.
		/// </summary>
		protected override void _InitializeFields()
		{
			base._InitializeFields();
					
			Trait<Checkable>()
				.SetName("GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision")
				.SetText("Use convex collision")
				.SetValue(Options.ContainsKey("ConvexCollision") ? (bool)Options["ConvexCollision"] : false)
				.SetMargin(5, "right")
				.SetAction( Callable.From( () => { _OnUseConvexCollision(Trait<Checkable>().Select(0).GetValue()); } ))
				.Instantiate();
				
			Trait<Checkable>()
				.SetName("GroupBuilderEditorGroupObjectAdvancedContainerConvexClean")
				.SetText("Clean")
				.SetValue(Options.ContainsKey("ConvexClean") ? (bool)Options["ConvexClean"] : false)
				.SetMargin(5, "right")
				.SetAction( Callable.From( () => { _OnUseConvexClean(Trait<Checkable>().Select(1).GetValue()); } ))
				.Instantiate();
				
			Trait<Checkable>()
				.SetName("GroupBuilderEditorGroupObjectAdvancedContainerConvexSimplify")
				.SetText("Clean")
				.SetValue(Options.ContainsKey("ConvexSimplify") ? (bool)Options["ConvexSimplify"] : false)
				.SetAction( Callable.From( () => { _OnUseConvexSimplify(Trait<Checkable>().Select(2).GetValue()); } ))
				.Instantiate();
		}

		/// <summary>
		/// Finalizes UI setup for the ConvexCollision component.
		/// </summary>
		protected override void _FinalizeFields()
		{
			Godot.Container InnerContainer = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer();

			Trait<Checkable>()
				.Select(0)
				.AddToContainer(
					InnerContainer
				);

			Trait<Checkable>()
				.Select(1)
				.AddToContainer(
					InnerContainer
				);
				
			Trait<Checkable>()
				.Select(2)
				.AddToContainer(
					InnerContainer
				);

			base._FinalizeFields();			
		}

		/// <summary>
		/// Handles the action when using convex collision option.
		/// </summary>
		/// <param name="state">The state of the convex collision option.</param>
		private void _OnUseConvexCollision( bool state )
		{
			_GlobalExplorer.GroupBuilder._Editor.SetOption(Index, "ConvexCollision", state);
			HandleStatic.MaybeUpdateGroup(Index, "ConvexCollision", state);
			
			if( false == state ) 
			{
				HandleStatic.MaybeUpdateGrouped(Index, "ConvexSimplify", false);
				HandleStatic.MaybeUpdateGrouped(Index, "ConvexClean", false);
			}
			else 
			{
				HandleStatic.MaybeUpdateGrouped(Index, "SphereCollision", false);
				HandleStatic.MaybeUpdateGrouped(Index, "ConcaveCollision", false);
				
			}
			_TriggerGroupedUpdate();
		}
		
		/// <summary>
		/// Handles the action when using convex clean option.
		/// </summary>
		/// <param name="state">The state of the convex clean option.</param>
		private void _OnUseConvexClean( bool state )
		{
			_GlobalExplorer.GroupBuilder._Editor.SetOption(Index, "ConvexClean", state);
			HandleStatic.MaybeUpdateGrouped(Index, "ConvexClean", state);
			_TriggerGroupedUpdate();
		}
		
		/// <summary>
        /// Handles the action when using convex simplify option.
        /// </summary>
        /// <param name="state">The state of the convex simplify option.</param>
		private void _OnUseConvexSimplify( bool state )
		{
			_GlobalExplorer.GroupBuilder._Editor.SetOption(Index, "ConvexSimplify", state);
			HandleStatic.MaybeUpdateGrouped(Index, "ConvexSimplify", state);
			_TriggerGroupedUpdate();
		}
	}
}

#endif