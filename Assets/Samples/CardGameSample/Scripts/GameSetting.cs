using UnityEngine;

namespace Physalia.Flexi.Samples.CardGame
{
    [CreateAssetMenu(fileName = "GameSetting", menuName = "CardGameSample/GameSetting")]
    public class GameSetting : ScriptableObject
    {
        public AbilityAsset turnStartGraph;
        public AbilityAsset turnEndGraph;
        public AbilityAsset enemyGenerationGraph;
    }
}
