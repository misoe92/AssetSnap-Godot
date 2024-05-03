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

using AssetSnap.Component;
using Godot;

namespace AssetSnap.Front.Components
{
	/// <summary>
    /// Component responsible for displaying an introduction to the application.
    /// </summary>
	[Tool]
	public partial class Introduction : TraitableComponent
	{
		private readonly string TitleText = "AssetSnap";
		private readonly string DescriptionText = "Add folders to start, when an folder has been added a tab with the folder name will appear, Then go to the folder tab and browse your assets and place them. \n\nIf you wish to remove a library all you will have to do is click the red button on the right. In the same column as the library you wish to remove.";
	
		/// <summary>
        /// Constructor of the Introduction class.
        /// </summary> 
		public Introduction() : base()
		{
			Name = "Introduction";
			
			UsingTraits = new()
			{
				{ typeof(Labelable).ToString() },
			};

			//_include = false; 
		}
		
		/// <summary>
        /// Initializes the component.
        /// </summary>
		public override void Initialize()
		{
			base.Initialize();
			
			Initiated = true;

			Trait<Labelable>()
				.SetMargin(0, "bottom")
				.SetName("IntroductionTitle")
				.SetType(Labelable.TitleType.HeaderLarge)
				.SetText(TitleText)
				.SetSuffix("V." + _GlobalExplorer._Plugin.GetVersionString())
				.Instantiate();

			Trait<Labelable>()
				.SetMargin(0, "top")
				.SetMargin(0, "bottom")
				.SetName("IntroductionDescription")
				.SetType(Labelable.TitleType.TextMedium)
				.SetText(DescriptionText)
				.SetAutoWrap(TextServer.AutowrapMode.Word)
				.Instantiate();
				
			Trait<Labelable>()
				.Select(0) 
				.AddToContainer( this );  
				
			Trait<Labelable>()
				.Select(1) 
				.AddToContainer( this );  
		}
	}
}

#endif