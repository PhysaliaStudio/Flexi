using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Physalia.AbilityFramework
{
    public static class StatDefinitionCodeGenerator
    {
        private const string WARNING_COMMENT =
@"// ###############################################
// #### AUTO GENERATED CODE, DO NOT MODIFIE!! ####
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

                sb.Append($"public static readonly int {constantName} = {statDefinition.Id};");
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

            statDefinitionListAsset.scriptAssetPath = assetPath;
            EditorUtility.SetDirty(statDefinitionListAsset);
            AssetDatabase.SaveAssetIfDirty(statDefinitionListAsset);

            CreateScriptAsset(scriptText, assetPath);
        }

        private static string ShowSaveDialog(StatDefinitionListAsset statDefinitionListAsset)
        {
            string className;
            if (string.IsNullOrEmpty(statDefinitionListAsset.scriptAssetPath))
            {
                className = DEFAULT_CLASS_NAME;
            }
            else
            {
                className = PathToFileName(statDefinitionListAsset.scriptAssetPath);
            }

            string folderPath = !string.IsNullOrEmpty(statDefinitionListAsset.scriptAssetPath) ?
                AssetPathToFolderPath(statDefinitionListAsset.scriptAssetPath) : "Assets/";

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

        private static string AssetPathToFolderPath(string assetPath)
        {
            var fileInfo = new FileInfo(assetPath);
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
