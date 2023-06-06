using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphViewEditor
{
    using NodeData = Physalia.Flexi.Node;
    using PortData = Physalia.Flexi.Port;
    using EdgeData = Physalia.Flexi.Edge;
    using Node = UnityEditor.Experimental.GraphView.Node;
    using Port = UnityEditor.Experimental.GraphView.Port;
    using Edge = UnityEditor.Experimental.GraphView.Edge;

    public class AbilityGraphView : GraphView
    {
        // Note: BUG!!! Derive for preventing bug, otherwise it cannot show the grid
        private class TempGridBackground : GridBackground { }

        private readonly AbilityGraph abilityGraph;
        private readonly AbilityGraphEditorWindow window;

        private readonly Dictionary<NodeData, NodeView> nodeTable = new();
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
            NodeData nodeData = abilityGraph.AddNewNode(nodeType);
            nodeData.position = position;
            NodeView nodeView = CreateNodeElement(nodeData);
            nodeTable.Add(nodeData, nodeView);
        }

        public void CreateMacroNode(MacroLibrary macroLibrary, string macroKey, Vector2 position)
        {
            SubgraphNode node = macroLibrary.AddMacroNode(abilityGraph, macroKey);
            node.position = position;

            NodeView nodeView = CreateNodeElement(node);
            nodeTable.Add(node, nodeView);
        }

        public void AddNode(NodeData nodeData)
        {
            if (nodeData == null)
            {
                return;
            }

            abilityGraph.AddNode(nodeData);
            NodeView nodeView = CreateNodeElement(nodeData);
            nodeTable.Add(nodeData, nodeView);
        }

        private NodeView CreateNodeElement(NodeData nodeData)
        {
            var nodeView = new NodeView(nodeData, window, this);
            nodeView.SetPosition(new Rect(nodeData.position, nodeView.GetPosition().size));
            AddElement(nodeView);
            return nodeView;
        }

        public void RemoveNode(NodeData nodeData)
        {
            abilityGraph.RemoveNode(nodeData);
            if (nodeTable.TryGetValue(nodeData, out NodeView nodeView))
            {
                nodeTable.Remove(nodeData);
                nodeView.RemoveFromHierarchy();
            }
        }

        public void AddEdge(EdgeData edgeData)
        {
            NodeData nodeData1 = abilityGraph.GetNode(edgeData.id1);
            NodeData nodeData2 = abilityGraph.GetNode(edgeData.id2);
            PortData portData1 = nodeData1.GetPort(edgeData.port1);
            PortData portData2 = nodeData2.GetPort(edgeData.port2);
            portData1.Connect(portData2);

            NodeView currentNodeView = nodeTable[nodeData1];
            NodeView anotherNodeView = nodeTable[nodeData2];
            Port port1 = currentNodeView.GetPortView(portData1);
            Port port2 = anotherNodeView.GetPortView(portData2);
            EdgeView edgeView = port1.ConnectTo<EdgeView>(port2);
            AddElement(edgeView);
        }

        public void RemoveEdgeView(Edge edge)
        {
            edge.input?.Disconnect(edge);
            edge.output?.Disconnect(edge);
            RemoveElement(edge);
        }

        public void RemoveAllEdgeViewsFromPortView(Port port)
        {
            var edges = new List<Edge>(port.connections);
            for (var i = 0; i < edges.Count; i++)
            {
                RemoveEdgeView(edges[i]);
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

        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            foreach (Port port in ports.ToList())
            {
                if (startAnchor.node == port.node || startAnchor.direction == port.direction)
                {
                    continue;
                }

                bool canCast;
                if (startAnchor.direction == Direction.Output)
                {
                    canCast = ConversionUtility.CanConvert(startAnchor.portType, port.portType);
                }
                else
                {
                    canCast = ConversionUtility.CanConvert(port.portType, startAnchor.portType);
                }

                if (canCast)
                {
                    compatiblePorts.Add(port);
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

            IReadOnlyList<NodeData> nodeDatas = abilityGraph.Nodes;
            if (!abilityGraph.HasSubgraphElement() && nodeDatas.Count == 0)
            {
                return graphView;
            }

            // Create subgraph input/output if necessary
            if (abilityGraph.GraphInputNode != null)
            {
                NodeData nodeData = abilityGraph.GraphInputNode;

                var nodeView = new NodeView(nodeData, window, graphView);
                nodeView.SetPosition(new Rect(nodeData.position, nodeView.GetPosition().size));
                graphView.AddElement(nodeView);
                graphView.nodeTable.Add(nodeData, nodeView);
            }

            if (abilityGraph.GraphOutputNode != null)
            {
                NodeData nodeData = abilityGraph.GraphOutputNode;

                var nodeView = new NodeView(nodeData, window, graphView);
                nodeView.SetPosition(new Rect(nodeData.position, nodeView.GetPosition().size));
                graphView.AddElement(nodeView);
                graphView.nodeTable.Add(nodeData, nodeView);
            }

            // Create nodes
            for (var i = 0; i < nodeDatas.Count; i++)
            {
                NodeData nodeData = nodeDatas[i];

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
            var unhandledNodes = new HashSet<NodeData>();
            if (abilityGraph.GraphInputNode != null)
            {
                unhandledNodes.Add(abilityGraph.GraphInputNode);
            }
            if (abilityGraph.GraphOutputNode != null)
            {
                unhandledNodes.Add(abilityGraph.GraphOutputNode);
            }
            for (var i = 0; i < nodeDatas.Count; i++)
            {
                unhandledNodes.Add(nodeDatas[i]);
            }

            NodeData current = abilityGraph.GetFirstNode();
            SearchAllNodes(current, ref graphView, ref unhandledNodes);

            return graphView;
        }

        private static void SearchAllNodes(NodeData current, ref AbilityGraphView graphView, ref HashSet<NodeData> unhandledNodes)
        {
            if (!unhandledNodes.Contains(current))
            {
                return;
            }

            foreach (PortData currentPort in current.Ports)
            {
                IReadOnlyList<PortData> connections = currentPort.GetConnections();
                if (connections.Count == 0)
                {
                    continue;
                }

                foreach (PortData anotherPort in connections)
                {
                    if (!unhandledNodes.Contains(anotherPort.Node))
                    {
                        continue;
                    }

                    NodeView currentNodeView = graphView.nodeTable[current];
                    NodeView anotherNodeView = graphView.nodeTable[anotherPort.Node];

                    Port port1 = currentNodeView.GetPortView(currentPort);
                    Port port2 = anotherNodeView.GetPortView(anotherPort);

                    EdgeView edgeView = port1.ConnectTo<EdgeView>(port2);
                    graphView.AddElement(edgeView);
                }
            }

            unhandledNodes.Remove(current);

            foreach (PortData currentPort in current.Ports)
            {
                IReadOnlyList<PortData> connections = currentPort.GetConnections();
                if (connections.Count == 0)
                {
                    continue;
                }

                foreach (PortData anotherPort in connections)
                {
                    SearchAllNodes(anotherPort.Node, ref graphView, ref unhandledNodes);
                }
            }
        }
    }
}
