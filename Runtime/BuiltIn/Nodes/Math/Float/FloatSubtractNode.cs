namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathFloat, "Subtract (float)", 2)]
    public class FloatSubtractNode : ValueNode
    {
        public Inport<float> a;
        public Inport<float> b;
        public Outport<float> result;

        protected override void EvaluateSelf()
        {
            float diff = a.GetValue() - b.GetValue();
            result.SetValue(diff);
        }
    }
}
