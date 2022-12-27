using System;
using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public class LifoQueueRunner
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

        internal event Action<StepResult> StepExecuted;

        private readonly Stack<Queue<IAbilityFlow>> queueStack = new();
        private RunningState runningState = RunningState.IDLE;

        internal int CountOfQueue => queueStack.Count;

        public LifoQueueRunner()
        {
            queueStack.Push(new Queue<IAbilityFlow>());
        }

        public IAbilityFlow Peek()
        {
            Queue<IAbilityFlow> topmostQueue = queueStack.Peek();
            bool success = topmostQueue.TryPeek(out IAbilityFlow flow);
            if (success)
            {
                return flow;
            }

            return null;
        }

        public void EnqueueFlow(IAbilityFlow flow)
        {
            Queue<IAbilityFlow> topmostQueue = queueStack.Peek();
            topmostQueue.Enqueue(flow);
        }

        internal IAbilityFlow DequeueFlow()
        {
            Queue<IAbilityFlow> topmostQueue = queueStack.Peek();
            if (topmostQueue.Count > 0)
            {
                IAbilityFlow flow = topmostQueue.Dequeue();
                if (topmostQueue.Count == 0 && queueStack.Count > 1)
                {
                    _ = queueStack.Pop();
                }

                return flow;
            }

            return null;
        }

        public void AddNewQueue()
        {
            Queue<IAbilityFlow> queue = queueStack.Peek();
            if (queue.Count == 0)
            {
                Logger.Warn($"[{nameof(LifoQueueRunner)}] AddNewQueue() is skipped since the topmost queue is empty.");
                return;
            }

            queueStack.Push(new Queue<IAbilityFlow>());
        }

        public void Clear()
        {
            queueStack.Clear();
            queueStack.Push(new Queue<IAbilityFlow>());
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
                    break;
                case ExecutionType.FLOW_FINISH:
                    _ = DequeueFlow();
                    break;
            }

            StepExecuted?.Invoke(result);

            return keepRunning;
        }
    }
}
