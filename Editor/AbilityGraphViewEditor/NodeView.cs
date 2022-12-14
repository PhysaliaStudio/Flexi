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
        private static readonly Color ENTRY_COLOR = new(140f / 255f, 31f / 255f, 36f / 255f, 205f / 255f);
        private static readonly Color PROCESS_COLOR = new(73f / 255f, 114f / 255f, 140f / 255f, 205f / 255f);
        private static readonly Color CONSTANT_COLOR = new(104f / 255f, 54f / 255f, 175f / 255f, 205f / 255f);
        private static readonly Color MISSING_NODE_COLOR = new(1f, 0f, 0f, 205f / 255f);
        private static readonly Color MISSING_PORT_COLOR = new(1f, 0f, 0f, 240f / 255f);  // Alpha 240 is the default port alpha from the source code

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
            Type nodeType = node.GetType();
            if (node is EntryNode)
            {
                titleContainer.style.backgroundColor = ENTRY_COLOR;
            }
            else if (node is ProcessNode)
            {
                titleContainer.style.backgroundColor = PROCESS_COLOR;
            }

            if (node is MissingNode missingNode)
            {
                title = missingNode.TypeName;
                titleContainer.style.backgroundColor = MISSING_NODE_COLOR;
            }
            else if (nodeType == typeof(TrueNode))
            {
                title = "TRUE";
                titleContainer.style.backgroundColor = CONSTANT_COLOR;
                HandleConstantNodeStyle();
            }
            else if (nodeType == typeof(FalseNode))
            {
                title = "FALSE";
                titleContainer.style.backgroundColor = CONSTANT_COLOR;
                HandleConstantNodeStyle();
            }
            else if (nodeType == typeof(EqualNode))
            {
                title = "==";
                HandleConstantNodeStyle();
            }
            else if (nodeType == typeof(NotEqualNode))
            {
                title = "!=";
                HandleConstantNodeStyle();
            }
            else if (nodeType == typeof(LessNode))
            {
                title = "<";
                HandleConstantNodeStyle();
            }
            else if (nodeType == typeof(GreaterNode))
            {
                title = ">";
                HandleConstantNodeStyle();
            }
            else if (nodeType == typeof(LessOrEqualNode))
            {
                title = "<=";
                HandleConstantNodeStyle();
            }
            else if (nodeType == typeof(GreaterOrEqualNode))
            {
                title = ">=";
                HandleConstantNodeStyle();
            }
            else if (nodeType == typeof(AndNode))
            {
                title = "AND";
                HandleConstantNodeStyle();
            }
            else if (nodeType == typeof(OrNode))
            {
                title = "OR";
                HandleConstantNodeStyle();
            }
            else if (nodeType == typeof(XorNode))
            {
                title = "XOR";
                HandleConstantNodeStyle();
            }
            else if (nodeType == typeof(NotNode))
            {
                title = "NOT";
                HandleConstantNodeStyle();
            }
            else
            {
                title = nodeType.Name;
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
