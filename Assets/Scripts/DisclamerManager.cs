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
    public GameObject[] HandednessGameObjects;

    public VRUIToggleBehaviour ToggleButton;

    private AudioSource clickAudio;
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("Awake");
        LanguageManager.Init();

        // PlayerPrefs.SetInt("Language", -1);

        if (PlayerPrefs.GetInt("Language", -1) == -1)
        {
            // show Languages
            foreach (GameObject language in SelectLanguage)
                language.SetActive(true);

            // hide Startmenu
            foreach (GameObject standard in StandardGameObjects)
                standard.SetActive(false);
        }
        else
        {
            // if language is saved, set it to
            LanguageManager.language = (Language)PlayerPrefs.GetInt("Language", 0);
        }

        clickAudio = GetComponent<AudioSource>();


    }

    public void ShowDisclamer()
    {
        // hide Languages
        foreach (GameObject language in SelectLanguage)
            language.SetActive(false);

        // hide Startmenu
        foreach (GameObject standard in StandardGameObjects)
            standard.SetActive(false);

        // show Disclamer
        foreach (GameObject disclamer in DisclamerGameObjects)
            disclamer.SetActive(true);
    }

    public void ShowHandedness()
    {
        clickAudio.Play(0);

        // hide Disclamer
        foreach (GameObject disclamer in DisclamerGameObjects)
            disclamer.SetActive(false);

        // show Handeness
        foreach(GameObject handednessGO in HandednessGameObjects)
            handednessGO.SetActive(true);
    }

    public void ShowStartMenu()
    {
        // hide Handeness
        foreach (GameObject handednessGO in HandednessGameObjects)
            handednessGO.SetActive(false);

        // show Startmenu
        foreach (GameObject standard in StandardGameObjects)
            standard.SetActive(true);
    }

    public void SetLanguage(int languageId)
    {
        PlayerPrefs.SetInt("Language", languageId);
        LanguageManager.language = (Language)languageId;
        ToggleButton.ToggleIsStuck = false;

        LearnManger.IsInitalized = false;

        clickAudio.Play(0);

    }

    public void SetHandedness(int handednessId)
    {
        PlayerPrefs.SetInt("Handedness", handednessId);
        HandednessManager.SetHandedness(handednessId);

        clickAudio.Play(0);
    }


 
}
