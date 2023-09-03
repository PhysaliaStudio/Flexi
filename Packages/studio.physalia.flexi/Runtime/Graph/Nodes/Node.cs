using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Physalia.Flexi
{
    [JsonConverter(typeof(NodeConverter))]
    public abstract class Node
    {
        public int id;
        public Vector2 position;

        // These internal ports will be set in NodeFactory
        [NonSerialized]
        private readonly Dictionary<string, Port> portTable = new();
        [NonSerialized]
        private readonly Dictionary<string, Inport> inportTable = new();
        [NonSerialized]
        private readonly Dictionary<string, Outport> outportTable = new();

        private readonly List<Inport> inports = new();
        private readonly List<Outport> outports = new();
        private readonly List<Inport> dynamicInports = new();
        private readonly List<Outport> dynamicOutports = new();

        internal AbilityFlow flow;

        public IEnumerable<Port> Ports => portTable.Values;
        public IReadOnlyList<Inport> Inports => inports;
        public IReadOnlyList<Outport> Outports => outports;
        public IReadOnlyList<Inport> DynamicInports => dynamicInports;
        public IReadOnlyList<Outport> DynamicOutports => dynamicOutports;

        #region AbilityInstance Members
        public AbilityFlow Flow => flow;
        public Ability Ability => flow?.Ability;
        public Actor Actor => flow?.Actor;
        public AbilityDataContainer Container => flow?.Ability?.Container;
        #endregion

        internal void AddInport(string name, Inport inport)
        {
            portTable.Add(name, inport);
            inportTable.Add(name, inport);

            inports.Add(inport);
            if (inport.IsDynamic)
            {
                dynamicInports.Add(inport);
            }
        }

        internal void RemoveInport(Inport inport)
        {
            inport.DisconnectAll();

            string name = inport.Name;
            portTable.Remove(name);
            inportTable.Remove(name);

            inports.Remove(inport);
            if (inport.IsDynamic)
            {
                dynamicInports.Remove(inport);
            }
        }

        internal void AddOutport(string name, Outport outport)
        {
            portTable.Add(name, outport);
            outportTable.Add(name, outport);

            outports.Add(outport);
            if (outport.IsDynamic)
            {
                dynamicOutports.Add(outport);
            }
        }

        internal void RemoveOutport(Outport outport)
        {
            outport.DisconnectAll();

            string name = outport.Name;
            portTable.Remove(name);
            outportTable.Remove(name);

            outports.Remove(outport);
            if (outport.IsDynamic)
            {
                dynamicOutports.Remove(outport);
            }
        }

        public Port GetPort(string name)
        {
            if (portTable.TryGetValue(name, out Port port))
            {
                return port;
            }

            return null;
        }

        public Inport GetInport(string name)
        {
            if (inportTable.TryGetValue(name, out Inport inport))
            {
                return inport;
            }

            return null;
        }

        public Outport GetOutport(string name)
        {
            if (outportTable.TryGetValue(name, out Outport outport))
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

            // Ensure the port is dynamic
            if (!port.IsDynamic)
            {
                Logger.Error($"The port with the old name '{oldName}' is not dynamic! You can only modify dynamic ports.");
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
            portTable.Remove(oldName);
            portTable.Add(newName, port);

            if (port is Inport inport)
            {
                inportTable.Remove(oldName);
                inportTable.Add(newName, inport);
            }
            else if (port is Outport outport)
            {
                outportTable.Remove(oldName);
                outportTable.Add(newName, outport);
            }

            return true;
        }

        public void ChangeDynamicPortType(string portName, Type newType)
        {
            Port oldPort = GetPort(portName);

            //IReadOnlyList<Port> connections = oldPort.GetConnections();  // Cache the connections

            if (oldPort is Inport inport)
            {
                int oldIndex = dynamicInports.IndexOf(inport);  // Cache the index;

                RemoveInport(inport);
                Port newPort = this.CreateInportWithArgumentType(newType, portName, true);
                InsertOrMoveDynamicPort(oldIndex, newPort);
            }
            else if (oldPort is Outport outport)
            {
                int oldIndex = dynamicOutports.IndexOf(outport);  // Cache the index;

                RemoveOutport(outport);
                Port newPort = this.CreateOutportWithArgumentType(newType, portName, true);
                InsertOrMoveDynamicPort(oldIndex, newPort);
            }
        }

        public int GetCountOfStaticInport()
        {
            return inports.Count - dynamicInports.Count;
        }

        public int GetCountOfStaticOutport()
        {
            return outports.Count - dynamicOutports.Count;
        }

        public Inport GetDynamicInport(int index)
        {
            return dynamicInports[index];
        }

        public Outport GetDynamicOutport(int index)
        {
            return dynamicOutports[index];
        }

        public int GetIndexOfDynamicPort(Port port)
        {
            // Ensure the port belong to this node
            if (port.Node != this)
            {
                Logger.Error($"The port with the name '{port.Name}' does not belong to this node!");
                return -1;
            }

            // Ensure the port is dynamic
            if (!port.IsDynamic)
            {
                Logger.Error($"The port with the name '{port.Name}' is not dynamic! You can only modify dynamic ports.");
                return -1;
            }

            if (port is Inport inport)
            {
                return dynamicInports.IndexOf(inport);
            }
            else if (port is Outport outport)
            {
                return dynamicOutports.IndexOf(outport);
            }

            return -1;
        }

        public void SwapDynamicInportIndex(int index1, int index2)
        {
            Inport temp = dynamicInports[index1];
            dynamicInports[index1] = dynamicInports[index2];
            dynamicInports[index2] = temp;
        }

        public void SwapDynamicOutportIndex(int index1, int index2)
        {
            Outport temp = dynamicOutports[index1];
            dynamicOutports[index1] = dynamicOutports[index2];
            dynamicOutports[index2] = temp;
        }

        public void InsertOrMoveDynamicPort(int index, Port port)
        {
            // Ensure the port belong to this node
            if (port.Node != this)
            {
                Logger.Error($"The port with the name '{port.Name}' does not belong to this node!");
                return;
            }

            // Ensure the port is dynamic
            if (!port.IsDynamic)
            {
                Logger.Error($"The port with the name '{port.Name}' is not dynamic! You can only modify dynamic ports.");
                return;
            }

            if (port is Inport inport)
            {
                if (index < 0 || index > dynamicInports.Count)
                {
                    throw new ArgumentException();
                }

                dynamicInports.Remove(inport);
                dynamicInports.Insert(index, inport);
            }
            else if (port is Outport outport)
            {
                if (index < 0 || index > dynamicOutports.Count)
                {
                    throw new ArgumentException();
                }

                dynamicOutports.Remove(outport);
                dynamicOutports.Insert(index, outport);
            }
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
            if (flow == null)
            {
                return null;
            }

            return flow.Payload as T;
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
