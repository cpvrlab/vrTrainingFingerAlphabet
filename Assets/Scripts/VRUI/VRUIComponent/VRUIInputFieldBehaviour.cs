using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VRUIInputFieldBehaviour : MonoBehaviour
{
    public TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("h"))
            inputField.ActivateInputField();
    }

    public void ChangeTextTo(string text)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        inputField.ActivateInputField();
    }
}
