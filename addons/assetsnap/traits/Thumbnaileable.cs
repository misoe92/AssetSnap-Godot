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
using System.Collections.Generic;
using AssetSnap.Nodes;
using AssetSnap.Trait;
using Godot;

namespace AssetSnap.Component
{
	[Tool]
	public partial class Thumbnaileable : ContainerTrait
	{
		/*
		** Private
		*/
		private TextureRect.ExpandModeEnum ExpandMode = TextureRect.ExpandModeEnum.KeepSize;
		private TextureRect.StretchModeEnum StretchMode = TextureRect.StretchModeEnum.Keep;
		private string FilePath;

		
		/* The `public Thumbnaileable()` constructor in the C# code snippet is initializing the
		`Thumbnaileable` class by setting the `Name` property to "Thumbnaileable" and the `TypeString`
		property to the string representation of the class type. This constructor is called when an
		instance of the `Thumbnaileable` class is created, and it helps in setting initial values or
		configurations for the class properties. */
		public Thumbnaileable()
		{
			Name = "Thumbnaileable";
			TypeString = GetType().ToString();
		}

		/// <summary>
		/// This C# function instantiates a thumbnail preview element based on file paths and adds it to a
		/// container.
		/// </summary>
		/// <returns>
		/// An instance of the class that this method belongs to, which implements the `Thumbnaileable`
		/// interface.
		/// </returns>
		public override Thumbnaileable Instantiate()
		{
			base._Instantiate();
			base.Instantiate();

			AsModelViewerRect _TextureRect = new()
			{
				Name = TraitName + "-Preview",
				ExpandMode = ExpandMode,
				StretchMode = StretchMode,
				CustomMinimumSize = new Vector2I(62, 62)
			};

			List<string> folderList = new(FilePath.Split("/"));
			string FileName = folderList.ToArray()[folderList.Count - 1];
			string LibraryName = folderList.ToArray()[folderList.Count - 2];
			folderList.RemoveAt(folderList.Count - 1);
			string FolderPath = folderList.ToArray().Join("/");

			if (true == FileAccess.FileExists("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default.png"))
			{
				Texture2D image = GD.Load<Texture2D>("res://assetsnap/previews/" + LibraryName + "/" + FileName.Split(".")[0] + "/default.png");
				_TextureRect._MeshPreviewReady(FolderPath + "/" + FileName, image, image, _TextureRect);
			}
			else
			{
				ModelPreviewer.Singleton.AddToQueue(FolderPath + "/" + FileName, _TextureRect, LibraryName);
			}

			GetInnerContainer(0).AddChild(_TextureRect);

			Dependencies[TraitName + "_WorkingNode"] = _TextureRect;

			Plugin.Singleton.traitGlobal.AddInstance(Iteration, _TextureRect, OwnerName, TypeString, Dependencies);
			Plugin.Singleton.traitGlobal.AddName(Iteration, TraitName, OwnerName, TypeString);

			Reset();
			Iteration += 1;
			Dependencies = new();

			return this;
		}

		/// <summary>
		/// This C# function selects a thumbnail from an array by index and updates dependencies if needed.
		/// </summary>
		/// <param name="index">The `index` parameter is an integer value that represents the position of the
		/// thumbnail in the nodes array that you want to select.</param>
		/// <param name="debug">The `debug` parameter in the `Select` method is a boolean parameter that is
		/// used to specify whether debugging information should be output during the execution of the method.
		/// When `debug` is set to `true`, additional debugging information may be displayed or logged to help
		/// with troubleshooting or understanding the behavior of</param>
		/// <returns>
		/// The method `Select` is returning an object that implements the `Thumbnaileable` interface.
		/// </returns>
		public override Thumbnaileable Select(int index, bool debug = false)
		{
			base._Select(index);

			if (false != Dependencies.ContainsKey(TraitName + "_WorkingNode"))
			{
				Godot.Collections.Dictionary<string, Variant> dependencies = Plugin.Singleton.traitGlobal.GetDependencies(index, TypeString, OwnerName);
				Dependencies = dependencies;
			}

			return this;
		}

		/// <summary>
		/// This C# function selects a thumbnail from an array of nodes based on its name and sets it as a
		/// working node.
		/// </summary>
		/// <param name="name">The `name` parameter in the `SelectByName` method is a string that represents
		/// the name of the thumbnail you want to select from the nodes array.</param>
		/// <returns>
		/// The method `SelectByName` is returning the current instance of the class that contains this
		/// method, which is indicated by `return this;`.
		/// </returns>
		public override Thumbnaileable SelectByName(string name)
		{
			foreach (Container container in Nodes)
			{
				if (container.Name == name)
				{
					Dependencies[TraitName + "_WorkingNode"] = container;
					break;
				}
			}

			return this;
		}

		/// <summary>
		/// The function AddToContainer adds the currently chosen thumbnail to a specified container in C#.
		/// </summary>
		/// <param name="Node">In the provided code snippet, the `Node Container` parameter represents a node
		/// that serves as the container where the currently chosen thumbnail will be added. This method
		/// `AddToContainer` is responsible for adding the thumbnail to the specified container node.</param>
		/// <returns>
		/// The code snippet provided is a method named `AddToContainer` that adds the currently chosen
		/// thumbnail to a specified container.
		/// </returns>
		public void AddToContainer(Node Container)
		{
			if (false == Dependencies.ContainsKey(TraitName + "_MarginContainer"))
			{
				GD.PushError("Container was not found @ AddToContainer");
				GD.PushError("AddToContainer::Keys-> ", Dependencies.Keys);
				GD.PushError("AddToContainer::ADDTO-> ", TraitName + "_MarginContainer");
				return;
			}

			base._AddToContainer(Container, Dependencies[TraitName + "_MarginContainer"].As<MarginContainer>());
		}

		/// <summary>
		/// This C# function sets the name of the thumbnail and returns the object implementing the Thumbnaileable
		/// interface.
		/// </summary>
		/// <param name="text">The `text` parameter in the `SetName` method is a string that represents the name
		/// you want to set for the current thumbnail.</param>
		/// <returns>
		/// Buttonable
		/// </returns>
		public Thumbnaileable SetName(string text)
		{
			base._SetName(text);

			return this;
		}

		/// <summary>
		/// The SetFilePath function sets the file path of the current thumbnail and returns the
		/// Thumbnaileable object.
		/// </summary>
		/// <param name="path">The `path` parameter in the `SetFilePath` method is a string that represents
		/// the file path of the current thumbnail.</param>
		/// <returns>
		/// The method `SetFilePath` is returning an object of type `Thumbnaileable`.
		/// </returns>
		public Thumbnaileable SetFilePath(string path)
		{
			FilePath = path;

			return this;
		}

		/// <summary>
		/// This C# function sets the visibility state of the currently chosen thumbnail.
		/// </summary>
		/// <param name="state">The `state` parameter in the `SetVisible` method is a boolean value that
		/// determines the visibility state of the currently chosen thumbnail. If `state` is `true`, the
		/// thumbnail will be visible; if `state` is `false`, the thumbnail will be hidden.</param>
		/// <returns>
		/// The method `SetVisible` is returning an object of type `Thumbnaileable`.
		/// </returns>
		public override Thumbnaileable SetVisible(bool state)
		{
			base.SetVisible(state);

			return this;
		}

		/// <summary>
		/// The SetDimensions method in C# sets the dimensions of the current thumbnail and returns the object
		/// implementing the Thumbnaileable interface.
		/// </summary>
		/// <param name="width">The `width` parameter specifies the width of the thumbnail image.</param>
		/// <param name="height">The height parameter in the SetDimensions method is used to specify the
		/// height of the thumbnail image. It allows you to set the desired height for the thumbnail when
		/// generating or resizing it.</param>
		/// <returns>
		/// The method is returning an object of type Thumbnaileable.
		/// </returns>
		public override Thumbnaileable SetDimensions(int width, int height)
		{
			base.SetDimensions(width, height);

			return this;
		}

		/// <summary>
		/// The function SetContainerHorizontalSizeFlag in C# sets a horizontal size flag for a container and returns the object
		/// implementing the Thumbnaileable interface.
		/// </summary>
		/// <param name="flag">The `flag` parameter in the `SetContainerHorizontalSizeFlag` method is of type
		/// `Control.SizeFlags`. It is used to set the horizontal size flag for the container.</param>
		/// <returns>
		/// An instance of the current class, Thumbnaileable.
		/// </returns>
		public override Thumbnaileable SetContainerHorizontalSizeFlag(Control.SizeFlags flag)
		{
			base.SetContainerHorizontalSizeFlag(flag);

			return this;
		}

		/// <summary>
		/// This C# function sets the expand mode of a texture rect and returns a Thumbnaileable object.
		/// </summary>
		/// <param name="mode">The `mode` parameter in the `SetExpandMode` method is of type
		/// `TextureRect.ExpandModeEnum`. This parameter is used to set the expand mode of the texture
		/// rectangle. The `ExpandModeEnum` is an enumeration that likely contains different options for how
		/// the texture rect should expand.</param>
		/// <returns>
		/// The method returns an instance of the class that implements the Thumbnaileable interface.
		/// </returns>
		public Thumbnaileable SetExpandMode(TextureRect.ExpandModeEnum mode)
		{
			ExpandMode = mode;

			return this;
		}

		/// <summary>
		/// This C# function sets the stretch mode of a texture rect and returns a Thumbnaileable object.
		/// </summary>
		/// <param name="mode">The `mode` parameter in the `SetStretchMode` method is of type
		/// `TextureRect.StretchModeEnum`. This parameter is used to specify the stretch mode of the texture
		/// rect.</param>
		/// <returns>
		/// The method `SetStretchMode` is returning an object of type `Thumbnaileable`.
		/// </returns>
		public Thumbnaileable SetStretchMode(TextureRect.StretchModeEnum mode)
		{
			StretchMode = mode;

			return this;
		}

		/// <summary>
		/// This C# function sets the horizontal size flags for a control and returns a Thumbnaileable object.
		/// </summary>
		/// <param name="flag">The `flag` parameter is of type `Control.SizeFlags` and it controls the x-axis
		/// behavior of the control.</param>
		/// <returns>
		/// The method is returning an object of type Thumbnaileable.
		/// </returns>
		public override Thumbnaileable SetHorizontalSizeFlags(Control.SizeFlags flag)
		{
			base.SetHorizontalSizeFlags(flag);

			return this;
		}

		/// <summary>
		/// The SetVerticalSizeFlags method sets the horizontal size flag for controlling the y-axis behavior.
		/// </summary>
		/// <param name="flag">The `flag` parameter is of type `Control.SizeFlags` and it controls the y-axis
		/// behavior of the control.</param>
		/// <returns>
		/// The method is returning an object of type Thumbnaileable.
		/// </returns>
		public override Thumbnaileable SetVerticalSizeFlags(Control.SizeFlags flag)
		{
			base.SetVerticalSizeFlags(flag);

			return this;
		}

		/// <summary>
		/// This C# function sets the orientation of a container and returns a Thumbnaileable object.
		/// </summary>
		/// <param name="ContainerOrientation">ContainerOrientation is an enum that defines the orientation of
		/// a container. It typically represents whether the container is oriented horizontally or
		/// vertically.</param>
		/// <returns>
		/// The method is returning an instance of the class that implements the Thumbnaileable interface.
		/// </returns>
		public override Thumbnaileable SetOrientation(ContainerOrientation orientation)
		{
			base.SetOrientation(orientation);

			return this;
		}

		/// <summary>
		/// This C# function sets margin values for the currently chosen thumbnail and returns the object
		/// implementing the Thumbnaileable interface.
		/// </summary>
		/// <param name="value">The `value` parameter represents the margin value that you want to set for the
		/// currently chosen thumbnail.</param>
		/// <param name="side">The `side` parameter in the `SetMargin` method is used to specify which side of
		/// the thumbnail you want to set the margin for. It is a string parameter that can take values such
		/// as "top", "bottom", "left", or "right" to indicate the specific side where the</param>
		/// <returns>
		/// The method is returning an object of type `Thumbnaileable`.
		/// </returns>
		public override Thumbnaileable SetMargin(int value, string side = "")
		{
			base.SetMargin(value, side);

			return this;
		}
	}
}
#endif