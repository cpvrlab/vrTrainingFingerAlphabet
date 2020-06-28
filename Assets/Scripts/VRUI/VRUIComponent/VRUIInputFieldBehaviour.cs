/********************************************************************************//*
Created as part of a Bsc in Computer Science for the BFH Biel
Created by:   Steven Henz
Date:         26.05.20
Email:        steven.henz93@gmail.com
************************************************************************************/
using UnityEngine;
using TMPro;

/// <summary>
/// Unfinished. Will be used to input text.
/// </summary>
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
