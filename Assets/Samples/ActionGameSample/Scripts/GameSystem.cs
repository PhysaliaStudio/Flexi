using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.ActionGame
{
    public class GameSystem : MonoBehaviour, IFlexiCoreWrapper
    {
        private AssetManager assetManager;
        private FlexiCore flexiCore;
        private readonly DefaultModifierHandler modifierHandler = new();

        private Unit playerUnit;

        private void Awake()
        {
            assetManager = new AssetManager("Flexi/ActionGameSample");
            flexiCore = CreateFlexiCore(this, assetManager);
            playerUnit = BuildPlayer(assetManager, flexiCore);

            AbilitySlotView slotView = FindObjectOfType<AbilitySlotView>();
            slotView.SetUnit(playerUnit);
        }

        private static FlexiCore CreateFlexiCore(IFlexiCoreWrapper wrapper, AssetManager assetManager)
        {
            var builder = new FlexiCoreBuilder();
            builder.SetWrapper(wrapper);
            builder.SetRunner(new RealTimeFlowRunner());
            FlexiCore flexiCore = builder.Build();

            MacroAsset[] macroAssets = assetManager.LoadAll<MacroAsset>("AbilityGraphs");
            for (var i = 0; i < macroAssets.Length; i++)
            {
                MacroAsset macroAsset = macroAssets[i];
                flexiCore.LoadMacroGraph(macroAsset.name, macroAsset);
            }

            return flexiCore;
        }

        private static Unit BuildPlayer(AssetManager assetManager, FlexiCore flexiCore)
        {
            UnitAvatar avatar = FindObjectOfType<UnitAvatar>();
            Unit unit = new Unit(avatar);
            unit.AddStat(StatId.SPEED, 200);
            unit.AddStat(StatId.CONTROLLABLE, 1);

            AbilityAsset abilityAsset = assetManager.Load<AbilityAsset>("AbilityGraphs/Combo");
            AbilityData abilityData = abilityAsset.Data;
            flexiCore.LoadAbilityAll(abilityData, 2);
            for (var i = 0; i < abilityData.graphGroups.Count; i++)
            {
                var container = new DefaultAbilityContainer(abilityData, i);
                unit.AppendAbilityContainer(container);
            }

            return unit;
        }

        #region Implement IFlexiCoreWrapper
        public void OnEventReceived(IEventContext eventContext)
        {

        }

        public void ResolveEvent(FlexiCore flexiCore, IEventContext eventContext)
        {
            flexiCore.TryEnqueueAbility(playerUnit.AbilityContainers, eventContext);
        }

        public IReadOnlyList<StatOwner> CollectStatRefreshOwners()
        {
            var result = new List<StatOwner>();
            result.Add(playerUnit);
            return result;
        }

        public IReadOnlyList<AbilityContainer> CollectStatRefreshContainers()
        {
            var result = new List<AbilityContainer>();
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
                        _ = flexiCore.TryEnqueueAbility(playerUnit.AbilityContainers[0]);
                        flexiCore.Run();
                    }
                    else if (state == AbilitySlot.State.RECAST)
                    {
                        flexiCore.Resume(new RecastContext());
                    }
                }
            }

            playerUnit.Tick();
            flexiCore.Tick();
        }
    }
}
