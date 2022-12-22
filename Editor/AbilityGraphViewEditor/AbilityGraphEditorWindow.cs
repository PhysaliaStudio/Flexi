using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using EdgeView = UnityEditor.Experimental.GraphView.Edge;

namespace Physalia.AbilityFramework.GraphViewEditor
{
    // Note: For handling missing clear callback method
    // https://forum.unity.com/threads/clearing-previous-registervaluechangedcallbacks-or-passing-custom-arguments-to-the-callback.1042819/#post-6765157
    internal class ElementCallbackToken<T> : IDisposable
    {
        internal INotifyValueChanged<T> element;
        internal EventCallback<ChangeEvent<T>> callback;

        public void Dispose() => element.UnregisterValueChangedCallback(callback);
    }

    public class AbilityGraphEditorWindow : EditorWindow
    {
        private static readonly string WINDOW_TITLE = "Ability Graph Editor";
        private static readonly string DEFAULT_FOLDER_PATH = "Assets/";

        // Since we need to use names to find the correct VisualTreeAsset to replace,
        // these names should be corresponding to the contents of the UXML file.
        private static readonly string FILE_FIELD_NAME = "file-field";
        private static readonly string NEW_BUTTON_NAME = "new-button";
        private static readonly string SAVE_BUTTON_NAME = "save-button";
        private static readonly string RELOAD_BUTTON_NAME = "reload-button";
        private static readonly string NEW_MACRO_BUTTON_NAME = "new-macro-button";

        private static readonly string NODE_INSPECTOR_PARENT_NAME = "node-inspector-parent";
        private static readonly string GRAPH_VIEW_PARENT_NAME = "graph-view-parent";
        private static readonly string GRAPH_VIEW_NAME = "graph-view";

        [SerializeField]
        private VisualTreeAsset uiAsset = null;
        [SerializeField]
        private StyleSheet uiStyleSheet;

        [Space]
        [SerializeField]
        private VisualTreeAsset nodeInspectorAsset;
        [SerializeField]
        private VisualTreeAsset portListViewItemAsset;
        [SerializeField]
        private VisualTreeAsset blackboardItemAsset = null;
        [HideInInspector]
        [SerializeField]
        private AbilityAsset currentAsset = null;

        private ObjectField objectField;

        private AbilityGraphView graphView;
        private NodeInspector nodeInspector;
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

        [OnOpenAsset()]
        private static bool OpenWithAsset(int instanceID, int line)
        {
            UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceID);
            AbilityAsset asset = obj as AbilityAsset;
            if (asset == null)
            {
                return false;
            }

            AbilityGraphEditorWindow window = GetWindow<AbilityGraphEditorWindow>(WINDOW_TITLE);
            window.Focus();

            bool ok = window.AskForOpenAssetIfDirty(asset);
            if (ok)
            {
                window.objectField.SetValueWithoutNotify(asset);
            }

            return ok;
        }

        private void CreateGUI()
        {
            if (uiAsset == null)
            {
                Logger.Error($"[{nameof(AbilityGraphEditorWindow)}] Missing UIAsset, set the corrent reference in {nameof(AbilityGraphEditorWindow)} ScriptAsset might fix this");
                return;
            }

            uiAsset.CloneTree(rootVisualElement);
            rootVisualElement.styleSheets.Add(uiStyleSheet);

            objectField = rootVisualElement.Query<ObjectField>(FILE_FIELD_NAME).First();
            objectField.objectType = typeof(AbilityAsset);
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

                var asset = evt.newValue as AbilityAsset;
                bool ok = AskForOpenAssetIfDirty(asset);
                if (!ok)
                {
                    objectField.SetValueWithoutNotify(evt.previousValue);
                }
            });

            Button newButton = rootVisualElement.Query<Button>(NEW_BUTTON_NAME).First();
            newButton.clicked += () => OnNewButtonClicked(false);

            Button saveButton = rootVisualElement.Query<Button>(SAVE_BUTTON_NAME).First();
            saveButton.clicked += OnSaveButtonClicked;

            Button reloadButton = rootVisualElement.Query<Button>(RELOAD_BUTTON_NAME).First();
            reloadButton.clicked += ReloadFile;

            Button newMacroButton = rootVisualElement.Query<Button>(NEW_MACRO_BUTTON_NAME).First();
            newMacroButton.clicked += () => OnNewButtonClicked(true);

            SetUpNodeInspector();

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

        private void SetUpNodeInspector()
        {
            VisualElement nodeInspectorParent = rootVisualElement.Query<VisualElement>(NODE_INSPECTOR_PARENT_NAME).First();
            nodeInspector = new NodeInspector(this, nodeInspectorAsset, portListViewItemAsset);
            nodeInspectorParent.Add(nodeInspector);
        }

        public void ShowNodeInspector(NodeView nodeView)
        {
            nodeInspector.SetNodeView(nodeView);
        }

        public void HideNodeInspector()
        {
            nodeInspector.SetNodeView(null);
        }

        private bool LoadFile(AbilityAsset asset)
        {
            HideNodeInspector();

            AbilityGraph abilityGraph = AbilityGraphUtility.Deserialize(asset.name, asset.GraphJsons[0], MacroLibraryCache.Get());
            if (asset is MacroGraphAsset && !abilityGraph.HasCorrectSubgraphElement())
            {
                abilityGraph.AddSubgraphInOutNodes();
                abilityGraph.GraphInputNode.position = new Vector2(0, 250);
                abilityGraph.GraphOutputNode.position = new Vector2(500, 250);
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
            // Is the graph has any missing element, cancel saving
            AbilityGraph abilityGraph = graphView.GetAbilityGraph();
            if (abilityGraph.HasMissingElement())
            {
                ShowNotification(new GUIContent("You must fix all the missing elements before saving!"));
                return false;
            }

            // Save as new asset if necessary
            if (currentAsset == null)
            {
                string assetPath = EditorUtility.SaveFilePanelInProject("Save ability", "NewGraph", "asset",
                    "Please enter a file name to save to", DEFAULT_FOLDER_PATH);
                if (assetPath.Length == 0)
                {
                    return false;
                }

                if (abilityGraph.HasSubgraphElement())
                {
                    MacroGraphAsset newAsset = CreateInstance<MacroGraphAsset>();
                    newAsset.Text = AbilityGraphUtility.Serialize(abilityGraph);
                    AssetDatabase.CreateAsset(newAsset, assetPath);
                }
                else
                {
                    AbilityAsset newAsset = CreateInstance<AbilityAsset>();
                    newAsset.AddGraphJson(AbilityGraphUtility.Serialize(abilityGraph));
                    AssetDatabase.CreateAsset(newAsset, assetPath);
                }

                currentAsset = AssetDatabase.LoadAssetAtPath<AbilityAsset>(assetPath);
                objectField.SetValueWithoutNotify(currentAsset);
            }
            else
            {
                currentAsset.GraphJsons[0] = AbilityGraphUtility.Serialize(abilityGraph);
                EditorUtility.SetDirty(currentAsset);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            SetDirty(false);

            return true;
        }

        private void OnNewButtonClicked(bool isMacro)
        {
            bool ok = AskForSaveIfDirty();
            if (ok)
            {
                if (isMacro)
                {
                    NewMacroGraphView();
                }
                else
                {
                    NewGraphView();
                }
            }
        }

        private void OnSaveButtonClicked()
        {
            SaveFile();
        }

        private bool AskForOpenAssetIfDirty(AbilityAsset asset)
        {
            bool ok = AskForSaveIfDirty();
            if (ok)
            {
                bool success = LoadFile(asset);
                if (success)
                {
                    currentAsset = asset;
                    return true;
                }

                return false;
            }

            return false;
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
            HideNodeInspector();

            SetUpGraphView(new AbilityGraphView(this));
            SetDirty(false);
            objectField.SetValueWithoutNotify(null);
            currentAsset = null;
        }

        private void NewMacroGraphView()
        {
            HideNodeInspector();

            AbilityGraph graph = new AbilityGraph();
            graph.AddSubgraphInOutNodes();
            graph.GraphInputNode.position = new Vector2(0, 250);
            graph.GraphOutputNode.position = new Vector2(500, 250);

            AbilityGraphView graphView = AbilityGraphView.Create(graph, this);
            SetUpGraphView(graphView);
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
                    if (node is IIsMissing)
                    {
                        continue;
                    }

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
                        if (outport is IIsMissing || inport is IIsMissing)
                        {
                            continue;
                        }

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

            if (partialGraph.nodes.Count == 0 && partialGraph.edges.Count == 0)
            {
                return null;
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
