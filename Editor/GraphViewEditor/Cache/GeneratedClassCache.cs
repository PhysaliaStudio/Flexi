#if (UNITY_EDITOR || UNITY_STANDALONE) && !ENABLE_IL2CPP && !NET_STANDARD_2_0
#define CAN_EMIT
#endif

using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Globalization;
using System.Linq;
using System.Reflection;
#if CAN_EMIT
using System.Reflection.Emit;
#endif
using UnityEngine.Assertions;

namespace Physalia.Flexi
{
    /// <summary>
    /// A class that emits classes and stores them in cache so that they are not emitted twice.
    /// </summary>
    /// <remarks>
    /// Reference: https://github.com/SolidAlloy/GenericUnityObjects
    /// </remarks>
    internal static class GeneratedClassCache
    {
        // MonoBehaviours and ScriptableObjects are emitted identically. There are two caches to reduce number of
        // objects in each cache and speed up the search.
        private static readonly CacheImplementation CACHE_FOR_MONO_BEHAVIOUR = new();
        private static readonly CacheImplementation CACHE_FOR_SCRIPTABLE_OBJECT = new();

        public static Type GetClassForMonoBehaviour(Type genericMonoBehaviourWithArgs) => CACHE_FOR_MONO_BEHAVIOUR.GetClass(genericMonoBehaviourWithArgs);

        public static Type GetClassForScriptableObject(Type genericScriptableObjectWithArgs) => CACHE_FOR_SCRIPTABLE_OBJECT.GetClass(genericScriptableObjectWithArgs);

        private class CacheImplementation
        {
#if !CAN_EMIT
            public Type GetClass(Type genericTypeWithArgs)
            {
                return null;
            }
#else
            public Type GetClass(Type genericTypeWithArgs)
            {
                string className = GetClassName(genericTypeWithArgs);

                if (classTable.TryGetValue(className, out Type classType))
                    return classType;

                classType = CreateClass(className, genericTypeWithArgs);
                classTable[className] = classType;
                return classType;
            }

            // When module builder tries to emit symbols in a build, it throws NullReferenceException while initializing symbolWriter.
#if UNITY_EDITOR
            private const bool EmitSymbolInfo = true;
#else
            private const bool EmitSymbolInfo = false;
#endif

            private const string AssemblyName = "GenericUnityObjects.DynamicAssembly";

            private static readonly AssemblyBuilder ASSEMBLY_BUILDER =
#if NET_STANDARD
                AssemblyBuilder
#else
                AppDomain.CurrentDomain
#endif
                .DefineDynamicAssembly(
                    new AssemblyName(AssemblyName)
                    {
                        CultureInfo = CultureInfo.InvariantCulture,
                        Flags = AssemblyNameFlags.None,
                        ProcessorArchitecture = ProcessorArchitecture.MSIL,
                        VersionCompatibility = AssemblyVersionCompatibility.SameDomain
                    },
                    AssemblyBuilderAccess.Run);

#if NET_STANDARD
            private static readonly ModuleBuilder MODULE_BUILDER = ASSEMBLY_BUILDER.DefineDynamicModule(AssemblyName);
#else
            private static readonly ModuleBuilder MODULE_BUILDER = ASSEMBLY_BUILDER.DefineDynamicModule(AssemblyName, EmitSymbolInfo);
#endif

            private readonly Dictionary<string, Type> classTable = new();

            private static string GetClassName(Type genericTypeWithArgs)
            {
                string genericTypeName = GetIdentifierSafeName(genericTypeWithArgs.GetGenericTypeDefinition());
                IEnumerable<string> argNames = genericTypeWithArgs.GetGenericArguments().Select(GetIdentifierSafeName);

                // Length shouldn't be an issue. The old length limit was 511 characters, but now there seems to be no limit.
                return $"{genericTypeName}_{string.Join("_", argNames)}";

                static string GetIdentifierSafeName(Type type)
                {
                    string fullName = type.FullName;
                    Assert.IsNotNull(fullName);

                    return fullName.Replace('.', '_').Replace('`', '_');
                }
            }

            private static Type CreateClass(string className, Type genericTypeWithArgs)
            {
                TypeBuilder typeBuilder = MODULE_BUILDER.DefineType(className, TypeAttributes.NotPublic, genericTypeWithArgs);
                return typeBuilder.CreateType();
            }
#endif
        }
    }
}
