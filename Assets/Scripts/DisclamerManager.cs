using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisclamerManager : MonoBehaviour
{
    public GameObject[] DisclamerGameObjects;
    public GameObject[] StandardGameObjects;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("FIRSTTIMEOPENING", 1) == 1)
        {

            //Set first time opening to false
            PlayerPrefs.SetInt("FIRSTTIMEOPENING", 0);

            //Do your stuff here
            foreach (GameObject disclamer in DisclamerGameObjects)
                disclamer.SetActive(true);

            foreach (GameObject standard in StandardGameObjects)
                standard.SetActive(false);
        }
        
    }

 
}
