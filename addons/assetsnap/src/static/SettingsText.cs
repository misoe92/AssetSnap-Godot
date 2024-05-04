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

namespace AssetSnap.Static
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Utility class for handling settings text.
	/// </summary>
	public static class SettingsText
	{

		/// <summary>
		/// Takes a key and converts it to the partnering string found in the class.
		/// </summary>
		/// <param name="key">The key to convert.</param>
		/// <returns>The corresponding string value.</returns>
		public static string KeyToString(string key)
		{
			Type type = typeof(SettingsText);
			string FinalKey = key;
			MethodInfo text = type.GetMethod(FinalKey, BindingFlags.Static | BindingFlags.Public);

			return text.Invoke(null, null) as string;
		}

		/// <summary>
		/// Fetches snap_boundary_box's title.
		/// </summary>
		/// <returns>The title of snap_boundary_box.</returns>
		public static string show_snap_boundary_box_title()
		{
			return "Snap Boundary Box";
		}

		/// <summary>
		/// Fetches snap_boundary_box's description.
		/// </summary>
		/// <returns>The description of snap_boundary_box.</returns>
		public static string show_snap_boundary_box_description()
		{
			return "Used to allow snap in place placements of objects, which allows for quick work when updating the scene.";
		}

		/// <summary>
		/// Fetches allow_drag_add's title.
		/// </summary>
		/// <returns>The title of allow_drag_add.</returns>
		public static string allow_drag_add_title()
		{
			return "Allow drag add of objects";
		}

		/// <summary>
		/// Fetches allow_drag_add's description.
		/// </summary>
		/// <returns>The description of allow_drag_add.</returns>
		public static string allow_drag_add_description()
		{
			return "When enabled a placement while pressing ALT will start drag mode, which places objects side by side.";
		}

		/// <summary>
		/// Fetches push_to_scene's title.
		/// </summary>
		/// <returns>The title of push_to_scene.</returns>
		public static string push_to_scene_title()
		{
			return "Push added objects to the current scene tree";
		}

		/// <summary>
		/// Fetches push_to_scene's description.
		/// </summary>
		/// <returns>The description of push_to_scene.</returns>
		public static string push_to_scene_description()
		{
			return "If enabled, newly added objects will be pushed to the current scene tree, and added directly to the file.";
		}

		/// <summary>
		/// Fetches the title for "Use Asset Snap Meshinstance Overlay".
		/// </summary>
		/// <returns>The title string.</returns>
		public static string use_as_overlay_title()
		{
			return "Use Asset Snap Meshinstance Overlay";
		}

		/// <summary>
		/// Fetches the description for "Use Asset Snap Meshinstance Overlay".
		/// </summary>
		/// <returns>The description string.</returns>
		public static string use_as_overlay_description()
		{
			return "Using the overlay allows for more actions in the editor, but in the game functions as a normal mesh instance.";
		}

		/// <summary>
		/// Fetches the title for "Focus asset after it's placed".
		/// </summary>
		/// <returns>The title string.</returns>
		public static string focus_placed_asset_title()
		{
			return "Focus asset after it's placed";
		}

		/// <summary>
		/// Fetches the description for "Focus asset after it's placed".
		/// </summary>
		/// <returns>The description string.</returns>
		public static string focus_placed_asset_description()
		{
			return "Instead of preparing a new instance when a asset is placed, editor targets the newly placed asset.";
		}

		/// <summary>
		/// Fetches the title for "Define snap boundary box opaqueness".
		/// </summary>
		/// <returns>The title string.</returns>
		public static string boundary_box_opacity_title()
		{
			return "Define snap boundary box opaqueness";
		}

		/// <summary>
		/// Fetches the description for "Define snap boundary box opaqueness".
		/// </summary>
		/// <returns>The description string.</returns>
		public static string boundary_box_opacity_description()
		{
			return "Enables you to control how opaque the Snap Boundary Boxes will be, thus customizing it to your needs.";
		}

		/// <summary>
		/// Fetches the title for "Model placed collision shapes".
		/// </summary>
		/// <returns>The title string.</returns>
		public static string add_collisions_title()
		{
			return "Model placed collision shapes.";
		}

		/// <summary>
		/// Fetches the description for "Model placed collision shapes".
		/// </summary>
		/// <returns>The description string.</returns>
		public static string add_collisions_description()
		{
			return "Allows you to control if you wish the models you place should have auto generated collision shapes.";
		}

		/// <summary>
		/// Fetches the title for "Allow continued asset placement".
		/// </summary>
		/// <returns>The title string.</returns>
		public static string allow_multi_drop_title()
		{
			return "Allow continued asset placement.";

		}

		/// <summary>
		/// Fetches the description for "Allow continued asset placement".
		/// </summary>
		/// <returns>The description string.</returns>
		public static string allow_multi_drop_description()
		{
			return "Allows you to continue placing assets as long as you hold shift + alt in when clicking.";
		}

		/// <summary>
		/// Fetches the title for "Grab models and move it with snapping".
		/// </summary>
		/// <returns>The title string.</returns>
		public static string allow_model_grab_title()
		{
			return "Grab models and move it with snapping";

		}

		/// <summary>
		/// Fetches the description for "Grab models and move it with snapping".
		/// </summary>
		/// <returns>The description string.</returns>
		public static string allow_model_grab_description()
		{
			return "Allows you to grab models by simply click shift + alt + g when model is in focus.";
		}

		/// <summary>
		/// Fetches the title for "Group objects together and place them".
		/// </summary>
		/// <returns>The title string.</returns>
		public static string allow_group_builder_title()
		{
			return "Group objects toghether and place them";
		}

		/// <summary>
		/// Fetches the description for "Group objects together and place them".
		/// </summary>
		/// <returns>The description string.</returns>
		public static string allow_group_builder_description()
		{
			return "Allows you to use the placement tool to place a group of models at the same time, predefining their structure.";
		}

		/// <summary>
		/// Fetches the title for "Fade in objects on spawn".
		/// </summary>
		/// <returns>The title string.</returns>
		public static string enable_model_spawn_in_title()
		{
			return "Fade in objects on spawn";
		}
		
		/// <summary>
		/// Fetches the description for "Fade in objects on spawn".
		/// </summary>
		/// <returns>The description string.</returns>
		public static string enable_model_spawn_in_description()
		{
			return "Allows you to use a transparent version of the model while placing, to easier see the area it's placed on.";

		}

		/// <summary>
		/// Fetches the title for "Initial model transparency level".
		/// </summary>
		/// <returns>The title string.</returns>
		public static string initial_model_spawn_in_level_title()
		{
			return "Initial model transparency level";
		}
		
		/// <summary>
		/// Fetches the description for "Initial model transparency level".
		/// </summary>
		/// <returns>The description string.</returns>
		public static string initial_model_spawn_in_level_description()
		{
			return "The initial model transparency when a model is not yet placed, this only works if 'Model Spawn In' is active. Range(0-1)";
		}

		/// <summary>
		/// Fetches the title for "Model preview image size".
		/// </summary>
		/// <returns>The title string.</returns>
		public static string model_preview_size_title()
		{
			return "Model preview image size";
		}

		/// <summary>
		/// Fetches the description for "Model preview image size".
		/// </summary>
		/// <returns>The description string.</returns>
		public static string model_preview_size_description()
		{
			return "Allows you to configure the preview image size of the models. Range(32-512)";
		}

		/// <summary>
		/// Fetches the title for "Model spawn in duration".
		/// </summary>
		/// <returns>The title string.</returns>
		public static string model_spawn_in_duration_title()
		{
			return "Model spawn in duration";
		}

		/// <summary>
		/// Fetches the description for "Model spawn in duration".
		/// </summary>
		/// <returns>The description string.</returns>
		public static string model_spawn_in_duration_description()
		{
			return "The fade in spawn time for the models when fade in spawn is active. Range(0,2)";
		}

		/// <summary>
		/// Fetches the title for "Fade boundary from center".
		/// </summary>
		/// <returns>The title string.</returns>
		public static string boundary_box_flat_title()
		{
			return "Fade boundary from center";
		}

		/// <summary>
        /// Fetches the description for "Fade boundary from center".
        /// </summary>
        /// <returns>The description string.</returns>
		public static string boundary_box_flat_description()
		{
			return "If enabled, the boundary box will fade more and more the further it get from the center of the boundary.";
		}
	}
}