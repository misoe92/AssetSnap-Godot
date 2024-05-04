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

using Godot;

namespace AssetSnap
{
	/// <summary>
	/// Partial class representing a camera explorer, inheriting from BaseExplorer.
	/// </summary>
	[Tool]
	public partial class CameraExplorer : BaseExplorer
	{
		protected Vector3? _ProjectRayOrigin;
		protected Vector3? _ProjectRayNormal;
		protected Vector3? _PositionDraw;
		protected Vector3? _NormalDraw;
		
		/// <summary>
		/// Gets or sets the projected ray origin.
		/// </summary>
		public Vector3? ProjectRayOrigin 
		{
			get => _ProjectRayOrigin;
			set
			{
				_ProjectRayOrigin = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the projected ray normal.
		/// </summary>
		public Vector3? ProjectRayNormal 
		{
			get => _ProjectRayNormal;
			set
			{
				_ProjectRayNormal = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the drawn position.
		/// </summary>
		public Vector3? PositionDraw
		{
			get => _PositionDraw;
			set
			{
				_PositionDraw = value;
			}	
		}
		
		/// <summary>
		/// Gets or sets the drawn normal.
		/// </summary>
		public Vector3? NormalDraw
		{
			get => _NormalDraw;
			set
			{
				_NormalDraw = value;
			}
		}
		
		/// <summary>
		/// Gets the projected ray origin as a Vector3.
		/// </summary>
		/// <returns>The projected ray origin.</returns>
		public Vector3 GetProjectOrigin()
		{
			return (Vector3)ProjectRayOrigin;
		}
		
		/// <summary>
		/// Gets the projected ray normal as a Vector3.
		/// </summary>
		/// <returns>The projected ray normal.</returns>
		public Vector3 GetProjectNormal()
		{
			return (Vector3)ProjectRayNormal;
		}
		
		/// <summary>
		/// Gets the drawn position as a Vector3.
		/// </summary>
		/// <returns>The drawn position.</returns>
		public Vector3 GetPositionDrawn()
		{
			return null != PositionDraw ? (Vector3)PositionDraw : Vector3.Zero;
		}
		
		/// <summary>
		/// Gets the drawn normal as a Vector3.
		/// </summary>
		/// <returns>The drawn normal.</returns>
		public Vector3 GetNomalDrawn()
		{
			return (Vector3)NormalDraw;
		}
		
		/// <summary>
		/// Checks if the projected ray origin is set.
		/// </summary>
		/// <returns>True if the projected ray origin is set, false otherwise.</returns>
		public bool HasProjectOrigin()
		{
			return ProjectRayOrigin != null;
		}
		
		/// <summary>
		/// Checks if the projected ray normal is set.
		/// </summary>
		/// <returns>True if the projected ray normal is set, false otherwise.</returns>
		public bool HasProjectNormal()
		{
			return ProjectRayNormal != null;
		}
		
		/// <summary>
		/// Checks if the drawn position is set.
		/// </summary>
		/// <returns>True if the drawn position is set, false otherwise.</returns>
		public bool HasPositionDrawn()
		{
			return PositionDraw != null;
		}
		
		/// <summary>
        /// Checks if the drawn normal is set.
        /// </summary>
        /// <returns>True if the drawn normal is set, false otherwise.</returns>
		public bool HasNormalDrawn()
		{
			return NormalDraw != null;
		}
	}
}
#endif