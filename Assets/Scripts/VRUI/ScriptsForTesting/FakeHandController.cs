using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeHandController : MonoBehaviour
{
    public float speed = 1.0f;
    public bool keyboardControlsOn = false;

    private Vector3 startPosition;
    private bool resetPosition;
    private float moveHorizontal;
    private float moveVertical;
    private float moveUpDown;
    private bool fasterMovement;

    private VRUIGestureController gestureController;

    // Start is called before the first frame update
    void Start()
    {
        gestureController = GetComponent<VRUIGestureController>();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (keyboardControlsOn)
        {
            resetPosition = Input.GetKey("r");
            fasterMovement = Input.GetKey("left shift");
            moveHorizontal = Input.GetAxis("Horizontal");
            moveVertical = Input.GetAxis("Vertical");
            if (Input.GetKey("q"))
                moveUpDown = 1f;
            else if (Input.GetKey("e"))
                moveUpDown = -1f;
            else
                moveUpDown = 0f;
            if (Input.GetKeyDown("f"))
            {
                if (gestureController.VRUIGesture != VRUIGesture.None)
                {
                    gestureController.VRUIGesture += 1;
                }
                else
                {
                    gestureController.VRUIGesture = 0;
                }
                Debug.Log("Active VRUIGesture = " + gestureController.VRUIGesture);
            }
            if (Input.GetKeyDown("1"))
            {
                gestureController.VRUIGesture = 0;
                Debug.Log("Active VRUIGesture = " + gestureController.VRUIGesture);
            }
            else if (Input.GetKeyDown("2"))
            {
                gestureController.VRUIGesture = (VRUIGesture)1;
                Debug.Log("Active VRUIGesture = " + gestureController.VRUIGesture);
            }
            else if (Input.GetKeyDown("3"))
            {
                gestureController.VRUIGesture = (VRUIGesture)2;
                Debug.Log("Active VRUIGesture = " + gestureController.VRUIGesture);
            }
            else if (Input.GetKeyDown("4"))
            {
                gestureController.VRUIGesture = (VRUIGesture)3;
                Debug.Log("Active VRUIGesture = " + gestureController.VRUIGesture);
            }
        }
    }

    void FixedUpdate()
    {
        if (keyboardControlsOn && !resetPosition)
        {
            Vector3 movement = new Vector3(moveHorizontal, moveUpDown, moveVertical);
            movement = movement * speed * Time.deltaTime;
            if (fasterMovement)
                movement *= 2;
            transform.position += movement;
        }
        if (resetPosition)
            transform.position = startPosition;
    }
}
