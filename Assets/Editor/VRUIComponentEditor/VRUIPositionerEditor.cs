using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VRUIPositioner))]
public class VRUIPositionerEditor : Editor
{
    //Context menu string constants
    private const string STRING_RESET_POSITION_UNDO = "VRUIPositioner Reset Position";
    private const string STRING_RESET_ROTATION_UNDO = "VRUIPositioner Reset Rotation";
    //InspectorGUI string constants
        //Position field
    private const string RELATIVE_POSITION_LABEL = "Relative Position";
    private const string RELATIVE_POSITION_TOOLTIP = "The position of this object relative to its anchor.";
        //Rotation field
    private const string RELATIVE_ROTATION_LABEL = "Relative Rotation";
    private const string RELATIVE_ROTATION_TOOLTIP = "The rotation of this object relative to its parent object.";
        //Anchor field
    private const string ANCHOR_LABEL = "Anchor";
    private const string ANCHOR_TOOLTIP = "The elements chosen anchor. At position (0, 0, 0) the element has the same position as the anchor.";

    private VRUIPositioner m_target;

    private SerializedProperty relativePositionProp;
    private SerializedProperty relativeEulerRotationProp;
    private SerializedProperty anchorProp;
    //private SerializedProperty anchorPositionProp;

    private Vector3 oldRelativePosition;
    private Vector3 oldRelativeEulerRotation;
    private VRUIAnchor oldAnchor;
    //private Vector3 oldAnchorPosition;

    [MenuItem("CONTEXT/VRUIPositioner/Reset Relative Position")]
    static void ResetPosition(MenuCommand command)
    {
        VRUIPositioner target = (VRUIPositioner)command.context;
        Undo.RecordObjects(new Object[] { target, target.transform }, STRING_RESET_POSITION_UNDO);
        target.RelativePosition = Vector3.zero;
        target.SetPositionRelativeToAnchor();
    }
    /*
    [MenuItem("CONTEXT/VRUIPositioner/Reset Relative Rotation")]
    static void ResetRotation(MenuCommand command)
    {
        VRUIPositioner target = (VRUIPositioner)command.context;
        Undo.RecordObjects(new Object[] { target, target.transform }, STRING_RESET_ROTATION_UNDO);
        target.RelativeRotation = Quaternion.identity;
        target.SetRotationRelativeToParent();
    }*/

    private void OnEnable()
    {
        m_target = (VRUIPositioner)target;

        relativePositionProp = serializedObject.FindProperty("relativePosition");
        relativeEulerRotationProp = serializedObject.FindProperty("relativeRotation");
        relativeEulerRotationProp = serializedObject.FindProperty("relativeEulerRotation");
        anchorProp = serializedObject.FindProperty("anchor");
        //anchorPositionProp = serializedObject.FindProperty("anchorPosition");
        //oldAnchorPosition = anchorPositionProp.vector3Value;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        oldRelativePosition = relativePositionProp.vector3Value;
        oldRelativeEulerRotation = relativeEulerRotationProp.vector3Value;
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
            //EditorUtility.SetDirty(m_target);
            //Undo.RegisterCompleteObjectUndo(m_target.gameObject.transform, "Test2");
        }
        if(oldAnchor != (VRUIAnchor)anchorProp.intValue)
        {
            m_target.SetupAnchor();
            //EditorUtility.SetDirty(m_target);
            //Undo.undoRedoPerformed += m_target.;
        }
        PrefabUtility.RecordPrefabInstancePropertyModifications(m_target.gameObject);
    }

    private void UpdateRelativeValues()
    {
        m_target.SetupAnchor();
        relativePositionProp.vector3Value = m_target.transform.position - m_target.AnchorPosition;
        //anchorPositionProp.vector3Value = oldAnchorPosition;
        m_target.transform.hasChanged = false;
        m_target.HaveToUpdateRelativeValues = false;
    }

    private void DrawRelativePositionField()
    {
        //relativePositionProp.vector3Value = EditorGUILayout.Vector3Field(new GUIContent(RELATIVE_POSITION_LABEL, RELATIVE_POSITION_TOOLTIP), relativePositionProp.vector3Value);
        EditorGUILayout.PropertyField(relativePositionProp, new GUIContent(RELATIVE_POSITION_LABEL, RELATIVE_POSITION_TOOLTIP));
    }

    private void DrawRelativeRotationField()
    {
        EditorGUILayout.PropertyField(relativeEulerRotationProp, new GUIContent(RELATIVE_ROTATION_LABEL, RELATIVE_ROTATION_TOOLTIP));
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
