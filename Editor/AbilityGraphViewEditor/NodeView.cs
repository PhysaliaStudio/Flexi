using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using PortView = UnityEditor.Experimental.GraphView.Port;

namespace Physalia.AbilitySystem.GraphViewEditor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        private readonly Node node;
        private readonly Dictionary<Port, PortView> portDataToViewTable = new();
        private readonly Dictionary<PortView, Port> portViewToDataTable = new();

        public Node Node => node;

        public NodeView(Node node) : base()
        {
            this.node = node;
            title = node.GetType().Name;
            CreatePorts();
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
                    port.portName = field.Name;
                    inputContainer.Add(port);

                    var portData = field.GetValue(node) as Port;
                    portDataToViewTable.Add(portData, port);
                    portViewToDataTable.Add(port, portData);
                }

                if (field.FieldType.IsSubclassOf(typeof(Outport)))
                {
                    PortView port = InstantiatePort(Orientation.Horizontal, Direction.Output, PortView.Capacity.Multi, field.FieldType.GetGenericArguments()[0]);
                    port.portName = field.Name;
                    outputContainer.Add(port);

                    var portData = field.GetValue(node) as Port;
                    portDataToViewTable.Add(portData, port);
                    portViewToDataTable.Add(port, portData);
                }

                if (field.FieldType.IsSubclassOf(typeof(Variable)))
                {
                    Type genericType = field.FieldType.GetGenericArguments()[0];
                    CreateVariableField creationMethod = VariableFieldTypeCache.GetCreationMethod(genericType);
                    var variable = field.GetValue(node) as Variable;
                    IVariableField variableField = creationMethod.Invoke(field.Name, variable);
                    extensionContainer.Add(variableField.VisualElement);
                }
            }

            // Unity rule: After adding custom elements to the extensionContainer, call this method in order for them to become visible.
            RefreshExpandedState();
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
