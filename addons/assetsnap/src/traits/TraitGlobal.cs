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
using Godot;
using Godot.Collections;

namespace AssetSnap.Trait
{
	/// <summary>
	/// This class manages global traits and their instances.
	/// </summary>
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

		/// <summary>
		/// Retrieves the name associated with the given index, type, and owner.
		/// </summary>
		/// <param name="index">The index of the name.</param>
		/// <param name="typeString">The type string.</param>
		/// <param name="owner">The owner of the name.</param>
		/// <param name="debug">Optional debug flag. If true, error messages will be printed.</param>
		/// <returns>The retrieved name.</returns>
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
		
		/// <summary>
		/// Adds a name to the dictionary.
		/// </summary>
		/// <param name="index">The index of the name.</param>
		/// <param name="name">The name to add.</param>
		/// <param name="owner">The owner of the name.</param>
		/// <param name="typeString">The type string.</param>
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
		
		/// <summary>
		/// Removes a name from the dictionary.
		/// </summary>
		/// <param name="index">The index of the name to remove.</param>
		/// <param name="owner">The owner of the name.</param>
		/// <param name="typeString">The type string.</param>
		/// <param name="debug">Optional debug flag. If true, error messages will be printed.</param>
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
		
		/// <summary>
		/// Formats the given type string.
		/// </summary>
		/// <param name="typeString">The type string to format.</param>
		/// <returns>The formatted type string.</returns>
		public string FormatTypeString(string TypeString ) 
		{
			return TypeString.Split(".").Join("");
		}
		
		/// <summary>
		/// Retrieves all instances of the given type and owner.
		/// </summary>
		/// <param name="typeString">The type string.</param>
		/// <param name="owner">The owner of the instances.</param>
		/// <param name="debug">Optional debug flag. If true, error messages will be printed.</param>
		/// <returns>The dictionary containing all instances.</returns>
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
		
		/// <summary>
		/// Checks if an instance exists at the specified index, type, and owner.
		/// </summary>
		/// <param name="index">The index of the instance.</param>
		/// <param name="typeString">The type string.</param>
		/// <param name="owner">The owner of the instance.</param>
		/// <param name="debug">Optional debug flag. If true, error messages will be printed.</param>
		/// <returns>True if the instance exists, false otherwise.</returns>
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

		/// <summary>
		/// Retrieves an instance at the specified index, type, and owner.
		/// </summary>
		/// <param name="index">The index of the instance.</param>
		/// <param name="typeString">The type string.</param>
		/// <param name="owner">The owner of the instance.</param>
		/// <param name="debug">Optional debug flag. If true, error messages will be printed.</param>
		/// <returns>The retrieved instance if found; otherwise, null.</returns>
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
		
		/// <summary>
		/// Adds an instance to the collection.
		/// </summary>
		/// <param name="index">The index of the instance.</param>
		/// <param name="Instance">The instance to add.</param>
		/// <param name="owner">The owner of the instance.</param>
		/// <param name="typeString">The type string.</param>
		/// <param name="dependencies">Dictionary containing dependencies for the instance.</param>
		/// <returns>True if the instance was successfully added; otherwise, false.</returns>
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
		
		/// <summary>
		/// Removes an instance at the specified index, type, and owner.
		/// </summary>
		/// <param name="index">The index of the instance.</param>
		/// <param name="typeString">The type string.</param>
		/// <param name="owner">The owner of the instance.</param>
		/// <param name="debug">Optional debug flag. If true, error messages will be printed.</param>
		/// <returns>True if the instance was successfully removed; otherwise, false.</returns>
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
		
		/// <summary>
		/// Retrieves the dependencies for the instance at the specified index, type, and owner.
		/// </summary>
		/// <param name="index">The index of the instance.</param>
		/// <param name="typeString">The type string.</param>
		/// <param name="name">The name of the instance.</param>
		/// <returns>The dictionary containing dependencies for the instance.</returns>
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
		
		/// <summary>
		/// Counts the total number of instances.
		/// </summary>
		/// <returns>The total number of instances.</returns>
		public int Count()
		{
			return Instances.Count;
		}
		
		/// <summary>
		/// Counts the total number of instances owned by the specified owner.
		/// </summary>
		/// <param name="owner">The owner whose instances to count.</param>
		/// <param name="debug">Optional debug flag. If true, error messages will be printed.</param>
		/// <returns>The total number of instances owned by the specified owner.</returns>
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

		/// <summary>
		/// Called before serialization.
		/// </summary>
		public void OnBeforeSerialize()
		{
			// 
		}

		/// <summary>
		/// Called after deserialization.
		/// </summary>
		public void OnAfterDeserialize()
		{
			_Instance = this;
		}
	}
}