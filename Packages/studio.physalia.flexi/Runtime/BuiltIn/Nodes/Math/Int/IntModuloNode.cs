namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathInt, "Modulo (int)", 5)]
    public class IntModuloNode : ValueNode
    {
        public Inport<int> a;
        public Inport<int> b;
        public Outport<int> result;

        protected override void EvaluateSelf()
        {
            int remainder = a.GetValue() % b.GetValue();
            result.SetValue(remainder);
        }
    }
}
