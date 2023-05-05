using UnityEngine;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathVector3, "Add (Vector3)", 1)]
    internal class Vector3AddNode : ValueNode
    {
        public Inport<Vector3> a;
        public Inport<Vector3> b;
        public Outport<Vector3> result;

        protected override void EvaluateSelf()
        {
            Vector3 sum = a.GetValue() + b.GetValue();
            result.SetValue(sum);
        }
    }
}
