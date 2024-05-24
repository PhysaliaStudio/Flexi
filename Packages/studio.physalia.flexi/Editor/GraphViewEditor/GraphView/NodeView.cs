using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
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

        private const string USS_CLASS_INPUT_FIELD = "port-input-element";
        private const string USS_CLASS_INPUT_FIELD_HIDDEN = "port-input-element__hidden";

        private static readonly Color MISSING_PORT_COLOR = new(1f, 0f, 0f);

        private VisualElement inputFieldContainer;
        private readonly List<PortView> inputPortViews = new();
        private readonly List<PortView> outputPortViews = new();
        private readonly Dictionary<PortView, VisualElement> inputPortToFieldTable = new();

        private readonly NodeData nodeData;
        private readonly AbilityGraphEditorWindow window;
        private readonly AbilityGraphView graphView;
        private readonly Dictionary<PortData, Port> portDataToViewTable = new();
        private readonly Dictionary<Port, PortData> portViewToDataTable = new();

        public NodeData NodeData => nodeData;
        public IReadOnlyList<PortView> InputPorts => inputPortViews;
        public IReadOnlyList<PortView> OutputPorts => outputPortViews;

        public NodeView(NodeData nodeData, AbilityGraphEditorWindow window, AbilityGraphView graphView)
            : base(EditorConst.PackagePath + "Editor/GraphViewEditor/UiAssets/Node.uxml")
        {
            UseDefaultStyling();

            this.nodeData = nodeData;
            this.window = window;
            this.graphView = graphView;
            HandleNodeStyles(nodeData);

            CreateInputFieldContainer();
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

        private void CreateInputFieldContainer()
        {
            inputFieldContainer = new VisualElement { name = "input-field-container" };
            inputFieldContainer.SendToBack();
            inputFieldContainer.pickingMode = PickingMode.Ignore;
            mainContainer.parent.Add(inputFieldContainer);
        }

        private void HandleNodeStyles(NodeData node)
        {
            mainContainer.style.overflow = Overflow.Visible;

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

            HandleCustomColorIfSet(node);
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

        private void HandleCustomColorIfSet(NodeData nodeData)
        {
            NodeColor nodeColor = nodeData.GetType().GetCustomAttribute<NodeColor>();
            if (nodeColor != null && nodeColor.IsValid)
            {
                titleContainer.style.backgroundColor = nodeColor.Color;
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
                AddPortView(portData);
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

        private PortView CreateInputPortView(Inport inportData, int index)
        {
            string portName = GetPortPrettyName(inportData.Name);
            Type portType = inportData.ValueType;
            var portView = new PortView(Orientation.Horizontal, Direction.Input, Capacity.Multi, portType) { portName = portName };

            VisualElement inputField = CreateInputField(inportData);
            inputPortToFieldTable.Add(portView, inputField);

            if (index == -1)
            {
                inputPortViews.Add(portView);
                inputContainer.Add(portView);
                inputFieldContainer.Add(inputField);
            }
            else
            {
                inputPortViews.Insert(index, portView);
                inputContainer.Insert(index, portView);
                inputFieldContainer.Insert(index, inputField);
            }

            return portView;
        }

        private PortView CreateOutputPortView(Outport outport, int index = -1)
        {
            string portName = GetPortPrettyName(outport.Name);
            Type portType = outport.ValueType;
            var portView = new PortView(Orientation.Horizontal, Direction.Output, Capacity.Multi, portType) { portName = portName };

            if (index == -1)
            {
                outputPortViews.Add(portView);
                outputContainer.Add(portView);
            }
            else
            {
                outputPortViews.Insert(index, portView);
                outputContainer.Insert(index, portView);
            }

            return portView;
        }

        private VisualElement CreateInputField(Inport inportData)
        {
            // TODO: Disable input fields for subgraph nodes for now, since I didn't find the solution.
            var box = new VisualElement();
            if (inportData.Node is SubgraphNode)
            {
                box.AddToClassList(USS_CLASS_INPUT_FIELD_HIDDEN);
                return box;
            }

            Type portType = inportData.ValueType;

            BindableElement field = null;
            if (inportData.ValueType.IsEnum)
            {
                var enumField = new EnumField(inportData.DefaultValue as Enum);
                enumField.RegisterValueChangedCallback(evt =>
                {
                    inportData.DefaultValue = evt.newValue;
                    window.SetDirty(true);
                });
                field = enumField;
            }
            else if (inportData is Inport<bool> inportBool)
            {
                var toggle = new Toggle();
                toggle.SetValueWithoutNotify(inportBool.DefaultValue);
                toggle.RegisterValueChangedCallback(evt =>
                {
                    inportData.DefaultValue = evt.newValue;
                    window.SetDirty(true);
                });
                field = toggle;
            }
            else if (inportData is Inport<int> inportInt)
            {
                var integerField = new IntegerField();
                integerField.SetValueWithoutNotify(inportInt.DefaultValue);
                integerField.RegisterValueChangedCallback(evt =>
                {
                    inportData.DefaultValue = evt.newValue;
                    window.SetDirty(true);
                });
                field = integerField;
            }
            else if (inportData is Inport<long> inportLong)
            {
                var longField = new LongField();
                longField.SetValueWithoutNotify(inportLong.DefaultValue);
                longField.RegisterValueChangedCallback(evt =>
                {
                    inportData.DefaultValue = evt.newValue;
                    window.SetDirty(true);
                });
                field = longField;
            }
            else if (inportData is Inport<float> inportFloat)
            {
                var floatField = new FloatField();
                floatField.SetValueWithoutNotify(inportFloat.DefaultValue);
                floatField.RegisterValueChangedCallback(evt =>
                {
                    inportData.DefaultValue = evt.newValue;
                    window.SetDirty(true);
                });
                field = floatField;
            }
            else if (inportData is Inport<double> inportDouble)
            {
                var doubleField = new DoubleField();
                doubleField.SetValueWithoutNotify(inportDouble.DefaultValue);
                doubleField.RegisterValueChangedCallback(evt =>
                {
                    inportData.DefaultValue = evt.newValue;
                    window.SetDirty(true);
                });
                field = doubleField;
            }
            else if (inportData is Inport<Vector2> inportVector2)
            {
                var vector2Field = new Vector2Field();
                vector2Field.SetValueWithoutNotify(inportVector2.DefaultValue);
                vector2Field.RegisterValueChangedCallback(evt =>
                {
                    inportData.DefaultValue = evt.newValue;
                    window.SetDirty(true);
                });
                field = vector2Field;
            }
            else if (inportData is Inport<Vector3> inportVector3)
            {
                var vector3Field = new Vector3Field();
                vector3Field.SetValueWithoutNotify(inportVector3.DefaultValue);
                vector3Field.RegisterValueChangedCallback(evt =>
                {
                    inportData.DefaultValue = evt.newValue;
                    window.SetDirty(true);
                });
                field = vector3Field;
            }
            else if (inportData is Inport<Vector4> inportVector4)
            {
                var vector4Field = new Vector4Field();
                vector4Field.SetValueWithoutNotify(inportVector4.DefaultValue);
                vector4Field.RegisterValueChangedCallback(evt =>
                {
                    inportData.DefaultValue = evt.newValue;
                    window.SetDirty(true);
                });
                field = vector4Field;
            }
            else if (inportData is Inport<Vector2Int> inportVector2Int)
            {
                var vector2IntField = new Vector2IntField();
                vector2IntField.SetValueWithoutNotify(inportVector2Int.DefaultValue);
                vector2IntField.RegisterValueChangedCallback(evt =>
                {
                    inportData.DefaultValue = evt.newValue;
                    window.SetDirty(true);
                });
                field = vector2IntField;
            }
            else if (inportData is Inport<Vector3Int> inportVector3Int)
            {
                var vector3IntField = new Vector3IntField();
                vector3IntField.SetValueWithoutNotify(inportVector3Int.DefaultValue);
                vector3IntField.RegisterValueChangedCallback(evt =>
                {
                    inportData.DefaultValue = evt.newValue;
                    window.SetDirty(true);
                });
                field = vector3IntField;
            }
            else if (inportData is Inport<string> inportString)
            {
                var textField = new TextField();
                textField.SetValueWithoutNotify(inportString.DefaultValue);
                textField.RegisterValueChangedCallback(evt =>
                {
                    inportData.DefaultValue = evt.newValue;
                    window.SetDirty(true);
                });
                field = textField;
            }

            if (field != null)
            {
                box.AddToClassList(USS_CLASS_INPUT_FIELD);
                box.Add(field);
            }
            else
            {
                box.AddToClassList(USS_CLASS_INPUT_FIELD_HIDDEN);
            }

            return box;
        }

        private void AddPortView(PortData portData, int index = -1)
        {
            if (portData is Inport inportData)
            {
                Port port = CreateInputPortView(inportData, index);

                if (portData is MissingInport)
                {
                    port.portColor = MISSING_PORT_COLOR;
                }

                portDataToViewTable.Add(portData, port);
                portViewToDataTable.Add(port, portData);
            }
            else if (portData is Outport outportData)
            {
                Port port = CreateOutputPortView(outportData, index);

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

        public void OnInputPortConnected(PortView input)
        {
            HideInputField(input);
        }

        public void OnInputPortDisconnected(PortView input)
        {
            ShowInputField(input);
        }

        private void ShowInputField(PortView input)
        {
            bool success = inputPortToFieldTable.TryGetValue(input, out VisualElement inputField);
            if (success)
            {
                inputField.RemoveFromClassList(USS_CLASS_INPUT_FIELD_HIDDEN);
                inputField.AddToClassList(USS_CLASS_INPUT_FIELD);
            }
        }

        private void HideInputField(PortView input)
        {
            bool success = inputPortToFieldTable.TryGetValue(input, out VisualElement inputField);
            if (success)
            {
                inputField.RemoveFromClassList(USS_CLASS_INPUT_FIELD);
                inputField.AddToClassList(USS_CLASS_INPUT_FIELD_HIDDEN);
            }
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
