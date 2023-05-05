using UnityEditor;

namespace Physalia.Flexi.GraphViewEditor
{
    public static class AssetVersionConverter
    {
        [MenuItem("Tools/Flexi/Upgrade Assets Version")]
        private static void ConvertFromSelectedObjects()
        {
            var objects = Selection.objects;
            for (var i = 0; i < objects.Length; i++)
            {
                if (objects[i] is AbilityAsset abilityAsset)
                {
                    UpgradeAbilityAsset(abilityAsset);
                }
                else if (objects[i] is MacroAsset macroAsset)
                {
                    UpgradeMacroAsset(macroAsset);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void UpgradeAbilityAsset(AbilityAsset abilityAsset)
        {
            for (var i = 0; i < abilityAsset.GraphJsons.Count; i++)
            {
                abilityAsset.GraphJsons[i] = abilityAsset.GraphJsons[i].Replace("Physalia.AbilityFramework", "Physalia.Flexi");
            }
            EditorUtility.SetDirty(abilityAsset);
        }

        private static void UpgradeMacroAsset(MacroAsset macroAsset)
        {
            macroAsset.Text = macroAsset.Text.Replace("Physalia.AbilityFramework", "Physalia.Flexi");
            EditorUtility.SetDirty(macroAsset);
        }
    }
}
