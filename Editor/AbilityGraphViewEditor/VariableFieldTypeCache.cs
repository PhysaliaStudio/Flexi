using System;
using System.Collections.Generic;

namespace Physalia.AbilityFramework.GraphViewEditor
{
    public delegate IVariableField CreateVariableField(string label, Variable variable, AbilityGraphEditorWindow window);

    public static class VariableFieldTypeCache
    {
        private static readonly Type INT_TYPE = typeof(int);
        private static readonly Type STRING_TYPE = typeof(string);

        private static readonly Dictionary<Type, CreateVariableField> createFunctions = new();

        public static CreateVariableField GetCreationMethod(Type type)
        {
            if (createFunctions.TryGetValue(type, out CreateVariableField function))
            {
                return function;
            }

            if (type == INT_TYPE)
            {
                static IVariableField method(string label, Variable variable, AbilityGraphEditorWindow window)
                {
                    if (variable is Variable<int> intVariable)
                    {
                        return new IntVariableField(label, intVariable, window);
                    }
                    else
                    {
                        return null;
                    }
                }

                createFunctions.Add(type, method);
                return method;
            }
            else if (type == STRING_TYPE)
            {
                static IVariableField method(string label, Variable variable, AbilityGraphEditorWindow window)
                {
                    if (variable is Variable<string> stringVariable)
                    {
                        return new StringVariableField(label, stringVariable, window);
                    }
                    else
                    {
                        return null;
                    }
                }

                createFunctions.Add(type, method);
                return method;
            }
            else if (type.IsEnum)
            {
                static IVariableField method(string label, Variable variable, AbilityGraphEditorWindow window)
                {
                    if (variable.ValueType.IsEnum)
                    {
                        Type constructedType = typeof(EnumVariableField<>).MakeGenericType(variable.ValueType);
                        var field = Activator.CreateInstance(constructedType, new object[] { label, variable, window }) as EnumVariableField;
                        return field;
                    }
                    else
                    {
                        return null;
                    }
                }

                createFunctions.Add(type, method);
                return method;
            }

            return null;
        }
    }
}
