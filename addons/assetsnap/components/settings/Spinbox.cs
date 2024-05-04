
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
	/// A spin box component for handling settings.
	/// </summary>
	[Tool]
	public partial class SettingsSpinBox : TraitableComponent
	{
		private float _value = 0.0f;
		private string _key;

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
		public float value
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
		public SettingsSpinBox()
		{
			Name = "SettingsSpinBox";

			UsingTraits = new()
			{
				{ typeof(Panelable).ToString() },
				{ typeof(Containerable).ToString() },
				{ typeof(Labelable).ToString() },
				{ typeof(Spinboxable).ToString() },
			};

			//_include = false;   
		}

		/// <summary>
        /// Initializes the component.
        /// </summary>
		public override void Initialize()
		{
			base.Initialize();

			string title = GetTitle(key);
			string description = GetDescription(key);

			Initiated = true;

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
				.SetName("SettingsSpinboxContainer")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetMargin(10, "top")
				.SetMargin(10, "bottom")
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetDimensions(0, 150)
				.Instantiate();

			Trait<Spinboxable>()
				.SetName(key)
				.SetDimensions(0, 20)
				.SetVerticalSizeFlags(Control.SizeFlags.ShrinkEnd)
				.SetStep(0.0f)
				.SetMaxValue(1000)
				.SetValue(value)
				.SetAction(new Callable(this, "UpdateKey"))
				.Instantiate();

			ConfigureTitle(title, description);

			Container innerContainer = Trait<Containerable>()
					.Select(0)
					.GetInnerContainer();

			if (null != title)
			{
				Trait<Labelable>()
					.Select(0)
					.AddToContainer(
						innerContainer
					);
			}

			if (null != description)
			{
				Trait<Labelable>()
					.Select(1)
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
					this
				);
		}

		/// <summary>
        /// Configures the title.
        /// </summary>
        /// <param name="title">The title of the setting.</param>
        /// <param name="description">The description of the setting.</param>
		private void ConfigureTitle(string title, string description)
		{
			if (title != null || description != null)
			{
				if (title != null)
				{
					Trait<Labelable>()
						.SetMargin(0)
						.SetName("SettingTitle")
						.SetText(title)
						.SetType(Labelable.TitleType.HeaderSmall)
						.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkBegin)
						.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
						.Instantiate();
				}

				if (description != null)
				{
					Trait<Labelable>()
						.SetMargin(0)
						.SetName("SettingDescription")
						.SetText(description)
						.SetType(Labelable.TitleType.TextSmall)
						.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
						.SetVerticalSizeFlags(Control.SizeFlags.ShrinkBegin)
						.SetAutoWrap(TextServer.AutowrapMode.Word)
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
		public string GetTitle(string key)
		{
			string FinalKey = key + "_title";
			return SettingsText.KeyToString(FinalKey);
		}

		/// <summary>
        /// Fetches the description of the setting.
        /// </summary>
        /// <param name="key">The key of the setting.</param>
        /// <returns>The description of the setting.</returns>
		public string GetDescription(string key)
		{
			string FinalKey = key + "_description";
			return SettingsText.KeyToString(FinalKey);
		}

		/// <summary>
        /// Updates the settings key and interval value.
        /// </summary>
        /// <param name="NewValue">The new value of the setting.</param>
		public void UpdateKey(float NewValue)
		{
			value = NewValue;
			ExplorerUtils.Get().Settings.SetKey(key, NewValue);
		}
	}
}

#endif