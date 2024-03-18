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
		public static string KeyToString( string key )
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
		
	}
}
#endif