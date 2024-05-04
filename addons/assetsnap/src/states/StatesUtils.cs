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

namespace AssetSnap.States
{
	/// <summary>
	/// Utility class for accessing and modifying global states.
	/// </summary>
	public static class StatesUtils
	{
		/// <summary>
		/// Sets the load state of a specified key.
		/// </summary>
		/// <param name="key">The key of the state to set.</param>
		/// <param name="value">The value to set the state to.</param>
		public static void SetLoad( string key, bool value ) 
		{
			string FullKey = "Is" + key + "Loaded";
			GlobalStates globalStates = Get();
			globalStates.SetLoadState(FullKey, value ? LoadStates.LoadedState.Loaded : LoadStates.LoadedState.Unloaded );
		}
		
		/// <summary>
		/// Retrieves the global states instance.
		/// </summary>
		/// <returns>The global states instance.</returns>
		public static GlobalStates Get()
		{
			return GlobalExplorer.GetInstance().States;
		}
	}
}