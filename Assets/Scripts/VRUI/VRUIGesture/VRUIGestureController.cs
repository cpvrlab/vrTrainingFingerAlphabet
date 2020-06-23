using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
