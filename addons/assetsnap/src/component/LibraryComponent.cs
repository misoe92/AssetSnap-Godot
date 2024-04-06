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
namespace AssetSnap.Component
{
	using Godot;

	[Tool]
	public partial class LibraryComponent : TraitableComponent
	{
		protected Library.Instance _Library;
		
		public Library.Instance Library 
		{
			get => _Library;
			set
			{
				_Library = value;
				_OnLibraryChange();
			}
		}
		
		/*
		** Virtual method which are called each time
		** a library change is happening.
		**
		** @return void
		*/
		public virtual void _OnLibraryChange(){}
		
		public virtual void Sync(){}
		
		/*
		** Updates the current handle's spawn settings
		** This can be data like the collision type used
		** and more
		**
		** @param string key
		** @param Variant value
		** @return void
		*/
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

		public override void _ExitTree()
		{
			Library = null;
			
			base._ExitTree();
		}
	}
}
#endif