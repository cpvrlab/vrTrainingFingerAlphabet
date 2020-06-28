/********************************************************************************//*
Created as part of a Bsc in Computer Science for the BFH Biel
Created by:   Steven Henz
Date:         26.05.20
Email:        steven.henz93@gmail.com
************************************************************************************/
using UnityEngine;

/// <summary>
/// This abstract class can be implemented to control the controller vibration in conjunction with the VRUI components.
/// </summary>
public abstract class VRUIVibration : MonoBehaviour
{
    [SerializeField]
    private float vibrationFrequency;
    [SerializeField]
    private float vibrationAmplitude;
    [SerializeField]
    private float vibrationDuration;

    public abstract void Vibrate();

    public float VibrationFrequency
    {
        get { return vibrationFrequency; }
        set
        {
            if (value <= 0)
                vibrationFrequency = 0;
            else if (value >= 1)
                vibrationFrequency = 1;
            else
                vibrationFrequency = value;
        }
    }

    public float VibrationAmplitude
    {
        get { return vibrationAmplitude; }
        set
        {
            if (value <= 0)
                vibrationAmplitude = 0;
            else if (value >= 1)
                vibrationAmplitude = 1;
            else
                vibrationAmplitude = value;
        }
    }

    public float VibrationDuration
    {
        get { return vibrationDuration; }
        set { vibrationDuration = value; }
    }
}
