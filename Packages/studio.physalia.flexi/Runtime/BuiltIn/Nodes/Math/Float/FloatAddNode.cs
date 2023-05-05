namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathFloat, "Add (float)", 1)]
    public class FloatAddNode : ValueNode
    {
        public Inport<float> a;
        public Inport<float> b;
        public Outport<float> result;

        protected override void EvaluateSelf()
        {
            float sum = a.GetValue() + b.GetValue();
            result.SetValue(sum);
        }
    }
}
