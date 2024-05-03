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
using Godot;

namespace AssetSnap.Front.Components.Library.Sidebar
{
	/// <summary>
	/// Component for simple object placement.
	/// </summary>
	[Tool]
	public partial class SimplePlacement : CheckableComponent
	{
		private readonly string _Title = "Simple placement";
		private readonly string _CheckboxTooltip = "Can be used for single object scripts and when you only need few objects";

		/// <summary>
		/// Constructor of the SimplePlacement component.
		/// </summary>
		public SimplePlacement()
		{
			Name = "LSSimplePlacement";

			UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};

			//_include = false;
		}

		/// <summary>
		/// Initializes the SimplePlacement component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });

			Initiated = true;

			Trait<Checkable>()
				.SetName("SimplePlacementCheckbox")
				.SetAction(_callable)
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(5, "top")
				.SetMargin(7, "bottom")
				.SetText(_Title)
				.SetTooltipText(_CheckboxTooltip)
				.SetValue(IsActive())
				.Instantiate()
				.Select(0)
				.AddToContainer(this);

			Plugin.GetInstance().StatesChanged += (Godot.Collections.Array data) => { MaybeUpdateValue(data); };
		}

		/// <summary>
        /// Updates the value of the component based on global states.
        /// </summary>
        /// <param name="data">Data array containing information about the global states.</param>
		public void MaybeUpdateValue(Godot.Collections.Array data)
		{
			if (data[0].As<string>() == "PlacingType")
			{
				if (
					false == IsValid()
				)
				{
					return;
				}

				Trait<Checkable>().Select(0).SetValue(data[1].As<string>() == "Simple");
			}
		}

		/// <summary>
        /// Event handler for checkbox pressed.
        /// </summary>
		private void _OnCheckboxPressed()
		{
			if (
				false == IsValid()
			)
			{
				return;
			}

			_GlobalExplorer.States.PlacingType = GlobalStates.PlacingTypeEnum.Simple;
			UpdateSpawnSettings("PlacingType", "Simple");
		}

		/// <summary>
        /// Checks if the component state is active.
        /// </summary>
        /// <returns>True if the component state is active, otherwise false.</returns>
		public bool IsActive()
		{
			return GlobalExplorer.GetInstance().States.PlacingType == GlobalStates.PlacingTypeEnum.Simple;
		}

		/// <summary>
        /// Checks if the component state is optimized.
        /// </summary>
        /// <returns>True if the component state is optimized, otherwise false.</returns>
		public bool IsOptimized()
		{
			return _GlobalExplorer.States.PlacingType == GlobalStates.PlacingTypeEnum.Optimized;
		}

		/// <summary>
        /// Syncronizes the component's value to a global central state controller.
        /// </summary>
		public override void Sync()
		{
			if (
				IsValid() &&
				Trait<Checkable>().Select(0).GetValue()
			)
			{
				_GlobalExplorer.States.PlacingType = GlobalStates.PlacingTypeEnum.Simple;
			}
		}
	}
}

#endif