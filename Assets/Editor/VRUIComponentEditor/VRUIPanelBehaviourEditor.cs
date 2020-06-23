using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VRUIPanelBehaviour)), CanEditMultipleObjects]
public class VRUIPanelBehaviourEditor : Editor
{
    SerializedProperty panelSizeXProp;
    SerializedProperty panelSizeYProp;

    private void OnEnable()
    {
        panelSizeXProp = serializedObject.FindProperty("panelSizeX");
        panelSizeYProp = serializedObject.FindProperty("panelSizeY");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Panel Dimensions", EditorStyles.boldLabel);
        DrawSizeFields();
        if (serializedObject.ApplyModifiedProperties())
        {
            foreach (VRUIPanelBehaviour panel in targets)
            {
                if ((PrefabUtility.GetPrefabAssetType(panel) != PrefabAssetType.Regular) || (PrefabUtility.GetPrefabAssetType(panel) != PrefabAssetType.Variant))
                    panel.RedrawPanel();
            }
        }
    }

    private void DrawSizeFields()
    {
        panelSizeXProp.floatValue = EditorGUILayout.FloatField("Panel Size X", panelSizeXProp.floatValue);
        panelSizeYProp.floatValue = EditorGUILayout.FloatField("Panel Size Y", panelSizeYProp.floatValue);
    }

    public void OnSceneGUI()
    {
        DrawSizeChooserGizmo();
    }

    private void DrawSizeChooserGizmo()
    {
        VRUIPanelBehaviour m_target = (VRUIPanelBehaviour)target;

        float panelSizeX = m_target.PanelSizeX;
        float panelSizeY = m_target.PanelSizeY;

        float size = HandleUtility.GetHandleSize(m_target.gameObject.transform.position) * 0.7f;

        //We need to check if something changed in the inspector so we can record the change for Unity's undo history.
        EditorGUI.BeginChangeCheck();
        Handles.color = Color.magenta;
        panelSizeX = Handles.ScaleSlider(panelSizeX, m_target.gameObject.transform.position, m_target.gameObject.transform.right, m_target.gameObject.transform.rotation, size, 0.5f);
        Handles.color = Color.yellow;
        panelSizeY = Handles.ScaleSlider(panelSizeY, m_target.gameObject.transform.position, m_target.gameObject.transform.up, m_target.gameObject.transform.rotation, size, 0.5f);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(new Object[] { m_target.GetComponent<VRUIPanelBehaviour>(), m_target.transform }, "Undo VRUI Panel Size");

            m_target.PanelSizeX = panelSizeX;
            m_target.PanelSizeY = panelSizeY;
            m_target.RedrawPanel();
        }
    }
}
