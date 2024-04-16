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

namespace AssetSnap.Front.Components.Groups.Builder.GroupObject.AdvancedContainers
{
	using AssetSnap.Component;
	using AssetSnap.Static;
	using Godot;

	[Tool]
	public partial class ConvexCollision : AdvancedGroupComponent
	{
		public ConvexCollision()
		{
			Text = "Convex Collision";
			
			UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
				{ typeof(Labelable).ToString() },
				{ typeof(Checkable).ToString() },
			};
		}

		protected override void _RegisterTraits()
		{
			base._RegisterTraits();
		}

		
		protected override void _InitializeFields()
		{
			base._InitializeFields();
					
			Trait<Checkable>()
				.SetName("GroupBuilderEditorGroupObjectAdvancedContainerConvexCollision")
				.SetText("Use convex collision")
				.SetValue(Options.ContainsKey("ConvexCollision") ? (bool)Options["ConvexCollision"] : false)
				.SetAction( Callable.From( () => { _OnUseConvexCollision(Trait<Checkable>().Select(0).GetValue()); } ))
				.Instantiate();
				
			Trait<Checkable>()
				.SetName("GroupBuilderEditorGroupObjectAdvancedContainerConvexClean")
				.SetText("Clean")
				.SetValue(Options.ContainsKey("ConvexClean") ? (bool)Options["ConvexClean"] : false)
				.SetAction( Callable.From( () => { _OnUseConvexClean(Trait<Checkable>().Select(1).GetValue()); } ))
				.Instantiate();
				
			Trait<Checkable>()
				.SetName("GroupBuilderEditorGroupObjectAdvancedContainerConvexSimplify")
				.SetText("Clean")
				.SetValue(Options.ContainsKey("ConvexSimplify") ? (bool)Options["ConvexSimplify"] : false)
				.SetAction( Callable.From( () => { _OnUseConvexSimplify(Trait<Checkable>().Select(2).GetValue()); } ))
				.Instantiate();
		}

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
		
		private void _OnUseConvexClean( bool state )
		{
			_GlobalExplorer.GroupBuilder._Editor.SetOption(Index, "ConvexClean", state);
			HandleStatic.MaybeUpdateGrouped(Index, "ConvexClean", state);
			_TriggerGroupedUpdate();
		}
		
		private void _OnUseConvexSimplify( bool state )
		{
			_GlobalExplorer.GroupBuilder._Editor.SetOption(Index, "ConvexSimplify", state);
			HandleStatic.MaybeUpdateGrouped(Index, "ConvexSimplify", state);
			_TriggerGroupedUpdate();
		}
	}
}