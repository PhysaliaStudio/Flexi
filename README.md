## Quickstart

### Step 1: Create your stat list

We don't know what kind of stats do you need. Maybe you don't need "Attack". Maybe you need "Cost". Maybe you need another hundruds kinds of stats like POE.

So the first thing is to define them!  
`Create -> Physalia -> Ability System -> Stat List Asset`

Then you can click the "Generate Code" button the create the script.

### Step 2: Define your custom nodes

**WIP**

### Step 3: Create your ability graphs

**WIP**

### Step 4: Build the ability system and load your graphs in your codes

```csharp
// Build the AbilitySystem
var builder = new AbilitySystemBuilder();
builder.SetStatDefinitions(statListAsset);
AbilitySystem abilitySystem = builder.Build();

// Load your graphs into the system
abilitySystem.LoadAbilityGraph(987001, yourGraphAsset1);
abilitySystem.LoadAbilityGraph(987002, yourGraphAsset2);
```

### Step 5: Define your in-game actors

Actor is an container object to hold the stats, abilities and modifiers.  
For example, units have health and attack, and cards have mana cost and damage. So Unit and Card should derived from Actor.

```csharp
public class Unit : Actor
{
    private readonly UnitData unitData;

    public Unit(UnitData unitData, StatOwner owner) : base(owner)
    {
        this.unitData = unitData;
    }
}

public class Card : Actor
{
    private readonly CardData cardData;

    public Card(CardData cardData, StatOwner owner) : base(owner)
    {
        this.cardData = cardData;
    }
}
```

### Step 6: Create your actors

```csharp
private Unit CreateUnit(UnitData unitData)
{
    // Request a StatOwner from the system
    StatOwner owner = abilitySystem.CreateOwner();
    
    // Set the stats and their start values.
    // Things might be complex when there are some special condition in your design.
    // So it should better define them here.
    owner.AddStat(StatId.HEALTH, unitData.Health);
    owner.AddStat(StatId.ATTACK, unitData.Attack);

    // Normally in RPG, units have skills and statuses at start.
    // Append them with the ability system.
    IReadOnlyList<int> startStatusIds = unitData.StartStatusIds;
    for (var i = 0; i < startStatusIds.Count; i++)
    {
        // This will create an AbilityInstance and append to the StatOwner
        abilitySystem.AppendAbility(owner, startStatusIds[i]);
    }

    Unit unit = new Unit(unitData, owner);
    return unit;
}
```

### Step 7: Use the abilities

```csharp
// Create your custom payload data for start your ability
var context = new PlayCardNode.Context
{
    game = this,
    player = player,
    owner = heroUnit,
    card = card,
    random = generalRandom,
};

AbilityInstance ability = card.Abilities[0];  // Find saved ability instances from your actors
if (ability.CanExecute(context))  // Check if it's valid to start with the context
{
    abilitySystem.EnqueueAbility(ability, context);
    abilitySystem.Run();
}
```

### Step 8: Receive your custom events from nodes

IEventContext is an interface for customizing user defined events when something happened when running abilities.
There are 2 aspects to using the events:
1. Trigger more abilities, which has already handled in the system.
2. Represent what happened to players, and of course this should be implemented by the user. You can register the event.

```csharp
public void Initialize()
{
    abilitySystem.EventReceived += OnEventReceived;
}

private void OnEventReceived(IEventContext eventContext)
{
    // Normally here is for handling the representations for each events
    // ex. DamageEvent, DeathEvent, DrawCardEvent...etc.
}
```

## Stat

A stat have these properties:
* OriginalBase: This is the start value, and will never change.
* CurrentBase: This equals the start value at the start, but can be edit.
* CurrentValue: This is a readonly property, which might be modified by CurrentBase and other modifiers.

```csharp
actor.GetStat(StatId.ATTACK).OriginalBase;
actor.GetStat(StatId.ATTACK).CurrentBase;
actor.GetStat(StatId.ATTACK).CurrentValue;
```

If we need to modify or set the stat values. Use `Actor.SetStat()` or `Actor.ModifyStat()`.

```csharp
actor.SetStat(StatId.ATTACK, 10);  // Set the attack to 10. Note this is just the base value.
actor.ModifyStat(StatId.ATTACK, 5);  // Add 5 to the attack, so the attack base value becomes 15.
```

## AbilityInstance


## StatModifier and StatModierInstance

Don't take mistake modifiers for statuses.  
Normally a status contains both ability effects and modifiers.

A StatModifierItem represents a change data to a specific stat.
A StatModifier represents a collection data of StatModifierItems.
A StatModierInstance represents an instance of a StatModifier.

Modifiers should be defined in the data of abilities, statuses, items, etc., and it always need to reach some conditions to get the modifiers.

```csharp
StatModifer modifier;
var modierInstance = new StatModierInstance(modifier);

// Modifier related methods
actor.AppendModifier(modierInstance);
actor.RemoveModifier(modierInstance);
actor.ClearAllModifiers();
```
