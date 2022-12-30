using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi
{
    [CreateAssetMenu(fileName = "NewAbilityAsset", menuName = "Flexi/Ability Asset", order = 151)]
    public sealed class AbilityAsset : GraphAsset
    {
        [SerializeField]
        private List<BlackboardVariable> blackboard = new();
        [HideInInspector]
        [SerializeField]
        private List<string> graphJsons = new();

        private AbilityData abilityData;

        internal List<BlackboardVariable> Blackboard
        {
            get
            {
                return blackboard;
            }
            set
            {
                blackboard.Clear();
                if (value != null)
                {
                    // Clone each variable to prevent modify the source
                    for (var i = 0; i < value.Count; i++)
                    {
                        blackboard.Add(value[i].Clone());
                    }
                }
            }
        }

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

        private void OnValidate()
        {
            if (abilityData != null)
            {
                abilityData.name = name;
                abilityData.blackboard = blackboard;
                abilityData.graphJsons = graphJsons;
            }
        }
    }
}
