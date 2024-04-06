
using System.Text.RegularExpressions;
using AssetSnap.Front.Nodes;
using Godot;

namespace AssetSnap.Static
{
	public static class HandleStatic
	{
		public static Node3D Get()
		{
			return GlobalExplorer.GetInstance().GetHandle();	
		}
		
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