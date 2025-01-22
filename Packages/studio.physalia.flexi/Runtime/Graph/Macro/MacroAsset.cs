using UnityEngine;

namespace Physalia.Flexi
{
    [CreateAssetMenu(fileName = "NewMacroAsset", menuName = "Flexi/Macro Asset", order = 152)]
    public sealed class MacroAsset : GraphAsset
    {
        [SerializeField]
        [HideInInspector]
        private string json;

        internal string Json { get => json; set => json = value; }
    }
}
