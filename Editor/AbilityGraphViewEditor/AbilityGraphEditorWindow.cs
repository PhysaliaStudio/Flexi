using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using EdgeView = UnityEditor.Experimental.GraphView.Edge;

namespace Physalia.AbilityFramework.GraphViewEditor
{
    public class AbilityGraphEditorWindow : EditorWindow
    {
        // Note: For handling missing clear callback method
        // https://forum.unity.com/threads/clearing-previous-registervaluechangedcallbacks-or-passing-custom-arguments-to-the-callback.1042819/#post-6765157
        private class ElementCallbackToken<T> : IDisposable
        {
            public INotifyValueChanged<T> element;
            public EventCallback<ChangeEvent<T>> callback;

            public void Dispose() => element.UnregisterValueChangedCallback(callback);
        }

        private static readonly string WINDOW_TITLE = "Ability Graph Editor";
        private static readonly string DEFAULT_FOLDER_PATH = "Assets/";

        // Since we need to use names to find the correct VisualTreeAsset to replace,
        // these names should be corresponding to the contents of the UXML file.
        private static readonly string FILE_FIELD_NAME = "file-field";
        private static readonly string NEW_BUTTON_NAME = "new-button";
        private static readonly string SAVE_BUTTON_NAME = "save-button";
        private static readonly string RELOAD_BUTTON_NAME = "reload-button";
        private static readonly string GRAPH_VIEW_PARENT_NAME = "graph-view-parent";
        private static readonly string GRAPH_VIEW_NAME = "graph-view";

        [SerializeField]
        private VisualTreeAsset uiAsset = null;
        [SerializeField]
        private VisualTreeAsset blackboardItemAsset = null;
        [HideInInspector]
        [SerializeField]
        private AbilityGraphAsset currentAsset = null;

        private ObjectField objectField;
        private AbilityGraphView graphView;
        private Blackboard blackboard;
        private bool isDirty;

        private readonly HashSet<VisualElement> usingVariableItems = new();
        private readonly Dictionary<VisualElement, IDisposable> callbackTable = new();

        [MenuItem("Tools/Physalia/Ability Graph Editor (GraphView) &1")]
        private static void Open()
        {
            AbilityGraphEditorWindow window = GetWindow<AbilityGraphEditorWindow>(WINDOW_TITLE);
            window.Show();
        }

        private void OnEnable()
        {
            if (uiAsset == null)
            {
                Logger.Error($"[{nameof(AbilityGraphEditorWindow)}] Missing UIAsset, set the corrent reference in {nameof(AbilityGraphEditorWindow)} ScriptAsset might fix this");
                return;
            }

            uiAsset.CloneTree(rootVisualElement);

            objectField = rootVisualElement.Query<ObjectField>(FILE_FIELD_NAME).First();
            objectField.objectType = typeof(AbilityGraphAsset);
            objectField.RegisterValueChangedCallback(evt =>
            {
                if (evt.previousValue == evt.newValue)
                {
                    return;
                }

                if (evt.newValue == null)
                {
                    objectField.SetValueWithoutNotify(evt.previousValue);
                    return;
                }

                bool ok = AskForSaveIfDirty();
                if (ok)
                {
                    var asset = evt.newValue as AbilityGraphAsset;
                    bool success = LoadFile(asset);
                    if (!success)
                    {
                        objectField.SetValueWithoutNotify(evt.previousValue);
                    }
                    else
                    {
                        currentAsset = asset;
                    }
                }
                else
                {
                    objectField.SetValueWithoutNotify(evt.previousValue);
                }
            });

            Button newButton = rootVisualElement.Query<Button>(NEW_BUTTON_NAME).First();
            newButton.clicked += OnNewButtonClicked;

            Button saveButton = rootVisualElement.Query<Button>(SAVE_BUTTON_NAME).First();
            saveButton.clicked += OnSaveButtonClicked;

            Button reloadButton = rootVisualElement.Query<Button>(RELOAD_BUTTON_NAME).First();
            reloadButton.clicked += ReloadFile;

            if (currentAsset == null)
            {
                NewGraphView();
            }
            else
            {
                objectField.SetValueWithoutNotify(currentAsset);
                LoadFile(currentAsset);
            }
        }

        private bool LoadFile(AbilityGraphAsset asset)
        {
            AbilityGraph abilityGraph = AbilityGraphUtility.Deserialize(asset.name, asset.Text);
            if (abilityGraph == null)
            {
                return false;
            }

            AbilityGraphView graphView = AbilityGraphView.Create(abilityGraph, this);
            SetUpGraphView(graphView);
            SetDirty(false);
            return true;
        }

        private void ReloadFile()
        {
            if (currentAsset == null)
            {
                return;
            }

            bool ok = AskForSaveIfDirty();
            if (ok)
            {
                LoadFile(currentAsset);
            }
        }

        private bool SaveFile()
        {
            if (currentAsset == null)
            {
                string assetPath = EditorUtility.SaveFilePanelInProject("Save ability", "NewGraph", "asset",
                    "Please enter a file name to save to", DEFAULT_FOLDER_PATH);
                if (assetPath.Length == 0)
                {
                    return false;
                }

                AbilityGraphAsset newAsset = CreateInstance<AbilityGraphAsset>();
                AssetDatabase.CreateAsset(newAsset, assetPath);
                currentAsset = AssetDatabase.LoadAssetAtPath<AbilityGraphAsset>(assetPath);
                objectField.SetValueWithoutNotify(currentAsset);
            }

            SetDirty(false);
            AbilityGraph abilityGraph = graphView.GetAbilityGraph();
            currentAsset.Text = AbilityGraphUtility.Serialize(abilityGraph);
            EditorUtility.SetDirty(currentAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return true;
        }

        private void OnNewButtonClicked()
        {
            bool ok = AskForSaveIfDirty();
            if (ok)
            {
                NewGraphView();
            }
        }

        private void OnSaveButtonClicked()
        {
            SaveFile();
        }

        private bool AskForSaveIfDirty()
        {
            if (!isDirty)
            {
                return true;
            }

            int option = EditorUtility.DisplayDialogComplex("Unsaved Changes",
                "Do you want to save the changes you made before quitting?",
                "Save",
                "Cancel",
                "Don't Save");

            switch (option)
            {
                default:
                    Logger.Error("Unrecognized option");
                    return false;
                // Save
                case 0:
                    return SaveFile();
                // Cancel
                case 1:
                    return false;
                // Don't Save
                case 2:
                    return true;
            }
        }

        private void NewGraphView()
        {
            SetUpGraphView(new AbilityGraphView(this));
            SetDirty(false);
            objectField.SetValueWithoutNotify(null);
            currentAsset = null;
        }

        private void SetUpGraphView(AbilityGraphView graphView)
        {
            VisualElement graphViewParent = rootVisualElement.Query<VisualElement>(GRAPH_VIEW_PARENT_NAME).First();
            graphViewParent.Clear();
            callbackTable.Clear();

            this.graphView = graphView;
            graphView.name = GRAPH_VIEW_NAME;
            graphView.graphViewChanged += OnGraphViewChanged;
            graphView.serializeGraphElements += SerializeGraphElements;
            graphView.canPasteSerializedData += CanPasteSerializedData;
            graphView.unserializeAndPaste += UnserializeAndPaste;
            graphViewParent.Add(graphView);

            Blackboard blackboard = CreateBlackboard(graphView);
            graphViewParent.Add(blackboard);
        }

        private string SerializeGraphElements(IEnumerable<GraphElement> elements)
        {
            var partialGraph = new PartialGraph();

            // Collect nodes
            foreach (GraphElement element in elements)
            {
                if (element is NodeView nodeView)
                {
                    Node node = nodeView.Node;
                    partialGraph.nodes.Add(node);
                }
            }

            // Collect edges
            foreach (GraphElement element in elements)
            {
                if (element is EdgeView edgeView)
                {
                    var outputNodeView = edgeView.output.node as NodeView;
                    var inputNodeView = edgeView.input.node as NodeView;

                    // If there's any node not in this selection, remove this edge.
                    if (partialGraph.nodes.Contains(outputNodeView.Node) && partialGraph.nodes.Contains(inputNodeView.Node))
                    {
                        Port outport = outputNodeView.GetPort(edgeView.output);
                        Port inport = inputNodeView.GetPort(edgeView.input);
                        Edge edge = new Edge
                        {
                            id1 = outputNodeView.Node.id,
                            id2 = inputNodeView.Node.id,
                            port1 = outport.Name,
                            port2 = inport.Name,
                        };

                        partialGraph.edges.Add(edge);
                    }
                }
            }

            string json = JsonConvert.SerializeObject(partialGraph);
            return json;
        }

        private void UnserializeAndPaste(string operationName, string data)
        {
            Vector2 localMousePosition = graphView.LastContextPosition;

            PartialGraph partialGraph;
            try
            {
                partialGraph = JsonConvert.DeserializeObject<PartialGraph>(data);
            }
            catch (Exception e)
            {
                Logger.Fatal(e);
                return;
            }

            SetDirty(true);

            // Calculate the most top-left point
            var topLeft = new Vector2(float.MaxValue, float.MaxValue);
            for (var i = 0; i < partialGraph.nodes.Count; i++)
            {
                Node node = partialGraph.nodes[i];
                if (node.position.x < topLeft.x)
                {
                    topLeft.x = node.position.x;
                }

                if (node.position.y < topLeft.y)
                {
                    topLeft.y = node.position.y;
                }
            }

            // Offset the nodes to match the menu position
            for (var i = 0; i < partialGraph.nodes.Count; i++)
            {
                Node node = partialGraph.nodes[i];
                node.id = -node.id;
                node.position += localMousePosition - topLeft;
                graphView.AddNode(node);
            }

            // Connect the edges
            for (var i = 0; i < partialGraph.edges.Count; i++)
            {
                Edge edge = partialGraph.edges[i];
                edge.id1 = -edge.id1;
                edge.id2 = -edge.id2;
                graphView.AddEdge(edge);
            }

            graphView.ValidateNodeIds();
        }

        private bool CanPasteSerializedData(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return false;
            }

            var canParse = true;
            try
            {
                _ = JsonConvert.DeserializeObject<PartialGraph>(data);
            }
            catch
            {
                canParse = false;
            }

            return canParse;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                SetDirty(true);
                foreach (GraphElement element in graphViewChange.elementsToRemove)
                {
                    if (element is NodeView nodeView)
                    {
                        graphView.RemoveNode(nodeView.Node);
                    }
                    else if (element is EdgeView edgeView)
                    {
                        var outputNodeView = edgeView.output.node as NodeView;
                        var inputNodeView = edgeView.input.node as NodeView;
                        Port outport = outputNodeView.GetPort(edgeView.output);
                        Port inport = inputNodeView.GetPort(edgeView.input);
                        outport.Disconnect(inport);

                        if (outport is MissingOutport && outport.GetConnections().Count == 0)
                        {
                            outputNodeView.DestroyPort(outport);
                        }

                        if (inport is MissingInport && inport.GetConnections().Count == 0)
                        {
                            inputNodeView.DestroyPort(inport);
                        }
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                SetDirty(true);
                foreach (EdgeView edgeView in graphViewChange.edgesToCreate)
                {
                    var outputNodeView = edgeView.output.node as NodeView;
                    var inputNodeView = edgeView.input.node as NodeView;
                    Port outport = outputNodeView.GetPort(edgeView.output);
                    Port inport = inputNodeView.GetPort(edgeView.input);
                    outport.Connect(inport);
                }
            }

            if (graphViewChange.movedElements != null)
            {
                SetDirty(true);
                foreach (GraphElement element in graphViewChange.movedElements)
                {
                    if (element is NodeView nodeView)
                    {
                        nodeView.Node.position = element.GetPosition().position;
                    }
                }
            }

            return graphViewChange;
        }

        private Blackboard CreateBlackboard(AbilityGraphView graphView)
        {
            blackboard = new Blackboard(graphView)
            {
                scrollable = true,
                windowed = true,
            };
            blackboard.style.width = 200f;
            blackboard.style.height = Length.Percent(80f);
            blackboard.style.position = Position.Absolute;
            blackboard.style.alignSelf = Align.FlexEnd;

            var listView = new ListView(graphView.GetAbilityGraph().BlackboardVariables)
            {
                reorderable = true,
                showAddRemoveFooter = true,
            };
            listView.itemsAdded += OnBlackboardItemAdded;
            listView.itemsRemoved += OnBlackboardItemRemoved;
            listView.itemIndexChanged += OnBlackboardItemIndexChanged;
            listView.makeItem += blackboardItemAsset.CloneTree;
            listView.bindItem += BindVariableItem;
            listView.unbindItem += UnbindVariableItem;
            blackboard.contentContainer.Add(listView);

            return blackboard;
        }

        private void OnBlackboardItemAdded(IEnumerable<int> indexes)
        {
            SetDirty(true);
            foreach (int i in indexes)
            {
                graphView.GetAbilityGraph().BlackboardVariables[i] = new BlackboardVariable();
            }
        }

        private void OnBlackboardItemRemoved(IEnumerable<int> indexes)
        {
            SetDirty(true);
        }

        private void OnBlackboardItemIndexChanged(int index1, int index2)
        {
            SetDirty(true);
        }

        // Unity use pool to manage variable items, and all items respawn when the list changes.
        // So there are many unseen bind and unbind method calls when the list changes.
        private void BindVariableItem(VisualElement element, int i)
        {
            BlackboardVariable variable = graphView.GetAbilityGraph().BlackboardVariables[i];

            if (usingVariableItems.Contains(element))
            {
                UnbindVariableItem(element, -1);
            }

            usingVariableItems.Add(element);

            {
                var keyField = element.Q<TextField>("key");
                keyField.SetValueWithoutNotify(variable.key);
                var token = new ElementCallbackToken<string>
                {
                    element = keyField,
                    callback = evt =>
                    {
                        SetDirty(true);
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
                        SetDirty(true);
                        variable.value = evt.newValue;
                    },
                };
                callbackTable.Add(valueField, token);
                valueField.RegisterValueChangedCallback(token.callback);
            }
        }

        private void UnbindVariableItem(VisualElement element, int i)
        {
            usingVariableItems.Remove(element);

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

        public void SetDirty(bool value)
        {
            if (isDirty != value)
            {
                isDirty = value;
                if (isDirty)
                {
                    titleContent = new GUIContent("*" + WINDOW_TITLE);
                }
                else
                {
                    titleContent = new GUIContent(WINDOW_TITLE);
                }
            }
        }
    }
}
