using System.Collections.Generic;

namespace Physalia.Flexi
{
    public sealed class LifoQueueRunner : TurnBaseRunner
    {
        private readonly Stack<Queue<IAbilityFlow>> queueStack = new();

        internal int CountOfQueue => queueStack.Count;

        public LifoQueueRunner()
        {
            queueStack.Push(new Queue<IAbilityFlow>());
        }

        public override IAbilityFlow Peek()
        {
            Queue<IAbilityFlow> topmostQueue = queueStack.Peek();
            bool success = topmostQueue.TryPeek(out IAbilityFlow flow);
            if (success)
            {
                return flow;
            }

            return null;
        }

        public override void AddFlow(IAbilityFlow flow)
        {
            base.AddFlow(flow);
            Queue<IAbilityFlow> topmostQueue = queueStack.Peek();
            topmostQueue.Enqueue(flow);
        }

        internal override IAbilityFlow DequeueFlow()
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

        internal void AddNewQueue()
        {
            Queue<IAbilityFlow> queue = queueStack.Peek();
            if (queue.Count == 0)
            {
                Logger.Warn($"[{nameof(LifoQueueRunner)}] AddNewQueue() is skipped since the topmost queue is empty.");
                return;
            }

            queueStack.Push(new Queue<IAbilityFlow>());
        }

        internal void RemoveEmptyQueues()
        {
            while (queueStack.Count > 1 && queueStack.Peek().Count == 0)
            {
                _ = queueStack.Pop();
            }
        }

        public override void BeforeTriggerEvents()
        {
            AddNewQueue();
        }

        public override void AfterTriggerEvents()
        {
            RemoveEmptyQueues();
        }

        public override void Clear()
        {
            base.Clear();
            queueStack.Clear();
            queueStack.Push(new Queue<IAbilityFlow>());
        }
    }
}
