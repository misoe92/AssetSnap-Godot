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
namespace AssetSnap.Front.Nodes
{
using System;
using Godot;

	[Tool] 
	public partial class DropdownButton : Button
	{
		private static readonly Texture2D OpenIcon = GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/chevron-down.svg");	
		private static readonly Texture2D CloseIcon = GD.Load<Texture2D>("res://addons/assetsnap/assets/icons/chevron-up.svg");
		
		[Export]
		public Godot.Collections.Dictionary<string, int> Margin = new();
	
		public override void _EnterTree()
		{
			Connect(SignalName.Pressed, Callable.From( () => { _OnToggleVisibility(); } ) );
			
			base._EnterTree();
		}
		
		private void _OnToggleVisibility()
		{
			VBoxContainer InnerContainer = GetParent() as VBoxContainer;
			MarginContainer container = InnerContainer.GetChild(1) as MarginContainer;
			PanelContainer panel = InnerContainer.GetParent() as PanelContainer;
			MarginContainer _MarginContainer = panel.GetParent().GetParent().GetParent().GetParent().GetParent() as MarginContainer;

			container.Visible = !container.Visible;
			
			if( container.Visible ) 
			{
				Icon = CloseIcon;				
				_MarginContainer.AddThemeConstantOverride("margin_bottom", 10);
			}
			else 
			{
				Icon = OpenIcon;
				_MarginContainer.AddThemeConstantOverride("margin_bottom", Margin["bottom"]);
			}
		}
		
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