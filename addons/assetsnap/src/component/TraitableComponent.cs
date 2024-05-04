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

using System;
using System.Linq;
using Godot;

namespace AssetSnap.Component
{
	/// <summary>
	/// Base class for components that can have traits attached to them.
	/// </summary>
	[Tool]
	public partial class TraitableComponent : BaseComponent
	{
		[Export]
		public Godot.Collections.Array<Trait.Base> BoundTraits;
		
		protected bool _Initiated = false;

		/// <summary>
		/// Default constructor for TraitableComponent.
		/// </summary>
		public TraitableComponent()
		{
			BoundTraits = new();
		}

		/// <summary>
		/// Initializes the component and adds attached traits.
		/// </summary>
		public override void Initialize()
		{
			if( false == _Initiated ) 
			{
				foreach( string traitString in _UsingTraits ) 
				{
					AddTrait(Type.GetType(traitString), this);
				}
			}
			
			base.Initialize();
		}

		/// <summary>
		/// Clears the component, including its attached traits.
		/// </summary>
		/// <param name="debug">Optional parameter to enable debugging output.</param>
		public override void Clear(bool debug = false)
		{
			ClearTrait( debug );
			base.Clear(debug);
		}
		
		/// <summary>
		/// Adds a trait to the component.
		/// </summary>
		/// <param name="traitType">Type of the trait to add.</param>
		/// <param name="container">Container node for the trait.</param>
		public void AddTrait(Type traitType, Node container)
		{
			object instance = Activator.CreateInstance(traitType);
			if( instance is Trait.Base trait ) 
			{
				trait.OwnerName = Name;
				container.AddChild(trait);
				// Check if the trait is already bound
				if (!BoundTraits.Contains(trait))
				{
					BoundTraits.Add(trait);
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
		
		/// <summary>
		/// Checks if the component has a specified type of trait.
		/// </summary>
		/// <typeparam name="T">Type of the trait to check.</typeparam>
		/// <param name="debug">Optional parameter to enable debugging output.</param>
		/// <returns>True if the component has the specified trait type; otherwise, false.</returns>
		public bool HasTrait<T>( bool debug = false)
		{
			if( BoundTraits.Count == 0 ) 
			{
				if (debug)
				{
					GD.PushError("No traits found");
				}
				
				return false;
			}

			Trait.Base traitInstance = BoundTraits.FirstOrDefault(
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
		
		/// <summary>
		/// Clears the trait with the specified type from the component.
		/// </summary>
		/// <typeparam name="T">Type of the trait to clear.</typeparam>
		/// <param name="debug">Optional parameter to enable debugging output.</param>
		/// <returns>True if the trait was successfully cleared; otherwise, false.</returns>
		public bool ClearTrait<T>(bool debug = false) 
		{
			if( BoundTraits.Count == 0 ) 
			{
				GD.PushWarning("No traits was found");
				return false;
			}
	
			foreach( string traitString in _UsingTraits ) 
			{
				if( debug ) 
				{
					GD.Print("Clearing trait: ", traitString, "::", Name, "->Count(", BoundTraits.Count, ")");
				}
			
				Trait(Type.GetType(traitString)).Clear(-1, debug);
				Trait(Type.GetType(traitString)).Iteration = 0;
				BoundTraits.Remove(Trait(Type.GetType(traitString)));
				
				if( debug ) 
				{
					GD.Print("Cleared: ", traitString, "->Count(", BoundTraits.Count, ")" );
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// Clears all traits from the component.
		/// </summary>
		/// <param name="debug">Optional parameter to enable debugging output.</param>
		/// <returns>True if all traits were successfully cleared; otherwise, false.</returns>
		public bool ClearTrait(bool debug = false) 
		{
			if( BoundTraits.Count == 0 ) 
			{
				GD.PushWarning("No traits was found");
				return false;
			}
			
			foreach( string traitString in _UsingTraits ) 
			{
				if( debug ) 
				{
					GD.Print("Clearing trait: ", traitString, "::", Name, "->Count(", BoundTraits.Count, ")");
				}
				
				Trait(Type.GetType(traitString)).Clear(-1, debug);
				Trait(Type.GetType(traitString)).Iteration = 0;
				BoundTraits.Remove(Trait(Type.GetType(traitString)));
				
				if( debug ) 
				{
					GD.Print("Cleared: ", traitString, "->Count(", BoundTraits.Count, ")" );
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// Retrieves a trait of the specified type from the component.
		/// </summary>
		/// <typeparam name="T">Type of the trait to retrieve.</typeparam>
		/// <returns>The trait instance if found; otherwise, null.</returns>
		public T Trait<T>() where T : class
		{
			if( BoundTraits.Count == 0 ) 
			{
				return null;
			}
			
			Trait.Base traitInstance = BoundTraits.FirstOrDefault(
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
		
		/// <summary>
		/// Retrieves a trait of the specified type from the component.
		/// </summary>
		/// <param name="type">Type of the trait to retrieve.</param>
		/// <returns>The trait instance if found; otherwise, null.</returns>
		public Trait.Base Trait( Type type )
		{
			if( BoundTraits.Count == 0 ) 
			{
				return null;
			}
			
			Trait.Base traitInstance = BoundTraits.FirstOrDefault(
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

			if( null != traitInstance && traitInstance is Trait.Base traitbase) 
			{
				string TypeString = traitbase.GetType().ToString();
				
				return traitbase;
			}
					
			return null;
		}
	}
}

#endif