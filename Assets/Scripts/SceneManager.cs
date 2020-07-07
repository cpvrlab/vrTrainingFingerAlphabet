using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the Scenes
/// 
/// Author: Cédric Girardin
/// </summary>
public class SceneManager : MonoBehaviour
{

    AudioSource audioData;

    private void Start()
    {
        audioData = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Loads a new Scene
    /// </summary>
    /// <param name="SceneName">name of the Scene</param>
    public void OnButtonDownLoadScene(string SceneName)
    {
        audioData.Play(0);
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
    }

}
