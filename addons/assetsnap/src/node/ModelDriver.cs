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

namespace AssetSnap.Nodes
{
	using AssetSnap.Front.Nodes;
	using Godot;
	
	/// <summary>
	/// Represents a class responsible for determining the type of a model.
	/// </summary>
	[Tool]
	public class ModelDriver
	{
		/// <summary>
		/// Defines the possible types of models.
		/// </summary>
		public enum ModelTypes 
		{
			Simple,
			SceneBased,
		};
		
		/// <summary>
        /// Gets or sets the type of the model.
        /// </summary>
		public ModelTypes ModelType { get; set;}
		
		/// <summary>
        /// Determines the type of model based on the node structure.
        /// </summary>
        /// <param name="node">The root node of the model.</param>
        /// <remarks>
        /// This method goes through the node structure to decide what type of model we are working with.
        /// </remarks>
		public void RegisterType( Node3D node ) 
		{
			if( node is AsMeshInstance3D ) 
			{
				ModelType = ModelTypes.Simple;
			}
			
			if( node is AsNode3D ) 
			{
				ModelType = ModelTypes.SceneBased;
			}
		}
		
		/// <summary>
        /// Gets the defined model type.
        /// </summary>
        /// <returns>The type of the model.</returns>
		public ModelTypes GetModelType()
		{
			return ModelType;
		}
	}
}