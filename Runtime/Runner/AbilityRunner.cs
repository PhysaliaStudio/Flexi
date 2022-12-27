using System;

namespace Physalia.AbilityFramework
{
    public abstract class AbilityRunner
    {
        internal struct StepResult
        {
            internal IAbilityFlow flow;
            internal FlowNode node;
            internal ExecutionType type;
            internal ResultState state;

            internal StepResult(IAbilityFlow flow, FlowNode node, ExecutionType type, ResultState state)
            {
                this.flow = flow;
                this.node = node;
                this.type = type;
                this.state = state;
            }
        }

        internal enum ExecutionType
        {
            NODE_EXECUTION,
            NODE_RESUME,
            FLOW_FINISH,
        }

        internal enum ResultState
        {
            SUCCESS, FAILED, ABORT, PAUSE
        }

        private enum RunningState
        {
            IDLE, RUNNING, PAUSE
        }

        internal enum EventTriggerMode
        {
            EACH_NODE,
            EACH_FLOW,
            NEVER,
        }

        internal event Action<StepResult> StepExecuted;

        internal AbilitySystem abilitySystem;

        private EventTriggerMode eventTriggerMode = EventTriggerMode.EACH_NODE;
        private RunningState runningState = RunningState.IDLE;

        internal void SetEventTriggerMode(EventTriggerMode eventTriggerMode)
        {
            this.eventTriggerMode = eventTriggerMode;
        }

        public abstract IAbilityFlow Peek();
        public abstract void EnqueueFlow(IAbilityFlow flow);
        internal abstract IAbilityFlow DequeueFlow();
        public abstract void AddNewQueue();
        public abstract void RemoveEmptyQueues();

        public virtual void Clear()
        {
            runningState = RunningState.IDLE;
        }

        public void Start()
        {
            if (runningState != RunningState.IDLE)
            {
                Logger.Error($"[{nameof(AbilityRunner)}] Failed to start! The runner is still running or waiting.");
                return;
            }

            while (Peek() != null)
            {
                StepResult result = ExecuteStep();
                bool keepRunning = HandleStepResult(result);
                if (!keepRunning)
                {
                    return;
                }
            }

            runningState = RunningState.IDLE;
        }

        public void Resume(IResumeContext resumeContext)
        {
            if (runningState != RunningState.PAUSE)
            {
                Logger.Error($"[{nameof(AbilityRunner)}] Failed to resume! The runner is not in PAUSE state.");
                return;
            }

            StepResult result = ResumeStep(resumeContext);
            bool keepRunning = HandleStepResult(result);
            if (!keepRunning)
            {
                return;
            }

            while (Peek() != null)
            {
                result = ExecuteStep();
                keepRunning = HandleStepResult(result);
                if (!keepRunning)
                {
                    return;
                }
            }

            runningState = RunningState.IDLE;
        }

        private StepResult ExecuteStep()
        {
            IAbilityFlow flow = Peek();
            if (!flow.MoveNext())
            {
                // The graph is empty or has already reached the final node.
                // We keep it until resolving all flows pushed, and dequeue it at here.
                return new StepResult(flow, null, ExecutionType.FLOW_FINISH, ResultState.SUCCESS);
            }

            FlowNode node = flow.Current;
            AbilityState state = node.Run();
            if (state == AbilityState.ABORT)
            {
                return new StepResult(flow, node, ExecutionType.NODE_EXECUTION, ResultState.ABORT);
            }
            else if (state == AbilityState.PAUSE)
            {
                return new StepResult(flow, node, ExecutionType.NODE_EXECUTION, ResultState.PAUSE);
            }
            else
            {
                return new StepResult(flow, node, ExecutionType.NODE_EXECUTION, ResultState.SUCCESS);
            }
        }

        private StepResult ResumeStep(IResumeContext resumeContext)
        {
            IAbilityFlow flow = Peek();
            FlowNode node = flow.Current;

            bool success = node.CheckNodeContext(resumeContext);
            if (!success)
            {
                Logger.Error($"[{nameof(AbilityRunner)}] Failed to resume runner! The resume context is invalid, NodeType: {node.GetType()}");
                return new StepResult(flow, node, ExecutionType.NODE_RESUME, ResultState.FAILED);
            }

            AbilityState state = node.Resume(resumeContext);
            if (state == AbilityState.ABORT)
            {
                return new StepResult(flow, node, ExecutionType.NODE_RESUME, ResultState.ABORT);
            }
            else if (state == AbilityState.PAUSE)
            {
                return new StepResult(flow, node, ExecutionType.NODE_RESUME, ResultState.PAUSE);
            }
            else
            {
                return new StepResult(flow, node, ExecutionType.NODE_RESUME, ResultState.SUCCESS);
            }
        }

        private bool HandleStepResult(StepResult result)
        {
            var keepRunning = true;

            switch (result.type)
            {
                case ExecutionType.NODE_EXECUTION:
                case ExecutionType.NODE_RESUME:
                    if (result.state == ResultState.FAILED)
                    {
                        keepRunning = false;
                        break;
                    }
                    else if (result.state == ResultState.ABORT)
                    {
                        _ = DequeueFlow();
                    }
                    else if (result.state == ResultState.PAUSE)
                    {
                        keepRunning = false;
                        runningState = RunningState.PAUSE;
                    }

                    if (abilitySystem != null)
                    {
                        abilitySystem.TriggerCachedEvents();
                        abilitySystem.RefreshStatsAndModifiers();
                    }
                    break;
                case ExecutionType.FLOW_FINISH:
                    _ = DequeueFlow();
                    break;
            }

            TriggerEvent(result);
            StepExecuted?.Invoke(result);

            return keepRunning;
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

            abilitySystem.TriggerCachedEvents();
            abilitySystem.RefreshStatsAndModifiers();
        }
    }
}
