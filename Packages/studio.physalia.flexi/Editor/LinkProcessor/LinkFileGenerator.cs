using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;

namespace Physalia.Flexi
{
    public class LinkFileGenerator
    {
        private static readonly HashSet<string> SkippedAssemblyNames = new()
        {
            EditorConst.AssemblyNameMain,
            EditorConst.AssemblyNameEditorTests,
            EditorConst.AssemblyNamePerformanceTests,
        };

        [MenuItem(EditorConst.MenuFolder + "Generate link.xml", priority = 1003)]
        private static void Generate()
        {
            string assetPath = ShowSaveFilePanel();
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }

            Generate(assetPath);
        }

        public static void Generate(string assetPath)
        {
            string content = GenerateContent();
            string fullPath = Utility.AssetPathToFullPath(assetPath);

            // Note: Brackets are necessary since we need to close the stream before refreshing AssetDatabase
            using (var sw = new StreamWriter(fullPath))
            {
                sw.Write(content);
            }

            AssetDatabase.Refresh();
        }

        private static string ShowSaveFilePanel()
        {
            string assetPath;

            do
            {
                assetPath = EditorUtility.SaveFilePanelInProject("Save link.xml", "link", "xml",
                    "Please seperate from other link files", "Assets");
            }
            while (!IsValidPath(assetPath));

            return assetPath;

            static bool IsValidPath(string assetPath)
            {
                if (string.IsNullOrEmpty(assetPath))
                {
                    return true;
                }

                if (Path.GetFileName(assetPath) != "link.xml")
                {
                    EditorUtility.DisplayDialog("Invalid Name", "The file name should be 'link.xml'.", "OK");
                    return false;
                }

                // EditorUtility.SaveFilePanelInProject has already helped checking the directory, so we don't need to do it.

                return true;
            }
        }

        private static string GenerateContent()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<linker>");

            var nodeTypes = new List<Type>();
            foreach (Assembly assembly in ReflectionUtilities.GetAssemblies())
            {
                // Skip assemblies in the package
                string assemblyName = assembly.GetName().Name;
                if (SkippedAssemblyNames.Contains(assemblyName))
                {
                    continue;
                }

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Node)))
                    {
                        nodeTypes.Add(type);
                    }
                }

                if (nodeTypes.Count == 0)
                {
                    continue;
                }

                sb.AppendLine($"  <assembly fullname=\"{assemblyName}\">");
                for (var i = 0; i < nodeTypes.Count; i++)
                {
                    sb.AppendLine($"    <type fullname=\"{nodeTypes[i].FullName}\" preserve=\"all\" />");
                }
                sb.AppendLine("  </assembly>");

                nodeTypes.Clear();
            }

            sb.AppendLine("</linker>");

            string content = sb.ToString();
            return content;
        }
    }
}
