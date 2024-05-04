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

using System.Collections.Generic;
using AssetSnap.Front.Components;
using AssetSnap.Front.Configs;
using AssetSnap.States;
using Godot;

namespace AssetSnap.Settings
{
	public partial class BaseContainer : PanelContainer
	{
		public bool Initialized = false;
		
		private ScrollContainer _ScrollContainer;
		private MarginContainer _MarginContainer;
		private VBoxContainer _VBoxContainer;
		private HBoxContainer _HBoxContainer;
		private VBoxContainer _SubContainerOne;
		private VBoxContainer _SubContainerTwo;
		private VBoxContainer _SubContainerThree;
		private VBoxContainer _SubContainerFour;

		/// <summary>
		/// Constructor for the BaseContainer class.
		/// </summary>
		public BaseContainer()
		{
			Name = "SettingsBaseContainer";
		}

		/// <summary>
		/// Initializes the settings container.
		/// </summary>
		/// <param name="Config">The settings configuration.</param>
		public void Initialize()
		{
			if (Initialized)
			{
				// Clear the instances first
				_ScrollContainer.GetParent().RemoveChild(_ScrollContainer);
				_ScrollContainer.QueueFree();
			}

			Initialized = true;

			SettingsConfig _Config = GlobalExplorer.GetInstance().Settings;
			Name = "Settings";

			_ScrollContainer = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			_MarginContainer = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			_MarginContainer.AddThemeConstantOverride("margin_top", 5);
			_MarginContainer.AddThemeConstantOverride("margin_left", 5);
			_MarginContainer.AddThemeConstantOverride("margin_right", 5);
			_MarginContainer.AddThemeConstantOverride("margin_bottom", 5);

			_VBoxContainer = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			_HBoxContainer = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			_SubContainerOne = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			_SubContainerTwo = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			_SubContainerThree = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			_SubContainerFour = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			_RenderTypes();

			_HBoxContainer.AddChild(_SubContainerOne);
			_HBoxContainer.AddChild(_SubContainerTwo);
			_HBoxContainer.AddChild(_SubContainerThree);
			_HBoxContainer.AddChild(_SubContainerFour);
			_VBoxContainer.AddChild(_HBoxContainer);
			_MarginContainer.AddChild(_VBoxContainer);
			_ScrollContainer.AddChild(_MarginContainer);
			AddChild(_ScrollContainer);
			StatesUtils.SetLoad("Settings", true);
		}

		/// <summary>
		/// Fetches the setting type by method name.
		/// </summary>
		/// <param name="method">The name of the method.</param>
		/// <returns>The input type of the setting.</returns>
		public string GetInputTypeByMethod(string method)
		{
			return (string)new Callable(this, method).Call();
		}

		/// <summary>
		/// Checks if the given type method exists.
		/// </summary>
		/// <param name="str">The name of the method.</param>
		/// <returns>True if the method exists, otherwise false.</returns>
		public bool HasInputTypeMethod(string str)
		{
			var Type = GetType();
			return Type.GetMethod(str) != null;
		}

		/// <summary>
		/// Fetches the settings container based on iteration.
		/// </summary>
		/// <param name="Iteration">The iteration number.</param>
		/// <returns>The settings container.</returns>
		private VBoxContainer _GetSettingsContainer(int Iteration)
		{
			VBoxContainer EntryContainer = _SubContainerOne;
			if (Iteration == 1)
			{
				EntryContainer = _SubContainerTwo;
			}

			if (Iteration == 2)
			{
				EntryContainer = _SubContainerThree;
			}

			if (Iteration == 3)
			{
				EntryContainer = _SubContainerFour;
			}

			return EntryContainer;
		}

		/// <summary>
		/// Renders various settings.
		/// </summary>
		private void _RenderTypes()
		{
			var Iteration = 0;
			SettingsConfig _Config = GlobalExplorer.GetInstance().Settings;
			Godot.Collections.Dictionary<string, Variant> _Settings = _Config.GetSettings();
			
			foreach ((string key, Variant value) in _Settings)
			{
				string k = key + "_type";
				if (HasInputTypeMethod(k))
				{
					string InputType = GetInputTypeByMethod(k);
					VBoxContainer EntryContainer = _GetSettingsContainer(Iteration);

					if (value is Godot.Variant VariantValue)
					{
						string StringValue = VariantValue.As<string>();
						float FloatValue = VariantValue.As<float>();

						if (StringValue.ToLower() == "false" || StringValue.ToLower() == "true")
						{
							bool FinalValue = false;

							if (StringValue == "true")
							{
								FinalValue = true;
							}
							_RenderBoolType(key, FinalValue, InputType, EntryContainer);
						}
						else
						{
							_RenderIntegerType(key, FloatValue, InputType, EntryContainer);
						}
					}
					else
					{
						GD.PushWarning("Found no type matching:", value.GetType());
					}
				}

				Iteration += 1;
				if (Iteration > 3)
				{
					Iteration = 0;
				}
			}
		}

		/// <summary>
		/// Renders a string type setting.
		/// </summary>
		/// <param name="key">The key of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		/// <param name="Type">The type of the setting.</param>
		/// <param name="_Container">The container to render the setting in.</param>
		/// <returns>void</returns>
		private void _RenderStringType(string key, string value, string Type, VBoxContainer _Container)
		{
			switch (Type)
			{
				case "LineEdit":

					break;
			}
		}

		/// <summary>
		/// Renders an integer type setting.
		/// </summary>
		/// <param name="key">The key of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		/// <param name="Type">The type of the setting.</param>
		/// <param name="_Container">The container to render the setting in.</param>
		/// <returns>void</returns>
		private void _RenderIntegerType(string key, float value, string Type, VBoxContainer _Container)
		{
			switch (Type)
			{
				case "SpinBox":
					List<string> Components = new()
					{
						"SettingsSpinBox",
					};

					if (GlobalExplorer.GetInstance().Components.HasAll(Components.ToArray()))
					{
						SettingsSpinBox _SettingsSpinBox = GlobalExplorer.GetInstance().Components.Single<SettingsSpinBox>(true);

						_SettingsSpinBox.Key = key;
						_SettingsSpinBox.Value = value;
						_SettingsSpinBox.Initialize();
						_Container.AddChild(_SettingsSpinBox);
					}
					break;
			}
		}

		/// <summary>
		/// Renders a boolean type setting.
		/// </summary>
		/// <param name="key">The key of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		/// <param name="Type">The type of the setting.</param>
		/// <param name="_Container">The container to render the setting in.</param>
		/// <returns>void</returns>
		private void _RenderBoolType(string key, bool value, string Type, VBoxContainer _Container)
		{
			switch (Type)
			{
				case "CheckBox":
					List<string> Components = new()
					{
						"SettingsCheckbox",
					};

					if (GlobalExplorer.GetInstance().Components.HasAll(Components.ToArray()))
					{
						SettingsCheckbox _SettingsCheckbox = GlobalExplorer.GetInstance().Components.Single<SettingsCheckbox>(true);

						_SettingsCheckbox.Key = key;
						_SettingsCheckbox.Value = value;
						_SettingsCheckbox.Initialize();
						_Container.AddChild(_SettingsCheckbox);
					}
					break;
			}
		}

		/// <summary>
		/// Converts a key to a label.
		/// </summary>
		/// <param name="key">The key to convert.</param>
		/// <returns>The converted label.</returns>
		private string _KeyToLabel(string key)
		{
			return key.Capitalize().Split('_').Join(" ");
		}

		/// <summary>
		/// Defines the input type for allow multi drop.
		/// </summary>
		/// <returns>The input type for allow multi drop.</returns>
		public string allow_multi_drop_type()
		{
			return "CheckBox";
		}

		/// <summary>
		/// Defines the input type for allowing model grab.
		/// </summary>
		/// <returns>The input type for allowing model grab.</returns>
		public string allow_model_grab_type()
		{
			return "CheckBox";
		}

		/// <summary>
		/// Defines the input type for adding collisions.
		/// </summary>
		/// <returns>The input type for adding collisions.</returns>
		public string add_collisions_type()
		{
			return "CheckBox";
		}

		/// <summary>
		/// Defines the input type for boundary box opacity.
		/// </summary>
		/// <returns>The input type for boundary box opacity.</returns>
		public string boundary_box_opacity_type()
		{
			return "SpinBox";
		}

		/// <summary>
		/// Defines the input type for focusing on placed asset.
		/// </summary>
		/// <returns>The input type for focusing on placed asset.</returns>
		public string focus_placed_asset_type()
		{
			return "CheckBox";
		}

		/// <summary>
		/// Defines the input type for using as overlay.
		/// </summary>
		/// <returns>The input type for using as overlay.</returns>
		public string use_as_overlay_type()
		{
			return "CheckBox";
		}

		/// <summary>
		/// Defines the input type for pushing to scene.
		/// </summary>
		/// <returns>The input type for pushing to scene.</returns>
		public string push_to_scene_type()
		{
			return "CheckBox";
		}

		/// <summary>
		/// Defines the input type for showing snap boundary box.
		/// </summary>
		/// <returns>The input type for showing snap boundary box.</returns>
		public string show_snap_boundary_box_type()
		{
			return "CheckBox";
		}

		/// <summary>
		/// Defines the input type for allowing drag and add.
		/// </summary>
		/// <returns>The input type for allowing drag and add.</returns>
		public string allow_drag_add_type()
		{
			return "CheckBox";
		}

		/// <summary>
		/// Defines the input type for allowing group builder.
		/// </summary>
		/// <returns>The input type for allowing group builder.</returns>
		public string allow_group_builder_type()
		{
			return "CheckBox";
		}

		/// <summary>
		/// Defines the input type for enabling model spawn in.
		/// </summary>
		/// <returns>The input type for enabling model spawn in.</returns>
		public string enable_model_spawn_in_type()
		{
			return "CheckBox";
		}

		/// <summary>
		/// Defines the input type for the initial model spawn in level.
		/// </summary>
		/// <returns>The input type for the initial model spawn in level.</returns>
		public string initial_model_spawn_in_level_type()
		{
			return "SpinBox";
		}

		/// <summary>
		/// Defines the input type for model preview size.
		/// </summary>
		/// <returns>The input type for model preview size.</returns>
		public string model_preview_size_type()
		{
			return "SpinBox";
		}

		/// <summary>
		/// Defines the input type for model spawn in duration.
		/// </summary>
		/// <returns>The input type for model spawn in duration.</returns>
		public string model_spawn_in_duration_type()
		{
			return "SpinBox";
		}

		/// <summary>
		/// Defines the input type for boundary box flat.
		/// </summary>
		/// <returns>The input type for boundary box flat.</returns>
		public string boundary_box_flat_type()
		{
			return "CheckBox";
		}
	}
}