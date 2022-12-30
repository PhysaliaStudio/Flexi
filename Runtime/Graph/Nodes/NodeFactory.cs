using System;
using System.Reflection;

namespace Physalia.Flexi
{
    public static class NodeFactory
    {
        public static T Create<T>() where T : Node, new()
        {
            return (T)Create(typeof(T));
        }

        public static Node Create(Type type)
        {
            if (!type.IsSubclassOf(typeof(Node)))
            {
                return null;
            }

            var node = Activator.CreateInstance(type) as Node;

            FieldInfo[] fields = type.GetFieldsIncludeBasePrivate();
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
                        Inport inport = node.CreateInportWithPortType(fieldType, field.Name);
                        field.SetValue(node, inport);
                    }
                }
                else if (fieldType.IsSubclassOf(typeof(Outport)))
                {
                    // If the outport is not defined, create a new instance.
                    if (field.GetValue(node) == null)
                    {
                        Outport outport = node.CreateOutportWithPortType(fieldType, field.Name);
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
