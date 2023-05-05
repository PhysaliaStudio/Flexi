using UnityEditor;

namespace Physalia.Flexi.GraphViewEditor
{
    internal static class MacroLibraryCache
    {
        internal class Postprocessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
            {
                Rebuild();
            }
        }

        private static readonly MacroLibrary macroLibrary = new();

        internal static MacroLibrary Get()
        {
            return macroLibrary;
        }

        private static void Rebuild()
        {
            RebuildMacroLibrary();
        }

        private static void RebuildMacroLibrary()
        {
            macroLibrary.Clear();

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(MacroAsset)}");
            for (var i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                MacroAsset macroAsset = AssetDatabase.LoadAssetAtPath<MacroAsset>(assetPath);
                macroLibrary.Add(macroAsset.name, macroAsset.Text);
            }
        }
    }
}
