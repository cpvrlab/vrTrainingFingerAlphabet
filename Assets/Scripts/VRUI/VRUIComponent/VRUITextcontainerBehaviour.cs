/********************************************************************************//*
Created as part of a Bsc in Computer Science for the BFH Biel
Created by:   Steven Henz
Date:         26.05.20
Email:        steven.henz93@gmail.com
************************************************************************************/
using UnityEngine;
using TMPro;

/// <summary>
/// This component helps placing and managing textboxes in 3D space.
/// </summary>
public class VRUITextcontainerBehaviour : MonoBehaviour
{
    [SerializeField]
    [Tooltip("If this is true, the text will always \"look\" at the main camera of the scene.")]
    private bool billboardBehaviourActive;

    private GameObject textcontainer;
    private TextMeshPro textMesh;

    // Start is called before the first frame update
    void Awake()
    {
        textcontainer = transform.Find("TMPro Text").gameObject;
        if (textcontainer)
        {
            textMesh = textcontainer.gameObject.GetComponent<TextMeshPro>();
        }   
        else
            Debug.LogError("No texcontainer found!");
    }

    // Update is called once per frame
    void Update()
    {
        if (billboardBehaviourActive)
        {
            transform.LookAt(2 * transform.position - Camera.main.transform.position);
        }
    }

    void Reset()
    {
        if (transform.childCount < 1 || !transform.Find("TMPro Text").gameObject)
        {
            textcontainer = new GameObject("TMPro Text");
            textcontainer.transform.SetParent(transform);
        } else
        {
            textcontainer = transform.Find("TMPro Text").gameObject;
        }

        if (!textcontainer.GetComponent<TextMeshPro>())
        {
            textMesh = textcontainer.gameObject.AddComponent<TextMeshPro>();
            textMesh.fontSize = 1f;
            textMesh.text = "New Text";
            textcontainer.GetComponent<RectTransform>().sizeDelta = new Vector2(1f, 0.25f);
        }
    }

    public bool BillboardBehaviourActive
    {
        get { return billboardBehaviourActive; }
        set { billboardBehaviourActive = value; }
    }

    public void ChangeTextTo(string text)
    {
        if(textMesh)
            textMesh.text = text;
    }

    public void ChangeTextToFloat(float value)
    {
        if (textMesh)
            textMesh.text = value.ToString("0.00");
    }
}
