using System.Collections.Generic;
using static Physalia.Flexi.AbilityFlowStepper;

namespace Physalia.Flexi
{
    public abstract class TurnBaseRunner : AbilityFlowRunner
    {
        private readonly HashSet<IAbilityFlow> runningFlows = new();

        private RunningState runningState = RunningState.IDLE;

        public abstract IAbilityFlow Peek();
        internal abstract IAbilityFlow DequeueFlow();

        public override void AddFlow(IAbilityFlow flow)
        {
            runningFlows.Add(flow);
        }

        public override bool IsFlowRunning(IAbilityFlow flow)
        {
            return runningFlows.Contains(flow);
        }

        public override void Start()
        {
            if (runningState != RunningState.IDLE)
            {
                Logger.Error($"[{nameof(TurnBaseRunner)}] Failed to start! The runner is still running or waiting.");
                return;
            }

            runningState = RunningState.RUNNING;

            // If there is no flow at start, trigger cached events to see if there's any new flow.
            IAbilityFlow flow = Peek();
            if (flow == null)
            {
                flexiCore.TriggerCachedEvents(this);
            }

            flow = Peek();
            while (flow != null)
            {
                StepResult result = ExecuteStep(flow);
                bool keepRunning = HandleStepResult(result);
                if (!keepRunning)
                {
                    return;
                }

                flow = Peek();
            }

            runningState = RunningState.IDLE;
        }

        public override void Resume(IResumeContext resumeContext)
        {
            if (runningState != RunningState.PAUSE)
            {
                Logger.Error($"[{nameof(TurnBaseRunner)}] Failed to resume! The runner is not in PAUSE state.");
                return;
            }

            if (resumeContext == null)
            {
                Logger.Error($"[{nameof(TurnBaseRunner)}] Failed to resume! resumeContext is null.");
                return;
            }

            runningState = RunningState.RUNNING;

            IAbilityFlow flow = Peek();
            StepResult result = ResumeStep(flow, resumeContext);
            if (result.state == ResultState.Fail)
            {
                Logger.Error($"[{nameof(TurnBaseRunner)}] Failed to resume runner! The resume context is invalid, NodeType: {flow.Current.GetType()}");
            }

            bool keepRunning = HandleStepResult(result);
            if (!keepRunning)
            {
                return;
            }

            flow = Peek();
            while (flow != null)
            {
                result = ExecuteStep(flow);
                keepRunning = HandleStepResult(result);
                if (!keepRunning)
                {
                    return;
                }

                flow = Peek();
            }

            runningState = RunningState.IDLE;
        }

        public override void Tick()
        {
            IAbilityFlow flow = Peek();
            if (flow == null)
            {
                return;
            }

            if (runningState != RunningState.PAUSE)
            {
                Logger.Error($"[{nameof(TurnBaseRunner)}] Failed to tick! The runner is not in PAUSE state.");
                return;
            }

            runningState = RunningState.RUNNING;

            StepResult result = TickStep(flow);
            bool keepRunning = HandleStepResult(result);
            if (!keepRunning)
            {
                return;
            }

            flow = Peek();
            while (flow != null)
            {
                result = ExecuteStep(flow);
                keepRunning = HandleStepResult(result);
                if (!keepRunning)
                {
                    return;
                }

                flow = Peek();
            }

            runningState = RunningState.IDLE;
        }

        private bool HandleStepResult(StepResult result)
        {
            var keepRunning = true;

            switch (result.type)
            {
                case ExecutionType.NodeExecution:
                case ExecutionType.NodeResume:
                case ExecutionType.NodeTick:
                    if (result.state == ResultState.Fail)
                    {
                        keepRunning = false;
                        runningState = RunningState.PAUSE;
                    }
                    else if (result.state == ResultState.Abort)
                    {
                        runningFlows.Remove(result.flow);
                        IAbilityFlow flow = DequeueFlow();
                        NotifyFlowFinished(flow);
                    }
                    else if (result.state == ResultState.Pause)
                    {
                        keepRunning = false;
                        runningState = RunningState.PAUSE;
                    }
                    break;
                case ExecutionType.FlowFinish:
                    {
                        runningFlows.Remove(result.flow);
                        IAbilityFlow flow = DequeueFlow();
                        NotifyFlowFinished(flow);
                    }
                    break;
            }

            TriggerEvent(result);
            NotifyStepResult(result);

            return keepRunning;
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
            runningFlows.Clear();
            runningState = RunningState.IDLE;
        }
    }
}
