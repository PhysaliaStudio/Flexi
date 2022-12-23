using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public abstract class AbilityRunner
    {
        private readonly Stack<Queue<AbilityFlow>> queueStack = new();

        private AbilityState currentState = AbilityState.CLEAN;

        public void Add(AbilityFlow flow)
        {
            if (queueStack.Count == 0)
            {
                PushNewAbilityQueue();
            }

            Queue<AbilityFlow> queue = queueStack.Peek();
            queue.Enqueue(flow);
        }

        public void PushNewAbilityQueue()
        {
            queueStack.Push(new Queue<AbilityFlow>());
        }

        public void PopEmptyQueues()
        {
            while (queueStack.Count > 0 && queueStack.Peek().Count == 0)
            {
                queueStack.Pop();
            }
        }

        public void Clear()
        {
            queueStack.Clear();
            currentState = AbilityState.CLEAN;
        }

        public void Run(AbilitySystem abilitySystem)
        {
            if (currentState != AbilityState.CLEAN && currentState != AbilityState.ABORT && currentState != AbilityState.DONE)
            {
                Logger.Error($"[{nameof(AbilityRunner)}] You can not execute any unfinished ability instance!");
                return;
            }

            if (queueStack.Count > 0)
            {
                Iterate(abilitySystem);
            }
        }

        public void Resume(AbilitySystem abilitySystem, IResumeContext resumeContext)
        {
            AbilityFlow flow = Peek();
            AbilityGraph graph = flow.Graph;

            if (currentState != AbilityState.PAUSE)
            {
                Logger.Error($"[{nameof(AbilityRunner)}] Failed to resume runner! You can not resume any unpaused ability instance!");
                return;
            }

            bool success = graph.Current.CheckNodeContext(resumeContext);
            if (!success)
            {
                Logger.Error($"[{nameof(AbilityRunner)}] Failed to resume runner! The resume context is invalid, NodeType: {graph.Current.GetType()}");
                return;
            }

            currentState = graph.Current.Resume(resumeContext);
            if (currentState == AbilityState.ABORT)
            {
                Dequeue();
                return;
            }

            if (currentState != AbilityState.RUNNING)
            {
                return;
            }

            abilitySystem.RefreshStatsAndModifiers();
            abilitySystem.TriggerCachedEvents();

            Iterate(abilitySystem);
        }

        private void Iterate(AbilitySystem abilitySystem)
        {
            while (queueStack.Count > 0)
            {
                AbilityFlow flow = Peek();
                AbilityGraph graph = flow.Graph;

                if (graph.MoveNext())
                {
                    currentState = graph.Current.Run();
                    if (currentState == AbilityState.ABORT)
                    {
                        Dequeue();
                        return;
                    }

                    if (currentState != AbilityState.RUNNING)
                    {
                        return;
                    }

                    abilitySystem.RefreshStatsAndModifiers();
                    abilitySystem.TriggerCachedEvents();  // Events may make more abilities pushed into the stack
                }
                else
                {
                    Dequeue();
                }
            }

            currentState = AbilityState.DONE;
        }

        private AbilityFlow Peek()
        {
            if (queueStack.Count == 0)
            {
                return null;
            }

            Queue<AbilityFlow> queue = queueStack.Peek();
            AbilityFlow flow = queue.Peek();
            return flow;
        }

        private void Dequeue()
        {
            if (queueStack.Count == 0)
            {
                return;
            }

            Queue<AbilityFlow> queue = queueStack.Peek();
            if (queue.Count > 0)
            {
                queue.Dequeue();
                if (queue.Count == 0)
                {
                    queueStack.Pop();
                }
            }
        }
    }
}
