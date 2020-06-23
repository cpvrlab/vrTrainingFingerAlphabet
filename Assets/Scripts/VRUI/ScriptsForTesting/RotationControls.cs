using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationControls : MonoBehaviour
{
    public float xRotation;
    public float yRotation;
    public float zRotation;

    public VRUISliderBehaviour xSlider;
    public VRUISliderBehaviour ySlider;

    private Vector3 rotation;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        rotation = new Vector3(xRotation, yRotation, zRotation);
        transform.localEulerAngles = rotation;
    }

    public void SetYRotationWithSlider()
    {
        yRotation = ySlider.CurrentValue;
    }

    public void SetXRotationWithSlider()
    {
        xRotation = xSlider.CurrentValue;
    }
}
