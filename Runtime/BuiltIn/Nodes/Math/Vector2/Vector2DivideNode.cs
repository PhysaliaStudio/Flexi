using UnityEngine;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathVector2, "Divide (Vector2)", 4)]
    public class Vector2DivideNode : ValueNode
    {
        public Inport<Vector2> a;
        public Inport<float> b;
        public Outport<Vector2> result;

        protected override void EvaluateSelf()
        {
            Vector2 quotient = a.GetValue() / b.GetValue();
            result.SetValue(quotient);
        }
    }
}
