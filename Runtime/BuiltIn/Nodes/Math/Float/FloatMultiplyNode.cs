namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathFloat, "Multiply (float)", 3)]
    public class FloatMultiplyNode : ValueNode
    {
        public Inport<float> a;
        public Inport<float> b;
        public Outport<float> result;

        protected override void EvaluateSelf()
        {
            float product = a.GetValue() * b.GetValue();
            result.SetValue(product);
        }
    }
}
