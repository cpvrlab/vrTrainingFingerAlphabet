/********************************************************************************//*
Created as part of a Bsc in Computer Science for the BFH Biel
Created by:   Steven Henz
Date:         26.05.20
Email:        steven.henz93@gmail.com
************************************************************************************/
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simulates a 3D button that needs to be physically touched to use it.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[ExecuteInEditMode]
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
    //Events that fire when the button gets activated/changes material.
    [System.Serializable]
    public class VRUIOnButtonDownEvent : UnityEvent<string> { }
    [Header("UnityEvents")]
    [SerializeField]
    public VRUIOnButtonDownEvent m_onVRUIButtonDown;
    public VRUIOnButtonDownEvent onVRUIButtonDown
    {
        get { return m_onVRUIButtonDown; }
        set { m_onVRUIButtonDown = value; }
    }
    //Events that fire every frame the button is active.
    [System.Serializable]
    public class VRUIOnButtonEvent : UnityEvent<string> { }
    [SerializeField]
    private VRUIOnButtonEvent m_onVRUIButton;
    public VRUIOnButtonEvent onVRUIButton
    {
        get { return m_onVRUIButton; }
        set { m_onVRUIButton = value; }
    }
    //Events that fire when the Button deactivates.
    [System.Serializable]
    public class VRUIOnButtonUpEvent : UnityEvent<string> { }
    [SerializeField]
    private VRUIOnButtonUpEvent m_onVRUIButtonUp;
    public VRUIOnButtonUpEvent onVRUIButtonUp
    {
        get { return m_onVRUIButtonUp; }
        set { m_onVRUIButtonUp = value; }
    }

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

    private Vector3 lastScale;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying)
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
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
            if (!BaseMaterial)
                BaseMaterial = PhysicalButton.GetComponent<Renderer>().material;
        lastScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = lastScale;
        if (Application.isPlaying)
        {
            UpdatePositionValues();
            UpdateButtonStates();
            FireButtonEvents();
        }
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
        }
        //ButtonDown should only be true at that moment where the button activates
        GetVRUIButtonDown = getVRUIButton;
        //ButtonUp should only be true at that moment where the button deactivates
        GetVRUIButtonUp = !getVRUIButton;
    }

    private void FireButtonEvents()
    {
        if (getVRUIButton)
        {
            meshRenderer.material = activeMaterial;
            m_onVRUIButton.Invoke(name);
        }
        else
        {
            meshRenderer.material = BaseMaterial;
        }
    }

    void FixedUpdate()
    {
        //if the object that touches the button gets deactivated, reset the touching object references
        if (touchingObjectTransform)
        {
            if (!touchingObjectTransform.gameObject.activeInHierarchy)
            {
                touchingObjectTransform = null;

                startTouchPosition = Vector3.zero;
                currentTouchPosition = Vector3.zero;
                buttonIsTouched = false;
            }
        }
        //Move button according to the position of the finger/hand that touches it, if our maxPushDistance is bigger than 0.
        if (buttonIsTouched && MaxPushDistance > 0)
        {
            currentTouchPosition = touchingObjectTransform.position;
            deltaTouchPosition = currentTouchPosition - startTouchPosition;
            float maxDeltaFinger = Mathf.Max(Mathf.Abs(deltaTouchPosition.x), Mathf.Abs(deltaTouchPosition.y), Mathf.Abs(deltaTouchPosition.z));

            if (Mathf.Abs(deltaTouchPosition.y) < MaxPushDistance && maxDeltaFinger > 0.0f)
            {
                Vector3 targetPos = new Vector3(physicalButton.transform.localPosition.x, physicalButton.transform.localPosition.y - maxDeltaFinger, physicalButton.transform.localPosition.z);
                //The button should not be able to move farther away than the maxPushDistance 
                //TODO: Better calculation. maybe the buttons local position is not (0, 0, 0)
                if (targetPos.y < -MaxPushDistance)
                {
                    physicalButton.transform.localPosition = Vector3.Lerp(physicalButton.transform.localPosition, new Vector3(physicalButton.transform.localPosition.x, -maxPushDistance, physicalButton.transform.localPosition.z), stiffness * 2);
                }
                else
                {
                    physicalButton.transform.localPosition = Vector3.Lerp(physicalButton.transform.localPosition, targetPos, stiffness);
                }
            }
            waitTime = 0.0f;
        }
        //If the button is not touched slowly return it to its startPosition
        else if (currentPosition != startPosition)
        {
            waitTime += Time.fixedDeltaTime;
            //Only move the button to its start position after the chosen delay
            if (waitTime >= returnDelay)
            {
                physicalButton.transform.localPosition = Vector3.Lerp(physicalButton.transform.localPosition, startPosition, returnSpeed);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (useGestureController)
        {
            if (other.attachedRigidbody)
                gestureController = other.attachedRigidbody.gameObject.GetComponent<VRUIGestureController>();
            else
                return;
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
            if (other.attachedRigidbody)
                gestureController = other.attachedRigidbody.gameObject.GetComponent<VRUIGestureController>();
            else
                return;
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
                m_onVRUIButtonDown.Invoke(name);
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
                m_onVRUIButtonUp.Invoke(name);
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
