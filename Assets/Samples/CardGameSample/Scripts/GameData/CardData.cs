using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    [CreateAssetMenu(fileName = "CardData", menuName = "CardGameSample/CardData")]
    public class CardData : ScriptableObject, IHasGameId
    {
        [SerializeField]
        private int id;
        [SerializeField]
        private int cost;
        [SerializeField]
        private string cardName;
        [SerializeField]
        private string text;
        [SerializeField]
        private AbilityAsset abilityAsset;

        public int Id => id;
        public int Cost => cost;
        public string Name => cardName;
        public string Text => text;
        public AbilityAsset AbilityAsset => abilityAsset;
    }
}
