# Flexi - Unity Ability System Framework

![GitHub tag (latest SemVer)](https://img.shields.io/github/v/tag/PhysaliaStudio/Flexi?sort=semver)
![GitHub](https://img.shields.io/github/license/PhysaliaStudio/Flexi)
![Discord](https://img.shields.io/discord/1334847919441838120?style=flat&link=https%3A%2F%2Fdiscord.gg%2FU24EsyyGfa)

- **This project is still work-in-progress** 
- Sorry that some docs are not updated yet.
- Though Flexi is still in development, it's actually in usable state, and already worked on a real project. You can join the [Discord Server](https://discord.gg/U24EsyyGfa) if you like to discuss about Flexi.
- Progress  
    :green_square::green_square::green_square::green_square::green_square::green_square::green_square::green_square::green_square::white_large_square: 90%

-----

## What is Flexi?

Flexi is an ***ability system framework*** for Unity.

The basic concept is:
- Flexi has done some general low-level logic.
- Programmers can directly jump into extend with Flexi, and create your ***gameplay ability system***.
- Then designers can use the ***graph view editor*** to create/edit ability.
- At runtime, load these abilities and run with ***built-in runner***.

P.S. Flexi is inspired from **[Unreal GAS (Gameplay Ability System)](https://docs.unrealengine.com/5.1/en-US/gameplay-ability-system-for-unreal-engine/)**, but not the same concept.

![Flexi Scope](https://raw.githubusercontent.com/wiki/PhysaliaStudio/Flexi/images/flexi-scope.png)

## Features

- **Customizable**: Flexi only do low-level things. So you have space to implement your own gameplay.
- **Stat & Modifier**: With stat refresh process, modify unit stats with abilities.
- **Built-in ability runners**: Start from easy and not make your hands dirty.
- **Node-based ability editor**: Built with GraphView and UIToolkit.
- **OOP approach**: Not rely on Unity. Not singleton approach.
- ~~**Macro**: Reuse your partial graphs~~ (Failed to find GC free approach. Suspend until main feature finished.)

![Flexi Editor](https://raw.githubusercontent.com/wiki/PhysaliaStudio/Flexi/images/flexi-editor.gif)![Card Game Sample 1](https://raw.githubusercontent.com/wiki/PhysaliaStudio/Flexi/images/card-game-samples-1.gif)

![Recast Sample](https://user-images.githubusercontent.com/12347255/212114826-effc1d31-de16-4fb2-aa15-6b0dd68c0441.png)![Real-Time Sample 1](https://user-images.githubusercontent.com/12347255/212114905-b9c80f7f-6aed-4ac0-a3af-5cbba58c44a6.gif)

## Installation

**Download repository and Export unitypackage**
1. Donwload and unzip. Choose the latest tag for stable state.
2. Open this Unity project. (Dev in 2021.3)
3. Export the package with `Tools/Export UnityPackage`. This excludes unnecessary assets.
4. It will show you the result package, then you can import this to your project.

## Resources

- Quickstart
  - [1. Hello World](https://github.com/PhysaliaStudio/Flexi/wiki/1-Hello-World)
  - [2. Custom Node](https://github.com/PhysaliaStudio/Flexi/wiki/2-Custom-Node)
  - [3. Ability Chain](https://github.com/PhysaliaStudio/Flexi/wiki/3-Ability-Chain)
  - [API Summary](https://github.com/PhysaliaStudio/Flexi/wiki/API-Summary)
- References
  - [tranek/GASDocumentation](https://github.com/tranek/GASDocumentation)
  - [Exploring the Gameplay Ability System (GAS) with an Action RPG](https://www.youtube.com/watch?v=tc542u36JR0)
  - [Networking Scripted Weapons and Abilities in Overwatch](https://www.youtube.com/watch?v=ScyZjcjTlA4)

## Limitations

- Not support DOTS. (This is full OOP approach.)
- It's a tool for programmers. Recommend to have experienced programming skill.

## TODO List

- [ ] Finish Macro
- [ ] UX Optimize
- [ ] Better Samples
- [ ] ListView for blackboard might easily broken. (Layout update is struggling to process current layout)
