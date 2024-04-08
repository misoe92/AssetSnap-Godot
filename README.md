<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->
<a name="readme-top"></a>
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Don't forget to give the project a star!
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->

[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/othneildrew/Best-README-Template">
    <img src="images/logo.jpg" alt="Logo" width="300" height="225">
  </a>

  <h3 align="center">AssetSnap</h3>

  <p align="center">
    Asset placement toolkit for Godot 4 Mono
    <br />
    <a href="#"><strong>Docs comming soon »</strong></a>
    <br />
    <br />
    <a href="https://github.com/misoe92/AssetSnap-Godot-Template">Example Project (Soon)</a>
    ·
    <a href="https://github.com/misoe92/AssetSnap-Godot/issues/new?labels=bug&template=bug-report---.md">Report Bug</a>
    ·
    <a href="https://github.com/misoe92/AssetSnap-Godot/issues/new?labels=enhancement&template=feature-request---.md">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

[![Product Name Screen Shot][product-screenshot]](https://example.com)

This toolkit was started as a way for me to give my nephew an easier entry in to game development, more specifically 3D game development. This toolkit essentially aims to statisfy all the level building needs that you might have. And if it does not yet do this, please post an issue and we will see on how we can fit your need into the calculation. It is a work in progress, so bugs still might occur. Therefore usage in professional environment should still be cautious. Will update this part of the text when the product is polished enough for a professional usage case. 

The plugin is quite extensive already, but below you can get a heads up about the overall functionality that it provides below. And then i can only recommend to download it and try it out. It is after all completely free - and i would love the feedback.

### Groups
![image](https://github.com/misoe92/AssetSnap-Godot/assets/38956582/0043dd77-443a-412e-9682-0b9caf5bfdfa)

Currently it allows for the creation of Groups, which are groups of 3D objects that have their positions defined in local space. Which allows for building a house as an example with modular blocks and then easily use it again, or duplicate an earlier house, change 1 block and use that. 

#### Persistency
Groups inner structure are persistent, meaning if you change an group 3D object origin, rotation or scale it will instantly be updated on all instances of that group. This does not count for things like default collisions and more that are set when you spawn your group.

#### 3D Preview
![image](https://github.com/misoe92/AssetSnap-Godot/assets/38956582/85489a08-130f-4676-bc1d-453dd533f6fa)

Preview your creation in a 3D Preview environment while you are building it. Allowing you to fly around your 3D object to ensure all the details are as they need to be.

### Libraries
![image](https://github.com/misoe92/AssetSnap-Godot/assets/38956582/39809aa6-8288-4e93-b03a-8bd871a8938f)

Create libraries that houses a folder and all of it 3D models, allowing you to search, place, and perform various actions on the models when placing them in the 3D world.

#### Add and remove libraries
You can add and remove libraries easily from the main page of the tab "Assets"

#### Placement Modes
##### Simple placement mode
Use to place mesh instances, this is mainly for fewer spawns and where you need certain things to have a certain script and such.

##### Advanced placement mode
Use to place multi meshes, in configureable chunks. Which allows for a more optimized end result when you are spawning alot of objects.

#### Continuos placement
Fast placement of many objects with continous placement, which relieves you from the task of having to select the item again and again.
All you have to do is hold alt + shift.

#### Drag path placement
Drag a path and spawn a configureable amount using an offset to control the total spawn amount.

#### Plane Snapping
Snap to planes on the X,Y and Z Axis, the axis boundary can be configured to match the needs you have for the task.

#### Object Snapping
Snap to other placed 3D objects in the 3D world as long as they have been spawned with the same spawn layer as the object you are spawning.

#### Auto collisions
Easy auto collisions for your objects when you space them, or change them afterwards directly while the object is in focus.

#### Grab already placed objects
By holding shift + alt and clicking A you can grab up already placed objects in the world, making it an ease to pick them up and move them to their new place.

#### Inspector
Ability to keep an eye on the states, this is mainly usefull when things don't work.. So let's hope it does not need to be used.

### Modifiers
Ability to convert Mesh instances into Scatter or Array Modifiers.

#### Scatter Modifier
Scatter an configureable amount of entries of an object in an specified radius with various of tools and optimizations to go with the modifier.

#### Array Modifier
Spawn an configureable array of objects with various of tools and optimizations to go with the modifier.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Built With

This plugin was build with Godot and for Godot.

Beside that Icons from <a href="https://heroicons.com">HeroIcons</a> have been used.
And 3D objects from libraries from [Here](https://fertile-soil-productions.itch.io/modular-village-pack) and [Here](https://fertile-soil-productions.itch.io/modular-terrain-pack) has been used while building it, and is the 3D objects seen in the preview images.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

Below you can find a detailed guide on how to download and install AssetSnap for your Godot 4.2 Mono project.
If you rather get going quickly you can also download a free example project with AssetSnap already setup from <a>here</a>

### Prerequisites
Godot 4.2 Mono
* For Windows
  ```sh
  https://godotengine.org/download/windows/
  ```
* For Linux
  ```sh
  https://godotengine.org/download/linux/
  ```

### Installation

Download your copy of AssetSnap by either downloading a zip of the current repository, Forking or Cloning the repository.

* Download Zip
1. Click on the green button labeled `Code`
2. Click on `Download Zip`
3. Save the zip file at a place of your choice
4. Unzip the zip file
5. Copy the folder named Addons from within the new folder into your project
6. **Only for new projects** Click on the ``Project`` menu
7. **Only for new projects** Move your cursor to ``Tools``
8. **Only for new projects** Move your cursor to ``C#``
9. **Only for new projects** Click ``Create C# Solution``
10. Click on the hammer in the right corner to build your current solution
11. Click on ``Project``, And click on ``Project Settings``
12. Click on ``Plugin`` in the top of the new window, and check the ``Enable`` checkbox next to the plugin named AssetSnap
13. AssetSnap Should now work as expected

* Forking
1. Explanation for this section is comming soong

* Cloning
1. Explanation for this section is comming soong

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- USAGE EXAMPLES -->
## Usage

For usage resources please either head to the Youtube Channel, where things will be posted on how to use various of things.

[Channel](https://www.youtube.com/channel/UCt0HT7KVysrvraMEn1U3QXQ)

or wait for a bit longer until i get a documentation set up.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ROADMAP -->
## Roadmap
As of 0.1.1
- [x] Simple placement of objects using meshinstances
- [x] Optimized placement of objects using multimeshinstances
- [x] Auto collisions
- [x] Placement of pre defined groups of objects
- [X] Live preview of grouped objects while building
- [x] Continuous placement
- [x] Drag placement with offsetting
- [x] Object snapping on multiple layers
- [x] Plane Snapping
- [x] Library listing of objects with previews and search
- [x] Searchable library listing
- [x] Simple Settings control
- [x] Scatter modifier, that enables configureable scattering of an object
- [x] Array modifier, that enables configureable array spawn of an object

As of 0.1.2
- [ ] Upgraded settings UI
- [ ] Level of detail Control
- [ ] Better Decal UI
- [ ] Better folder structure for components

See the [open issues](https://github.com/misoe92/AssetSnap-Godot/issues) for a full list of proposed features (and known issues).

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- CONTACT -->
## Contact

Mike Sørensen - [@MikeSrensen19](https://twitter.com/MikeSrensen19) - mikebsorensen1@gmail.com

Project Link: [https://github.com/misoe92/AssetSnap-Godot](https://github.com/misoe92/AssetSnap-Godot)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

For this project several of resources have been used, below you will find a link and description of the various items.
None of the items beside HeroIcons can be found in the project when you download it.

* [Readme Layout](https://github.com/othneildrew/Best-README-Template)
* [Modular Terrain Pack](https://fertile-soil-productions.itch.io/modular-terrain-pack)
* [Modular Village Pack](https://fertile-soil-productions.itch.io/modular-village-pack)
* [HeroIcons](https://heroicons.com/)

Also a mention should go to Godot as well, for their free and awesome piece of software. 

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/misoe92/AssetSnap-Godot.svg?style=for-the-badge
[contributors-url]: https://github.com/misoe92/AssetSnap-Godot/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/misoe92/AssetSnap-Godot.svg?style=for-the-badge
[forks-url]: https://github.com/misoe92/AssetSnap-Godot/network/members
[stars-shield]: https://img.shields.io/github/stars/misoe92/AssetSnap-Godot.svg?style=for-the-badge
[stars-url]: https://github.com/misoe92/AssetSnap-Godot/stargazers
[issues-shield]: https://img.shields.io/github/issues/misoe92/AssetSnap-Godot.svg?style=for-the-badge
[issues-url]: https://github.com/misoe92/AssetSnap-Godot/issues
[license-shield]: https://img.shields.io/github/license/misoe92/AssetSnap-Godot.svg?style=for-the-badge
[license-url]: https://github.com/misoe92/AssetSnap-Godot/LICENSE
[product-screenshot]: images/screenshot.png
[Next.js]: https://img.shields.io/badge/next.js-000000?style=for-the-badge&logo=nextdotjs&logoColor=white
[Next-url]: https://nextjs.org/
[React.js]: https://img.shields.io/badge/React-20232A?style=for-the-badge&logo=react&logoColor=61DAFB
[React-url]: https://reactjs.org/
[Vue.js]: https://img.shields.io/badge/Vue.js-35495E?style=for-the-badge&logo=vuedotjs&logoColor=4FC08D
[Vue-url]: https://vuejs.org/
[Angular.io]: https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white
[Angular-url]: https://angular.io/
[Svelte.dev]: https://img.shields.io/badge/Svelte-4A4A55?style=for-the-badge&logo=svelte&logoColor=FF3E00
[Svelte-url]: https://svelte.dev/
[Laravel.com]: https://img.shields.io/badge/Laravel-FF2D20?style=for-the-badge&logo=laravel&logoColor=white
[Laravel-url]: https://laravel.com
[Bootstrap.com]: https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white
[Bootstrap-url]: https://getbootstrap.com
[JQuery.com]: https://img.shields.io/badge/jQuery-0769AD?style=for-the-badge&logo=jquery&logoColor=white
[JQuery-url]: https://jquery.com 
