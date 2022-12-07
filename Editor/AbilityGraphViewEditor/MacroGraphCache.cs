using System.Collections.Generic;
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
        private static readonly Dictionary<string, string> macroGuidToNameTable = new();

        internal static MacroLibrary Library => macroLibrary;
        internal static Dictionary<string, string> GuidToNameTable => macroGuidToNameTable;

        private static void Rebuild()
        {
            RebuildMacroLibrary();
            RebuildNameToGuidTable();
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

        private static void RebuildNameToGuidTable()
        {
            macroGuidToNameTable.Clear();

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(MacroGraphAsset)}");
            var assetPaths = new string[guids.Length];
            for (var i = 0; i < guids.Length; i++)
            {
                string guid = guids[i];

                assetPaths[i] = AssetDatabase.GUIDToAssetPath(guid);
                string name = assetPaths[i].Split('/')[^1];
                if (name.EndsWith(".asset"))
                {
                    name = name.Substring(0, name.Length - ".asset".Length);
                }

                macroGuidToNameTable.Add(guid, name);
            }
        }
    }
}
