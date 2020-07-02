using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Manages the current asked Alphabet Letter for the User
/// 
/// Author: Cédric Girardin
/// </summary>
public class HandManager : MonoBehaviour
{
    #region Properties

    public Text CurrentLetterText;
    public HandLoader HandData;
    public HandFormRenderer VirtualHand;
    public float SecondsUnitlCorrect = 1f;

    // All possible Alphabet letters
    // private static char[] ALPHABET = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'S', 'T', 'U', 'V', 'W', 'X', 'Y' };
   
    private char _CurrentLetter;
    private bool _HandFormsLoaded;
    private float _PreviousFalseTime;
    private float _CurrentTime;
    private HandForm _ObjectiveHandForm;
    private HandFormValidator _Validator;


    #endregion

    #region UnityFunctions
    private void Start()
    {
        _HandFormsLoaded = false;
        // _CurrentLetter = GetRandomAlphabetLetter();
        _Validator = GetComponent<HandFormValidator>();
        _Validator.VirtualHand = VirtualHand;

        UnityEvent loadedEvent = new UnityEvent();
        loadedEvent.AddListener(LoadFirstHand);

        HandData.LoadData(loadedEvent);

    }

    void Update()
    {
        // Checks if Handforms are ready to be validated
        if (_HandFormsLoaded)
        {


            // Update _CurrentTime
            _CurrentTime += Time.deltaTime;

            // Checks if the HandForm is Wrong
            if(!_Validator.IsHandCorrect(_ObjectiveHandForm))
            {
                _PreviousFalseTime = _CurrentTime;
            }

            // When the HandForm was right for the SecondsUnitlCorrect 
            if(_CurrentTime - _PreviousFalseTime > SecondsUnitlCorrect)
            {
                LoadNewHand();
                _PreviousFalseTime = _CurrentTime;
            }
        }

    }
    #endregion

    #region HandFormMangamentFunctions

    // TODO find other solution (callback for example)!

    /// <summary>
    /// Loads a new random HandForm and updates the Alphabetscreen
    /// </summary>
    /// 
    public void LoadNewHand()
    {
        Debug.Log("loaded");
        char alphabetChar = GetNewLetter();

        _ObjectiveHandForm = HandData.GetHandForm(alphabetChar);
        UpdateVirtualHandPos();
    }

    public void LoadFirstHand()
    {
        Debug.Log("loaded");
        char alphabetChar = GetNewLetter();

        _ObjectiveHandForm = HandData.GetHandForm(alphabetChar);

        Invoke("UpdateVirtualHandPos", 0.2f);
        
    }

    private void UpdateVirtualHandPos()
    {

        Debug.Log("invoked");
        VirtualHand.DisplayHandForm(_ObjectiveHandForm);
        VirtualHand.UpdateVirtualHandPosition(_ObjectiveHandForm.BoneGO.transform);
        _HandFormsLoaded = true;
    }



    #endregion

    #region AlphabetLetterFunctions
    /// <summary>
    /// Gets random letter form the array Alphabet
    /// </summary>
    /// <returns>A random letter from the array Alphabet</returns>
    private char GetRandomAlphabetLetter()
    {
        Debug.Log(HandData.ALPHABET.Length);
        return HandData.ALPHABET[Random.Range(0, HandData.ALPHABET.Length)];
    }

    /// <summary>
    /// Displays a new letter on the AlphabetScreen
    /// </summary>
    /// <returns>The new letter that is displayed on the AlphabetScreen</returns>
    private char GetNewLetter()
    {
        char rndLetter = GetRandomAlphabetLetter();
        if (_CurrentLetter == rndLetter)
        {
            return GetNewLetter();
        }
        _CurrentLetter = rndLetter;
        CurrentLetterText.text = _CurrentLetter.ToString();

        return _CurrentLetter;
    }

    private void GenerateAlphabet()
    {

    }
    #endregion


}
