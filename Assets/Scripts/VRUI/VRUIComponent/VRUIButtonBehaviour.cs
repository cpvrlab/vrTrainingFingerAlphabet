using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class VRUIButtonBehaviour : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField]
    [Tooltip("The maximum distance units the physical button object can be moved when it is beeing pressed.\nRepresented by the magenta colored preview")]
    private float maxPushDistance;
    [SerializeField]
    [Tooltip("0 = activation on touch\n1 = activation on Max Push distance.\nRepresented by the yellow colored preview.")]
    [Range(0, 1)]
    private float minPushToActivate = 0.4f;
    [SerializeField]
    [Tooltip("0 means the button never returns to its position and 1 means it returns instantly.")]
    [Range(0.00001f, 1)]
    private float returnSpeed = 0.1f;
    [SerializeField]
    [Tooltip("Amount of seconds the script waits before it returns the button to the starting position, when the button isn't touched.")]
    private float returnDelay = 0.3f;
    [SerializeField]
    [Tooltip("How fast the button adjusts its position on the way down when beeing pressed. 0 means it never reaches the new position, 1 it reaches it almost instantly.")]
    [Range(0.00001f, 1)]
    private float stiffness = 0.2f;
    [SerializeField]
    [Tooltip("If this is true, it is assumed that the GestureController component is on the hand/object that can touch this button.\n" +
             "If this is false, any trigger collider can interact with this button.")]
    private bool useGestureController = true;
    [SerializeField]
    [Tooltip("Choose here which gesture can activate this button.")]
    private VRUIGesture[] allowedGestures;
    private Material baseMaterial;
    [Header("Material")]
    [SerializeField]
    [Tooltip("The material of the button after it was pressed.")]
    private Material activeMaterial;
    [Header("UnityEvents")]
    //Events that fire when the button gets activated/changes material.
    [SerializeField]
    private UnityEvent onVRUIButtonDown = null;
    //Events that fire every frame the button is active.
    [SerializeField]
    private UnityEvent onVRUIButton = null;
    //Events that fire when the Button deactivates.
    [SerializeField]
    private UnityEvent onVRUIButtonUp = null;
    private VRUIVibration vibrationBehaviour;
    [Header("State")]
    [SerializeField]
    private bool getVRUIButtonDown;
    [SerializeField]
    private bool getVRUIButton;
    [SerializeField]
    private bool getVRUIButtonUp;
    [Header("Assigned Objects")]
    [SerializeField]
    private GameObject physicalButton;


    //Used to change the material of the button
    private MeshRenderer meshRenderer;
    //Variables for position and state calculations of the button
    private Vector3 startPosition;
    private Vector3 currentPosition;
    private Vector3 deltaPosition;
    //Variables that store the position and other values of the hand/finger/object that touches the button
    private Transform touchingObjectTransform;
    private bool buttonIsTouched;
    private Vector3 startTouchPosition;
    private Vector3 currentTouchPosition;
    private Vector3 deltaTouchPosition;
    private VRUIGestureController gestureController;
    //Time value that is used to delay the positional return of the button to its start position
    private float waitTime;

    private bool correctGesture = false;
    // Start is called before the first frame update
    void Start()
    {
        if (allowedGestures.Length == 0)
        {
            allowedGestures = new VRUIGesture[1];
            allowedGestures[0] = VRUIGesture.IndexPointing;
        }
        //Get componentss
        BaseMaterial = PhysicalButton.GetComponent<Renderer>().material;
        VibrationBehaviour = GetComponent<VRUIVibration>();
        meshRenderer = physicalButton.GetComponent<MeshRenderer>();
        //The rigidbody should be kinematic and unaffected by gravity.
        Rigidbody rBody = GetComponent<Rigidbody>();
        rBody.isKinematic = true;
        rBody.useGravity = false;
        //Set default values of non inspector variables
        startPosition = currentPosition = physicalButton.transform.localPosition;
        deltaPosition = Vector3.zero;
        getVRUIButtonDown = false;
        getVRUIButtonUp = true;
        gestureController = null;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePositionValues();
        UpdateButtonStates();
        FireButtonEvents();
    }

    private void UpdatePositionValues()
    {
        currentPosition = physicalButton.transform.localPosition;
        deltaPosition = currentPosition - startPosition;
    }

    private void UpdateButtonStates()
    {
        //If the maxPushDistance is 0 the button behaves like a 2D UI button (activation on touch).
        if (maxPushDistance <= 0)
        {
            getVRUIButton = buttonIsTouched;
        }
        else
        {
            //If we reach the minimal push distance for the activation, getVRUIButton is true
            getVRUIButton = Mathf.Abs(deltaPosition.y) >= (MaxPushDistance * minPushToActivate);
            //Debug.Log("Button Active: " + getVRUIButton + "; Delta: " + deltaPosition.y);
        }
        //Debug.Log("Button DeltaY=" + Mathf.Abs(deltaPosition.y));
        //ButtonDown should only be true at that moment where the button activates
        GetVRUIButtonDown = getVRUIButton;
        //ButtonUp should only be true at that moment where the button deactivates
        GetVRUIButtonUp = !getVRUIButton;
    }

    //TODO: Check if buttonup/down can be invoked here instead of the seperate methods
    private void FireButtonEvents()
    {
        if (getVRUIButton)
        {
            meshRenderer.material = activeMaterial;
            onVRUIButton.Invoke();
        }
        else
        {
            meshRenderer.material = BaseMaterial;
        }
    }

    void FixedUpdate()
    {
        //Move button according to the position of the finger/hand that touches it, if our maxPushDistance is bigger than 0.
        if (buttonIsTouched && maxPushDistance > 0)
        {
            currentTouchPosition = touchingObjectTransform.position;
            deltaTouchPosition = currentTouchPosition - startTouchPosition;
            float maxDeltaFinger = Mathf.Max(Mathf.Abs(deltaTouchPosition.x), Mathf.Abs(deltaTouchPosition.y), Mathf.Abs(deltaTouchPosition.z)); //TODO: Vector algebra for this?

            if (Mathf.Abs(deltaTouchPosition.y) < maxPushDistance && maxDeltaFinger > 0.0f)
            {
                //Vector3 targetPos = new Vector3(physicalButton.transform.localPosition.x, physicalButton.transform.localPosition.y - maxDeltaFinger / physicalButton.transform.lossyScale.y, physicalButton.transform.localPosition.z);
                Vector3 targetPos = new Vector3(physicalButton.transform.localPosition.x, physicalButton.transform.localPosition.y - maxDeltaFinger, physicalButton.transform.localPosition.z);
                //The button should not be able to move farther away than the maxPushDistance 
                //TODO: Better calculation. maybe the buttons local position is not (0, 0, 0)
                if (targetPos.y < -maxPushDistance)
                {
                    //physicalButton.transform.localPosition = new Vector3(physicalButton.transform.localPosition.x, -maxPushDistance, physicalButton.transform.localPosition.z);
                    physicalButton.transform.localPosition = Vector3.Lerp(physicalButton.transform.localPosition, new Vector3(physicalButton.transform.localPosition.x, -maxPushDistance, physicalButton.transform.localPosition.z), stiffness * 2);
                }
                else
                {
                    //physicalButton.transform.localPosition = targetPos;
                    physicalButton.transform.localPosition = Vector3.Lerp(physicalButton.transform.localPosition, targetPos, stiffness);
                }
            }
            waitTime = 0.0f;
        }
        //If the button is not touched slowly return it to its startPosition
        else if (currentPosition != startPosition)
        {
            waitTime += Time.deltaTime;
            //Only move the button to its start position after the chosen delay
            if (waitTime >= returnDelay)
            {
                physicalButton.transform.localPosition = Vector3.Lerp(physicalButton.transform.localPosition, startPosition, returnSpeed);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        gestureController = other.attachedRigidbody.gameObject.GetComponent<VRUIGestureController>();
        if (useGestureController)
        {
            if (!gestureController)
                return;
            if (!CorrectGestureUsed())
                return;
        }
        touchingObjectTransform = other.transform;

        startTouchPosition = touchingObjectTransform.position;
        currentTouchPosition = touchingObjectTransform.position;
        buttonIsTouched = true;
        if (VibrationBehaviour != null)
        {
            if ((currentPosition == startPosition || waitTime > 0) && currentPosition.y < maxPushDistance)
            {
                VibrationBehaviour.Vibrate();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (useGestureController)
        {
            gestureController = other.attachedRigidbody.gameObject.GetComponent<VRUIGestureController>();
            if (!gestureController)
                return;
        }
        touchingObjectTransform = null;

        buttonIsTouched = false;
    }

    private bool CorrectGestureUsed()
    {
        correctGesture = false;
        foreach (VRUIGesture gesture in allowedGestures)
        {
            if (!correctGesture && gestureController.VRUIGesture == gesture)
            {
                correctGesture = true;
            }
        }
        return correctGesture;
    }

    //TODO: besserer name, wie heisst dieses Konzept?
    private bool GetVRUIButtonDown
    {
        get { return getVRUIButtonDown; }
        set
        {
            if (value == getVRUIButtonDown)
                return;

            getVRUIButtonDown = value;
            if (getVRUIButtonDown)
            {
                onVRUIButtonDown.Invoke();
            }
        }
    }

    private bool GetVRUIButtonUp
    {
        get { return getVRUIButtonUp; }
        set
        {
            if (value == getVRUIButtonUp)
                return;

            getVRUIButtonUp = value;
            if (getVRUIButtonUp)
            {
                onVRUIButtonUp.Invoke();
            }
        }
    }

    public float MaxPushDistance
    {
        get { return maxPushDistance; }
        set { maxPushDistance = value; }
    }

    public float MinPushToActivate
    {
        get { return minPushToActivate; }
        set
        {
            if (value <= 0)
                minPushToActivate = 0;
            else if (value >= 1)
                minPushToActivate = 1;
            else
                minPushToActivate = value;
        }
    }

    public float ReturnSpeed
    {
        get { return returnSpeed; }
        set
        {
            if (value <= 0.00001f)
                returnSpeed = 0.00001f;
            else if (value >= 1)
                returnSpeed = 1;
            else
                returnSpeed = value;
        }
    }

    public float ReturnDelay
    {
        get { return returnDelay; }
        set { returnDelay = value; }
    }

    public Material BaseMaterial
    {
        get { return baseMaterial; }
        set { baseMaterial = value; }
    }

    public Material ActiveMaterial
    {
        get { return activeMaterial; }
        set { activeMaterial = value; }
    }

    public VRUIVibration VibrationBehaviour
    {
        get { return vibrationBehaviour; }
        set { vibrationBehaviour = value; }
    }

    public GameObject PhysicalButton
    {
        get { return physicalButton; }
        set { physicalButton = value; }
    }

    public VRUIGestureController LastRegisteredGestureController
    {
        get { return gestureController; }
    }
}
