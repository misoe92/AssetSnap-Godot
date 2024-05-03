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

using AssetSnap.Front.Nodes;
using Godot;

namespace AssetSnap.Static
{
	/// <summary>
	/// Utility class for handling static objects in the scene.
	/// </summary>
	public static class HandleStatic
	{
		/// <summary>
		/// Retrieves the handle node for static objects in the scene.
		/// </summary>
		/// <returns>The handle node.</returns>
		public static Node3D Get()
		{
			return GlobalExplorer.GetInstance().GetHandle();	
		}
		
		/// <summary>
		/// Updates the properties of grouped objects in the scene.
		/// </summary>
		/// <param name="index">The index of the child object in the group.</param>
		/// <param name="key">The property key to update.</param>
		/// <param name="value">The new value for the property.</param>
		public static void MaybeUpdateGrouped(int index, string key, Variant value)
		{
			string GroupPath = GlobalExplorer.GetInstance().GroupBuilder._Editor.GroupPath;
			
			if( "" != GroupPath && GlobalExplorer.GetInstance().States.GroupedObjects.ContainsKey(GroupPath) ) 
			{
				Godot.Collections.Array<AsGrouped3D> GroupObjects = GlobalExplorer.GetInstance().States.GroupedObjects[GroupPath];
				
				for( int i = 0; i < GroupObjects.Count; i++ ) 
				{
					if( GroupObjects[i] is AsGrouped3D asGrouped3D ) 
					{
						if( asGrouped3D.ChildOptions.Count < i ) 
						{
							continue;
						}
						
						if( false == asGrouped3D.ChildOptions[index].ContainsKey( key ) ) 
						{
							asGrouped3D.ChildOptions[index].Add(key, value);	
						}
						else 
						{
							asGrouped3D.ChildOptions[index][key] = value;	
						}

						asGrouped3D.Update();
					}
				}
			}
		}
		
		/// <summary>
        /// Updates the properties of a single grouped object in the scene.
        /// </summary>
        /// <param name="index">The index of the child object in the group.</param>
        /// <param name="key">The property key to update.</param>
        /// <param name="value">The new value for the property.</param>
		public static void MaybeUpdateGroup( int index, string key, Variant value)
		{
			Node3D Handle = Get();
			if( Handle is AsGrouped3D asGrouped3D ) 
			{
				Node3D Child = asGrouped3D.GetChild(index) as Node3D;
				if( Child is AsMeshInstance3D meshInstance3D ) 
				{
					if( meshInstance3D.HasSetting( key ) ) 
					{
						meshInstance3D.RemoveSetting(key);
					}
					
					meshInstance3D.AddSetting(key, value);
				}
				
				if( Child is AsStaticBody3D staticBody3D && staticBody3D.GetChild(0) is AsMeshInstance3D asMeshInstance3D) 
				{
					if( asMeshInstance3D.HasSetting( key ) ) 
					{
						asMeshInstance3D.RemoveSetting(key);
					}
					
					asMeshInstance3D.AddSetting(key, value);
				}

				asGrouped3D.Update();
			}
		}
	}
}