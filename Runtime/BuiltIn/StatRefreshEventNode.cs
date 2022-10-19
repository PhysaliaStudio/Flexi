namespace Physalia.AbilityFramework
{
    public class StatRefreshEventNode : EntryNode
    {
        public override bool CanExecute()
        {
            return GetPayload<StatRefreshEvent>() != null;
        }
    }
}
