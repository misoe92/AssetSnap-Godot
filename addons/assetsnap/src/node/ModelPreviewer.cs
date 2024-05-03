namespace AssetSnap.Nodes
{
	using System;
	using System.Threading.Tasks;
	using AssetSnap.Explorer;
	using AssetSnap.Static;
	using Godot;

	[Tool]
	class ModelPreviewer
	{
		public static ModelPreviewer Singleton
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = new();
				}

				return _Instance;
			}
		}


		private static ModelPreviewer _Instance;

		public Vector2I Size = new Vector2I(SettingsStatic.PreviewImageSize(), SettingsStatic.PreviewImageSize());

		public Godot.Collections.Array<string> Queue = new();
		public Godot.Collections.Array<string> QueueSignals = new();
		public Godot.Collections.Array<string> QueueLibrary = new();
		public Godot.Collections.Array<TextureRect> QueueTextures = new();

		private SubViewport _viewport;
		private Camera3D Camera;

		/*
		** Prepare conditions for preview image extraction
		*/
		public void Enter()
		{
			Godot.Environment environment = new()
			{
				BackgroundMode = Godot.Environment.BGMode.Sky,
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
			environment.AmbientLightSource = Godot.Environment.AmbientSource.Sky;
			environment.AmbientLightEnergy = 1;
			
			World3D world = new()
			{
				Environment = environment
			};

			_viewport = new()
			{
				Size = Size,
				World3D = world,

				// DebugDraw = Viewport.DebugDrawEnum.Disabled,
				RenderTargetUpdateMode = SubViewport.UpdateMode.Always,
				// ProcessMode = Node.ProcessModeEnum.Disabled,
				GuiDisableInput = true,
				TransparentBg = true,
				PhysicsObjectPicking = false,

				CanvasItemDefaultTextureFilter = Viewport.DefaultCanvasItemTextureFilter.NearestWithMipmaps,
				CanvasItemDefaultTextureRepeat = Viewport.DefaultCanvasItemTextureRepeat.Disabled,

				Msaa3D = ProjectSettings.GetSetting("rendering/anti_aliasing/quality/msaa_3d").As<Viewport.Msaa>(),
				FsrSharpness = ProjectSettings.GetSetting("rendering/scaling_3d/fsr_sharpness").As<float>(),
				PositionalShadowAtlas16Bits = ProjectSettings.GetSetting("rendering/lights_and_shadows/positional_shadow/atlas_16_bits").As<bool>(),
				PositionalShadowAtlasQuad0 = ProjectSettings.GetSetting("rendering/lights_and_shadows/positional_shadow/atlas_quadrant_0_subdiv").As<Viewport.PositionalShadowAtlasQuadrantSubdiv>(),
				PositionalShadowAtlasQuad1 = ProjectSettings.GetSetting("rendering/lights_and_shadows/positional_shadow/atlas_quadrant_1_subdiv").As<Viewport.PositionalShadowAtlasQuadrantSubdiv>(),
				PositionalShadowAtlasQuad2 = ProjectSettings.GetSetting("rendering/lights_and_shadows/positional_shadow/atlas_quadrant_2_subdiv").As<Viewport.PositionalShadowAtlasQuadrantSubdiv>(),
				PositionalShadowAtlasQuad3 = ProjectSettings.GetSetting("rendering/lights_and_shadows/positional_shadow/atlas_quadrant_3_subdiv").As<Viewport.PositionalShadowAtlasQuadrantSubdiv>(),
				PositionalShadowAtlasSize = ProjectSettings.GetSetting("rendering/lights_and_shadows/positional_shadow/atlas_size").As<int>(),

				Scaling3DMode = ProjectSettings.GetSetting("rendering/scaling_3d/mode").As<Viewport.Scaling3DModeEnum>(),
				Scaling3DScale = ProjectSettings.GetSetting("rendering/scaling_3d/scale").As<float>(),

				ScreenSpaceAA = ProjectSettings.GetSetting("rendering/anti_aliasing/quality/screen_space_aa").As<Viewport.ScreenSpaceAAEnum>(),
				TextureMipmapBias = ProjectSettings.GetSetting("rendering/textures/default_filters/texture_mipmap_bias").As<float>(),
			};

			Plugin.Singleton
				.GetInternalContainer()
				.AddChild(_viewport, true);

			// Setup Camera + Scene Light
			DirectionalLight3D SceneLight = new DirectionalLight3D()
			{
				DirectionalShadowMode = DirectionalLight3D.ShadowMode.Parallel4Splits,
				LightBakeMode = Light3D.BakeMode.Static,
				ShadowEnabled = true,
			};

			SceneLight.Basis *= new Basis(Vector3.Up, Mathf.DegToRad(45.0f));
			SceneLight.Basis *= new Basis(Vector3.Left, Mathf.DegToRad(65.0f));

			_viewport.AddChild(SceneLight);

			Camera = new Camera3D()
			{
				Current = false,
				Fov = 22.5f,
			};

			_viewport.AddChild(Camera);
		}

		public static void ClearPreviewImages(string AbsolutePath)
		{
			if (DirAccess.DirExistsAbsolute(AbsolutePath))
			{
				DirAccess folder = DirAccess.Open(AbsolutePath);
				folder.IncludeHidden = false;
				folder.IncludeNavigational = false;
				folder.ListDirBegin();

				while (true)
				{
					string fileName = folder.GetNext();
					if (fileName == "")
					{
						break;
					}

					string filePath = $"{AbsolutePath}/{fileName}";

					if (DirAccess.DirExistsAbsolute(filePath))
					{
						// Recursively clear subdirectories
						ClearPreviewImages(filePath);
						DirAccess.RemoveAbsolute(filePath);
					}
					else
					{
						DirAccess.RemoveAbsolute(filePath);
					}
				}

				folder.ListDirEnd();
			}
			else
			{
				GD.PushWarning("Path not found");
			}
		}

		public void AddToQueue(string FilePath, TextureRect _TextureRect, string LibraryName)
		{
			Queue.Add(FilePath);
			QueueTextures.Add(_TextureRect);
			QueueLibrary.Add(LibraryName);
		}

		/*
		** Cleanup after usage
		*/
		public void Leave()
		{
			Plugin.Singleton
				.GetInternalContainer()
				.RemoveChild(_viewport);

			_viewport.QueueFree();
			_viewport = null;
		}

		public int Count()
		{
			return Queue.Count;
		}

		public async void GeneratePreviews()
		{
			int MaxCount = Queue.Count;

			for (int i = MaxCount - 1; i >= 0; i--)
			{
				string path = Queue[i];
				TextureRect _TextureRect = QueueTextures[i];
				string LibraryName = QueueLibrary[i];

				Texture2D image = await GeneratePreviewTexture(path, Size.X, Size.Y, LibraryName);
				if (_TextureRect is AsModelViewerRect modelViewerRect)
				{
					modelViewerRect._MeshPreviewReady(path, image, image, modelViewerRect);
				}
				else
				{
					GD.PushWarning("Invalid ModelViewerRect");
				}

				Queue.RemoveAt(i);
			}
		}

		public async Task<Texture2D> GeneratePreviewTexture(string fbxPath, int width, int height, string LibraryName)
		{
			// Generate the preview image
			Image image = await GeneratePreviewImage(fbxPath, width, height, LibraryName);

			// Create a new ImageTexture from the Image
			ImageTexture imageTexture = ImageTexture.CreateFromImage(image);

			return imageTexture;
		}

		public async Task<Image> GeneratePreviewImage(string path, int width, int height, string LibraryName)
		{
			try 
			{
				string[] pathSplit = path.Split("/");
				string FilenameWithExt = pathSplit[pathSplit.Length - 1];
				string FilenameWithoutExt = FilenameWithExt.Split(".")[0];

				// Load the FBX scene
				Node3D node = null;
				if (path.Contains(".fbx") || path.Contains(".gltf") || path.Contains(".glb"))
				{
					PackedScene fbxScene = GD.Load<PackedScene>(path);
					node = fbxScene.Instantiate<Node3D>();
				}
				else if (path.Contains(".obj"))
				{
					node = new()
					{
						Name = "ObjectPreviewer"
					};

					Mesh mesh = GD.Load<Mesh>(path);
					MeshInstance3D meshinstance3D = new()
					{
						Mesh = mesh,
					};

					node.AddChild(meshinstance3D);
				}

				if (node == null)
				{
					throw new Exception("No model was found");
				}

				// Add the model to the viewport
				_viewport.AddChild(node);

				// Correct the camera focus
				_AdjustCameraFocus(node);

				await Plugin.Singleton.ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
				// Take image with Camera
				Image preview = _MakePreview();

				// Save camera image
				_SavePreview(FilenameWithoutExt, preview, "", LibraryName);

				// -90
				node.RotationDegrees = new Vector3(0, -90, 0);
				await Plugin.Singleton.ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);

				preview = _MakePreview();
				// Save camera image
				_SavePreview(FilenameWithoutExt, preview, "minus-90", LibraryName);

				// -180
				node.RotationDegrees = new Vector3(0, -180, 0);
				await Plugin.Singleton.ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);

				preview = _MakePreview();
				// Save camera image
				_SavePreview(FilenameWithoutExt, preview, "minus-180", LibraryName);

				// 90
				node.RotationDegrees = new Vector3(0, 90, 0);
				await Plugin.Singleton.ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);

				preview = _MakePreview();
				// Save camera image
				_SavePreview(FilenameWithoutExt, preview, "90", LibraryName);

				// 180
				node.RotationDegrees = new Vector3(0, 180, 0);
				await Plugin.Singleton.ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);

				preview = _MakePreview();
				// Save camera image
				_SavePreview(FilenameWithoutExt, preview, "180", LibraryName);

				Aabb aabb = NodeUtils.CalculateNodeAabb(node);
				ExplorerUtils.Get().Settings.AddModelSizeToCache(FilenameWithoutExt, aabb.Size);
				_viewport.RemoveChild(node);
				node.QueueFree();

				// Return saved image
				return preview;
			}
			catch( Exception e ) 
			{
				GD.PushError(e.Message);
				return new Image();	
			}
		}

		private void _AdjustCameraFocus(Node node)
		{
			Transform3D transform = new Transform3D(Basis.Identity, Vector3.Zero);
			transform.Basis *= new Basis(Vector3.Up, Mathf.DegToRad(40.0f));
			transform.Basis *= new Basis(Vector3.Left, Mathf.DegToRad(22.5f));

			Aabb aabb = NodeUtils.CalculateNodeAabb(node);
			float distance = aabb.GetLongestAxisSize() / Mathf.Tan(Mathf.DegToRad(Camera.Fov) * 0.5f);

			transform.Origin = transform * (Vector3.Back * distance) + aabb.GetCenter();
			Camera.GlobalTransform = transform.Orthonormalized();
		}

		private Image _MakePreview()
		{
			// Set the viewport size to the specified Size
			// _viewport.Size = Size;

			// Render the viewport to the Image
			Image preview = _viewport.GetTexture().GetImage();

			// Copy the ImageTexture data to the preview Image
			// preview.BlitRect(preview, new Rect2I(0, 0, Size.X, Size.Y), new Vector2I(0, 0));

			return preview;
		}

		private void _SavePreview(string name, Image preview, string suffix = "", string LibraryName = "")
		{
			if (false == DirAccess.DirExistsAbsolute("res://assetsnap"))
			{
				DirAccess.MakeDirAbsolute("res://assetsnap");
			}

			if (false == DirAccess.DirExistsAbsolute("res://assetsnap/previews"))
			{
				DirAccess.MakeDirAbsolute("res://assetsnap/previews");
			}

			if (false == DirAccess.DirExistsAbsolute("res://assetsnap/previews/" + LibraryName))
			{
				DirAccess.MakeDirAbsolute("res://assetsnap/previews/" + LibraryName);
			}

			if (false == DirAccess.DirExistsAbsolute("res://assetsnap/previews/" + LibraryName + "/" + name))
			{
				DirAccess.MakeDirAbsolute("res://assetsnap/previews/" + LibraryName + "/" + name);
			}

			if (true == FileAccess.FileExists("res://assetsnap/previews/" + LibraryName + "/" + name + "/default" + (suffix != "" ? "-" + suffix : "") + ".png"))
			{
				return;
			}

			Error result = preview.SavePng("assetsnap/previews/" + LibraryName + "/" + name + "/default" + (suffix != "" ? "-" + suffix : "") + ".png");

			if (result != Error.Ok)
			{
				GD.Print("Something went wrong: ", name);
				return;
			}
		}
	}
}
