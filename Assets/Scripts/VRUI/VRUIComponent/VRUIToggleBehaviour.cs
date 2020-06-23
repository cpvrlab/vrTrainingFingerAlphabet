using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//TODO: Improve VRUIToggle. Maybe dont use VRUIButton as template.
[RequireComponent(typeof(Rigidbody))]
public class VRUIToggleBehaviour : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField]
    [Tooltip("The maximum distance units the physical toggle object can be moved when it is beeing pressed.\nRepresented by the magenta colored preview")]
    private float maxPushDistance;
    [SerializeField]
    [Tooltip("0 = activation on touch\n1 = activation on Max Push distance.\nRepresented by the yellow colored preview.")]
    [Range(0.05f, 1)]
    private float minPushToActivate = 0.4f;
    [SerializeField]
    [Tooltip("Distance where the toggle gets stuck after activating it.\n0 = Same position as the startPosition.\n1 = Position at the minimal push distance.\nRepresented by the green preview")]
    [Range(0.05f, 0.95f)]
    private float stuckDistance = 0.4f;
    [SerializeField]
    [Tooltip("0 means the toggle never returns to its position and 1 means it returns instantly.")]
    [Range(0.00001f, 1)]
    private float returnSpeed = 0.1f;
    [SerializeField]
    [Tooltip("Amount of seconds the script waits before it returns the toggle to the starting position, when the toggle isn't touched.")]
    private float returnDelay = 0.3f;
    [SerializeField]
    [Tooltip("How fast the toggle adjusts its position on the way down when beeing pressed. 0 means it never reaches the new position, 1 it reaches it almost instantly.")]
    [Range(0.00001f, 1)]
    private float stiffness = 0.2f;
    [SerializeField]
    [Tooltip("If this is true, it is assumed that the GestureController component is on the hand/object that can touch this toggle.\n" +
             "If this is false, any trigger collider can interact with this toggle.")]
    private bool useGestureController = true;
    [SerializeField]
    [Tooltip("Choose here which gestures can activate this toggle.")]
    private VRUIGesture[] allowedGestures;
    private Material baseMaterial;
    [Header("Material")]
    [SerializeField]
    [Tooltip("The material of the toggle after it was pressed.")]
    private Material activeMaterial;
    [Header("UnityEvents")]
    //Events that fire when the toggle gets activated/changes material.
    [SerializeField]
    private UnityEvent onVRUIToggleDown = null;
    //Events that fire every frame the toggle is active.
    [SerializeField]
    private UnityEvent onVRUIToggleStuck = null;
    //Events that fire when the toggle deactivates.
    [SerializeField]
    private UnityEvent onVRUIToggleUp = null;
    private VRUIVibration vibrationBehaviour;
    [Header("State")]
    [SerializeField]
    private bool getVRUIToggleDown;
    [SerializeField]
    private bool getVRUIToggle;
    [SerializeField]
    private bool getVRUIToggleUp;
    [SerializeField]
    private bool toggleIsStuck;
    [Header("Assigned Objects")]
    [SerializeField]
    private GameObject physicalToggle;


    //Used to change the material of the toggle
    private MeshRenderer meshRenderer;
    //Variables for position and state calculations of the toggle
    private Vector3 startPosition;
    private Vector3 currentPosition;
    private Vector3 deltaPosition;
    //Variables that store the position and other values of the hand/finger/object that touches the toggle
    private Transform touchingObjectTransform;
    private bool toggleIsTouched;
    private Vector3 startTouchPosition;
    private Vector3 currentTouchPosition;
    private Vector3 deltaTouchPosition;
    private VRUIGestureController gestureController;
    private bool correctGesture = false;
    //Time value that is used to delay the positional return of the toggle to its start or stuck position
    private float waitTime;

    // Start is called before the first frame update
    void Start()
    {
        if (allowedGestures.Length == 0)
        {
            allowedGestures = new VRUIGesture[1];
            allowedGestures[0] = VRUIGesture.IndexPointing;
        }
        //Get componentss
        BaseMaterial = PhysicalToggle.GetComponent<Renderer>().material;
        VibrationBehaviour = GetComponent<VRUIVibration>();
        meshRenderer = physicalToggle.GetComponent<MeshRenderer>();
        //The rigidbody should be kinematic and unaffected by gravity.
        Rigidbody rBody = GetComponent<Rigidbody>();
        rBody.isKinematic = true;
        rBody.useGravity = false;
        //Set default values of non inspector variables
        startPosition = currentPosition = physicalToggle.transform.localPosition;
        deltaPosition = Vector3.zero;
        getVRUIToggleDown = false;
        getVRUIToggleUp = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePositionValues();
        UpdateToggleStates();
        FireToggleEvents();
    }

    private void UpdatePositionValues()
    {
        currentPosition = physicalToggle.transform.localPosition;
        deltaPosition = currentPosition - startPosition;
    }

    private void UpdateToggleStates()
    {
        //If the maxPushDistance is 0 the toggle behaves like a 2D UI toggle (activation on touch).
        if (maxPushDistance <= 0)
        {
            getVRUIToggle = toggleIsTouched;
        }
        else
        {
            //If we reach the minimal push distance for the activation, getVRUItoggle is true
            getVRUIToggle = Mathf.Abs(deltaPosition.y) >= (MaxPushDistance * MinPushToActivate);
        }
        //toggleDown should only be true at that moment where the toggle activates
        GetVRUIToggleDown = getVRUIToggle;
        //toggleUp should only be true at that moment where the toggle deactivates
        GetVRUIToggleUp = !getVRUIToggle;
    }

    //TODO: Check if toggleup/down can be invoked here instead of the seperate methods
    private void FireToggleEvents()
    {
        if (ToggleIsStuck)
        {
            meshRenderer.material = activeMaterial;
            onVRUIToggleStuck.Invoke();
        }
        else
        {
            meshRenderer.material = BaseMaterial;
        }
    }

    void FixedUpdate()
    {
        //Move toggle according to the position of the finger/hand that touches it, if our maxPushDistance is bigger than 0.
        if (toggleIsTouched && maxPushDistance > 0)
        {
            currentTouchPosition = touchingObjectTransform.position;
            deltaTouchPosition = currentTouchPosition - startTouchPosition;
            float maxDeltaFinger = Mathf.Max(Mathf.Abs(deltaTouchPosition.x), Mathf.Abs(deltaTouchPosition.y), Mathf.Abs(deltaTouchPosition.z)); //TODO: Vector algebra for this?

            if (Mathf.Abs(deltaTouchPosition.y) < maxPushDistance && maxDeltaFinger > 0.0f)
            {
                Vector3 targetPos = new Vector3(physicalToggle.transform.localPosition.x, physicalToggle.transform.localPosition.y - maxDeltaFinger, physicalToggle.transform.localPosition.z);
                //The toggle should not be able to move farther away than the maxPushDistance //TODO: Better calculation. maybe the toggles local position is not (0, 0, 0)
                if (targetPos.y < -maxPushDistance)
                {
                    physicalToggle.transform.localPosition = Vector3.Lerp(physicalToggle.transform.localPosition, new Vector3(physicalToggle.transform.localPosition.x, -maxPushDistance, physicalToggle.transform.localPosition.z), stiffness * 2);
                }
                else
                {
                    physicalToggle.transform.localPosition = Vector3.Lerp(physicalToggle.transform.localPosition, targetPos, stiffness);
                }
            }
            waitTime = 0.0f;
        }
        //If the toggle is not touched and is not stuck slowly return it to its startPosition
        else if (currentPosition != startPosition && !toggleIsStuck)
        {
            waitTime += Time.deltaTime;
            //Only move the toggle to its start position after the chosen delay
            if (waitTime >= returnDelay)
            {
                physicalToggle.transform.localPosition = Vector3.Lerp(physicalToggle.transform.localPosition, startPosition, returnSpeed);
            }
        }
        else if(currentPosition != startPosition && toggleIsStuck)
        {
            waitTime += Time.deltaTime;
            //Only move the toggle to its start position after the chosen delay
            if (waitTime >= returnDelay)
            {
                Vector3 stuckPosition = new Vector3(physicalToggle.transform.localPosition.x, -MaxPushDistance * MinPushToActivate * StuckDistance, physicalToggle.transform.localPosition.z);
                physicalToggle.transform.localPosition = Vector3.Lerp(physicalToggle.transform.localPosition, stuckPosition, returnSpeed);
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
        toggleIsTouched = true;
        //Vibration
        if (VibrationBehaviour != null)
        {
            if (currentPosition == startPosition || waitTime > 0 && currentPosition.y < maxPushDistance)
            {
                VibrationBehaviour.Vibrate();
            }
        }
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

    private void OnTriggerExit(Collider other)
    {
        if (useGestureController)
        {
            gestureController = other.attachedRigidbody.gameObject.GetComponent<VRUIGestureController>();
            if (!gestureController)
                return;
        }
        touchingObjectTransform = null;

        toggleIsTouched = false;
    }

    //TODO: besserer name, wie heisst dieses Konzept?
    private bool GetVRUIToggleDown
    {
        get { return getVRUIToggleDown; }
        set
        {
            if (value == getVRUIToggleDown)
                return;

            getVRUIToggleDown = value;
            if (getVRUIToggleDown)
            {
                onVRUIToggleDown.Invoke();
                toggleIsStuck = !toggleIsStuck;
            }
        }
    }

    private bool GetVRUIToggleUp
    {
        get { return getVRUIToggleUp; }
        set
        {
            if (value == getVRUIToggleUp)
                return;

            getVRUIToggleUp = value;
            if (getVRUIToggleUp)
            {
                onVRUIToggleUp.Invoke();
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

    public float StuckDistance
    {
        get { return stuckDistance; }
        set
        {
            if (value <= 0)
                stuckDistance = 0;
            else if (value >= 1)
                stuckDistance = 1;
            else
                stuckDistance = value;
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

    public GameObject PhysicalToggle
    {
        get { return physicalToggle; }
        set { physicalToggle = value; }
    }

    public VRUIGestureController LastRegisteredGestureController
    {
        get { return gestureController; }
    }

    public bool ToggleIsStuck
    {
        get { return toggleIsStuck; }
        set { toggleIsStuck = value; }
    }
}
