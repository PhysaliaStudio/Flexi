using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Physalia.Flexi.GraphViewEditor
{
    public class BlackboardInspector : VisualElement
    {
        private readonly AbilityGraphEditorWindow window;
        private readonly VisualTreeAsset uiAsset;
        private readonly VisualTreeAsset listViewItemAsset;

        private ListView listView;
        private readonly List<BlackboardVariable> variables = new();

        private readonly HashSet<VisualElement> bindingItems = new();
        private readonly Dictionary<VisualElement, IDisposable> callbackTable = new();

        public BlackboardInspector(AbilityGraphEditorWindow window, VisualTreeAsset uiAsset, VisualTreeAsset listViewItemAsset) : base()
        {
            this.window = window;
            this.uiAsset = uiAsset;
            this.listViewItemAsset = listViewItemAsset;
            CreateGUI();
        }

        public List<BlackboardVariable> GetBlackboard()
        {
            return variables;
        }

        public void SetBlackboard(List<BlackboardVariable> variables)
        {
            this.variables.Clear();

            if (variables != null)
            {
                // Clone each variable to prevent modify the source
                for (var i = 0; i < variables.Count; i++)
                {
                    this.variables.Add(variables[i].Clone());
                }

                listView.Rebuild();
                listView.visible = true;
            }
            else
            {
                listView.Rebuild();
                listView.visible = false;
            }
        }

        private void CreateGUI()
        {
            uiAsset.CloneTree(this);

            listView = this.Query<ListView>();
            listView.itemsAdded += OnItemsAdded;
            listView.itemsRemoved += OnItemsRemoved;
            listView.itemIndexChanged += OnItemIndexChanged;
            listView.makeItem += MakeItem;
            listView.bindItem += BindItem;
            listView.unbindItem += UnbindItem;

            listView.itemsSource = variables;
        }

        private void OnItemsAdded(IEnumerable<int> indexes)
        {
            window.SetDirty(true);
            foreach (int i in indexes)
            {
                variables[i] = new BlackboardVariable();
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
            BlackboardVariable variable = variables[i];

            if (bindingItems.Contains(element))
            {
                UnbindItem(element, -1);
            }

            bindingItems.Add(element);

            {
                var keyField = element.Q<TextField>("key");
                keyField.SetValueWithoutNotify(variable.key);
                var token = new ElementCallbackToken<string>
                {
                    element = keyField,
                    callback = evt =>
                    {
                        window.SetDirty(true);
                        variable.key = evt.newValue;
                    },
                };
                callbackTable.Add(keyField, token);
                keyField.RegisterValueChangedCallback(token.callback);
            }

            {
                var valueField = element.Q<IntegerField>("value");
                valueField.SetValueWithoutNotify(variable.value);
                var token = new ElementCallbackToken<int>
                {
                    element = valueField,
                    callback = evt =>
                    {
                        window.SetDirty(true);
                        variable.value = evt.newValue;
                    },
                };
                callbackTable.Add(valueField, token);
                valueField.RegisterValueChangedCallback(token.callback);
            }
        }

        private void UnbindItem(VisualElement element, int i)
        {
            bindingItems.Remove(element);

            var keyField = element.Q<TextField>("key");
            var keyToken = callbackTable[keyField] as ElementCallbackToken<string>;
            keyField.UnregisterValueChangedCallback(keyToken.callback);
            callbackTable[keyField].Dispose();
            callbackTable.Remove(keyField);

            var valueField = element.Q<IntegerField>("value");
            var valueToken = callbackTable[valueField] as ElementCallbackToken<int>;
            valueField.UnregisterValueChangedCallback(valueToken.callback);
            callbackTable[valueField].Dispose();
            callbackTable.Remove(valueField);
        }
    }
}
