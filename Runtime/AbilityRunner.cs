using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public abstract class AbilityRunner
    {
        private readonly Stack<AbilityInstance> abilityStack = new();

        private AbilityState currentState = AbilityState.CLEAN;

        public void Add(AbilityInstance instance)
        {
            abilityStack.Push(instance);
        }

        public void Clear()
        {
            abilityStack.Clear();
            currentState = AbilityState.CLEAN;
        }

        public void Run(AbilitySystem abilitySystem)
        {
            if (currentState != AbilityState.CLEAN && currentState != AbilityState.ABORT && currentState != AbilityState.DONE)
            {
                Logger.Error($"[{nameof(AbilityRunner)}] You can not execute any unfinished ability instance!");
                return;
            }

            Iterate(abilitySystem);
        }

        public void Resume(AbilitySystem abilitySystem, NodeContext context)
        {
            AbilityInstance instance = abilityStack.Peek();
            AbilityGraph graph = instance.Graph;

            if (currentState != AbilityState.PAUSE)
            {
                Logger.Error($"[{nameof(AbilityRunner)}] Failed to resume runner! You can not resume any unpaused ability instance!");
                return;
            }

            bool success = graph.Current.CheckNodeContext(context);
            if (!success)
            {
                Logger.Error($"[{nameof(AbilityRunner)}] Failed to resume runner! The resume context is invalid, NodeType: {graph.Current.GetType()}");
                return;
            }

            currentState = graph.Current.Resume(context);
            if (currentState == AbilityState.ABORT)
            {
                abilityStack.Pop();
                return;
            }

            if (currentState != AbilityState.RUNNING)
            {
                return;
            }

            abilitySystem.RefreshStatsAndModifiers();
            abilitySystem.TriggerNextEvent();

            Iterate(abilitySystem);
        }

        private void Iterate(AbilitySystem abilitySystem)
        {
            while (abilityStack.Count > 0)
            {
                AbilityInstance instance = abilityStack.Peek();
                AbilityGraph graph = instance.Graph;

                if (graph.MoveNext())
                {
                    currentState = graph.Current.Run();
                    if (currentState == AbilityState.ABORT)
                    {
                        abilityStack.Pop();
                        return;
                    }

                    if (currentState != AbilityState.RUNNING)
                    {
                        return;
                    }

                    abilitySystem.RefreshStatsAndModifiers();
                    abilitySystem.TriggerNextEvent();  // Events may make more abilities pushed into the stack
                }
                else
                {
                    abilityStack.Pop();
                }
            }

            currentState = AbilityState.DONE;
        }
    }
}
