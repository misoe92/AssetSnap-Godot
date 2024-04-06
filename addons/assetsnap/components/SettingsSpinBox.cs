
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

namespace AssetSnap.Front.Components
{
	using AssetSnap.Component;
	using AssetSnap.Static;
	using Godot;

	[Tool]
	public partial class SettingsSpinBox : BaseComponent
	{
		private float _value = 0.0f;
		private string _key;

		private MarginContainer OuterContainer; 
		private MarginContainer CheckBoxOuterContainer;
		private VBoxContainer InnerContainer;
		private Label TitleLabel;
		private Label DescriptionLabel;
		private SpinBox _Element; 
	
		private Callable? ValueChanged;

		public string key 
		{
			get => _key;
			set 
			{
				_key = value;
			}
		}
		
		public float value 
		{
			get => _value;
			set 
			{
				_value = value;
			}
		}
		
		/*
		** Constructor of the component
		** 
		** @return void
		*/			 
		public SettingsSpinBox()
		{
			Name = "SettingsSpinBox";
			//_include = false;   
		}
		
		/*
		** Initializing the component
		**   
		** @return void 
		*/
		public override void Initialize() 
		{
			string title = GetTitle( key );
			string description = GetDescription( key );
			OuterContainer = new()
			{
				Size = new Vector2I(0, 100)
			};
			InnerContainer = new();
			ValueChanged = new Callable(this, "UpdateKey");

			_Element = new SpinBox() 
			{
				Name = key,
				Step = 0.0f,
				Value = value,
			};

			ConfigureOuterContainer(); 
			ConfigureTitle( title, description );

			if (IfHadTitleOrDescription()) 
			{
				FinalizeOuterContainer();
				AddToContainer(OuterContainer);
			}  
			else 
			{
				AddToContainer(_Element);
			}
			
			if( ValueChanged is Callable _callable ) 
			{
				_Element.Connect(SpinBox.SignalName.ValueChanged, _callable);
			}
		}
		
		/*
		** Adds component to a container
		** 
		** @return void
		*/
		private void AddToContainer( Node what) 
		{
			Container.AddChild(what);
		}
		
		/*
		** Checks if component has a title or description
		** 
		** @return bool
		*/
		private bool IfHadTitleOrDescription()
		{
			return TitleLabel != null || DescriptionLabel != null;
		}
		
		/*
		** Configures the outer container
		** 
		** @return void
		*/
		private void ConfigureOuterContainer()
		{
			OuterContainer.AddThemeConstantOverride("margin_top", 20);
			OuterContainer.AddThemeConstantOverride("margin_bottom", 20);
			OuterContainer.AddThemeConstantOverride("margin_left", 30); 
			OuterContainer.AddThemeConstantOverride("margin_right", 30);
		}
		
		/*
		** Finalizes the outer container
		** 
		** @return void
		*/
		private void FinalizeOuterContainer()
		{
			CheckBoxOuterContainer = new(); 
			CheckBoxOuterContainer.AddThemeConstantOverride("margin_top", 10);
			
			CheckBoxOuterContainer.AddChild(_Element);
			InnerContainer.AddChild(CheckBoxOuterContainer);
			OuterContainer.AddChild(InnerContainer);
		}
		
		/*
		** Configures the title
		** 
		** @return void
		*/
		private void ConfigureTitle( string title, string description)
		{
			if (title != null || description != null)
			{
				if (title != null)
				{
					TitleLabel = new()
					{
						ThemeTypeVariation = "HeaderSmall",
						Text = title,
					};

					InnerContainer.AddChild(TitleLabel); 
				}
				
				if (description != null)
				{
					MarginContainer DescriptionMarginContainer = new()
					{
						SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
						SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
						CustomMinimumSize = new Vector2I(0, 70),
					};
					
					DescriptionLabel = new()
					{
						Name = "SettingDescription",
						Text = description,
						AutowrapMode = TextServer.AutowrapMode.Word,
						SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
					};

					DescriptionMarginContainer.AddChild(DescriptionLabel);
					InnerContainer.AddChild(DescriptionMarginContainer);
				}
			}
		}
		
		/*
		** Fetches the title
		** 
		** @return string
		*/
		public string GetTitle( string key )
		{
			string FinalKey = key + "_title";
			return SettingsText.KeyToString(FinalKey);
		}
		
		/*
		** Fetches the description
		** 
		** @return string
		*/
		public string GetDescription( string key )
		{
			string FinalKey = key + "_description";
			return SettingsText.KeyToString(FinalKey);
		}
		
		/*
		** Updates the settings key and
		** interval value
		** 
		** @return void
		*/
		public void UpdateKey( float NewValue )
		{
			value = NewValue;
			_GlobalExplorer.Settings.SetKey(key, NewValue);
		}
	
		/*
		** Cleans up in references, fields and parameters.
		** 
		** @return void
		*/
		public override void _ExitTree()
		{
			if( IsInstanceValid(_Element) && _Element != null && ValueChanged is Callable _callable ) 
			{
				if( _Element.IsConnected(SpinBox.SignalName.ValueChanged, _callable) ) 
				{
					_Element.Disconnect(SpinBox.SignalName.ValueChanged, _callable);
				}
			}
			
			if( IsInstanceValid(DescriptionLabel) ) 
			{
				DescriptionLabel.QueueFree();
				DescriptionLabel = null;
			}
			
			if( IsInstanceValid(TitleLabel) ) 
			{
				TitleLabel.QueueFree();
				TitleLabel = null;
			}
			 
			if( IsInstanceValid(_Element) )
			{
				_Element.QueueFree();
				_Element = null;
			}
			
			if( IsInstanceValid(CheckBoxOuterContainer) ) 
			{
				CheckBoxOuterContainer.QueueFree();
				CheckBoxOuterContainer = null;
			}
			
			if( IsInstanceValid(InnerContainer) ) 
			{
				InnerContainer.QueueFree();
				InnerContainer = null;
			}
			
			if( IsInstanceValid(OuterContainer) ) 
			{
				OuterContainer.QueueFree();
				OuterContainer = null;
			}
		}
	}
}