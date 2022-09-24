using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using EdgeView = UnityEditor.Experimental.GraphView.Edge;
using PortView = UnityEditor.Experimental.GraphView.Port;

namespace Physalia.AbilitySystem.GraphViewEditor
{
    public class AbilityGraphView : GraphView
    {
        // Note: BUG!!! Derive for preventing bug, otherwise it cannot show the grid
        private class TempGridBackground : GridBackground { }

        private readonly AbilityGraph abilityGraph;

        public AbilityGraphView() : this(new AbilityGraph())
        {

        }

        public AbilityGraphView(AbilityGraph abilityGraph) : base()
        {
            this.abilityGraph = abilityGraph;

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            Insert(0, new TempGridBackground());
            this.StretchToParentSize();
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new ClickSelector());
            this.AddManipulator(new RectangleSelector());

            var searchWindowProvider = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
            searchWindowProvider.Initialize(this);

            nodeCreationRequest = context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindowProvider);
            };
        }

        public AbilityGraph GetAbilityGraph()
        {
            return abilityGraph;
        }

        public void CreateNewNode(Type nodeType, Vector2 position)
        {
            Node node = abilityGraph.AddNewNode(nodeType);
            var nodeView = new NodeView(node);
            nodeView.SetPosition(new Rect(position, nodeView.GetPosition().size));
            AddElement(nodeView);
        }

        public override List<PortView> GetCompatiblePorts(PortView startAnchor, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<PortView>();
            foreach (PortView portView in ports.ToList())
            {
                if (startAnchor.node == portView.node || startAnchor.direction == portView.direction)
                {
                    continue;
                }

                if (startAnchor.portType != portView.portType &&
                    !startAnchor.portType.IsAssignableFrom(portView.portType) && !portView.portType.IsAssignableFrom(startAnchor.portType))
                {
                    continue;
                }

                compatiblePorts.Add(portView);
            }
            return compatiblePorts;
        }

        public static AbilityGraphView Create(AbilityGraph abilityGraph)
        {
            var graphView = new AbilityGraphView(abilityGraph);

            IReadOnlyList<Node> nodes = abilityGraph.Nodes;
            if (nodes.Count == 0)
            {
                return graphView;
            }

            var nodeTable = new Dictionary<Node, NodeView>();

            // Create nodes
            for (var i = 0; i < nodes.Count; i++)
            {
                Node nodeData = nodes[i];

                var node = new NodeView(nodeData);
                node.SetPosition(new Rect(nodeData.position, node.GetPosition().size));
                graphView.AddElement(node);

                if (nodeData.id <= 0)
                {
                    nodeData.id = UnityEngine.Random.Range(1, 1000000);
                }

                nodeTable.Add(nodeData, node);
            }

            // Create edges with DFS
            var unhandledNodes = new HashSet<Node>();
            for (var i = 0; i < nodes.Count; i++)
            {
                unhandledNodes.Add(nodes[i]);
            }

            Node current = nodes[0];
            SearchAllNodes(current, nodeTable, ref graphView, ref unhandledNodes);

            return graphView;
        }

        private static void SearchAllNodes(Node current, Dictionary<Node, NodeView> nodeTable,
            ref AbilityGraphView graphView, ref HashSet<Node> unhandledNodes)
        {
            if (!unhandledNodes.Contains(current))
            {
                return;
            }

            foreach (Port currentPort in current.Ports)
            {
                IReadOnlyList<Port> connections = currentPort.GetConnections();
                if (connections.Count == 0)
                {
                    continue;
                }

                foreach (Port anotherPort in connections)
                {
                    if (!unhandledNodes.Contains(anotherPort.Node))
                    {
                        continue;
                    }

                    NodeView currentNodeView = nodeTable[current];
                    NodeView anotherNodeView = nodeTable[anotherPort.Node];

                    PortView portView1 = currentNodeView.GetPortView(currentPort);
                    PortView portView2 = anotherNodeView.GetPortView(anotherPort);

                    EdgeView edgeView = portView1.ConnectTo(portView2);
                    graphView.AddElement(edgeView);
                }

                unhandledNodes.Remove(current);

                foreach (Port anotherPort in connections)
                {
                    SearchAllNodes(anotherPort.Node, nodeTable, ref graphView, ref unhandledNodes);
                }
            }
        }
    }
}
