using UnityEngine;

namespace Physalia.Flexi
{
    [CreateAssetMenu(fileName = "NewMacroAsset", menuName = "Flexi/Macro Asset", order = 152)]
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
