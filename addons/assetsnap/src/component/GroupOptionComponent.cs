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
	/// <summary>
	/// Base class for components managing group options.
	/// </summary>
	[Tool]
	public partial class GroupOptionComponent : TraitableComponent
	{
		/// <summary>
		/// Event handler delegate for group option changed event.
		/// </summary>
		[Signal]
		public delegate void GroupOptionChangedEventHandler(string name, Variant value);

		/// <summary>
		/// Gets the value of the group option as a Variant.
		/// </summary>
		/// <returns>The value of the group option as a Variant.</returns>
		public virtual Variant GetValueVariant()
		{
			return false;
		}
		
		/// <summary>
		/// Updates the grouped data with the specified key and value.
		/// </summary>
		/// <param name="key">The key of the data to update.</param>
		/// <param name="value">The value to set.</param>
		protected void _MaybeUpdateGrouped(string key, Variant value)
		{
			Node3D Handle = _GlobalExplorer.GetHandle();

			if (Handle is AsGrouped3D grouped3D)
			{
				grouped3D.Set(key, value);
			}
		}

		/// <summary>
		/// Emits the group option changed signal.
		/// </summary>
		protected void _HasGroupDataHasChanged()
		{
			EmitSignal(SignalName.GroupOptionChanged, Name, GetValueVariant());
		}
	}
}

#endif