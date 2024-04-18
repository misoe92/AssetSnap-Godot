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
	using AssetSnap.Component;
	using AssetSnap.Front.Nodes;
	using Godot;

	[Tool]
	public partial class Editor : LibraryComponent
	{	
		public Editor()
		{
			// _include = false;
			Name = "GroupBuilderEditor";
			SizeFlagsVertical = Control.SizeFlags.ExpandFill;
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
		}
		
		public GroupResource Group {
			get => _Group;
			set
			{
				if( _Group != value ) 
				{
					_Group = value;
					_GlobalExplorer.States.Group = value;

					Listing.Show();
					GroupOptions.Hide(); 
		
					Update();
				}
			}
		}
		
		public string GroupPath {
			get => _GroupPath;
			set
			{
				_GroupPath = value;
				if( value != "" && null != value ) 
				{
					Resource _resource = GD.Load<Resource>(value);
					Group = _resource as GroupResource;
				}
				else 
				{
					Group = null;
				}
			}
		}

		/*
		** Components
		*/
		public EditorTopbar Topbar;
		public EditorListing Listing;
		public EditorGroupOptions GroupOptions;
		
		/*
		** Group Resource
		*/
		private GroupResource _Group;
		private string _GroupPath = "";

		public override void Initialize()
		{
			base.Initialize();
		
			Initiated = true;
			_SetupTopbar();
			_SetupGroupItemsList();
			_SetupGroupOptions();

			Plugin.GetInstance()
				.Connect(
					Plugin.SignalName.StatesChanged,
					Callable.From(
						() => { GroupOptions._UpdateGroupOptions();  }
					)
				);
		}
		
		public void Update()
		{
			Topbar.Update();
			
			if (Group == null || false == IsInstanceValid(Group))
			{
				if( "" != GroupPath ) 
				{
					GD.PushWarning("Failed to find a valid group to display");
				}

				Listing.Reset();
				Listing.Update();
			
				return;
			}

			Listing.Reset();
			Listing.Update();
			
			GroupOptions._UpdateGroupOptions();

			if( _GlobalExplorer.States.GroupedObjects.ContainsKey(GroupPath) ) 
			{
				foreach( Node3D node in _GlobalExplorer.States.GroupedObjects[GroupPath] ) 
				{
					if( node is AsGrouped3D asGrouped3D ) 
					{
						asGrouped3D.Update();
					}
				}
			}	
		}
			
		public void OpenGroupOptions()
		{
			GroupOptions.DoShow();
			Listing.DoHide();
		}
		
		public void AddMeshToGroup( string MeshPath )
		{
			if( null == Group ) 
			{
				return;
			}

			Group._Paths.Add(MeshPath);
			Group._Origins.Add(Group._Paths.Count - 1, Vector3.Zero);
			Group._Rotations.Add(Group._Paths.Count - 1, Vector3.Zero);
			Group._Scales.Add(Group._Paths.Count - 1, new Vector3(1,1,1));

			ResourceSaver.Save(Group, "res://groups/" + Group.Name + ".tres");
			GlobalExplorer.GetInstance().GroupMainScreen.Update();
			Update();
		}

		public void UpdateMeshInGroup(int index, Vector3 Origin, Vector3 Rotation, Vector3 Scale)
		{
			bool NameChanged = false;
			string OldName = "";
			
			if( false == Topbar.TitleEquals(Group.Name) ) 
			{
				NameChanged = true;
				OldName = Group.Name;
				Group.Name = Topbar.GetTitle();
			}
	
			Group._Origins[index] = Origin;
			Group._Rotations[index] = Rotation;
			Group._Scales[index] = Scale;

			if( NameChanged ) 
			{
				GlobalExplorer.GetInstance().GroupBuilder._Sidebar.RemoveGroup("res://groups/" + OldName + ".tres");
				ResourceSaver.Save(Group, "res://groups/" + Group.Name + ".tres");
			}
			else 
			{
				ResourceSaver.Save(Group, "res://groups/" + Group.Name + ".tres");
			}
			
			GlobalExplorer.GetInstance().GroupMainScreen.Update();
		}
		
		public void DuplicateMeshInGroup( int index )
		{
			if( null == Group ) 
			{
				return;
			}
			
			Vector3 OldOrigins = Group._Origins[index];
			Vector3 OldRotation = Group._Rotations[index];
			Vector3 OldScale = Group._Scales[index];
			
			Group._Paths.Add(Group._Paths[index]);
			Group._Origins.Add(Group._Paths.Count - 1, OldOrigins);
			Group._Rotations.Add(Group._Paths.Count - 1, OldRotation);
			Group._Scales.Add(Group._Paths.Count - 1, OldScale);

			ResourceSaver.Save(Group, "res://groups/" + Group.Name + ".tres");
			GlobalExplorer.GetInstance().GroupMainScreen.Update();
			Update();
		}
		
		public void RemoveMeshInGroup(int index, string path)
		{
			Godot.Collections.Dictionary<int, Vector3> OldOrigins = Group._Origins;
			Godot.Collections.Dictionary<int, Vector3> OldRotation = Group._Rotations;
			Godot.Collections.Dictionary<int, Vector3> OldScale = Group._Scales;

			int newIndex = 0;
			Group._Origins = new();
			Group._Rotations = new();
			Group._Scales = new();
			
			for( int i = 0; i < Group._Paths.Count; i++ ) 
			{
				if( i != index ) 
				{
					Group._Origins.Add(newIndex, OldOrigins[i]);
					Group._Rotations.Add(newIndex, OldRotation[i]);
					Group._Scales.Add(newIndex, OldScale[i]);

					newIndex++;
				}
			}

			Group._Paths.Remove(path);
			
			ResourceSaver.Save(Group, "res://groups/" + Group.Name + ".tres");
			
			GlobalExplorer.GetInstance().GroupMainScreen.Update();
			Update();
		}
		
		public void UpdateGroup()
		{
			bool NameChanged = false;
			string OldName = "";
			
			if( Topbar.TitleEquals( Group.Name ) == false ) 
			{
				NameChanged = true;
				OldName = Group.Name;
				Group.Name = Topbar.GetTitle();
			}

			if( NameChanged ) 
			{
				ResourceSaver.Save(Group, "res://groups/" + Group.Name + ".tres");
				GlobalExplorer.GetInstance().GroupBuilder._Sidebar.RemoveGroup("res://groups/" + OldName + ".tres");
				GroupPath = "res://groups/" + Group.Name + ".tres";
			}
			else 
			{
				ResourceSaver.Save(Group, "res://groups/" + Group.Name + ".tres");
			}
			
			Listing.DoShow();
			GroupOptions.DoHide(); 

			GlobalExplorer.GetInstance().GroupBuilder._Sidebar.RefreshExistingGroups();
			
			if( _GlobalExplorer.States.GroupedObjects.ContainsKey(GroupPath) ) 
			{
				foreach( Node3D node in _GlobalExplorer.States.GroupedObjects[GroupPath] ) 
				{
					if( node is AsGrouped3D asGrouped3D ) 
					{
						asGrouped3D.Update();
					}
				}
			}	
		}
		
		public void SetOption( int index, string key, Variant value ) 
		{
			if( Group._Options.Count > index && Group._Options[index].ContainsKey( key ) ) 
			{
				Group._Options[index].Remove(key);
			}
			
			if( Group._Options.Count > index ) 
			{
				Group._Options[index].Add(key, value);
			}
			else 
			{
				Group._Options.Add(new() { { key, value } });
			}
		}
		
		/*
		** Set's up the list title
		** 
		** @return void
		*/
		private void _SetupTopbar()
		{
			List<string> Components = new()
			{
				"Groups.BuilderEditor.Topbar",
			};
			
			if (_GlobalExplorer.Components.HasAll( Components.ToArray() )) 
			{
				Topbar = _GlobalExplorer.Components.Single<EditorTopbar>();
				Topbar.Container = this;
				Topbar.Initialize();
			}
		}
		
		private void _SetupGroupItemsList()
		{
			List<string> Components = new()
			{
				"Groups.Builder.EditorListing",
			};
			
			if (_GlobalExplorer.Components.HasAll( Components.ToArray() )) 
			{
				Listing = _GlobalExplorer.Components.Single<EditorListing>();
				Listing.Container = this;
				Listing.Initialize();
			}
		}
		
		private void _SetupGroupOptions()
		{
			List<string> Components = new()
			{
				"Groups.Builder.EditorGroupOptions",
			};
			
			if (_GlobalExplorer.Components.HasAll( Components.ToArray() )) 
			{
				GroupOptions = _GlobalExplorer.Components.Single<EditorGroupOptions>();
				GroupOptions.Container = this;
				GroupOptions.Initialize();
			}
		}
	}
}