using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VRUISliderBehaviour)), CanEditMultipleObjects]
public class VRUISliderBehaviourEditor : Editor
{
    private readonly static string CREATE_SLIDER_TOOLTIP = "Takes the first child of this object and defines it as the object that holds the path visuals.\n" +
                                                         "Takes the second child of this object and defines it as the physical knob of the slider." +
                                                         "If there is no child or one is missing it will create them with some default settings.";
    private readonly static float CREATE_BUTTON_HEIGHT = 50f;
    private readonly static Vector3 DEFAULT_COLLIDER_SIZE = new Vector3(0.1f, 0.1f, 0.1f);
    private readonly static string NO_PATH_ERROR = "No gameobject with a LineRenderer component found. Please create one or remove all gameobjects from the VRUISlider other than the knob, before trying again.";
    private readonly static string NO_KNOB_ERROR = "No gameobject with a Collider component found. Please create one or remove all gameobjects from the VRUISlider other than the path, before trying again.";

    private VRUISliderBehaviour m_target;

    private void OnEnable()
    {
        m_target = (VRUISliderBehaviour)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        DrawCreateButtonObjectButton();
        if (m_target.PhysicalKnob == null)
            EditorGUILayout.HelpBox("No slider created/assigned yet.\nPress the \"Create VRUISlider\" button to do so.", MessageType.Warning);
        if (!Application.isPlaying && m_target.PhysicalKnob && m_target.Path)
        {
            m_target.CreatePath();
            m_target.UpdateKnobPosition();
        }
    }

    private void DrawCreateButtonObjectButton()
    {
        if (GUILayout.Button(new GUIContent("Create/Assign VRUISlider parts", CREATE_SLIDER_TOOLTIP), GUILayout.Height(CREATE_BUTTON_HEIGHT)))
        {
            GameObject path = null;
            GameObject knob = null;
            //Find out how many children exist and act accordingly.
            if (m_target.transform.childCount < 1)
            {
                //Create path gameobject
                path = new GameObject();
                path.name = "Slider Path";
                path.transform.SetParent(m_target.transform);
                path.transform.localPosition = Vector3.zero;
                LineRenderer lineRenderer = path.AddComponent<LineRenderer>();
                lineRenderer.startWidth = m_target.WidthOfPath;
                lineRenderer.endWidth = m_target.WidthOfPath;
                lineRenderer.material = m_target.PathMaterial;
                //Create the physical knob
                knob = GameObject.CreatePrimitive(PrimitiveType.Cube);
                knob.name = "Slider Knob";
                knob.transform.SetParent(m_target.transform);
                knob.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                knob.transform.localPosition = Vector3.zero;
                knob.GetComponent<BoxCollider>().isTrigger = true;
            }
            else if(m_target.transform.childCount == 1)
            {
                if (m_target.transform.GetChild(0).gameObject.GetComponent<LineRenderer>())
                {
                    //Child 0 renders the path.
                    path = m_target.transform.GetChild(0).gameObject;
                    LineRenderer lineRenderer = path.GetComponent<LineRenderer>();
                    lineRenderer.startWidth = m_target.WidthOfPath;
                    lineRenderer.endWidth = m_target.WidthOfPath;
                    lineRenderer.material = m_target.PathMaterial;
                    //Create the physical knob
                    knob = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    knob.name = "Slider Knob";
                    knob.transform.SetParent(m_target.transform);
                    knob.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                    knob.transform.localPosition = Vector3.zero;
                    knob.GetComponent<BoxCollider>().isTrigger = true;
                }
                else if(m_target.transform.GetChild(0).gameObject.GetComponent<Collider>())
                {
                    //Child 0 is the physical knob
                    knob = m_target.transform.GetChild(0).gameObject;
                    //Create path gameobject
                    path = new GameObject();
                    path.name = "Slider Path";
                    path.transform.SetParent(m_target.transform);
                    path.transform.localPosition = Vector3.zero;
                    LineRenderer lineRenderer = path.AddComponent<LineRenderer>();
                    lineRenderer.startWidth = m_target.WidthOfPath;
                    lineRenderer.endWidth = m_target.WidthOfPath;
                    lineRenderer.material = m_target.PathMaterial;
                    path.transform.SetAsFirstSibling();
                }
            }
            else
            {
                //Check every child of the target and act according to the found component
                foreach (Transform child in m_target.transform)
                {
                    //If the object has a line renderer it is used to render the path.
                    if (!path && child.gameObject.GetComponent<LineRenderer>())
                    {
                        //The path and the knob can not be the same object
                        if(knob != child.gameObject)
                        {
                            path = child.gameObject;
                            LineRenderer lineRenderer = path.GetComponent<LineRenderer>();
                            lineRenderer.startWidth = m_target.WidthOfPath;
                            lineRenderer.endWidth = m_target.WidthOfPath;
                            lineRenderer.material = m_target.PathMaterial;
                        }
                    }
                    //If the object has a collider it is used as the knob.
                    if (!knob && child.gameObject.GetComponent<Collider>())
                    {
                        //The path and the knob can not be the same object
                        if (path != child.gameObject)
                            knob = child.gameObject;
                    }
                }
                knob.transform.SetAsFirstSibling();
                path.transform.SetAsFirstSibling();
            }
            //If we found a suitable gameobject assign it, else show an error message.
            if (!path)
            {
                Debug.LogError(NO_PATH_ERROR);
            }
            else
            {
                m_target.Path = path;
            }
            if (!knob)
            {
                Debug.LogError(NO_KNOB_ERROR);
            }
            else
            {
                m_target.PhysicalKnob = knob;
            }
        }
    }
    public void OnSceneGUI()
    {
        if (m_target.PhysicalKnob && m_target.Path)
        {
            DrawLengthChooser();
        }
    }

    private void DrawLengthChooser()
    {
        float pathLength = m_target.LengthOfPath;

        float size = HandleUtility.GetHandleSize(m_target.gameObject.transform.position) * 0.7f;

        //We need to check if something changed in the inspector so we can record the change for Unity's undo history.
        EditorGUI.BeginChangeCheck();
        Handles.color = Color.magenta;
        pathLength = Handles.ScaleSlider(pathLength, m_target.gameObject.transform.position, m_target.gameObject.transform.up, m_target.gameObject.transform.rotation, size, 0.1f);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(new Object[] { m_target.GetComponent<VRUISliderBehaviour>(), m_target.transform }, "Undo VRUI Slider Length");

            m_target.LengthOfPath = pathLength;
            m_target.CreatePath();
            m_target.UpdateKnobPosition();
        }
    }
}
