using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public static class EditorTabsFetcher
{
	// Method to fetch the names of current tabs in the editor
	public static List<string> GetEditorTabs()
	{
		var tabs = new List<string>();

		// Access the editor interface
		var editorInterface = EditorInterface.Singleton;
		if (editorInterface != null)
		{
			// Get the main editor view
			var mainEditorView = editorInterface.GetBaseControl();
			if (mainEditorView != null)
			{
				// Assuming the tabs are contained within a TabContainer
				var tabContainers = FindTabContainer(mainEditorView, new());
				foreach( TabContainer tabContainer in tabContainers ) 
				{
					if (tabContainer != null && 0 != tabContainer.GetTabCount() )
					{
						// tabs.Add(tabContainer.GetPath());
						// // Iterate through the tabs and add their names to the list
						// for (int i = 0; i < tabContainer.GetTabCount(); i++)
						// {
						// 	tabs.Add(tabContainer.GetChild(i).Name);
						// }
					}
					else 
					{
						tabs.Add(tabContainer.GetPath());
						
					}
				}
				
			}
		}

		return tabs;
	}


	// Helper method to find the TabContainer within the editor view
	private static Godot.Collections.Array<TabContainer> FindTabContainer(Control control,Godot.Collections.Array<TabContainer> Containers)
	{
		if( 0 != control.GetChildCount() ) 
		{
			foreach (Node child in control.GetChildren())
			{
				if (control is Godot.TabContainer tabContainer)
				{
					Containers.Add(tabContainer);
				}
				else if( child is Control childControl && 0 != control.GetChildCount() ) 
				{
					Containers = FindTabContainer(childControl, Containers);
				}
			}
		}

		return Containers;
	}
	
	public static bool HasProperty(Control control, string propertyName)
	{
		Type type = control.GetType();
		PropertyInfo propertyInfo = type.GetProperty(propertyName);
		return propertyInfo != null;
	}
		
	public static bool TryGetProperty<T>(Control control, string propertyName, out T value)
	{
		value = default(T);
		Type type = control.GetType();
		PropertyInfo propertyInfo = type.GetProperty(propertyName);
		if (propertyInfo != null)
		{
			try
			{
				value = (T)propertyInfo.GetValue(control);
				return true;
			}
			catch (Exception e)
			{
				GD.PrintErr($"Failed to get property {propertyName}: {e.Message}");
				return false;
			}
		}
		else
		{
			return false;
		}
	}
}