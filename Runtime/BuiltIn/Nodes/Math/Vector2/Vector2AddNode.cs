using UnityEngine;

namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathVector2, "Add (Vector2)", 1)]
    internal class Vector2AddNode : ValueNode
    {
        public Inport<Vector2> a;
        public Inport<Vector2> b;
        public Outport<Vector2> result;

        protected override void EvaluateSelf()
        {
            Vector2 sum = a.GetValue() + b.GetValue();
            result.SetValue(sum);
        }
    }
}
