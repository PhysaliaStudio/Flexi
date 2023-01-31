using UnityEngine;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathVector3, "Subtract (Vector3)", 2)]
    public class Vector3SubtractNode : ValueNode
    {
        public Inport<Vector3> a;
        public Inport<Vector3> b;
        public Outport<Vector3> result;

        protected override void EvaluateSelf()
        {
            Vector3 diff = a.GetValue() - b.GetValue();
            result.SetValue(diff);
        }
    }
}
