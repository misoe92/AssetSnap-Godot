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

using AssetSnap.Explorer;
using Godot;

namespace AssetSnap.Component
{
	/// <summary>
	/// Base class for components related to library management.
	/// </summary>
	[Tool]
	public partial class LibraryComponent : TraitableComponent
	{
		/// <summary>
		/// Gets the library instance associated with this component.
		/// </summary>
		public Library.Instance Library 
		{
			get {
				return ExplorerUtils.Get().GetLibraryByName(LibraryName);
			}
		}

		/// <summary>
		/// Gets or sets the name of the library.
		/// </summary>
		public string LibraryName { get; set; }
		
		/// <summary>
		/// Virtual method called each time a library change occurs.
		/// </summary>
		public virtual void _OnLibraryChange(){}
		
		/// <summary>
		/// Synchronizes the component.
		/// </summary>
		public virtual void Sync(){}
		
		/// <summary>
		/// Updates the spawn settings of the current handle.
		/// </summary>
		/// <param name="key">The key of the setting to update.</param>
		/// <param name="value">The value to set for the setting.</param>
		public void UpdateSpawnSettings(string key, Variant value) 
		{
			Node3D _handle = GetHandle();
			if( _handle is AssetSnap.Front.Nodes.AsMeshInstance3D asMeshInstance3D ) 
			{
				if( asMeshInstance3D.HasSetting(key) )
				{
					asMeshInstance3D.RemoveSetting(key);
				}

				asMeshInstance3D.AddSetting(key, value);
			}
		}
	}
}

#endif