using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    [CreateAssetMenu(fileName = "HeroData", menuName = "CardGameSample/HeroData")]
    public class HeroData : ScriptableObject, IUnitData
    {
        [SerializeField]
        private int id;
        [SerializeField]
        private string heroName;
        [SerializeField]
        private UnitAvatarAnimation avatarPrefab;

        [Space]
        [SerializeField]
        private int startHealth;
        [SerializeField]
        private int startCoin;
        [SerializeField]
        private List<int> startCardIds = new();

        public int Id => id;
        public string Name => heroName;
        public UnitAvatarAnimation AvatarPrefab => avatarPrefab;

        public UnitType UnitType => UnitType.PLAYER;
        public int StartHealth => startHealth;
        public int StartCoin => startCoin;
        public IReadOnlyList<int> StartCardIds => startCardIds;
    }
}
