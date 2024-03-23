# AssetSnapGodot4
###### Version 0.1 - Development Status: In Development - Stable Projection: 3-4 Months - Codebase: C#
![image](https://github.com/misoe92/AssetSnap-Godot/assets/38956582/f653b54a-f4ac-44b1-a1b2-31f334a6d109)

### Description
This tool was mainly started due to the fact that my nephew wanted to make a game - as such i found it prudent to do my best to help him.
My background is not in C# programming nor is it in the production of games as such my know how and knowledge in the area was and still is not complete in any way. And i still learn every day.

Some periods i might be off for some days or even a week, due to health issues. So keep that in mind if i don't answer right away.
If you wish to follow the project on a more often basis plus getting sneak peaks from time to time this can be gotten from the thread here:
https://forum.godotengine.org/t/asset-placement-toolkit/48328/4

Where i will post things im working on, and often also mention something about when i expect x things to be ready for an initial realease.

I really hope that people will enjoy the addon, and even more i hope others will join and help make it better :) This tool for me personally is mainly being build due to me wanting to make a game with my nephew, the more help i can get by others the better tool we will have to work with when he and i starts in 3-4 months time. 

The tool tries to streamline the asset management and placement of them, it does this by enabling certain tools to be used to make the tasks less tedious which is needed if you have to keep a young persons interest throughout the process.

Some of these features are
1. Ability to add folders as libraries, enabling preview and easy placement.
2. Ability to snap model to X,Y,Z Axis or snap the model to other models with the ability to offset
3. Ability to add modifiers to placed models, so far Array and Scatter modifier is available.
4. Ability to drag add models, in which it takes 2 clicks which it then spawns models in that path based on rules you set
5. Ability to randomnize rotation or/and scale to children of group nodes / modifiers.
6. Ability to make continous placement of a model holding alt + shit while placing
7. Ability to grab already placed models to replace them, with the ability to also continous placement and more.
8. Ability to add auto collisions to models placed, both before and after placement.
9. Persistent spawn settings for models, allowing for easy updating of it later on.
10. Shortcuts to quickly change between rotate/scale on models you are placing.
11. Lot's of control in how the addon works through settings.

Above points is obviously just a start, but before i add anything else i want to make sure they are completely stable and ready to use and that the codebase is final before adding anything else.
So the only thing needed to think of at that point is to ensure features and stability.

I would also love to setup some testing framework for the code, but my experience sadly is lacking in that area. Any help in this area would be greatly appreciated.

### Getting started

#### Installing the plugin
To get started using this addon merely download this repository as a zip file, and unzip the content of that zip file into your project folder, or open the zip file and move into the addons folder and move that folder directly to your project folder.

Ensure that a C# Solution is initialized by going to:
Project -> Tools -> C# -> Create C# Solution

After that you can activate it in the project under:
Project->Project Settings->Plugins

And then make sure AssetSnap is checked. 
You might have to rebuild. Don't know if it builds the addons files upon adding only the addon folder, will find out in a couple of days and update this after.

#### Add a library
In the general window of the new "Assets" tab in the bottom dock you will find a button named "Add Folder",
Click on it and in the new window that opens move to the folder where your model files are and click "Select Current Folder"

This will add a new library, and the models in that folder will be loaded in and made available.

#### Place a model
When you have added a library, you will see a tab in your "Assets" window named the name of your folder.
Click on that, so we move into the library.

Click on the model you wish to place and click on the snapping you wish to use while placing, most of the time "Snap to height" will be the one used.
Move the cursor around the 3D world, and notice that the 3D model is following your mouse. Click with left click and notice the model is now placed.

#### Using modifiers
When you have placed an model you can perform certain tasks on them depending on your needs, let's say you need to insert a straight road for a good amount then you can repeat the model through an array modifier, which can both use normal meshinstances and multimeshinstance again depending on the needs.

When you click your model, a menu on the right bottom corner becomes visible, with options to perform modifiers on the model. Click on the one you want, i.e "Array".
Right after you will see the scene tree change, and your model has become a child of a new node. This is a modifier node, which you can then control and configure by clicking on the modifier node in the scene tree and edit it's settings.

When you change the settings the changes will be updated on the model instantly. So you can see the result right away. 

#### Using object snapping
If you already have placed models in the 3D world and you wish to ensure you snap to that object with exact precision you can then use "Snap to objects" which opens up for custom offset as well.
When this is enabled and you move close to another model in the 3D world your model will snap to it. This feature is still heavily under development and still needs alot of polishment. So beware of issues.

When it's snapped to the object you can left click as normal and it will then snap to that object. If you wish to add offset to the model you can do that by setting the offset input boxes after Object snapping has been turned on.

### Video Introduction
The video which will have it's links placed below in a small weeks time, it will be about how the plugin works, how it can be used and where various of things are.
It will overall give an more in depth feel of usage. That said my skills in building envrionments aren't great.. So have that in mind.. 

#### Installing AssetSnap
https://www.youtube.com/watch?v=giCKSwaCULs

#### Adding / Removing libraries
https://www.youtube.com/watch?v=A9d1HyfMfE4

More will come soon, merely need to create them. But im going to fix bugs while doing it to ensure that i catch the bugs i've found.

### Current state
The project is currently under development, hence the code is not ready in any way for an professional environment. And alot of improvement and polish would still need to be done before anything in that manner would be a talking point.
The various areas is functional now, and most if not all things are working but might still contain small amount of bugs in smaller degrees.
The code base also needs to be polished alot more, and maybe a better structure.

### Known Issues
1. Upon rebuild of C# project it has issues regarding activating some parts of the plugin until a reload of the scene. I am aware of it, and working to find out why and how i can fix it.
2. Some minor issues with the 2 modifiers - they still work but they throw a few errors still, tho this should be fixed in a couple of days.
   
### License
This project is released under the MIT License.

### Credits
misoe92 - So far for everything

### Want to help?
Im still new to c# - a more experienced developer would prob. ensure a much better structure and overall experience with the addon. And i would love to improve myself.
So if you want to help out let me know. Especially if you have experience in setting up testing envrionment. That can be run to ensure everything works before a compilation.

You can write to me at mikebsorensen1@gmail.com ATT: AssetSnap
