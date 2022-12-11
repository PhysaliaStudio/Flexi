using System;
using System.Collections.Generic;
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

            ListView listView = this.Query<ListView>().First();
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
                portDatas[i] = new PortData();
            }
        }

        private void OnItemsRemoved(IEnumerable<int> indexes)
        {
            window.SetDirty(true);
        }

        private void OnItemIndexChanged(int index1, int index2)
        {
            window.SetDirty(true);
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
                        window.SetDirty(true);
                        portData.name = evt.newValue;
                        // TODO: Rename port
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
                        window.SetDirty(true);
                        Type type = ReflectionUtilities.GetTypeByName(evt.newValue);
                        if (type != null)
                        {
                            portData.type = type;
                            // TODO: Change Port Type
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
        }
    }
}
