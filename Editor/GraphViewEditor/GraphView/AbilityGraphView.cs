using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using EdgeView = UnityEditor.Experimental.GraphView.Edge;
using PortView = UnityEditor.Experimental.GraphView.Port;

namespace Physalia.Flexi.GraphViewEditor
{
    public class AbilityGraphView : GraphView
    {
        // Note: BUG!!! Derive for preventing bug, otherwise it cannot show the grid
        private class TempGridBackground : GridBackground { }

        private readonly AbilityGraph abilityGraph;
        private readonly AbilityGraphEditorWindow window;

        private readonly Dictionary<Node, NodeView> nodeTable = new();
        private Vector2 lastContextPosition;

        public Vector2 LastContextPosition => lastContextPosition;

        public AbilityGraphView(AbilityGraphEditorWindow window) : this(new AbilityGraph(), window)
        {

        }

        public AbilityGraphView(AbilityGraph abilityGraph, AbilityGraphEditorWindow window) : base()
        {
            this.abilityGraph = abilityGraph;
            this.window = window;

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

        public void CreateMacroNode(MacroLibrary macroLibrary, string macroKey, Vector2 position)
        {
            SubgraphNode node = macroLibrary.AddMacroNode(abilityGraph, macroKey);
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
            var nodeView = new NodeView(node, window, this);
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

        public void RemoveEdgeView(EdgeView edgeView)
        {
            edgeView.input?.Disconnect(edgeView);
            edgeView.output?.Disconnect(edgeView);
            RemoveElement(edgeView);
        }

        public void RemoveAllEdgeViewsFromPortView(PortView portView)
        {
            var edgeViews = new List<EdgeView>(portView.connections);
            for (var i = 0; i < edgeViews.Count; i++)
            {
                RemoveEdgeView(edgeViews[i]);
            }
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

        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            if (selection.Count == 1 && selection[0] is NodeView nodeView)
            {
                window.ShowNodeInspector(nodeView);
            }
            else
            {
                window.HideNodeInspector();
            }
        }

        public override void RemoveFromSelection(ISelectable selectable)
        {
            base.RemoveFromSelection(selectable);
            if (selection.Count == 1 && selection[0] is NodeView nodeView)
            {
                window.ShowNodeInspector(nodeView);
            }
            else
            {
                window.HideNodeInspector();
            }
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            window.HideNodeInspector();
        }

        public static AbilityGraphView Create(AbilityGraph abilityGraph, AbilityGraphEditorWindow window)
        {
            var graphView = new AbilityGraphView(abilityGraph, window);

            IReadOnlyList<Node> nodes = abilityGraph.Nodes;
            if (!abilityGraph.HasSubgraphElement() && nodes.Count == 0)
            {
                return graphView;
            }

            // Create subgraph input/output if necessary
            if (abilityGraph.GraphInputNode != null)
            {
                Node node = abilityGraph.GraphInputNode;

                var nodeView = new NodeView(node, window, graphView);
                nodeView.SetPosition(new Rect(node.position, nodeView.GetPosition().size));
                graphView.AddElement(nodeView);
                graphView.nodeTable.Add(node, nodeView);
            }

            if (abilityGraph.GraphOutputNode != null)
            {
                Node node = abilityGraph.GraphOutputNode;

                var nodeView = new NodeView(node, window, graphView);
                nodeView.SetPosition(new Rect(node.position, nodeView.GetPosition().size));
                graphView.AddElement(nodeView);
                graphView.nodeTable.Add(node, nodeView);
            }

            // Create nodes
            for (var i = 0; i < nodes.Count; i++)
            {
                Node nodeData = nodes[i];

                var node = new NodeView(nodeData, window, graphView);
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
            if (abilityGraph.GraphInputNode != null)
            {
                unhandledNodes.Add(abilityGraph.GraphInputNode);
            }
            if (abilityGraph.GraphOutputNode != null)
            {
                unhandledNodes.Add(abilityGraph.GraphOutputNode);
            }
            for (var i = 0; i < nodes.Count; i++)
            {
                unhandledNodes.Add(nodes[i]);
            }

            Node current = abilityGraph.GetFirstNode();
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
