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
using AssetSnap.Library;

namespace AssetSnap.Static
{
	public static class SettingsStatic
	{
		/*
		** Checks if new children should be pushed to the current scene
		**
		** @return bool
		*/
		public static bool ShouldPushToScene()
		{
			bool PushToScene = GlobalExplorer.GetInstance().Settings.GetKey("push_to_scene").As<bool>();
			return PushToScene;
		}
		
		/*
		** Checks if collisions should be added
		**
		** @return bool
		*/
		public static bool ShouldAddCollision()
		{
			bool AddCollisions = GlobalExplorer.GetInstance().Settings.GetKey("add_collisions").As<bool>();
			return AddCollisions;
		}
		
		/*
		** Checks if Multi drop is allowed
		**
		** @returns bool
		*/
		public static bool CanMultiDrop()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			return _GlobalExplorer.Settings.GetKey("allow_multi_drop").As<bool>();
		}
		
		/*
		** Checks if a asset should be focused after placement
		**
		** @return bool
		*/
		public static bool ShouldFocusAsset()
		{
			bool value = GlobalExplorer.GetInstance().Settings.GetKey("focus_placed_asset").As<bool>();
			
			if( value is bool valueBool ) 
			{
				return valueBool;
			}

			return false;
		}
	}
}
#endif