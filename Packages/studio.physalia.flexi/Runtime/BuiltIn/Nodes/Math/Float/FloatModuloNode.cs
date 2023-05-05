namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathFloat, "Modulo (float)", 5)]
    public class FloatModuloNode : ValueNode
    {
        public Inport<float> a;
        public Inport<float> b;
        public Outport<float> result;

        protected override void EvaluateSelf()
        {
            float remainder = a.GetValue() % b.GetValue();
            result.SetValue(remainder);
        }
    }
}
