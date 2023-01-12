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
                case Variable<string> stringVariable:
                    {
                        var field = new TextField(label);
                        SetUpField(field, stringVariable, window);
                        return field;
                    }
                case Variable<bool> boolVariable:
                    {
                        var field = new Toggle(label);
                        SetUpField(field, boolVariable, window);
                        return field;
                    }
                case Variable<int> intVariable:
                    {
                        var field = new IntegerField(label);
                        SetUpField(field, intVariable, window);
                        return field;
                    }
                case Variable<long> longVariable:
                    {
                        var field = new LongField(label);
                        SetUpField(field, longVariable, window);
                        return field;
                    }
                case Variable<float> floatVariable:
                    {
                        var field = new FloatField(label);
                        SetUpField(field, floatVariable, window);
                        return field;
                    }
                case Variable<double> doubleVariable:
                    {
                        var field = new DoubleField(label);
                        SetUpField(field, doubleVariable, window);
                        return field;
                    }
                case Variable<Vector2> vector2Variable:
                    {
                        var field = new Vector2Field(label);
                        SetUpField(field, vector2Variable, window);
                        return field;
                    }
                case Variable<Vector3> vector3Variable:
                    {
                        var field = new Vector3Field(label);
                        SetUpField(field, vector3Variable, window);
                        return field;
                    }
                case Variable<Vector4> vector4Variable:
                    {
                        var field = new Vector4Field(label);
                        SetUpField(field, vector4Variable, window);
                        return field;
                    }
                case Variable<Vector2Int> vector2IntVariable:
                    {
                        var field = new Vector2IntField(label);
                        SetUpField(field, vector2IntVariable, window);
                        return field;
                    }
                case Variable<Vector3Int> vector3IntVariable:
                    {
                        var field = new Vector3IntField(label);
                        SetUpField(field, vector3IntVariable, window);
                        return field;
                    }
                case Variable<Color> colorVariable:
                    {
                        var field = new ColorField(label);
                        SetUpField(field, colorVariable, window);
                        return field;
                    }
                case Variable<Rect> rectVariable:
                    {
                        var field = new RectField(label);
                        SetUpField(field, rectVariable, window);
                        return field;
                    }
                case Variable<RectInt> rectIntVariable:
                    {
                        var field = new RectIntField(label);
                        SetUpField(field, rectIntVariable, window);
                        return field;
                    }
                case Variable<Bounds> boundsVariable:
                    {
                        var field = new BoundsField(label);
                        SetUpField(field, boundsVariable, window);
                        return field;
                    }
                case Variable<BoundsInt> boundsIntVariable:
                    {
                        var field = new BoundsIntField(label);
                        SetUpField(field, boundsIntVariable, window);
                        return field;
                    }
            }
        }

        private static void SetUpField<T>(BaseField<T> field, Variable<T> variable, AbilityGraphEditorWindow window)
        {
            field.RegisterValueChangedCallback(evt =>
            {
                window.SetDirty(true);
                variable.Value = evt.newValue;
            });

            field.SetValueWithoutNotify(variable.Value);
        }

        private static VisualElement CreateEnumField(string label, Variable variable, AbilityGraphEditorWindow window)
        {
            var field = new EnumField(label, variable.Value as Enum);
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
