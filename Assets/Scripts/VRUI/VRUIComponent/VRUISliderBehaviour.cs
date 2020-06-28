/********************************************************************************//*
Created as part of a Bsc in Computer Science for the BFH Biel
Created by:   Steven Henz
Date:         26.05.20
Email:        steven.henz93@gmail.com
************************************************************************************/
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simulates a 3D slider that needs to be physically touched to use it.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[ExecuteInEditMode]
public class VRUISliderBehaviour : MonoBehaviour
{
    [Header("Value Settings")]
    [SerializeField]
    [Tooltip("The minimal value that can be set with this slider.")]
    private float minValue = 0f;
    [SerializeField]
    [Tooltip("Tha maximum value that can be set with this slider.")]
    private float maxValue = 100f;
    [SerializeField]
    [Tooltip("The value where the knob starts. Can not be higher than the maxValue or lower than the minValue.")]
    private float startValue;
    [SerializeField]
    [Tooltip("The currently set value.")]
    private float currentValue;
    [Header("Path Settings")]
    [SerializeField]
    [Tooltip("The length of the drawn path.")]
    private float lengthOfPath = 1f;
    [SerializeField]
    [Tooltip("The width of the drawn path.")]
    private float widthOfPath = 0.05f;
    [Header("Knob Settings")]
    [SerializeField]
    [Tooltip("The z-position of the knob, relative to the path.")]
    float zPositionKnob = -0.025f;
    [Header("Interaction Settings")]
    [SerializeField]
    [Tooltip("If the object/hand that touches the knob is farther away than this, the knob wont be moved.")]
    private float maxAllowedDistanceToMove = 0.2f;
    [SerializeField]
    [Tooltip("If this is true, it is assumed that the GestureController component is on the hand/object that can touch the knob.\n" +
             "If this is false, any trigger collider can interact with the knob.")]
    private bool useGestureController = true;
    [SerializeField]
    [Tooltip("Choose here which gesture can activate this button.")]
    private VRUIGesture[] allowedGestures;
    [SerializeField]
    [Tooltip("The amount of time the slider can not be interacted with, after the gesture of the touching hand changed.")]
    private float lockoutTime = 0.5f;
    [Header("Material")]
    [SerializeField]
    [Tooltip("The material used to render the path.")]
    private Material pathMaterial;
    [SerializeField]
    [Tooltip("The color of the knob when touched.")]
    private Material knobTouchedMaterial;
    private Material baseMaterial;
    private VRUIVibration vibrationBehaviour;
    [Header("Assigned Objects")]
    [SerializeField]
    private GameObject path;
    [SerializeField]
    private GameObject physicalKnob;
    [System.Serializable]
    public class VRUISliderEvent : UnityEvent<float> { }
    [Header("Unity Events")]
    [SerializeField]
    private VRUISliderEvent m_OnValueChanged = new VRUISliderEvent();
    public VRUISliderEvent onValueChanged
    {
        get { return m_OnValueChanged; }
        set { m_OnValueChanged = value; }
    }
    private float oldValue;

    //Variables that store the position and other such values of the path.
    private Vector3 startOfPath;
    private Vector3 endOfPath;
    //Variables that store values of the touching finger
    private Transform touchingObjectTransform;
    private bool knobIsTouched;
    private Vector3 startTouchKnobPosition;
    private Vector3 currentTouchPosition;
    private Vector3 deltaTouchPosition;

    private VRUIGestureController gestureController;
    private VRUIGestureController gestureControllerToMonitor;
    private VRUIGesture lastGesture = VRUIGesture.None;
    private bool correctGesture = false;

    private bool locked = false;
    private float currentLockoutTime = 0;

    private MeshRenderer knobRenderer;

    private Vector3 lastScale;

    // Start is called before the first frame update
    void Start()
    {
        if (allowedGestures.Length == 0)
        {
            allowedGestures = new VRUIGesture[1];
            allowedGestures[0] = VRUIGesture.IndexPointing;
        }
        if (path)
        {
            //Create the path visuals
            CreatePath();
        }
        if (physicalKnob)
        {
            //Place handle at the start position
            SetupSliderKnobPosition();
        }
        oldValue = currentValue;
        gestureController = null;
        gestureControllerToMonitor = null;
        if (Application.isPlaying)
        {
            knobRenderer = PhysicalKnob.GetComponent<MeshRenderer>();
            baseMaterial = knobRenderer.material;
        }
    }

    private void OnEnable()
    {
        lastScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = lastScale;
        startOfPath = new Vector3(0.0f, -lengthOfPath / 2, 0.0f);
        endOfPath = new Vector3(0.0f, lengthOfPath / 2, 0.0f);
        if (path && physicalKnob)
            UpdateCurrentValue();
        if (locked)
        {
            currentLockoutTime += Time.deltaTime;
            if (currentLockoutTime >= LockoutTime)
            {
                locked = false;
                currentLockoutTime = 0;
            }
        }
        if (Application.isPlaying)
        {
            if (gestureControllerToMonitor)
            {
                if (!CorrectGestureUsed())
                {
                    InteractionFinished();
                }
                if (GestureChanged())
                {
                    InteractionFinished();
                    locked = true;
                }
            }
            if (touchingObjectTransform)
            {
                currentTouchPosition = touchingObjectTransform.position;
                if (Vector3.Distance(touchingObjectTransform.position, PhysicalKnob.transform.position) > maxAllowedDistanceToMove)
                {
                    InteractionFinished();
                    locked = true;
                }
            }
            //When the value changed, invoke the OnValueChanged event
            if (currentValue != oldValue)
            {
                m_OnValueChanged.Invoke(CurrentValue);
            }
            oldValue = currentValue;
            if (knobIsTouched)
            {
                knobRenderer.material = knobTouchedMaterial;
            }
            else
            {
                knobRenderer.material = baseMaterial;
            }
        }
    }

    private void InteractionFinished()
    {
        knobIsTouched = false;
        gestureControllerToMonitor = null;
        touchingObjectTransform = null;
        lastGesture = VRUIGesture.None;
    }

    private void FixedUpdate()
    {
        if (touchingObjectTransform)
        {
            //if the object that touches the knob gets deactivated, reset the touching object references
            if (!touchingObjectTransform.gameObject.activeInHierarchy)
            {
                touchingObjectTransform = null;

                startTouchKnobPosition = Vector3.zero;
                currentTouchPosition = Vector3.zero;

                gestureControllerToMonitor = null;

                knobIsTouched = false;
            }
        }
        if (knobIsTouched && !locked)
        {
            currentTouchPosition = touchingObjectTransform.position;
            deltaTouchPosition = currentTouchPosition - startTouchKnobPosition;

            Vector3 targetPosition = startTouchKnobPosition + deltaTouchPosition;

            PhysicalKnob.transform.position = targetPosition;
            PhysicalKnob.transform.localPosition = new Vector3(0, PhysicalKnob.transform.localPosition.y, zPositionKnob);
            if (PhysicalKnob.transform.localPosition.y >= endOfPath.y)
            {
                PhysicalKnob.transform.localPosition = new Vector3(0f, endOfPath.y, zPositionKnob);
            }
            else if (PhysicalKnob.transform.localPosition.y <= startOfPath.y)
            {
                PhysicalKnob.transform.localPosition = new Vector3(0f, startOfPath.y, zPositionKnob);
            }
        }
    }

    public void UpdateKnobPosition()
    {
        if (MaxValue == 0)
        {
            MaxValue = 1;
        }
        float distance = Vector3.Distance(startOfPath, endOfPath);
        Vector3 targetPos = (endOfPath - startOfPath).normalized
                          * distance
                          * ((CurrentValue - minValue) / (MaxValue - minValue))
                          - (endOfPath - startOfPath).normalized * (distance / 2);
        PhysicalKnob.transform.localPosition = new Vector3(targetPos.x, targetPos.y, targetPos.z + zPositionKnob);
    }

    public void CreatePath()
    {
        startOfPath = new Vector3(0.0f, -lengthOfPath / 2, 0.0f);
        endOfPath = new Vector3(0.0f, lengthOfPath / 2, 0.0f);
        LineRenderer pathRenderer = path.GetComponent<LineRenderer>();
        pathRenderer.positionCount = 2;
        pathRenderer.SetPositions(new Vector3[] {
            startOfPath,
            endOfPath
        });
        pathRenderer.useWorldSpace = false;
        pathRenderer.startWidth = pathRenderer.endWidth = widthOfPath;
        pathRenderer.material = pathMaterial;
        pathRenderer.alignment = LineAlignment.TransformZ;
        pathRenderer.generateLightingData = true;
    }

    private void SetupSliderKnobPosition()
    {
        float distance = Vector3.Distance(startOfPath, endOfPath);
        Vector3 targetPos = (endOfPath - startOfPath).normalized
                          * distance
                          * ((StartValue - minValue) / (MaxValue - minValue))
                          - (endOfPath - startOfPath).normalized * (distance / 2);
        PhysicalKnob.transform.localPosition = new Vector3(targetPos.x, targetPos.y, targetPos.z + zPositionKnob);
    }

    private void UpdateCurrentValue()
    {
        Vector3 knobWithoutZ = new Vector3(PhysicalKnob.transform.localPosition.x, PhysicalKnob.transform.localPosition.y);
        float distanceKnobToStart = Vector3.Distance(knobWithoutZ, startOfPath);
        float distance = Vector3.Distance(endOfPath, startOfPath);

        if (distanceKnobToStart <= 0)
        {
            CurrentValue = MinValue;
        }
        else if (distanceKnobToStart >= distance)
        {
            CurrentValue = MaxValue;
        }
        else
        {
            CurrentValue = (distanceKnobToStart / distance * MaxValue) - (distanceKnobToStart / distance * MinValue) + MinValue;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == touchingObjectTransform)
            return;
        if (other.attachedRigidbody)
            gestureController = other.attachedRigidbody.gameObject.GetComponent<VRUIGestureController>();
        if (useGestureController)
        {
            if (locked)
                return;
            if (!gestureController)
                return;
            if (!CorrectGestureUsed())
                return;
        }
        touchingObjectTransform = other.transform;

        startTouchKnobPosition = PhysicalKnob.transform.position;
        currentTouchPosition = touchingObjectTransform.position;

        gestureControllerToMonitor = gestureController;

        knobIsTouched = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform == touchingObjectTransform)
            return;
        if (other.attachedRigidbody)
            gestureController = other.attachedRigidbody.gameObject.GetComponent<VRUIGestureController>();
        if (useGestureController)
        {
            if (locked)
                return;
            if (!gestureController)
                return;
            if (!CorrectGestureUsed())
                return;
        }
        touchingObjectTransform = other.transform;

        startTouchKnobPosition = PhysicalKnob.transform.position;
        currentTouchPosition = touchingObjectTransform.position;

        knobIsTouched = true;
        if (VibrationBehaviour != null)
        {
            if (gestureControllerToMonitor)
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

    private bool GestureChanged()
    {
        if (!gestureControllerToMonitor)
        {
            return true;
        }
        if (lastGesture != VRUIGesture.None && gestureControllerToMonitor.VRUIGesture != lastGesture)
        {
            lastGesture = gestureControllerToMonitor.VRUIGesture;
            return true;
        }
        else
        {
            lastGesture = gestureControllerToMonitor.VRUIGesture;
            return false;
        }
    }

    public float MinValue
    {
        get { return minValue; }
        set { minValue = value; }
    }

    public float MaxValue
    {
        get { return maxValue; }
        set { maxValue = value; }
    }

    public float StartValue
    {
        get { return startValue; }
        set
        {
            if (MinValue < MaxValue)
            {
                if (value < MinValue)
                {
                    startValue = MinValue;
                }
                else if (value > MaxValue)
                {
                    startValue = MaxValue;
                }
                else
                {
                    startValue = value;
                }
            }
            else
            {
                if (value > MinValue)
                {
                    startValue = MinValue;
                }
                else if (value < MaxValue)
                {
                    startValue = MaxValue;
                }
                else
                {
                    startValue = value;
                }
            }
        }
    }

    public float LengthOfPath
    {
        get { return lengthOfPath; }
        set { lengthOfPath = Mathf.Abs(value); }
    }

    public float WidthOfPath
    {
        get { return widthOfPath; }
        set { widthOfPath = Mathf.Abs(value); }
    }

    public float CurrentValue
    {
        get { return currentValue; }
        set { currentValue = value; }
    }

    public float LockoutTime
    {
        get { return lockoutTime; }
        set { lockoutTime = value; }
    }

    public Material PathMaterial
    {
        get { return pathMaterial; }
        set { pathMaterial = value; }
    }

    public VRUIVibration VibrationBehaviour
    {
        get { return vibrationBehaviour; }
        set { vibrationBehaviour = value; }
    }

    public GameObject Path
    {
        get { return path; }
        set { path = value; }
    }

    public GameObject PhysicalKnob
    {
        get { return physicalKnob; }
        set { physicalKnob = value; }
    }

    public VRUIGestureController LastRegisteredGestureController
    {
        get { return gestureController; }
    }
}
