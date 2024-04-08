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

namespace AssetSnap.Front.Components.Library.Sidebar
{
	using AssetSnap.Component;
	using Godot;

	[Tool]
	public partial class SnapLayer : LibraryComponent
	{
		private readonly string _Title = "Snap layer";
		private readonly string _Tooltip = "Defines which layer the object placed will be placed on, only objects on the same layer snaps to each other.";

		/*
		** Component constructor
		**  
		** @return void
		*/	
		public SnapLayer()
		{
			Name = "LSSnapLayer";
			//_include = false;
		}
		 
		/*
		** Initializes the component
		** 
		** @return void
		*/
		public override void Initialize()
		{
			base.Initialize();
			AddTrait(typeof(Spinboxable));

			Callable _callable = Callable.From((double value) => { _OnSpinBoxValueChange((int)value); });
			
			Initiated = true;
			
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
				.AddToContainer( Container );
				
			Plugin.GetInstance().StatesChanged += () => { MaybeUpdateValue(); };
		}
		
		private void MaybeUpdateValue()
		{
			Trait<Spinboxable>()
				.Select(0)
				.SetValue(_GlobalExplorer.States.SnapLayer);
		}
		
		/*
		** Synchronizes spawn setting value
		** when changed
		** 
		** @return void
		*/
		private void _OnSpinBoxValueChange( int value )
		{
			_GlobalExplorer.States.SnapLayer = value;						
			UpdateSpawnSettings("SnapLayer", value);
		}
		
		/*
		** Resets the component
		** 
		** @return void
		*/
		public void Reset()
		{
			_GlobalExplorer.States.SnapLayer = 0;
		}
		
		public bool IsValid()
		{
			return
				null != _GlobalExplorer &&
				null != _GlobalExplorer.States &&
				false != Initiated &&
				null != Trait<Spinboxable>() &&
				false != HasTrait<Spinboxable>() &&
				IsInstanceValid( Trait<Spinboxable>() );
		}
		
		/*
		** Syncronizes it's value to a global
		** central state controller
		**
		** @return void
		*/
		public override void Sync() 
		{
			if( false == IsValid() ) 
			{
				return;
			}
			
			_GlobalExplorer.States.SnapLayer = (int)Trait<Spinboxable>().GetValue();
		}
		
		public override void _ExitTree()
		{
			Initiated = false;
			
			base._ExitTree();
		}
	}
}