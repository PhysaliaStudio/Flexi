using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using PortView = UnityEditor.Experimental.GraphView.Port;

namespace Physalia.AbilityFramework.GraphViewEditor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        private const string USS_CLASS_ENTRY_NODE = "entry-node";
        private const string USS_CLASS_FLOW_CONTROL_NODE = "flow-control-node";
        private const string USS_CLASS_PROCESS_NODE = "process-node";
        private const string USS_CLASS_CONSTANT_NODE = "constant-node";
        private const string USS_CLASS_MACRO_NODE = "macro-node";
        private const string USS_CLASS_MISSING_NODE = "missing-node";

        private const string USS_CLASS_INTEGER_NODE = "integer-node";
        private const string USS_CLASS_STRING_NODE = "string-node";

        private static readonly Color MISSING_PORT_COLOR = new(1f, 0f, 0f);

        private readonly Node node;
        private readonly AbilityGraphEditorWindow window;
        private readonly AbilityGraphView graphView;
        private readonly Dictionary<Port, PortView> portDataToViewTable = new();
        private readonly Dictionary<PortView, Port> portViewToDataTable = new();

        public Node Node => node;

        public NodeView(Node node, AbilityGraphEditorWindow window, AbilityGraphView graphView) : base()
        {
            this.node = node;
            this.window = window;
            this.graphView = graphView;
            HandleNodeStyles(node);
            CreatePorts();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Edit Script", action =>
            {
                Utility.OpenScriptOfType(node.GetType());
            });
            evt.menu.AppendSeparator();
            base.BuildContextualMenu(evt);
        }

        private void HandleNodeStyles(Node node)
        {
            switch (node)
            {
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
                case IfElseNode:
                case ForLoopNode:
                    AddToClassList(USS_CLASS_FLOW_CONTROL_NODE);
                    break;
                case MissingNode missingNode:
                    title = missingNode.TypeName;
                    AddToClassList(USS_CLASS_MISSING_NODE);
                    break;
                case IntegerNode:
                    AddToClassList(USS_CLASS_INTEGER_NODE);
                    break;
                case StringNode:
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
                    AddToClassList(USS_CLASS_CONSTANT_NODE);
                    break;
            }

            switch (node)
            {
                default:
                    title = GetNodePrettyName(node.GetType().Name);
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

        private void HandleConstantNodeStyle()
        {
            titleButtonContainer.style.display = DisplayStyle.None;
            Label label = titleContainer.Query<Label>("title-label").First();
            if (label != null)
            {
                label.style.fontSize = 24f;

                // Because we hide the titleButtonContainer
                // Bug? No effect?
                label.style.marginRight = label.style.marginLeft;
            }
        }

        private void CreatePorts()
        {
            foreach (Port portData in node.Ports)
            {
                if (portData is Inport)
                {
                    PortView port = InstantiatePort(Orientation.Horizontal, Direction.Input, PortView.Capacity.Multi, portData.ValueType);
                    port.portName = GetPortName(portData.Name);
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
                    PortView port = InstantiatePort(Orientation.Horizontal, Direction.Output, PortView.Capacity.Multi, portData.ValueType);
                    port.portName = GetPortName(portData.Name);
                    outputContainer.Add(port);

                    if (portData is MissingOutport)
                    {
                        port.portColor = MISSING_PORT_COLOR;
                    }

                    portDataToViewTable.Add(portData, port);
                    portViewToDataTable.Add(port, portData);
                }
            }

            FieldInfo[] fields = node.GetType().GetFieldsIncludeBasePrivate();
            for (var i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                if (field.FieldType.IsSubclassOf(typeof(Variable)))
                {
                    Type genericType = field.FieldType.GetGenericArguments()[0];
                    CreateVariableField creationMethod = VariableFieldTypeCache.GetCreationMethod(genericType);
                    if (creationMethod == null)
                    {
                        continue;
                    }

                    var variable = field.GetValue(node) as Variable;
                    IVariableField variableField = creationMethod.Invoke(field.Name, variable, window);
                    extensionContainer.Add(variableField.VisualElement);
                }
            }

            // Unity rule: After adding custom elements to the extensionContainer, call this method in order for them to become visible.
            RefreshExpandedState();
        }

        private static string GetNodePrettyName(string nodeName)
        {
            if (nodeName.EndsWith("Node"))
            {
                return nodeName[0..^4];
            }

            return nodeName;
        }

        private string GetPortName(string fieldName)
        {
            if (fieldName.EndsWith("Port"))
            {
                return fieldName.Substring(0, fieldName.Length - 4);
            }

            return fieldName;
        }

        public void DestroyPort(Port port)
        {
            PortView portView = GetPortView(port);

            if (port is Outport outport)
            {
                port.Node.RemoveOutport(outport);
                outputContainer.Remove(portView);
            }

            if (port is Inport inport)
            {
                port.Node.RemoveInport(inport);
                inputContainer.Remove(portView);
            }

            portDataToViewTable.Remove(port);
            portViewToDataTable.Remove(portView);
        }

        public PortView GetPortView(Port port)
        {
            if (portDataToViewTable.TryGetValue(port, out PortView portView))
            {
                return portView;
            }

            return null;
        }

        public Port GetPort(PortView portView)
        {
            if (portViewToDataTable.TryGetValue(portView, out Port port))
            {
                return port;
            }

            return null;
        }

        public Port GetPort(string name)
        {
            return node.GetPort(name);
        }

        public void AddPort(string name, Direction direction, Type type)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            Port port = node.GetPort(name);
            if (port != null)
            {
                return;
            }

            if (direction == Direction.Input)
            {
                port = node.CreateInportWithArgumentType(type, name, true);
            }
            else if (direction == Direction.Output)
            {
                port = node.CreateOutportWithArgumentType(type, name, true);
            }

            AddPortView(port);
        }

        private void AddPortView(Port port, int index = -1)
        {
            if (port is Inport)
            {
                PortView portView = InstantiatePort(Orientation.Horizontal, Direction.Input, PortView.Capacity.Multi, port.ValueType);
                portView.portName = GetPortName(port.Name);
                if (index == -1)
                {
                    inputContainer.Add(portView);
                }
                else
                {
                    inputContainer.Insert(index, portView);
                }

                if (port is MissingInport)
                {
                    portView.portColor = MISSING_PORT_COLOR;
                }

                portDataToViewTable.Add(port, portView);
                portViewToDataTable.Add(portView, port);
            }
            else if (port is Outport)
            {
                PortView portView = InstantiatePort(Orientation.Horizontal, Direction.Output, PortView.Capacity.Multi, port.ValueType);
                portView.portName = GetPortName(port.Name);
                if (index == -1)
                {
                    outputContainer.Add(portView);
                }
                else
                {
                    outputContainer.Insert(index, portView);
                }

                if (port is MissingOutport)
                {
                    portView.portColor = MISSING_PORT_COLOR;
                }

                portDataToViewTable.Add(port, portView);
                portViewToDataTable.Add(portView, port);
            }
        }

        public void RemovePort(Direction direction, int index)
        {
            if (direction == Direction.Input)
            {
                Inport inport = node.GetDynamicInport(index);
                if (inport == null)
                {
                    return;
                }

                node.RemoveInport(inport);
                RemovePortView(inport);
            }
            else if (direction == Direction.Output)
            {
                Outport outport = node.GetDynamicOutport(index);
                if (outport == null)
                {
                    return;
                }

                node.RemoveOutport(outport);
                RemovePortView(outport);
            }
        }

        private void RemovePortView(Port port)
        {
            PortView portView = GetPortView(port);
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

            portDataToViewTable.Remove(port);
            portViewToDataTable.Remove(portView);
        }

        public void ChangePortIndex(Direction direction, int index1, int index2)
        {
            if (direction == Direction.Input)
            {
                Port port = node.GetDynamicInport(index1);
                node.InsertOrMoveDynamicPort(index2, port);

                PortView portView = GetPortView(port);
                inputContainer.Remove(portView);

                int portViewNewIndex = index2 + node.GetCountOfStaticInport();
                inputContainer.Insert(portViewNewIndex, portView);
            }
            else if (direction == Direction.Output)
            {
                Port port = node.GetDynamicOutport(index1);
                node.InsertOrMoveDynamicPort(index2, port);

                PortView portView = GetPortView(port);
                outputContainer.Remove(portView);

                int portViewNewIndex = index2 + node.GetCountOfStaticOutport();
                outputContainer.Insert(portViewNewIndex, portView);
            }
        }

        public bool TryRenamePort(string oldName, string newName)
        {
            // Ensure the port with the old name exists.
            Port port = node.GetPort(oldName);
            if (port == null)
            {
                Logger.Error($"The port with the old name '{oldName}' doesn't exist!");
                return false;
            }

            // Ensure the new name is not used.
            Port portWithNewName = node.GetPort(newName);
            if (portWithNewName != null)
            {
                Logger.Error($"The new name '{newName}' has been used!");
                return false;
            }

            bool success = node.TryRenamePort(oldName, newName);
            if (!success)
            {
                return false;
            }

            PortView portView = portDataToViewTable[port];
            portView.portName = newName;
            return true;
        }

        public void ChangePortType(string name, Type type)
        {
            Port oldPort = node.GetPort(name);
            RemovePortView(oldPort);

            node.ChangeDynamicPortType(name, type);

            Port newPort = node.GetPort(name);
            int index = node.GetIndexOfDynamicPort(newPort);

            if (newPort is Inport)
            {
                index += node.GetCountOfStaticInport();
            }
            else if (newPort is Outport)
            {
                index += node.GetCountOfStaticOutport();
            }

            AddPortView(newPort, index);
        }
    }
}
