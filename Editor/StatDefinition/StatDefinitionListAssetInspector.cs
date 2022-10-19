using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Physalia.AbilitySystem
{
    [CustomEditor(typeof(StatDefinitionListAsset))]
    public class StatDefinitionListAssetInspector : Editor
    {
        [SerializeField]
        private VisualTreeAsset editorAsset = null;
        [SerializeField]
        private VisualTreeAsset itemAsset = null;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement rootVisualElement = new VisualElement();
            if (editorAsset == null)
            {
                Debug.LogError($"[{nameof(StatDefinitionListAssetInspector)}] Missing VisualTreeAsset, set the corrent reference in {nameof(StatDefinitionListAssetInspector)} ScriptAsset might fix this");
                return rootVisualElement;
            }

            if (itemAsset == null)
            {
                Debug.LogError($"[{nameof(StatDefinitionListAssetInspector)}] Missing VisualTreeAsset, set the corrent reference in {nameof(StatDefinitionListAssetInspector)} ScriptAsset might fix this");
                return rootVisualElement;
            }

            editorAsset.CloneTree(rootVisualElement);

            BindGenerator(rootVisualElement);
            BindListView(rootVisualElement);

            return rootVisualElement;
        }

        private void BindGenerator(VisualElement rootVisualElement)
        {
            var namespaceField = rootVisualElement.Q<TextField>("namespace-field");
            namespaceField.BindProperty(serializedObject.FindProperty(nameof(StatDefinitionListAsset.namespaceName)));

            string assetPath = serializedObject.FindProperty(nameof(StatDefinitionListAsset.scriptAssetPath)).stringValue;
            var scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
            var scriptField = rootVisualElement.Q<ObjectField>("script-field");
            scriptField.value = scriptAsset;
            scriptField.SetEnabled(false);

            var generateButton = rootVisualElement.Q<Button>("generate-button");
            generateButton.clicked += GenerateCode;
        }

        private void GenerateCode()
        {
            StatDefinitionListAsset asset = serializedObject.targetObject as StatDefinitionListAsset;
            StatDefinitionCodeGenerator.Generate(asset);
        }

        private void BindListView(VisualElement rootVisualElement)
        {
            var listView = rootVisualElement.Q<ListView>();
            SerializedProperty listProperty = serializedObject.FindProperty(nameof(StatDefinitionListAsset.stats));
            listView.BindProperty(listProperty);
            listView.makeItem = itemAsset.CloneTree;
            listView.bindItem = BindItem;
            listView.unbindItem = UnbindItem;
        }

        private void BindItem(VisualElement element, int i)
        {
            SerializedProperty listProperty = serializedObject.FindProperty(nameof(StatDefinitionListAsset.stats));

            // Note: For preventing ListView refresh bug when deleting
            if (i < 0 || i >= listProperty.arraySize)
            {
                return;
            }

            var idField = element.Q<IntegerField>("id-field");
            var nameField = element.Q<TextField>("name-field");

            SerializedProperty property = listProperty.GetArrayElementAtIndex(i);
            idField.BindProperty(property.FindPropertyRelative(nameof(StatDefinition.Id)));
            nameField.BindProperty(property.FindPropertyRelative(nameof(StatDefinition.Name)));
        }

        private void UnbindItem(VisualElement element, int i)
        {
            var idField = element.Q<IntegerField>(name: "id-field");
            var nameField = element.Q<TextField>(name: "name-field");

            idField.Unbind();
            nameField.Unbind();
        }
    }
}
