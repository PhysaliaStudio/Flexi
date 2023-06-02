using System;
using System.Collections.Generic;
using System.IO;
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
        private static readonly string PREFS_KEY_LAST_SAVED_FOLDER_PATH = "LastSavedFolderPath";
        private static readonly string DEFAULT_FOLDER_PATH = "Assets/";

        // Since we need to use names to find the correct VisualTreeAsset to replace,
        // these names should be corresponding to the contents of the UXML file.
        private static readonly string FILE_FIELD_NAME = "file-field";
        private static readonly string NEW_BUTTON_NAME = "new-button";
        private static readonly string SAVE_BUTTON_NAME = "save-button";
        private static readonly string RELOAD_BUTTON_NAME = "reload-button";
        private static readonly string NEW_MACRO_BUTTON_NAME = "new-macro-button";

        private static readonly string ABILITY_FLOW_MENU_PARENT_NAME = "ability-flow-menu-parent";
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
        private VisualTreeAsset abilityFlowMenuAsset;
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
        [HideInInspector]
        [SerializeField]
        private GraphAsset tempAsset = null;
        [HideInInspector]
        [SerializeField]
        private int currentGraphIndex = 0;

        private ObjectField objectField;

        private AbilityGraphView graphView;
        private AbilityFlowMenu abilityFlowMenu;
        private NodeInspector nodeInspector;
        private BlackboardInspector blackboardInspector;
        private bool isDirty;

        internal int GraphCount
        {
            get
            {
                if (tempAsset is AbilityAsset abilityAsset)
                {
                    return abilityAsset.GraphJsons.Count;
                }
                else
                {
                    return 1;
                }
            }
        }

        internal int CurrentGraphIndex => currentGraphIndex;

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

            SetUpAbilityFlowMenu();
            SetUpNodeInspector();
            SetUpBlackboardInspector();

            if (currentAsset == null)
            {
                NewGraphView();  // Note: This will set currentAsset, so currentAsset is always not null.
            }
            else
            {
                LoadFile(currentAsset, currentGraphIndex);
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

            if (abilityFlowMenuAsset == null)
            {
                abilityFlowMenuAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(folderPath + "AbilityFlowMenu.uxml");
                if (abilityFlowMenuAsset == null)
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

        private void SetUpAbilityFlowMenu()
        {
            VisualElement abilityFlowMenuParent = rootVisualElement.Query<VisualElement>(ABILITY_FLOW_MENU_PARENT_NAME).First();
            abilityFlowMenu = new AbilityFlowMenu(this);
            abilityFlowMenu.CreateGUI(abilityFlowMenuAsset);
            abilityFlowMenuParent.Add(abilityFlowMenu);
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

        private bool LoadFile(GraphAsset asset, int graphIndex)
        {
            HideNodeInspector();

            AbilityGraph abilityGraph;
            switch (asset)
            {
                default:
                    blackboardInspector.SetBlackboard(null);
                    return false;
                case AbilityAsset abilityAsset:
                    if (graphIndex < 0 || graphIndex >= abilityAsset.GraphJsons.Count)
                    {
                        graphIndex = 0;
                    }

                    // In case of empty graph
                    if (abilityAsset.GraphJsons.Count == 0)
                    {
                        graphIndex = -1;
                        RemoveGraphView();

                        ResetAssetState(asset, graphIndex);
                        return true;
                    }

                    blackboardInspector.SetBlackboard(abilityAsset.Blackboard);
                    string graphJson = abilityAsset.GraphJsons[graphIndex];
                    abilityGraph = AbilityGraphUtility.Deserialize(abilityAsset.name, graphJson, MacroLibraryCache.Get());
                    break;
                case MacroAsset macroAsset:
                    graphIndex = 0;
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
            ResetAssetState(asset, graphIndex);
            return true;
        }

        private void ReloadFile()
        {
            bool isBlankAssets = IsBlankAsset(currentAsset);
            if (isBlankAssets)
            {
                ShowNotification(new GUIContent("You can not reload non-asset!"));
                return;
            }

            bool ok = AskForSaveIfDirty();
            if (ok)
            {
                LoadFile(currentAsset, currentGraphIndex);
            }
        }

        private bool SaveFile()
        {
            bool success = SaveToTemp(currentGraphIndex);
            if (!success)
            {
                return false;
            }

            // Save as new asset if necessary
            bool isBlankAssets = IsBlankAsset(currentAsset);
            if (isBlankAssets)
            {
                string lastSavedFolderPath = PlayerPrefs.GetString(PREFS_KEY_LAST_SAVED_FOLDER_PATH, DEFAULT_FOLDER_PATH);
                string assetPath = EditorUtility.SaveFilePanelInProject("Save ability", "NewGraph", "asset",
                    "Please enter a file name to save to", lastSavedFolderPath);
                if (assetPath.Length == 0)
                {
                    return false;
                }

                AssetDatabase.CreateAsset(tempAsset, assetPath);
                currentAsset = AssetDatabase.LoadAssetAtPath<GraphAsset>(assetPath);
                objectField.SetValueWithoutNotify(currentAsset);

                string savedFolderPath = Path.GetDirectoryName(assetPath);
                PlayerPrefs.SetString(PREFS_KEY_LAST_SAVED_FOLDER_PATH, savedFolderPath);

                AssetDatabase.Refresh();
                SetDirty(false);
                return true;
            }

            switch (tempAsset)
            {
                case AbilityAsset abilityAsset:
                    CopyAbilityAsset(abilityAsset, currentAsset as AbilityAsset);
                    break;
                case MacroAsset macroAsset:
                    CopyMacroAsset(macroAsset, currentAsset as MacroAsset);
                    break;
            }

            EditorUtility.SetDirty(currentAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SetDirty(false);
            return true;
        }

        private void CopyAbilityAsset(AbilityAsset source, AbilityAsset destination)
        {
            destination.Blackboard = source.Blackboard;  // This should be copy

            int sourceGraphCount = source.GraphJsons.Count;
            int destinationGraphCount = destination.GraphJsons.Count;
            if (destinationGraphCount > sourceGraphCount)
            {
                destination.GraphJsons.RemoveRange(destinationGraphCount - 1, destinationGraphCount - sourceGraphCount);
                destinationGraphCount = sourceGraphCount;
            }

            for (var i = 0; i < destinationGraphCount; i++)
            {
                destination.GraphJsons[i] = source.GraphJsons[i];
            }

            for (var i = destinationGraphCount; i < sourceGraphCount; i++)
            {
                destination.AddGraphJson(source.GraphJsons[i]);
            }
        }

        private void CopyMacroAsset(MacroAsset source, MacroAsset destination)
        {
            destination.Text = source.Text;
        }

        private bool IsBlankAsset(GraphAsset graphAsset)
        {
            string assetPath = AssetDatabase.GetAssetPath(graphAsset);
            bool isBlankAssets = string.IsNullOrEmpty(assetPath);
            return isBlankAssets;
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
                bool success = LoadFile(asset, 0);
                if (success)
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        internal bool AskForSaveIfDirty()
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

        internal void CreateNewGraph()
        {
            // Note: We want to keep the origianl asset unchanged, so we modified the temp object.
            if (tempAsset is AbilityAsset abilityAsset)
            {
                bool success = SaveToTemp(currentGraphIndex);
                if (!success)
                {
                    return;
                }

                abilityAsset.AddGraphJson("");
                currentGraphIndex = abilityAsset.GraphJsons.Count - 1;
                SetDirty(true);

                abilityFlowMenu.Refresh();

                string graphJson = abilityAsset.GraphJsons[currentGraphIndex];
                AbilityGraph abilityGraph = AbilityGraphUtility.Deserialize(abilityAsset.name, graphJson, MacroLibraryCache.Get());
                AbilityGraphView graphView = AbilityGraphView.Create(abilityGraph, this);
                SetUpGraphView(graphView);
            }
        }

        internal void SelectGraph(int index)
        {
            if (tempAsset is AbilityAsset abilityAsset)
            {
                if (index >= 0 && index < abilityAsset.GraphJsons.Count)
                {
                    bool success = SaveToTemp(currentGraphIndex);
                    if (!success)
                    {
                        return;
                    }

                    currentGraphIndex = index;
                    abilityFlowMenu.Refresh();

                    string graphJson = abilityAsset.GraphJsons[index];
                    AbilityGraph abilityGraph = AbilityGraphUtility.Deserialize(abilityAsset.name, graphJson, MacroLibraryCache.Get());
                    AbilityGraphView graphView = AbilityGraphView.Create(abilityGraph, this);
                    SetUpGraphView(graphView);
                }
            }
        }

        internal void DeleteGraph(int index)
        {
            if (tempAsset is AbilityAsset abilityAsset)
            {
                if (index >= 0 && index < abilityAsset.GraphJsons.Count)
                {
                    abilityAsset.GraphJsons.RemoveAt(index);
                    SetDirty(true);

                    abilityFlowMenu.Refresh();

                    ShowNotification(new GUIContent($"Flow {index} is deleted!"));

                    if (abilityAsset.GraphJsons.Count == 0)
                    {
                        currentGraphIndex = -1;
                        RemoveGraphView();
                    }
                    else
                    {
                        if (currentGraphIndex >= abilityAsset.GraphJsons.Count)
                        {
                            currentGraphIndex = abilityAsset.GraphJsons.Count - 1;
                        }

                        string graphJson = abilityAsset.GraphJsons[currentGraphIndex];
                        AbilityGraph abilityGraph = AbilityGraphUtility.Deserialize(abilityAsset.name, graphJson, MacroLibraryCache.Get());
                        AbilityGraphView graphView = AbilityGraphView.Create(abilityGraph, this);
                        SetUpGraphView(graphView);
                    }
                }
            }
        }

        /// <summary>
        /// Save the current graph to temp, so we can restore it later.
        /// </summary>
        private bool SaveToTemp(int index)
        {
            // If graphView is null, it means there is no graph in this asset, so we can skip saving.
            if (graphView == null)
            {
                return true;
            }

            // Is the graph has any missing element, cancel saving
            AbilityGraph abilityGraph = graphView.GetAbilityGraph();
            if (abilityGraph.HasMissingElement())
            {
                ShowNotification(new GUIContent("You must fix all the missing elements before saving!"));
                return false;
            }

            switch (tempAsset)
            {
                case AbilityAsset abilityAsset:
                    abilityAsset.Blackboard = blackboardInspector.GetBlackboard();
                    string json = AbilityGraphUtility.Serialize(abilityGraph);
                    abilityAsset.GraphJsons[index] = json;
                    break;
                case MacroAsset macroAsset:
                    macroAsset.Text = AbilityGraphUtility.Serialize(abilityGraph);
                    break;
            }

            return true;
        }

        private void NewGraphView()
        {
            HideNodeInspector();
            blackboardInspector.SetBlackboard(new List<BlackboardVariable>());

            // Create new asset in memory, and add an empty graph
            AbilityAsset newAsset = CreateInstance<AbilityAsset>();
            newAsset.name = "NewAbility";
            newAsset.AddGraphJson("");

            SetUpGraphView(new AbilityGraphView(this));

            ResetAssetState(newAsset, 0);
        }

        private void NewMacroGraphView()
        {
            HideNodeInspector();
            blackboardInspector.SetBlackboard(null);

            // Create new asset in memory, and add an empty graph
            MacroAsset newAsset = CreateInstance<MacroAsset>();
            newAsset.name = "NewMacro";
            newAsset.Text = "";

            AbilityGraph graph = new AbilityGraph();
            graph.AddSubgraphInOutNodes();
            graph.GraphInputNode.position = new Vector2(0, 250);
            graph.GraphOutputNode.position = new Vector2(500, 250);

            AbilityGraphView graphView = AbilityGraphView.Create(graph, this);
            SetUpGraphView(graphView);

            ResetAssetState(newAsset, 0);
        }

        private void ResetAssetState(GraphAsset graphAsset, int graphIndex)
        {
            SetDirty(false);
            currentAsset = graphAsset;
            tempAsset = Instantiate(currentAsset);
            currentGraphIndex = graphIndex;

            objectField.SetValueWithoutNotify(currentAsset);
            abilityFlowMenu.Refresh();
        }

        private void RemoveGraphView()
        {
            VisualElement graphViewParent = rootVisualElement.Query<VisualElement>(GRAPH_VIEW_PARENT_NAME).First();
            graphViewParent.Clear();
            graphView = null;
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

        internal bool IsDirty()
        {
            return isDirty;
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

        private void OnDestroy()
        {
            AskForSaveIfDirty();
        }
    }
}
