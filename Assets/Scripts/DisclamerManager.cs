using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.IO;
using UnityEditor;
using Newtonsoft.Json;

public class DisclamerManager : MonoBehaviour
{
    public GameObject[] DisclamerGameObjects;
    public GameObject[] SelectLanguage;
    public GameObject[] StandardGameObjects;

    public VRUIToggleBehaviour ToggleButton;
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Awake");
        LanguageManager.Init();

        if (PlayerPrefs.GetInt("Language", -1) == -1)
        {
            foreach (GameObject language in SelectLanguage)
                language.SetActive(true);

            foreach (GameObject standard in StandardGameObjects)
                standard.SetActive(false);
        }
        else
        {
            // if language is saved, set it to
            LanguageManager.language = (Language)PlayerPrefs.GetInt("Language", 0);
        }
        
    }

    public void ShowDisclamer()
    {

            foreach (GameObject language in SelectLanguage)
            language.SetActive(false);

            foreach (GameObject standard in StandardGameObjects)
            standard.SetActive(false);

            foreach (GameObject disclamer in DisclamerGameObjects)
                disclamer.SetActive(true);
    }

    public void ShowStartMenu()
    {
        foreach (GameObject disclamer in DisclamerGameObjects)
            disclamer.SetActive(false);

        foreach (GameObject standard in StandardGameObjects)
            standard.SetActive(true);
    }

    public void SetLanguage(int languageId)
    {
        PlayerPrefs.SetInt("Language", languageId);
        LanguageManager.language = (Language)languageId;
        ToggleButton.ToggleIsStuck = false;
    }

 
}
