namespace Physalia.Flexi.PerformanceTests
{
    // This node is for testing boxing/unboxing
    [NodeCategory("PerformanceTests")]
    public class CustomSumNode : ProcessNode
    {
        public Inport<int> a;
        public Inport<int> b;
        public Outport<int> sumPort;

        protected override AbilityState DoLogic()
        {
            int sum = a.GetValue() + b.GetValue();
            sumPort.SetValue(sum);
            return AbilityState.RUNNING;
        }
    }
}
