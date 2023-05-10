using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Physalia.Flexi
{
    public static class StatDefinitionCodeGenerator
    {
        private const string WARNING_COMMENT =
@"// ###############################################
// #### AUTO GENERATED CODE, DO NOT MODIFY!! #####
// ###############################################
";
        private const string DEFAULT_CLASS_NAME = "StatId";

        public static void Generate(StatDefinitionListAsset statDefinitionListAsset)
        {
            string assetPath = ShowSaveDialog(statDefinitionListAsset);
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

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

                sb.Append($"public const int {constantName} = {statDefinition.Id};");
                if (i != statDefinitionListAsset.stats.Count - 1)
                {
                    sb.AppendLine();
                }
            }

            string className = PathToFileName(assetPath);
            string scriptText;
            if (hasNamespace)
            {
                scriptText =
$@"{WARNING_COMMENT}
namespace {statDefinitionListAsset.namespaceName}
{{
{tab}public static class {className}
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
public static class {className}
{{
{sb}
}}
";
            }

            statDefinitionListAsset.scriptGuid = AssetDatabase.AssetPathToGUID(assetPath);
            EditorUtility.SetDirty(statDefinitionListAsset);
            AssetDatabase.SaveAssetIfDirty(statDefinitionListAsset);

            CreateScriptAsset(scriptText, assetPath);
        }

        private static string ShowSaveDialog(StatDefinitionListAsset statDefinitionListAsset)
        {
            string scriptAssetPath = AssetDatabase.GUIDToAssetPath(statDefinitionListAsset.scriptGuid);

            string className;
            if (string.IsNullOrEmpty(scriptAssetPath))
            {
                className = DEFAULT_CLASS_NAME;
            }
            else
            {
                className = PathToFileName(scriptAssetPath);
            }

            string folderPath = GetStartFolderPathFromAsset(statDefinitionListAsset);
            var assetPath = EditorUtility.SaveFilePanelInProject("Save StatId cs file", className, "cs",
                "Please enter a file name to save the script to", folderPath);
            return assetPath;
        }

        private static string PathToFileName(string path)
        {
            var fileInfo = new FileInfo(path);
            string fileNameWithExt = fileInfo.Name;
            string ext = fileInfo.Extension;
            string className = fileNameWithExt.Remove(fileNameWithExt.Length - ext.Length, ext.Length);
            return className;
        }

        private static string GetStartFolderPathFromAsset(StatDefinitionListAsset asset)
        {
            string scriptAssetPath = AssetDatabase.GUIDToAssetPath(asset.scriptGuid);
            if (string.IsNullOrEmpty(scriptAssetPath))
            {
                return "Assets/";
            }

            // If there is path but the file doesn't exist, it's because file movement or similar reasons.
            var fileInfo = new FileInfo(scriptAssetPath);
            if (!fileInfo.Exists)
            {
                return "Assets/";
            }

            string folderPath = fileInfo.Directory.FullName.Replace('\\', '/');
            return folderPath;
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

        private static void CreateScriptAsset(string text, string assetPath)
        {
            string fullPath = Utility.AssetPathToFullPath(assetPath);

            // Note: Brackets are necessary since we need to close the stream before refreshing AssetDatabase
            using (var sw = new StreamWriter(fullPath))
            {
                sw.Write(text);
            }

            AssetDatabase.Refresh();
        }
    }
}
