using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    [CreateAssetMenu(fileName = "EnemyGroupData", menuName = "CardGameSample/EnemyGroupData")]
    public class EnemyGroupData : ScriptableObject, IHasGameId
    {
        [SerializeField]
        private int id;
        [SerializeField]
        private List<int> enemyIds = new();

        public int Id => id;
        public IReadOnlyList<int> EnemyIds => enemyIds;
    }
}
