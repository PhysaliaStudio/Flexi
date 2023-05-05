using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "CardGameSample/EnemyData")]
    public class EnemyData : ScriptableObject, IUnitData
    {
        [SerializeField]
        private int id;
        [SerializeField]
        private string enemyName = "";
        [SerializeField]
        private UnitAvatarAnimation avatarPrefab;

        [Space]
        [SerializeField]
        private UnitType unitType = UnitType.NORMAL;
        [SerializeField]
        private int health;
        [SerializeField]
        private int attack;
        [SerializeField]
        private List<int> startStatusIds = new();

        public int Id => id;
        public string Name => enemyName;
        public UnitAvatarAnimation AvatarPrefab => avatarPrefab;

        public UnitType UnitType => unitType;
        public int Health => health;
        public int Attack => attack;
        public IReadOnlyList<int> StartStatusIds => startStatusIds;
    }
}
