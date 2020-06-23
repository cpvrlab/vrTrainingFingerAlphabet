using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VRUIScrollPanelBehaviour)), CanEditMultipleObjects]
public class VRUIScrollPanelBehaviourEditor : Editor
{
    private const float BUTTONWIDTH = 120f;
    private const string MAX_DISP_ELEM_TOOLTIP = "The maximum amount of displayed elements. This determines how the available space of the panel gets split. " +
                                                 "Can not be set higher than the amount of child objects of this object.";

    VRUIScrollPanelBehaviour m_target;
    //PanelProperties
    SerializedProperty panelSizeXProp;
    SerializedProperty panelSizeYProp;
    //ScrollPanelProperties
    SerializedProperty maxDisplayedElementsProp;
    SerializedProperty layoutProp;
    SerializedProperty zPositionOfChildrenProp;

    private void OnEnable()
    {
        panelSizeXProp = serializedObject.FindProperty("panelSizeX");
        panelSizeYProp = serializedObject.FindProperty("panelSizeY");

        maxDisplayedElementsProp = serializedObject.FindProperty("maxDisplayedElements");
        layoutProp = serializedObject.FindProperty("layout");
        zPositionOfChildrenProp = serializedObject.FindProperty("zPositionOfChildren");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        DrawScrollPanelFields();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Panel Dimensions", EditorStyles.boldLabel);
        DrawSizeFields();
        m_target = (VRUIScrollPanelBehaviour)target;
        
        if (m_target.transform.childCount < 1)
        {
            EditorGUILayout.HelpBox("Add at least one child to use the preview functions.", MessageType.Warning);
        }
        else
        {
            DrawStepButtons();
        }
        //This value cant be smaller than 1
        if (maxDisplayedElementsProp.intValue < 1)
            maxDisplayedElementsProp.intValue = 1;
        //Call functions after applying new values
        if (serializedObject.ApplyModifiedProperties())
        {
            foreach (VRUIPanelBehaviour panel in targets)
            {
                if ((PrefabUtility.GetPrefabAssetType(panel) != PrefabAssetType.Regular) || (PrefabUtility.GetPrefabAssetType(panel) != PrefabAssetType.Variant))
                {
                    panel.RedrawPanel();
                }
            }
            foreach (VRUIScrollPanelBehaviour scrollPanel in targets)
            {
                if ((PrefabUtility.GetPrefabAssetType(scrollPanel) != PrefabAssetType.Regular) || (PrefabUtility.GetPrefabAssetType(scrollPanel) != PrefabAssetType.Variant))
                {
                    scrollPanel.DisplayCorrectChildElements();
                    scrollPanel.ArrangeElements();
                }
            }
        }
    }

    private void DrawSizeFields()
    {
        panelSizeXProp.floatValue = EditorGUILayout.FloatField("Panel Size X", panelSizeXProp.floatValue);
        panelSizeYProp.floatValue = EditorGUILayout.FloatField("Panel Size Y", panelSizeYProp.floatValue);
    }

    private void DrawScrollPanelFields()
    {
        EditorGUILayout.PropertyField(maxDisplayedElementsProp, new GUIContent("Max Displayed Elements", MAX_DISP_ELEM_TOOLTIP));
        string[] choices = System.Enum.GetNames(typeof(VRUIScrollPanelBehaviour.Layout));
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent("Layout", "Defines how the child elements get arranged."));
        layoutProp.intValue = EditorGUILayout.Popup(layoutProp.intValue, choices);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(zPositionOfChildrenProp, new GUIContent("Z Position Of Children", "The local z-position of the child objects."));
    }

    private void DrawStepButtons()
    {
        EditorGUILayout.Space();
        //Build step forward and step backward buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Step Forward", "Calls StepForward() and steps one step forward."), GUILayout.Width(BUTTONWIDTH)))
        {
            m_target.StepForward();
            EditorUtility.SetDirty(m_target);
        }
        if (GUILayout.Button(new GUIContent("Step Backward", "Calls StepBackward() and steps one step back."), GUILayout.Width(BUTTONWIDTH)))
        {
            m_target.StepBackward();
            EditorUtility.SetDirty(m_target);
        }
        EditorGUILayout.EndHorizontal();
        //Build step to start and step to end buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Step to Start", "Calls StepToStart() and sets the position in the list to 0, so that the first elements are displayed."), GUILayout.Width(BUTTONWIDTH)))
        {
            m_target.StepToStart();
            EditorUtility.SetDirty(m_target);
        }
        if (GUILayout.Button(new GUIContent("Step to End", "Calls StepToEnd() and sets the position in the list to (childCount - maxDisplayedElements), so that the last elements are displayed."), GUILayout.Width(BUTTONWIDTH)))
        {
            m_target.StepToEnd();
            EditorUtility.SetDirty(m_target);
        }
        EditorGUILayout.EndHorizontal();
    }

    public void OnSceneGUI()
    {
        m_target = (VRUIScrollPanelBehaviour)target;
        //Draw the size chooser gizmo.
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
