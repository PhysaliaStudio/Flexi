namespace Physalia.Flexi
{
    [NodeCategory(BuiltInCategory.Common)]
    public class AbortNode : ProcessNode
    {
        protected override FlowState OnExecute()
        {
            return FlowState.Abort;
        }
    }
}
