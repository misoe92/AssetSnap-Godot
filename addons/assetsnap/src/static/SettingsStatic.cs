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
	/// <summary>
	/// Utility class for handling settings.
	/// </summary>
	public static class SettingsStatic
	{
		/// <summary>
		/// Gets the transparency level.
		/// </summary>
		/// <returns>The transparency level as a float value.</returns>
		public static float TransparencyLevel()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			return _GlobalExplorer.Settings.GetKey("initial_model_spawn_in_level").As<float>();
		}

		/// <summary>
		/// Gets the transparency fade duration.
		/// </summary>
		/// <returns>The transparency fade duration as a float value.</returns>
		public static float TransparencyFadeDuration()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			return _GlobalExplorer.Settings.GetKey("model_spawn_in_duration").As<float>();
		}
		
		/// <summary>
		/// Gets the preview image size.
		/// </summary>
		/// <returns>The preview image size as an integer value.</returns>
		public static int PreviewImageSize()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			return _GlobalExplorer.Settings.GetKey("model_preview_size").As<int>();
		}
		
		/// <summary>
		/// Checks if new children should be pushed to the current scene.
		/// </summary>
		/// <returns>True if new children should be pushed to the current scene; otherwise, false.</returns>
		public static bool ShouldPushToScene()
		{
			bool PushToScene = GlobalExplorer.GetInstance().Settings.GetKey("push_to_scene").As<bool>();
			return PushToScene;
		}

		/// <summary>
		/// Checks if collisions should be added.
		/// </summary>
		/// <returns>True if collisions should be added; otherwise, false.</returns>
		public static bool ShouldAddCollision()
		{
			bool AddCollisions = GlobalExplorer.GetInstance().Settings.GetKey("add_collisions").As<bool>();
			return AddCollisions;
		}

		/// <summary>
		/// Checks if Multi drop is allowed.
		/// </summary>
		/// <returns>True if Multi drop is allowed; otherwise, false.</returns>
		public static bool CanMultiDrop()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			return _GlobalExplorer.Settings.GetKey("allow_multi_drop").As<bool>();
		}

		/// <summary>
		/// Checks if model transparency is active.
		/// </summary>
		/// <returns>True if model transparency is active; otherwise, false.</returns>
		public static bool ModelTransparencyActive()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			return _GlobalExplorer.Settings.GetKey("enable_model_spawn_in").As<bool>();
		}

		/// <summary>
		/// Checks if an asset should be focused after placement.
		/// </summary>
		/// <returns>True if an asset should be focused after placement; otherwise, false.</returns>
		public static bool ShouldFocusAsset()
		{
			bool value = GlobalExplorer.GetInstance().Settings.GetKey("focus_placed_asset").As<bool>();

			if (value is bool valueBool)
			{
				return valueBool;
			}

			return false;
		}
		
		/// <summary>
		/// Checks if AS overlay should be used.
		/// </summary>
		/// <returns>True if AS overlay should be used; otherwise, false.</returns>
		public static bool ShouldUseASOverlay()
		{
			bool value = GlobalExplorer.GetInstance().Settings.GetKey("use_as_overlay").As<bool>();

			if (value is bool valueBool)
			{
				return valueBool;
			}

			return false;
		}
	}
}