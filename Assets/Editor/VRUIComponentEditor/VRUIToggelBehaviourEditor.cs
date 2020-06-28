/********************************************************************************//*
Created as part of a Bsc in Computer Science for the BFH Biel
Created by:   Steven Henz
Date:         26.05.20
Email:        steven.henz93@gmail.com
************************************************************************************/
using UnityEngine;
using UnityEditor;

/// <summary>
/// The Custom Editor Script for the VRUIToggleBehaviour Script. Amongst other things draws the wire previews from the scene view.
/// </summary>
[CustomEditor(typeof(VRUIToggleBehaviour)), CanEditMultipleObjects]
public class VRUIToggleBehaviourEditor : Editor
{
    private readonly static string CREATE_TOGGLE_TOOLTIP = "Takes the first child of this object and defines it as the physical part of the toggle.\n" +
                                                         "If there is no child it will create a basic cube that will be used as a toggle.";
    private readonly static float CREATE_TOGGLE_HEIGHT = 50f;
    private readonly static Vector3 DEFAULT_COLLIDER_SIZE = new Vector3(0.1f, 0.1f, 0.1f);

    private VRUIToggleBehaviour m_target;

    private void OnEnable()
    {
        m_target = (VRUIToggleBehaviour)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        DrawCreateToggleObjectButton();
        if (m_target.PhysicalToggle == null)
            EditorGUILayout.HelpBox("No toggle assigned yet.\nPress the \"Create VRUIToggle\" toggle to do so.", MessageType.Warning);
    }

    /// <summary>
    /// Creates the lowest button in the inspector. Has logic to check if all of the necessary components are present.
    /// </summary>
    private void DrawCreateToggleObjectButton()
    {
        if (GUILayout.Button(new GUIContent("Create/Assign VRUIToggle", CREATE_TOGGLE_TOOLTIP), GUILayout.Height(CREATE_TOGGLE_HEIGHT)))
        {
            GameObject toggle;
            //If there is no child that should be used as a toggle we create one.
            if (m_target.transform.childCount < 1)
            {
                toggle = GameObject.CreatePrimitive(PrimitiveType.Cube);
                toggle.name = "Toggle";
                toggle.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                toggle.transform.SetParent(m_target.transform);
                toggle.transform.localPosition = Vector3.zero;
                toggle.GetComponent<BoxCollider>().isTrigger = true;
                m_target.PhysicalToggle = toggle;
            }
            else
            {
                //If there already is a child we can use it
                toggle = m_target.transform.GetChild(0).gameObject;
                //We need a collider component so we can register if an object touches the toggle.
                //If we do not find a collider we try to create one that roughly fits the object.
                if (!toggle.GetComponent<Collider>())
                {
                    //We first try to fit the collider to the mesh.
                    MeshFilter meshFilter = toggle.GetComponent<MeshFilter>();
                    if (meshFilter)
                    {
                        Vector3 boundSize = meshFilter.sharedMesh.bounds.size;
                        Vector3 centerPosition = meshFilter.sharedMesh.bounds.center;
                        BoxCollider boxCollider = toggle.AddComponent<BoxCollider>();
                        boxCollider.size = boundSize;
                        boxCollider.center = centerPosition;
                        boxCollider.isTrigger = true;
                    }
                    //If there is no mesh, we add a fixed size collider that assumes we have cube with the size of (0.1f, 0.1f, 0.1f).
                    else
                    {
                        BoxCollider boxCollider = toggle.AddComponent<BoxCollider>();
                        boxCollider.size = DEFAULT_COLLIDER_SIZE;
                        boxCollider.center = Vector3.zero;
                        boxCollider.isTrigger = true;
                    }
                }
                //If there already is a collider we make sure it is set as a trigger.
                else
                {
                    Collider collider = toggle.GetComponent<Collider>();
                    collider.isTrigger = true;
                }
                m_target.PhysicalToggle = toggle;
            }
        }
    }

    public void OnSceneGUI()
    {
        //Draw the toggle previews
        if (m_target.PhysicalToggle)
        {
            GameObject toggle = m_target.PhysicalToggle;
            Collider collider = toggle.GetComponent<Collider>();
            //Creates a box preview
            if (collider.GetType() == typeof(BoxCollider))
            {
                Vector3 size = ((BoxCollider)collider).size;
                //The position of the maxPushDistance preview box (magenta)
                Vector3 previewPosition = new Vector3(0f, -m_target.MaxPushDistance / toggle.transform.localScale.y, 0f);
                Matrix4x4 matrix = toggle.transform.localToWorldMatrix * Matrix4x4.Translate(previewPosition);
                Handles.matrix = matrix;
                Handles.color = Color.magenta;
                Vector3 drawAtPosition = Vector3.zero;
                if (((BoxCollider)collider).center != Vector3.zero)
                    drawAtPosition = ((BoxCollider)collider).center;
                Handles.DrawWireCube(drawAtPosition, size);
                //The position of the minPushDistance preview box (yellow)
                previewPosition = new Vector3(0f, -(m_target.MaxPushDistance * m_target.MinPushToActivate) / toggle.transform.localScale.y, 0f);
                matrix = toggle.transform.localToWorldMatrix * Matrix4x4.Translate(previewPosition);
                Handles.matrix = matrix;
                Handles.color = Color.yellow;
                Handles.DrawWireCube(drawAtPosition, size);
                //The position of the stuckDistance preview box (green)
                previewPosition = new Vector3(0f, -(m_target.MaxPushDistance * m_target.MinPushToActivate * m_target.StuckDistance) / toggle.transform.localScale.y, 0f);
                matrix = toggle.transform.localToWorldMatrix * Matrix4x4.Translate(previewPosition);
                Handles.matrix = matrix;
                Handles.color = Color.green;
                Handles.DrawWireCube(drawAtPosition, size);
            }
        }
    }
}
