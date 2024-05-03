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
	/// Component for optimized placement functionality.
	/// </summary>
	[Tool]
	public partial class OptimizedPlacement : CheckableComponent
	{
		private readonly string _Title = "Optimized placement";
		private readonly string _CheckboxTooltip = "Will use multi mesh for placing the elements, will be good when a large amount of objects is needed to be spawned";

		/// <summary>
		/// Constructor of the component.
		/// </summary>
		public OptimizedPlacement()
		{
			Name = "LSOptimizedPlacement";

			UsingTraits = new()
			{
				{ typeof(Checkable).ToString() },
			};

			// _include = false;
		}

		/// <summary>
		/// Initializes the component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			Callable _callable = Callable.From(() => { _OnCheckboxPressed(); });
			Initiated = true;

			Trait<Checkable>()
				.SetName("OptimizedPlacementCheckbox")
				.SetAction(_callable)
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(2, "top")
				.SetMargin(10, "bottom")
				.SetText(_Title)
				.SetTooltipText(_CheckboxTooltip)
				.SetValue(IsActive())
				.Instantiate()
				.Select(0)
				.AddToContainer(this);

			Plugin.GetInstance().StatesChanged += (Godot.Collections.Array data) => { MaybeUpdateValue(data); };
		}

		/// <summary>
        /// Checks if an update of value is necessary and updates it if needed.
        /// </summary>
        /// <param name="data">The data to be checked for updates.</param>
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

				Trait<Checkable>().Select(0).SetValue(data[1].As<string>() == "Optimized");
			}
		}

		/// <summary>
        /// Updates collisions and spawn settings of the model.
        /// </summary>
		private void _OnCheckboxPressed()
		{
			_GlobalExplorer.States.PlacingType = GlobalStates.PlacingTypeEnum.Optimized;
			UpdateSpawnSettings("PlacingType", "Optimized");
		}

		/// <summary>
        /// Checks if the component state is active.
        /// </summary>
        /// <returns>True if active, otherwise false.</returns>
		public bool IsActive()
		{
			return _GlobalExplorer.States.PlacingType == GlobalStates.PlacingTypeEnum.Optimized;
		}

		/// <summary>
        /// Checks if the component state is simple.
        /// </summary>
        /// <returns>True if simple, otherwise false.</returns>
		public bool IsSimple()
		{
			return _GlobalExplorer.States.PlacingType == GlobalStates.PlacingTypeEnum.Simple;
		}

		/// <summary>
        /// Synchronizes its value to a global central state controller.
        /// </summary>
		public override void Sync()
		{
			if (
				IsValid() &&
				Trait<Checkable>().Select(0).GetValue()
			)
			{
				_GlobalExplorer.States.PlacingType = GlobalStates.PlacingTypeEnum.Optimized;
			}
		}
	}
}

#endif