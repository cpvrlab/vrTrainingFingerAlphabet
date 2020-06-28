using UnityEngine;

public class ConsoleOutputBehaviourTest : MonoBehaviour
{
    public VRUISliderBehaviour slider;

    public void OnButtonDown()
    {
        Debug.Log("OnButtonDown event");
    }

    public void OnButton()
    {
        Debug.Log("OnButton event");
    }

    public void OnButtonUp()
    {
        Debug.Log("OnButtonUp event");
    }

    public void OnValueChanged()
    {
        Debug.Log("OnValueChanged event. New Value = " + slider.CurrentValue);
    }

    public void OnToggleStuck()
    {
        Debug.Log("OnToggleStuck event");
    }
}
