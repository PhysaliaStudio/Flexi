# Flexi - Ability System Framework for Unity

![GitHub package.json version](https://img.shields.io/github/package-json/v/PhysaliaStudio/Flexi)
![GitHub](https://img.shields.io/github/license/PhysaliaStudio/Flexi)

-   **This repository is for source reference and demo**
-   **This project is still work-in-progress**  
    :green_square::green_square::green_square::green_square::green_square::green_square::green_square::white_large_square::white_large_square::white_large_square: 70%

-----

## Introduction

Flexi is an ***ability system framework*** for Unity.

While Flexi has done the general low-level logic:

- Programmers can directly jump into **creating the _gameplay ability system_**.
- Programmers can ***customize*** their task nodes and dicide how they work with the game.
- Then designers can use the ***built-in editor*** to edit ability data.
- The game finally runs abilities with the ***built-in runner***.

P.S. Yes, it's inspired from **[Unreal GAS (Gameplay Ability System)](https://docs.unrealengine.com/5.1/en-US/gameplay-ability-system-for-unreal-engine/)**, but Flexi is created with different concept.

## Features

- **Customizable**: Focus on your gameplay with user-defined stats, nodes, logics, events, etc.
- **Actor**: Base class to hold the stats, abilities and modifiers
- **Built-in ability runners**: Start from easy and not make your hands dirty
- **Node-based ability editor**: Built with GraphView
- **Macro**: Reuse your partial graphs
- **Blackboard**: Seperate values from graphs and inject from other sources
- **Non-singleton approach**: You can have multiple and different systems simutaneously

![Flexi Editor](https://raw.githubusercontent.com/wiki/PhysaliaStudio/Flexi/images/flexi-editor.gif)

## Resources

- [Wiki Home](https://github.com/PhysaliaStudio/Flexi/wiki)
- [Installation](https://github.com/PhysaliaStudio/Flexi/wiki/Installation)
- Getting Started
  - [1. Build your environment](https://github.com/PhysaliaStudio/Flexi/wiki/1.-Build-your-environment)
  - [2. Runtime Usage](https://github.com/PhysaliaStudio/Flexi/wiki/2.-Runtime-Usage)
  - [API Summary](https://github.com/PhysaliaStudio/Flexi/wiki/API-Summary)

## Limitations

- (v0.7) Not natively support real-time genre for now
- Not support DOTS (and not in roadmap for now)
- It's a tool for programmers, and recommended to have experienced programming skill

## TODO

- Support real-time genre
- Support undo/redo for the editor
- Support AOT
- Better implementation for Status & Aura
- Enable/Disable abilities
- Add Examples: Need to show how to implement each common machanic, to lower the learning curve.
