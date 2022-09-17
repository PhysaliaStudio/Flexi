using System;
using System.Reflection;

namespace Physalia.AbilitySystem
{
    internal static class NodeFactory
    {
        internal static T Create<T>() where T : Node, new()
        {
            return (T)Create(typeof(T));
        }

        internal static Node Create(Type type)
        {
            if (!type.IsSubclassOf(typeof(Node)))
            {
                return null;
            }

            var node = Activator.CreateInstance(type) as Node;

            FieldInfo[] fields = node.GetType().GetFields();
            for (var i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                if (field.IsStatic)
                {
                    continue;
                }

                Type fieldType = field.FieldType;

                if (fieldType.IsAbstract)
                {
                    continue;
                }

                if (fieldType.IsDefined(typeof(NonSerializedAttribute), true))
                {
                    continue;
                }

                if (fieldType.IsSubclassOf(typeof(Inport)))
                {
                    // If the inport is not defined, create a new instance.
                    if (field.GetValue(node) == null)
                    {
                        var inport = Activator.CreateInstance(fieldType) as Inport;
                        inport.node = node;
                        field.SetValue(node, inport);
                    }
                }
                else if (fieldType.IsSubclassOf(typeof(Outport)))
                {
                    // If the outport is not defined, create a new instance.
                    if (field.GetValue(node) == null)
                    {
                        var outport = Activator.CreateInstance(fieldType) as Outport;
                        outport.node = node;
                        field.SetValue(node, outport);
                    }
                }
                else if (fieldType.IsSubclassOf(typeof(Variable)))
                {
                    // If the variable is not defined, create a new instance.
                    if (field.GetValue(node) == null)
                    {
                        var variable = Activator.CreateInstance(fieldType) as Variable;
                        field.SetValue(node, variable);
                    }
                }
            }

            return node;
        }
    }
}
