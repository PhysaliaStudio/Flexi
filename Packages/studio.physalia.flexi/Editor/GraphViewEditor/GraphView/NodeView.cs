using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphViewEditor
{
    using static UnityEditor.Experimental.GraphView.Port;
    using NodeData = Node;
    using Port = UnityEditor.Experimental.GraphView.Port;
    using PortData = Port;

    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        private const string USS_CLASS_ENTRY_NODE = "entry-node";
        private const string USS_CLASS_FLOW_CONTROL_NODE = "flow-control-node";
        private const string USS_CLASS_PROCESS_NODE = "process-node";
        private const string USS_CLASS_CONSTANT_NODE = "constant-node";
        private const string USS_CLASS_MACRO_NODE = "macro-node";
        private const string USS_CLASS_MISSING_NODE = "missing-node";

        private const string USS_CLASS_COMMON_VALUE_NODE = "common-value-node";
        private const string USS_CLASS_STRING_NODE = "string-node";
        private const string USS_CLASS_INTEGER_NODE = "integer-node";
        private const string USS_CLASS_OTHER_NODE = "other-node";

        private static readonly Color MISSING_PORT_COLOR = new(1f, 0f, 0f);

        private readonly NodeData nodeData;
        private readonly AbilityGraphEditorWindow window;
        private readonly AbilityGraphView graphView;
        private readonly Dictionary<PortData, Port> portDataToViewTable = new();
        private readonly Dictionary<Port, PortData> portViewToDataTable = new();

        public NodeData NodeData => nodeData;

        public NodeView(NodeData nodeData, AbilityGraphEditorWindow window, AbilityGraphView graphView)
            : base(EditorConst.PackagePath + "Editor/GraphViewEditor/UiAssets/Node.uxml")
        {
            UseDefaultStyling();

            this.nodeData = nodeData;
            this.window = window;
            this.graphView = graphView;
            HandleNodeStyles(nodeData);
            CreatePorts();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            switch (nodeData)
            {
                default:
                    evt.menu.AppendAction("Edit Script", action =>
                    {
                        Utility.OpenScriptOfType(nodeData.GetType());
                    });
                    evt.menu.AppendSeparator();
                    break;
                case SubgraphNode:
                case GraphInputNode:
                case GraphOutputNode:
                    break;
            }

            base.BuildContextualMenu(evt);
        }

        private void HandleNodeStyles(NodeData node)
        {
            switch (node)
            {
                default:
                    HandleOtherNodeStyles(node);
                    break;
                case EntryNode:
                    AddToClassList(USS_CLASS_ENTRY_NODE);
                    break;
                case SubgraphNode:
                case GraphInputNode:
                case GraphOutputNode:
                    AddToClassList(USS_CLASS_MACRO_NODE);
                    break;
                case ProcessNode:
                    AddToClassList(USS_CLASS_PROCESS_NODE);
                    break;
                case FlowNode:
                    AddToClassList(USS_CLASS_FLOW_CONTROL_NODE);
                    break;
                case MissingNode missingNode:
                    title = missingNode.TypeName;
                    AddToClassList(USS_CLASS_MISSING_NODE);
                    break;
            }

            switch (node)
            {
                default:
                    title = GetNodeName(node.GetType());
                    break;
                case SubgraphNode subgraphNode:
                    title = subgraphNode.key;
                    break;
                case TrueNode:
                    title = "TRUE";
                    break;
                case FalseNode:
                    title = "FALSE";
                    break;
                case EqualNode:
                    title = "==";
                    break;
                case NotEqualNode:
                    title = "!=";
                    break;
                case LessNode:
                    title = "<";
                    break;
                case GreaterNode:
                    title = ">";
                    break;
                case LessOrEqualNode:
                    title = "<=";
                    break;
                case GreaterOrEqualNode:
                    title = ">=";
                    break;
                case AndNode:
                    title = "AND";
                    break;
                case OrNode:
                    title = "OR";
                    break;
                case XorNode:
                    title = "XOR";
                    break;
                case NotNode:
                    title = "NOT";
                    break;
            }
        }

        private void HandleOtherNodeStyles(NodeData node)
        {
            switch (node)
            {
                default:
                    AddToClassList(USS_CLASS_OTHER_NODE);
                    break;
                case IntegerNode:
                    AddToClassList(USS_CLASS_COMMON_VALUE_NODE);
                    AddToClassList(USS_CLASS_INTEGER_NODE);
                    break;
                case StringNode:
                    AddToClassList(USS_CLASS_COMMON_VALUE_NODE);
                    AddToClassList(USS_CLASS_STRING_NODE);
                    break;
                case TrueNode:
                case FalseNode:
                case EqualNode:
                case NotEqualNode:
                case LessNode:
                case GreaterNode:
                case LessOrEqualNode:
                case GreaterOrEqualNode:
                case AndNode:
                case OrNode:
                case XorNode:
                case NotNode:
                    AddToClassList(USS_CLASS_COMMON_VALUE_NODE);
                    break;
            }
        }

        public override Port InstantiatePort(Orientation orientation, Direction direction, Capacity capacity, Type type)
        {
            return new PortView(orientation, direction, capacity, type);
        }

        private void CreatePorts()
        {
            foreach (PortData portData in nodeData.Ports)
            {
                if (portData is Inport)
                {
                    Port port = InstantiatePort(Orientation.Horizontal, Direction.Input, Capacity.Multi, portData.ValueType);
                    port.portName = GetPortPrettyName(portData.Name);
                    inputContainer.Add(port);

                    if (portData is MissingInport)
                    {
                        port.portColor = MISSING_PORT_COLOR;
                    }

                    portDataToViewTable.Add(portData, port);
                    portViewToDataTable.Add(port, portData);
                }

                if (portData is Outport)
                {
                    Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Capacity.Multi, portData.ValueType);
                    port.portName = GetPortPrettyName(portData.Name);
                    outputContainer.Add(port);

                    if (portData is MissingOutport)
                    {
                        port.portColor = MISSING_PORT_COLOR;
                    }

                    portDataToViewTable.Add(portData, port);
                    portViewToDataTable.Add(port, portData);
                }
            }

            FieldInfo[] fields = nodeData.GetType().GetFieldsIncludeBasePrivate();
            for (var i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                Type type = field.FieldType;
                if (type.InstanceOfGenericType(typeof(Variable<>)))
                {
                    var variable = field.GetValue(nodeData) as Variable;
                    VisualElement variableField = VariableFieldFactory.Create(field.Name, variable, window);
                    extensionContainer.Add(variableField);
                }
            }

            // Unity rule: After adding custom elements to the extensionContainer, call this method in order for them to become visible.
            RefreshExpandedState();
        }

        private static string GetNodeName(Type type)
        {
            NodeCategory nodeCategory = type.GetCustomAttribute<NodeCategory>();
            if (nodeCategory != null && !string.IsNullOrEmpty(nodeCategory.Name))
            {
                return nodeCategory.Name;
            }
            else if (type.Name.EndsWith("Node"))
            {
                return type.Name[0..^4];
            }
            else
            {
                return type.Name;
            }
        }

        private static string GetPortPrettyName(string portName)
        {
            if (portName.EndsWith("Port"))
            {
                return portName[0..^4];
            }

            return portName;
        }

        public void DestroyPort(PortData portData)
        {
            Port port = GetPortView(portData);

            if (portData is Outport outport)
            {
                portData.Node.RemoveOutport(outport);
                outputContainer.Remove(port);
            }

            if (portData is Inport inport)
            {
                portData.Node.RemoveInport(inport);
                inputContainer.Remove(port);
            }

            portDataToViewTable.Remove(portData);
            portViewToDataTable.Remove(port);
        }

        public Port GetPortView(PortData portData)
        {
            if (portDataToViewTable.TryGetValue(portData, out Port port))
            {
                return port;
            }

            return null;
        }

        public PortData GetPortData(Port port)
        {
            if (portViewToDataTable.TryGetValue(port, out PortData portData))
            {
                return portData;
            }

            return null;
        }

        public PortData GetPortData(string name)
        {
            return nodeData.GetPort(name);
        }

        public void AddPort(string name, Direction direction, Type type)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            PortData portData = nodeData.GetPort(name);
            if (portData != null)
            {
                return;
            }

            if (direction == Direction.Input)
            {
                portData = nodeData.CreateInportWithArgumentType(type, name, true);
            }
            else if (direction == Direction.Output)
            {
                portData = nodeData.CreateOutportWithArgumentType(type, name, true);
            }

            AddPortView(portData);
        }

        private void AddPortView(PortData portData, int index = -1)
        {
            if (portData is Inport)
            {
                Port port = InstantiatePort(Orientation.Horizontal, Direction.Input, Capacity.Multi, portData.ValueType);
                port.portName = GetPortPrettyName(portData.Name);
                if (index == -1)
                {
                    inputContainer.Add(port);
                }
                else
                {
                    inputContainer.Insert(index, port);
                }

                if (portData is MissingInport)
                {
                    port.portColor = MISSING_PORT_COLOR;
                }

                portDataToViewTable.Add(portData, port);
                portViewToDataTable.Add(port, portData);
            }
            else if (portData is Outport)
            {
                Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Capacity.Multi, portData.ValueType);
                port.portName = GetPortPrettyName(portData.Name);
                if (index == -1)
                {
                    outputContainer.Add(port);
                }
                else
                {
                    outputContainer.Insert(index, port);
                }

                if (portData is MissingOutport)
                {
                    port.portColor = MISSING_PORT_COLOR;
                }

                portDataToViewTable.Add(portData, port);
                portViewToDataTable.Add(port, portData);
            }
        }

        public void RemovePort(Direction direction, int index)
        {
            if (direction == Direction.Input)
            {
                Inport inport = nodeData.GetDynamicInport(index);
                if (inport == null)
                {
                    return;
                }

                nodeData.RemoveInport(inport);
                RemovePortView(inport);
            }
            else if (direction == Direction.Output)
            {
                Outport outport = nodeData.GetDynamicOutport(index);
                if (outport == null)
                {
                    return;
                }

                nodeData.RemoveOutport(outport);
                RemovePortView(outport);
            }
        }

        private void RemovePortView(PortData portData)
        {
            Port portView = GetPortView(portData);
            if (portView == null)
            {
                return;
            }

            // Remove all connected edges
            graphView.RemoveAllEdgeViewsFromPortView(portView);

            // Remove the port
            if (portView.direction == Direction.Input)
            {
                inputContainer.Remove(portView);
            }
            else if (portView.direction == Direction.Output)
            {
                outputContainer.Remove(portView);
            }

            portDataToViewTable.Remove(portData);
            portViewToDataTable.Remove(portView);
        }

        public void ChangePortIndex(Direction direction, int index1, int index2)
        {
            if (direction == Direction.Input)
            {
                PortData portData = nodeData.GetDynamicInport(index1);
                nodeData.InsertOrMoveDynamicPort(index2, portData);

                Port port = GetPortView(portData);
                inputContainer.Remove(port);

                int portNewIndex = index2 + nodeData.GetCountOfStaticInport();
                inputContainer.Insert(portNewIndex, port);
            }
            else if (direction == Direction.Output)
            {
                PortData portData = nodeData.GetDynamicOutport(index1);
                nodeData.InsertOrMoveDynamicPort(index2, portData);

                Port port = GetPortView(portData);
                outputContainer.Remove(port);

                int portNewIndex = index2 + nodeData.GetCountOfStaticOutport();
                outputContainer.Insert(portNewIndex, port);
            }
        }

        public bool TryRenamePort(string oldName, string newName)
        {
            // Ensure the port with the old name exists.
            PortData portData = nodeData.GetPort(oldName);
            if (portData == null)
            {
                Logger.Error($"The port with the old name '{oldName}' doesn't exist!");
                return false;
            }

            // Ensure the new name is not used.
            PortData portWithNewName = nodeData.GetPort(newName);
            if (portWithNewName != null)
            {
                Logger.Error($"The new name '{newName}' has been used!");
                return false;
            }

            bool success = nodeData.TryRenamePort(oldName, newName);
            if (!success)
            {
                return false;
            }

            Port port = portDataToViewTable[portData];
            port.portName = newName;
            return true;
        }

        public void ChangePortType(string name, Type type)
        {
            PortData oldPortData = nodeData.GetPort(name);
            RemovePortView(oldPortData);

            nodeData.ChangeDynamicPortType(name, type);

            PortData newPortData = nodeData.GetPort(name);
            int index = nodeData.GetIndexOfDynamicPort(newPortData);

            if (newPortData is Inport)
            {
                index += nodeData.GetCountOfStaticInport();
            }
            else if (newPortData is Outport)
            {
                index += nodeData.GetCountOfStaticOutport();
            }

            AddPortView(newPortData, index);
        }
    }
}
