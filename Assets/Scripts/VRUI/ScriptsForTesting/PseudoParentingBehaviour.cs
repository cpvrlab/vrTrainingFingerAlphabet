using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PseudoParentingBehaviour : MonoBehaviour
{
    public Transform pseudoChild;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 deltaPosition;
    private Quaternion deltaRotation;
    private Vector3 childPositionStart;
    private Quaternion childRotationStart;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        deltaPosition = transform.position - startPosition;
        deltaRotation = transform.rotation * Quaternion.Inverse(startRotation);
        childPositionStart = pseudoChild.transform.position;
        childRotationStart = pseudoChild.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        deltaPosition = transform.position - startPosition;
        deltaRotation = transform.rotation * Quaternion.Inverse(startRotation);
        pseudoChild.transform.position = childPositionStart + deltaPosition;
        pseudoChild.rotation = childRotationStart * deltaRotation;
    }
}
