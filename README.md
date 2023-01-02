# Flexi - Ability System Framework for Unity

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

## Limitations

- Not support DOTS (and not in roadmap for now)
- It's a tool for programmers, and recommended to have experienced programming skill

## TODO

- Support real-time genre
- Support undo/redo for the editor
- Support AOT
- Better implementation for Status & Aura
- Enable/Disable abilities
- Add Examples: Need to show how to implement each common machanic, to lower the learning curve.

## Installation

-   **Using Git URL (`manifest.json`)**

    - This method is sufficient for reference and learning, but not good for production.
    - Note that you need to have Git installed in your system.
    - Note that embedded Git, like Fork or SourceTree use as default, is not considered in the system.
    
    ```
    {
      "dependencies": {
        "studio.physalia.flexi": "https://github.com/PhysaliaStudio/Flexi.git#0.7.0"
      }
    }
    ```

## API Quickstart

### Customize Actors

Define your ability owner by deriving from `Actor`, ex. units, cards, etc.

**Example**

```csharp
public class Unit : Actor
{
    private readonly UnitData unitData;

    public Unit(UnitData unitData, AbilitySystem abilitySystem) : base(abilitySystem)
    {
        this.unitData = unitData;
    }
}

// Specify stats and starting values you need
unit.AddStat(StatId.HEALTH, 100);
unit.AddStat(StatId.ATTACK, 10);

// Create ability instances
Ability ability = unit.AppendAbility(abilityAsset);
```

### Customize Contexts

**Contexts** are for telling your system **"what happened in detail?"**.  
They should be custom data classes and it's you to decide how to handle these contexts.

There are 3 kinds of contexts as **marker interfaces**:

- `IEventContext`: For (1) starting running the ability queue, or (2) notifing game event happened.
- `IChoiceContext`: For notify a choice happened and pause.
- `IResumeContext`: For resume from the paused state, and normally respond to the choice.

**Example**

```csharp
public class DamageEvent : IEventContext
{
    attacker = attacker,
    targets = new List<Unit>(targets),
    amount = value,
}
```

### Customize Nodes

**Example**

```csharp
[NodeCategory("Card Game Sample")]
public class DamageNode : ProcessNode
{
    public Inport<Unit> attackerPort;
    public Inport<IReadOnlyList<Unit>> targetsPort;
    public Inport<int> valuePort;

    protected override AbilityState DoLogic()
    {
        var attacker = attackerPort.GetValue();
        var targets = targetsPort.GetValue();
        if (targets.Count == 0)
        {
            return AbilityState.RUNNING;
        }

        var value = valuePort.GetValue();
        for (var i = 0; i < targets.Count; i++)
        {
            targets[i].ModifyStat(StatId.HEALTH, -value);
        }

        EnqueueEvent(new DamageEvent
        {
            attacker = attacker,
            targets = new List<Unit>(targets),
            amount = value,
        });

        return AbilityState.RUNNING;
    }
}
```

### Run Abilities

Run your abilities with the following API:

- `bool AbilitySystem.TryEnqueueAndRunAbility(ability/ies, eventContext)`
- `bool AbilitySystem.TryEnqueueAbility(ability/ies, eventContext)` + `void AbilitySystem.Run()`

**Example**

```csharp
bool success = abilitySystem.TryEnqueueAndRunAbility(unit.Abilitiees, eventContext);
```

### Handle Context Events

There are some events you should register or implement.

-   `event Action<IEventContext> EventOccurred`  
    Triggered from your custom nodes, this event is for presenting what happened.
    
-   `event Action<IChoiceContext> ChoiceOccurred`  
    Triggered from your custom nodes, this event is for presenting a choice.
    
-   `Action<IEventContext> EventResolveMethod`  
    This is not an event but a pure delegate. Cached event contexts will be resolved at specific timing. You should decide how to chain more abilities by custom iteration.
    
    **Example**
    
    ```
    // Iterate each unit to chain more abilities
    private void ResolveEvent(IEventContext context)
    {
        abilitySystem.TryEnqueueAbility(heroUnit.Abilities, context);
        for (var i = 0; i < enemyUnits.Count; i++)
        {
            abilitySystem.TryEnqueueAbility(enemyUnits[i].Abilities, context);
        }
    }
    ```
