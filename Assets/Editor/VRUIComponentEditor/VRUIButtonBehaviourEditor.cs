using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VRUIButtonBehaviour)), CanEditMultipleObjects]
public class VRUIButtonBehaviourEditor : Editor
{
    private readonly static string CREATE_BUTTON_TOOLTIP = "Takes the first child of this object and defines it as the physical part of the button.\n" +
                                                         "If there is no child it will create a basic cube that will be used as a button.";
    private readonly static float CREATE_BUTTON_HEIGHT = 50f;
    private readonly static Vector3 DEFAULT_COLLIDER_SIZE = new Vector3(0.1f, 0.1f, 0.1f);

    private VRUIButtonBehaviour m_target;

    private void OnEnable()
    {
        m_target = (VRUIButtonBehaviour)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        DrawCreateButtonObjectButton();
        if(m_target.PhysicalButton == null)
            EditorGUILayout.HelpBox("No button assigned yet.\nPress the \"Create VRUIButton\" button to do so.", MessageType.Warning);
    }

    private void DrawCreateButtonObjectButton()
    {
        if (GUILayout.Button(new GUIContent("Create/Assign VRUIButton", CREATE_BUTTON_TOOLTIP), GUILayout.Height(CREATE_BUTTON_HEIGHT)))
        {
            GameObject button;
            //If there is no child that should be used as a button we create one.
            if(m_target.transform.childCount < 1)
            {
                button = GameObject.CreatePrimitive(PrimitiveType.Cube);
                button.name = "Button";
                button.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                button.transform.SetParent(m_target.transform);
                button.transform.localPosition = Vector3.zero;
                button.GetComponent<BoxCollider>().isTrigger = true;
                m_target.PhysicalButton = button;
            }
            else
            {
                //If there already is a child we can use it
                button = m_target.transform.GetChild(0).gameObject;
                //We need a collider component so we can register if an object touches the button.
                //If we do not find a collider we try to create one that roughly fits the object.
                if (!button.GetComponent<Collider>())
                {
                    //We first try to fit the collider to the mesh.
                    MeshFilter meshFilter = button.GetComponent<MeshFilter>();
                    if (meshFilter)
                    {
                        Vector3 boundSize = meshFilter.sharedMesh.bounds.size;
                        Vector3 centerPosition = meshFilter.sharedMesh.bounds.center;
                        BoxCollider boxCollider = button.AddComponent<BoxCollider>();
                        boxCollider.size = boundSize;
                        boxCollider.center = centerPosition;
                        boxCollider.isTrigger = true;
                    }
                    //If there is no mesh, we add a fixed size collider that assumes we have cube with the size of (0.1f, 0.1f, 0.1f).
                    else
                    {
                        BoxCollider boxCollider = button.AddComponent<BoxCollider>();
                        boxCollider.size = DEFAULT_COLLIDER_SIZE;
                        boxCollider.center = Vector3.zero;
                        boxCollider.isTrigger = true;
                    }
                }
                //If there already is a collider we make sure it is set as a trigger.
                else
                {
                    Collider collider = button.GetComponent<Collider>();
                    collider.isTrigger = true;
                }
                m_target.PhysicalButton = button;
            }
        }
    }

    public void OnSceneGUI()
    {
        //Draw the button previews
        if (m_target.PhysicalButton)
        {
            GameObject button = m_target.PhysicalButton;
            Collider collider = button.GetComponent<Collider>();
            //Create a box preview
            if (collider.GetType() == typeof(BoxCollider))
            {
                Vector3 size = ((BoxCollider)collider).size;
                Vector3 buttonSize = new Vector3(size.x * button.transform.localScale.x, size.y * button.transform.localScale.y, size.z * button.transform.localScale.z);
                //The position of the maxPushDistance preview box (magenta)
                Vector3 previewPosition = new Vector3(0f, -m_target.MaxPushDistance / button.transform.localScale.y, 0f);
                Matrix4x4 matrix = button.transform.localToWorldMatrix * Matrix4x4.Translate(previewPosition);
                Handles.matrix = matrix;
                Handles.color = Color.magenta;
                Handles.DrawWireCube(Vector3.zero, Vector3.one);
                //The position of the minPushDistance preview box (yellow)
                previewPosition = new Vector3(0f, -(m_target.MaxPushDistance * m_target.MinPushToActivate) / button.transform.localScale.y, 0f);
                matrix = button.transform.localToWorldMatrix * Matrix4x4.Translate(previewPosition);
                Handles.matrix = matrix;
                Handles.color = Color.yellow;
                Handles.DrawWireCube(Vector3.zero, Vector3.one);
            }
        }
    }
}
