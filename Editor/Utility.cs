using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Physalia.Flexi
{
    public static class Utility
    {
        public static string AssetPathToFullPath(string assetPath)
        {
            DirectoryInfo root = new DirectoryInfo(Application.dataPath).Parent;
            string fullPath = Path.Combine(root.FullName, assetPath).Replace('/', '\\');
            return fullPath;
        }

        public static bool OpenScriptOfType(Type type)
        {
            MonoScript monoScript = GetMonoScriptFromType(type);
            if (monoScript != null)
            {
                AssetDatabase.OpenAsset(monoScript);
                return true;
            }

            Debug.LogError($"Can't open script of type '{type.Name}', because a script with the same name does not exist.");
            return false;
        }

        private static MonoScript GetMonoScriptFromType(Type type)
        {
            if (type == null)
            {
                return null;
            }

            string typeName = type.Name;
            if (type.IsGenericType)
            {
                type = type.GetGenericTypeDefinition();
                typeName = typeName.Substring(0, typeName.IndexOf('`'));
            }

            return AssetDatabase.FindAssets($"{typeName} t:MonoScript")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
                .FirstOrDefault(x => x != null && x.GetClass() == type);
        }
    }
}
