/********************************************************************************//*
Created as part of a Bsc in Computer Science for the BFH Biel
Created by:   Steven Henz
Date:         26.05.20
Email:        steven.henz93@gmail.com
************************************************************************************/
using UnityEngine;

/// <summary>
/// Demonstrates how the VRUIGestureController class can be implemented to recognize gestures. Specifically made for the Oculus Quest for both controllers
/// and handtracking.
/// </summary>
public class VRUIOVRHandTrackingGestureController : VRUIGestureController
{
    private OVRHand ovrHand;

    private bool handTracking;

    public bool deactivateThumbColliderOnPointing;
    public Collider thumbCollider;

    public bool deactivateGrabColliderOnPointing;
    public Collider grabCollider;

    public float minPinchStrength;
    public float maxPinchStrengthForPointing;
    public Collider[] collidersToDeactivateOnPinch;
    public Collider[] collidersToDeactivateOnPointing;

    // Start is called before the first frame update
    void Start()
    {
        handTracking = GetComponent<OVRCustomSkeleton>();
        ovrHand = GetComponent<OVRHand>();
    }

    // Update is called once per frame
    void Update()
    {
        //Handtracking gesture recognition
        if (handTracking)
        {
            OVRHand.TrackingConfidence indexFingerPointingConfidence = ovrHand.GetFingerConfidence(OVRHand.HandFinger.Index);
            float pinchStrength = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
            //Pinch Gesture
            if (pinchStrength >= minPinchStrength)
            {
                VRUIGesture = VRUIGesture.Pinch;
                if (deactivateThumbColliderOnPointing)
                {
                    thumbCollider.enabled = true;
                }
                if (deactivateGrabColliderOnPointing)
                {
                    grabCollider.enabled = true;
                }
                if (collidersToDeactivateOnPinch.Length > 0)
                {
                    foreach (Collider collider in collidersToDeactivateOnPinch)
                    {
                        collider.enabled = false;
                    }
                }
                if (collidersToDeactivateOnPointing.Length > 0)
                {
                    foreach (Collider collider in collidersToDeactivateOnPointing)
                    {
                        collider.enabled = true;
                    }
                }
            }
            //Pointing Gesture
            else if (indexFingerPointingConfidence == OVRHand.TrackingConfidence.High && pinchStrength < maxPinchStrengthForPointing)
            {
                VRUIGesture = VRUIGesture.IndexPointing;
                if (deactivateThumbColliderOnPointing)
                {
                    thumbCollider.enabled = false;
                }
                if (deactivateGrabColliderOnPointing)
                {
                    grabCollider.enabled = false;
                }
                if (collidersToDeactivateOnPinch.Length > 0)
                {
                    foreach (Collider collider in collidersToDeactivateOnPinch)
                    {
                        collider.enabled = true;
                    }
                }
                if (collidersToDeactivateOnPointing.Length > 0)
                {
                    foreach (Collider collider in collidersToDeactivateOnPointing)
                    {
                        collider.enabled = false;
                    }
                }
            }
            else
            {
                VRUIGesture = VRUIGesture.None;
                if (deactivateThumbColliderOnPointing)
                {
                    thumbCollider.enabled = true;
                }
                if (deactivateGrabColliderOnPointing)
                {
                    grabCollider.enabled = true;
                }
                if (collidersToDeactivateOnPinch.Length > 0)
                {
                    foreach (Collider collider in collidersToDeactivateOnPinch)
                    {
                        collider.enabled = true;
                    }
                }
                if (collidersToDeactivateOnPointing.Length > 0)
                {
                    foreach (Collider collider in collidersToDeactivateOnPointing)
                    {
                        collider.enabled = true;
                    }
                }
            }
        }
        //Controller gesture recognition
        else
        {
            //Left hand
            if (Hand == HandSide.left)
            {
                if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) >= minPinchStrength)
                {
                    VRUIGesture = VRUIGesture.Pinch;
                    if (deactivateThumbColliderOnPointing)
                    {
                        thumbCollider.enabled = true;
                    }
                    if (deactivateGrabColliderOnPointing)
                    {
                        grabCollider.enabled = true;
                    }
                    if (collidersToDeactivateOnPinch.Length > 0)
                    {
                        foreach (Collider collider in collidersToDeactivateOnPinch)
                        {
                            collider.enabled = false;
                        }
                    }
                    if (collidersToDeactivateOnPointing.Length > 0)
                    {
                        foreach (Collider collider in collidersToDeactivateOnPointing)
                        {
                            collider.enabled = true;
                        }
                    }
                }
                else if (!OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger))
                {
                    VRUIGesture = VRUIGesture.IndexPointing;
                    if (deactivateThumbColliderOnPointing)
                    {
                        thumbCollider.enabled = false;
                    }
                    if (deactivateGrabColliderOnPointing)
                    {
                        grabCollider.enabled = false;
                    }
                    if (collidersToDeactivateOnPinch.Length > 0)
                    {
                        foreach (Collider collider in collidersToDeactivateOnPinch)
                        {
                            collider.enabled = true;
                        }
                    }
                    if (collidersToDeactivateOnPointing.Length > 0)
                    {
                        foreach (Collider collider in collidersToDeactivateOnPointing)
                        {
                            collider.enabled = false;
                        }
                    }
                }
                else
                {
                    VRUIGesture = VRUIGesture.None;
                    if (deactivateThumbColliderOnPointing)
                    {
                        thumbCollider.enabled = true;
                    }
                    if (deactivateGrabColliderOnPointing)
                    {
                        grabCollider.enabled = true;
                    }
                    if (collidersToDeactivateOnPinch.Length > 0)
                    {
                        foreach (Collider collider in collidersToDeactivateOnPinch)
                        {
                            collider.enabled = true;
                        }
                    }
                    if (collidersToDeactivateOnPointing.Length > 0)
                    {
                        foreach (Collider collider in collidersToDeactivateOnPointing)
                        {
                            collider.enabled = true;
                        }
                    }
                }
            }
            //Right hand
            else
            {
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) >= minPinchStrength)
                {
                    VRUIGesture = VRUIGesture.Pinch;
                    if (deactivateThumbColliderOnPointing)
                    {
                        thumbCollider.enabled = true;
                    }
                    if (deactivateGrabColliderOnPointing)
                    {
                        grabCollider.enabled = true;
                    }
                    if (collidersToDeactivateOnPinch.Length > 0)
                    {
                        foreach (Collider collider in collidersToDeactivateOnPinch)
                        {
                            collider.enabled = false;
                        }
                    }
                    if (collidersToDeactivateOnPointing.Length > 0)
                    {
                        foreach (Collider collider in collidersToDeactivateOnPointing)
                        {
                            collider.enabled = true;
                        }
                    }
                }
                else if (!OVRInput.Get(OVRInput.Touch.SecondaryIndexTrigger))
                {
                    VRUIGesture = VRUIGesture.IndexPointing;
                    if (deactivateThumbColliderOnPointing)
                    {
                        thumbCollider.enabled = false;
                    }
                    if (deactivateGrabColliderOnPointing)
                    {
                        grabCollider.enabled = false;
                    }
                    if (collidersToDeactivateOnPinch.Length > 0)
                    {
                        foreach (Collider collider in collidersToDeactivateOnPinch)
                        {
                            collider.enabled = true;
                        }
                    }
                    if (collidersToDeactivateOnPointing.Length > 0)
                    {
                        foreach (Collider collider in collidersToDeactivateOnPointing)
                        {
                            collider.enabled = false;
                        }
                    }
                }
                else
                {
                    VRUIGesture = VRUIGesture.None;
                    if (deactivateThumbColliderOnPointing)
                    {
                        thumbCollider.enabled = true;
                    }
                    if (deactivateGrabColliderOnPointing)
                    {
                        grabCollider.enabled = true;
                    }
                    if (collidersToDeactivateOnPinch.Length > 0)
                    {
                        foreach (Collider collider in collidersToDeactivateOnPinch)
                        {
                            collider.enabled = true;
                        }
                    }
                    if (collidersToDeactivateOnPointing.Length > 0)
                    {
                        foreach (Collider collider in collidersToDeactivateOnPointing)
                        {
                            collider.enabled = true;
                        }
                    }
                }
            }
        }
    }
}