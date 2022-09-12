using Newtonsoft.Json;
using UnityEngine;

namespace Physalia.AbilitySystem
{
    [JsonConverter(typeof(NodeConverter))]
    public abstract class Node
    {
        public int id;
        public Vector2 position;
    }
}
