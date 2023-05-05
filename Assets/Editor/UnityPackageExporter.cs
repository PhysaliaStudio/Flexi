using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

public static class UnityPackageExporter
{
    [MenuItem("Tools/Export UnityPackage", priority = 10000)]
    private static void ExportPackage()
    {
        const string AssemblyName = "Physalia.Flexi";
        const string PackageName = "Flexi";

        // Find the package info
        PackageInfo packageInfo = FindPackageInfo(AssemblyName);
        if (packageInfo == null)
        {
            return;
        }

        // Collect all assets from the package
        var assetPathsList = new List<string>();
        string[] assetGuids = AssetDatabase.FindAssets("", new[] { packageInfo.assetPath });
        foreach (string guid in assetGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            assetPathsList.Add(assetPath);
        }

        // Get version and unitypackage name
        string version = packageInfo.version;
        string exportedPackageName = string.IsNullOrEmpty(version) ? $"{PackageName}.unitypackage" : $"{PackageName} {version}.unitypackage";

        // Export
        AssetDatabase.ExportPackage(assetPathsList.ToArray(), exportedPackageName,
            ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
    }

    private static PackageInfo FindPackageInfo(string assemblyName)
    {
        Assembly assembly = FindAssembly(assemblyName);
        if (assembly == null)
        {
            Debug.LogError($"Cannot find Assembly with name '{assembly.GetName().Name}'");
            return null;
        }

        PackageInfo packageInfo = PackageInfo.FindForAssembly(assembly);
        if (packageInfo == null)
        {
            Debug.LogError($"Cannot find PackageInfo with Assembly '{assembly.GetName().Name}'");
            return null;
        }

        return packageInfo;
    }

    private static Assembly FindAssembly(string assemblyName)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            if (assembly.GetName().Name == assemblyName)
            {
                return assembly;
            }
        }
        return null;
    }
}
