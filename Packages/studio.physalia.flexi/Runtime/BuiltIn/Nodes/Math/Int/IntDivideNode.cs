namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.MathInt, "Divide (int)", 4)]
    public class IntDivideNode : ValueNode
    {
        public Inport<int> a;
        public Inport<int> b;
        public Outport<int> result;

        protected override void EvaluateSelf()
        {
            int quotient = a.GetValue() / b.GetValue();
            result.SetValue(quotient);
        }
    }
}
