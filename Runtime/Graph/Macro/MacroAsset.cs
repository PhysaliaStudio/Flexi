using UnityEngine;

namespace Physalia.AbilityFramework
{
    [CreateAssetMenu(fileName = "NewMacroAsset", menuName = "Ability System/Macro Asset", order = 152)]
    public sealed class MacroAsset : GraphAsset
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
