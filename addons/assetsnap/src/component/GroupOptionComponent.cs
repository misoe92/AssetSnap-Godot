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
using AssetSnap.Front.Nodes;
using Godot;

namespace AssetSnap.Component
{
	[Tool]
	public partial class GroupOptionComponent : TraitableComponent
	{
		[Signal]
		public delegate void GroupOptionChangedEventHandler(string name, Variant value);
		
		protected void _MaybeUpdateGrouped(string key, Variant value)
		{
			Node3D Handle = _GlobalExplorer.GetHandle();
			
			if( Handle is AsGrouped3D grouped3D ) 
			{
				grouped3D.Set(key, value);
				// We are currently targeting a grouped item
				for( int i = 0; i<grouped3D.GetChildCount();i++)
				{
					Node3D child = grouped3D.GetChild(i) as Node3D;
					int type = 0;
					int state = 0;
					if( child is AsStaticBody3D asStaticBody3D ) 
					{
						Godot.Collections.Dictionary<string, Variant> options = grouped3D.ChildOptions[i];
						
						if( options.ContainsKey("ConvexCollision") && options["ConvexCollision"].As<bool>() == true) 
						{
							type = 1;

							bool isConvexClean = options.ContainsKey("ConvexClean") && options["ConvexClean"].As<bool>() == true;
							bool isConvexSimplify = options.ContainsKey("ConvexSimplify") && options["ConvexSimplify"].As<bool>() == true;
							
							if(
								isConvexClean &&
								!isConvexSimplify
							) 
							{
								state = 1;
							} 
							
							if(
								!isConvexClean &&
								isConvexSimplify
							) 
							{
								state = 2;
							} 
							
							if(
								isConvexClean &&
								isConvexSimplify
							) 
							{
								state = 3;
							} 
						}
						else if( options.ContainsKey("ConcaveCollision") && options["ConcaveCollision"].As<bool>() == true ) 
						{
							type = 2;
						}
						else if( options.ContainsKey("SphereCollision") && options["SphereCollision"].As<bool>() == true ) 
						{
							type = 3;
						}
						
						// asStaticBody3D.UpdateCollision(type, state);
					}
				}
			}
		}
		
		protected void _HasGroupDataHasChanged()
		{
			EmitSignal(SignalName.GroupOptionChanged, Name, GetValueVariant());
		}
		
		public virtual Variant GetValueVariant()
		{
			return false;
		}
	}
}
#endif