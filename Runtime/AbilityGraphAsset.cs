using UnityEngine;

namespace Physalia.AbilityFramework
{
    [CreateAssetMenu(fileName = "GraphAsset", menuName = "Physalia/Ability System/Graph Asset", order = 1)]
    public sealed class AbilityGraphAsset : ScriptableObject
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
