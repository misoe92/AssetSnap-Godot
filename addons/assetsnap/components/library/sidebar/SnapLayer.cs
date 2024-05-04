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
using AssetSnap.States;
using Godot;

namespace AssetSnap.Front.Components.Library.Sidebar
{
	/// <summary>
	/// Component representing a snap layer in the library sidebar.
	/// </summary>
	[Tool]
	public partial class SnapLayer : LibraryComponent
	{
		private readonly string _Title = "Snap layer";
		private readonly string _Tooltip = "Defines which layer the object placed will be placed on, only objects on the same layer snaps to each other.";

		/// <summary>
		/// Constructor for SnapLayer component.
		/// </summary>
		public SnapLayer()
		{
			Name = "LSSnapLayer";
			
			_UsingTraits = new()
			{
				{ typeof(Spinboxable).ToString() },
			};
			
			//_include = false;
		}
		 
		/// <summary>
		/// Initializes the SnapLayer component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			Callable _callable = Callable.From((double value) => { _OnSpinBoxValueChange((int)value); });
			
			_Initiated = true;
			
			Trait<Spinboxable>()
				.SetName("SnapLayerSpinBox")
				.SetAction(_callable)
				.SetStep(1)
				.SetPrefix(_Title + ": ")
				.SetMinValue(0)
				.SetDimensions(140, 20)
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(5, "top")
				.SetMargin(7, "bottom")
				.SetTooltipText(_Tooltip)
				.Instantiate()
				.Select(0)
				.AddToContainer( this );
				
			Plugin.GetInstance().StatesChanged += (Godot.Collections.Array data) => { _MaybeUpdateValue(data); };
		}
		
		/// <summary>
		/// Resets the SnapLayer component.
		/// </summary>
		public void Reset()
		{
			StatesUtils.Get().SnapLayer = 0;
		}
			
		/// <summary>
		/// Syncronizes the SnapLayer value to a global central state controller.
		/// </summary>
		public override void Sync() 
		{
			if( false == IsValid() ) 
			{
				return;
			}
			
			StatesUtils.Get().SnapLayer = (int)Trait<Spinboxable>().GetValue();
		}
		
		/// <summary>
		/// Checks if the SnapLayer component is valid.
		/// </summary>
		/// <returns>True if the component is valid, otherwise false.</returns>
		public bool IsValid()
		{
			return
				null != _GlobalExplorer &&
				null != StatesUtils.Get() &&
				false != _Initiated &&
				null != Trait<Spinboxable>() &&
				false != HasTrait<Spinboxable>();
		}
	
		/// <summary>
		/// Updates the SnapLayer value if relevant data is received.
		/// </summary>
		/// <param name="data">Array containing relevant data.</param>
		private void _MaybeUpdateValue(Godot.Collections.Array data)
		{
			if( data[0].As<string>() == "SnapLayer" ) 
			{
				Trait<Spinboxable>()
					.Select(0)
					.SetValue(data[1].As<double>());
			} 
		}
		
		/// <summary>
		/// Handles the change in spin box value.
		/// </summary>
		/// <param name="value">The new value of the spin box.</param>
		private void _OnSpinBoxValueChange( int value )
		{
			StatesUtils.Get().SnapLayer = value;						
			UpdateSpawnSettings("SnapLayer", value);
		}
	}
}

#endif