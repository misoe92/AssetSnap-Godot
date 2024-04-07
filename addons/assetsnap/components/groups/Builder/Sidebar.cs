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
	using System.Collections.Generic;
	using System.IO;
	using AssetSnap.Component;
	using AssetSnap.Front.Nodes;
	using AssetSnap.Helpers;
	using Godot;

	[Tool]
	public partial class GroupBuilderSidebar : LibraryComponent
	{	
		public GroupBuilderSidebar()
		{
			Name = "GroupBuilderSidebar";
			//_include = false;
		}
		
		private readonly string TitleText = "Current Groups";
		private readonly string ButtonText = "Create new Group";

		private VBoxContainer GroupContainer;

		private Godot.Collections.Dictionary<string, GroupBuilderListingEntry> _Instances; 
				
		public override void Initialize()
		{
			AddTrait(typeof(Buttonable));
			AddTrait(typeof(Containerable));
			AddTrait(typeof(Labelable));
			
			Initiated = true;
			
			_SetupTopbar();
			_SetupExistingGroupList();
		}
		
		public void Show()
		{
			List<string> OuterComponents = new()
			{
				"GroupContainer",
			};
		
			/** Add the tab item if settings is turned on **/
			Component.Base Components = _GlobalExplorer.Components;
			if (Components.HasAll(OuterComponents.ToArray()))
			{
				GroupContainer _GroupContainer = Components.Single<GroupContainer>();
				_GroupContainer.GetLeftInnerContainer().Visible = true;
			}
		}
		
		public void Hide()
		{
			List<string> OuterComponents = new()
			{
				"GroupContainer",
			};
		
			/** Add the tab item if settings is turned on **/
			Component.Base Components = _GlobalExplorer.Components;
			if (Components.HasAll(OuterComponents.ToArray()))
			{
				GroupContainer _GroupContainer = Components.Single<GroupContainer>();
				_GroupContainer.GetLeftInnerContainer().Visible = false;
			}
		}
		
		public void Update()
		{
			if( null == _GlobalExplorer ) 
			{
				_GlobalExplorer = GlobalExplorer.GetInstance();
			}
			
			string path = _GlobalExplorer.GroupBuilder._Editor.GroupPath;
			
			if( path == "" ) 
			{
				// None chosen, show sidebar
				Show();
			}
			else 
			{
				Hide();
			}
		}
		
		public void FocusGroup(string FilePath) 
		{
			foreach( (string fp, GroupBuilderListingEntry _Instance ) in _Instances ) 
			{
				if( fp == FilePath ) 
				{
					_Instance.Focus();
				}
				else 
				{
					_Instance.Unfocus();
				}
			}
		}
		
		public void UnFocusGroup(string FilePath) 
		{
			foreach( (string fp, GroupBuilderListingEntry _Instance ) in _Instances ) 
			{
				if( fp == FilePath ) 
				{
					_Instance.Unfocus();
				}
			}
		}
		
		public void SelectGroup(string FilePath) 
		{
			foreach( (string fp, GroupBuilderListingEntry _Instance ) in _Instances ) 
			{
				if( fp == FilePath ) 
				{
					_Instance.SelectGroup();
				}
			}
		}
		
		public void DeselectGroup(string FilePath)
		{
			foreach( (string fp, GroupBuilderListingEntry _Instance ) in _Instances ) 
			{
				if( fp == FilePath ) 
				{
					_Instance.DeselectGroup();
				}
			}
		}
		
		private void _SetupTopbar()
		{
			Trait<Containerable>()
				.SetName("GroupBuilderSidebarContainer")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetLayout(Containerable.ContainerLayout.TwoColumns)
				.SetMargin(10, "top")
				.SetMargin(10, "left")
				.SetMargin(10, "right")
				.SetMargin(4, "bottom")
				.SetOrientation(Containerable.ContainerOrientation.Horizontal)
				.Instantiate();

			Container InnerContainerLeft = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(0);
				
			Container InnerContainerRight = Trait<Containerable>()
				.Select(0)
				.GetInnerContainer(1);

			InnerContainerLeft.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
			InnerContainerRight.SizeFlagsHorizontal = Control.SizeFlags.ShrinkEnd;
			
			_SetupListTitle(InnerContainerLeft);
			_SetupNewGroupButton(InnerContainerRight);

			Trait<Containerable>()
				.AddToContainer(
					Container
				);
		}
		
		private void _SetupExistingGroupList()
		{
			GroupContainer = new();
			_Instances = new();

			string[] Groups = _GetCurrentGroupPaths();

			foreach( string path in Groups ) 
			{
				string title = path;
				List<string> Components = new()
				{
					"GroupBuilderListingEntry",
				};
				
				if (GlobalExplorer.GetInstance().Components.HasAll( Components.ToArray() )) 
				{
					GroupBuilderListingEntry SingleEntry = GlobalExplorer.GetInstance().Components.Single<GroupBuilderListingEntry>(true);
					
					SingleEntry.title = title;
					SingleEntry.Container = GroupContainer;
					SingleEntry.Initialize();

					_Instances.Add(title, SingleEntry);
				}
			}
			
			Container.AddChild(GroupContainer);
		}
		
		public void RefreshExistingGroups()
		{
			GroupContainer.GetParent().RemoveChild(GroupContainer);
			GroupContainer.QueueFree();

			_SetupExistingGroupList();
		}
		
		private void _SetupListTitle( Container container ) 
		{
			Trait<Labelable>()
				.SetName("GroupBuilderSidebarTitle")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetType(Labelable.TitleType.HeaderMedium)
				.SetText(TitleText)
				.SetMargin(0)
				.Instantiate()
				.Select(0)
				.AddToContainer(
					container
				);
		}
		
		private void _SetupNewGroupButton( Container container )
		{
			Trait<Buttonable>()
				.SetName("GroupBuilderSidebarNewGroupBtn")
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkEnd)
				.SetType(Buttonable.ButtonType.SmallActionButton)
				.SetText("Create new")
				.SetAction(() => { _SetupNewGroup(); })
				.Instantiate()
				.Select(0)
				.AddToContainer(
					container
				);
		}
		
		private string[] _GetCurrentGroupPaths()
		{
			// Directory path of the 'res://groups' folder
			string directoryPath = "res://groups/";

			// Get an array of file paths inside the directory
			string[] filePaths = _GetFilePathsInDirectory(directoryPath);
			
			return filePaths;
		}
		
		private string[] _GetFilePathsInDirectory(string directoryPath)
		{
			// Initialize an empty list to store file paths
			var filePaths = new List<string>();
			Godot.DirAccess files = Godot.DirAccess.Open(directoryPath);
			// Ensure the directory exists
			if ( null != files )
			{
				// Iterate through each file path
				foreach (string file in files.GetFiles())
				{
					// Convert the file path to a Godot-friendly path
					string godotPath = ProjectSettings.GlobalizePath(file);
					// Add the Godot-friendly path to the list
					filePaths.Add(directoryPath + godotPath);
				}
			}
			else
			{
				GD.PrintErr("Directory not found: " + directoryPath);
			}

			// Convert the list to an array and return
			return filePaths.ToArray();
		}
		
		private void _SetupNewGroup()
		{
			string Name = "Group-" + UniqueHelper.GenerateId();
			GroupResource _Resource = new()
			{
				Name = Name
			};
			
			string savePath = "res://groups/" + Name + ".tres";
			// Save the texture to the specified path
			Error success = ResourceSaver.Save(_Resource, savePath);

			if (success == Error.Ok)
			{
				GD.Print("New group saved successfully at: " + savePath);
			}
			else
			{
				GD.PrintErr("Failed to save group at: " + savePath);
			}

			RefreshExistingGroups();
		}
		
		public void RemoveGroup( string filepath )
		{
			string absolutePath = ProjectSettings.GlobalizePath(filepath);
			// Check if the file exists
			if (File.Exists(absolutePath))
			{
				// Delete the file
				File.Delete(absolutePath);

				GD.Print("File removed successfully: " + absolutePath);
			}
			else
			{
				GD.PrintErr("File not found: " + absolutePath);
			}
		}

		public override void _ExitTree()
		{
			_Instances = null; 
			base._ExitTree();
		}
	}
}