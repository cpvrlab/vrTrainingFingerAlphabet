/********************************************************************************//*
Created as part of a Bsc in Computer Science for the BFH Biel
Created by:   Steven Henz
Date:         26.05.20
Email:        steven.henz93@gmail.com
************************************************************************************/
using UnityEngine;
using UnityEditor;

/// <summary>
/// The Custom Editor Script for the VRUIScrollPanelBehaviour Script. Among other things creates the buttons in the inspector, that can be used
/// to preview the step function of the script.
/// </summary>
[CustomEditor(typeof(VRUIScrollPanelBehaviour)), CanEditMultipleObjects]
public class VRUIScrollPanelBehaviourEditor : Editor
{
    private const float BUTTONWIDTH = 120f;
    private const string MAX_DISP_ELEM_TOOLTIP = "The maximum amount of displayed elements. This determines how the available space of the panel gets split. " +
                                                 "Can not be set higher than the amount of child objects of this object.";
    private const string CAN_OVERSTEP_TOOLTIP = "If this is true calling StepForward() at the end of the list will bring the user back to the start " +
                                                 "and calling StepBackward() at the start of the list will bring the user to the end of the list.";
    private const string SCROLL_STEP_SIZE_TOOLTIP = "Defines how many elements we scroll if StepForward() or StepBackward() gets called. Can't be higher than maxDisplayElements.";

    VRUIScrollPanelBehaviour m_target;
    //ScrollPanelProperties
    SerializedProperty maxDisplayedElementsProp;
    SerializedProperty layoutProp;
    SerializedProperty scrollStepSizeProp;
    SerializedProperty canOverstepProp;
    SerializedProperty zPositionOfChildrenProp;

    private void OnEnable()
    {
        m_target = m_target = (VRUIScrollPanelBehaviour)target;
        m_target.panel = m_target.GetComponent<VRUIPanelBehaviour>();

        if (m_target.transform.childCount > 0)
        {
            m_target.DisplayCorrectChildElements();
            m_target.ArrangeElements();
        }

        maxDisplayedElementsProp = serializedObject.FindProperty("maxDisplayedElements");
        layoutProp = serializedObject.FindProperty("layout");
        scrollStepSizeProp = serializedObject.FindProperty("scrollStepSize");
        canOverstepProp = serializedObject.FindProperty("canOverstep");
        zPositionOfChildrenProp = serializedObject.FindProperty("zPositionOfChildren");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        DrawScrollPanelFields();
        EditorGUILayout.Space();
        m_target = (VRUIScrollPanelBehaviour)target;
        //If there are no child objects, we can not use the preview functions, because there is nothing to scroll through.
        if (m_target.transform.childCount < 1)
        {
            EditorGUILayout.HelpBox("Add at least one child to use the preview functions.", MessageType.Warning);
        }
        else
        {
            DrawStepButtons();
        }
        //This value cant be smaller than 1, because there should always be at least one object shown.
        if (maxDisplayedElementsProp.intValue < 1)
            maxDisplayedElementsProp.intValue = 1;
        //Call functions after applying new values
        if (serializedObject.ApplyModifiedProperties())
        {
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

    private void DrawScrollPanelFields()
    {
        EditorGUILayout.LabelField("Scroll Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(scrollStepSizeProp, new GUIContent("Scroll Step Size", SCROLL_STEP_SIZE_TOOLTIP));
        EditorGUILayout.PropertyField(canOverstepProp, new GUIContent("Can Overstep", CAN_OVERSTEP_TOOLTIP));
        EditorGUILayout.PropertyField(maxDisplayedElementsProp, new GUIContent("Max Displayed Elements", MAX_DISP_ELEM_TOOLTIP));
        string[] choices = System.Enum.GetNames(typeof(VRUIScrollPanelBehaviour.Layout));
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent("Layout", "Defines how the child elements get arranged."));
        layoutProp.intValue = EditorGUILayout.Popup(layoutProp.intValue, choices);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(zPositionOfChildrenProp, new GUIContent("Z Position Of Children", "The local z-position of the child objects."));
    }

    /// <summary>
    /// Creates the buttons that can be used to preview the scroll functions.
    /// </summary>
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
}
