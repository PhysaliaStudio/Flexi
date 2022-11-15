namespace Physalia.AbilityFramework
{
    public class StatRefreshEventNode : EntryNode
    {
        public override bool CanExecute(object payloadObj)
        {
            return payloadObj is StatRefreshEvent;
        }
    }
}
