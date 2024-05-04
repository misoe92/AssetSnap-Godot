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
using AssetSnap.States;
using Godot;

namespace AssetSnap.Front.Components.Library.Sidebar
{
	/// <summary>
	/// Partial class for managing the editing component in the library sidebar.
	/// </summary>
	public partial class Editing : LibraryComponent
	{
		private readonly string _Title = "Currently Editing";

		private MarginContainer _MarginContainer;
		private PanelContainer _PanelContainer;
		private MarginContainer _InnerMarginContainer;
		private	VBoxContainer _InnerContainer;
		private	Label _Label;
		private	Label _LabelEditing;
			 
		/// <summary>
		/// Constructor for the Editing component.
		/// </summary>	 
		public Editing()
		{
			Name = "LSEditing";
			//_include = false;
		}
		
		/// <summary>
		/// Initializes the component.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			
			_Initiated = true;
			
			_MarginContainer = new();
			_InnerMarginContainer = new();
			_PanelContainer = new();
			_InnerContainer = new();

			_InnerContainer.AddThemeConstantOverride("separation", 0);
			
			_Label = new()
			{
				ThemeTypeVariation = "HeaderXtraSmall",
				Text = _Title
			};
							
			_LabelEditing = new()
			{
				ThemeTypeVariation = "HeaderSmall", 
				Text = _GlobalExplorer.HasModel ? _GetModelName() : "None",
			};
			
			_MarginContainer.AddThemeConstantOverride("margin_left", 0); 
			_MarginContainer.AddThemeConstantOverride("margin_right", 0);
			_MarginContainer.AddThemeConstantOverride("margin_top", 0);
			_MarginContainer.AddThemeConstantOverride("margin_bottom", 0);
			
			_InnerMarginContainer.AddThemeConstantOverride("margin_left", 12); 
			_InnerMarginContainer.AddThemeConstantOverride("margin_right", 12);
			_InnerMarginContainer.AddThemeConstantOverride("margin_top", 5);
			_InnerMarginContainer.AddThemeConstantOverride("margin_bottom", 5);
			
			_InnerContainer.AddChild(_Label); 
			_InnerContainer.AddChild(_LabelEditing); 
			_InnerMarginContainer.AddChild(_InnerContainer); 
			_PanelContainer.AddChild(_InnerMarginContainer);
			_MarginContainer.AddChild(_PanelContainer);

			AddChild(_MarginContainer);
		}
		
		/// <summary>
		/// Sets the text of the editing label.
		/// </summary>
		/// <param name="text">The text to set.</param>
		public void SetText( string text ) 
		{
			_LabelEditing.Text = _PrepareModelName(text);
			StatesUtils.Get().EditingTitle = _PrepareModelName(text);
		}

		/// <summary>
		/// Synchronizes the editing title with the global explorer.
		/// </summary>
		public override void Sync() 
		{
			StatesUtils.Get().EditingTitle = _PrepareModelName(_LabelEditing.Text);
		}
		
		/// <summary>
		/// Retrieves the name of the model being edited.
		/// </summary>
		/// <returns>The name of the model being edited.</returns>
		private string _GetModelName()
		{
			string _Name = _GlobalExplorer.GetHandle().Name;
			return _PrepareModelName( _Name );
		}
		
		/// <summary>
		/// Prepares the model name for display.
		/// </summary>
		/// <param name="name">The original model name.</param>
		/// <returns>The prepared model name.</returns>
		private string _PrepareModelName( string name ) 
		{
			string _Name = name;
			if( _Name.Length > 20 ) 
			{
				_Name = _Name.Substr(0, 19);
				_Name = _Name + "...";
			}

			return _Name;
		}
	}
}

#endif