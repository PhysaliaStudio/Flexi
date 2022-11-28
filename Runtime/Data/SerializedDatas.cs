using System;
using System.Collections.Generic;
using UnityEngine;

namespace Physalia.AbilityFramework
{
    internal class GraphInputData
    {
        public Vector2 position;
        public List<PortData> portDatas = new();

        internal GraphInputData()
        {

        }

        internal GraphInputData(GraphInputNode node)
        {
            position = node.position;
            foreach (Port port in node.Outports)
            {
                if (port == node.next)
                {
                    continue;
                }

                Type dataType = port.GetType().GenericTypeArguments[0];
                portDatas.Add(new PortData { name = port.name, type = dataType.FullName });
            }
        }
    }

    internal class GraphOutputData
    {
        public Vector2 position;
        public List<PortData> portDatas = new();

        internal GraphOutputData()
        {

        }

        internal GraphOutputData(GraphOutputNode node)
        {
            position = node.position;
            foreach (Port port in node.Inports)
            {
                if (port == node.previous)
                {
                    continue;
                }

                Type dataType = port.GetType().GenericTypeArguments[0];
                portDatas.Add(new PortData { name = port.name, type = dataType.FullName });
            }
        }
    }

    internal class PortData
    {
        public string name;
        public string type;
    }
}
