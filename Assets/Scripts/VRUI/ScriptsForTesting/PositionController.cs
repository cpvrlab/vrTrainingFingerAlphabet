using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionController : MonoBehaviour
{
    public float xPos;
    public float zPos;

    public VRUISliderBehaviour xSlider;
    public VRUISliderBehaviour zSlider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        xPos = xSlider.CurrentValue;
        zPos = zSlider.CurrentValue;

        Vector3 position = transform.position;
        position.x = xPos;
        position.z = zPos;

        transform.position = position;
    }
}
