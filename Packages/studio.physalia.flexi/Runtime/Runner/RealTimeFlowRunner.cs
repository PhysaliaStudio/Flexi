using System.Collections.Generic;
using static Physalia.Flexi.AbilityFlowStepper;

namespace Physalia.Flexi
{
    public class RealTimeFlowRunner : AbilityFlowRunner
    {
        private struct StepHandleAction
        {
            public bool keepRunning;
            public bool removeFlow;
        }

        private readonly List<IAbilityFlow> flows = new();
        private readonly List<int> indiceForRemoved = new();

        public override void AddFlow(IAbilityFlow flow)
        {
            flows.Add(flow);
        }

        public override void Start()
        {
            for (var i = 0; i < flows.Count; i++)
            {
                // If the flow is already running, skip it, or it will move next.
                if (flows[i].Current != null)
                {
                    continue;
                }

                StepHandleAction handleAction;
                do
                {
                    StepResult result = ExecuteStep(flows[i]);
                    handleAction = HandleStepResult(result);
                }
                while (handleAction.keepRunning);

                if (handleAction.removeFlow)
                {
                    indiceForRemoved.Add(i);
                }
            }

            for (var i = indiceForRemoved.Count - 1; i >= 0; i--)
            {
                flows.RemoveAt(i);
            }
            indiceForRemoved.Clear();
        }

        public override void Resume(IResumeContext resumeContext)
        {
            for (var i = 0; i < flows.Count; i++)
            {
                StepResult result = ResumeStep(flows[i], resumeContext);
                StepHandleAction handleAction = HandleStepResult(result);
                if (handleAction.removeFlow)
                {
                    indiceForRemoved.Add(i);
                }

                if (!handleAction.keepRunning)
                {
                    continue;
                }

                // Run until stopped
                do
                {
                    result = ExecuteStep(flows[i]);
                    handleAction = HandleStepResult(result);
                }
                while (handleAction.keepRunning);

                if (handleAction.removeFlow)
                {
                    indiceForRemoved.Add(i);
                }
            }

            for (var i = indiceForRemoved.Count - 1; i >= 0; i--)
            {
                flows.RemoveAt(i);
            }
            indiceForRemoved.Clear();
        }

        public override void Tick()
        {
            for (var i = 0; i < flows.Count; i++)
            {
                StepResult result = TickStep(flows[i]);
                StepHandleAction handleAction = HandleStepResult(result);
                if (handleAction.removeFlow)
                {
                    indiceForRemoved.Add(i);
                }

                if (!handleAction.keepRunning)
                {
                    continue;
                }

                // Run until stopped
                do
                {
                    result = ExecuteStep(flows[i]);
                    handleAction = HandleStepResult(result);
                }
                while (handleAction.keepRunning);

                if (handleAction.removeFlow)
                {
                    indiceForRemoved.Add(i);
                }
            }

            for (var i = indiceForRemoved.Count - 1; i >= 0; i--)
            {
                flows.RemoveAt(i);
            }
            indiceForRemoved.Clear();
        }

        private StepHandleAction HandleStepResult(StepResult result)
        {
            var keepRunning = true;
            var removeFlow = false;

            switch (result.type)
            {
                case ExecutionType.NODE_EXECUTION:
                case ExecutionType.NODE_RESUME:
                case ExecutionType.NODE_TICK:
                    if (result.state == ResultState.FAILED)
                    {
                        keepRunning = false;
                        break;
                    }
                    else if (result.state == ResultState.ABORT)
                    {
                        keepRunning = false;
                        removeFlow = true;
                    }
                    else if (result.state == ResultState.PAUSE)
                    {
                        keepRunning = false;
                    }
                    break;
                case ExecutionType.FLOW_FINISH:
                    keepRunning = false;
                    removeFlow = true;
                    break;
            }

            TriggerEvent(result);
            NotifyStepResult(result);

            return new StepHandleAction
            {
                keepRunning = keepRunning,
                removeFlow = removeFlow,
            };
        }

        private void TriggerEvent(StepResult result)
        {
            if (abilitySystem == null)
            {
                return;
            }

            switch (eventTriggerMode)
            {
                case EventTriggerMode.EACH_NODE:
                    if (result.type != ExecutionType.NODE_EXECUTION && result.type != ExecutionType.NODE_RESUME)
                    {
                        return;
                    }
                    break;
                case EventTriggerMode.EACH_FLOW:
                    if (result.type != ExecutionType.FLOW_FINISH)
                    {
                        return;
                    }
                    break;
                case EventTriggerMode.NEVER:
                    return;
            }

            abilitySystem.TriggerCachedEvents(this);
            abilitySystem.RefreshStatsAndModifiers();
        }

        public override void Clear()
        {
            flows.Clear();
        }
    }
}
