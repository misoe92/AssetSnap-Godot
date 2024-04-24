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
namespace AssetSnap.Static
{
	using System;
	using System.Reflection;

	public static class SettingsText
	{

		/*
		** Takes a key and converts it to 
		** the partnering string found in the
		** class
		**
		** @param string key
		** @return string
		*/
		public static string KeyToString(string key)
		{
			Type type = typeof(SettingsText);
			string FinalKey = key;
			MethodInfo text = type.GetMethod(FinalKey, BindingFlags.Static | BindingFlags.Public);

			return text.Invoke(null, null) as string;
		}

		/*
		** Fetches snap_boundary_box's title
		**
		** @return string
		*/
		public static string show_snap_boundary_box_title()
		{
			return "Snap Boundary Box";
		}

		/*
		** Fetches snap_boundary_box's description
		**
		** @return string
		*/
		public static string show_snap_boundary_box_description()
		{
			return "Used to allow snap in place placements of objects, which allows for quick work when updating the scene.";
		}

		/*
		** Fetches allow_drag_add's title
		**
		** @return string
		*/
		public static string allow_drag_add_title()
		{
			return "Allow drag add of objects";
		}

		/*
		** Fetches allow_drag_add's description
		**
		** @return string
		*/
		public static string allow_drag_add_description()
		{
			return "When enabled a placement while pressing ALT will start drag mode, which places objects side by side.";
		}

		/*
		** Fetches push_to_scene's title
		**
		** @return string
		*/
		public static string push_to_scene_title()
		{
			return "Push added objects to the current scene tree";
		}

		/*
		** Fetches push_to_scene's description
		**
		** @return string
		*/
		public static string push_to_scene_description()
		{
			return "If enabled, newly added objects will be pushed to the current scene tree, and added directly to the file.";
		}

		/*
		** Fetches use_as_overlay's title
		**
		** @return string
		*/
		public static string use_as_overlay_title()
		{
			return "Use Asset Snap Meshinstance Overlay";
		}

		/*
		** Fetches use_as_overlay's description
		**
		** @return string
		*/
		public static string use_as_overlay_description()
		{
			return "Using the overlay allows for more actions in the editor, but in the game functions as a normal mesh instance.";
		}

		/*
		** Fetches focus_placed_asset's title
		**
		** @return string
		*/
		public static string focus_placed_asset_title()
		{
			return "Focus asset after it's placed";
		}

		/*
		** Fetches focus_placed_asset's description
		**
		** @return string
		*/
		public static string focus_placed_asset_description()
		{
			return "Instead of preparing a new instance when a asset is placed, editor targets the newly placed asset.";
		}

		/*
		** Fetches boundary_box_opacity's title
		**
		** @return string
		*/
		public static string boundary_box_opacity_title()
		{
			return "Define snap boundary box opaqueness";
		}

		/*
		** Fetches boundary_box_opacity's description
		**
		** @return string
		*/
		public static string boundary_box_opacity_description()
		{
			return "Enables you to control how opaque the Snap Boundary Boxes will be, thus customizing it to your needs.";
		}

		/*
		** Fetches add_collisions's title
		**
		** @return string
		*/
		public static string add_collisions_title()
		{
			return "Model placed collision shapes.";
		}

		/*
		** Fetches add_collisions's description
		**
		** @return string
		*/
		public static string add_collisions_description()
		{
			return "Allows you to control if you wish the models you place should have auto generated collision shapes.";
		}

		public static string allow_multi_drop_title()
		{
			return "Allow continued asset placement.";

		}

		public static string allow_multi_drop_description()
		{
			return "Allows you to continue placing assets as long as you hold shift + alt in when clicking.";
		}

		public static string allow_model_grab_title()
		{
			return "Grab models and move it with snapping";

		}

		public static string allow_model_grab_description()
		{
			return "Allows you to grab models by simply click shift + alt + g when model is in focus.";
		}

		public static string allow_group_builder_title()
		{
			return "Group objects toghether and place them";
		}

		public static string allow_group_builder_description()
		{
			return "Allows you to use the placement tool to place a group of models at the same time, predefining their structure.";
		}

		public static string enable_model_spawn_in_title()
		{
			return "Fade in objects on spawn";
		}
		public static string enable_model_spawn_in_description()
		{
			return "Allows you to use a transparent version of the model while placing, to easier see the area it's placed on.";

		}

		public static string initial_model_spawn_in_level_title()
		{
			return "Initial model transparency level";
		}
		public static string initial_model_spawn_in_level_description()
		{
			return "The initial model transparency when a model is not yet placed, this only works if 'Model Spawn In' is active. Range(0-1)";
		}

		public static string model_preview_size_title()
		{
			return "Model preview image size";
		}

		public static string model_preview_size_description()
		{
			return "Allows you to configure the preview image size of the models. Range(32-512)";
		}

		public static string model_spawn_in_duration_title()
		{
			return "Model spawn in duration";
		}

		public static string model_spawn_in_duration_description()
		{
			return "The fade in spawn time for the models when fade in spawn is active. Range(0,2)";
		}

		public static string boundary_box_flat_title()
		{
			return "Fade boundary from center";
		}

		public static string boundary_box_flat_description()
		{
			return "If enabled, the boundary box will fade more and more the further it get from the center of the boundary.";
		}

	}
}
#endif