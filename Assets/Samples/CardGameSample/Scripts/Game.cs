using System;
using System.Collections.Generic;

namespace Physalia.Flexi.Samples.CardGame
{
    public interface IUnitRepository
    {
        IReadOnlyList<Unit> Enemies { get; }
    }

    public class Game : IUnitRepository
    {
        public event Action<Unit, PlayArea> GameSetUp;
        public event Action<Card> CardSelected;
        public event Action<object> GameEventReceived;
        public event Action<Card, IChoiceContext> ChoiceOccurred;

        private readonly GameSetting gameSetting;
        private readonly GameDataManager gameDataManager;
        private readonly AbilitySystem abilitySystem;

        private readonly Random generalRandom = new();
        private readonly HashSet<EnemyGroupData> groupDatas = new();

        private List<Ability> gameStartProcess;
        private List<Ability> turnEndProcess;

        private Player player;
        private Unit heroUnit;
        private PlayArea playArea;
        private readonly List<Unit> enemyUnits = new();

        public Player Player => player;
        public IReadOnlyList<Unit> Enemies => enemyUnits;

        public Game(GameSetting gameSetting, GameDataManager gameDataManager, AbilitySystem abilitySystem)
        {
            this.gameSetting = gameSetting;
            this.gameDataManager = gameDataManager;
            this.abilitySystem = abilitySystem;
        }

        public void SetUp(HeroData heroData)
        {
            var groupDataTable = gameDataManager.GetTable<EnemyGroupData>();
            foreach (EnemyGroupData groupData in groupDataTable.Values)
            {
                groupDatas.Add(groupData);
            }

            abilitySystem.EventOccurred += OnEventOccurred;
            abilitySystem.ChoiceOccurred += OnChoiceOccurred;
            abilitySystem.EventResolveMethod = ResolveEvent;

            Ability turnStartEffect = abilitySystem.GetAbility(gameSetting.turnStartGraph.Data, 0);
            Ability turnEndEffect = abilitySystem.GetAbility(gameSetting.turnEndGraph.Data, 0);
            Ability enemyGenerationEffect = abilitySystem.GetAbility(gameSetting.enemyGenerationGraph.Data, 0);

            gameStartProcess = new List<Ability> { enemyGenerationEffect, turnStartEffect };
            turnEndProcess = new List<Ability> { turnEndEffect, enemyGenerationEffect, turnStartEffect };

            player = CreatePlayer(heroData);
            heroUnit = CreateHeroUnit(heroData);
            playArea = CreateStartDeck(heroData);
            playArea.ShuffleDrawPile();
            GameSetUp?.Invoke(heroUnit, playArea);
        }

        private void OnEventOccurred(IEventContext context)
        {
            if (context is PlayCardEvent playCardEvent)
            {
                playArea.MoveCardFromHandToDiscardPile(playCardEvent.card);
            }
            else if (context is DeathEvent deathEvent)
            {
                enemyUnits.Remove(deathEvent.target);
            }

            GameEventReceived?.Invoke(context);
        }

        private void ResolveEvent(IEventContext context)
        {
            abilitySystem.TryEnqueueAbility(heroUnit.Abilities, context);
            for (var i = 0; i < enemyUnits.Count; i++)
            {
                abilitySystem.TryEnqueueAbility(enemyUnits[i].Abilities, context);
            }
        }

        private Player CreatePlayer(HeroData heroData)
        {
            var player = new Player(abilitySystem);
            player.AddStat(StatId.MANA_RECOVER, 3);
            player.AddStat(StatId.DRAW, 5);

            player.Mana = 0;
            player.Coin = heroData.StartCoin;

            return player;
        }

        private Unit CreateHeroUnit(HeroData heroData)
        {
            var unit = new Unit(heroData, abilitySystem);
            unit.AddStat(StatId.HEALTH_MAX, heroData.StartHealth);
            unit.Health = heroData.StartHealth;
            return unit;
        }

        private Unit CreateEnemyUnit(EnemyData enemyData)
        {
            Unit unit = new Unit(enemyData, abilitySystem);
            unit.AddStat(StatId.HEALTH_MAX, enemyData.Health);
            unit.AddStat(StatId.ATTACK, enemyData.Attack);
            unit.Health = enemyData.Health;

            var startStatusIds = enemyData.StartStatusIds;
            for (var i = 0; i < startStatusIds.Count; i++)
            {
                StatusData statusData = gameDataManager.GetData<StatusData>(startStatusIds[i]);
                AbilityData abilityData = statusData.AbilityAsset.Data;
                for (var groupIndex = 0; groupIndex < abilityData.graphGroups.Count; groupIndex++)
                {
                    AbilityDataSource abilityDataSource = abilityData.CreateDataSource(groupIndex);
                    unit.AppendAbilityDataSource(abilityDataSource);
                }
            }
            return unit;
        }

        private PlayArea CreateStartDeck(HeroData heroData)
        {
            var startCardDatas = new List<CardData>();
            for (var i = 0; i < heroData.StartCardIds.Count; i++)
            {
                CardData cardData = gameDataManager.GetData<CardData>(heroData.StartCardIds[i]);
                startCardDatas.Add(cardData);
            }

            var startCards = new List<Card>();
            for (var i = 0; i < startCardDatas.Count; i++)
            {
                Card card = CreateCard(startCardDatas[i]);
                startCards.Add(card);
            }

            var playArea = new PlayArea(startCards);
            return playArea;
        }

        private Card CreateCard(CardData cardData)
        {
            Card card = new Card(cardData, abilitySystem);
            card.AddStat(StatId.COST, cardData.Cost);
            if (cardData.AbilityAsset != null)
            {
                AbilityData abilityData = cardData.AbilityAsset.Data;
                for (var groupIndex = 0; groupIndex < abilityData.graphGroups.Count; groupIndex++)
                {
                    AbilityDataSource abilityDataSource = abilityData.CreateDataSource(groupIndex);
                    card.AppendAbilityDataSource(abilityDataSource);
                }
            }

            return card;
        }

        public Unit GetEnemyUnit(int index)
        {
            if (index < 0 || index >= enemyUnits.Count)
            {
                return null;
            }

            return enemyUnits[index];
        }

        public void Start()
        {
            abilitySystem.TryEnqueueAndRunAbility(gameStartProcess, new SystemProcessPayload { game = this });
        }

        public IReadOnlyList<Unit> RandomGenerateEnemyGroup()
        {
            var result = new List<Unit>();

            EnemyGroupData groupData = groupDatas.RandomPickOne(generalRandom);
            for (var i = 0; i < groupData.EnemyIds.Count; i++)
            {
                var enemyData = gameDataManager.GetData<EnemyData>(groupData.EnemyIds[i]);
                Unit unit = CreateEnemyUnit(enemyData);
                enemyUnits.Add(unit);
                result.Add(unit);
            }

            return result;
        }

        public IReadOnlyList<Card> DrawCard(int count)
        {
            return playArea.Draw(count);
        }

        public int ReturnFromDiscardPile()
        {
            return playArea.ReturnFromDiscardPile();
        }

        public IReadOnlyList<Card> DiscardAllCards()
        {
            return playArea.DiscardAllCardsFromHand();
        }

        public void SelectCard(Card card)
        {
            var payload = new PlayCardNode.Payload
            {
                game = this,
                player = player,
                owner = heroUnit,
                card = card,
                random = generalRandom,
            };

            bool success = abilitySystem.TryEnqueueAndRunAbility(card.Abilities, payload);
            if (success)
            {
                CardSelected?.Invoke(card);
            }
        }

        private void OnChoiceOccurred(IChoiceContext context)
        {
            if (context is SingleTargetChoiceContext singleTargetChoiceContext)
            {
                Card card = singleTargetChoiceContext.actor as Card;
                if (card != null)
                {
                    ChoiceOccurred?.Invoke(card, context);
                }
            }
        }

        public void SelectTarget(Unit unit)
        {
            abilitySystem.Resume(new SingleTargetAnswerContext { unit = unit });
        }

        public void CancelSelection()
        {
            abilitySystem.Resume(new CancellationContext());
        }

        public int GetUnitStatusStack(Unit unit, int statusId)
        {
            StatusData statusData = gameDataManager.GetData<StatusData>(statusId);
            int stack = unit.GetStatusStack(statusData);
            return stack;
        }

        public void ApplyStatus(Unit unit, int statusId, int stack)
        {
            StatusData statusData = gameDataManager.GetData<StatusData>(statusId);
            unit.AddAbilityStack(statusData, stack);
        }

        public void RemoveStatus(Unit unit, int statusId, int stack)
        {
            StatusData statusData = gameDataManager.GetData<StatusData>(statusId);
            unit.RemoveAbilityStack(statusData, stack);
        }

        public void EndTurn()
        {
            abilitySystem.TryEnqueueAndRunAbility(turnEndProcess, new SystemProcessPayload { game = this });
        }
    }
}
