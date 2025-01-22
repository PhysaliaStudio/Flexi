using System.Collections.Generic;
using static Physalia.Flexi.AbilityFlowStepper;

namespace Physalia.Flexi
{
    internal sealed class StatRefreshRunner
    {
        private readonly List<IAbilityFlow> flows = new(16);

        public void AddFlow(IAbilityFlow flow)
        {
            flows.Add(flow);
        }

        public void Start()
        {
            for (var i = 0; i < flows.Count; i++)
            {
                IAbilityFlow flow = flows[i];

                bool keepRunning;
                do
                {
                    StepResult result = ExecuteStep(flow);
                    keepRunning = HandleStepResult(result);
                }
                while (keepRunning);
            }

            // Since no flow should be paused, every flows will be finished. Directly clear the list.
            flows.Clear();
        }

        private bool HandleStepResult(StepResult result)
        {
            switch (result.type)
            {
                default:
                    return false;
                case ExecutionType.NODE_EXECUTION:
                    if (result.state != ResultState.SUCCESS)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
            }
        }
    }
}
