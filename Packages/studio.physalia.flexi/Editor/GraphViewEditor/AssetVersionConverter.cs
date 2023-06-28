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
            if (abilityAsset.GraphJsons.Count > 0)
            {
                var newGroup = new AbilityGraphGroup();
                newGroup.graphs.AddRange(abilityAsset.GraphJsons);
                abilityAsset.GraphGroups.Add(newGroup);

                abilityAsset.GraphJsons.Clear();
            }

            for (var groupIndex = 0; groupIndex < abilityAsset.GraphGroups.Count; groupIndex++)
            {
                AbilityGraphGroup group = abilityAsset.GraphGroups[groupIndex];
                for (var graphIndex = 0; graphIndex < group.graphs.Count; graphIndex++)
                {
                    group.graphs[graphIndex] = group.graphs[graphIndex].Replace("Physalia.AbilityFramework", "Physalia.Flexi");
                }
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
