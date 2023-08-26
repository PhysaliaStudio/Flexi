using DG.Tweening;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class CompositionRoot : MonoBehaviour
    {
        [SerializeField]
        private GameSetting gameSetting;

        private AssetManager assetManager;
        private GameDataManager gameDataManager;
        private GameSystem gameSystem;

        private MacroAsset[] macroAssets;

        private void Awake()
        {
            LoadAllGameData();

            DOTween.Init();
            CreateGameSystem();
        }

        private void LoadAllGameData()
        {
            assetManager = new AssetManager("Flexi/CardGameSample");
            gameDataManager = new GameDataManager(assetManager);
            macroAssets = assetManager.LoadAll<MacroAsset>("AbilityGraphs");

            gameDataManager.LoadAllData<CardData>("GameData/Cards");
            gameDataManager.LoadAllData<HeroData>("GameData/Heroes");
            gameDataManager.LoadAllData<EnemyData>("GameData/Enemies");
            gameDataManager.LoadAllData<EnemyGroupData>("GameData/EnemyGroups");
            gameDataManager.LoadAllData<StatusData>("GameData/Statuses");
        }

        private void CreateGameSystem()
        {
            var builder = new AbilitySystemBuilder();
            AbilitySystem abilitySystem = builder.Build();
            for (var i = 0; i < macroAssets.Length; i++)
            {
                MacroAsset macroAsset = macroAssets[i];
                abilitySystem.LoadMacroGraph(macroAsset.name, macroAsset);
            }

            gameSystem = new GameSystem(gameDataManager, abilitySystem, gameSetting);
            gameSystem.BuildGame();
            gameSystem.StartGame();
        }

        private void Update()
        {
            gameSystem.Update();
        }
    }
}
