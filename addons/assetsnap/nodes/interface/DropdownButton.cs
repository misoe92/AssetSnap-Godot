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

using Godot;

namespace AssetSnap.Front.Nodes
{
	/// <summary>
	/// Represents a dropdown button with toggleable visibility.
	/// </summary>
	[Tool] 
	public partial class DropdownButton : Button
	{
		/// <summary>
		/// The margins of the dropdown button.
		/// </summary>
		[Export]
		public Godot.Collections.Dictionary<string, int> Margin = new();
		
		private static readonly Texture2D _OpenIcon = GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/chevron-down.svg");	
		private static readonly Texture2D _CloseIcon = GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/chevron-up.svg");
		
		/// <summary>
		/// Called when entering the scene tree.
		/// </summary>
		public override void _EnterTree()
		{
			Connect(SignalName.Pressed, Callable.From( () => { _OnToggleVisibility(); } ) );
			
			base._EnterTree();
		}
		
		/// <summary>
		/// Toggles the visibility of the dropdown container.
		/// </summary>
		private void _OnToggleVisibility()
		{
			VBoxContainer InnerContainer = GetParent() as VBoxContainer;
			MarginContainer container = InnerContainer.GetChild(1) as MarginContainer;
			PanelContainer panel = InnerContainer.GetParent() as PanelContainer;
			MarginContainer _MarginContainer = panel.GetParent().GetParent().GetParent().GetParent().GetParent() as MarginContainer;

			container.Visible = !container.Visible;
			
			if( container.Visible ) 
			{
				Icon = _CloseIcon;				
				_MarginContainer.AddThemeConstantOverride("margin_bottom", 10);
			}
			else 
			{
				Icon = _OpenIcon;
				_MarginContainer.AddThemeConstantOverride("margin_bottom", Margin["bottom"]);
			}
		}
		
		/// <summary>
		/// Called when exiting the scene tree.
		/// </summary>
		public override void _ExitTree()
		{
			if( IsConnected( SignalName.Pressed, Callable.From( () => { _OnToggleVisibility(); } ) ) ) 
			{
				Disconnect(SignalName.Pressed, Callable.From( () => { _OnToggleVisibility(); } ) );
			}
			
			base._ExitTree();
		}
	}
}

#endif