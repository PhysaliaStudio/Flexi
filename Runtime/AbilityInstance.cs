using UnityEngine;

namespace Physalia.AbilitySystem
{
    public sealed class AbilityInstance
    {
        private readonly AbilityGraph graph;

        private AbilityState currentState = AbilityState.CLEAN;

        public AbilityState CurrentState => currentState;

        internal AbilityInstance(AbilityGraph graph)
        {
            this.graph = graph;
        }

        public void Execute(object payload)
        {
            if (currentState != AbilityState.CLEAN && currentState != AbilityState.DONE)
            {
                Debug.LogError($"[{nameof(AbilityInstance)}] You can not execute any unfinished ability instance!");
                return;
            }

            graph.Reset(0);
            for (var i = 0; i < graph.Nodes.Count; i++)
            {
                graph.Nodes[i].payload = payload;
            }

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
            for (var i = 0; i < graph.Nodes.Count; i++)
            {
                graph.Nodes[i].payload = null;
            }
        }
    }
}
