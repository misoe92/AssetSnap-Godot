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

using AssetSnap.Explorer;
using AssetSnap.States;
using Godot;

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// Represents a PanelContainer used in a library for displaying assets.
	/// </summary>
	[Tool]
	public partial class AsLibraryPanelContainer : PanelContainer
	{
		/// <summary>
		/// Gets or sets the library instance.
		/// </summary>
		[Export]
		public AssetSnap.Library.Instance Library
		{
			get => _Library;
			set
			{
				_Library = value;
			}
		}
		
		private string _FileName = "";
		private string _Path = "";
		private float _TransitionSpeed = 6.0f; // Adjust the transition speed as needed
		private bool _Active = false;
		private bool _IsMouseOver = false;
		private Color _TargetColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		private Mesh _Mesh;
		private Node3D _Instance;
		private Resource _Ressource;
		private AssetSnap.Library.Instance _Library;

		

		/// <summary>
		/// Sets flags, default data, and connects to signals.
		/// </summary>
		public override void _Ready()
		{
			SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
			SizeFlagsVertical = Control.SizeFlags.ShrinkBegin;
			CustomMinimumSize = new Vector2(0, 140);
			MouseDefaultCursorShape = Control.CursorShape.PointingHand;

			Connect(PanelContainer.SignalName.GuiInput, new Callable(this, "_ForwardGuiInput"));
			Connect(Control.SignalName.MouseEntered, new Callable(this, "_OnMouseEntered"));
			Connect(Control.SignalName.MouseExited, new Callable(this, "_OnMouseExited"));

			Plugin.Singleton.Connect(Plugin.SignalName.LibraryChanged, Callable.From((string name) => { _OnLibraryChange(name); }));
		}

		/// <summary>
		/// Handles smooth transition of background color.
		/// </summary>
		public override void _Process(double delta)
		{
			// Smoothly interpolate the modulate property toward the target color
			SelfModulate = SelfModulate.Lerp(_TargetColor, (float)delta * _TransitionSpeed);
		}

		/// <summary>
		/// Handles click event on the panel, enabling the active state of the panel.
		/// </summary>
		/// <param name="_event">The input event.</param>
		public void _ForwardGuiInput(InputEvent _event)
		{
			if (_event is InputEventMouseButton _buttonEvent && _buttonEvent.Position.X > 29 && _buttonEvent.Position.X < 161)
			{
				if (false == _buttonEvent.Pressed && _buttonEvent.ButtonIndex == MouseButton.Left && _Active == false)
				{
					if (_FileName == null || _FileName == "")
					{
						GD.PushError("Filename was not available, hence model could not be selected");
						return;
					}

					if (_Ressource == null)
					{
						GD.PushError("Ressource was not available, hence model could not be selected");
						return;
					}

					if (Library == null)
					{
						GD.PushError("Library was not available, hence model could not be selected");
						return;
					}

					Library.ClearActivePanelState(this);
					SetState(true);
					PrepareMeshInstance();

					Library._LibrarySettings.SnapToHeight.SetState(true);
					Library._LibrarySettings.SnapToHeight.Sync();

					ExplorerUtils.Get().SetFocusToNode(_Instance);

					StatesUtils.Get().GroupedObject = null;
					StatesUtils.Get().Group = null;
				}
				else if (false == _buttonEvent.Pressed && _buttonEvent.ButtonIndex == MouseButton.Left && _Active == true)
				{
					ExplorerUtils.Get().SetFocusToNode(null);
					SetState(false);
				}
				else if (false == _buttonEvent.Pressed && _buttonEvent.ButtonIndex == MouseButton.Right)
				{
					ExplorerUtils.Get().GroupBuilder.ShowMenu(_buttonEvent.GlobalPosition, _Path);
				}
			}
		}

		/// <summary>
		/// Handles the event when the library changes.
		/// </summary>
		/// <param name="LibraryName">The name of the library.</param>
		private void _OnLibraryChange(string LibraryName)
		{
			if (IsActive() && Library.GetName() == LibraryName)
			{
				if (_Instance == StatesUtils.Get().EditingObject)
				{
					return;
				}

				if (StatesUtils.Get().MultiDrop)
				{
					return;
				}

				ExplorerUtils.Get().SetFocusToNode(_Instance);
			}
		}

		/// <summary>
		/// Sets the file path for the Library Panel.
		/// </summary>
		/// <param name="path">The file path.</param>
		public void SetFilePath(string path)
		{
			_Path = path;
			var _FileNameArr = path.Split('/');
			_FileName = _FileNameArr[_FileNameArr.Length - 1].Split('.')[0];
			_Ressource = GD.Load(_Path);
		}

		/// <summary>
		/// Sets the state of the library panel.
		/// </summary>
		/// <param name="state">The state to set.</param>
		public void SetState(bool state)
		{
			_Active = state;
			_TargetColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

			if (state == true)
			{
				Theme = GD.Load<Theme>("res://addons/assetsnap/assets/themes/ActivePanelContainer.tres");
			}
			else
			{
				// Remove active theme
				Theme = null;
			}
		}

		/// <summary>
		/// Checks if the library panel's state is active.
		/// </summary>
		/// <returns>True if active, false otherwise.</returns>
		public bool IsActive()
		{
			if (_Active == true)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Prepares the MeshInstance for rendering based on the resource type.
		/// </summary>
		private void PrepareMeshInstance()
		{
			if (_Ressource is Mesh _Mesh)
			{
				var FileNameSplit = _FileName.Split("/");
				AssetSnap.Front.Nodes.AsMeshInstance3D instance = new()
				{
					Floating = true,
					Mesh = _Mesh,
					Name = FileNameSplit[FileNameSplit.Length - 1],
					SpawnSettings = new(),
					LibraryName = Library.GetName(),
				};

				_Instance = instance;
			}
			else if (_Ressource is PackedScene _Scene)
			{
				var FileNameSplit = _FileName.Split("/");

				Variant sceneInstance = _Scene.Instantiate();
				Node3D node = sceneInstance.As<Node3D>();

				AsNode3D AsNode = new()
				{
					Floating = true,
					Name = FileNameSplit[FileNameSplit.Length - 1],
					SpawnSettings = new(),
					Transform = node.Transform,
					LibraryName = Library.GetName(),
				};

				if (node.GetChildCount() > 0)
				{
					foreach (Node child in node.GetChildren())
					{
						if (child is MeshInstance3D meshinstance3D)
						{
							AsMeshInstance3D meshinstanceChild = new AsMeshInstance3D()
							{
								Name = meshinstance3D.Name,
								Mesh = meshinstance3D.Mesh,
								Transform = meshinstance3D.Transform,
								SpawnSettings = new(),
								LibraryName = Library.GetName(),
							};

							Transform3D transform = meshinstance3D.Transform;
							transform.Origin.Y = meshinstance3D.GetAabb().Size.Y / 2;
							meshinstance3D.Transform = transform;

							AsNode.AddChild(
								meshinstanceChild
							);
						}
					}
				}

				_Instance = AsNode;
				node.QueueFree();
			}
		}

		/// <summary>
		/// Handles mouse enter of the hover functionality.
		/// </summary>
		private void _OnMouseEntered()
		{
			_IsMouseOver = true;

			if (_Active == false)
			{
				_TargetColor = new Color(0.25f, 0.25f, 0.25f, 1.0f);
			}
			else
			{
				_TargetColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			}
		}

		/// <summary>
		/// Handles mouse exit of the hover functionality.
		/// </summary>
		private void _OnMouseExited()
		{
			_IsMouseOver = false;
			_TargetColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		}

		/// <summary>
		/// Cleans up references, parameters, and fields.
		/// </summary>
		public override void _ExitTree()
		{
			_Active = false;
			_Library = null;
			_Mesh = null;
			_Ressource = null;

			if (null != GlobalExplorer.GetInstance() && null != GlobalExplorer.GetInstance()._Plugin && GlobalExplorer.GetInstance()._Plugin.IsConnected(Plugin.SignalName.LibraryChanged, Callable.From((string name) => { _OnLibraryChange(name); })))
			{
				GlobalExplorer.GetInstance()._Plugin.Disconnect(Plugin.SignalName.LibraryChanged, Callable.From((string name) => { _OnLibraryChange(name); }));
			}

			if (IsConnected(PanelContainer.SignalName.GuiInput, new Callable(this, "_ForwardGuiInput")))
			{
				Disconnect(PanelContainer.SignalName.GuiInput, new Callable(this, "_ForwardGuiInput"));
			}
			if (IsConnected(Control.SignalName.MouseEntered, new Callable(this, "_OnMouseEntered")))
			{
				Disconnect(Control.SignalName.MouseEntered, new Callable(this, "_OnMouseEntered"));
			}
			if (IsConnected(Control.SignalName.MouseExited, new Callable(this, "_OnMouseExited")))
			{
				Disconnect(Control.SignalName.MouseExited, new Callable(this, "_OnMouseExited"));
			}

			if (IsInstanceValid(_Instance))
			{
				_Instance.QueueFree();
			}
		}
	}
}

#endif