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

namespace AssetSnap.Config
{
	using Godot;
	
	[Tool]
	public partial class BaseConfig : Node
	{
		protected readonly string BasePath = "res://addons/assetsnap/"; 
		protected readonly string DefaultConfigSection = "Settings";
		protected string LoadedConfigFilename;
		protected bool LoadOk; 
		protected ConfigFile _Config;
		
		/*
		** Construction of our config
		*/		
		public BaseConfig()
		{
			Name = "AssetSnapConfig";
			LoadOk = false;
		}

		/*
		** Loads our config given a specified config file name
		** It overwrites the current _Config field.
		**
		** @param string _ConfigName
		** @return void
		*/
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
		
		/*
		** Fetches a single key value from the config
		**
		** @param string _key
		** @return Variant
		*/
		public virtual Variant GetKey( string _key )
		{
			return _Config.GetValue( DefaultConfigSection, _key );
		}
		
		/*
		** Sets a value in the config and saves it
		**
		** @param string _key
		** @param Variant _value
		** @return void
		*/
		public virtual void SetKey( string _key, Variant _value )
		{
			_Config.SetValue(Name, _key, _value);
			_Config.Save(BasePath + LoadedConfigFilename);
			GlobalExplorer.GetInstance()._Plugin.EmitSignal(Plugin.SignalName.SettingKeyChanged, new Godot.Collections.Array() { _key, _value });
		}

		public override void _ExitTree()
		{
			_Config = null;

			base._ExitTree();
		}
	} 
}