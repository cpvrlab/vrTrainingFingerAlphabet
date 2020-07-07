using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudColorManager : MonoBehaviour
{
    public Color color;
    // Start is called before the first frame update
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();

        Debug.Log(rend);
        Debug.Log(rend.material.GetColor("Color_4089CE7A"));

        rend.material.SetColor("Color_4089CE7A", color);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
