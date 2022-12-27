using System;
using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public class LifoQueueRunner
    {
        internal event Action<FlowNode> NodeExecuted;

        private readonly Stack<Queue<IAbilityFlow>> queueStack = new();

        private AbilityState currentState = AbilityState.CLEAN;

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
            currentState = AbilityState.CLEAN;
        }

        public void Start()
        {
            if (currentState != AbilityState.CLEAN && currentState != AbilityState.ABORT && currentState != AbilityState.DONE)
            {
                Logger.Error($"[{nameof(AbilityRunner)}] You can not execute any unfinished ability instance!");
                return;
            }

            IterateFlows();
        }

        public void Resume(IResumeContext resumeContext)
        {
            IAbilityFlow flow = Peek();
            FlowNode node = flow.Current;

            if (currentState != AbilityState.PAUSE)
            {
                Logger.Error($"[{nameof(AbilityRunner)}] Failed to resume runner! You can not resume any unpaused ability instance!");
                return;
            }

            bool success = node.CheckNodeContext(resumeContext);
            if (!success)
            {
                Logger.Error($"[{nameof(AbilityRunner)}] Failed to resume runner! The resume context is invalid, NodeType: {node.GetType()}");
                return;
            }

            currentState = node.Resume(resumeContext);

            if (node != null)
            {
                NodeExecuted?.Invoke(node);
            }

            if (currentState == AbilityState.ABORT)
            {
                _ = DequeueFlow();
                return;
            }
            else if (currentState == AbilityState.PAUSE)
            {
                return;
            }

            IterateFlows();
        }

        private void IterateFlows()
        {
            IAbilityFlow currentFlow = Peek();
            while (currentFlow != null)
            {
                if (!currentFlow.MoveNext())
                {
                    // The graph is empty or has already reached the final node.
                    // We keep it until resolving all flows pushed, and dequeue it at here.
                    _ = DequeueFlow();
                    currentFlow = Peek();
                    continue;
                }

                FlowNode node = currentFlow.Current;
                currentState = node.Run();

                if (node != null)
                {
                    NodeExecuted?.Invoke(node);
                }

                if (currentState == AbilityState.ABORT)
                {
                    _ = DequeueFlow();
                }
                else if (currentState == AbilityState.PAUSE)
                {
                    return;
                }

                currentFlow = Peek();
            }

            currentState = AbilityState.DONE;
        }
    }
}
