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
namespace AssetSnap
{
	using AssetSnap.Abstracts;
	using AssetSnap.Front.Nodes;
	using Godot;

	[Tool]
	public partial class BaseExplorer : AbstractExplorerBase
	{
		/*
		** Counter that keeps check on our total delta time 
		*/
		public float DeltaTime
		{
			get => _DeltaTime;
			set 
			{
				_DeltaTime = value;	
			}
		}
				
		/*
		** The active library
		*/
		public Godot.Collections.Array<AsSelectList> SelectLists 
		{
			get => _SelectLists;
			set
			{
				_SelectLists = value;
			}
		}
				
		/*
		** Contains the current mouse input type
		*/
		public EventMouse CurrentMouseInput
		{
			get => _CurrentMouseInput;
			set 
			{
				_CurrentMouseInput = value;
			}	
		}
		
		/*
		** Allow scroll
		*/
		public ScrollState AllowScroll
		{
			get => _AllowScroll;
			set 
			{
				_AllowScroll = value;
			}	
		}
	}
}
#endif