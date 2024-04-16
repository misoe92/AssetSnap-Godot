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
namespace AssetSnapGroupPreview
{
	using Godot;

	[Tool]
	public partial class AssetSnapGroupPreview : EditorPlugin
	{
		[Signal]
		public delegate void SettingKeyChangedEventHandler();
	
		/** Internal data **/
		private readonly string _Version = "0.1";
		private readonly string _Name = "AssetSnapGroupPreview";

		private static AssetSnapGroupPreview _Instance;
		private AssetSnap.GroupBuilder.MainScreen mainScreenContainer;

		
		public static AssetSnapGroupPreview GetInstance()
		{
			return _Instance;
		}
		
		/*
		** Initialization of our plugin
		** 
		** @return void
		*/ 
		public override void _EnterTree() 
		{
			AssetSnapGroupPreview._Instance = this; 

			if( null == AssetSnap.GlobalExplorer.GetInstance() ) 
			{
				GD.PushError("No explorer is available");
				return;
			}
			 
			mainScreenContainer = new AssetSnap.GroupBuilder.MainScreen();
			EditorInterface.Singleton.GetEditorMainScreen().AddChild(mainScreenContainer);

			AssetSnap.GlobalExplorer.GetInstance().GroupMainScreen = mainScreenContainer;
		}
		
		public override void _MakeVisible(bool visible)
		{
			if (mainScreenContainer != null)
			{
				mainScreenContainer.Visible = visible;
			}
		}

		public override string _GetPluginName()
		{
			return "Group Preview";
		}
		
		public override bool _HasMainScreen()
		{
			return true;
		}

		public override Texture2D _GetPluginIcon()
		{
			// Must return some kind of Texture for the icon.
			return EditorInterface.Singleton.GetEditorTheme().GetIcon("Node", "EditorIcons");
		}
		
		/*
		** Fetches the current plugin version
		**
		** @return double
		*/
		public string GetVersion()
		{
			return _Version;
		}
		
		/*
		** Fetches the current plugin version in a string
		**
		** @return string
		*/
		public string GetVersionString()
		{
			return _Version.ToString().Split(",").Join(".");
		}
	}
}
#endif
