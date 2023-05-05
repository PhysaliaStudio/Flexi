using UnityEngine;

namespace Physalia.Flexi.GraphViewEditor
{
    public abstract class VariableObject : ScriptableObject
    {
        public abstract Variable Variable { get; set; }
    }

    /// <summary>
    /// This ScriptableObject is just for using serialization in NodeView
    /// </summary>
    public class VariableObject<T> : VariableObject
    {
        public Variable<T> variable = new();

        public override Variable Variable
        {
            get => variable;
            set
            {
                if (value is Variable<T> genericValue)
                {
                    variable = genericValue;
                }
                else
                {
                    variable = new();
                }
            }
        }
    }
}
