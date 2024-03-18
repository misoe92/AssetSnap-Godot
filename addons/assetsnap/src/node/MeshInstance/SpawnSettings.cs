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

namespace AssetSnap.ASNode.MeshInstance
{
	using System;
	using AssetSnap.Front.Components;
	using Godot;

    public partial class SpawnSettings
	{
		GlobalExplorer _Explorer;
			
		Godot.Collections.Array<string> Keys = new()
		{
			"_LSSnapObject.state",
			"_LSSnapOffsetX.value",
			"_LSSnapOffsetZ.value",
			"_LSSnapToHeight.state",
			"_LSSnapToHeight.UsingGlue",
			"_LSSnapToHeight.SnapHeight",
			"_LSSnapToX.state",
			"_LSSnapToX.UsingGlue",
			"_LSSnapToX.SnapXValue",
			"_LSSnapToZ.state",
			"_LSSnapToZ.UsingGlue",
			"_LSSnapToZ.SnapZValue",
			"_LSSimpleSphereCollision.state",
			"_LSConvexPolygonCollision.state",
			"_LSConvexPolygonCollision.clean",
			"_LSConvexPolygonCollision.simplify",
			"_LSConcaveCollision.state",
			"_LSSnapLayer.value",
		};
		
		Godot.Collections.Dictionary<string, Variant> _data;
		
		public SpawnSettings()
		{
			_Explorer = GlobalExplorer.GetInstance();
			_data = new();
		}
		
		public Variant Get( string key )
		{
			try 
			{
				return _data[key];
			}
			catch( Exception e )
			{
				// GD.Print(e.Message);
			}

			return (Variant)false;
		}
		
		public Godot.Collections.Dictionary<string, Variant> Data()
		{
			return _data;
		}
		
		public bool Has( string key )
		{
			return _data.ContainsKey(key);
		}
		
		public void Add( string key, Variant value )
		{
			_data.Add(key, value);
		}
		
		public void Update()
		{
			Clear();
			foreach( string key in Keys ) 
			{
				if( false == _data.ContainsKey(key) ) 
				{
					var keyArray = key.Split(".");
					string keyName = keyArray[0];
					string keyField = keyArray[1];

					LibrarySettings librarySettings = _Explorer.CurrentLibrary._LibrarySettings;
					
					if( true == librarySettings.FieldExists(keyName) ) 
					{
						Add(key, librarySettings.AccessField(keyName).GetValueFor(keyField) );
					}
				}
			}
		}
		 
		public void Remove( string key )
		{
			_data.Remove(key);
		}
		
		public void Clear()
		{
			_data = new();
		}
	}
}