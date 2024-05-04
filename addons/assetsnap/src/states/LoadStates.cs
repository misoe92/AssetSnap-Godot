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

using System;
using System.Reflection;
using Godot;

namespace AssetSnap.States
{
	/// <summary>
	/// Defines states for loading various components.
	/// </summary>
	[Tool]
	public partial class LoadStates
	{
		/// <summary>
		/// Enumeration representing the loaded state.
		/// </summary>
		public enum LoadedState 
		{
			Unloaded,
			Loaded,
		};
		
		/// <summary>
		/// Gets or sets the loaded state for settings.
		/// </summary>
		[ExportCategory("Load States")]
		[Export]
		public LoadedState IsSettingsLoaded = LoadedState.Unloaded;
		
		/// <summary>
		/// Gets or sets the loaded state for settings container.
		/// </summary>
		[Export]
		public LoadedState IsSettingsContainerLoaded = LoadedState.Unloaded;
		
		/// <summary>
		/// Gets or sets the loaded state for the group builder.
		/// </summary>
		[Export]
		public LoadedState IsGroupBuilderLoaded = LoadedState.Unloaded;

		/// <summary>
		/// Gets or sets the loaded state for the group builder container.
		/// </summary>
		[Export]
		public LoadedState IsGroupBuilderContainerLoaded = LoadedState.Unloaded;
		
		/// <summary>
		/// Sets the load state for a specified key.
		/// </summary>
		/// <param name="key">The key of the state to set.</param>
		/// <param name="value">The value to set the state to.</param>
		public void SetLoadState( string key, LoadedState value )
		{
			// Get the type of the class
			Type type = GetType();
			
			// Get the field or property with the provided name
			FieldInfo field = type.GetField(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			LoadedState EnumVal = value;
			field.SetValue(this, EnumVal);
			
			StateChanged( key, (int)value );
		}
		
		/// <summary>
        /// Emits a signal when the state changes.
        /// </summary>
        /// <param name="key">The key of the state that changed.</param>
        /// <param name="value">The new value of the state.</param>
		protected void StateChanged( string key, Variant value )
		{
			if( null != Plugin.Singleton ) 
			{
				Plugin.Singleton.EmitSignal(Plugin.SignalName.StatesChanged, new Godot.Collections.Array(){ key, value });
			}
		}
	}
}
#endif