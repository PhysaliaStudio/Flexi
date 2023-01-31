using UnityEngine;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathVector2, "Subtract (Vector2)", 2)]
    public class Vector2SubtractNode : ValueNode
    {
        public Inport<Vector2> a;
        public Inport<Vector2> b;
        public Outport<Vector2> result;

        protected override void EvaluateSelf()
        {
            Vector2 diff = a.GetValue() - b.GetValue();
            result.SetValue(diff);
        }
    }
}
