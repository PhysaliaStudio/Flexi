using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphViewEditor
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

        public static VisualElement CreateVariableField(string label, Variable variable, AbilityGraphEditorWindow window)
        {
            Type variableType = variable.GetType();
            Type valueType = variableType.GetGenericArguments()[0];
            Type scriptableObjectType = typeof(VariableObject<>).MakeGenericType(valueType);
            Type usableType = GeneratedClassCache.GetClassForScriptableObject(scriptableObjectType);

            var variableObject = ScriptableObject.CreateInstance(usableType) as VariableObject;
            variableObject.Variable = variable;

            var serializedObject = new SerializedObject(variableObject);
            SerializedProperty property = serializedObject.FindProperty("variable.value");

            // Bug: The ListView (for List/Array types) in PropertyField is broken in 2021.3, due to the drag handling simutaneously on NodeView and ListView.
            // But it's work perfectly with IMGUI version...
            //var propertyField = new PropertyField();  // Bug: Failed to use the constructor to directly create the child fields
            //propertyField.BindProperty(property);
            //propertyField.label = label;
            //return propertyField;

            var container = new IMGUIContainer(() =>
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(property, new GUIContent(label), GUILayout.Width(300f));
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties();
                    window.SetDirty(true);
                }
            });
            return container;
        }
    }
}
