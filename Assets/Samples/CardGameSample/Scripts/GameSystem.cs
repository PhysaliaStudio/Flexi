using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class GameSystem
    {
        private readonly GameDataManager gameDataManager;
        private readonly AbilitySystem abilitySystem;
        private readonly GameSetting gameSetting;

        private GamePresenter gamePresenter;

        public GameSystem(GameDataManager gameDataManager, AbilitySystem abilitySystem, GameSetting gameSetting)
        {
            this.gameDataManager = gameDataManager;
            this.abilitySystem = abilitySystem;
            this.gameSetting = gameSetting;
        }

        public void BuildGame()
        {
            var game = new Game(gameSetting, gameDataManager, abilitySystem);
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
