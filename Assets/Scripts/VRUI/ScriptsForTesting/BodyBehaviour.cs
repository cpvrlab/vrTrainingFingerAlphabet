using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyBehaviour : MonoBehaviour
{
    public Transform headToFollow;

    public float distanceToHead = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(headToFollow.position.x, headToFollow.position.y - distanceToHead, headToFollow.position.z);
    }
}
