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
	/// A component representing concave collision behavior for an advanced group object.
	/// </summary>
	[Tool]
	public partial class ConcaveCollision : AdvancedGroupComponent
	{
		/// <summary>
		/// Constructor for the ConcaveCollision class.
		/// </summary>
		public ConcaveCollision()
		{
			Text = "Concave Collision";
			
			UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
				{ typeof(Labelable).ToString() },
				{ typeof(Checkable).ToString() },
			};
		}
		
		/// <summary>
		/// Registers traits required for the concave collision component.
		/// </summary>
		protected override void _RegisterTraits()
		{
			base._RegisterTraits();
		}
		
		/// <summary>
		/// Initializes fields for the concave collision component.
		/// </summary>
		protected override void _InitializeFields()
		{
			base._InitializeFields();
		
			Trait<Checkable>()
				.SetName("GroupBuilderEditorGroupObjectAdvancedContainerConcaveCollision")
				.SetText("Use concave collision")
				.SetValue(Options.ContainsKey("ConcaveCollision") ? (bool)Options["ConcaveCollision"] : false)
				.SetAction(Callable.From(() => { _OnUseConcaveCollision(Trait<Checkable>().Select(0).GetValue()); }))
				.Instantiate();
		}

		/// <summary>
		/// Finalizes fields for the concave collision component.
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
		
			base._FinalizeFields();
		}

		/// <summary>
        /// Handles the action when using concave collision.
        /// </summary>
        /// <param name="state">The state indicating whether concave collision is enabled or not.</param>
		private void _OnUseConcaveCollision( bool state )
		{
			_GlobalExplorer.GroupBuilder._Editor.SetOption(Index, "ConcaveCollision", state);
			HandleStatic.MaybeUpdateGrouped(Index, "ConcaveCollision", state); 
			
			if( state ) 
			{
				HandleStatic.MaybeUpdateGrouped(Index, "SphereCollision", false); 
				HandleStatic.MaybeUpdateGrouped(Index, "ConvexCollision", false); 
				HandleStatic.MaybeUpdateGrouped(Index, "ConvexClean", false); 
				HandleStatic.MaybeUpdateGrouped(Index, "ConvexSimplify", false); 
			}
			
			_TriggerGroupedUpdate();
		}
	}
}

#endif