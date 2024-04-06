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
		protected Godot.Collections.Array boundTraits = new();

		protected bool Initiated = false;

		public override void Initialize()
		{
			base.Initialize();
		}

		public void AddTrait(Type traitType)
		{
			object instance = Activator.CreateInstance(traitType);
			if( instance is Trait.Base trait ) 
			{
				AddChild(trait);
				// Check if the trait is already bound
				if (!boundTraits.Contains(trait))
				{
					boundTraits.Add(trait);
				}
				else 
				{ 
					trait.Free();
				}
			}
			else 
			{
				GD.PushError("Cannot use a type that is not derived from AssetSnap.Trait.Base");
			}
		}
		
		public bool HasTrait<T>()
		{
			if( boundTraits.Count == 0 ) 
			{
				return false;
			}

			Variant traitInstance = boundTraits.FirstOrDefault(
				(t) =>
				{
					if (
						IsInstanceValid(t.AsGodotObject()) &&
						t.AsGodotObject().GetType() == typeof(T)
					)
					{
						return true;
					}

					return false;
				}
			);
			
			if( traitInstance.VariantType == Variant.Type.Object ) 
			{
				return true;
			}
			
			return false;
		}
		
		public bool ClearTrait<T>() 
		{
			if( boundTraits.Count == 0 ) 
			{
				GD.PushWarning("No traits was found");
				return false;
			}

			Variant traitInstance = boundTraits.FirstOrDefault(
				(t) =>
				{
					if( false == IsInstanceValid(t.AsGodotObject()) ) 
					{
						return false;
					}
					
					if (t.AsGodotObject().GetType() == typeof(T))
					{
						return true;
					}

					return false;
				}
			);
			
			switch (traitInstance.VariantType)
			{
				case Variant.Type.Object:
					boundTraits.Remove(traitInstance);
					if( traitInstance.AsGodotObject().GetType() == typeof(T) ) 
					{
						Trait.Base trait = traitInstance.AsGodotObject() as Trait.Base;
						if( null != trait.GetParent() ) 
						{
							trait.GetParent().RemoveChild(trait);
						}

						trait.disposed = true;
						trait.QueueFree();
					}
					return true;
				
				default :
					GD.PushWarning("Invalid type", traitInstance.VariantType);
					break;

			}
			return false;
		}
		
		public T Trait<T>() where T : class
		{
		
			if( boundTraits.Count == 0 ) 
			{
				return null;
			}
			
			Variant traitInstance = boundTraits.FirstOrDefault(
				(t) =>
				{
					if (IsInstanceValid(t.AsGodotObject()) && t.AsGodotObject().GetType() == typeof(T))
					{
						return true;
					}

					return false;
				}
			);

			switch (traitInstance.VariantType)
			{
				case Variant.Type.Nil:
					return null;
				case Variant.Type.Object:
					if(
						IsInstanceValid(traitInstance.AsGodotObject()) &&
						traitInstance.AsGodotObject().GetType() == typeof(T)
					) 
					{
						return traitInstance.AsGodotObject() as T;
					}
					
					return null;
			}
			return null;
		}
		
		private void ClearTraits()
		{
			for( int i = 0; i < boundTraits.Count; i++) 
			{
				Variant traitInstance = boundTraits[i].AsGodotObject();
				
				boundTraits.Remove(traitInstance);
				if( traitInstance.AsGodotObject() is Trait.Base ) 
				{
					Trait.Base trait = traitInstance.AsGodotObject() as Trait.Base;
					if( null != trait.GetParent() ) 
					{
						trait.GetParent().RemoveChild(trait);
					}
					
					trait.Free();
				}
			}
		}

		public override void _ExitTree()
		{
			// if( null != boundTraits && boundTraits.Count != 0 ) 
			// {
			// 	for( int i = 0; i < boundTraits.Count; i++ ) 
			// 	{
			// 		if( null != boundTraits[i] && IsInstanceValid(boundTraits[i]) ) 
			// 		{
			// 			GodotObject inst = boundTraits[i];
			// 			inst.Free();

			// 			if( null != inst && IsInstanceValid(inst) ) 
			// 			{
			// 				if( inst is Trait.Base trait ) 
			// 				{
			// 					GD.Print("Failed trait unload index: " + i + ", " + Name + ", " + trait.Name );
			// 				}
			// 				else
			// 				{
			// 					GD.Print("Failed trait unload index: " + i + ", " + Name + ", " + inst.GetType() );
			// 				} 
			// 			}
			// 			else 
			// 			{
			// 				GD.Print("Unloaded: " + i + ", " + Name + ", " + inst.GetType() );
			// 			}
			// 		}
			// 	}

			// 	// boundTraits = null;
			// }

			// boundTraits = new();
			base._ExitTree();
		}
	}
}
#endif