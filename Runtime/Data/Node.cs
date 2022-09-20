using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Physalia.AbilitySystem
{
    [JsonConverter(typeof(NodeConverter))]
    public abstract class Node
    {
        public int id;
        public Vector2 position;

        // These internal ports will be set in NodeFactory
        [NonSerialized]
        private readonly Dictionary<string, Inport> inports = new();
        [NonSerialized]
        private readonly Dictionary<string, Outport> outports = new();

        internal IEnumerable<Inport> Inports => inports.Values;
        internal IEnumerable<Outport> Outports => outports.Values;

        internal void AddInport(string name, Inport inport)
        {
            inports.Add(name, inport);
        }

        internal void AddOutport(string name, Outport outport)
        {
            outports.Add(name, outport);
        }

        internal Inport GetInput(string name)
        {
            if (inports.TryGetValue(name, out Inport inport))
            {
                return inport;
            }

            return null;
        }

        internal Outport GetOutput(string name)
        {
            if (outports.TryGetValue(name, out Outport outport))
            {
                return outport;
            }

            return null;
        }
    }
}
