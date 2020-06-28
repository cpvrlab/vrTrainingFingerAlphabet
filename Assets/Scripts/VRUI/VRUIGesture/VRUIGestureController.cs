/********************************************************************************//*
Created as part of a Bsc in Computer Science for the BFH Biel
Created by:   Steven Henz
Date:         26.05.20
Email:        steven.henz93@gmail.com
************************************************************************************/
using UnityEngine;

/// <summary>
/// The base class VRUIGestureController. Used in conjunction with the VRUIElements to make sure the correct gesture is used to interact.
/// Can be used as is for testing or be implemented to create your own gesture recognition
/// </summary>
public class VRUIGestureController : MonoBehaviour
{
    public enum HandSide
    {
        left,
        right
    }

    [SerializeField]
    [Tooltip("Which hand does this object represent?")]
    private HandSide hand;
    [SerializeField]
    [Tooltip("Which VRUIGesture does the object have at the moment?")]
    private VRUIGesture vruiGesture;

    public HandSide Hand
    {
        get { return hand; }
        set { hand = value; }
    }

    public VRUIGesture VRUIGesture
    {
        get { return vruiGesture; }
        set { vruiGesture = value; }
    }
}
