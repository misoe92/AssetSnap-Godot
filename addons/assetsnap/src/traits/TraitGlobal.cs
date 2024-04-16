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
	using System;
	using Godot;
	using Godot.Collections;

	[Tool]
	public partial class TraitGlobal : Node
	{
		private string version = "0.0.1";
		
		[Export]
		public static TraitGlobal _Instance = null;

		[Export]
		public Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<int, string>>> Names { get; set; } = new();
		
		[Export]
		public Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<int, Node>>> Instances { get; set; } = new();
		
		[Export]
		public Godot.Collections.Dictionary<string, Godot.Collections.Dictionary<string,Godot.Collections.Dictionary<int, Godot.Collections.Dictionary<string, Variant>>>> InstanceDependencies { get; set; } = new();
		
		public static TraitGlobal Singleton {
			get
			{
				if( _Instance == null ) 
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
		
		public Dictionary<int, Node> AllInstances(string typeString, string owner, bool debug = false)
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
	
		public Node GetInstance( int index, string typeString, string owner, bool debug = false )
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
			
			return Instances[owner][typeKey][index];
		}
		
		public bool AddInstance( int index, Node Instance, string owner, string typeString, Godot.Collections.Dictionary<string, Variant> dependencies ) 
		{
			if( null == typeString || null == owner || null == Instance ) 
			{
				return false;
			}
			
			string typeKey = FormatTypeString(typeString);
			
			Node existing = GetInstance(index, typeString, owner);
			
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
			// GD.Print(_Instance.Count());
			
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

			Node instance = GetInstance(index, typeString, owner, debug);
			string typeKey = FormatTypeString(typeString);
			
			if( null == instance ) 
			{
				if( debug ) 
				{
					GD.PushError("No instance was found for: ", index, "::", typeKey, "::", owner);
				}
				return false;
			}
			
			instance.GetParent().RemoveChild(instance);
			Instances[owner][typeKey].Remove(index);
			InstanceDependencies[owner][typeKey].Remove(index);
			
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
		
			// Godot.Collections.Dictionary<string, Godot.Control> controls = InstanceDependencies[name][typeKey][index];
			// foreach( (string key, Control control) in controls ) 
			// {
			// 	if( null != control.GetParent() ) 
			// 	{
			// 		control.GetParent().RemoveChild(control);
			// 	}
			// }
				
			return true;
		}
		
		
		public Godot.Collections.Dictionary<string, Variant> GetDependencies( int index, string typeString, string name )
		{
			string typeKey = typeString.Split(".").Join("");
			// GD.Print(index, name);
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
				// GD.Print(InstanceDependencies[name][typeKey].Keys);
				throw new Exception("No indexed dependency found: " + index);
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
			// GD.Print(_Instance);
			_Instance = this;
			// GD.Print(_Instance.Count());
			// throw new NotImplementedException();
		}
	}
}