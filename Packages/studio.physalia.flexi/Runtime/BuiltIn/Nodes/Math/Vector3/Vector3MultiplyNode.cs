using UnityEngine;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathVector3, "Multiply (Vector3)", 3)]
    public class Vector3MultiplyNode : ValueNode
    {
        public Inport<Vector3> a;
        public Inport<float> b;
        public Outport<Vector3> result;

        protected override void EvaluateSelf()
        {
            Vector3 product = a.GetValue() * b.GetValue();
            result.SetValue(product);
        }
    }
}
