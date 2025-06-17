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
        private readonly HashSet<IAbilityFlow> runningFlows = new();
        private readonly List<int> indiceForRemoved = new();

        // Real-time runner never stop.
        public override RunningState State => RunningState.RUNNING;

        public override void AddFlow(IAbilityFlow flow)
        {
            flows.Add(flow);
            runningFlows.Add(flow);
        }

        public override bool IsFlowRunning(IAbilityFlow flow)
        {
            return runningFlows.Contains(flow);
        }

        public override void Start()
        {
            // If there is no flow at start, trigger cached events to see if there's any new flow.
            if (flows.Count == 0)
            {
                flexiCore.TriggerCachedEvents(this);
            }

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
                int removeIndex = indiceForRemoved[i];
                IAbilityFlow flow = flows[removeIndex];
                runningFlows.Remove(flow);
                flows.RemoveAt(removeIndex);
                NotifyFlowFinished(flow);
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
                int removeIndex = indiceForRemoved[i];
                IAbilityFlow flow = flows[removeIndex];
                runningFlows.Remove(flow);
                flows.RemoveAt(removeIndex);
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
                int removeIndex = indiceForRemoved[i];
                IAbilityFlow flow = flows[removeIndex];
                runningFlows.Remove(flow);
                flows.RemoveAt(removeIndex);
            }
            indiceForRemoved.Clear();
        }

        private StepHandleAction HandleStepResult(StepResult result)
        {
            var keepRunning = true;
            var removeFlow = false;

            switch (result.type)
            {
                case ExecutionType.NodeExecution:
                case ExecutionType.NodeResume:
                case ExecutionType.NodeTick:
                    if (result.state == ResultState.Fail)
                    {
                        keepRunning = false;
                        break;
                    }
                    else if (result.state == ResultState.Abort)
                    {
                        keepRunning = false;
                        removeFlow = true;
                    }
                    else if (result.state == ResultState.Pause)
                    {
                        keepRunning = false;
                    }
                    break;
                case ExecutionType.FlowFinish:
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
            if (flexiCore == null)
            {
                return;
            }

            switch (eventTriggerMode)
            {
                default:
                case EventTriggerMode.NEVER:
                    return;
                case EventTriggerMode.EACH_NODE:
                    if (result.type == ExecutionType.NodeExecution || result.type == ExecutionType.NodeResume)
                    {
                        if (result.node.ShouldTriggerChainEvents)
                        {
                            flexiCore.TriggerCachedEvents(this);
                            flexiCore.RefreshStatsAndModifiers();
                        }
                    }
                    break;
                case EventTriggerMode.EACH_FLOW:
                    if (result.type == ExecutionType.FlowFinish)
                    {
                        flexiCore.TriggerCachedEvents(this);
                        flexiCore.RefreshStatsAndModifiers();
                    }
                    break;
            }
        }

        public override void Clear()
        {
            flows.Clear();
            runningFlows.Clear();
        }
    }
}
