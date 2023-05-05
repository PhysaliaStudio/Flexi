using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    [CreateAssetMenu(fileName = "StatusData", menuName = "CardGameSample/StatusData")]
    public class StatusData : ScriptableObject, IHasGameId
    {
        [SerializeField]
        private int id;
        [SerializeField]
        private string statusName = "";
        [SerializeField]
        private AbilityAsset abilityAsset;

        public int Id => id;
        public string Name => statusName;
        public AbilityAsset AbilityAsset => abilityAsset;
    }
}
