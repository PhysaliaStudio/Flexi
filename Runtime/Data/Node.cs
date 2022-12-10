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

        internal void RemoveInport(Inport inport)
        {
            inport.DisconnectAll();

            string name = inport.Name;
            ports.Remove(name);
            inports.Remove(name);
        }

        internal void AddOutport(string name, Outport outport)
        {
            ports.Add(name, outport);
            outports.Add(name, outport);
        }

        internal void RemoveOutport(Outport outport)
        {
            outport.DisconnectAll();

            string name = outport.Name;
            ports.Remove(name);
            outports.Remove(name);
        }

        public Port GetPort(string name)
        {
            if (ports.TryGetValue(name, out Port port))
            {
                return port;
            }

            return null;
        }

        public Inport GetInport(string name)
        {
            if (inports.TryGetValue(name, out Inport inport))
            {
                return inport;
            }

            return null;
        }

        public Outport GetOutport(string name)
        {
            if (outports.TryGetValue(name, out Outport outport))
            {
                return outport;
            }

            return null;
        }

        public bool TryRenamePort(string oldName, string newName)
        {
            // Ensure the port with the old name exists.
            Port port = GetPort(oldName);
            if (port == null)
            {
                Logger.Error($"The port with the old name '{oldName}' doesn't exist!");
                return false;
            }

            // Ensure the new name is not used.
            Port portWithNewName = GetPort(newName);
            if (portWithNewName != null)
            {
                Logger.Error($"The new name '{newName}' has been used!");
                return false;
            }

            port.Name = newName;
            ports.Remove(oldName);
            ports.Add(newName, port);

            if (port is Inport inport)
            {
                inports.Remove(oldName);
                inports.Add(oldName, inport);
            }
            else if (port is Outport outport)
            {
                outports.Remove(oldName);
                outports.Add(oldName, outport);
            }

            return true;
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

        public virtual bool CheckNodeContext(IResumeContext resumeContext)
        {
            return false;
        }

        protected internal virtual void Reset()
        {

        }
    }
}
