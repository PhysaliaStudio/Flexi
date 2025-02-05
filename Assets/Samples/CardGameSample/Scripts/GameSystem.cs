using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class GameSystem : MonoBehaviour
    {
        [SerializeField]
        private GameSetting gameSetting;

        private AssetManager assetManager;
        private GameDataManager gameDataManager;
        private GamePresenter gamePresenter;

        private void Awake()
        {
            LoadAllGameData();

            DOTween.Init();
            BuildGame();
            StartGame();
        }

        private void LoadAllGameData()
        {
            assetManager = new AssetManager("CardGameSample");
            gameDataManager = new GameDataManager(assetManager);

            gameDataManager.LoadAllData<CardData>("GameData/Cards");
            gameDataManager.LoadAllData<HeroData>("GameData/Heroes");
            gameDataManager.LoadAllData<EnemyData>("GameData/Enemies");
            gameDataManager.LoadAllData<EnemyGroupData>("GameData/EnemyGroups");
            gameDataManager.LoadAllData<StatusData>("GameData/Statuses");
        }

        private void BuildGame()
        {
            var game = new Game(assetManager, gameDataManager, gameSetting);
            GameView gameView = FindObjectOfType<GameView>();
            gamePresenter = new GamePresenter(game, gameView);

            gamePresenter.Initialize();

            IReadOnlyDictionary<int, HeroData> heroTable = gameDataManager.GetTable<HeroData>();
            HeroData randomHeroData = heroTable.RandomPickOne(new System.Random()).Value;
            gamePresenter.SetUp(randomHeroData);
        }

        private void StartGame()
        {
            gamePresenter.Start();
        }

        private void Update()
        {
            if (gamePresenter == null)
            {
                return;
            }

            gamePresenter.Update();
        }
    }
}
