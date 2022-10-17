using UnityEngine;

namespace Physalia.AbilitySystem
{
    public sealed class AbilityInstance
    {
        private readonly int abilityId;
        private readonly AbilitySystem system;
        private readonly AbilityGraph graph;

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
        }

        internal void SetOwner(StatOwner owner)
        {
            this.owner = owner;
        }

        public void SetPayload(object payload)
        {
            this.payload = payload;
        }

        public bool CanExecute(object payload)
        {
            this.payload = payload;

            graph.Reset(0);
            if (graph.Current == null)
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
                Debug.LogError($"[{nameof(AbilityInstance)}] You can not execute any unfinished ability instance!");
                return;
            }

            if (!CanExecute(payload))
            {
                Debug.LogError($"[{nameof(AbilityInstance)}] Cannot execute ability, because the payload doesn't match the condition. Normally you should call CanExecute() to check.");
                return;
            }

            graph.Reset(0);

            IterateGraph();
        }

        public void Resume()
        {
            if (currentState != AbilityState.PAUSE)
            {
                Debug.LogError($"[{nameof(AbilityInstance)}] You can not resume any unpaused ability instance!");
                return;
            }

            currentState = graph.Current.Resume();
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

        public void Reset()
        {
            graph.Reset(0);
            currentState = AbilityState.CLEAN;
            SetPayload(null);
        }
    }
}
