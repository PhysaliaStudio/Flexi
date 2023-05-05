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

namespace Physalia.Flexi.GraphViewEditor
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
        private static readonly string WINDOW_TITLE = "Flexi Ability Editor";
        private static readonly string DEFAULT_FOLDER_PATH = "Assets/";

        // Since we need to use names to find the correct VisualTreeAsset to replace,
        // these names should be corresponding to the contents of the UXML file.
        private static readonly string FILE_FIELD_NAME = "file-field";
        private static readonly string NEW_BUTTON_NAME = "new-button";
        private static readonly string SAVE_BUTTON_NAME = "save-button";
        private static readonly string RELOAD_BUTTON_NAME = "reload-button";
        private static readonly string NEW_MACRO_BUTTON_NAME = "new-macro-button";

        private static readonly string NODE_INSPECTOR_PARENT_NAME = "node-inspector-parent";
        private static readonly string BLACKBOARD_INSPECTOR_PARENT_NAME = "blackboard-inspector-parent";
        private static readonly string GRAPH_VIEW_PARENT_NAME = "graph-view-parent";
        private static readonly string GRAPH_VIEW_NAME = "graph-view";

        [SerializeField]
        private VisualTreeAsset uiAsset;
        [SerializeField]
        private StyleSheet uiStyleSheet;

        [Space]
        [SerializeField]
        private VisualTreeAsset nodeInspectorAsset;
        [SerializeField]
        private VisualTreeAsset portListViewItemAsset;

        [Space]
        [SerializeField]
        private VisualTreeAsset blackboardInspectorAsset = null;
        [SerializeField]
        private VisualTreeAsset blackboardItemAsset = null;

        [HideInInspector]
        [SerializeField]
        private GraphAsset currentAsset = null;

        private ObjectField objectField;

        private AbilityGraphView graphView;
        private NodeInspector nodeInspector;
        private BlackboardInspector blackboardInspector;
        private bool isDirty;

        [MenuItem("Tools/Flexi/Ability Editor &1")]
        private static void Open()
        {
            AbilityGraphEditorWindow window = GetWindow<AbilityGraphEditorWindow>(WINDOW_TITLE);
            window.Show();
        }

        [OnOpenAsset()]
        private static bool OpenWithAsset(int instanceID, int line)
        {
            UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceID);
            GraphAsset asset = obj as GraphAsset;
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
            bool checkResult = CheckUiAssets();
            if (!checkResult)
            {
                Logger.Error($"[{nameof(AbilityGraphEditorWindow)}] Missing UIAsset! Set the correct UIAsset in {nameof(AbilityGraphEditorWindow)} ScriptAsset might fix this.");
                return;
            }

            uiAsset.CloneTree(rootVisualElement);
            rootVisualElement.styleSheets.Add(uiStyleSheet);

            objectField = rootVisualElement.Query<ObjectField>(FILE_FIELD_NAME).First();
            objectField.objectType = typeof(GraphAsset);
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

                var asset = evt.newValue as GraphAsset;
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
            SetUpBlackboardInspector();

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

        private bool CheckUiAssets()
        {
            string folderPath = "Packages/studio.physalia.flexi/Editor/GraphViewEditor/UiAssets/";

            if (uiAsset == null)
            {
                uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(folderPath + "AbilityGraphEditorWindow.uxml");
                if (uiAsset == null)
                {
                    return false;
                }
            }

            if (uiStyleSheet == null)
            {
                uiStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(folderPath + "AbilityGraphEditorWindow.uss");
                if (uiStyleSheet == null)
                {
                    return false;
                }
            }

            if (nodeInspectorAsset == null)
            {
                nodeInspectorAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(folderPath + "NodeInspector.uxml");
                if (nodeInspectorAsset == null)
                {
                    return false;
                }
            }

            if (portListViewItemAsset == null)
            {
                portListViewItemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(folderPath + "PortListViewItem.uxml");
                if (portListViewItemAsset == null)
                {
                    return false;
                }
            }

            if (blackboardInspectorAsset == null)
            {
                blackboardInspectorAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(folderPath + "BlackboardInspector.uxml");
                if (blackboardInspectorAsset == null)
                {
                    return false;
                }
            }

            if (blackboardItemAsset == null)
            {
                blackboardItemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(folderPath + "BlackboardItem.uxml");
                if (blackboardItemAsset == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void SetUpNodeInspector()
        {
            VisualElement nodeInspectorParent = rootVisualElement.Query<VisualElement>(NODE_INSPECTOR_PARENT_NAME).First();
            nodeInspector = new NodeInspector(this, nodeInspectorAsset, portListViewItemAsset);
            nodeInspectorParent.Add(nodeInspector);
        }

        private void SetUpBlackboardInspector()
        {
            VisualElement blackboardInspectorParent = rootVisualElement.Query<VisualElement>(BLACKBOARD_INSPECTOR_PARENT_NAME).First();
            blackboardInspector = new BlackboardInspector(this, blackboardInspectorAsset, blackboardItemAsset);
            blackboardInspectorParent.Add(blackboardInspector);
        }

        public void ShowNodeInspector(NodeView nodeView)
        {
            nodeInspector.SetNodeView(nodeView);
        }

        public void HideNodeInspector()
        {
            nodeInspector.SetNodeView(null);
        }

        private bool LoadFile(GraphAsset asset)
        {
            HideNodeInspector();

            AbilityGraph abilityGraph;
            switch (asset)
            {
                default:
                    blackboardInspector.SetBlackboard(null);
                    return false;
                case AbilityAsset abilityAsset:
                    blackboardInspector.SetBlackboard(abilityAsset.Blackboard);
                    string graphJson = abilityAsset.GraphJsons.Count > 0 ? abilityAsset.GraphJsons[0] : "";
                    abilityGraph = AbilityGraphUtility.Deserialize(abilityAsset.name, graphJson, MacroLibraryCache.Get());
                    break;
                case MacroAsset macroAsset:
                    blackboardInspector.SetBlackboard(null);
                    abilityGraph = AbilityGraphUtility.Deserialize(macroAsset.name, macroAsset.Text, MacroLibraryCache.Get());
                    if (!abilityGraph.HasCorrectSubgraphElement())
                    {
                        abilityGraph.AddSubgraphInOutNodes();
                        abilityGraph.GraphInputNode.position = new Vector2(0, 250);
                        abilityGraph.GraphOutputNode.position = new Vector2(500, 250);
                    }
                    break;
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
                    MacroAsset newAsset = CreateInstance<MacroAsset>();
                    newAsset.Text = AbilityGraphUtility.Serialize(abilityGraph);
                    AssetDatabase.CreateAsset(newAsset, assetPath);
                }
                else
                {
                    AbilityAsset newAsset = CreateInstance<AbilityAsset>();
                    newAsset.Blackboard = blackboardInspector.GetBlackboard();
                    newAsset.AddGraphJson(AbilityGraphUtility.Serialize(abilityGraph));
                    AssetDatabase.CreateAsset(newAsset, assetPath);
                }

                currentAsset = AssetDatabase.LoadAssetAtPath<GraphAsset>(assetPath);
                objectField.SetValueWithoutNotify(currentAsset);
            }
            else
            {
                switch (currentAsset)
                {
                    case AbilityAsset abilityAsset:
                        abilityAsset.Blackboard = blackboardInspector.GetBlackboard();

                        string json = AbilityGraphUtility.Serialize(abilityGraph);
                        if (abilityAsset.GraphJsons.Count == 0)
                        {
                            abilityAsset.AddGraphJson(json);
                        }
                        else
                        {
                            abilityAsset.GraphJsons[0] = json;
                        }
                        EditorUtility.SetDirty(currentAsset);
                        break;
                    case MacroAsset macroAsset:
                        macroAsset.Text = AbilityGraphUtility.Serialize(abilityGraph);
                        EditorUtility.SetDirty(currentAsset);
                        break;
                }
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

        private bool AskForOpenAssetIfDirty(GraphAsset asset)
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
            blackboardInspector.SetBlackboard(new List<BlackboardVariable>());

            SetUpGraphView(new AbilityGraphView(this));
            SetDirty(false);
            objectField.SetValueWithoutNotify(null);
            currentAsset = null;
        }

        private void NewMacroGraphView()
        {
            HideNodeInspector();
            blackboardInspector.SetBlackboard(null);

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

            this.graphView = graphView;
            graphView.name = GRAPH_VIEW_NAME;
            graphView.graphViewChanged += OnGraphViewChanged;
            graphView.serializeGraphElements += SerializeGraphElements;
            graphView.canPasteSerializedData += CanPasteSerializedData;
            graphView.unserializeAndPaste += UnserializeAndPaste;
            graphViewParent.Add(graphView);
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
