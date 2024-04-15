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

namespace AssetSnap.Settings 
{
	using System.Collections.Generic;
	using AssetSnap.Front.Components;
	using AssetSnap.Front.Configs;
	using AssetSnap.States;
	using Godot;

	public partial class BaseContainer : PanelContainer
	{
		private ScrollContainer _ScrollContainer;
		private MarginContainer _MarginContainer;
		private VBoxContainer _VBoxContainer;
		private HBoxContainer _HBoxContainer;
		private VBoxContainer SubContainerOne;
		private VBoxContainer SubContainerTwo;
		private VBoxContainer SubContainerThree;
		private VBoxContainer SubContainerFour;
		
		public bool Initialized = false;

		/*
		** Initializes the settings container
		**
		** @param SettingsConfig Config
		** @return void
		*/
		public void Initialize()
		{
			if( Initialized ) 
			{
				// Clear the instances first
				_ScrollContainer.GetParent().RemoveChild(_ScrollContainer);
				_ScrollContainer.QueueFree();
			}

			Initialized = true;
			
			SettingsConfig _Config = GlobalExplorer.GetInstance().Settings;
			Name = "Settings";
			
			_ScrollContainer = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			_MarginContainer = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			_MarginContainer.AddThemeConstantOverride("margin_top", 5);
			_MarginContainer.AddThemeConstantOverride("margin_left", 5);
			_MarginContainer.AddThemeConstantOverride("margin_right", 5);
			_MarginContainer.AddThemeConstantOverride("margin_bottom", 5);
			
			_VBoxContainer = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};
			
			_HBoxContainer = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			}; 

			SubContainerOne = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};
			
			SubContainerTwo = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};
			
			SubContainerThree = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};

			SubContainerFour = new()
			{
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
			};
			
			RenderTypes();
			
			_HBoxContainer.AddChild(SubContainerOne);
			_HBoxContainer.AddChild(SubContainerTwo);
			_HBoxContainer.AddChild(SubContainerThree);
			_HBoxContainer.AddChild(SubContainerFour);
			_VBoxContainer.AddChild(_HBoxContainer);
			_MarginContainer.AddChild(_VBoxContainer);
			_ScrollContainer.AddChild(_MarginContainer);
			AddChild(_ScrollContainer);
			StatesUtils.SetLoad("Settings", true);
		}
 
		/*
		** Fetches the setting type by method name
		**
		** @param string method
		** @return string
		*/
		public string GetInputTypeByMethod( string method ) 
		{
			return (string)new Callable(this, method).Call();
		}
		
		/*
		** Checks if the given type is existing
		**
		** @param string str
		** @return bool
		*/
		public bool HasInputTypeMethod( string str ) 
		{
			var Type = GetType();
			return Type.GetMethod(str) != null; 
		}
		
		/*
		** Fetches the settings container
		**
		** @param int Iteration
		** @return VBoxContainer
		*/
		private VBoxContainer GetSettingsContainer( int Iteration)
		{
			VBoxContainer EntryContainer = SubContainerOne;
			if( Iteration == 1 ) 
			{
				EntryContainer = SubContainerTwo; 
			}
			
			if( Iteration == 2 )
			{
				EntryContainer = SubContainerThree; 
			}
			
			if( Iteration == 3 )
			{
				EntryContainer = SubContainerFour; 
			}

			return EntryContainer;
		}
		
		/*
		** Renders the various setting
		**
		** @return void
		*/
		private void RenderTypes()
		{
			var Iteration = 0;
			SettingsConfig _Config = GlobalExplorer.GetInstance().Settings;
			Godot.Collections.Dictionary<string, Variant> _Settings = _Config.GetSettings(); 
			foreach ( (string key, Variant value) in _Settings ) 
			{
				string k = key + "_type"; 
				if( HasInputTypeMethod(k) ) 
				{
					string InputType = GetInputTypeByMethod(k);
					VBoxContainer EntryContainer = GetSettingsContainer(Iteration);

					if( value is Godot.Variant VariantValue ) 
					{
						string StringValue = VariantValue.As<string>();
						float FloatValue = VariantValue.As<float>();
						
						if( StringValue.ToLower() == "false" || StringValue.ToLower() == "true" )
						{
							bool FinalValue = false; 
							
							if( StringValue == "true" )  
							{
								FinalValue = true;
							}
							RenderBoolType(key, FinalValue, InputType, EntryContainer); 
						}
						else 
						{
							RenderIntegerType(key, FloatValue, InputType, EntryContainer); 
						}
					}
					else 
					{
						GD.PushWarning("Found no type matching:", value.GetType());	
					}
				}
	
				Iteration += 1; 
				if( Iteration > 3 )
				{
					Iteration = 0;
				}
			} 
		}
		
		/*
		** Renders a string type setting
		**
		** @param string key
		** @param string value
		** @param string Type
		** @param VBoxContainer _Container
		** @return void
		*/
		public void RenderStringType(string key, string value, string Type, VBoxContainer _Container) 
		{
			switch( Type ) 
			{
				case "LineEdit":

					break;
			}
		}
			
		/*
		** Renders a integer type setting
		**
		** @param string key
		** @param float value
		** @param string Type
		** @param VBoxContainer _Container
		** @return void
		*/	
		public void RenderIntegerType(string key, float value, string Type, VBoxContainer _Container) 
		{
			switch( Type ) 
			{
				case "SpinBox":
					List<string> Components = new()
					{
						"SettingsSpinBox",
					};
					
					if (GlobalExplorer.GetInstance().Components.HasAll( Components.ToArray() )) 
					{
						SettingsSpinBox _SettingsSpinBox = GlobalExplorer.GetInstance().Components.Single<SettingsSpinBox>(true);

						_SettingsSpinBox.key = key;
						_SettingsSpinBox.value = value;
						_SettingsSpinBox.Container = _Container;
						_SettingsSpinBox.Initialize();
					}
					break;
			}
		}
		
		/*
		** Renders a boolable type setting
		**
		** @param string key
		** @param bool value
		** @param string Type
		** @param VBoxContainer _Container
		** @return void
		*/	
		public void RenderBoolType(string key, bool value, string Type, VBoxContainer _Container) 
		{
			switch( Type ) 
			{
				case "CheckBox":
					List<string> Components = new()
					{
						"SettingsCheckbox",
					};
					
					if (GlobalExplorer.GetInstance().Components.HasAll( Components.ToArray() )) 
					{
						SettingsCheckbox _SettingsCheckbox = GlobalExplorer.GetInstance().Components.Single<SettingsCheckbox>(true);

						_SettingsCheckbox.key = key;
						_SettingsCheckbox.value = value;
						_SettingsCheckbox.Container = _Container;
						_SettingsCheckbox.Initialize();
					}
					break;
			}
		}
			
		/*
		** Converts a key to a label
		**
		** @param string key
		** @return string
		*/	
		public string KeyToLabel( string key ) 
		{
			return key.Capitalize().Split('_').Join(" ");
		}
		
		/*
		** Defines the input type of allow multi drop
		**
		** @return string
		*/	
		public string allow_multi_drop_type()
		{
			return "CheckBox";
		}
		
		/*
		** Defines the input type of allow model grab
		**
		** @return string
		*/	
		public string allow_model_grab_type()
		{
			return "CheckBox";
		}
				
		/*
		** Defines the input type of add_collisions
		**
		** @return string
		*/	
		public string add_collisions_type()
		{
			return "CheckBox";
		}
				
		/*
		** Defines the input type of boundary_box_opacity
		**
		** @return string
		*/	
		public string boundary_box_opacity_type()
		{
			return "SpinBox";
		}

		/*
		** Defines the input type of focus_placed_asset
		**
		** @return string
		*/	
		public string focus_placed_asset_type()
		{
			return "CheckBox";
		}
		
		/*
		** Defines the input type of use_as_overlay
		**
		** @return string
		*/	
		public string use_as_overlay_type()
		{
			return "CheckBox";
		}
				
		/*
		** Defines the input type of push_to_scene
		**
		** @return string
		*/	
		public string push_to_scene_type()
		{
			return "CheckBox";
		}
				
		/*
		** Defines the input type of show_snap_boundary_box
		**
		** @return string
		*/	
		public string show_snap_boundary_box_type()
		{
			return "CheckBox";
		}
				
		/*
		** Defines the input type of allow_drag_add
		**
		** @return string
		*/
		public string allow_drag_add_type()
		{
			return "CheckBox";
		}
		
		/*
		** Defines the input type of allow_group_builder
		**
		** @return string
		*/
		public string allow_group_builder_type()
		{
			return "CheckBox";
		}
		
		/*
		** Cleans the various references and fields of the class
		**
		** @return void
		*/
		public override void _ExitTree()
		{
			foreach( Node child in SubContainerOne.GetChildren() ) 
			{
				if( IsInstanceValid(child) )
				{
					// SubContainerOne.RemoveChild(child);
					child.QueueFree();
				}
			}
			
			foreach( Node child in SubContainerTwo.GetChildren() ) 
			{
				if( IsInstanceValid(child) )
				{
					// SubContainerTwo.RemoveChild(child);
					child.QueueFree();
				}
			}
			
			foreach( Node child in SubContainerThree.GetChildren() ) 
			{
				if( IsInstanceValid(child) )
				{
					// SubContainerThree.RemoveChild(child);
					child.QueueFree();
				}
			}
			
			if( IsInstanceValid(SubContainerOne) )
			{
				// _HBoxContainer.RemoveChild(SubContainerOne);
				SubContainerOne.QueueFree();
			}
			
			if( IsInstanceValid(SubContainerTwo) )
			{
				// _HBoxContainer.RemoveChild(SubContainerTwo);
				SubContainerTwo.QueueFree();
			} 
			
			if( IsInstanceValid(SubContainerThree) )
			{
				// _HBoxContainer.RemoveChild(SubContainerThree);
				SubContainerThree.QueueFree();
			}
			
			if( IsInstanceValid(_HBoxContainer) )
			{
				// _ScrollContainer.RemoveChild(_HBoxContainer);
				_HBoxContainer.QueueFree();
			}
			
			if( IsInstanceValid(_ScrollContainer) )
			{
				// RemoveChild(_ScrollContainer);
				_ScrollContainer.QueueFree();
			}

			SubContainerOne = null;
			SubContainerTwo = null;
			SubContainerThree = null;
			_HBoxContainer = null;
			_ScrollContainer = null;
		}
	}
}