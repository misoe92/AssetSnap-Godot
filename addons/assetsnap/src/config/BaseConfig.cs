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
		protected string Name = "AssetSnapConfig"; 
		protected readonly string BasePath = "res://addons/assetsnap/"; 
		protected readonly string DefaultConfigSection = "Settings";
		protected string LoadedConfigFilename;
		protected bool LoadOk; 
		protected ConfigFile _Config;
		
		/// <summary>
        /// Constructor for the BaseConfig class.
        /// </summary>
        /// <remarks>
        /// Initializes the LoadOk field to false.
        /// </remarks>	
		public BaseConfig()
		{
			LoadOk = false;
		}

		/// <summary>
        /// Loads the configuration from the specified file name.
        /// </summary>
        /// <param name="_ConfigName">The name of the configuration file.</param>
        /// <returns>Void.</returns>
		protected void LoadConfig( string _ConfigName)
		{
			_Config = new();
			
			LoadedConfigFilename = _ConfigName;
			Error err = _Config.Load(BasePath + LoadedConfigFilename);

			if (err == Error.Ok)
			{
				LoadOk = true;
			}
		}
		
		/// <summary>
        /// Retrieves a single key value from the configuration.
        /// </summary>
        /// <param name="_key">The key to retrieve the value for.</param>
        /// <returns>The value associated with the specified key.</returns>
		public virtual Variant GetKey( string _key )
		{
			return _Config.GetValue( DefaultConfigSection, _key );
		}
		
		/// <summary>
        /// Sets a value in the configuration and saves it.
        /// </summary>
        /// <param name="_key">The key to set the value for.</param>
        /// <param name="_value">The value to set.</param>
        /// <returns>Void.</returns>
		public virtual void SetKey( string _key, Variant _value )
		{
			_Config.SetValue(Name, _key, _value);
			_Config.Save(BasePath + LoadedConfigFilename);
			GlobalExplorer.GetInstance()._Plugin.EmitSignal(Plugin.SignalName.SettingKeyChanged, new Godot.Collections.Array() { _key, _value });
		}
	} 
}