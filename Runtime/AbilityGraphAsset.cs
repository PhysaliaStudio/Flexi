using UnityEngine;

namespace Physalia.AbilityFramework
{
    [CreateAssetMenu(fileName = "GraphAsset", menuName = "Ability System/Graph Asset", order = 151)]
    public class AbilityGraphAsset : ScriptableObject
    {
        [SerializeField]
        [HideInInspector]
        private string text;

        internal string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }
    }
}
