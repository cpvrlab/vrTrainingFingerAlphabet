using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
[ExecuteInEditMode]
public class VRUIPositioner : MonoBehaviour
{
    private const string NO_ANCHOR_DEFINED_MESSAGE = "No anchor defined! This should not happen! MiddleCenter anchor set as anchor!";

    [SerializeField]
    [HideInInspector]
    private Vector3 relativePosition;
    [SerializeField]
    [HideInInspector]
    private Quaternion relativeRotation;
    [SerializeField]
    [HideInInspector]
    private Vector3 relativeEulerRotation;
    [SerializeField]
    [HideInInspector]
    private VRUIAnchor anchor;
    [SerializeField]
    [HideInInspector]
    private Vector3 parentSize;
    [SerializeField]
    [HideInInspector]
    private Vector3 anchorPosition;
    [SerializeField]
    [HideInInspector]
    private VRUIAnchor oldAnchor;
    [SerializeField]
    [HideInInspector]
    private bool vruiPositionerChangedTransform;
    [SerializeField]
    [HideInInspector]
    private Transform parent;
    [SerializeField]
    [HideInInspector]
    private Transform lastParent;
    [SerializeField]
    [HideInInspector]
    private bool haveToUpdateRelativeValues;
    [SerializeField]
    [HideInInspector]
    private Quaternion lastParentRotationValue;

    //Null check references
    private VRUIPanelBehaviour parentPanel;
    private MeshFilter parentMeshFilter;
    private VRUIScrollPanelBehaviour scrollPanelBehaviour;

    private void Reset()
    {
        parent = transform.parent ? transform.parent : null;
        lastParent = parent;

        parentPanel = null;
        parentMeshFilter = null;

        if (parent)
        {
            parentPanel = transform.parent.gameObject.GetComponent<VRUIPanelBehaviour>();
            parentMeshFilter = transform.parent.gameObject.GetComponent<MeshFilter>();
            scrollPanelBehaviour = parent.GetComponent<VRUIScrollPanelBehaviour>();
            lastParentRotationValue = transform.parent.rotation;
        }
        
        RelativePosition = Vector3.zero;
        RelativeRotation = Quaternion.identity;
        Anchor = VRUIAnchor.MiddleCenter;
        anchorPosition = Vector3.zero;
        oldAnchor = VRUIAnchor.MiddleCenter;
        vruiPositionerChangedTransform = false;
            
        transform.rotation = Quaternion.identity;

        if (HasParentVRUIPositioner())
            SetupAnchor();
        SetPositionRelativeToAnchor();
        SetRotationRelativeToParent();
    }

    private void Update()
    {
        //If the parent has a VRUIScrollPanelBehaviour, this script shouldnt control the objects position
        if (!Application.isPlaying && !scrollPanelBehaviour) {
            //Debug.Log("vruiPositionerChangedTransform=" + vruiPositionerChangedTransform + ";transform.hasChanged=" + transform.hasChanged + ";name=" + name);
            if (VRUIPositionerChangedTransform)
            {
                HaveToUpdateRelativeValues = false;
            } else if (transform.hasChanged)
            {
                HaveToUpdateRelativeValues = true;
            }
            parent = transform.parent ? transform.parent : null;
            //If the parent changed recalculate the anchorposition and reposition object.
            if (parent && lastParent != parent)
            {
                SetupAnchor();
                SetPositionRelativeToAnchor();
                //SetRotationRelativeToParent();
            }
            //If rotation changed recalculate the anchorposition
            if (parent && lastParentRotationValue != transform.parent.rotation)
            {
                SetupAnchor();
            }
            lastParent = parent;
            lastParentRotationValue = transform.rotation;
            transform.hasChanged = false;
            VRUIPositionerChangedTransform = false;
        }
    }

    public void SetupAnchor()
    {
        if (!HasParentVRUIPositioner())
            return;
        //We save the old anchor position so we can use it to calculate the correct PositionOnPanel after the anchor change.
        Vector3 oldAnchorPosition = AnchorPosition;
        //Get bounding box of the parent to calculate the anchorpoints if no bounds were defined
        if (transform.parent.GetComponent<VRUIPanelBehaviour>())
        {
            parentSize = transform.parent.GetComponent<VRUIPanelBehaviour>().PanelBounds.extents;
            //Debug.Log("Panel: " + boundsOfParent);
        }
        else if (transform.parent.GetComponent<MeshFilter>())
        {
            //Mesh.bounds is called because it has its bounding box aligned to local space
            parentSize = transform.parent.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
            //Debug.Log("Meshfilter: " + boundsOfParent);
        }
        else
        {
            //If we find no reference for the bounding box we just create a default box with size one.
            parentSize = new Bounds(transform.parent.position, Vector3.one).extents;
            //Debug.Log("No bounds found");
        }
        //Check which anchorpoint was chosen and calculate the position of said anchor.
        switch (anchor)
        {
            case VRUIAnchor.TopLeft:
                anchorPosition = transform.parent.position + -(transform.parent.right * parentSize.x) + (transform.parent.up * parentSize.y);
                break;
            case VRUIAnchor.TopCenter:
                anchorPosition = transform.parent.position + (transform.parent.up * parentSize.y);
                break;
            case VRUIAnchor.TopRight:
                anchorPosition = transform.parent.position + (transform.parent.right * parentSize.x) + (transform.parent.up * parentSize.y);
                break;
            case VRUIAnchor.MiddleLeft:
                anchorPosition = transform.parent.position + -(transform.parent.right * parentSize.x);
                break;
            case VRUIAnchor.MiddleCenter:
                anchorPosition = transform.parent.position;
                break;
            case VRUIAnchor.MiddleRight:
                anchorPosition = transform.parent.position + (transform.parent.right * parentSize.x);
                break;
            case VRUIAnchor.BottomLeft:
                anchorPosition = transform.parent.position + -(transform.parent.right * parentSize.x) + -(transform.parent.up * parentSize.y);
                break;
            case VRUIAnchor.BottomCenter:
                anchorPosition = transform.parent.position + -(transform.parent.up * parentSize.y);
                break;
            case VRUIAnchor.BottomRight:
                anchorPosition = transform.parent.position + (transform.parent.right * parentSize.x) + -(transform.parent.up * parentSize.y);
                break;
            default:
                anchor = VRUIAnchor.MiddleCenter;
                anchorPosition = transform.parent.position;
                Debug.LogError(NO_ANCHOR_DEFINED_MESSAGE);
                break;
        }
        if (oldAnchor != Anchor)
        {
            RelativePosition = oldAnchorPosition - AnchorPosition + RelativePosition;
        }
        oldAnchor = Anchor;
        transform.hasChanged = false;
    }

    public void SetPositionRelativeToAnchor()
    {
        transform.position = (transform.right * RelativePosition.x) + (transform.up * RelativePosition.y) + (transform.forward * RelativePosition.z) + AnchorPosition;
        VRUIPositionerChangedTransform = true;
        //transform.hasChanged = false;
    }

    public void SetRotationRelativeToParent()
    {
        if (transform.parent)
            transform.rotation = transform.parent.rotation * Quaternion.Euler(RelativeEulerRotation);
        else
            transform.rotation = Quaternion.Euler(RelativeEulerRotation);
        VRUIPositionerChangedTransform = true;
        //transform.hasChanged = false;
    }

    public bool HasParentVRUIPositioner()
    {
        //If there is no parent at all we do not need to check for a VRUITransformOld2 in the parent.
        if (transform.parent)
        {
            VRUIPositioner parent = transform.parent.gameObject.GetComponent<VRUIPositioner>();
            return parent;
        }
        return false;
    }

    public bool VRUIPositionerChangedTransform
    {
        get { return vruiPositionerChangedTransform; }
        set
        {
            vruiPositionerChangedTransform = value;
            if (HasParentVRUIPositioner())
            {
                transform.parent.GetComponent<VRUIPositioner>().VRUIPositionerChangedTransform = value;
            }
            //Debug.Log("vruiPositionerChangedTransform=" + vruiPositionerChangedTransform + ";name=" + name);
        }
    }

    public Vector3 RelativePosition
    {
        get { return relativePosition; }
        set { relativePosition = value; }
    }

    public Quaternion RelativeRotation
    {
        get { return relativeRotation; }
        set { relativeRotation = value; }
    }

    public Vector3 RelativeEulerRotation
    {
        get { return relativeEulerRotation; }
        set { relativeEulerRotation = value; }
    }

    public Vector3 AnchorPosition
    {
        get { return anchorPosition; }
        set { anchorPosition = value; }
    }

    public VRUIAnchor Anchor
    {
        get { return anchor; }
        set { anchor = value; }
    }

    public bool HaveToUpdateRelativeValues
    {
        get { return haveToUpdateRelativeValues; }
        set { haveToUpdateRelativeValues = value; }
    }
}
