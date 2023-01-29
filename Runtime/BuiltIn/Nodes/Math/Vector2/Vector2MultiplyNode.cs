using UnityEngine;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathVector2, "Multiply (Vector2)", 3)]
    public class Vector2MultiplyNode : ValueNode
    {
        public Inport<Vector2> a;
        public Inport<float> b;
        public Outport<Vector2> result;

        protected override void EvaluateSelf()
        {
            Vector2 product = a.GetValue() * b.GetValue();
            result.SetValue(product);
        }
    }
}
