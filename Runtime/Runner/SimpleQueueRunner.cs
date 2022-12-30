using System.Collections.Generic;

namespace Physalia.Flexi
{
    public sealed class SimpleQueueRunner : AbilityRunner
    {
        private readonly Queue<IAbilityFlow> queue = new();

        public override IAbilityFlow Peek()
        {
            bool success = queue.TryPeek(out IAbilityFlow flow);
            if (success)
            {
                return flow;
            }

            return null;
        }

        public override void EnqueueFlow(IAbilityFlow flow)
        {
            queue.Enqueue(flow);
        }

        internal override IAbilityFlow DequeueFlow()
        {
            bool success = queue.TryDequeue(out IAbilityFlow flow);
            if (success)
            {
                return flow;
            }

            return null;
        }

        public override void AddNewQueue()
        {

        }

        public override void RemoveEmptyQueues()
        {

        }

        public override void Clear()
        {
            base.Clear();
            queue.Clear();
        }
    }
}
