using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.ActionGame
{
    public class GameSystem : MonoBehaviour, IAbilitySystemWrapper
    {
        private AssetManager assetManager;
        private AbilitySystem abilitySystem;
        private readonly DefaultModifierHandler modifierHandler = new();

        private Unit playerUnit;

        private void Awake()
        {
            assetManager = new AssetManager("Flexi/ActionGameSample");
            abilitySystem = CreateAbilitySystem(this, assetManager);
            playerUnit = BuildPlayer(assetManager, abilitySystem);

            AbilitySlotView slotView = FindObjectOfType<AbilitySlotView>();
            slotView.SetUnit(playerUnit);
        }

        private static AbilitySystem CreateAbilitySystem(IAbilitySystemWrapper wrapper, AssetManager assetManager)
        {
            var builder = new AbilitySystemBuilder();
            builder.SetWrapper(wrapper);
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
            Unit unit = new Unit(avatar);
            unit.AddStat(StatId.SPEED, 200);
            unit.AddStat(StatId.CONTROLLABLE, 1);

            AbilityAsset abilityAsset = assetManager.Load<AbilityAsset>("AbilityGraphs/Combo");
            AbilityData abilityData = abilityAsset.Data;
            for (var i = 0; i < abilityData.graphGroups.Count; i++)
            {
                AbilityHandle abilityHandle = abilityData.CreateHandle(i);
                var container = new AbilityContainer { Handle = abilityHandle };
                abilitySystem.CreateAbilityPool(abilityHandle, 2);
                unit.AppendAbilityContainer(container);
            }

            return unit;
        }

        #region Implement IAbilitySystemWrapper
        public void OnEventReceived(IEventContext eventContext)
        {

        }

        public void ResolveEvent(AbilitySystem abilitySystem, IEventContext eventContext)
        {
            abilitySystem.TryEnqueueAbility(playerUnit.AbilityContainers, eventContext);
        }

        public IReadOnlyList<StatOwner> CollectStatRefreshOwners()
        {
            var result = new List<StatOwner>();
            result.Add(playerUnit);
            return result;
        }

        public IReadOnlyList<AbilityDataContainer> CollectStatRefreshContainers()
        {
            var result = new List<AbilityDataContainer>();
            result.AddRange(playerUnit.AbilityContainers);
            return result;
        }

        public void OnBeforeCollectModifiers()
        {

        }

        public void ApplyModifiers(StatOwner statOwner)
        {
            modifierHandler.ApplyModifiers(statOwner);
        }
        #endregion

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
                        _ = abilitySystem.TryEnqueueAbility(playerUnit.AbilityContainers[0]);
                        abilitySystem.Run();
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
