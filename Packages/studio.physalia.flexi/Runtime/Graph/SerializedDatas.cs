using System;
using System.Collections.Generic;
using UnityEngine;

namespace Physalia.Flexi
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

            IReadOnlyList<Outport> dynamicOutports = node.DynamicOutports;
            for (var i = 0; i < dynamicOutports.Count; i++)
            {
                Port port = dynamicOutports[i];
                Type dataType = port.GetType().GenericTypeArguments[0];
                portDatas.Add(new PortData { name = port.Name, type = dataType.FullName });
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

            IReadOnlyList<Inport> dynamicInports = node.DynamicInports;
            for (var i = 0; i < dynamicInports.Count; i++)
            {
                Port port = dynamicInports[i];
                Type dataType = port.GetType().GenericTypeArguments[0];
                portDatas.Add(new PortData { name = port.Name, type = dataType.FullName });
            }
        }
    }

    internal class PortData
    {
        public string name;
        public string type;
    }
}
