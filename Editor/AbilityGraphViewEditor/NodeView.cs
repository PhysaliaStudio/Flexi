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

        private readonly Node node;
        private readonly Dictionary<Port, PortView> portDataToViewTable = new();
        private readonly Dictionary<PortView, Port> portViewToDataTable = new();

        public Node Node => node;

        public NodeView(Node node) : base()
        {
            this.node = node;
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

            if (nodeType == typeof(TrueNode))
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
            FieldInfo[] fields = node.GetType().GetFieldsIncludeBasePrivate();
            for (var i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                if (field.FieldType.IsSubclassOf(typeof(Inport)))
                {
                    PortView port = InstantiatePort(Orientation.Horizontal, Direction.Input, PortView.Capacity.Multi, field.FieldType.GetGenericArguments()[0]);
                    port.portName = GetPortName(field.Name);
                    inputContainer.Add(port);

                    var portData = field.GetValue(node) as Port;
                    portDataToViewTable.Add(portData, port);
                    portViewToDataTable.Add(port, portData);
                }

                if (field.FieldType.IsSubclassOf(typeof(Outport)))
                {
                    PortView port = InstantiatePort(Orientation.Horizontal, Direction.Output, PortView.Capacity.Multi, field.FieldType.GetGenericArguments()[0]);
                    port.portName = GetPortName(field.Name);
                    outputContainer.Add(port);

                    var portData = field.GetValue(node) as Port;
                    portDataToViewTable.Add(portData, port);
                    portViewToDataTable.Add(port, portData);
                }

                if (field.FieldType.IsSubclassOf(typeof(Variable)))
                {
                    Type genericType = field.FieldType.GetGenericArguments()[0];
                    CreateVariableField creationMethod = VariableFieldTypeCache.GetCreationMethod(genericType);
                    if (creationMethod == null)
                    {
                        continue;
                    }

                    var variable = field.GetValue(node) as Variable;
                    IVariableField variableField = creationMethod.Invoke(field.Name, variable);
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
    }
}
