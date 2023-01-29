namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathInt, "Subtract (int)", 2)]
    public class IntSubtractNode : ValueNode
    {
        public Inport<int> a;
        public Inport<int> b;
        public Outport<int> result;

        protected override void EvaluateSelf()
        {
            int diff = a.GetValue() - b.GetValue();
            result.SetValue(diff);
        }
    }
}
