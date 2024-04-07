
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

namespace AssetSnap.Front.Components
{
	using AssetSnap.Component;
	using AssetSnap.Explorer;
	using AssetSnap.Static;
	using Godot;

	[Tool]
	public partial class SettingsSpinBox : TraitableComponent
	{
		private float _value = 0.0f;
		private string _key;

		public string key 
		{
			get => _key;
			set 
			{
				_key = value;
			}
		}
		
		public float value 
		{
			get => _value;
			set 
			{
				_value = value;
			}
		}
		
		/*
		** Constructor of the component
		** 
		** @return void
		*/			 
		public SettingsSpinBox()
		{
			Name = "SettingsSpinBox";
			//_include = false;   
		}
		
		/*
		** Initializing the component
		**   
		** @return void 
		*/
		public override void Initialize() 
		{
			AddTrait(typeof(Panelable));
			AddTrait(typeof(Containerable));
			AddTrait(typeof(Labelable));
			AddTrait(typeof(Spinboxable));
			
			string title = GetTitle( key );
			string description = GetDescription( key );

			Initiated = true;

			Trait<Panelable>()
				.SetName("SettingsPanel")
				.SetType(Panelable.PanelType.RoundedPanelContainer)
				.SetMargin(5, "top")
				.SetMargin(5, "bottom")
				.SetMargin(5, "left")
				.SetMargin(5, "right")
				.Instantiate();
			
			Trait<Containerable>()
				.SetName("SettingsSpinboxContainer")
				.SetMargin(10, "top")
				.SetMargin(10, "bottom")
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetDimensions(0, 100)
				.Instantiate();

			Trait<Spinboxable>()
				.SetName(key)
				.SetStep(0.0f)
				.SetValue(value)
				.SetAction( new Callable(this, "UpdateKey") )
				.Instantiate();
			
			ConfigureTitle( title, description );

			Container innerContainer = Trait<Containerable>()
					.Select(0)
					.GetInnerContainer();
			
			if(
				Trait<Labelable>().Select(0).IsValid()
			)
			{
				Trait<Labelable>()
					.AddToContainer(
						innerContainer
					);
			}
			
			if(
				Trait<Labelable>().Select(1).IsValid()
			)
			{
				Trait<Labelable>()
					.AddToContainer(
						innerContainer
					);
			}
			
			Trait<Spinboxable>()
				.Select(0)
				.AddToContainer(
					innerContainer
				);

			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					Trait<Panelable>()
						.Select(0)
						.GetNode()
				);
				
			Trait<Panelable>()
				.Select(0)
				.AddToContainer(
					Container
				);
		}
		
		/*
		** Configures the title
		** 
		** @return void
		*/
		private void ConfigureTitle( string title, string description)
		{
			if (title != null || description != null)
			{
				if (title != null)
				{
					Trait<Labelable>()
						.SetMargin(0)
						.SetName("SettingTitle")
						.SetText(title)
						.SetType(Labelable.TitleType.HeaderMedium)
						.Instantiate();
				}
				
				if (description != null)
				{
					Trait<Labelable>()
						.SetMargin(0)
						.SetName("SettingDescription")
						.SetText(description)
						.SetType(Labelable.TitleType.HeaderSmall)
						.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
						.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
						.SetAutoWrap(TextServer.AutowrapMode.Word)
						.SetDimensions(0, 70)
						.Instantiate();
				}
			}
		}
		
		/*
		** Fetches the title
		** 
		** @return string
		*/
		public string GetTitle( string key )
		{
			string FinalKey = key + "_title";
			return SettingsText.KeyToString(FinalKey);
		}
		
		/*
		** Fetches the description
		** 
		** @return string
		*/
		public string GetDescription( string key )
		{
			string FinalKey = key + "_description";
			return SettingsText.KeyToString(FinalKey);
		}
		
		/*
		** Updates the settings key and
		** interval value
		** 
		** @return void
		*/
		public void UpdateKey( float NewValue )
		{
			value = NewValue;
			ExplorerUtils.Get().Settings.SetKey(key, NewValue);
		}
	}
}