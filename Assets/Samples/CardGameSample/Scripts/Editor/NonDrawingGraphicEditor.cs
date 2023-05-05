using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

/// <remarks>
/// Reference: http://answers.unity.com/answers/1157876/view.html
/// </remarks>
[CanEditMultipleObjects, CustomEditor(typeof(NonDrawingGraphic), false)]
public class NonDrawingGraphicEditor : GraphicEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_Script, new GUILayoutOption[0]);

        // Skip AppearanceControlsGUI
        RaycastControlsGUI();
        serializedObject.ApplyModifiedProperties();
    }
}
