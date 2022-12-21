using System.Collections.Generic;
using UnityEngine;

namespace Physalia.AbilityFramework
{
    [CreateAssetMenu(fileName = "NewAbilityAsset", menuName = "Ability System/Ability Asset", order = 151)]
    public class AbilityAsset : ScriptableObject
    {
        [SerializeField]
        private List<BlackboardVariable> blackboard = new();
        [SerializeField]
        private List<string> graphJsons = new();

        private AbilityData abilityData;

        public AbilityData Data
        {
            get
            {
                if (abilityData == null)
                {
                    abilityData = new AbilityData
                    {
                        name = name,
                        blackboard = blackboard,
                        graphJsons = graphJsons,
                    };
                }

                return abilityData;
            }
        }
    }
}
