namespace Physalia.Flexi.PerformanceTests
{
    [NodeCategory("PerformanceTests")]
    public class CustomDoSomethingNode : ProcessNode
    {
        public Inport<int> inport;
        public Outport<int> outport;

        protected override FlowState OnExecute()
        {
            outport.SetValue(inport.GetValue());
            return FlowState.Success;
        }
    }
}
