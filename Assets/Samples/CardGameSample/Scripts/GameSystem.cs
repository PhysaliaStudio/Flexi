using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class GameSystem
    {
        private readonly AssetManager assetManager;
        private readonly GameDataManager gameDataManager;
        private readonly GameSetting gameSetting;

        private GamePresenter gamePresenter;

        public GameSystem(AssetManager assetManager, GameDataManager gameDataManager, GameSetting gameSetting)
        {
            this.assetManager = assetManager;
            this.gameDataManager = gameDataManager;
            this.gameSetting = gameSetting;
        }

        public void BuildGame()
        {
            var game = new Game(assetManager, gameDataManager, gameSetting);
            GameView gameView = Object.FindObjectOfType<GameView>();
            gamePresenter = new GamePresenter(game, gameView);

            gamePresenter.Initialize();

            IReadOnlyDictionary<int, HeroData> heroTable = gameDataManager.GetTable<HeroData>();
            HeroData randomHeroData = heroTable.RandomPickOne(new System.Random()).Value;
            gamePresenter.SetUp(randomHeroData);
        }

        public void StartGame()
        {
            gamePresenter.Start();
        }

        public void Update()
        {
            if (gamePresenter == null)
            {
                return;
            }

            gamePresenter.Update();
        }
    }
}
