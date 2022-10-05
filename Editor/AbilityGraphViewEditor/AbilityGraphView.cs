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

        private readonly Dictionary<Node, NodeView> nodeTable = new();
        private Vector2 lastContextPosition;

        public Vector2 LastContextPosition => lastContextPosition;

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
            node.position = position;
            NodeView nodeView = CreateNodeElement(node);
            nodeTable.Add(node, nodeView);
        }

        public void AddNode(Node node)
        {
            if (node == null)
            {
                return;
            }

            abilityGraph.AddNode(node);
            NodeView nodeView = CreateNodeElement(node);
            nodeTable.Add(node, nodeView);
        }

        private NodeView CreateNodeElement(Node node)
        {
            var nodeView = new NodeView(node);
            nodeView.SetPosition(new Rect(node.position, nodeView.GetPosition().size));
            AddElement(nodeView);
            return nodeView;
        }

        public void RemoveNode(Node node)
        {
            abilityGraph.RemoveNode(node);
            if (nodeTable.TryGetValue(node, out NodeView nodeView))
            {
                nodeTable.Remove(node);
                nodeView.RemoveFromHierarchy();
            }
        }

        public void AddEdge(Edge edge)
        {
            Node node1 = abilityGraph.GetNode(edge.id1);
            Node node2 = abilityGraph.GetNode(edge.id2);
            Port port1 = node1.GetPort(edge.port1);
            Port port2 = node2.GetPort(edge.port2);
            port1.Connect(port2);

            NodeView currentNodeView = nodeTable[node1];
            NodeView anotherNodeView = nodeTable[node2];
            PortView portView1 = currentNodeView.GetPortView(port1);
            PortView portView2 = anotherNodeView.GetPortView(port2);
            EdgeView edgeView = portView1.ConnectTo(portView2);
            AddElement(edgeView);
        }

        public void ValidateNodeIds()
        {
            abilityGraph.HandleInvalidNodeIds();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            var worldPos = ElementAt(0).LocalToWorld(evt.localMousePosition);
            lastContextPosition = ElementAt(1).WorldToLocal(worldPos);
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

                bool canCast;
                if (startAnchor.direction == Direction.Output)
                {
                    canCast = Port.CanPortCast(startAnchor.portType, portView.portType);
                }
                else
                {
                    canCast = Port.CanPortCast(portView.portType, startAnchor.portType);
                }

                if (canCast)
                {
                    compatiblePorts.Add(portView);
                }
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

                graphView.nodeTable.Add(nodeData, node);
            }

            // Create edges with DFS
            var unhandledNodes = new HashSet<Node>();
            for (var i = 0; i < nodes.Count; i++)
            {
                unhandledNodes.Add(nodes[i]);
            }

            Node current = nodes[0];
            SearchAllNodes(current, ref graphView, ref unhandledNodes);

            return graphView;
        }

        private static void SearchAllNodes(Node current, ref AbilityGraphView graphView, ref HashSet<Node> unhandledNodes)
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

                    NodeView currentNodeView = graphView.nodeTable[current];
                    NodeView anotherNodeView = graphView.nodeTable[anotherPort.Node];

                    PortView portView1 = currentNodeView.GetPortView(currentPort);
                    PortView portView2 = anotherNodeView.GetPortView(anotherPort);

                    EdgeView edgeView = portView1.ConnectTo(portView2);
                    graphView.AddElement(edgeView);
                }
            }

            unhandledNodes.Remove(current);

            foreach (Port currentPort in current.Ports)
            {
                IReadOnlyList<Port> connections = currentPort.GetConnections();
                if (connections.Count == 0)
                {
                    continue;
                }

                foreach (Port anotherPort in connections)
                {
                    SearchAllNodes(anotherPort.Node, ref graphView, ref unhandledNodes);
                }
            }
        }
    }
}
