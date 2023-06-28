using UnityEngine;

namespace Physalia.Flexi.Samples.ActionGame
{
    public class CompositionRoot : MonoBehaviour
    {
        private AssetManager assetManager;

        private AbilitySystem abilitySystem;
        private Unit playerUnit;

        private void Awake()
        {
            assetManager = new AssetManager("Flexi/ActionGameSample");
            abilitySystem = CreateAbilitySystem(assetManager);
            playerUnit = BuildPlayer(assetManager, abilitySystem);

            AbilitySlotView slotView = FindObjectOfType<AbilitySlotView>();
            slotView.SetUnit(playerUnit);
        }

        private static AbilitySystem CreateAbilitySystem(AssetManager assetManager)
        {
            var builder = new AbilitySystemBuilder();

            StatDefinitionListAsset statListAsset = assetManager.Load<StatDefinitionListAsset>("StatList");
            builder.SetStatDefinitions(statListAsset);
            builder.SetRunner(new RealTimeFlowRunner());

            AbilitySystem abilitySystem = builder.Build();

            MacroAsset[] macroAssets = assetManager.LoadAll<MacroAsset>("AbilityGraphs");
            for (var i = 0; i < macroAssets.Length; i++)
            {
                MacroAsset macroAsset = macroAssets[i];
                abilitySystem.LoadMacroGraph(macroAsset.name, macroAsset);
            }

            return abilitySystem;
        }

        private static Unit BuildPlayer(AssetManager assetManager, AbilitySystem abilitySystem)
        {
            UnitAvatar avatar = FindObjectOfType<UnitAvatar>();
            Unit unit = new Unit(avatar, abilitySystem);
            unit.AddStat(StatId.SPEED, 200);
            unit.AddStat(StatId.CONTROLLABLE, 1);

            AbilityAsset abilityAsset = assetManager.Load<AbilityAsset>("AbilityGraphs/Combo");
            AbilityData abilityData = abilityAsset.Data;
            for (var i = 0; i < abilityData.graphGroups.Count; i++)
            {
                AbilityDataSource abilityDataSource = abilityData.CreateDataSource(i);
                unit.AppendAbility(abilityDataSource);
            }

            return unit;
        }

        private void Update()
        {
            if (playerUnit.IsControllable())
            {
                float x = Input.GetAxis("Horizontal");
                float y = Input.GetAxis("Vertical");
                playerUnit.Move(x, y);

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    AbilitySlot.State state = playerUnit.AbilitySlot.GetState();
                    if (state == AbilitySlot.State.OPEN)
                    {
                        _ = abilitySystem.TryEnqueueAndRunAbility(playerUnit.Abilities[0], null);
                    }
                    else if (state == AbilitySlot.State.RECAST)
                    {
                        abilitySystem.Resume(new RecastContext());
                    }
                }
            }

            playerUnit.Tick();
            abilitySystem.Tick();
        }
    }
}
