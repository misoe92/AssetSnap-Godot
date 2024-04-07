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
namespace AssetSnap.States
{
	using System;
	using System.Reflection;
	using Godot;

	[Tool]
	public partial class LoadStates
	{
		public enum LoadedState 
		{
			Unloaded,
			Loaded,
		};
		
		[ExportCategory("Load States")]
		[Export]
		public LoadedState IsSettingsLoaded = LoadedState.Unloaded;
		
		[Export]
		public LoadedState IsSettingsContainerLoaded = LoadedState.Unloaded;
		
		[Export]
		public LoadedState IsGroupBuilderLoaded = LoadedState.Unloaded;
	
		[Export]
		public LoadedState IsGroupBuilderContainerLoaded = LoadedState.Unloaded;
		
		public void SetLoadState( string key, LoadedState value )
		{
			// Get the type of the class
			Type type = GetType();
			
			// Get the field or property with the provided name
			FieldInfo field = type.GetField(key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			LoadedState EnumVal = value;
			field.SetValue(this, EnumVal);
			
			StateChanged();
		}
		
		protected void StateChanged()
		{
			GlobalExplorer _GlobalExplorer = GlobalExplorer.GetInstance();
			if( null != _GlobalExplorer._Plugin ) 
			{
				Plugin _plugin = _GlobalExplorer._Plugin;
				_plugin.EmitSignal(Plugin.SignalName.StatesChanged);
			}
		}
	}
}
#endif