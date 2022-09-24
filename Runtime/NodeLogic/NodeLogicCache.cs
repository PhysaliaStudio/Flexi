using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Physalia.AbilitySystem
{
    internal static class NodeLogicCache
    {
        private static readonly Type EMPTY_LOGIC_TYPE = typeof(EmptyNodeLogic);
        private static readonly Type NODE_TYPE = typeof(Node);

        private static readonly Dictionary<Type, Type> nodeToLogicCache = new();

        static NodeLogicCache()
        {
            ReloadAllDefines();
        }

        internal static void ReloadAllDefines()
        {
            Clear();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();
                for (var j = 0; j < types.Length; j++)
                {
                    Type type = types[j];
                    if (!type.IsAbstract && NODE_TYPE.IsAssignableFrom(type))
                    {
                        AddNodeDefine(type);
                    }
                }
            }
        }

        private static void AddNodeDefine(Type nodeType)
        {
            if (!NODE_TYPE.IsAssignableFrom(nodeType))
            {
                Debug.LogWarning($"[{nameof(NodeLogicCache)}] Not a Node type: {nodeType.FullName}");
                return;
            }

            if (nodeToLogicCache.ContainsKey(nodeType))
            {
                Debug.LogWarning($"[{nameof(NodeLogicCache)}] Already contains Node type: {nodeType.FullName}");
                return;
            }

            Attribute[] attributes = Attribute.GetCustomAttributes(nodeType);
            foreach (Attribute attribute in attributes)
            {
                if (attribute is NodeLogicAttribute nodeLogicAttribute)
                {
                    Type nodeLogicType = nodeLogicAttribute.NodeLogicType;
                    nodeToLogicCache.Add(nodeType, nodeLogicType);
                    break;
                }
            }
        }

        internal static Type GetNodeLogicType(Node node)
        {
            Type nodeType = node.GetType();
            if (nodeToLogicCache.TryGetValue(nodeType, out Type nodeLogicType))
            {
                return nodeLogicType;
            }

            Debug.LogWarning($"[{nameof(NodeLogicCache)}] Cannot find NodeLogic type for Node type: {nodeType.FullName}");
            return EMPTY_LOGIC_TYPE;
        }

        internal static void Clear()
        {
            nodeToLogicCache.Clear();
        }
    }
}
