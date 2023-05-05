using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    public class GamePresenter
    {
        private readonly Game model;
        private readonly GameView view;

        public GamePresenter(Game model, GameView view)
        {
            this.model = model;
            this.view = view;
        }

        public void Initialize()
        {
            model.GameSetUp += OnGameSetUp;
            model.CardSelected += OnCardSelectedFromModel;
            model.GameEventReceived += OnGameEventReceived;
            model.ChoiceOccurred += OnChoiceOccurred;

            view.CardSelected += OnCardSelectedFromView;
            view.TargetSelected += OnTargetSelected;
            view.SelectionCanceled += OnSelectionCanceled;
            view.EndTurnRequested += OnEndTurnRequested;
        }

        private void OnGameSetUp(Unit heroUnit, PlayArea playArea)
        {
            Debug.Log("===== Game SetUp =====");
            Debug.Log($"Hero: {heroUnit.Name}");
            view.SpawnUnit(heroUnit);
        }

        private void OnCardSelectedFromModel(Card card)
        {
            Debug.Log($"===== Card Selected: {card} =====");
        }

        private void OnGameEventReceived(object payload)
        {
            Debug.Log($"===== Game Event: {payload}");
            view.AddGameEvent(payload);
        }

        private void OnChoiceOccurred(Card card, IChoiceContext context)
        {
            Debug.Log($"!!!!!!!!!! Choice Occurred => {context.GetType().Name}");
            var selectionData = new SelectionData
            {
                isTargetless = false,
            };
            view.SetUpCardSelection(card, selectionData);
        }

        private void OnCardSelectedFromView(Card card)
        {
            model.SelectCard(card);
        }

        private void OnTargetSelected(Unit unit)
        {
            model.SelectTarget(unit);
        }

        private void OnSelectionCanceled()
        {
            model.CancelSelection();
        }

        private void OnEndTurnRequested()
        {
            model.EndTurn();
        }

        public void SetUp(HeroData heroData)
        {
            model.SetUp(heroData);
        }

        public void Start()
        {
            model.Start();
        }

        public void Update()
        {

        }
    }
}
