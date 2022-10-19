using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Physalia.AbilitySystem
{
    public static class StatDefinitionCodeGenerator
    {
        private const string WARNING_COMMENT =
@"// ###############################################
// #### AUTO GENERATED CODE, DO NOT MODIFIE!! ####
// ###############################################
";

        public static void Generate(StatDefinitionListAsset statDefinitionListAsset)
        {
            var sb = new StringBuilder();

            bool hasNamespace = !string.IsNullOrEmpty(statDefinitionListAsset.namespaceName);
            string tab = "    ";
            for (var i = 0; i < statDefinitionListAsset.stats.Count; i++)
            {
                StatDefinition statDefinition = statDefinitionListAsset.stats[i];
                string constantName = ToConstantName(statDefinition.Name);
                if (hasNamespace)
                {
                    sb.Append(tab);
                    sb.Append(tab);
                }
                else
                {
                    sb.Append(tab);
                }

                sb.Append($"public static readonly int {constantName} = {statDefinition.Id};");
                if (i != statDefinitionListAsset.stats.Count - 1)
                {
                    sb.AppendLine();
                }
            }

            string scriptText;
            if (hasNamespace)
            {
                scriptText =
$@"{WARNING_COMMENT}
namespace {statDefinitionListAsset.namespaceName}
{{
{tab}public static class {statDefinitionListAsset.className}
{tab}{{
{sb}
{tab}}}
}}
";
            }
            else
            {
                scriptText =
$@"{WARNING_COMMENT}
public static class {statDefinitionListAsset.className}
{{
{sb}
}}
";
            }

            var assetPath = EditorUtility.SaveFilePanelInProject("Save StatId cs file", statDefinitionListAsset.className, "cs",
                "Please enter a file name to save the script to", statDefinitionListAsset.scriptAssetPath);
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            statDefinitionListAsset.scriptAssetPath = assetPath;
            EditorUtility.SetDirty(statDefinitionListAsset);
            AssetDatabase.SaveAssetIfDirty(statDefinitionListAsset);

            CreateScriptAsset(scriptText, assetPath);
        }

        private static string ToConstantName(string name)
        {
            name = name.Replace(' ', '_');
            for (var i = 0; i < name.Length - 1; i++)
            {
                if (char.IsLower(name[i]) && char.IsUpper(name[i + 1]))
                {
                    name = name.Insert(i + 1, "_");
                    i++;
                }
            }

            name = name.ToUpper();
            return name;
        }

        private static void CreateScriptAsset(string text, string path)
        {
            var root = new DirectoryInfo(Application.dataPath).Parent;
            string fullPath = Path.Combine(root.FullName, path).Replace('/', '\\');
            using (var sw = new StreamWriter(fullPath))
            {
                sw.Write(text);
            }

            AssetDatabase.Refresh();
        }
    }
}
