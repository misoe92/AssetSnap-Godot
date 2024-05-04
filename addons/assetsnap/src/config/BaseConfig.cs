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

using Godot;

namespace AssetSnap.Config
{
	/// <summary>
	/// Base class for configuration handling.
	/// </summary>
	[Tool]
	public partial class BaseConfig
	{
		protected readonly string _BasePath = "res://addons/assetsnap/"; 
		protected readonly string _DefaultConfigSection = "Settings";
		protected string _Name = "AssetSnapConfig"; 
		protected string _LoadedConfigFilename;
		protected bool _LoadOk; 
		protected ConfigFile _Config;
		
		/// <summary>
		/// Constructor for the BaseConfig class.
		/// </summary>
		/// <remarks>
		/// Initializes the LoadOk field to false.
		/// </remarks>	
		public BaseConfig()
		{
			_LoadOk = false;
		}
		
		/// <summary>
		/// Sets a value in the configuration and saves it.
		/// </summary>
		/// <param name="_key">The key to set the value for.</param>
		/// <param name="_value">The value to set.</param>
		/// <returns>Void.</returns>
		public virtual void SetKey( string _key, Variant _value )
		{
			_Config.SetValue(_Name, _key, _value);
			_Config.Save(_BasePath + _LoadedConfigFilename);
			GlobalExplorer.GetInstance()._Plugin.EmitSignal(Plugin.SignalName.SettingKeyChanged, new Godot.Collections.Array() { _key, _value });
		}
		
		/// <summary>
		/// Retrieves a single key value from the configuration.
		/// </summary>
		/// <param name="_key">The key to retrieve the value for.</param>
		/// <returns>The value associated with the specified key.</returns>
		public virtual Variant GetKey( string _key )
		{
			return _Config.GetValue( _DefaultConfigSection, _key );
		}
		
		/// <summary>
		/// Loads the configuration from the specified file name.
		/// </summary>
		/// <param name="_ConfigName">The name of the configuration file.</param>
		/// <returns>Void.</returns>
		protected void LoadConfig( string _ConfigName)
		{
			_Config = new();
			
			_LoadedConfigFilename = _ConfigName;
			Error err = _Config.Load(_BasePath + _LoadedConfigFilename);

			if (err == Error.Ok)
			{
				_LoadOk = true;
			}
		}
	} 
}