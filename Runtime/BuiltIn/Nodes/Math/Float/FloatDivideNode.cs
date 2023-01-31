namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathFloat, "Divide (float)", 4)]
    public class FloatDivideNode : ValueNode
    {
        public Inport<float> a;
        public Inport<float> b;
        public Outport<float> result;

        protected override void EvaluateSelf()
        {
            float quotient = a.GetValue() / b.GetValue();
            result.SetValue(quotient);
        }
    }
}
