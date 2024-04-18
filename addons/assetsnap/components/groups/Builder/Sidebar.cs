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

namespace AssetSnap.Front.Components.Groups.Builder
{
	using System.Collections.Generic;
	using System.IO;
	using AssetSnap.Component;
	using AssetSnap.Explorer;
	using AssetSnap.Front.Nodes;
	using AssetSnap.Helpers;
	using Godot;

	[Tool]
	public partial class Sidebar : LibraryComponent
	{	
		private readonly string TitleText = "Current Groups";
		private readonly string ButtonText = "Create new Group";

		private VBoxContainer GroupContainer;

		private Godot.Collections.Dictionary<string, ListingEntry> _Instances; 
		
		public Sidebar()
		{
			Name = "GroupBuilderSidebar";
			
			UsingTraits = new()
			{
				{ typeof(Buttonable).ToString() },
				{ typeof(Labelable).ToString() },
				{ typeof(Containerable).ToString() },
			};
			
			SizeFlagsVertical = Control.SizeFlags.ExpandFill;
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
			
			//_include = false;
		}
		
		public override void Initialize()
		{
			base.Initialize();
			Initiated = true;
			
			_SetupTopbar();
			_SetupExistingGroupList();
		}
		
		public void DoShow()
		{
			List<string> OuterComponents = new()
			{
				"Groups.Container",
			};
		
			/** Add the tab item if settings is turned on **/
			Component.Base Components = _GlobalExplorer.Components;
			if (Components.HasAll(OuterComponents.ToArray()))
			{
				Groups.Container _GroupContainer = Components.Single<Groups.Container>();
				_GroupContainer.GetLeftInnerContainer().Visible = true;
			}
		}
		
		public void DoHide()
		{
			List<string> OuterComponents = new()
			{
				"Groups.Container",
			};
		
			/** Add the tab item if settings is turned on **/
			Component.Base Components = _GlobalExplorer.Components;
			if (Components.HasAll(OuterComponents.ToArray()))
			{
				Groups.Container _GroupContainer = Components.Single<Groups.Container>();
				_GroupContainer.GetLeftInnerContainer().Visible = false;
			}
		}
		
		public void Update()
		{
			string path = ExplorerUtils.Get()
				.GroupBuilder
				._Editor
				.GroupPath;
			
			if( path == "" ) 
			{
				// None chosen, show sidebar
				DoShow();
			}
			else 
			{
				DoHide();
			}
		}
		
		public void FocusGroup(string FilePath) 
		{
			foreach( (string fp, ListingEntry _Instance ) in _Instances ) 
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
			foreach( (string fp, ListingEntry _Instance ) in _Instances ) 
			{
				if( fp == FilePath ) 
				{
					_Instance.Unfocus();
				}
			}
		}
		
		public void SelectGroup(string FilePath) 
		{
			foreach( (string fp, ListingEntry _Instance ) in _Instances ) 
			{
				if( fp == FilePath ) 
				{
					_Instance.SelectGroup();
				}
			}
		}
		
		public void DeselectGroup(string FilePath)
		{
			foreach( (string fp, ListingEntry _Instance ) in _Instances ) 
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
				
			if( null == InnerContainerLeft || null == InnerContainerRight ) 
			{
				GD.PushError("No inner container was found @ group topbar");
			}

			InnerContainerLeft.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
			InnerContainerRight.SizeFlagsHorizontal = Control.SizeFlags.ShrinkEnd;
			
			_SetupListTitle(InnerContainerLeft);
			_SetupNewGroupButton(InnerContainerRight);

			Trait<Containerable>()
				.Select(0)
				.AddToContainer(
					this
				);
		}
		
		private void _SetupExistingGroupList()
		{
			_Instances = new();

			string[] Groups = _GetCurrentGroupPaths();

			foreach( string path in Groups ) 
			{
				string title = path;
				List<string> Components = new()
				{
					"Groups.Builder.ListingEntry",
				};
				
				if (GlobalExplorer.GetInstance().Components.HasAll( Components.ToArray() )) 
				{
					ListingEntry SingleEntry = GlobalExplorer.GetInstance().Components.Single<ListingEntry>(true);
					
					SingleEntry.title = title;
					SingleEntry.Container = this;
					SingleEntry.Initialize();

					_Instances.Add(title, SingleEntry);
					// AddChild(SingleEntry);
				}
			}
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
				.SetMargin(2, "top")
				.Instantiate()
				.Select(0)
				.AddToContainer(
					container
				);
		}
		
		private void _SetupNewGroupButton( Container container )
		{
			Trait<Containerable>()
				.SetName("NewGroupButtonContainer")
				.SetMargin(4, "top")
				.SetHorizontalSizeFlags(Control.SizeFlags.ExpandFill)
				.SetLayout(Containerable.ContainerLayout.OneColumn)
				.SetOrientation(Containerable.ContainerOrientation.Horizontal)
				.Instantiate()
				.Select(1)
				.AddToContainer(container);

			Trait<Buttonable>()
				.SetName("GroupBuilderSidebarNewGroupBtn")
				.SetHorizontalSizeFlags(Control.SizeFlags.ShrinkEnd)
				.SetType(Buttonable.ButtonType.SmallActionButton)
				.SetText("Create new")
				.SetAction(() => { _SetupNewGroup(); })
				.Instantiate()
				.Select(0)
				.AddToContainer(
					Trait<Containerable>()
						.Select(1)
						.GetInnerContainer()
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

		public override void Clear(bool debug = false)
		{
			base.Clear(debug);
		}

		public override void _ExitTree()
		{
			_Instances = null; 
			base._ExitTree();
		}
	}
}