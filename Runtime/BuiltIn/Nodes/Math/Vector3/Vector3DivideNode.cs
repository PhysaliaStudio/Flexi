using UnityEngine;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathVector3, "Divide (Vector3)", 4)]
    public class Vector3DivideNode : ValueNode
    {
        public Inport<Vector3> a;
        public Inport<float> b;
        public Outport<Vector3> result;

        protected override void EvaluateSelf()
        {
            Vector3 quotient = a.GetValue() / b.GetValue();
            result.SetValue(quotient);
        }
    }
}
