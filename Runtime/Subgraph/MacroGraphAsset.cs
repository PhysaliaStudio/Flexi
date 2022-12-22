using UnityEngine;

namespace Physalia.AbilityFramework
{
    [CreateAssetMenu(fileName = "MacroGraphAsset", menuName = "Ability System/Macro Graph Asset", order = 152)]
    public sealed class MacroGraphAsset : ScriptableObject
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
