using System;
using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public interface IUnitRepository
    {
        IReadOnlyList<Unit> Enemies { get; }
    }

    public class Game : IFlexiEventResolver, IFlexiStatRefreshResolver, IUnitRepository
    {
        public event Action<Unit, PlayArea> GameSetUp;
        public event Action<Card> CardSelected;
        public event Action<object> GameEventReceived;
        public event Action<Card> ChoiceOccurred;

        private readonly AssetManager assetManager;
        private readonly GameDataManager gameDataManager;
        private readonly GameSetting gameSetting;
        private readonly FlexiCore flexiCore;
        private readonly DefaultModifierHandler modifierHandler = new();

        private readonly System.Random generalRandom = new();
        private readonly HashSet<EnemyGroupData> groupDatas = new();

        private List<DefaultAbilityContainer> gameStartProcess;
        private List<DefaultAbilityContainer> turnEndProcess;

        private Player player;
        private Unit heroUnit;
        private PlayArea playArea;
        private readonly List<Unit> enemyUnits = new();
        private readonly List<Card> cards = new();

        public Player Player => player;
        public IReadOnlyList<Unit> Enemies => enemyUnits;
        public System.Random Random => generalRandom;

        public Game(AssetManager assetManager, GameDataManager gameDataManager, GameSetting gameSetting)
        {
            this.assetManager = assetManager;
            this.gameDataManager = gameDataManager;
            this.gameSetting = gameSetting;

            var builder = new FlexiCoreBuilder();
            builder.SetEventResolver(this);
            builder.SetStatRefreshResolver(this);
            flexiCore = builder.Build();

            MacroAsset[] macroAssets = assetManager.LoadAll<MacroAsset>("AbilityGraphs");
            for (var i = 0; i < macroAssets.Length; i++)
            {
                MacroAsset macroAsset = macroAssets[i];
                flexiCore.LoadMacroGraph(macroAsset.name, macroAsset);
            }
        }

        public void SetUp(HeroData heroData)
        {
            var groupDataTable = gameDataManager.GetTable<EnemyGroupData>();
            foreach (EnemyGroupData groupData in groupDataTable.Values)
            {
                groupDatas.Add(groupData);
            }

            AbilityData turnStartEffect = gameSetting.turnStartGraph.Data;
            AbilityData turnEndEffect = gameSetting.turnEndGraph.Data;
            AbilityData enemyGenerationEffect = gameSetting.enemyGenerationGraph.Data;

            flexiCore.LoadAbility(turnStartEffect, 0, 1);
            flexiCore.LoadAbility(turnEndEffect, 0, 1);
            flexiCore.LoadAbility(enemyGenerationEffect, 0, 1);
            CacheAllStatusAbility();

            gameStartProcess = new List<DefaultAbilityContainer> {
                new(this, enemyGenerationEffect, 0),
                new(this, turnStartEffect, 0),
            };
            turnEndProcess = new List<DefaultAbilityContainer> {
                new(this, turnEndEffect, 0),
                new(this, enemyGenerationEffect, 0),
                new(this, turnStartEffect, 0),
            };

            player = CreatePlayer(heroData);
            heroUnit = CreateHeroUnit(heroData);
            playArea = CreateStartDeck(heroData);
            playArea.ShuffleDrawPile();
            GameSetUp?.Invoke(heroUnit, playArea);
        }

        private void CacheAllStatusAbility()
        {
            IReadOnlyDictionary<int, StatusData> table = gameDataManager.GetTable<StatusData>();
            foreach (StatusData statusData in table.Values)
            {
                AbilityData abilityData = statusData.AbilityAsset.Data;
                for (var groupIndex = 0; groupIndex < abilityData.graphGroups.Count; groupIndex++)
                {
                    flexiCore.LoadAbility(abilityData, groupIndex);
                }
            }
        }

        #region Implement IFlexiEventResolver
        public void OnEventReceived(IEventContext eventContext)
        {
            if (eventContext is PlayCardEvent playCardEvent)
            {
                playArea.MoveCardFromHandToDiscardPile(playCardEvent.card);
            }
            else if (eventContext is DeathEvent deathEvent)
            {
                enemyUnits.Remove(deathEvent.target);
            }

            GameEventReceived?.Invoke(eventContext);
        }

        public void ResolveEvent(FlexiCore flexiCore, IEventContext eventContext)
        {
            flexiCore.TryEnqueueAbility(heroUnit.AbilityContainers, eventContext);
            for (var i = 0; i < enemyUnits.Count; i++)
            {
                flexiCore.TryEnqueueAbility(enemyUnits[i].AbilityContainers, eventContext);
            }
        }
        #endregion

        #region Implement IFlexiStatRefreshResolver
        public IReadOnlyList<StatOwner> CollectStatRefreshOwners()
        {
            var result = new List<StatOwner>();
            result.Add(heroUnit);
            result.AddRange(enemyUnits);
            result.AddRange(cards);
            return result;
        }

        public IReadOnlyList<AbilityContainer> CollectStatRefreshContainers()
        {
            var result = new List<AbilityContainer>();
            result.AddRange(heroUnit.AbilityContainers);
            for (var i = 0; i < enemyUnits.Count; i++)
            {
                result.AddRange(enemyUnits[i].AbilityContainers);
            }
            for (var i = 0; i < cards.Count; i++)
            {
                result.AddRange(cards[i].AbilityContainers);
            }
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

        private Player CreatePlayer(HeroData heroData)
        {
            var player = new Player();
            player.AddStat(StatId.MANA_RECOVER, 3);
            player.AddStat(StatId.DRAW, 5);

            player.Mana = 0;
            player.Coin = heroData.StartCoin;

            return player;
        }

        private Unit CreateHeroUnit(HeroData heroData)
        {
            var unit = new Unit(heroData);
            unit.AddStat(StatId.HEALTH_MAX, heroData.StartHealth);
            unit.Health = heroData.StartHealth;
            return unit;
        }

        private Unit CreateEnemyUnit(EnemyData enemyData)
        {
            Unit unit = new Unit(enemyData);
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
                    var container = new DefaultAbilityContainer(this, abilityData, groupIndex);
                    unit.AppendAbilityContainer(container);

                    if (!flexiCore.IsAbilityLoaded(abilityData, groupIndex))
                    {
                        flexiCore.LoadAbility(abilityData, groupIndex);
                    }
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
                cards.Add(card);
                startCards.Add(card);
            }

            var playArea = new PlayArea(startCards);
            return playArea;
        }

        private Card CreateCard(CardData cardData)
        {
            Card card = new Card(cardData);
            card.AddStat(StatId.COST, cardData.Cost);
            if (cardData.AbilityAsset != null)
            {
                AbilityData abilityData = cardData.AbilityAsset.Data;
                for (var groupIndex = 0; groupIndex < abilityData.graphGroups.Count; groupIndex++)
                {
                    var container = new DefaultAbilityContainer(this, abilityData, groupIndex);
                    card.AppendAbilityContainer(container);

                    if (!flexiCore.IsAbilityLoaded(abilityData, groupIndex))
                    {
                        flexiCore.LoadAbility(abilityData, groupIndex);
                    }
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
            _ = flexiCore.TryEnqueueAbility(gameStartProcess, new SystemProcessContext());
            flexiCore.Run();
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
            var context = new PlayCardNode.Context
            {
                owner = heroUnit,
                card = card,
            };

            bool success = flexiCore.TryEnqueueAbility(card.AbilityContainers, context);
            if (success)
            {
                flexiCore.Run();
                CardSelected?.Invoke(card);
            }
        }

        public void StartSingleTargetChoice(Card card, UnitType unitType)
        {
            if (card != null)
            {
                Debug.Log($"!!!!!!!!!! Start Single Choice");
                ChoiceOccurred?.Invoke(card);
            }
        }

        public void SelectTarget(Unit unit)
        {
            flexiCore.Resume(new SingleTargetAnswerContext { unit = unit });
        }

        public void CancelSelection()
        {
            flexiCore.Resume(new CancellationContext());
        }

        public void Damage(Unit attacker, IReadOnlyList<Unit> targets, int amount)
        {
            if (targets.Count == 0)
            {
                return;
            }

            // Damage
            for (var i = 0; i < targets.Count; i++)
            {
                targets[i].Health -= amount;
                if (targets[i].Health < 0)
                {
                    targets[i].Health = 0;
                }
            }

            flexiCore.EnqueueEvent(new DamageContext
            {
                attacker = attacker,
                targets = new List<Unit>(targets),
                amount = amount,
            });

            // Check Death
            for (var i = 0; i < targets.Count; i++)
            {
                if (targets[i].Health <= 0)
                {
                    flexiCore.EnqueueEvent(new DeathEvent
                    {
                        target = targets[i],
                    });
                }
            }
        }

        public int GetUnitStatusStack(Unit unit, int statusId)
        {
            StatusData statusData = gameDataManager.GetData<StatusData>(statusId);
            int stack = unit.GetStatusStack(statusData);
            return stack;
        }

        public void ApplyStatus(Unit unit, int statusId, int stack)
        {
            if (stack <= 0)
            {
                Debug.LogWarning($"ApplyStatus failed! Given stack is less or equal to 0 (stack = {stack})");
                return;
            }

            StatusData statusData = gameDataManager.GetData<StatusData>(statusId);
            if (unit.GetStatusStack(statusData) == 0)
            {
                AbilityData abilityData = statusData.AbilityAsset.Data;
                if (abilityData.graphGroups.Count > 0)
                {
                    var container = new DefaultAbilityContainer(this, abilityData, 0);
                    unit.AppendStatusContainer(statusData, container);
                }
            }
            unit.AddStatusStack(statusData, stack);
        }

        public void RemoveStatus(Unit unit, int statusId, int stack)
        {
            if (stack <= 0)
            {
                Debug.LogWarning($"RemoveStatus failed! Given stack is less or equal to 0 (stack = {stack})");
                return;
            }

            StatusData statusData = gameDataManager.GetData<StatusData>(statusId);
            unit.RemoveStatusStack(statusData, stack);
            if (unit.GetStatusStack(statusData) == 0)
            {
                unit.RemoveStatusContainer(statusData);
            }
        }

        public void EndTurn()
        {
            _ = flexiCore.TryEnqueueAbility(turnEndProcess, new SystemProcessContext());
            flexiCore.Run();
        }
    }
}
