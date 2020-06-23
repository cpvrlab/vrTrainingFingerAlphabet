using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VRUITextcontainerBehaviour : MonoBehaviour
{
    [SerializeField]
    [Tooltip("If this is true, the text will always \"look\" at the main camera of the scene.")]
    private bool billboardBehaviourActive;
    /*
    [SerializeField]
    [Tooltip("TODO: Not correctly implemented")]
    private bool limitRotation = false;
    [SerializeField]
    [Tooltip("The maximum deviation from the originally set rotation. Use this to prevent text from rotating in such a way, that it clips into geometry.")]
    private Vector3 maxDeltaRotation = Vector3.zero;
    */
    private Vector3 startRotation;

    private GameObject textcontainer;
    private TextMeshPro textMesh;

    // Start is called before the first frame update
    void Start()
    {
        startRotation = transform.eulerAngles;
        textcontainer = transform.Find("TMPro Text").gameObject;
        if (textcontainer)
        {
            textMesh = textcontainer.gameObject.GetComponent<TextMeshPro>();
            if (billboardBehaviourActive)
            {
                //Rotate the text by 180 degerees so it isn't the wrong way arround when we use the billboard effect.
                //textMesh.transform.Rotate(0f, 180f, 0f);
            }
        }   
        else
            Debug.LogError("No texcontainer found!");
    }

    // Update is called once per frame
    void Update()
    {
        if (billboardBehaviourActive)
        {
            /*
            if (limitRotation)
            {
                transform.LookAt(2 * transform.position - Camera.main.transform.position);
                Vector3 deltaRotation = transform.eulerAngles - startRotation;
                Debug.Log("deltaRotation=" + deltaRotation);
                if (Mathf.Abs(deltaRotation.x) >= maxDeltaRotation.x)
                {
                    transform.eulerAngles = new Vector3(startRotation.x + maxDeltaRotation.x, transform.eulerAngles.y, transform.eulerAngles.z);
                }
                if (Mathf.Abs(deltaRotation.y) >= maxDeltaRotation.y)
                {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, startRotation.y + maxDeltaRotation.y, transform.eulerAngles.z);
                }
                if (Mathf.Abs(deltaRotation.z) >= maxDeltaRotation.z)
                {
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, startRotation.z + maxDeltaRotation.z);
                }
            }
            else
            {
                transform.LookAt(2 * transform.position - Camera.main.transform.position);
            }*/
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
        textMesh.text = text;
    }
}
