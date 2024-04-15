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
	using System;
	using System.Linq;
	using Godot;

	[Tool]
	public partial class TraitableComponent : BaseComponent
	{
		[Export]
		public Godot.Collections.Array<Trait.Base> boundTraits;

		protected bool Initiated = false;

		[Export]
		public Godot.Collections.Dictionary<string, int> Iterations = new();
		
		public TraitableComponent()
		{
			boundTraits = new();
		}

		public override void Initialize()
		{
			foreach( string traitString in UsingTraits ) 
			{
				AddTrait(Type.GetType(traitString), Container);
			}
			
			base.Initialize();
		}

		public override void Clear()
		{
			foreach( string traitString in UsingTraits ) 
			{
				// GD.Print(traitString);
				ClearTrait(Type.GetType(traitString), Name);
			}

			base.Clear();
		}
		
		public void AddTrait(Type traitType, Node container)
		{
			object instance = Activator.CreateInstance(traitType);
			if( instance is Trait.Base trait ) 
			{
				trait.OwnerName = Name;

				container.AddChild(trait);
				// Check if the trait is already bound
				if (!boundTraits.Contains(trait))
				{
					boundTraits.Add(trait);
				}
				else
				{
					container.RemoveChild(trait);
					trait.QueueFree();	
				}
			}
			else 
			{
				GD.PushError("Cannot use a type that is not derived from AssetSnap.Trait.Base");
			}
		}
		
		public bool HasTrait<T>( bool debug = false)
		{
			if( boundTraits.Count == 0 ) 
			{
				if (debug)
				{
					GD.PushError("No traits found");
				}
				
				return false;
			}

			Trait.Base traitInstance = boundTraits.FirstOrDefault(
				(t) =>
				{
					if (
						t.GetType() == typeof(T)
					)
					{
						return true;
					}

					return false;
				}
			);
			
			if(
				null != traitInstance &&
				IsInstanceValid(traitInstance) &&
				traitInstance.Select(0).IsValid() &&
				traitInstance.Build == false
			) 
			{
				return true;
			}
			
			if( debug )
			{
				if( null == traitInstance ) 
				{
					GD.PushError("No trait instance was set");
				}
				
				if( null != traitInstance && false == IsInstanceValid(traitInstance) ) 
				{
					GD.PushError("Trait instance was invalid");
				}
				
				if( null != traitInstance && false == traitInstance.Select(0).IsValid() ) 
				{
					GD.PushError("Trait is not valid: ", traitInstance.Select(0).IsValid());
				}
				
				if( null != traitInstance && true == traitInstance.Build ) 
				{
					GD.PushError("Trait instance was building");
				}
			}
			
			return false;
		}
		
		public bool ClearTrait<T>( string TraitType, string Owner) 
		{
			if( boundTraits.Count == 0 ) 
			{
				GD.PushWarning("No traits was found");
				return false;
			}
	
			foreach( string traitString in UsingTraits ) 
			{
				Trait(Type.GetType(traitString))._Select(0);
				
				if( Trait(Type.GetType(traitString)).IsValid() ) 
				{
					Trait(Type.GetType(traitString)).Clear();
					Trait(Type.GetType(traitString)).Iteration = 0;
					// GD.Print("Cleared: ", traitString);
				}
			}
			
			return false;
		}
		
		public bool ClearTrait(Type traitType, string Owner) 
		{
			if( boundTraits.Count == 0 ) 
			{
				GD.PushWarning("No traits was found");
				throw new Exception("");
				return false;
			}
			
			foreach( string traitString in UsingTraits ) 
			{
				// Trait(Type.GetType(traitString)).Iteration = 0;
				Trait(Type.GetType(traitString)).Clear();
				// GD.Print("Cleared: ", traitString);
			}

			// Godot.Collections.Array<AssetSnap.Trait.Base> instances = boundTraits;
			// if( null != instances && instances.Count != 0 ) 
			// {
			// 	foreach(Trait.Base TraitInstance in instances) 
			// 	{
			// 		TraitInstance.Clear(0);
			// 		TraitInstance.Index = 0;
			// 		GD.Print(TraitInstance.Name, " cleared");
			// 	}

			// 	// GD.Print(instances, " cleared");
			// }

			return false;
		}
		
		public T Trait<T>() where T : class
		{
			if( boundTraits.Count == 0 ) 
			{
				return null;
			}
			
			Trait.Base traitInstance = boundTraits.FirstOrDefault(
				(t) =>
				{
					if (
						t.GetType() == typeof(T)
					)
					{
						return true;
					}

					return false;
				}
			);

			if( null != traitInstance && traitInstance is Trait.Base traitbase) 
			{
				string TypeString = traitbase.GetType().ToString();
				
				return traitbase as T;
			}
					
			return null;
		}
		
		public Trait.Base Trait( Type type )
		{
			if( boundTraits.Count == 0 ) 
			{
				return null;
			}
			
			Trait.Base traitInstance = boundTraits.FirstOrDefault(
				(t) =>
				{
					if (
						t.GetType() == type
					)
					{
						return true;
					}

					return false;
				}
			);

			// GD.Print(traitInstance);

			if( null != traitInstance && traitInstance is Trait.Base traitbase) 
			{
				string TypeString = traitbase.GetType().ToString();
				
				return traitbase;
			}
					
			return null;
		}
		
		private void ClearTraits()
		{
			for( int i = 0; i < boundTraits.Count; i++) 
			{
				Trait.Base traitInstance = boundTraits[i];
				boundTraits.Remove(traitInstance);
			}
		}
	}
}
#endif