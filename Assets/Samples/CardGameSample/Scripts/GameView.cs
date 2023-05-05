using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Physalia.Flexi.Samples.CardGame
{
    public class GameView : MonoBehaviour
    {
        public event Action<Card> CardSelected;
        public event Action<Unit> TargetSelected;
        public event Action SelectionCanceled;
        public event Action EndTurnRequested;

        [SerializeField]
        private Camera mainCamera;
        [SerializeField]
        private Canvas canvas;

        [Space]
        [SerializeField]
        private GameSpace gameSpace;
        [SerializeField]
        private HandUI handUI;
        [SerializeField]
        private TMP_Text mana;
        [SerializeField]
        private Button endTurnButton;

        [Space]
        [SerializeField]
        private PopUpNumber popUpNumberPrefab;
        [SerializeField]
        private float popUpNumberFadeTime = 1f;

        private readonly Queue<object> cachedGameEvents = new();
        private GameObjectPool<PopUpNumber> popUpNumberPool;
        private UniTask currentTask = UniTask.CompletedTask;

        private void Awake()
        {
            popUpNumberPool = new GameObjectPool<PopUpNumber>(popUpNumberPrefab, canvas.transform, "", 4, 10);
        }

        private void Start()
        {
            handUI.CardSelected += OnCardSelected;
            handUI.TargetSelected += OnTargetSelected;
            handUI.SelectionCanceled += OnSelectionCanceled;

            endTurnButton.onClick.AddListener(() => EndTurnRequested?.Invoke());
        }

        private void Update()
        {
            if (!currentTask.Status.IsCompleted())
            {
                return;
            }

            if (cachedGameEvents.Count > 0)
            {
                currentTask = UniTask.Create(HandleNextEvent);
            }
        }

        private void OnCardSelected(Card card)
        {
            CardSelected?.Invoke(card);
        }

        private void OnTargetSelected(Unit unit)
        {
            TargetSelected?.Invoke(unit);
        }

        private void OnSelectionCanceled()
        {
            SelectionCanceled?.Invoke();
        }

        public void SetUpCardSelection(Card card, SelectionData selectionData)
        {
            handUI.SetUpCardSelection(card, selectionData);
        }

        public void SpawnUnit(Unit unit)
        {
            gameSpace.CreateAvatar(unit);
        }

        public void SpawnUnits(IReadOnlyList<Unit> units)
        {
            Debug.Log("===== Unit Spawned =====");
            for (var i = 0; i < units.Count; i++)
            {
                Debug.Log($"Unit: {units[i].Name}");
            }

            for (var i = 0; i < units.Count; i++)
            {
                gameSpace.CreateAvatar(units[i]);
            }
        }

        public void AddGameEvent(object gameEvent)
        {
            cachedGameEvents.Enqueue(gameEvent);
        }

        private async UniTask HandleNextEvent()
        {
            object gameEvent = cachedGameEvents.Dequeue();
            if (gameEvent is DrawCardEvent drawCardEvent)
            {
                await handUI.DrawCardsAnimation(drawCardEvent.cards);
            }
            else if (gameEvent is PlayCardEvent playCardEvent)
            {
                _ = handUI.PlayCardAnimation(playCardEvent.card);
            }
            else if (gameEvent is ReturnDiscardPileEvent)
            {
                await handUI.ReturnFromDiscardPileAnimation();
            }
            else if (gameEvent is DiscardCardEvent discardCardEvent)
            {
                await handUI.DiscardCardsAnimation(discardCardEvent.cards);
            }
            else if (gameEvent is ManaChangeEvent manaChangeEvent)
            {
                HandleManaChangeEvent(manaChangeEvent);
            }
            else if (gameEvent is HealEvent healEvent)
            {
                await HandleHealEvent(healEvent);
            }
            else if (gameEvent is DamageEvent damageEvent)
            {
                await HandleDamageEvent(damageEvent);
            }
            else if (gameEvent is UnitSpawnedEvent unitSpawnedEvent)
            {
                await HandleUnitSpawnedEvent(unitSpawnedEvent);
            }
            else if (gameEvent is DeathEvent deathEvent)
            {
                await HandleDeathEvent(deathEvent);
            }
        }

        private void HandleManaChangeEvent(ManaChangeEvent evt)
        {
            mana.text = evt.newAmount.ToString();
        }

        private async UniTask HandleHealEvent(HealEvent healEvent)
        {
            for (var i = 0; i < healEvent.targets.Count; i++)
            {
                Unit unit = healEvent.targets[i];
                UnitAvatar unitAvatar = gameSpace.GetAvatar(unit);
                unitAvatar.Heal(healEvent.amount);

                PopUpNumber popUpNumber = popUpNumberPool.Get();

                Vector3 followWorldPoint = unitAvatar.transform.position + new Vector3(0f, 1.5f, 0f);
                Vector2 localPoint = mainCamera.WorldToLocalPointInRectangle(canvas.GetRectTransform(), followWorldPoint);
                popUpNumber.GetRectTransform().anchoredPosition = localPoint;

                popUpNumber.Init();
                popUpNumber.SetNumber(healEvent.amount);
                popUpNumber.SetColor(Color.green);
                popUpNumber.Play(popUpNumberPool, popUpNumberFadeTime);
            }

            await UniTask.Delay((int)(popUpNumberFadeTime * 500));
        }

        private async UniTask HandleDamageEvent(DamageEvent damageEvent)
        {
            for (var i = 0; i < damageEvent.targets.Count; i++)
            {
                Unit unit = damageEvent.targets[i];
                UnitAvatar unitAvatar = gameSpace.GetAvatar(unit);
                unitAvatar.Damage(damageEvent.amount);

                PopUpNumber popUpNumber = popUpNumberPool.Get();

                Vector3 followWorldPoint = unitAvatar.PopupHook.position;
                Vector2 localPoint = mainCamera.WorldToLocalPointInRectangle(canvas.GetRectTransform(), followWorldPoint);
                popUpNumber.GetRectTransform().anchoredPosition = localPoint;

                popUpNumber.Init();
                popUpNumber.SetNumber(-damageEvent.amount);
                popUpNumber.Play(popUpNumberPool, popUpNumberFadeTime);
            }

            await UniTask.Delay((int)(popUpNumberFadeTime * 500));
        }

        private async UniTask HandleUnitSpawnedEvent(UnitSpawnedEvent unitSpawnedEvent)
        {
            SpawnUnits(unitSpawnedEvent.units);
            await UniTask.Delay(500);
        }

        private async UniTask HandleDeathEvent(DeathEvent deathEvent)
        {
            if (deathEvent.target is Unit unit)
            {
                gameSpace.RemoveAvatar(unit);
                await UniTask.Delay(500);
            }
        }
    }
}
