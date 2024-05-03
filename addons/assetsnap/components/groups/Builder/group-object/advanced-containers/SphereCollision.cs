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
	/// A component representing sphere collision behavior within a group object.
	/// </summary>
	[Tool]
	public partial class SphereCollision : AdvancedGroupComponent
	{
		/// <summary>
		/// Constructor for the SphereCollision component.
		/// </summary>
		public SphereCollision()
		{
			Text = "Sphere Collision";
			
			UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
				{ typeof(Labelable).ToString() },
				{ typeof(Checkable).ToString() },
			};
		}
		
		/// <summary>
		/// Registers traits specific to the SphereCollision component.
		/// </summary>
		protected override void _RegisterTraits()
		{
			base._RegisterTraits();
		}
		
		/// <summary>
		/// Initializes fields and UI elements specific to the SphereCollision component.
		/// </summary>
		protected override void _InitializeFields()
		{
			base._InitializeFields();
		
			Trait<Checkable>()
				.SetName("GroupBuilderEditorGroupObjectAdvancedContainerSphereCollision")
				.SetText("Use sphere collision")
				.SetValue(Options.ContainsKey("SphereCollision") ? (bool)Options["SphereCollision"] : false)
				.SetAction(Callable.From( () => { _OnUseSphereCollision(Trait<Checkable>().Select(0).GetValue()); } ))
				.Instantiate();
		}

		/// <summary>
		/// Finalizes fields and UI elements specific to the SphereCollision component.
		/// </summary>
		protected override void _FinalizeFields()
		{
			Container InnerContainer = Trait<Containerable>()
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
        /// Handles changes in the sphere collision state.
        /// </summary>
        /// <param name="state">The new state of sphere collision.</param>
		private void _OnUseSphereCollision( bool state )
		{
			_GlobalExplorer.GroupBuilder._Editor.SetOption(Index, "SphereCollision", state);
			HandleStatic.MaybeUpdateGrouped(Index, "SphereCollision", state);
			
			if( true == state ) 
			{
				HandleStatic.MaybeUpdateGrouped(Index, "ConcaveCollision", false);
				HandleStatic.MaybeUpdateGrouped(Index, "ConvexCollision", false);
				HandleStatic.MaybeUpdateGrouped(Index, "ConvexClean", false);
				HandleStatic.MaybeUpdateGrouped(Index, "ConvexSimplify", false);
			}
			
			_TriggerGroupedUpdate();
		}
	}
}

#endif