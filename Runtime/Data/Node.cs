using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Physalia.AbilityFramework
{
    [JsonConverter(typeof(NodeConverter))]
    public abstract class Node
    {
        public int id;
        public Vector2 position;

        // These internal ports will be set in NodeFactory
        [NonSerialized]
        private readonly Dictionary<string, Port> ports = new();
        [NonSerialized]
        private readonly Dictionary<string, Inport> inports = new();
        [NonSerialized]
        private readonly Dictionary<string, Outport> outports = new();

        internal AbilityInstance instance;

        public IEnumerable<Port> Ports => ports.Values;
        public IEnumerable<Inport> Inports => inports.Values;
        public IEnumerable<Outport> Outports => outports.Values;
        public AbilityInstance Instance => instance;

        internal void AddInport(string name, Inport inport)
        {
            ports.Add(name, inport);
            inports.Add(name, inport);
        }

        internal void AddOutport(string name, Outport outport)
        {
            ports.Add(name, outport);
            outports.Add(name, outport);
        }

        public Port GetPort(string name)
        {
            if (ports.TryGetValue(name, out Port port))
            {
                return port;
            }

            return null;
        }

        public Inport GetInput(string name)
        {
            if (inports.TryGetValue(name, out Inport inport))
            {
                return inport;
            }

            return null;
        }

        public Outport GetOutput(string name)
        {
            if (outports.TryGetValue(name, out Outport outport))
            {
                return outport;
            }

            return null;
        }

        public void DisconnectAllPorts()
        {
            foreach (Port port in Ports)
            {
                port.DisconnectAll();
            }
        }

        public T GetPayload<T>() where T : class
        {
            if (instance == null)
            {
                return null;
            }

            return instance.Payload as T;
        }

        public virtual bool CheckNodeContext(NodeContext nodeContext)
        {
            return false;
        }

        protected internal virtual void Reset()
        {

        }
    }
}
