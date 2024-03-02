namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Common)]
    public class ConcatNode : ValueNode
    {
        public Inport<string> a;
        public Inport<string> b;
        public Outport<string> result;

        protected override void EvaluateSelf()
        {
            result.SetValue(string.Concat(a, b));
        }
    }
}
