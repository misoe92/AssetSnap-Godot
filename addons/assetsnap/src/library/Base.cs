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

using AssetSnap.Explorer;
using AssetSnap.Front.Components;
using Godot;

namespace AssetSnap.Library
{
 	/// <summary>
	/// Base class for managing libraries.
	/// </summary>
	[Tool]
	public partial class Base : Node, ISerializationListener
	{
		[Export]
		public Godot.Collections.Array<GodotObject> _Libraries = new();
		private static Base _Instance;
		
		/// <summary>
		/// Gets the singleton instance of the Base class.
		/// </summary>
		public static Base Singleton
		{
			get
			{
				// if (_Instance == null)
				// {
				// 	_Instance = new();
				// }

				return _Instance;
			}
		}

		/// <summary>
		/// Gets the array of libraries.
		/// </summary>
		public Godot.Collections.Array<GodotObject> Libraries
		{
			get => _Libraries;
		}

		/// <summary>
		/// Constructor for the Base class.
		/// </summary>
		public Base()
		{
			Name = "LibraryBase";
			_Instance = this;
		}
		
		/// <summary>
		/// This method is called before the object is serialized.
		/// </summary>
		public void OnBeforeSerialize()
		{
			//
		}

		/// <summary>
		/// This method is called after the object has been deserialized.
		/// </summary>
		public void OnAfterDeserialize()
		{
			_Instance = this;
		}

		/// <summary>
		/// Initializes the libraries.
		/// </summary>
		public void Initialize()
		{
			if (Libraries.Count > 0)
			{
				int MaxCount = Libraries.Count;
				for (int i = MaxCount - 1; i >= 0; i--)
				{
					if (Libraries.Count > i)
					{
						GodotObject instance = Libraries[i];
						if (IsInstanceValid(instance) && instance is Instance finalInstance)
						{
							RemoveLibrary(finalInstance);
						}

						_Libraries.Remove(instance);
					}
				}
			}

			/** Initialize libraries **/
			if (ExplorerUtils.Get().Settings.FolderCount != 0)
			{
				string[] Folders = ExplorerUtils.Get().Settings.Folders;

				for (int i = 0; i < ExplorerUtils.Get().Settings.FolderCount; i++)
				{
					string Folder = Folders[i];
					New(ExplorerUtils.Get()._Plugin.GetTabContainer(), Folder, i);
				}
			}
		}

		/// <summary>
		/// Creates a new instance of Library.
		/// </summary>
		/// <param name="Dock">The tab container to dock.</param>
		/// <param name="_Folder">The folder path.</param>
		/// <param name="index">The index.</param>
		public void New(TabContainer Dock, string _Folder, int index)
		{
			if (false == IsGlobalExplorerValid())
			{
				return;
			}

			Instance _Base = new()
			{
				Folder = _Folder,
				Index = index,
				Dock = Dock,
			};
			
			_Libraries.Add(_Base);

			_Base.Name = _Base.FileName.Capitalize();
			_Base._Name = _Base.FileName.Capitalize();

			_Base.Initialize();
		}

		/// <summary>
		/// Checks if there is already a library with the specified folder path.
		/// </summary>
		/// <param name="Folder">The folder path to check.</param>
		/// <returns>True if a library with the specified folder path exists; otherwise, false.</returns>
		private bool AlreadyHaveFolder(string Folder)
		{
			foreach (Library.Instance instance in _Libraries)
			{
				if (Folder == instance._Folder)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Cleans up an old library with the specified folder path.
		/// </summary>
		/// <param name="Folder">The folder path of the library to clean up.</param>
		/// <returns>True if an old library was cleaned up; otherwise, false.</returns>
		private bool CleanOldLibrary(string Folder)
		{
			foreach (Library.Instance instance in _Libraries)
			{
				if (Folder == instance._Folder)
				{
					instance.Clear();

					instance.GetParent().RemoveChild(instance);
					instance.QueueFree();

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Rebinds the settings container to the bottom dock.
		/// </summary>
		private void RebindSettingsContainer()
		{
			ExplorerUtils.Get().Settings.InitializeContainer();
			if (EditorPlugin.IsInstanceValid(ExplorerUtils.Get().Settings.Container))
			{
				ExplorerUtils.Get().Settings.Container.Name = "Settings";
				ExplorerUtils.Get().BottomDock.Add(ExplorerUtils.Get().Settings.Container);
			}
		}

		/// <summary>
		/// Removes the specified library from the collection.
		/// </summary>
		/// <param name="library">The library instance to remove.</param>
		public void RemoveLibrary(Instance library)
		{
			if (_Libraries.Count != 0)
			{
				library.Clear();
				library.GetParent().RemoveChild(library);
				library.QueueFree();
			}
		}

		/// <summary>
		/// Resets all libraries in the collection.
		/// </summary>
		public void Reset()
		{
			foreach (Instance Library in _Libraries)
			{
				Library.ClearAllPanelState();
				Library._LibrarySettings.ClearAll();
			}
		}

		/// <summary>
		/// Gets the library listing component.
		/// </summary>
		/// <returns>The library listing component.</returns>
		private LibrariesListing GetLibraryListing()
		{
			LibrariesListing _LibrariesListing = ExplorerUtils.Get().Components.Single<LibrariesListing>();
			return _LibrariesListing;
		}

		/// <summary>
		/// Checks if there are any folders found.
		/// </summary>
		/// <returns>True if folders are found; otherwise, false.</returns>
		private bool HasFolders()
		{
			return ExplorerUtils.Get().Settings.FolderCount != 0;
		}

		/// <summary>
		/// Checks if the library listing exists.
		/// </summary>
		/// <returns>True if the library listing exists; otherwise, false.</returns>
		private bool HasLibraryListing()
		{
			LibrariesListing _LibrariesListing = ExplorerUtils.Get().Components.Single<LibrariesListing>();
			return null != _LibrariesListing;
		}

		/// <summary>
		/// Checks if the global class GlobalExplorer reference is available.
		/// </summary>
		/// <returns>True if GlobalExplorer is valid; otherwise, false.</returns>
		private bool IsGlobalExplorerValid()
		{
			return ExplorerUtils.IsValid();
		}
	}
}