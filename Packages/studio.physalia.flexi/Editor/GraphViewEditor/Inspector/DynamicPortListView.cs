using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphViewEditor
{
    public class DynamicPortListView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DynamicPortListView, UxmlTraits> { }

        private class PortData
        {
            public string name = "";
            public Type type = null;
        }

        private VisualTreeAsset listViewItemAsset;

        private AbilityGraphEditorWindow window;
        private Direction direction;

        private ListView listView;
        private NodeView currentNodeView;
        private readonly List<PortData> portDatas = new();

        private readonly HashSet<VisualElement> bindingItems = new();
        private readonly Dictionary<VisualElement, IDisposable> callbackTable = new();

        public DynamicPortListView()
        {
            CreateGUI();
        }

        public void SetUp(AbilityGraphEditorWindow window, Direction direction, VisualTreeAsset listViewItemAsset)
        {
            this.window = window;
            this.direction = direction;
            this.listViewItemAsset = listViewItemAsset;
        }

        private void CreateGUI()
        {
            listView = new ListView
            {
                showAddRemoveFooter = true,
                reorderable = true,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
            };

            listView.itemsAdded += OnItemsAdded;
            listView.itemsRemoved += OnItemsRemoved;
            listView.itemIndexChanged += OnItemIndexChanged;
            listView.makeItem += MakeItem;
            listView.bindItem += BindItem;
            listView.unbindItem += UnbindItem;

            Add(listView);
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
                currentNodeView.RemovePort(direction, i);
            }
        }

        private void OnItemIndexChanged(int index1, int index2)
        {
            window.SetDirty(true);
            currentNodeView.ChangePortIndex(direction, index1, index2);
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

                        Port portWithNewName = currentNodeView.GetPortData(evt.newValue);
                        if (portWithNewName != null)
                        {
                            nameField.SetValueWithoutNotify(evt.previousValue);
                            return;
                        }

                        Port port = currentNodeView.GetPortData(evt.previousValue);
                        if (port == null)
                        {
                            portData.name = evt.newValue;
                            if (!string.IsNullOrEmpty(evt.newValue) && portData.type != null)
                            {
                                window.SetDirty(true);
                                currentNodeView.AddPort(portData.name, direction, portData.type);
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

                        Port port = currentNodeView.GetPortData(portData.name);
                        if (port == null)
                        {
                            if (!string.IsNullOrEmpty(portData.name) && portData.type != null)
                            {
                                window.SetDirty(true);
                                currentNodeView.AddPort(portData.name, direction, portData.type);
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
            }
            else
            {
                listView.itemsSource = null;
                portDatas.Clear();

                var dynamicPorts = new List<Port>();
                if (direction == Direction.Input)
                {
                    IReadOnlyList<Inport> dynamicInports = nodeView.NodeData.DynamicInports;
                    dynamicPorts.AddRange(dynamicInports);
                }
                else if (direction == Direction.Output)
                {
                    IReadOnlyList<Outport> dynamicOutports = nodeView.NodeData.DynamicOutports;
                    dynamicPorts.AddRange(dynamicOutports);

                }

                for (var i = 0; i < dynamicPorts.Count; i++)
                {
                    var portData = new PortData
                    {
                        name = dynamicPorts[i].Name,
                        type = dynamicPorts[i].ValueType
                    };
                    portDatas.Add(portData);
                }

                listView.itemsSource = portDatas;
                listView.Rebuild();
            }
        }
    }
}
