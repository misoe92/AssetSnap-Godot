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

namespace AssetSnap.Front.Configs
{
	using System;
	using System.Collections.Generic;
	using AssetSnap.Settings;
	using AssetSnap.States;
	using Godot;

	public partial class SettingsConfig : Config.BaseConfig
	{
		[Signal]
		public delegate void FoldersLoadedEventHandler();
		
		private string _ConfigPath;
		private string[] _Folders;
		private Godot.Collections.Dictionary<string,Variant> _Settings;
		private BaseContainer _Container;
		
		public string _Name 
		{
			get => "Settings";
		}
		
		public ConfigFile Config
		{
			get => _Config;	
		}
		
		public int FolderCount 
		{
			get => _Folders.Length;
		}
		
		public string[] Folders 
		{
			get => _Folders;
		}
		
		public Godot.Collections.Dictionary<string,Variant> Settings 
		{
			get => _Settings;
		}
		
		public BaseContainer Container 
		{
			get => _Container;
		}
		
		public SettingsConfig()
		{ 
			Name = "AssetSnapConfig";
		}
		
		/* 
		** Initializes settings config
		** 
		** @return void
		*/
		public void Initialize() 
		{
			_ConfigPath = "config.cfg";
			LoadConfig( _ConfigPath );
			
			// If the file didn't load, ignore it.
			if (LoadOk)
			{
				List<string> _FolderList = new();
				_Settings = new();
				
				if( _Config.HasSection( "Folders") ) 
				{
					string[] FoldersSections = _Config.GetSectionKeys("Folders");
					
					for( int i = 0; i < FoldersSections.Length; i++) 
					{
						var Section = FoldersSections[i];
						var Value = _Config.GetValue("Folders", Section);
						_FolderList.Add(Value.ToString());
					}
				}
				
				if( _Config.HasSection(_Name) ) 
				{
					string[] SettingsSections = _Config.GetSectionKeys(_Name);
					for( int i = 0; i < SettingsSections.Length; i++) 
					{
						var Section = SettingsSections[i];
						var Value = _Config.GetValue(_Name, Section);
						
						_Settings.Add(Section, Value); 
					}
				}
				
				_Folders = _FolderList.ToArray();
				_FolderList.Clear();
			}
			else 
			{
				GD.PushError("Invalid Config @ SettingsConfig");
				return;
			}
		}
		
		public void MaybeEmitFoldersLoaded()
		{
			if( 0 != FolderCount ) 
			{
				EmitSignal(SignalName.FoldersLoaded);
			}
		}
		
		/*
		** Resets the settings config
		** 
		** @param bool WithContainer[true]
		** @return void
		*/
		public void Reset( bool WithContainer = true ) 
		{
			_Folders = Array.Empty<string>();
			_Settings = new();
			if( null != _Container && WithContainer )  
			{
				if( null != _Container.GetParent() && IsInstanceValid(_Container) ) 
				{
					_Container.GetParent().RemoveChild(_Container);
				}

				_Container.Free();
			}
			
			Initialize();
			MaybeEmitFoldersLoaded();
		}

		/*
		** Initializes the settings UI Container
		** 
		** @return void 
		*/
		public void InitializeContainer()
		{
			if( IsInstanceValid( _Container ) ) 
			{
				if( null != _Container.GetParent() && IsInstanceValid(_Container) ) 
				{
					_Container.GetParent().RemoveChild(_Container);
				}
	
				_Container.QueueFree();
			}
			
			if( FolderCount == 0 ) 
			{
				return;	
			}
			
			_Container = new();
			_Container.Initialize();
			
			StatesUtils.SetLoad("SettingsContainer", true);
		} 
		
		/*
		** Sets a config key value
		** 
		** @param string _key
		** @param Variant _value
		** @return void
		*/
		public override void SetKey( string _key, Variant _Value )
		{
			if( _Settings.ContainsKey(_key) == false ) 
			{
				return; 
			}
			
			_Config.SetValue(_Name, _key, _Value);
			_Settings[_key] = _Value;
			GlobalExplorer.GetInstance()._Plugin.EmitSignal(Plugin.SignalName.SettingKeyChanged, new Godot.Collections.Array() { _key, _Value });
			Error result = _Config.Save( BasePath + _ConfigPath );

			if( result != Error.Ok ) 
			{
				GD.PushError(result);
			}

			Reset(false);
		}
		
		/*
		** Adds a folder to the array
		** 
		** @return void
		*/
		public void AddFolder( string path )
		{
			_Config.SetValue("Folders", "Folder" + ( FolderCount + 1 ), path);
			Error result = _Config.Save( BasePath + _ConfigPath );

			if( result != Error.Ok ) 
			{
				GD.PushError(result);
			}
		}
		
		/*
		** Removes an folder from the array
		** 
		** @return void
		*/
		public void RemoveFolder( string path )
		{
			string[] keys = _Config.GetSectionKeys("Folders");
			
			if( keys.Length != 0 ) 
			{
				foreach( string key in keys ) 
				{
					string _Path = _Config.GetValue("Folders", key).As<string>();
					if( _Path == path ) 
					{
						_Config.EraseSectionKey("Folders", key);
					}
				}
			}
			
			Error result = _Config.Save( BasePath + _ConfigPath );

			if( result != Error.Ok ) 
			{
				GD.PushError(result);
			}
		}

		/*
		** Initializes the settings UI Container
		** 
		** @return void
		*/
		public override Variant GetKey( string _key )
		{
			if( null == _Settings ||  false == _Settings.ContainsKey(_key) ) 
			{
				return false;
			}
			
			return _Settings[_key];
		}
		
		/*
		** Converts key to label
		** 
		** @param string _key
		** @return string
		*/
		public string KeyToLabel( string key ) 
		{
			return key.Capitalize().Split('_').Join(" ");
		}
		
		/*
		** Fetches the settings
		** 
		** @return Godot.Collections.Dictionary<string,Variant>
		*/
		public Godot.Collections.Dictionary<string,Variant> GetSettings()
		{
			return _Settings;
		}
		
		/*
		** Cleans up references, parameters and fields
		** 
		** @return void 
		*/	
		public override void _ExitTree()
		{
			_ConfigPath = null;
			_Folders = null;
			_Settings = null; 

			if( IsInstanceValid(_Container)) 
			{
				_Container.QueueFree();
			}

			base._ExitTree();
		}
	}
}