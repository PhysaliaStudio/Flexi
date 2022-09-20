using System.Collections.Generic;
using Newtonsoft.Json;

namespace Physalia.AbilitySystem
{
    [JsonConverter(typeof(GrpahConverter))]
    public class Graph
    {
        internal List<Node> nodes = new();

        public void ReorderNodes()
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                nodes[i].id = i + 1;
            }
        }

        public Node GetNode(int id)
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].id == id)
                {
                    return nodes[i];
                }
            }

            return null;
        }
    }
}
