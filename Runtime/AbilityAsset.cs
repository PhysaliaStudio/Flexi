using System.Collections.Generic;
using UnityEngine;

namespace Physalia.AbilityFramework
{
    [CreateAssetMenu(fileName = "NewAbilityAsset", menuName = "Ability System/Ability Asset", order = 151)]
    public class AbilityAsset : ScriptableObject
    {
        [SerializeField]
        private List<BlackboardVariable> blackboard = new();
        [HideInInspector]
        [SerializeField]
        private List<string> graphJsons = new();

        private AbilityData abilityData;

        internal List<BlackboardVariable> Blackboard => blackboard;
        internal List<string> GraphJsons => graphJsons;

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

        internal void AddBlackboardVariable(BlackboardVariable variable)
        {
            blackboard.Add(variable);
        }

        internal void AddGraphJson(string graphJson)
        {
            graphJsons.Add(graphJson);
        }
    }
}
