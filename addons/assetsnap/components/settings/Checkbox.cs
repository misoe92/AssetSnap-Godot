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
using AssetSnap.Explorer;
using AssetSnap.Static;
using Godot;

namespace AssetSnap.Front.Components
{
	/// <summary>
	/// A checkbox component for handling settings.
	/// </summary>
	[Tool]
	public partial class SettingsCheckbox : TraitableComponent
	{
		private string _key;
		private bool _value = false;
		
		/// <summary>
		/// The key associated with this setting.
		/// </summary>
		public string key 
		{
			get => _key;
			set 
			{
				_key = value;
			}
		}
		
		/// <summary>
        /// The value of the setting.
        /// </summary>
		public bool value 
		{ 
			get => _value;
			set 
			{
				_value = value;
			}
		}
		
		/// <summary>
        /// Constructor of the component.
        /// </summary>
		public SettingsCheckbox()
		{
			Name = "SettingsCheckbox";
			
			UsingTraits = new()
			{
				{ typeof(Panelable).ToString() },
				{ typeof(Containerable).ToString() },
				{ typeof(Labelable).ToString() },
				{ typeof(Checkable).ToString() },
			};
			
			//_include = false;
		}
		
		/// <summary>
        /// Initializes the component.
        /// </summary>
		public override void Initialize() 
		{
			base.Initialize();
			
			Name = "SettingsCheckbox";
			if( key == null ) 
			{
				return;
			}
			
			Initiated = true;

			string title = GetTitle( key );
			string description = GetDescription( key );

			Trait<Panelable>()
				.SetName("SettingsPanel")
				.SetType(Panelable.PanelType.RoundedPanelContainer)
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetMargin(5, "top")
				.SetMargin(5, "bottom")
				.SetMargin(5, "left")
				.SetMargin(5, "right")
				.Instantiate();
				
			Trait<Containerable>()
				.SetName("SettingCheckboxContainer")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetMargin(10, "top")
				.SetMargin(10, "bottom")
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetDimensions(0, 150)
				.Instantiate();

			Trait<Checkable>()
				.SetName("SettingsCheckboxInput")
				.SetDimensions(0, 20)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkEnd)
				.SetText( ExplorerUtils.Get().Settings.KeyToLabel( key ) )
				.SetAction( new Callable( this, "UpdateKey" ) )
				.SetValue( value )
				.Instantiate();
			
			ConfigureTitle( title, description );
			
			Container innerContainer = Trait<Containerable>()
					.Select(0)
					.GetInnerContainer();
			
			if( null != title ) 
			{
				Trait<Labelable>()
					.Select(0)
					.AddToContainer(
						innerContainer
					);
			}
			
			if( null != description ) 
			{
				Trait<Labelable>()
					.Select(1)
					.AddToContainer(
						innerContainer
					);
			}
			
			Trait<Checkable>()
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
					this
				);
		}
		
		/// <summary>
        /// Configures the title.
        /// </summary>
        /// <param name="title">The title of the setting.</param>
        /// <param name="description">The description of the setting.</param>
		private void ConfigureTitle( string title, string description)
		{
			if( null != title || null != description )
			{
				if( null != title )
				{
					Trait<Labelable>()
						.SetMargin(0)
						.SetName("SettingTitle")
						.SetType(Labelable.TitleType.HeaderSmall)
						.SetText(title)
						.Instantiate();
				}
				
				if ( null != description )
				{
					Trait<Labelable>()
						.SetMargin(0)
						.SetName("SettingDescription")
						.SetText(description)
						.SetType(Labelable.TitleType.TextSmall)
						.SetAutoWrap(TextServer.AutowrapMode.Word)
						.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
						.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
						.SetDimensions(0, 85)
						.Instantiate();
				}
			}
		}
		
		/// <summary>
        /// Fetches the title of the setting.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <returns>The title of the setting.</returns>
		public string GetTitle( string key )
		{
			string FinalKey = key + "_title";
			return SettingsText.KeyToString(FinalKey);
		}
		
		/// <summary>
        /// Fetches the description of the setting.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <returns>The description of the setting.</returns>
		public string GetDescription( string key )
		{
			string FinalKey = key + "_description";
			return SettingsText.KeyToString(FinalKey);
		}
		
		/// <summary>
        /// Updates the settings key and interval value.
        /// </summary>
		public void UpdateKey()
		{
			value = !value;
			_GlobalExplorer.Settings.SetKey(key, value);
		}
	}
}

#endif