using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Physalia.AbilityFramework.GraphViewEditor
{
    public interface IVariableField
    {
        VisualElement VisualElement { get; }
    }

    public class IntVariableField : IntegerField, IVariableField
    {
        public VisualElement VisualElement => this;

        public IntVariableField(string label, Variable<int> variable) : base(label)
        {
            labelElement.style.minWidth = 50f;
            this.RegisterValueChangedCallback(evt =>
            {
                variable.Value = evt.newValue;
            });

            SetValueWithoutNotify(variable.Value);
        }
    }

    public class StringVariableField : TextField, IVariableField
    {
        public VisualElement VisualElement => this;

        public StringVariableField(string label, Variable<string> variable) : base(label)
        {
            labelElement.style.minWidth = 50f;
            this.RegisterValueChangedCallback(evt =>
            {
                variable.Value = evt.newValue;
            });

            SetValueWithoutNotify(variable.Value);
        }
    }

    public class EnumVariableField<T> : EnumVariableField where T : Enum
    {
        public EnumVariableField(string label, Variable<T> variable) : base(label, variable.Value)
        {
            labelElement.style.minWidth = 50f;
            this.RegisterValueChangedCallback(evt =>
            {
                variable.Value = (T)evt.newValue;
            });

            SetValueWithoutNotify(variable.Value);
        }
    }

    public abstract class EnumVariableField : EnumField, IVariableField
    {
        public VisualElement VisualElement => this;

        public EnumVariableField(string label, Enum defaultValue) : base(label, defaultValue)
        {

        }
    }
}
