/********************************************************************************//*
Created as part of a Bsc in Computer Science for the BFH Biel
Created by:   Steven Henz
Date:         26.05.20
Email:        steven.henz93@gmail.com
************************************************************************************/
using UnityEngine;
using UnityEditor;

/// <summary>
/// The Custom Editor Script for the VRUIPositioner Script.
/// </summary>
[CustomEditor(typeof(VRUIPositioner))]
public class VRUIPositionerEditor : Editor
{
    //Context menu string constants
    private const string STRING_RESET_POSITION_UNDO = "VRUIPositioner Reset Position";
    //InspectorGUI string constants
        //Position field
    private const string RELATIVE_POSITION_LABEL = "Relative Position";
    private const string RELATIVE_POSITION_TOOLTIP = "The position of this object relative to its anchor.";
        //Anchor field
    private const string ANCHOR_LABEL = "Anchor";
    private const string ANCHOR_TOOLTIP = "The elements chosen anchor. At position (0, 0, 0) the element has the same position as the anchor.";

    private VRUIPositioner m_target;

    private SerializedProperty relativePositionProp;
    private SerializedProperty relativeEulerRotationProp;
    private SerializedProperty anchorProp;

    private Vector3 oldRelativePosition;
    private VRUIAnchor oldAnchor;

    //Creates the "Reset Relative Position" option in the component context menu.
    [MenuItem("CONTEXT/VRUIPositioner/Reset Relative Position")]
    static void ResetPosition(MenuCommand command)
    {
        VRUIPositioner target = (VRUIPositioner)command.context;
        Undo.RecordObjects(new Object[] { target, target.transform }, STRING_RESET_POSITION_UNDO);
        target.RelativePosition = Vector3.zero;
        target.SetPositionRelativeToAnchor();
    }

    private void OnEnable()
    {
        m_target = (VRUIPositioner)target;

        relativePositionProp = serializedObject.FindProperty("relativePosition");
        relativeEulerRotationProp = serializedObject.FindProperty("relativeRotation");
        relativeEulerRotationProp = serializedObject.FindProperty("relativeEulerRotation");
        anchorProp = serializedObject.FindProperty("anchor");

        VRUIScrollPanelBehaviour parentScrollPanel = null;
        if (m_target.transform.parent)
            parentScrollPanel = m_target.transform.parent.gameObject.GetComponent<VRUIScrollPanelBehaviour>();
        if (parentScrollPanel)
        {
            Tools.hidden = true;
            m_target.hideFlags = HideFlags.NotEditable;
        }
    }

    private void OnDisable()
    {
        Tools.hidden = false;
        if(m_target != null)
            m_target.hideFlags = HideFlags.None;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        oldRelativePosition = relativePositionProp.vector3Value;
        oldAnchor = (VRUIAnchor)anchorProp.intValue;

        if (m_target.HaveToUpdateRelativeValues)
            UpdateRelativeValues();

        DrawRelativePositionField();
        if (m_target.HasParentVRUIPositioner())
        {
            EditorGUILayout.Space();
            DrawAnchorDropdown();
        }
        serializedObject.ApplyModifiedProperties();
        
        if (oldRelativePosition != relativePositionProp.vector3Value && !m_target.HaveToUpdateRelativeValues)
        {
            m_target.SetPositionRelativeToAnchor();
            m_target.SetupAnchor();
        }
        if(oldAnchor != (VRUIAnchor)anchorProp.intValue)
        {
            m_target.SetupAnchor();
        }
        PrefabUtility.RecordPrefabInstancePropertyModifications(m_target.gameObject);
    }

    /// <summary>
    /// If the VRUIElement was moved, we need to update the relative values correctly, depending on the change.
    /// This will only be done if the VRUIPositioner was not the source of the position change.
    /// </summary>
    private void UpdateRelativeValues()
    {
        m_target.SetupAnchor();
        relativePositionProp.vector3Value = m_target.transform.position - m_target.AnchorPosition;
        m_target.transform.hasChanged = false;
        m_target.HaveToUpdateRelativeValues = false;
    }

    private void DrawRelativePositionField()
    {
        EditorGUILayout.PropertyField(relativePositionProp, new GUIContent(RELATIVE_POSITION_LABEL, RELATIVE_POSITION_TOOLTIP));
    }

    private void DrawAnchorDropdown()
    {
        string[] choices = System.Enum.GetNames(typeof(VRUIAnchor));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(new GUIContent(ANCHOR_LABEL, ANCHOR_TOOLTIP));
        anchorProp.intValue = EditorGUILayout.Popup(anchorProp.intValue, choices);
        EditorGUILayout.EndHorizontal();
    }
}
