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

using System.Threading.Tasks;
using Godot;

namespace AssetSnap.GroupBuilder
{
	/// <summary>
	/// Main screen class responsible for managing the group preview and user input.
	/// </summary>
	[Tool]
	public partial class MainScreen : VBoxContainer
	{
		public bool PreviewEnvironment
		{
			get => _PreviewEnvironment;
			set
			{
				_PreviewEnvironment = value;
				_MaybeInitializeEnvironment(_Viewport);
			}
		}
		
		public bool ListensForInput
		{
			get => _ListensForInput;
			set
			{
				_ListensForInput = value;
			}
		}
		
		private float _Levitate = 0.0f;
		private float _LevitateRate = 1.5f;
		private float _MoveSpeedRate = 6.0f;
		
		// Sensitivity for mouse movement
		private float _MouseSensitivity = 0.25f;
		private float _MouseYSensitivity = 10;

		// Variables to store current rotation
		private float _Pitch = 0.0f;
		private float _Yaw = 0.0f;
		
		private Vector2I _Acceleration = new Vector2I(0, 0);
		
		private PanelContainer _ToolbarPanelContainer;
		private SubViewportContainer _ViewportContainer;
		private SubViewport _Viewport;
		private Node3D _CameraContainer;
		private Camera3D _Camera;
		private MarginContainer _MarginContainer;
		private bool _ListensForInput = false;
		private bool _PreviewEnvironment = false;

		/// <summary>
		/// Called when the node is added to the scene tree.
		/// </summary>
		public override void _EnterTree()
		{
			Name = "GroupMainScreen";
			Visible = false;

			SizeFlagsHorizontal = SizeFlags.ExpandFill;
			SizeFlagsVertical = SizeFlags.ExpandFill;

			if (
				false == HasGroup()
			)
			{
				_MarginContainer = new();

				_MarginContainer.AddThemeConstantOverride("margin_left", 25);
				_MarginContainer.AddThemeConstantOverride("margin_right", 25);
				_MarginContainer.AddThemeConstantOverride("margin_top", 10);
				_MarginContainer.AddThemeConstantOverride("margin_bottom", 0);

				Label label = new()
				{
					Text = "You first have to select a group before you can get a preview of it",
					ThemeTypeVariation = "HeaderLarge",
				};

				_MarginContainer.AddChild(label);
				AddChild(_MarginContainer);
				return;
			}

			_InitializeToolbar();
			_InitializePreview();
		}

		/// <summary>
		/// Process method called every frame.
		/// </summary>
		/// <param name="delta">The time in seconds since the last frame.</param>
		public override void _Process(double delta)
		{
			if( false == IsInstanceValid(this) ) 
			{
				return;
			}
			
			// Check if a group has been set
			// If so, render a representation of that group.
			if (IsInstanceValid(_Viewport) || false == HasGroup())
			{
				if (false == HasGroup() && IsInstanceValid(_ViewportContainer))
				{
					RemoveChild(_ToolbarPanelContainer);
					_ToolbarPanelContainer.QueueFree();

					RemoveChild(_ViewportContainer);
					_ViewportContainer.QueueFree();

					_MarginContainer = new();

					_MarginContainer.AddThemeConstantOverride("margin_left", 25);
					_MarginContainer.AddThemeConstantOverride("margin_right", 25);
					_MarginContainer.AddThemeConstantOverride("margin_top", 10);
					_MarginContainer.AddThemeConstantOverride("margin_bottom", 0);

					Label label = new()
					{
						Text = "You first have to select a group before you can get a preview of it",
						ThemeTypeVariation = "HeaderLarge",
					};

					_MarginContainer.AddChild(label);
					AddChild(_MarginContainer);
 
					return;
				}

				return;
			}

			if( IsInstanceValid(_MarginContainer) ) 
			{
				RemoveChild(_MarginContainer);
				_MarginContainer.QueueFree();
				
				_InitializeToolbar();
				_InitializePreview();
			}

			base._Process(delta);
		}

		/// <summary>
		/// Updates the group preview.
		/// </summary>
		public void Update()
		{
			Remove3DPreview(_Viewport);
			Render3DPreview(_Viewport);
		}

		/// <summary>
		/// Initializes the toolbar UI elements.
		/// </summary>
		private void _InitializeToolbar()
		{
			_ToolbarPanelContainer = new()
			{
				Name = "ToolbarPanelContainer",
				CustomMinimumSize = new Vector2(0, 20),
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
			};

			HBoxContainer ToolbarContainer = new()
			{
				Name = "ToolbarContainer",
			};

			Label label = new()
			{
				Name = "ToolbarTitle",
				Text = "Toolbar"
			};

			Button PreviewToggler = new()
			{
				ToggleMode = true,
				Name = "PreviewEnvironment",
				Text = "Preview Environment",
				MouseDefaultCursorShape = CursorShape.PointingHand,

			};

			PreviewToggler.Connect(Button.SignalName.Pressed, Callable.From(() => { PreviewEnvironment = !PreviewEnvironment; }));

			ToolbarContainer.AddChild(label);
			ToolbarContainer.AddChild(PreviewToggler);
			_ToolbarPanelContainer.AddChild(ToolbarContainer);
			AddChild(_ToolbarPanelContainer);
		}

		/// <summary>
		/// Initializes the environment preview if needed.
		/// </summary>
		/// <param name="Viewport">The viewport to render the environment.</param>
		/// <returns>True if the environment preview is initialized, false otherwise.</returns>
		private async Task<bool> _MaybeInitializeEnvironment(SubViewport Viewport)
		{
			if (PreviewEnvironment)
			{
				PlaneMesh mesh = new()
				{
					Size = new Vector2(200, 200),
					SubdivideDepth = 15,
					SubdivideWidth = 15,
				};

				Shader shader = GD.Load<Shader>("res://addons/assetsnap/shaders/simple-preview-terrain.gdshader");

				ShaderMaterial material = new()
				{
					Shader = shader,
				};

				NoiseTexture2D noiseTexture = new NoiseTexture2D()
				{
					Width = 25,
					Height = 25,
					Noise = new FastNoiseLite()
					{
						Seed = 4,
						NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin,
						Frequency = 0.99f,
					},
					GenerateMipmaps = true,
					Normalize = true,
				};

				await ToSignal(noiseTexture, NoiseTexture2D.SignalName.Changed);

				material.SetShaderParameter("noise", noiseTexture);

				material.SetShaderParameter("grass_texture", GD.Load<Texture2D>("res://addons/assetsnap/assets/materials/grass.jpg"));
				// material.SetShaderParameter("grass_texture", GD.Load<Texture2D>("res://addons/assetsnap/assets/materials/grass_roughness.jpg"));

				material.SetShaderParameter("stone_texture", GD.Load<Texture2D>("res://addons/assetsnap/assets/materials/stone.jpg"));
				material.SetShaderParameter("stone_roughness", GD.Load<Texture2D>("res://addons/assetsnap/assets/materials/stone_roughness.jpg"));

				material.SetShaderParameter("mountain_texture", GD.Load<Texture2D>("res://addons/assetsnap/assets/materials/mountain.jpg"));
				material.SetShaderParameter("mountain_roughness", GD.Load<Texture2D>("res://addons/assetsnap/assets/materials/mountain_roughness.jpg"));

				material.SetShaderParameter("snow_texture", GD.Load<Texture2D>("res://addons/assetsnap/assets/materials/snow.jpg"));
				material.SetShaderParameter("snow_roughness", GD.Load<Texture2D>("res://addons/assetsnap/assets/materials/snow_roughness.jpg"));

				MeshInstance3D meshInstance3D = new()
				{
					Name = "PreviewPlane",
					Mesh = mesh,
					MaterialOverride = material,
				};

				Viewport.AddChild(meshInstance3D);

				return true;
			}
			else
			{
				// Check if environment is initialized, if so remove it.
				for (int i = 0; i < Viewport.GetChildCount(); i++)
				{
					if (Viewport.GetChild(i).Name == "PreviewPlane")
					{
						Viewport.RemoveChild(Viewport.GetChild(i));
						Viewport.GetChild(i).QueueFree();
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Initializes the 3D preview.
		/// </summary>
		private async void _InitializePreview()
		{
			// Create a Viewport to render the 3D preview
			_ViewportContainer = new SubViewportContainer()
			{
				Stretch = true,
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				SizeFlagsVertical = SizeFlags.ExpandFill,
			};

			_Viewport = new SubViewport()
			{
				Name = "PreviewViewport",
				Size = new Vector2I(0, 0),
				RenderTargetUpdateMode = SubViewport.UpdateMode.Always,
			};

			World3D world = new World3D();

			Environment environment = new()
			{
				BackgroundMode = Environment.BGMode.Sky,
			};

			ProceduralSkyMaterial skyMaterial = new();

			Sky sky = new()
			{
				SkyMaterial = skyMaterial,
				RadianceSize = Sky.RadianceSizeEnum.Size256
			};

			// Set the procedural sky parameters
			environment.Sky = sky;

			// Set the ambient light
			environment.AmbientLightSource = Environment.AmbientSource.Sky;
			environment.AmbientLightEnergy = 1;

			world.Environment = environment;

			_Viewport.World3D = world;

			_CameraContainer = new();
			_Camera = new();

			Transform3D cameraTransform = new(Basis.Identity, Vector3.Zero);
			cameraTransform.Origin.Y = 1f;
			cameraTransform.Origin.Z = 5f;
			_Camera.Transform = cameraTransform;

			Vector3 cameraRot = _Camera.RotationDegrees;

			_Camera.RotationDegrees = cameraRot;

			DirectionalLight3D directionalLight = new DirectionalLight3D()
			{
				Name = "PreviewLight",
				Position = new Vector3(0, 100, 0),
				RotationDegrees = new Vector3(0.5f * 360 + 90, 0, 0),
				LightColor = new Color(1.0f, 0.98f, 0.93f),
				LightEnergy = 1.0f,
				ShadowEnabled = true,
			};

			_Viewport.AddChild(directionalLight);

			await _MaybeInitializeEnvironment(_Viewport);

			// Render the 3D preview onto the Viewport's texture
			Render3DPreview(_Viewport);

			// Add the Viewport to your main screen container
			_CameraContainer.AddChild(_Camera);
			_Viewport.AddChild(_CameraContainer);
			_ViewportContainer.AddChild(_Viewport);
			AddChild(_ViewportContainer);

			_ViewportContainer.Connect(Control.SignalName.GuiInput, Callable.From((InputEvent Event) => { _OnMaybeListenToInput(Event); }));
		}

		/// <summary>
		/// Handles user input events.
		/// </summary>
		/// <param name="@event">The input event.</param>
		public override void _Input(InputEvent @event)
		{
			if (ListensForInput)
			{
				// Check for movement on WSAD or arrow keys
				if (@event is InputEventKey inputEventKey)
				{
					// Move forward
					if ((inputEventKey.Keycode == Key.W || inputEventKey.Keycode == Key.Up) && true == inputEventKey.Pressed)
					{
						_Acceleration.Y = 1;
					}

					// Move backwards
					if ((inputEventKey.Keycode == Key.S || inputEventKey.Keycode == Key.Down) && true == inputEventKey.Pressed)
					{
						_Acceleration.Y = -1;
					}

					if ((inputEventKey.Keycode == Key.S || inputEventKey.Keycode == Key.Down || inputEventKey.Keycode == Key.W || inputEventKey.Keycode == Key.Up) && false == inputEventKey.Pressed)
					{
						_Acceleration.Y = 0;
					}

					// Move to the left
					if ((inputEventKey.Keycode == Key.A || inputEventKey.Keycode == Key.Left) && true == inputEventKey.Pressed)
					{
						_Acceleration.X = -1;
					}

					// Move to the right
					if ((inputEventKey.Keycode == Key.D || inputEventKey.Keycode == Key.Right) && true == inputEventKey.Pressed)
					{
						_Acceleration.X = 1;
					}

					if ((inputEventKey.Keycode == Key.A || inputEventKey.Keycode == Key.Left || inputEventKey.Keycode == Key.D || inputEventKey.Keycode == Key.Right) && false == inputEventKey.Pressed)
					{
						_Acceleration.X = 0;
					}

					// Go upwards
					if (inputEventKey.Keycode == Key.Space && true == inputEventKey.Pressed)
					{
						_Levitate = 1;
					}
					else if (inputEventKey.Keycode == Key.Space && false == inputEventKey.Pressed)
					{
						_Levitate = 0;
					}

					// Go downwards
					if (inputEventKey.Keycode == Key.Alt && true == inputEventKey.Pressed)
					{
						_Levitate = -1;
					}
					else if (inputEventKey.Keycode == Key.Alt && false == inputEventKey.Pressed)
					{
						_Levitate = 0;
					}

					if (inputEventKey.Keycode == Key.Q)
					{
						Transform3D transform = _Camera.Transform;
						transform.Origin = Vector3.Zero;
						_Camera.Transform = transform;
					}
				}

				if (@event is InputEventMouseMotion eventMouseMotion)
				{
					// Ability to turn around with the mouse as long as we listens.
					// Get the mouse motion since the last frame
					Vector2 mouseMotion = eventMouseMotion.Relative;
					// Update yaw (horizontal rotation) based on mouse X movement
					_Yaw -= mouseMotion.X * _MouseSensitivity;

					// Update pitch (vertical rotation) based on mouse Y movement
					_Pitch -= mouseMotion.Y * _MouseSensitivity;

					// Clamp the pitch to prevent flipping
					_Pitch = Mathf.Clamp(_Pitch, -Mathf.Pi / 0.1f, Mathf.Pi / 0.1f);

					// Set the new rotation of the camera
					_Camera.RotationDegrees = new Vector3(_Pitch, _Yaw, 0);
				}

				if (@event is InputEventMouseButton eventMouseButton)
				{
					// Ability to turn up movement speed
					if (eventMouseButton.ButtonIndex == MouseButton.WheelUp && false == eventMouseButton.Pressed)
					{
						_MoveSpeedRate += 1;
					}
					else if (eventMouseButton.ButtonIndex == MouseButton.WheelDown && false == eventMouseButton.Pressed)
					{
						_MoveSpeedRate -= 1;
					}
				}
			}

			base._Input(@event);
		}

		/// <summary>
		/// Physics process method called every physics frame.
		/// </summary>
		/// <param name="delta">The time in seconds since the last physics frame.</param>
		public override void _PhysicsProcess(double delta)
		{
			// Check if we listens to input
			if (ListensForInput)
			{
				// Get the forward and right vectors from the camera's orientation
				Vector3 forward = -_Camera.Basis.Z.Normalized();
				Vector3 right = _Camera.Basis.X.Normalized();

				Transform3D transform = _Camera.Transform;
				// Vector3 MovementVector = Vector3.Zero;
				Vector3 MovementVector = new(
					transform.Origin.X,
					transform.Origin.Y,
					transform.Origin.Z
				);

				// Move and rotate the camera based on the input received
				if (_Acceleration.X != 0 || _Acceleration.Y != 0)
				{
					// Calculate the movement vector relative to the camera's orientation
					MovementVector += right * _Acceleration.X;
					MovementVector += forward * _Acceleration.Y;
				}

				if (_Levitate != 0)
				{
					// We should move up or down on the Y axis
					transform.Origin.Y += _Levitate * (float)delta;
				}

				transform.Origin = transform.Origin.Lerp(MovementVector, _MoveSpeedRate * (float)delta);
				_Camera.Transform = transform;

			}
			base._PhysicsProcess(delta);
		}

		/// <summary>
		/// Handles input events to start or stop listening for input.
		/// </summary>
		/// <param name="Event">The input event.</param>
		private void _OnMaybeListenToInput(InputEvent Event)
		{
			if (Event is InputEventMouseButton eventMouseButton)
			{
				if (eventMouseButton.ButtonIndex == MouseButton.Right && true == eventMouseButton.Pressed)
				{
					ListensForInput = true;
					Input.Singleton.MouseMode = Input.MouseModeEnum.Captured;
					FocusMode = FocusModeEnum.All;
					GrabFocus();
				}
				else if (eventMouseButton.ButtonIndex == MouseButton.Right && false == eventMouseButton.Pressed)
				{
					ListensForInput = false;
					FocusMode = FocusModeEnum.None;
					Input.Singleton.MouseMode = Input.MouseModeEnum.Visible;

					// Reset movement when we tab out of listening
					_Acceleration.X = 0;
					_Acceleration.Y = 0;
					_Levitate = 0.0f;

				}
			}
		}

		/// <summary>
		/// Renders the 3D preview of the group.
		/// </summary>
		/// <param name="Viewport">The viewport to render the preview.</param>
		private void Render3DPreview(SubViewport Viewport)
		{
			Front.Nodes.GroupResource Group = GlobalExplorer.GetInstance().GroupBuilder._Editor.Group;
			for (int i = 0; i < Group._Paths.Count; i++)
			{
				string MeshPath = Group._Paths[i];
				Vector3 MeshOrigin = Group._Origins[i];
				Vector3 MeshScale = Group._Scales[i];
				Vector3 MeshRotation = Group._Rotations[i];

				GodotObject MeshObject = GD.Load(MeshPath);

				Transform3D transform = new(Basis.Identity, Vector3.Zero);
				transform.Origin = MeshOrigin;

				if (MeshObject is Mesh _mesh)
				{
					MeshInstance3D meshinstance3d = new()
					{
						Mesh = _mesh,
						Transform = transform,
						Scale = MeshScale,
						RotationDegrees = MeshRotation,
					};

					meshinstance3d.SetMeta("IsPreview", true);
					Viewport.AddChild(meshinstance3d);
				}

				if (MeshObject is PackedScene _scene)
				{
					// Scene instance
					Node3D node = _scene.Instantiate() as Node3D;
					node.Transform = transform;
					node.Scale = MeshScale;
					node.RotationDegrees = MeshRotation;

					node.SetMeta("IsPreview", true);
					Viewport.AddChild(node);
				}
			}
		}

		/// <summary>
		/// Removes the 3D preview from the viewport.
		/// </summary>
		/// <param name="Viewport">The viewport containing the preview.</param>
		private void Remove3DPreview(SubViewport Viewport)
		{
			foreach (Node previewChild in Viewport.GetChildren())
			{
				if (previewChild is MeshInstance3D meshInstance3D && meshInstance3D.HasMeta("IsPreview"))
				{
					Viewport.RemoveChild(meshInstance3D);
					meshInstance3D.QueueFree();
				}
			}
		}

		/// <summary>
		/// Updates the texture.
		/// </summary>
		private void UpdateTexture()
		{
			// // Capture the rendered texture from the Viewport
			// Image textureImage = viewport.GetTexture().GetData();

			// // Set the texture to the TextureRect
			// ImageTexture imageTexture = new ImageTexture();
			// imageTexture.CreateFromImage(textureImage);
			// textureRect.Texture = imageTexture;
		}

		/// <summary>
		/// Checks if a group is selected.
		/// </summary>
		/// <returns>True if a group is selected, false otherwise.</returns>
		public bool HasGroup()
		{
			GlobalExplorer explorer = GlobalExplorer.GetInstance();

			return
				null != explorer &&
				null != GlobalExplorer.GetInstance().GroupBuilder &&
				IsInstanceValid(GlobalExplorer.GetInstance().GroupBuilder._Editor) &&
				IsInstanceValid(GlobalExplorer.GetInstance().GroupBuilder._Editor.Group);
		}

		/// <summary>
		/// Called when the node is about to be removed from the scene tree.
		/// </summary>
		public override void _ExitTree()
		{
			// Clean up when the plugin is deactivated 
			if (
				null != _ViewportContainer &&
				IsInstanceValid(_ViewportContainer) &&
				_ViewportContainer.IsConnected(Control.SignalName.GuiInput, Callable.From((InputEvent Event) => { _OnMaybeListenToInput(Event); }))
			)
			{
				_ViewportContainer.Disconnect(Control.SignalName.GuiInput, Callable.From((InputEvent Event) => { _OnMaybeListenToInput(Event); }));
			}
		}
	}
}

#endif