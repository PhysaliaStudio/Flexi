namespace Physalia.AbilityFramework
{
    [NodeCategory("Built-in/Entry")]
    public class StatRefreshEventNode : EntryNode
    {
        public override bool CanExecute(object payloadObj)
        {
            return payloadObj is StatRefreshEvent;
        }
    }
}
