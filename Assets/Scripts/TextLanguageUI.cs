using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(TMP_Text))]
public class TextLanguageUI : MonoBehaviour
{

    public string key;

    private TMP_Text _Label;
    
    // Start is called before the first frame update
    void Start()
    {
        _Label = GetComponent<TMP_Text>();

        _Label.text = LanguageManager.GetLocalisedValue(key);
    }

    private void OnEnable()
    {
        _Label.text = LanguageManager.GetLocalisedValue(key);
    }
}
