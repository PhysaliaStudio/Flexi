namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Logical)]
    public class TrueNode : ValueNode
    {
        public Outport<bool> value;

        protected override void EvaluateSelf()
        {
            value.SetValue(true);
        }
    }
}
