using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public sealed class AbilityInstance
    {
        private readonly int abilityId;
        private readonly AbilitySystem system;
        private readonly AbilityGraph graph;

        private readonly Dictionary<string, int> blackboard = new();

        private StatOwner owner;
        private object payload;
        private AbilityState currentState = AbilityState.CLEAN;

        public int AbilityId => abilityId;
        public AbilitySystem System => system;
        public StatOwner Owner => owner;
        internal object Payload => payload;
        public AbilityState CurrentState => currentState;

        internal AbilityInstance(AbilityGraph graph) : this(0, null, graph)
        {

        }

        internal AbilityInstance(int abilityId, AbilitySystem system, AbilityGraph graph)
        {
            this.abilityId = abilityId;
            this.system = system;
            this.graph = graph;

            for (var i = 0; i < graph.Nodes.Count; i++)
            {
                graph.Nodes[i].instance = this;
            }

            for (var i = 0; i < graph.BlackboardVariables.Count; i++)
            {
                BlackboardVariable variable = graph.BlackboardVariables[i];
                blackboard.Add(variable.key, variable.value);
            }
        }

        internal void SetOwner(StatOwner owner)
        {
            this.owner = owner;
        }

        public void SetPayload(object payload)
        {
            this.payload = payload;
        }

        public void OverrideBlackboardVariable(string key, int value)
        {
            if (!blackboard.ContainsKey(key))
            {
                Logger.Warn($"[{nameof(AbilityInstance)}] Blackboard does not have key: {key}. Cancel the override");
                return;
            }

            blackboard[key] = value;
        }

        public int GetBlackboardVariable(string key)
        {
            if (blackboard.TryGetValue(key, out int value))
            {
                return value;
            }

            Logger.Warn($"[{nameof(AbilityInstance)}] Blackboard does not have key: {key}. Returns 0");
            return 0;
        }

        public bool CanExecute(object payload)
        {
            this.payload = payload;

            graph.Reset(0);
            if (!graph.MoveNext())
            {
                return false;
            }

            if (graph.Current is EntryNode entryNode)
            {
                return entryNode.CanExecute();
            }

            return false;
        }

        public void Execute()
        {
            if (currentState != AbilityState.CLEAN && currentState != AbilityState.DONE)
            {
                Logger.Error($"[{nameof(AbilityInstance)}] You can not execute any unfinished ability instance!");
                return;
            }

            if (!CanExecute(payload))
            {
                Logger.Error($"[{nameof(AbilityInstance)}] Cannot execute ability, because the payload doesn't match the condition. Normally you should call CanExecute() to check.");
                return;
            }

            graph.Reset(0);

            IterateGraph();
        }

        public void Resume(NodeContext nodeContext)
        {
            if (currentState != AbilityState.PAUSE)
            {
                Logger.Error($"[{nameof(AbilityInstance)}] You can not resume any unpaused ability instance!");
                return;
            }

            bool success = graph.Current.CheckNodeContext(nodeContext);
            if (!success)
            {
                Logger.Error($"[{nameof(AbilityInstance)}] The resume context is invalid, NodeType: {graph.Current.GetType()}");
                return;
            }

            currentState = graph.Current.Resume(nodeContext);
            if (currentState == AbilityState.PAUSE)
            {
                return;
            }

            IterateGraph();
        }

        private void IterateGraph()
        {
            while (graph.MoveNext())
            {
                currentState = graph.Current.Run();
                if (currentState == AbilityState.PAUSE)
                {
                    return;
                }
            }

            currentState = AbilityState.DONE;
        }

        internal void Push(FlowNode flowNode)
        {
            graph.Push(flowNode);
        }

        public void Reset()
        {
            graph.Reset(0);
            currentState = AbilityState.CLEAN;
            SetPayload(null);

            blackboard.Clear();
            for (var i = 0; i < graph.BlackboardVariables.Count; i++)
            {
                BlackboardVariable variable = graph.BlackboardVariables[i];
                blackboard.Add(variable.key, variable.value);
            }
        }
    }
}
