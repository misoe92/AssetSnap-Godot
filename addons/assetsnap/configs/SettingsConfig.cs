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

using System;
using System.Collections.Generic;
using System.Linq;
using AssetSnap.Settings;
using AssetSnap.States;
using Godot;

namespace AssetSnap.Front.Configs
{
	/// <summary>
	/// Represents the settings configuration.
	/// </summary>
	public partial class SettingsConfig : Config.BaseConfig
	{
		private string _ConfigPath;
		private string[] _Folders;
		private Godot.Collections.Dictionary<string, Variant> _Settings;
		private BaseContainer _Container;
		public bool Initialized = false;

		/// <summary>
		/// Gets the singleton instance of SettingsConfig.
		/// </summary>
		public static SettingsConfig Singleton
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = new SettingsConfig();
					_Instance.Initialize();
				}

				return _Instance;
			}
		}
		private static SettingsConfig _Instance = null;

		/// <summary>
		/// Gets the name of the settings configuration.
		/// </summary>
		public string _Name
		{
			get => "Settings";
		}

		/// <summary>
		/// Gets the configuration file.
		/// </summary>
		public ConfigFile Config
		{
			get => _Config;
		}

		/// <summary>
		/// Gets the count of folders.
		/// </summary>
		public int FolderCount
		{
			get => _Folders.Length;
		}

		/// <summary>
		/// Gets the array of folders.
		/// </summary>
		public string[] Folders
		{
			get => _Folders;
		}

		/// <summary>
		/// Gets the settings dictionary.
		/// </summary>
		public Godot.Collections.Dictionary<string, Variant> Settings
		{
			get => _Settings;
		}

		/// <summary>
		/// Gets the UI container.
		/// </summary>
		public BaseContainer Container
		{
			get => _Container;
		}

		/// <summary>
		/// Constructor for SettingsConfig.
		/// </summary>
		public SettingsConfig()
		{
			Name = "AssetSnapConfig";
		}

		/// <summary>
		/// Initializes the settings configuration.
		/// </summary>
		public void Initialize()
		{
			if (null == _Instance)
			{
				_Instance = this;
			}

			_ConfigPath = "config.cfg";
			LoadConfig(_ConfigPath);

			// If the file didn't load, ignore it.
			if (LoadOk)
			{
				List<string> _FolderList = new();
				_Settings = new();

				if (_Config.HasSection("Folders"))
				{
					string[] FoldersSections = _Config.GetSectionKeys("Folders");

					for (int i = 0; i < FoldersSections.Length; i++)
					{
						var Section = FoldersSections[i];
						var Value = _Config.GetValue("Folders", Section);
						_FolderList.Add(Value.ToString());
					}
				}

				if (_Config.HasSection(_Name))
				{
					string[] SettingsSections = _Config.GetSectionKeys(_Name);
					for (int i = 0; i < SettingsSections.Length; i++)
					{
						var Section = SettingsSections[i];
						var Value = _Config.GetValue(_Name, Section);

						_Settings.Add(Section, Value);
					}
				}

				_Folders = _FolderList.ToArray();
				_FolderList.Clear();
				Initialized = true;
			}
			else
			{
				GD.PushError("Invalid Config @ SettingsConfig");
				return;
			}
		}

		/// <summary>
		/// Emits the signal when folders are loaded.
		/// </summary>
		public void MaybeEmitFoldersLoaded()
		{
			if (null != Plugin.GetInstance())
			{
				Plugin.GetInstance().EmitSignal(Plugin.SignalName.FoldersLoaded);
			}
		}

		/// <summary>
		/// Resets the settings configuration.
		/// </summary>
		/// <param name="WithContainer">Determines whether to reset with container.</param>
		public void Reset(bool WithContainer = true)
		{
			if (null != _Container && WithContainer)
			{
				if (null != _Container.GetParent() && EditorPlugin.IsInstanceValid(_Container))
				{
					_Container.GetParent().RemoveChild(_Container);
				}

				_Container.Free();
			}
		}

		/// <summary>
		/// Initializes the settings UI Container.
		/// </summary>
		public void InitializeContainer()
		{
			if (EditorPlugin.IsInstanceValid(_Container))
			{
				if (null != _Container.GetParent() && EditorPlugin.IsInstanceValid(_Container))
				{
					_Container.GetParent().RemoveChild(_Container);
				}

				_Container.QueueFree();
			}

			if (FolderCount == 0)
			{
				return;
			}

			_Container = new();
			_Container.Initialize();

			StatesUtils.SetLoad("SettingsContainer", true);
		}

		/// <summary>
		/// Sets a config key value.
		/// </summary>
		/// <param name="_key">The key to set.</param>
		/// <param name="_Value">The value to set.</param>
		public override void SetKey(string _key, Variant _Value)
		{
			if (_Settings.ContainsKey(_key) == false)
			{
				return;
			}

			_Config.SetValue(_Name, _key, _Value);
			_Settings[_key] = _Value;
			GlobalExplorer.GetInstance()._Plugin.EmitSignal(Plugin.SignalName.SettingKeyChanged, new Godot.Collections.Array() { _key, _Value });
			Error result = _Config.Save(BasePath + _ConfigPath);

			if (result != Error.Ok)
			{
				GD.PushError(result);
			}

			Reset(false);
		}

		/// <summary>
		/// Adds a folder to the array.
		/// </summary>
		/// <param name="path">The path of the folder to add.</param>
		public void AddFolder(string path)
		{
			if (_Folders.Contains(path))
			{
				GD.PushError("Library with the same name already exists, and as such cannot be added.");
				return;
			}

			_Config.SetValue("Folders", "Folder" + (FolderCount + 1), path);
			Error result = _Config.Save(BasePath + _ConfigPath);

			if (result != Error.Ok)
			{
				GD.PushError(result);
			}

			LoadOk = false;

			Initialize();
			MaybeEmitFoldersLoaded();
		}

		/// <summary>
		/// Adds model size to cache.
		/// </summary>
		/// <param name="name">The name of the model.</param>
		/// <param name="Size">The size of the model.</param>
		public void AddModelSizeToCache(string name, Vector3 Size)
		{
			_Config.SetValue("ModelSizes", name, Size);
			Error result = _Config.Save(BasePath + _ConfigPath);

			if (result != Error.Ok)
			{
				GD.PushError(result);
			}

			Plugin.Singleton.EmitSignal(Plugin.SignalName.ModelSizeCacheChanged, new Variant[] { name, Size });
		}

		/// <summary>
		/// Checks if model size exists.
		/// </summary>
		/// <param name="name">The name of the model.</param>
		/// <returns>True if the model size exists, false otherwise.</returns>
		public bool HasModelSize(string name)
		{
			return _Config.HasSection("ModelSizes") && _Config.GetSectionKeys("ModelSizes").Contains(name);
		}

		/// <summary>
        /// Gets the model size.
        /// </summary>
        /// <param name="name">The name of the model.</param>
        /// <returns>The size of the model.</returns>
		public Vector3 GetModelSize(string name)
		{
			if (HasModelSize(name))
			{
				return _Config.GetValue("ModelSizes", name).As<Vector3>();
			}

			return Vector3.Zero;
		}

		/// <summary>
        /// Removes a folder from the array.
        /// </summary>
        /// <param name="path">The path of the folder to remove.</param>
		public void RemoveFolder(string path)
		{
			string[] keys = _Config.GetSectionKeys("Folders");

			if (keys.Length != 0)
			{
				foreach (string key in keys)
				{
					string _Path = _Config.GetValue("Folders", key).As<string>();
					if (_Path == path)
					{
						_Config.EraseSectionKey("Folders", key);
					}
				}
			}

			Error result = _Config.Save(BasePath + _ConfigPath);

			if (result != Error.Ok)
			{
				GD.PushError(result);
			}

			LoadOk = false;
			Initialize();
			MaybeEmitFoldersLoaded();
		}

		/// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="_key">The key whose value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
		public override Variant GetKey(string _key)
		{
			if (null == _Settings || false == _Settings.ContainsKey(_key))
			{
				return false;
			}

			return _Settings[_key];
		}

		/// <summary>
        /// Converts a key to label.
        /// </summary>
        /// <param name="key">The key to convert.</param>
        /// <returns>The label associated with the key.</returns>
		public string KeyToLabel(string key)
		{
			return key.Capitalize().Split('_').Join(" ");
		}

		/// <summary>
        /// Fetches the settings.
        /// </summary>
        /// <returns>The settings dictionary.</returns>
		public Godot.Collections.Dictionary<string, Variant> GetSettings()
		{
			return _Settings;
		}
	}
}