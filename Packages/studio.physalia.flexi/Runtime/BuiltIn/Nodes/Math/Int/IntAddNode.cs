namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathInt, "Add (int)", 1)]
    internal sealed class IntAddNode : ValueNode
    {
        public Inport<int> a;
        public Inport<int> b;
        public Outport<int> result;

        protected override void EvaluateSelf()
        {
            int sum = a.GetValue() + b.GetValue();
            result.SetValue(sum);
        }
    }
}
