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
	using Godot.Collections;

	[Tool]
	public partial class SnapLayer : AdvancedGroupComponent
	{
		public SnapLayer()
		{
			Text = "Snap Layer";
			
			UsingTraits = new()
			{
				{ typeof(Containerable).ToString() },
				{ typeof(Labelable).ToString() },
				{ typeof(Spinboxable).ToString() },
			};
		}
		
		protected override void _RegisterTraits()
		{
			base._RegisterTraits();
		}
		
		protected override void _InitializeFields()
		{
			base._InitializeFields();
			
			Trait<Spinboxable>()
				.SetName("GroupBuilderEditorGroupObjectAdvancedContainerSnapLayer")
				.SetValue(Options.ContainsKey("SnapLayer") ? (int)Options["SnapLayer"] : 0)
				.SetStep(0.1f)
				.SetMinValue(0.0f)
				.SetAction( Callable.From( ( double value ) => { _OnSnapLayerChanged((int)value); } ) )
				.Instantiate();

			Trait<Containerable>()
				.Select(0)
				.SetMargin(170, "left");
		}

		protected override void _FinalizeFields()
		{
			Container InnerContainer = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer();

			Trait<Spinboxable>()
				.Select(0)
				.AddToContainer(
					InnerContainer
				);
				
			base._FinalizeFields();
		}

		private void _OnSnapLayerChanged( int value )
		{
			GlobalExplorer.GetInstance().GroupBuilder._Editor.SetOption(Index, "SnapLayer", value);
			HandleStatic.MaybeUpdateGrouped(Index, "SnapLayer", value);
		}
	}
}