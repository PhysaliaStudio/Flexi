namespace Physalia.Flexi.PerformanceTests
{
    [NodeCategory("PerformanceTests")]
    public class CustomDoSomethingNode : ProcessNode
    {
        public Inport<int> inport;
        public Outport<int> outport;

        protected override AbilityState DoLogic()
        {
            outport.SetValue(inport.GetValue());
            return AbilityState.RUNNING;
        }
    }
}
