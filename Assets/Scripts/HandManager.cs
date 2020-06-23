using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private static char[] ALPHABET = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'S', 'T', 'U', 'V', 'W', 'X', 'Y' };
   
    private char _CurrentLetter;
    private bool _HandFormsLoaded;
    private float _PreviousFalseTime;
    private float _CurrentTime;
    private HandForm _ObjectiveHandForm;
    private HandFormValidator _Validator;

    #endregion

    #region UnityFunctions
    private void Awake()
    {
        _HandFormsLoaded = false;
        _CurrentLetter = GetRandomAlphabetLetter();
        _Validator = GetComponent<HandFormValidator>();
        _Validator.VirtualHand = VirtualHand;
        GetNewLetter();
        StartCoroutine(AfterLoaded());
    }

    void Update()
    {
        // Checks if Handforms are ready to be validated
        if(_HandFormsLoaded)
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
    private IEnumerator AfterLoaded()
    {
        while (!HandData.IsDataLoaded())
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.1f);

        LoadNewHand();
        _HandFormsLoaded = true;

    }

    /// <summary>
    /// Loads a new random HandForm and updates the Alphabetscreen
    /// </summary>
    public void LoadNewHand()
    {
        char alphabetChar = GetNewLetter();

        _ObjectiveHandForm = HandData.GetHandForm(alphabetChar);

        VirtualHand.DisplayHandForm(_ObjectiveHandForm);
    }



    #endregion

    #region AlphabetLetterFunctions
    /// <summary>
    /// Gets random letter form the array Alphabet
    /// </summary>
    /// <returns>A random letter from the array Alphabet</returns>
    private char GetRandomAlphabetLetter()
    {
        return ALPHABET[Random.Range(0, ALPHABET.Length)];
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
    #endregion


}
