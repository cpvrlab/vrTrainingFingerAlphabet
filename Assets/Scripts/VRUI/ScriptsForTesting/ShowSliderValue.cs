using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSliderValue : MonoBehaviour
{
    public VRUITextcontainerBehaviour textcontainerBehaviour;
    public VRUISliderBehaviour sliderBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        textcontainerBehaviour.ChangeTextTo("" + sliderBehaviour.CurrentValue.ToString("0"));
    }
}
