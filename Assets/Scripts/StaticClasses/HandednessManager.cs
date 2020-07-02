using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HandednessManager
{
    public static OVRHand.Hand ActiveHandedness = OVRHand.Hand.HandRight;

    public static void SetHandedness(int handednessId)
    {
        ActiveHandedness = (OVRHand.Hand)handednessId; 
    }
}
