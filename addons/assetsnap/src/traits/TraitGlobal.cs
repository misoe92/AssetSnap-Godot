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

namespace AssetSnap.Trait
{
	using Godot;
	using Godot.Collections;

	[Tool]
	public partial class TraitGlobal : Node, ISerializationListener
	{
		private string version = "0.0.1";
		public static TraitGlobal _Instance = null;

		[Export]
		public Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<int, string>>> Names { get; set; } = new();
		
		[Export]
		public Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<int, GodotObject>>> Instances { get; set; } = new();
		
		[Export]
		public Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string,Godot.Collections.Dictionary<int, Godot.Collections.Dictionary<string, Variant>>>> InstanceDependencies { get; set; } = new();

		[Export]
		public Godot.Collections.Array<GodotObject> DisposeQueue = new();
		
		public static TraitGlobal Singleton {
			get
			{
				if( _Instance == null || false == EditorPlugin.IsInstanceValid( _Instance ) ) 
				{
					_Instance = new()
					{
						IsSingleton = true,
					};
				}

				return _Instance;
			}
		}
		
		public TraitGlobal()
		{
			Name = "TraitGlobal";
		}

		public bool IsSingleton = false;

		public string GetName( int index, string typeString, string owner, bool debug = false ) 
		{
			string typeKey = FormatTypeString(typeString);
			
			if( false == Names.ContainsKey(owner) ) 
			{
				if( debug ) 
				{
					GD.PushError("No name with owner: ", owner );
				}

				return "NaN";
			}
			
			if( Names.ContainsKey(owner) && false == Names[owner].ContainsKey(typeKey) ) 
			{
				if( debug ) 
				{
					GD.PushError("No name with type: ", typeKey, "::", owner, Names[owner].Keys);
				}
				
				return "NaN";
			}

			if( Names.ContainsKey(owner) && Names[owner].ContainsKey(typeKey) && false == Names[owner][typeKey].ContainsKey(index) ) 
			{
				if( debug ) 
				{
					GD.PushError("No name at that index: ", index, Names[owner][typeKey].Keys, "--", typeKey, "--", owner);
				}
				
				return "NaN";
			}
			
			return Names[owner][typeKey][index];
		}
		
		public void AddName( int index, string name, string owner, string typeString ) 
		{
			if( null == typeString || null == owner ) 
			{
				return;
			}
			string typeKey = typeString.Split(".").Join("");

			if (false == Names.ContainsKey(owner))
			{
				Names.Add(owner, new()); 
			}
			
			if (false == Names[owner].ContainsKey(typeKey))
			{
				Names[owner].Add(typeKey, new()); 
			}
			
			if (false == Names[owner][typeKey].ContainsKey(index))
			{
				Names[owner][typeKey].Add(index, name); 
			}
			else 
			{
				Names[owner][typeKey][index] = name; 
			}

		}
		
		public void RemoveName( int index, string owner, string typeString, bool debug = false )
		{
			// GD.Print(typeString, "::", owner, " removes");
			if( null == typeString || null == owner ) 
			{
				return;
			}
			string typeKey = typeString.Split(".").Join("");
			
			if( false == Names.ContainsKey( owner ) ) 
			{
				if( debug ) 
				{
					GD.PushError("Invalid owner: ", owner, Names.Keys);
				}
				
				return;
			}
			
			if( false == Names[owner].ContainsKey( typeKey ) ) 
			{
				if( debug ) 
				{
					GD.PushError("Invalid type: ", typeKey, Names[owner].Keys);
				}
		
				return;
			}
			
			if( false == Names[owner][typeKey].ContainsKey( index ) ) 
			{
				if( debug ) 
				{
					GD.PushError("Invalid index");
				}
				return;
			}

			Names[owner][typeKey].Remove(index);

			if( Names[owner][typeKey].Count == 0 ) 
			{
				Names[owner].Remove(typeKey);
			}
			
			if( Names[owner].Count == 0 ) 
			{
				Names.Remove(owner);
			}
		}
		
		public string FormatTypeString(string TypeString ) 
		{
			return TypeString.Split(".").Join("");
		}
		
		public Dictionary<int, GodotObject> AllInstances(string typeString, string owner, bool debug = false)
		{
			if( null == typeString || null == owner ) 
			{
				return null;
			}

			string typeKey = FormatTypeString(typeString);
			
			if(
				false == Instances.ContainsKey(owner)
			) 
			{
				if( debug )
				{
					GD.PushError("Owner not found");
				}
				
				return null;
			}
			
			if(
				Instances.ContainsKey(owner) &&
				false == Instances[owner].ContainsKey(typeKey)
			) 
			{
				if( debug )
				{
					GD.PushError("type not found", typeKey);
				}
				return null;
			}

			return Instances[owner][typeKey];
		}
		
		public bool HasInstance( int index, string typeString, string owner, bool debug = false ) 
		{
			if( null == typeString || null == owner ) 
			{
				return false;
			}
			
			if( false == Instances.ContainsKey(owner) ) 
			{
				if( debug ) 
				{
					GD.PushError("No instance at owner: ", owner);
				}

				return false;
			}

			string typeKey = FormatTypeString(typeString);
			if( false == Instances[owner].ContainsKey(typeKey) ) 
			{
				if( debug ) 
				{
					GD.PushError("No instance at type: ", typeKey, "::", owner, Instances[owner].Keys);
				}
				return false;
			}
			
			if( false == Instances[owner][typeKey].ContainsKey(index) ) 
			{
				if( debug ) 
				{
					GD.PushError("No instance at index: ", index, " :: ", typeKey, "::", owner, Instances[owner][typeKey].Keys, Instances[owner].Keys);
				}
				return false;
			}
			
			bool res = EditorPlugin.IsInstanceValid(Instances[owner][typeKey][index]);
			
			if( false == res ) 
			{
				Instances[owner][typeKey].Remove(index);
			}

			return res;
		}
	
		public GodotObject GetInstance( int index, string typeString, string owner, bool debug = false )
		{
			if( null == typeString || null == owner ) 
			{
				return null;
			}
			
			if( false == Instances.ContainsKey(owner) ) 
			{
				if( debug ) 
				{
					GD.PushError("No instance at owner: ", owner);
				}

				return null;
			}

			string typeKey = FormatTypeString(typeString);
			if( false == Instances[owner].ContainsKey(typeKey) ) 
			{
				if( debug ) 
				{
					GD.PushError("No instance at type: ", typeKey, "::", owner, Instances[owner].Keys);
				}
				return null;
			}
			
			if( false == Instances[owner][typeKey].ContainsKey(index) ) 
			{
				if( debug ) 
				{
					GD.PushError("No instance at index: ", index, " :: ", typeKey, "::", owner, Instances[owner][typeKey].Keys, Instances[owner].Keys);
				}
				return null;
			}
			
			if( false == IsInstanceValid(Instances[owner][typeKey][index])) 
			{
				if( debug ) 
				{
					GD.PushError("Instance is invalid: ", index, " :: ", typeKey, "::", owner, Instances[owner][typeKey].Keys, Instances[owner].Keys);
				}
				return null;
			}
			
			return Instances[owner][typeKey][index];
		}
		
		public bool AddInstance( int index, Node Instance, string owner, string typeString, Godot.Collections.Dictionary<string, Variant> dependencies ) 
		{
			if( null == typeString || null == owner || null == Instance ) 
			{
				return false;
			}
			
			string typeKey = FormatTypeString(typeString);
			
			GodotObject existing = GetInstance(index, typeString, owner);
			
			if( existing != null ) 
			{
				// Already exist
				return false;
			}
			
			if( false == Instances.ContainsKey( owner ) ) 
			{
				Instances.Add(owner, new());
			}
			
			if( false == Instances[owner].ContainsKey(typeKey) ) 
			{
				Instances[owner].Add(typeKey, new());
			}

			Instances[owner][typeKey].Add(index, Instance);

			if( false == InstanceDependencies.ContainsKey(owner) )
			{
				InstanceDependencies.Add(owner, new());
			}
			
			if( false == InstanceDependencies[owner].ContainsKey(typeKey) )
			{
				InstanceDependencies[owner].Add(typeKey, new());
			}
			
			if( false == InstanceDependencies[owner][typeKey].ContainsKey(index)) 
			{
				InstanceDependencies[owner][typeKey].Add(index, dependencies);
			} 
			else 
			{
				InstanceDependencies[owner][typeKey][index] = dependencies;
			}
			
			return true;
		}
		
		public bool RemoveInstance( int index, string typeString, string owner, bool debug = false )
		{
			if( null == typeString || null == owner ) 
			{
				if( debug ) 
				{
					GD.PushError("No owner or type set: ", index, "::", typeString, "::", owner);
				}

				return false;
			}

			GodotObject instance = GetInstance(index, typeString, owner, debug);
			string typeKey = FormatTypeString(typeString);
			
			if( null == instance ) 
			{
				if( debug ) 
				{
					GD.PushError("No instance was found for: ", index, "::", typeKey, "::", owner);
				}
				return false;
			}
			
			if( EditorPlugin.IsInstanceValid(instance) && instance is Node node ) 
			{
				DisposeQueue.Add(node);
				if( null != node.GetParent() ) 
				{
					node.GetParent().RemoveChild(node);
				}

				Instances[owner][typeKey].Remove(index);
				InstanceDependencies[owner][typeKey].Remove(index);
				
				if( debug )
				{
					GD.Print("Trait Instance was removed at index(" + index + ")");
				}
				
				if( Instances[owner][typeKey].Count == 0 ) 
				{
					Instances[owner].Remove(typeKey);
					Plugin.Singleton.traitGlobal.RemoveName(index, owner, typeString);
					if( debug )
					{
						GD.PushError("Removed type: ", typeKey);
					}				
				}
				
				if( Instances[owner].Count == 0 ) 
				{
					Instances.Remove(owner);
					if( debug ) 
					{
						GD.PushError("Removed owner: ", owner);
					}
				}
				
				if( InstanceDependencies[owner][typeKey].Count == 0 ) 
				{
					InstanceDependencies[owner].Remove(typeKey);
					if( debug ) 
					{
						GD.PushError("Removed dependency type: ", typeKey);
					}
				}
				
				if( InstanceDependencies[owner].Count == 0 ) 
				{
					InstanceDependencies.Remove(owner);
					if( debug ) 
					{
						GD.PushError("Removed dependency owner: ", owner);
					}
				}

				if( debug ) 
				{
					GD.Print("Trait Instance count(" + Instances.Count + ") => Keys(", Instances.Keys, ")");
				}

				return true;
			}
			
			return false;
		}
		
		
		public Godot.Collections.Dictionary<string, Variant> GetDependencies( int index, string typeString, string name )
		{
			string typeKey = typeString.Split(".").Join("");
			if( false == InstanceDependencies.ContainsKey( name ) ) 
			{
				GD.PushError("No named dependency found: ", name);
				return null;
			}
			
			if( false == InstanceDependencies[name].ContainsKey( typeKey ) ) 
			{
				GD.PushError("No named dependency found: ", name);
				return null;
			}

			if( false == InstanceDependencies[name][typeKey].ContainsKey(index) )
			{
				GD.PushError("No indexed dependency found: " + index);
				return null;
			}

			return InstanceDependencies[name][typeKey][index];
		}
		
		public int Count()
		{
			return Instances.Count;
		}
		
		public int CountOwner( string owner, bool debug = false )
		{
			int count = 0;

			foreach ((string item, Dictionary<string, Dictionary<int, string>> obj) in Names)
			{
				if (item == owner)
				{
					foreach( (string typeKey, Dictionary<int, string> finalObj ) in obj ) 
					{
						count += finalObj.Count;
					}
				}
			}

			if( debug && count == 0 ) 
			{
				GD.Print("MISSINGOWNER::", owner);
			}

			return count;
		}

		public void OnBeforeSerialize()
		{
			// throw new NotImplementedException();
		}

		public void OnAfterDeserialize()
		{
			_Instance = this;
			// throw new NotImplementedException();
		}

		public override void _ExitTree()
		{
			// foreach( (string key, Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<int, GodotObject>> instance) in Instances) 
			// {
			// 	foreach(( string innerKey, Godot.Collections.Dictionary<int, GodotObject> innerInstance) in instance ) 
			// 	{
			// 		foreach((int index, GodotObject _object ) in innerInstance ) 
			// 		{
			// 			if( IsInstanceValid(_object) && _object is Node node ) 
			// 			{
			// 				if( IsInstanceValid(node) && node.HasMethod( Node.MethodName.Free ) ) 
			// 				{
			// 					if( null != node.GetParent() ) 
			// 					{
			// 						node.GetParent().RemoveChild(node);
			// 					}

			// 					node.QueueFree();
			// 				}
			// 				else 
			// 				{
			// 					GD.Print("Could not free", node.Name);
			// 				}
			// 			}
			// 		}
			// 	}
			// }
			
			// foreach( (string key, Godot.Collections.Dictionary<string,Godot.Collections.Dictionary<int, Godot.Collections.Dictionary<string, Variant>>> instance) in InstanceDependencies) 
			// {
			// 	foreach(( string innerKey, Godot.Collections.Dictionary<int, Godot.Collections.Dictionary<string, Variant>> innerInstance) in instance ) 
			// 	{
			// 		foreach((int index, Godot.Collections.Dictionary<string, Variant> finalInstance ) in innerInstance ) 
			// 		{
			// 			foreach((string finalKey, Variant _object ) in finalInstance ) 
			// 			{
			// 				if( IsInstanceValid(_object.AsGodotObject()) && _object.AsGodotObject() is Node node ) 
			// 				{
			// 					if( IsInstanceValid(node) && node.HasMethod( Node.MethodName.Free ) ) 
			// 					{
			// 						if( null != node.GetParent() ) 
			// 						{
			// 							node.GetParent().RemoveChild(node);
			// 						}

			// 						node.QueueFree();
			// 					}
			// 					else 
			// 					{
			// 						GD.Print("Could not free", node.Name);
			// 					}
			// 				}
			// 			}
			// 		}
			// 	}
			// }

			// Instances = new();
			// InstanceDependencies = new();
			

			// foreach( GodotObject _object in DisposeQueue ) 
			// {
			// 	if( IsInstanceValid(_object) && _object is Node node ) 
			// 	{
			// 		if( IsInstanceValid(node) && node.HasMethod( Node.MethodName.Free ) ) 
			// 		{
			// 			if( null != node.GetParent() ) 
			// 			{
			// 				node.GetParent().RemoveChild(node);
			// 			}

			// 			node.QueueFree();
			// 		}
			// 		else 
			// 		{
			// 			GD.Print("Could not free", node.Name);
			// 		}
			// 	}
			// 	else 
			// 	{
			// 		GD.Print("Not valid");
			// 	}
			// }
			// DisposeQueue = new();
			base._ExitTree();
		}
	}
}