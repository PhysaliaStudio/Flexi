using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphViewEditor
{
    public static class VariableFieldFactory
    {
        public static VisualElement Create(string label, Variable variable, AbilityGraphEditorWindow window)
        {
            switch (variable)
            {
                default:
                    if (variable.ValueType.IsEnum)
                    {
                        return CreateEnumField(label, variable, window);
                    }
                    else
                    {
                        return CreateVariableFieldWithProperty(label, variable, window);
                    }
                case Variable<int> intVariable:
                    return CreateIntegerField(label, intVariable, window);
                case Variable<string> stringVariable:
                    return CreateTextField(label, stringVariable, window);
            }
        }

        private static VisualElement CreateIntegerField(string label, Variable<int> variable, AbilityGraphEditorWindow window)
        {
            var field = new IntegerField(label);
            field.labelElement.style.minWidth = 50f;
            field.RegisterValueChangedCallback(evt =>
            {
                window.SetDirty(true);
                variable.Value = evt.newValue;
            });

            field.SetValueWithoutNotify(variable.Value);
            return field;
        }

        private static VisualElement CreateTextField(string label, Variable<string> variable, AbilityGraphEditorWindow window)
        {
            var field = new TextField(label);
            field.labelElement.style.minWidth = 50f;
            field.RegisterValueChangedCallback(evt =>
            {
                window.SetDirty(true);
                variable.Value = evt.newValue;
            });

            field.SetValueWithoutNotify(variable.Value);
            return field;
        }

        private static VisualElement CreateEnumField(string label, Variable variable, AbilityGraphEditorWindow window)
        {
            var field = new EnumField(label, variable.Value as Enum);
            field.labelElement.style.minWidth = 50f;
            field.RegisterValueChangedCallback(evt =>
            {
                window.SetDirty(true);
                variable.Value = evt.newValue;
            });

            field.SetValueWithoutNotify(variable.Value as Enum);
            return field;
        }

        private static VisualElement CreateVariableFieldWithProperty(string label, Variable variable, AbilityGraphEditorWindow window)
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
