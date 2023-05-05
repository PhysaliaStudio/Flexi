namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathInt, "Multiply (int)", 3)]
    public class IntMultiplyNode : ValueNode
    {
        public Inport<int> a;
        public Inport<int> b;
        public Outport<int> result;

        protected override void EvaluateSelf()
        {
            int product = a.GetValue() * b.GetValue();
            result.SetValue(product);
        }
    }
}
