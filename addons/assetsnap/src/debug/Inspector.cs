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

using System;
using System.Collections.Generic;
using System.Reflection;
using AssetSnap.Explorer;
using Godot;

namespace AssetSnap.Debug
{
	/// <summary>
	/// Partial class for managing the inspector in AssetSnap.
	/// </summary>
	public partial class Inspector : Node, ISerializationListener
	{
		/// <summary>
		/// Singleton instance of the Inspector class.
		/// </summary>
		public static Inspector Singleton 
		{
			get
			{
				return _Instance;
			}
		}
		
		private static readonly string _ThemePath = "res://addons/assetsnap/assets/themes/SnapTheme.tres";
		private static Inspector _Instance;
		
		private VBoxContainer _Control;
		private ScrollContainer _ScrollContainer;
		private VBoxContainer _InnerContainer;
		private GlobalExplorer _GlobalExplorer;
		private List<string> _Categories = new List<string>();
		private Godot.Collections.Dictionary<string, Node> _InspectorOptionInstances = new();
		
		/// <summary>
		/// Constructor for Inspector class.
		/// </summary>
		public Inspector()
		{
			Name = "Inspector";
			_Instance = this;
		}

		/// <summary>
		/// Method called before serialization.
		/// </summary>
		public void OnBeforeSerialize()
		{
			//
		}

		/// <summary>
		/// Method called after deserialization.
		/// </summary>
		public void OnAfterDeserialize()
		{
			_Instance = this;
		}

		/// <summary>
		/// Initializes the inspector.
		/// </summary>
		public void Initialize()
		{
			_GlobalExplorer = ExplorerUtils.Get();
		
			_Control = new()
			{
				Name = "AssetSnap Inspector",
				Theme = GD.Load<Theme>(_ThemePath),
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};

			_ScrollContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
				CustomMinimumSize = new Vector2(0, 200)
			};
			
			_InnerContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ExpandFill,
			};

			_ScrollContainer.AddChild(_InnerContainer);
			_Control.AddChild(_ScrollContainer); 

			_InitializeInspectorTitle();
			_BuildChildList();

			_GlobalExplorer._Plugin.StatesChanged += (Godot.Collections.Array data) => { _OnUpdate(data); };
		}	
		
		/// <summary>
		/// Adds the inspector to the dock.
		/// </summary>
		public void AddToDock()
		{
			if( null == _GlobalExplorer || null == _Control ) 
			{
				return;
			}
			
			_GlobalExplorer._Plugin.AddControlToDock( EditorPlugin.DockSlot.RightUl, _Control );
		}
		
		/// <summary>
		/// Adds a title to the inspector with the specified text.
		/// </summary>
		/// <param name="titleText">The text of the title to be added.</param>
		private void _AddTitle( string titleText )
		{
			VBoxContainer LabelContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			
			MarginContainer marginContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			marginContainer.SetMeta("ChildEntry", true);
			
			marginContainer.AddThemeConstantOverride("margin_left", 15);
			marginContainer.AddThemeConstantOverride("margin_right", 15);
			marginContainer.AddThemeConstantOverride("margin_top", 10);
			marginContainer.AddThemeConstantOverride("margin_bottom", 10);
			
			Label title = new()
			{
				Text = titleText,
				ThemeTypeVariation = "HeaderMedium",
			};
		
			LabelContainer.AddChild(title);
			marginContainer.AddChild(LabelContainer);
			_InnerContainer.AddChild(marginContainer);
		}
		
		/// <summary>
		/// Adds a checkbox with the specified name and value to the inspector.
		/// </summary>
		/// <param name="name">The name of the checkbox.</param>
		/// <param name="value">The initial value of the checkbox.</param>
		private void _AddCheckbox( string name, bool value ) 
		{
			PanelContainer panelContainer = new()
			{
				Name = "InspectorCheckboxPanelContainer",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};

			panelContainer.SetMeta("ChildEntry", true);

			MarginContainer marginContainer = new()
			{
				Name = "InspectorCheckboxMarginContainer",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			
			marginContainer.AddThemeConstantOverride("margin_left", 15);
			marginContainer.AddThemeConstantOverride("margin_right", 15);
			marginContainer.AddThemeConstantOverride("margin_top", 10);
			marginContainer.AddThemeConstantOverride("margin_bottom", 10);
			
			VBoxContainer OuterContainer = new()
			{
				Name = "InspectorCheckboxOuterContainer",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};

			HBoxContainer InputContainer = new()
			{
				Name = "InspectorCheckboxInputContainer",
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			CheckBox checkbox = new()
			{
				Text = name,
				ButtonPressed = value,
				Disabled = true,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};

			_InspectorOptionInstances.Add(name, checkbox);

			InputContainer.AddChild(checkbox);
			OuterContainer.AddChild(InputContainer);
			marginContainer.AddChild(OuterContainer);
			panelContainer.AddChild(marginContainer);
			_InnerContainer.AddChild(panelContainer);
		}
		
		/// <summary>
		/// Adds a label box with the specified name and value to the inspector.
		/// </summary>
		/// <param name="name">The name of the label box.</param>
		/// <param name="value">The initial value of the label box.</param>
		private void _AddLabelBox(string name, string value )
		{
			PanelContainer panelContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			
			panelContainer.SetMeta("ChildEntry", true);
			
			MarginContainer marginContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			
			marginContainer.AddThemeConstantOverride("margin_left", 15);
			marginContainer.AddThemeConstantOverride("margin_right", 15);
			marginContainer.AddThemeConstantOverride("margin_top", 10);
			marginContainer.AddThemeConstantOverride("margin_bottom", 10);
			
			VBoxContainer OuterContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};

			HBoxContainer LabelContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			Label label = new()
			{
				Text = name,
			};
			
			HBoxContainer InputContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			Label valueLabel = new()
			{
				Text = value,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
				AutowrapMode = TextServer.AutowrapMode.WordSmart,
			};
			_InspectorOptionInstances.Add(name, valueLabel);

			LabelContainer.AddChild(label);
			OuterContainer.AddChild(LabelContainer);
			
			InputContainer.AddChild(valueLabel);
			OuterContainer.AddChild(InputContainer);
			
			marginContainer.AddChild(OuterContainer);
			panelContainer.AddChild(marginContainer);
			_InnerContainer.AddChild(panelContainer);
		}
		
		/// <summary>
		/// Adds a spinbox with the specified name and value to the inspector.
		/// </summary>
		/// <param name="name">The name of the spinbox.</param>
		/// <param name="value">The initial value of the spinbox.</param>
		private void _AddSpinbox( string name, float value ) 
		{
			PanelContainer panelContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};

			panelContainer.SetMeta("ChildEntry", true);
			
			MarginContainer marginContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			
			marginContainer.AddThemeConstantOverride("margin_left", 15);
			marginContainer.AddThemeConstantOverride("margin_right", 15);
			marginContainer.AddThemeConstantOverride("margin_top", 10);
			marginContainer.AddThemeConstantOverride("margin_bottom", 10);
			
			VBoxContainer OuterContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};

			HBoxContainer LabelContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			Label label = new()
			{
				Text = name,
			};
			
			HBoxContainer InputContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			SpinBox spinbox = new()
			{
				Value = value,
				Editable = false,
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			_InspectorOptionInstances.Add(name, spinbox);

			LabelContainer.AddChild(label);
			OuterContainer.AddChild(LabelContainer);
			
			InputContainer.AddChild(spinbox);
			OuterContainer.AddChild(InputContainer);
			
			marginContainer.AddChild(OuterContainer);
			panelContainer.AddChild(marginContainer);
			_InnerContainer.AddChild(panelContainer);
		}
		
		/// <summary>
		/// Gets the name of the export category for the specified property.
		/// </summary>
		/// <param name="propertyInfo">The property info.</param>
		/// <returns>The name of the export category.</returns>
		private string _GetExportCategoryName(PropertyInfo propertyInfo)
		{
			// Check if the property has ExportCategory attribute
			ExportCategoryAttribute exportCategoryAttribute = propertyInfo.GetCustomAttribute<ExportCategoryAttribute>();

			if (exportCategoryAttribute != null)
			{
				return exportCategoryAttribute.Name;
			}

			// If the property does not have ExportCategory attribute, return empty string
			return "";
		}
		
		/// <summary>
		/// Initializes the inspector title with a predefined text and description.
		/// </summary>
		private void _InitializeInspectorTitle()
		{
			VBoxContainer LabelContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			
			MarginContainer marginContainer = new()
			{
				SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
				SizeFlagsVertical = Control.SizeFlags.ShrinkBegin,
			};
			
			marginContainer.AddThemeConstantOverride("margin_left", 15);
			marginContainer.AddThemeConstantOverride("margin_right", 15);
			marginContainer.AddThemeConstantOverride("margin_top", 10);
			marginContainer.AddThemeConstantOverride("margin_bottom", 10);
			
			Label title = new()
			{
				Text = "State Inspection",
				ThemeTypeVariation = "HeaderMedium",
			};
			
			Label description = new()
			{
				Text = "Use the field below to keep an eye on the various states currently set by the addon. These states will update in real time",
				AutowrapMode = TextServer.AutowrapMode.Word,
			};

			LabelContainer.AddChild(title);
			LabelContainer.AddChild(description);
			marginContainer.AddChild(LabelContainer);
			_InnerContainer.AddChild(marginContainer);
		}
		
		/// <summary>
		/// Builds the child list of the inspector, adding fields and properties as appropriate.
		/// </summary>
		private void _BuildChildList()
		{
			Type type = _GlobalExplorer.States.GetType();

			// Get all fields of the resource type
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

			// Get all properties of the resource type
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			// Iterate over fields and properties to find those marked as exported
			foreach (FieldInfo field in fields)
			{
				ExportAttribute exportAttribute = Attribute.GetCustomAttribute(field, typeof(ExportAttribute)) as ExportAttribute;
				if (exportAttribute != null)
				{
					// This field is exported
					object value = field.GetValue(_GlobalExplorer.States); // Get the value of the field
					if( value is bool BoolVal ) 
					{
						_AddCheckbox(field.Name, BoolVal);
					}
					else if( value is int intVal )
					{
						_AddSpinbox(field.Name, (float)intVal);
					}
					else if( value is double doubleVal )
					{
						_AddSpinbox(field.Name, (float)doubleVal);
					}
					else if( value is float floatVal )
					{
						_AddSpinbox(field.Name, floatVal);
					}
					else if( null != value ) 
					{
						// GD.Print(field.Name);
					}
				}
			}
			
			foreach (PropertyInfo property in properties)
			{
				ExportAttribute exportAttribute = Attribute.GetCustomAttribute(property, typeof(ExportAttribute)) as ExportAttribute;
				if ( null != exportAttribute )
				{
					string title = _GetExportCategoryName(property);
					if( "" != title ) 
					{
						if( false == _Categories.Contains(title) ) 
						{
							_AddTitle(title);
							_Categories.Add(title);							
						}
					}
					
				  	// This property is exported
					object value = property.GetValue(_GlobalExplorer.States); // Get the value of the property
					
					if( value is bool BoolVal ) 
					{
						_AddCheckbox(property.Name, BoolVal);
					}
					else if( value is int intVal )
					{
						_AddSpinbox(property.Name, (float)intVal);
					}
					else if( value is double doubleVal )
					{
						_AddSpinbox(property.Name, (float)doubleVal);
					}
					else if( value is float floatVal )
					{
						_AddSpinbox(property.Name, floatVal);
					}
					else if( value is Library.Instance libraryValue )
					{
						_AddLabelBox(property.Name, libraryValue.GetName());
					}
					else if( value is Node NodeValue )
					{
						if( EditorPlugin.IsInstanceValid( NodeValue ) ) 
						{
							_AddLabelBox(property.Name, NodeValue.Name);
						}
					}
					else if( null != value )
					{
						_AddLabelBox(property.Name, value.ToString());
					}
					else 
					{
						_AddLabelBox(property.Name, "N/A");
					}
				}
			}
		}
		
		/// <summary>
		/// Clears the current children of the inspector.
		/// </summary>
		private void _ClearCurrentChildren()
		{
			foreach( Node child in _InnerContainer.GetChildren() ) 
			{
				if( child.HasMeta("ChildEntry") ) 
				{
					_InnerContainer.RemoveChild(child);
					child.QueueFree();
				}
			}

			_Categories = new();
		}
		
		/// <summary>
		/// Callback method invoked when the inspector needs to be updated.
		/// </summary>
		/// <param name="data">The data array containing information about the update.</param>
		private void _OnUpdate( Godot.Collections.Array data )
		{
			string key = data[0].As<string>();
			if( _InspectorOptionInstances.ContainsKey(key) ) 
			{
				GodotObject _object = _InspectorOptionInstances[key];
				
				if( _object is SpinBox spinbox ) 
				{
					spinbox.Value = data[1].As<double>();
				}
				
				if( _object is CheckBox checkbox ) 
				{
					checkbox.ButtonPressed = data[1].As<bool>();
				}
				
				if( _object is Label label ) 
				{
					if( data[1].As<GodotObject>() is Library.Instance libraryValue )
					{
						label.Text = libraryValue.GetName();
					}
					else if( data[1].As<GodotObject>() is Node NodeValue )
					{
						if( EditorPlugin.IsInstanceValid( NodeValue ) ) 
						{
							label.Text = NodeValue.Name;
						}
					}
					else if( "" != data[1].As<string>() )
					{
						label.Text = data[1].As<string>();
					}
					else 
					{
						label.Text = "N/A";
					}
				}
			}
		}
	}
}
#endif