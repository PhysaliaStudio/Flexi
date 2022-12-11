using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Physalia.AbilityFramework.GraphViewEditor
{
    public class NodeInspector : VisualElement
    {
        private class PortData
        {
            public string name = "";
            public Type type = null;
        }

        private readonly AbilityGraphEditorWindow window;
        private readonly VisualTreeAsset uiAsset;
        private readonly VisualTreeAsset listViewItemAsset;

        private readonly List<PortData> portDatas = new();
        private readonly HashSet<VisualElement> bindingItems = new();
        private readonly Dictionary<VisualElement, IDisposable> callbackTable = new();

        private ListView listView;
        private NodeView currentNodeView;

        public NodeInspector(AbilityGraphEditorWindow window, VisualTreeAsset uiAsset, VisualTreeAsset listViewItemAsset) : base()
        {
            this.window = window;
            this.uiAsset = uiAsset;
            this.listViewItemAsset = listViewItemAsset;
            CreateGUI();
        }

        private void CreateGUI()
        {
            uiAsset.CloneTree(this);

            listView = this.Query<ListView>().First();
            listView.itemsSource = portDatas;

            listView.itemsAdded += OnItemsAdded;
            listView.itemsRemoved += OnItemsRemoved;
            listView.itemIndexChanged += OnItemIndexChanged;
            listView.makeItem += MakeItem;
            listView.bindItem += BindItem;
            listView.unbindItem += UnbindItem;
        }

        private void OnItemsAdded(IEnumerable<int> indexes)
        {
            window.SetDirty(true);
            foreach (int i in indexes)
            {
                // The new item has empty name and null type, so we don't create the port now
                portDatas[i] = new PortData();
            }
        }

        private void OnItemsRemoved(IEnumerable<int> indexes)
        {
            window.SetDirty(true);
            foreach (int i in indexes)
            {
                currentNodeView.RemovePort(Direction.Output, i);
            }
        }

        private void OnItemIndexChanged(int index1, int index2)
        {
            window.SetDirty(true);
            currentNodeView.ChangePortIndex(Direction.Output, index1, index2);
        }

        private VisualElement MakeItem()
        {
            return listViewItemAsset.CloneTree();
        }

        // Unity use pool to manage variable items, and all items respawn when the list changes.
        // So there are many unseen bind and unbind method calls when the list changes.
        private void BindItem(VisualElement element, int i)
        {
            PortData portData = portDatas[i];

            if (bindingItems.Contains(element))
            {
                UnbindItem(element, -1);
            }

            bindingItems.Add(element);

            {
                var nameField = element.Q<TextField>("name");
                nameField.SetValueWithoutNotify(portData.name);
                var token = new ElementCallbackToken<string>
                {
                    element = nameField,
                    callback = evt =>
                    {
                        if (string.IsNullOrEmpty(evt.newValue))
                        {
                            nameField.SetValueWithoutNotify(evt.previousValue);
                            return;
                        }

                        Port portWithNewName = currentNodeView.GetPort(evt.newValue);
                        if (portWithNewName != null)
                        {
                            nameField.SetValueWithoutNotify(evt.previousValue);
                            return;
                        }

                        Port port = currentNodeView.GetPort(evt.previousValue);
                        if (port == null)
                        {
                            portData.name = evt.newValue;
                            if (!string.IsNullOrEmpty(evt.newValue) && portData.type != null)
                            {
                                window.SetDirty(true);
                                currentNodeView.AddPort(portData.name, Direction.Output, portData.type);
                            }
                        }
                        else
                        {
                            bool success = currentNodeView.TryRenamePort(evt.previousValue, evt.newValue);
                            if (success)
                            {
                                window.SetDirty(true);
                                portData.name = evt.newValue;
                            }
                            else
                            {
                                nameField.SetValueWithoutNotify(evt.previousValue);
                            }
                        }
                    },
                };
                callbackTable.Add(nameField, token);
                nameField.RegisterValueChangedCallback(token.callback);
            }

            {
                var typeField = element.Q<TextField>("type");
                typeField.SetValueWithoutNotify(portData.type != null ? portData.type.FullName : "");
                var token = new ElementCallbackToken<string>
                {
                    element = typeField,
                    callback = evt =>
                    {
                        Type type = ReflectionUtilities.GetTypeByName(evt.newValue);
                        if (type == null)
                        {
                            typeField.SetValueWithoutNotify(evt.previousValue);
                            return;
                        }

                        portData.type = type;

                        Port port = currentNodeView.GetPort(portData.name);
                        if (port == null)
                        {
                            if (!string.IsNullOrEmpty(portData.name) && portData.type != null)
                            {
                                window.SetDirty(true);
                                currentNodeView.AddPort(portData.name, Direction.Output, portData.type);
                            }
                        }
                        else
                        {
                            window.SetDirty(true);
                            currentNodeView.ChangePortType(portData.name, type);
                        }
                    },
                };
                callbackTable.Add(typeField, token);
                typeField.RegisterValueChangedCallback(token.callback);
            }
        }

        private void UnbindItem(VisualElement element, int i)
        {
            bindingItems.Remove(element);

            var nameField = element.Q<TextField>("name");
            var keyToken = callbackTable[nameField] as ElementCallbackToken<string>;
            nameField.UnregisterValueChangedCallback(keyToken.callback);
            callbackTable[nameField].Dispose();
            callbackTable.Remove(nameField);

            var typeField = element.Q<TextField>("type");
            var valueToken = callbackTable[typeField] as ElementCallbackToken<string>;
            typeField.UnregisterValueChangedCallback(valueToken.callback);
            callbackTable[typeField].Dispose();
            callbackTable.Remove(typeField);
        }

        public void SetNodeView(NodeView nodeView)
        {
            currentNodeView = nodeView;
            if (currentNodeView == null)
            {
                listView.itemsSource = null;
                listView.Rebuild();
                portDatas.Clear();
                visible = false;
            }
            else
            {
                listView.itemsSource = null;
                portDatas.Clear();

                IReadOnlyList<Outport> dynamicOutports = nodeView.Node.DynamicOutports;
                for (var i = 0; i < dynamicOutports.Count; i++)
                {
                    var portData = new PortData
                    {
                        name = dynamicOutports[i].Name,
                        type = dynamicOutports[i].ValueType
                    };
                    portDatas.Add(portData);
                }

                listView.itemsSource = portDatas;
                listView.Rebuild();
                visible = true;
            }
        }
    }
}
