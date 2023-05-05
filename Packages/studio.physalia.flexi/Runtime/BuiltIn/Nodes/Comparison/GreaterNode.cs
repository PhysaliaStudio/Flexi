namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Comparison)]
    public class GreaterNode : ValueNode
    {
        public Inport<int> a;
        public Inport<int> b;
        public Outport<bool> result;

        protected override void EvaluateSelf()
        {
            result.SetValue(a.GetValue() > b.GetValue());
        }
    }
}
