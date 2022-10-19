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

            // Generate Button
            var generateButton = rootVisualElement.Q<Button>("generate-button");
            generateButton.clicked += GenerateCode;

            // List View
            var listView = rootVisualElement.Q<ListView>();
            SerializedProperty listProperty = serializedObject.FindProperty(nameof(StatDefinitionListAsset.stats));
            listView.BindProperty(listProperty);
            listView.makeItem = itemAsset.CloneTree;
            listView.bindItem = BindItem;
            listView.unbindItem = UnbindItem;

            return rootVisualElement;
        }

        private void GenerateCode()
        {
            StatDefinitionListAsset asset = serializedObject.targetObject as StatDefinitionListAsset;
            StatDefinitionCodeGenerator.Generate(asset);
        }

        private void BindItem(VisualElement element, int i)
        {
            SerializedProperty listProperty = serializedObject.FindProperty(nameof(StatDefinitionListAsset.stats));
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
