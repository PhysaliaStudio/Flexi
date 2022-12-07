using UnityEditor;

namespace Physalia.AbilityFramework.GraphViewEditor
{
    internal static class MacroGraphCache
    {
        internal class Postprocessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
            {
                Rebuild();
            }
        }

        private static readonly MacroLibrary macroLibrary = new();

        internal static MacroLibrary Library => macroLibrary;

        private static void Rebuild()
        {
            RebuildMacroLibrary();
        }

        private static void RebuildMacroLibrary()
        {
            macroLibrary.Clear();

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(MacroGraphAsset)}");
            for (var i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                MacroGraphAsset macroGraphAsset = AssetDatabase.LoadAssetAtPath<MacroGraphAsset>(assetPath);
                macroLibrary.Add(guids[i], macroGraphAsset.Text);
            }
        }
    }
}
