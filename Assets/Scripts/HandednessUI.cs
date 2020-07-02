using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VRUIPanelBehaviour))]
public class HandednessUI : MonoBehaviour
{
    public OVRHand.Hand UIForHand;
    
    private void Awake()
    {
        if (UIForHand != HandednessManager.ActiveHandedness)
            this.gameObject.SetActive(false);
    }
}
