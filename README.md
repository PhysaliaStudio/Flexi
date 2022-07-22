# How to Use

## Define the stats

First, we need to define what stats we need for our game.  
Create a file and define the stats we need as belowing, each line is a JSON format of `StatDefinition`:

```
{"Id":101,"Name":"Health"}
{"Id":111,"Name":"CurrentHealth"}
{"Id":201,"Name":"Attack"}
{"Id":202,"Name":"Defence"}
```

Actually, you can have your own format, like full JSON, CSV, ScriptableObject. We just need them becomes `List<StatDefinition>`.

## Create the owner repository and owners

Create the `StatOwnerRepository` from the list of definitions. This is the core object in this module.  
Then we need some "owners", like "characters", "items" or "cards", to bring the stats. Create owners from the repository.

```
List<StatDefinition> definitionList = LoadYourStatDefinitionFile();
StatOwnerRepository repository = StatOwnerRepository.Create(definitionList);
StatOwner owner = ownerRepository.CreateOwner();
```

## Add and get stats

```
owner.AddStat(101, 120);
owner.AddStat(111, 120);
owner.AddStat(201, 26);
owner.AddStat(202, 18);
```

Sometimes we need to directly modify the base value, which usually occurs in card games.  
Note that if you didn't add it previously, it just has no effect.
```
owner.SetStat(StatId.ATTACK, 36);
owner.SetStat(StatId.ATTACK, 36, false);  // set "refresh" to false if we don't want it.
```

You can get stats and check each values.
```
owner.GetStat(StatId.ATTACK).OriginalBase;  // returns 26
owner.GetStat(StatId.ATTACK).CurrentBase;  // returns 36
owner.GetStat(StatId.ATTACK).CurrentValue;  // returns 36 if "refresh" is true, else returns 26.
```

## Add and clear modifiers

We always need to temperary modify some stats. So add some modifier.

```
owner.AddModifier(new Modifier { StatId = StatId.ATTACK, Addend = 8 });
```

We don't have method to remove specific modifier. Because we don't need it.

Modifiers are from abilities, statuses, items, etc., which always need to reach some conditions to get the modifiers.

So normally we only need the clear method.

```
owner.ClearAllModifiers();
```

## Refresh the stats (Important)

Not finish yet. We added some modifiers, but this module don't automatically refresh the stats.

Since the logic deciding which modifiers to append can be very different, we leave it to business logic.

So after appending is done, we do refresh it manually.

```
owner.RefreshStats();
owner.GetStat(StatId.ATTACK).CurrentValue;  // returns 44 (= 36 + 8)
```
