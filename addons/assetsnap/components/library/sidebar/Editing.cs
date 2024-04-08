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

namespace AssetSnap.Front.Components.Library.Sidebar
{
	using AssetSnap.Component;
	using Godot;

	public partial class Editing : LibraryComponent
	{
		private readonly string _Title = "Currently Editing";

		private MarginContainer _MarginContainer;
		private PanelContainer _PanelContainer;
		private MarginContainer _InnerMarginContainer;
		private	VBoxContainer _InnerContainer;
		private	Label _Label;
		private	Label _LabelEditing;
			 
		/*
		** Component constructor
		** 
		** @return void
		*/	 
		public Editing()
		{
			Name = "LSEditing";
			//_include = false;
		}
		
		/*
		** Initializes the component
		** 
		** @return void
		*/
		public override void Initialize()
		{
			Initiated = true;
			
			
			if( Container is VBoxContainer BoxContainer ) 
			{
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
					Text = _GlobalExplorer.HasModel ? GetModelName() : "None",
				};
				
				_MarginContainer.AddThemeConstantOverride("margin_left", 0); 
				_MarginContainer.AddThemeConstantOverride("margin_right", 0);
				_MarginContainer.AddThemeConstantOverride("margin_top", 0);
				_MarginContainer.AddThemeConstantOverride("margin_bottom", 0);
				
				_InnerMarginContainer.AddThemeConstantOverride("margin_left", 10); 
				_InnerMarginContainer.AddThemeConstantOverride("margin_right", 10);
				_InnerMarginContainer.AddThemeConstantOverride("margin_top", 5);
				_InnerMarginContainer.AddThemeConstantOverride("margin_bottom", 5);
				
				_InnerContainer.AddChild(_Label); 
				_InnerContainer.AddChild(_LabelEditing); 
				_InnerMarginContainer.AddChild(_InnerContainer); 
				_PanelContainer.AddChild(_InnerMarginContainer);
				_MarginContainer.AddChild(_PanelContainer);

				BoxContainer.AddChild(_MarginContainer);
			}
		}
		
		private string GetModelName()
		{
			string _Name = _GlobalExplorer.GetHandle().Name;
			return PrepareModelName( _Name );
		}
		
		private string PrepareModelName( string name ) 
		{
			string _Name = name;
			if( _Name.Length > 20 ) 
			{
				_Name = _Name.Substr(0, 19);
				_Name = _Name + "...";
			}

			return _Name;
		}
		
		public void SetText( string text ) 
		{
			_LabelEditing.Text = PrepareModelName(text);
			_GlobalExplorer.States.EditingTitle = PrepareModelName(text);
		}

		public override void Sync() 
		{
			_GlobalExplorer.States.EditingTitle = PrepareModelName(_LabelEditing.Text);
		}

		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			if( IsInstanceValid(_LabelEditing) ) 
			{
				_LabelEditing.QueueFree();
				_LabelEditing = null;
			}
			
			if( IsInstanceValid(_Label) ) 
			{
				_Label.QueueFree();
				_Label = null;
			}
			
			if( IsInstanceValid(_InnerContainer) ) 
			{
				_InnerContainer.QueueFree();
				_InnerContainer = null;
			}
			
			if( IsInstanceValid(_InnerMarginContainer) ) 
			{
				_InnerMarginContainer.QueueFree();
				_InnerMarginContainer = null;
			}
			
			if( IsInstanceValid(_PanelContainer) ) 
			{
				_PanelContainer.QueueFree();
				_PanelContainer = null;
			}
			
			if( IsInstanceValid(_MarginContainer) ) 
			{
				_MarginContainer.QueueFree();
				_MarginContainer = null;
			}
		}
	}
}